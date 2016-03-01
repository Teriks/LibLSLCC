#region FileInfo

// 
// File: CSharpFunctionCallValidator.cs
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

#region Imports

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.CSharp
{
    /// <summary>
    ///     Result object for <see cref="CSharpFunctionCallValidator.Validate" />
    /// </summary>
    public sealed class CSharpFunctionCallValidationResult
    {
        /// <summary>
        ///     <c>true</c> if the function call was succesfully parsed.
        /// </summary>
        public bool Success { get; internal set; }

        /// <summary>
        ///     An error description if <see cref="Success" /> is <c>false</c>
        /// </summary>
        public string ErrorDescription { get; internal set; }

        /// <summary>
        ///     The index in the parsed string at which an error was detected if <see cref="Success" /> is <c>false</c>.
        /// </summary>
        public int ErrorIndex { get; internal set; }

        /// <summary>
        ///     The explicit generic parameters specified if any.
        /// </summary>
        public IReadOnlyGenericArray<CSharpClassNameValidationResult> ExplicitGenericParameters { get; internal set; }

        /// <summary>
        ///     The parsed function call parameters.
        /// </summary>
        public IReadOnlyGenericArray<CSharpParameterSignature> Parameters { get; internal set; }

        /// <summary>
        ///     The parsed method name from the function call signature.
        /// </summary>
        public string MethodName { get; internal set; }
    }


    /// <summary>
    ///     Represents the possible parameter modifiers of a CSharp function call.
    /// </summary>
    public enum CSharpParameterModifier
    {
        /// <summary>
        ///     no modifiers
        /// </summary>
        None,

        /// <summary>
        ///     the out modifier.
        /// </summary>
        Out,

        /// <summary>
        ///     the pass by reference modifier.
        /// </summary>
        Ref,

        /// <summary>
        ///     this means that the parameter passed was a brand new object, which was created directly in the parameter slot.
        /// </summary>
        New
    }

    /// <summary>
    ///     Represents the signature of a parameter passed to a CSharp function method call.
    /// </summary>
    public sealed class CSharpParameterSignature
    {
        internal CSharpParameterSignature(string parameterText, CSharpParameterModifier modifier)
        {
            Modifier = modifier;
            ParameterText = parameterText;
        }


        /// <summary>
        ///     The modifier that appears in front of the parameter
        /// </summary>
        public CSharpParameterModifier Modifier { get; private set; }

        /// <summary>
        ///     The text that represents the parsed parameter, after the modifier if one is present.
        /// </summary>
        public string ParameterText { get; private set; }


        /// <summary>
        ///     Calculate a hash for the parameter signature using <see cref="ParameterText" /> and <see cref="Modifier" />
        /// </summary>
        /// <returns>The generated hash code.</returns>
        public override int GetHashCode()
        {
            return (ParameterText != null ? ParameterText.GetHashCode() : 0);
        }


        /// <summary>
        ///     Test equality using <see cref="ParameterText" /> and <see cref="Modifier" /> <para/>
        ///     If <paramref name="obj"/> is not a <see cref="CSharpParameterSignature"/> the result will be false.
        /// </summary>
        /// <param name="obj">The other <see cref="CSharpParameterSignature"/> to test for equality with.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is an <see cref="CSharpParameterSignature"/> that is equal to this object; otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var o = obj as CSharpParameterSignature;
            if (o == null) return false;

            return string.Equals(o.ParameterText, ParameterText);
        }
    }


    /// <summary>
    ///     Static class containing utilities for validating a CSharp method call signature.
    /// </summary>
    public static class CSharpFunctionCallValidator
    {
        private static bool IsValidPlainParameter(string paramText, int paramStartIndex,
            out string err, out int errIndex)
        {
            errIndex = paramStartIndex;


            //Check the syntax of a plain parameter expression with no new keyword by abusing the CodeDom compiler..

            //Not using Roslyn.Compilers since its not a namespace included with mono by default yet.
            //Could use NRefactory, but.. it's an added assembly on windows and not on mono, it comes with mono by default and may be some other version. 
            //DLL hell?..

            //this is slow as hell for UI usage scenarios, but does what I want without adding large dependencies.

            string testp = string.Format("class P{{void F<T>(T a){{}}P(){{F({0});}}}}", paramText);
            const int pstartIndex = 31;

            using (var compiler = CodeDomProvider.CreateProvider("CSharp"))
            {
                var cParams = new CompilerParameters(new string[] {})
                {
                    GenerateInMemory = true,
                    GenerateExecutable = false
                };

                var r = compiler.CompileAssemblyFromSource(cParams, testp);

                //all errors except references to undefined variable/method/type references, and also type inference failures
                //CS0103: is undefined variable references
                //CS0246: is undefined type references
                //CS0411: is type cannot be inferred, this happens if the parameter has a syntactically valid lambda expression passed to it

                var errs = r.Errors.Cast<CompilerError>().ToList();
                var relevantErrors =
                    errs.Where(
                        x => x.ErrorNumber != "CS0103" && x.ErrorNumber != "CS0246" && x.ErrorNumber != "CS0411")
                        .ToList();

                if (relevantErrors.Count > 0)
                {
                    err = relevantErrors[0].ErrorText;
                    errIndex = (relevantErrors[0].Column - pstartIndex) + paramStartIndex;
                    return false;
                }
            }

            err = null;
            return true;
        }


        private static bool IsValidNewParameter(string paramText, int paramStartIndex, out string err,
            out int errIndex)
        {
            errIndex = paramStartIndex;
            err = null;

            char startChar = paramText[0];

            if (CSharpIDValidator.IsValidIdentifier(paramText))
            {
                errIndex = paramStartIndex;
                err = "'new' keyword cannot be used with a variable reference.";
                return false;
            }

            if (char.IsDigit(startChar))
            {
                errIndex = paramStartIndex;
                err = "'new' keyword cannot be used on a numeric literal.";
                return false;
            }

            if (startChar == '"' || paramText.StartsWith("@\""))
            {
                errIndex = paramStartIndex;
                err = "'new' keyword cannot be used on a string literal.";
                return false;
            }

            if (startChar == '\'')
            {
                errIndex = paramStartIndex;
                err = "'new' keyword cannot be used on a character literal.";
                return false;
            }


            string testp = string.Format("class P{{void F(object a){{}}P(){{F(new {0});}}}}", paramText);
            const int pstartIndex = 36;

            using (var compiler = CodeDomProvider.CreateProvider("CSharp"))
            {
                var cParams = new CompilerParameters(new string[] {})
                {
                    GenerateInMemory = true,
                    GenerateExecutable = false
                };

                var r = compiler.CompileAssemblyFromSource(cParams, testp);

                //all errors except references to undefined variable/method/type references
                //CS0103: is undefined variable references
                //CS0246: is undefined type references

                var errs = r.Errors.Cast<CompilerError>().ToList();
                var relevantErrors =
                    errs.Where(
                        x => x.ErrorNumber != "CS0103" && x.ErrorNumber != "CS0246").ToList();
                if (relevantErrors.Count > 0)
                {
                    err = relevantErrors[0].ErrorText;
                    errIndex = (relevantErrors[0].Column - pstartIndex) + paramStartIndex;
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        ///     Parses and validates a string as a CSharp method call.
        /// </summary>
        /// <param name="signature">The method call signature, without a semi-colon at the end.</param>
        /// <returns>A parse/validation results object.  <see cref="CSharpFunctionCallValidationResult" /></returns>
        /// <exception cref="ArgumentNullException"><paramref name="signature"/> is <c>null</c>.</exception>
        public static CSharpFunctionCallValidationResult Validate(string signature)
        {
            if (signature == null) throw new ArgumentNullException("signature");

            var result = new CSharpFunctionCallValidationResult {Success = true};

            var explicitGenericParameters = new GenericArray<CSharpClassNameValidationResult>();
            var parameters = new GenericArray<CSharpParameterSignature>();

            var lastModifier = CSharpParameterModifier.None;

            var state = States.WaitingForFirstCharacter;

            var stateBeforeBrackets = new Stack<States>();
            var stateBeforeGenericTypePart = new Stack<States>();
            var stateBeforeParenthesis = new Stack<States>();
            var stateBeforeCurlyBraces = new Stack<States>();

            string accum = "";
            int unmatchedGenericBrackets = 0;
            int unmatchedParenthesis = 0;
            int unmatchedCurlyBraces = 0;
            int unmatchedBrackets = 0;
            string methodName = "";

            for (int index = 0; index < signature.Length; index++)
            {
                var c = signature[index];
                accum += c;


                if (c == 'o' || c == 'r' || c == 'n')
                {
                    int wordBoundry = index + 2;
                    int ahead = wordBoundry + 1;

                    if (wordBoundry >= signature.Length) goto afterRefOutChecks;
                    var lookAhead = signature.Substring(index, 3);
                    var lookAheadAssertion = lookAhead == "out" || lookAhead == "ref" || lookAhead == "new";
                    if (ahead < signature.Length && !char.IsWhiteSpace(signature[ahead])) goto afterRefOutChecks;

                    if (lookAheadAssertion && state == States.WaitingForParameterName &&
                        lastModifier == CSharpParameterModifier.None)
                    {
                        lastModifier = lookAhead == "out"
                            ? CSharpParameterModifier.Out
                            : (lookAhead == "ref" ? CSharpParameterModifier.Ref : CSharpParameterModifier.New);

                        accum = "";
                        index = wordBoundry;
                        state = States.WaitingForParameterName;
                        continue;
                    }
                }


                afterRefOutChecks:

                if (c == '[')
                {
                    if (state == States.AccumulatingParameter || state == States.WaitingForParameterName)
                    {
                        stateBeforeBrackets.Push(States.AccumulatingParameter);
                        state = States.InBracketExpression;
                        unmatchedBrackets++;
                        continue;
                    }


                    if (state == States.InBracketExpression || state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        unmatchedBrackets++;
                        continue;
                    }
                }

                if (c == ']')
                {
                    if (state == States.InBracketExpression || state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        unmatchedBrackets--;
                        if (unmatchedBrackets < 0)
                        {
                            result.Success = false;
                            result.ErrorDescription = "Unexpected closing bracket.";
                            result.ErrorIndex = index;
                            return result;
                        }
                        if (unmatchedBrackets == 0)
                        {
                            if (stateBeforeBrackets.Count > 0)
                            {
                                state = stateBeforeBrackets.Pop();
                            }
                            else if (state == States.InBracketExpression)
                            {
                                goto unexpectedBracket;
                            }
                        }
                        continue;

                        unexpectedBracket:

                        result.Success = false;
                        result.ErrorDescription = "Unexpected closing bracket.";
                        result.ErrorIndex = index;
                        return result;
                    }
                }


                if (c == '{')
                {
                    if (state == States.AccumulatingParameter || state == States.WaitingForParameterName)
                    {
                        stateBeforeCurlyBraces.Push(States.AccumulatingParameter);
                        state = States.InCurlyBraceExpression;
                        unmatchedCurlyBraces++;
                        continue;
                    }

                    if (state == States.InBracketExpression || state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        unmatchedCurlyBraces++;
                        continue;
                    }
                }

                if (c == '}')
                {
                    if (state == States.InBracketExpression || state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        unmatchedCurlyBraces--;
                        if (unmatchedCurlyBraces < 0)
                        {
                            result.Success = false;
                            result.ErrorDescription = "Unexpected closing brace.";
                            result.ErrorIndex = index;
                            return result;
                        }
                        if (unmatchedCurlyBraces == 0)
                        {
                            if (stateBeforeCurlyBraces.Count > 0)
                            {
                                state = stateBeforeCurlyBraces.Pop();
                            }
                            else if (state == States.InCurlyBraceExpression)
                            {
                                goto unexpectedCurlyBrace;
                            }
                        }

                        continue;
                    }

                    unexpectedCurlyBrace:

                    result.Success = false;
                    result.ErrorDescription = "Unexpected closing brace.";
                    result.ErrorIndex = index;
                    return result;
                }

                if (c == ')')
                {
                    if (state == States.WaitingForParameterName && parameters.Count == 0)
                    {
                        if (lastModifier == CSharpParameterModifier.None)
                        {
                            state = States.AfterSignature;
                            accum = "";
                        }
                        //otherwise the signature is incomplete
                        continue;
                    }

                    if (state == States.InBracketExpression || state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        unmatchedParenthesis--;
                        if (unmatchedParenthesis < 0)
                        {
                            result.Success = false;
                            result.ErrorDescription = "Unexpected closing parenthesis.";
                            result.ErrorIndex = index;
                            return result;
                        }
                        if (unmatchedParenthesis == 0)
                        {
                            if (stateBeforeParenthesis.Count > 0)
                            {
                                state = stateBeforeParenthesis.Pop();
                            }
                            else if (state == States.InParenthesizedExpression)
                            {
                                goto unexpectedParenth;
                            }
                        }
                        continue;
                    }
                    if (state == States.AccumulatingParameter)
                    {
                        var param = accum.Substring(0, accum.Length - 1).Trim();


                        if (!CSharpIDValidator.IsValidIdentifier(param) &&
                            (lastModifier == CSharpParameterModifier.Ref || lastModifier == CSharpParameterModifier.Out))
                        {
                            result.Success = false;
                            result.ErrorDescription =
                                "'ref' and 'out' can only be used with a direct variable reference.";
                            result.ErrorIndex = index - (accum.Length - 1);
                            return result;
                        }


                        string err;
                        int errIndex;

                        if (lastModifier == CSharpParameterModifier.New &&
                            !IsValidNewParameter(param, index - (accum.Length - 1), out err, out errIndex))
                        {
                            result.Success = false;
                            result.ErrorDescription = err;
                            result.ErrorIndex = errIndex;
                            return result;
                        }


                        if (lastModifier == CSharpParameterModifier.None &&
                            !IsValidPlainParameter(param, index - (accum.Length - 1), out err, out errIndex))
                        {
                            result.Success = false;
                            result.ErrorDescription = err;
                            result.ErrorIndex = errIndex;
                            return result;
                        }

                        parameters.Add(new CSharpParameterSignature(param, lastModifier));
                        accum = "";
                        lastModifier = CSharpParameterModifier.None;
                        state = States.AfterSignature;
                        continue;
                    }

                    unexpectedParenth:
                    result.Success = false;
                    result.ErrorDescription = "Unexpected closing parenthesis.";
                    result.ErrorIndex = index;
                    return result;
                }
                if (c == '(')
                {
                    if (state == States.AccumulatingParameter || state == States.WaitingForParameterName)
                    {
                        stateBeforeParenthesis.Push(States.AccumulatingParameter);
                        state = States.InParenthesizedExpression;
                        unmatchedParenthesis++;
                        continue;
                    }
                    if (state == States.AfterExplicitGenericMethodParameters)
                    {
                        accum = "";
                        state = States.WaitingForParameterName;
                        continue;
                    }
                    if (state == States.AccumulatingMethodNamePart)
                    {
                        methodName = accum.TrimEnd('(').Trim();
                        accum = "";
                        state = States.WaitingForParameterName;
                        continue;
                    }

                    if (state == States.InBracketExpression || state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        unmatchedParenthesis++;
                        continue;
                    }

                    result.Success = false;
                    result.ErrorDescription = "Unexpected opening parenthesis.";
                    result.ErrorIndex = index;
                    return result;
                }


                if (c == ',')
                {
                    if (state == States.InBracketExpression || state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        continue;
                    }

                    if (state == States.AccumulatingExplicitGenericMethodParameters)
                    {
                        var param = accum.TrimEnd(',').Trim();

                        var validate = CSharpClassNameValidator.ValidateInitialization(param, true);
                        if (!validate.Success)
                        {
                            result.Success = false;
                            result.ErrorDescription = validate.ErrorDescription;
                            result.ErrorIndex = (index - (accum.Length - 1)) + validate.ErrorIndex;
                            return result;
                        }

                        explicitGenericParameters.Add(validate);

                        accum = "";
                        continue;
                    }
                    if (state == States.AccumulatingParameter)
                    {
                        var param = accum.TrimEnd(',').Trim();

                        if (!CSharpIDValidator.IsValidIdentifier(param) &&
                            (lastModifier == CSharpParameterModifier.Ref || lastModifier == CSharpParameterModifier.Out))
                        {
                            result.Success = false;
                            result.ErrorDescription =
                                "'ref' and 'out' can only be used with a direct variable reference.";
                            result.ErrorIndex = index - (accum.Length - 1);
                            return result;
                        }

                        string err;
                        int errIndex;

                        if (lastModifier == CSharpParameterModifier.New &&
                            !IsValidNewParameter(param, index - (accum.Length - 1), out err, out errIndex))
                        {
                            result.Success = false;
                            result.ErrorDescription = err;
                            result.ErrorIndex = errIndex;
                            return result;
                        }


                        if (lastModifier == CSharpParameterModifier.None &&
                            !IsValidPlainParameter(param, index - (accum.Length - 1), out err, out errIndex))
                        {
                            result.Success = false;
                            result.ErrorDescription = err;
                            result.ErrorIndex = errIndex;
                            return result;
                        }

                        state = States.WaitingForParameterName;
                        parameters.Add(new CSharpParameterSignature(param, lastModifier));
                        lastModifier = CSharpParameterModifier.None;
                        accum = "";
                        continue;
                    }

                    result.Success = false;
                    result.ErrorDescription = "Unexpected ',' character.";
                    result.ErrorIndex = index;
                    return result;
                }
                if (c == '<')
                {
                    if (state == States.AccumulatingParameter ||
                        state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression ||
                        state == States.InBracketExpression)
                    {
                        continue;
                    }
                    if (state == States.AccumulatingMethodNamePart)
                    {
                        methodName = accum.TrimEnd('<').Trim();
                        accum = "";
                        state = States.AccumulatingExplicitGenericMethodParameters;
                        continue;
                    }
                    if (state == States.AccumulatingExplicitGenericMethodParameters)
                    {
                        unmatchedGenericBrackets++;
                        stateBeforeGenericTypePart.Push(state);
                        state = States.AccumulatingGenericTypePart;
                        continue;
                    }
                    if (state == States.AccumulatingGenericTypePart)
                    {
                        unmatchedGenericBrackets++;
                        continue;
                    }

                    result.Success = false;
                    result.ErrorDescription = "Unexpected '<' character.";
                    result.ErrorIndex = index;
                    return result;
                }
                if (c == '>')
                {
                    if (state == States.AccumulatingParameter ||
                        state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression ||
                        state == States.InBracketExpression)
                    {
                        continue;
                    }
                    if (state == States.AccumulatingGenericTypePart)
                    {
                        unmatchedGenericBrackets--;
                        if (unmatchedGenericBrackets == 0)
                        {
                            state = stateBeforeGenericTypePart.Pop();
                        }
                        continue;
                    }
                    if (state == States.AccumulatingExplicitGenericMethodParameters)
                    {
                        var param = accum.Substring(0, accum.Length > 0 ? accum.Length - 1 : 0).Trim();

                        var validate = CSharpClassNameValidator.ValidateInitialization(param, true);
                        if (!validate.Success)
                        {
                            result.Success = false;
                            result.ErrorDescription = validate.ErrorDescription;
                            result.ErrorIndex = (index - (accum.Length - 1)) + validate.ErrorIndex;
                            return result;
                        }

                        explicitGenericParameters.Add(validate);
                        accum = "";
                        state = States.AfterExplicitGenericMethodParameters;
                        continue;
                    }

                    result.Success = false;
                    result.ErrorDescription = "Unexpected '>' character.";
                    result.ErrorIndex = index;
                    return result;
                }

                if (!char.IsWhiteSpace(c))
                {
                    if (state == States.AccumulatingParameter ||
                        state == States.AccumulatingExplicitGenericMethodParameters ||
                        state == States.AccumulatingMethodNamePart ||
                        state == States.AccumulatingGenericTypePart ||
                        state == States.InCurlyBraceExpression ||
                        state == States.InBracketExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        continue;
                    }
                    if (state == States.WaitingForParameterName)
                    {
                        accum = "" + c;
                        state = States.AccumulatingParameter;
                        continue;
                    }
                    if (state == States.WaitingForFirstCharacter)
                    {
                        accum = "" + c;
                        state = States.AccumulatingMethodNamePart;
                        continue;
                    }

                    result.Success = false;
                    result.ErrorDescription = string.Format("Unexpected '{0}' character.", c);
                    result.ErrorIndex = index;
                    return result;
                }
            }


            if (state != States.AfterSignature)
            {
                result.Success = false;
                result.ErrorDescription = "Call signature incomplete.";
                result.ErrorIndex = signature.Length - 1;
                return result;
            }


            result.MethodName = methodName;
            result.Parameters = parameters;
            result.ExplicitGenericParameters = explicitGenericParameters;
            return result;
        }


        private enum States
        {
            WaitingForFirstCharacter,
            AccumulatingMethodNamePart,
            AccumulatingExplicitGenericMethodParameters,
            AfterExplicitGenericMethodParameters,
            AccumulatingGenericTypePart,
            WaitingForParameterName,
            AccumulatingParameter,
            AfterSignature,
            InParenthesizedExpression,
            InCurlyBraceExpression,
            InBracketExpression
        }
    }
}