#region FileInfo
// 
// File: CSharpClassNameValidator.cs
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
using System.Text;

namespace LibLSLCC.CSharp
{
    /// <summary>
    /// Class name validation result's produced by <see cref="CSharpClassNameValidator.Validate"/>
    /// </summary>
    public class CSharpClassNameValidationResult
    {
        /// <summary>
        /// Gets a value indicating whether the given string passed to <see cref="CSharpClassNameValidator.Validate"/> was a syntactically valid C# type reference.
        /// </summary>
        /// <value>
        ///   <c>true</c> if parsing success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; internal set; }

        /// <summary>
        /// Gets the basic class name of the parsed type reference, without any namespace qualification.
        /// This does not include the generic arguments in the type reference, if there are any.
        /// </summary>
        /// <value>
        /// The base name of the class in the type reference.
        /// </value>
        public string BaseName { get; internal set; }

        /// <summary>
        /// Gets the fully qualified name of the parsed type reference, with any namespace qualification that was included in the string.
        /// This does not include the generic arguments in the type reference, if there are any.
        /// </summary>
        /// <value>
        /// The fully qualified name of the class in the type reference.
        /// </value>
        public string QualifiedName { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the parsed type reference represented a generic type with generic type arguments.
        /// </summary>
        /// <value>
        /// <c>true</c> if the type reference used generic type arguments; otherwise, <c>false</c>.
        /// </value>
        public bool IsGeneric { get; internal set; }

        /// <summary>
        /// Gets a user friendly parsing error description when <see cref="Success"/> is false, otherwise <c>null</c>.
        /// </summary>
        /// <value>
        /// The parsing error description, if <see cref="Success"/> is <c>false</c>; otherwise, <c>null</c>.
        /// </value>
        public string ErrorDescription { get; internal set; }

        /// <summary>
        /// Gets an index near where the parsing error occurred when <see cref="Success"/> is <c>false</c>, otherwise 0.
        /// </summary>
        /// <value>
        /// The string index of the error.
        /// </value>
        public int ErrorIndex { get; internal set; }


        public CSharpClassNameValidationResult[] GenericArguments { get; internal set; }
    }


    /// <summary>
    /// Tools to check the validity of raw class type names in strings
    /// </summary>
    public static class CSharpClassNameValidator
    {
        /// <summary>
        /// Validates that the specified input string is syntacticly valid C# type reference, including generic types.
        /// </summary>
        /// <param name="input">The input string containing the proposed type value.</param>
        /// <returns><see cref="CSharpClassNameValidationResult"/></returns>
        public static CSharpClassNameValidationResult Validate(string input)
        {
            return _Validate(input, 0);
        }

        private static CSharpClassNameValidationResult _Validate(string input, int index)
        {
            string basePart = "";
            List<CSharpClassNameValidationResult> genericArgs = new List<CSharpClassNameValidationResult>();

            List<Tuple<StringBuilder, int>> qualifications = new List<Tuple<StringBuilder, int>>()
            {
                new Tuple<StringBuilder, int>(new StringBuilder(), 0)
            };


            string genericPart = "";
            bool isGeneric = false;

            int genericBrackets = 0;

            foreach (var c in input)
            {
                index++;

                if (c == '<')
                {
                    isGeneric = true;
                    genericBrackets++;
                }

                if (!isGeneric)
                {
                    if (c == '.')
                    {
                        if (string.IsNullOrWhiteSpace(qualifications.Last().ToString()))
                        {
                            return new CSharpClassNameValidationResult
                            {
                                Success = false,
                                ErrorDescription =
                                    string.Format("Index {0}: '..' is not valid in a qualified type name.", index),
                                ErrorIndex = index,
                            };
                        }

                        qualifications.Add(new Tuple<StringBuilder, int>(new StringBuilder(), index));
                    }
                    else
                    {
                        qualifications.Last().Item1.Append(c);
                    }
                }

                if (genericBrackets == 0 && !isGeneric)
                {
                    basePart += c;
                }
                else if (genericBrackets == 0 && isGeneric)
                {
                    return new CSharpClassNameValidationResult
                    {
                        Success = false,
                        ErrorDescription = string.Format("Index {0}: extra content after generic argument list.", index),
                        ErrorIndex = index
                    };
                }
                else if (!(genericBrackets == 1 && c == '<'))
                {
                    if ((c == ',' || c == '>') && genericBrackets == 1)
                    {
                        var validateGenericArgument = _Validate(genericPart.Trim(), index);

                        if (!validateGenericArgument.Success) return validateGenericArgument;

                        genericArgs.Add(validateGenericArgument);

                        genericPart = "";
                    }
                    else
                    {
                        genericPart += c;
                    }
                }

                if (c == '>')
                {
                    genericBrackets--;
                }

                
            }

            if (genericBrackets > 0)
            {
                return new CSharpClassNameValidationResult
                {
                    Success = false,
                    ErrorDescription = "mismatched generic brackets."
                };
            }


            var baseName = qualifications.Last();

            if (qualifications.Count > 1)
            {
                foreach (var name in qualifications)
                {
                    if (string.IsNullOrWhiteSpace(name.Item1.ToString()))
                    {
                        return new CSharpClassNameValidationResult
                        {
                            Success = false,
                            ErrorDescription =
                                string.Format("Index {0}:  qualified type name '{1}' is incomplete.", name.Item2,
                                    basePart),
                            ErrorIndex = name.Item2
                        };
                    }
                }

                foreach (var name in qualifications)
                {
                    //allow primitive types like int
                    if (Type.GetType(basePart, false) == null &&
                        !CSharpCompilerSingleton.Compiler.IsValidIdentifier(name.Item1.ToString()))
                    {
                        return new CSharpClassNameValidationResult
                        {
                            Success = false,
                            ErrorDescription =
                                string.Format("Index {0}: '{1}' is not valid in the given qualified type name '{2}'.",
                                    name.Item2, name.Item1, basePart),
                            ErrorIndex = name.Item2
                        };
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(baseName.Item1.ToString()))
                {
                    return new CSharpClassNameValidationResult
                    {
                        Success = false,
                        ErrorDescription = string.Format("Index {0}:  missing generic type specifier/type name.", baseName.Item2),
                        ErrorIndex = baseName.Item2,
                    };
                }

                //allow primitive types like int 
                if (Type.GetType(baseName.Item1.ToString(), false) == null &&
                    !CSharpCompilerSingleton.Compiler.IsValidIdentifier(baseName.Item1.ToString()))
                {
                    return new CSharpClassNameValidationResult
                    {
                        Success = false,
                        ErrorDescription =
                            string.Format("Index {0}: '{1}' is not a valid type name.", baseName.Item2, baseName.Item1),
                        ErrorIndex = baseName.Item2
                    };
                }
            }


            return new CSharpClassNameValidationResult()
            {
                QualifiedName = basePart,
                BaseName = baseName.Item1.ToString(),
                GenericArguments = genericArgs.ToArray(),
                IsGeneric = isGeneric,
                Success = true
            };
        }
    }
}