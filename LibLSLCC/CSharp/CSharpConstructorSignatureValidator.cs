#region FileInfo
// 
// File: CSharpConstructorSignatureValidator.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace LibLSLCC.CSharp
{
    public enum CSharpConstructorParameterForwarding
    {
        None,
        Base,
        This
    }

    public class CSharpConstructorSignatureValidationResult
    {
        public bool Success { get; internal set; }
        public string ErrorDescription { get; internal set; }

        public int ErrorIndex { get; internal set; }

        public CSharpClassNameValidationResult[] ParameterTypes { get; internal set; }
        public string[] ParameterNames { get; internal set; }
        public CSharpConstructorParameterForwarding ParameterForwarding { get; internal set; }
        public string[] ForwardedParameters { get; internal set; }
    }

    public static class CSharpConstructorSignatureValidator
    {
        private enum States
        {
            Start,
            WaitingForParamType,
            AfterForwardingKeyword,
            AccumulatingForwardedParam,
            EndOfBasicSignature,
            WaitingForForwardingKeyword,
            AccumulatingParamType,
            AccumulatingGenericType,
            AccumulatingParamName,
            EndOfForwardingSignature,
            WaitingForParameterName,
            AccumulatingForwardingKeyword,
            WaitingForForwardedParam
        }


        public static CSharpConstructorSignatureValidationResult Validate(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input", "Constructor signature string cannot be null!");
            }


            var result = new CSharpConstructorSignatureValidationResult
            {
                Success = true,
                ParameterForwarding = CSharpConstructorParameterForwarding.None

            };



            if (string.IsNullOrWhiteSpace(input))
            {
                result.Success = false;
                result.ErrorDescription = "Constructor signature cannot be whitespace.";
                return result;
            }

            States state = 0;
            string accum = "";

            var parameterTypes = new List<CSharpClassNameValidationResult>();
            var parameterNames = new HashSet<string>();
            var forwardedParameters = new List<string>();

            int unclosedGenericsBrackets = 0;

            //weeeeeeeeeeeeeeee!
            for (int index = 0; index < input.Length; index++)
            {
                var c = input[index];
                accum += c;

                if (c == '(')
                {
                    switch (state)
                    {
                        case States.Start:
                            state = States.WaitingForParamType;
                            accum = "";
                            continue;
                        case States.AccumulatingForwardingKeyword:
                            switch (accum)
                            {
                                case "base(":
                                    result.ParameterForwarding = CSharpConstructorParameterForwarding.Base;
                                    break;
                                case "this(":
                                    result.ParameterForwarding = CSharpConstructorParameterForwarding.This;
                                    break;
                                default:
                                    result.Success = false;
                                    result.ErrorDescription = "Constructor forwarding keyword must be 'base' or 'this'.";
                                    result.ErrorIndex = index - 5;
                                    return result;
                            }
                            accum = "";
                            state = States.WaitingForForwardedParam;
                            continue;
                        case States.AfterForwardingKeyword:
                            state = States.WaitingForForwardedParam;
                            accum = "";
                            continue;
                        default:
                            result.Success = false;
                            result.ErrorDescription = "Unexpected opening parenthesis.";
                            result.ErrorIndex = index;
                            return result;
                    }
                }

                if (c == ':')
                {
                    if (state == States.EndOfBasicSignature)
                    {
                        accum = "";
                        state = States.WaitingForForwardingKeyword;
                        continue;
                    }

                    result.Success = false;
                    result.ErrorDescription = "Unexpected ':' character.";
                    result.ErrorIndex = index;
                    return result;
                }


                if (c == '<')
                {
                    if (state == States.AccumulatingParamType)
                    {
                        unclosedGenericsBrackets++;
                        state = States.AccumulatingGenericType;
                        continue;
                    }
                    if (state == States.AccumulatingGenericType)
                    {
                        unclosedGenericsBrackets++;
                        continue;
                    }
                    result.Success = false;
                    result.ErrorDescription = "Unexpected '<' character.";
                    result.ErrorIndex = index;
                    return result;
                }

                if (c == '>')
                {
                    if (state == States.AccumulatingGenericType)
                    {
                        unclosedGenericsBrackets--;
                        if (unclosedGenericsBrackets == 0)
                        {
                            state = States.AccumulatingParamType;
                        }
                        continue;
                    }
                    result.Success = false;
                    result.ErrorDescription = "Unexpected '>' character.";
                    result.ErrorIndex = index;
                    return result;
                }

                if (c == ',' || c == ')')
                {
                    switch (state)
                    {
                        case States.WaitingForParamType:
                            if (parameterTypes.Count > 0 || (parameterTypes.Count==0 && c== ','))
                            {
                                result.Success = false;
                                result.ErrorDescription = "Missing parameter declaration.";
                                result.ErrorIndex = index - 1;
                                return result;
                            }

                            accum = "";
                            state = States.EndOfBasicSignature;
                            continue;

                        case States.WaitingForForwardedParam:
                            if (forwardedParameters.Count > 0 || (forwardedParameters.Count == 0 && c == ','))
                            {
                                result.Success = false;
                                result.ErrorDescription = "Missing forwarded argument declaration.";
                                result.ErrorIndex = index;
                                return result;
                            }

                            accum = "";
                            state = States.EndOfForwardingSignature;
                            continue;

                        case States.AccumulatingParamType:
                            result.Success = false;
                            result.ErrorDescription = "Missing parameter name.";
                            result.ErrorIndex = index;
                            return result;

                        case States.AccumulatingParamName:
                        {
                            var pname = accum.TrimEnd(',', ')').Trim();
                            if (!CSharpCompilerSingleton.Compiler.IsValidIdentifier(pname))
                            {
                                result.Success = false;
                                result.ErrorDescription = string.Format("Invalid parameter name '{0}'.", pname);
                                result.ErrorIndex = index - accum.Length;
                                return result;
                            }
                            if (!parameterNames.Contains(pname))
                            {
                                parameterNames.Add(pname);
                            }
                            else
                            {
                                result.Success = false;
                                result.ErrorDescription = string.Format("Parameter name '{0}' used more than once.", pname);
                                result.ErrorIndex = index - accum.Length;
                                return result;
                            }
                            accum = "";
                            state = c == ',' ? States.WaitingForParamType : States.EndOfBasicSignature;
                            continue;
                        }
                        case States.AccumulatingForwardedParam:
                        {
                            var pname = accum.TrimEnd(',', ')').Trim();

                            if (!CSharpCompilerSingleton.Compiler.IsValidIdentifier(pname))
                            {
                                result.Success = false;
                                result.ErrorDescription =
                                    string.Format(
                                        "Invalid forwarded parameter '{0}', invalid identifier syntax.", pname);
                                result.ErrorIndex = index - accum.Length;
                                return result;
                            }

                            if (!parameterNames.Contains(pname))
                            {
                                result.Success = false;
                                result.ErrorDescription =
                                    string.Format(
                                        "Invalid forwarded parameter '{0}', name was not previously declared.", pname);

                                result.ErrorIndex = index - accum.Length;
                                return result;
                            }

                            forwardedParameters.Add(pname);

                            accum = "";
                            state = c == ',' ? States.WaitingForForwardedParam : States.EndOfForwardingSignature;
                            continue;
                        }
                        default:
                            if (c == ',' && state == States.AccumulatingGenericType) continue;

                            result.Success = false;
                            result.ErrorDescription = string.Format("Unexpected '{0}' character.", c);
                            result.ErrorIndex = index;
                            return result;
                    }
                }

                if (!char.IsWhiteSpace(c))
                {
                    switch (state)
                    {
                        case States.EndOfBasicSignature:
                        case States.EndOfForwardingSignature:
                            result.Success = false;
                            result.ErrorDescription = string.Format("Unexpected character '{0}' after signature.", c);
                            result.ErrorIndex = index;
                            return result;
                        case States.Start:
                            result.Success = false;
                            result.ErrorDescription = string.Format("Unexpected character '{0}', was expecting '('.", c);
                            result.ErrorIndex = index;
                            return result;
                        case States.WaitingForParamType:
                            state = States.AccumulatingParamType;
                            accum = "" + c;
                            continue;
                        case States.WaitingForParameterName:
                            state = States.AccumulatingParamName;
                            accum = "" + c;
                            continue;
                        case States.WaitingForForwardingKeyword:
                            state = States.AccumulatingForwardingKeyword;
                            accum = "" + c;
                            continue;
                        case States.WaitingForForwardedParam:
                            state = States.AccumulatingForwardedParam;
                            accum = "" + c;
                            continue;
                    }
                }

                if (char.IsWhiteSpace(c))
                {
                    switch (state)
                    {
                        case States.WaitingForParamType:
                        case States.WaitingForParameterName:
                        case States.EndOfBasicSignature:
                        case States.WaitingForForwardingKeyword:
                        case States.AfterForwardingKeyword:
                        case States.WaitingForForwardedParam:
                            accum = "";
                            continue;
                        case States.AccumulatingForwardingKeyword:
                            switch (accum)
                            {
                                case "base ":
                                    result.ParameterForwarding = CSharpConstructorParameterForwarding.Base;
                                    break;
                                case "this ":
                                    result.ParameterForwarding = CSharpConstructorParameterForwarding.This;
                                    break;
                                default:
                                    result.Success = false;
                                    result.ErrorDescription = "Constructor forwarding keyword must be 'base' or 'this'.";
                                    result.ErrorIndex = index - 5;
                                    return result;
                            }
                            accum = "";
                            state = States.AfterForwardingKeyword;
                            continue;
                        case States.AccumulatingParamType:
                            var ptype = accum.TrimEnd();
                            var validateParameter = CSharpClassNameValidator.ValidateInitialization(accum.TrimEnd(), true);
                            if (!validateParameter.Success)
                            {
                                result.Success = false;
                                result.ErrorDescription = string.Format("Invalid parameter type '{0}': {1}", ptype, validateParameter.ErrorDescription);
                                result.ErrorIndex = index - accum.Length;
                                return result;
                            }

                            parameterTypes.Add(validateParameter);
                            state = States.WaitingForParameterName;
                            accum = "";
                            continue;
                    }
                }
            }

            if (state != States.EndOfBasicSignature && state != States.EndOfForwardingSignature)
            {
                result.Success = false;
                result.ErrorDescription = "Incomplete constructor signature.";
                result.ErrorIndex = input.Length - 1;
                return result;
            }

            result.ParameterTypes = parameterTypes.ToArray();
            result.ParameterNames = parameterNames.ToArray();
            result.ForwardedParameters = forwardedParameters.ToArray();

            return result;
        }
    }
}