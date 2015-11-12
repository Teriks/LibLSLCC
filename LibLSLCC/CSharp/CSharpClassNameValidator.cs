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
    /// Context of a class name signature
    /// </summary>
    public enum CSharpClassNameType
    {
        /// <summary>
        /// The class name signature is used to declare a class, it could be a generic type definition signature.
        /// </summary>
        Declaration,

        /// <summary>
        /// The class name signature is used to refer to the class in a initialized context such as 
        /// after the new keyword or in a cast, it still might be generic with filled out type parameters.
        /// </summary>
        Initialization
    }


    /// <summary>
    /// Tools to check the validity of raw class type names in strings
    /// </summary>
    public static class CSharpClassNameValidator
    {
        /// <summary>
        /// Validates that the specified input string is syntacticly valid C# type reference or class definition signature, including generic types.
        /// The function is context sensitive to whether or not the name signature is used for declaring a class or initializing a new type.
        /// 
        /// <paramref name="signatureType"/> can be set to <see cref="CSharpClassNameType.Declaration"/> or <see cref="CSharpClassNameType.Initialization"/> respectively.
        /// </summary>
        /// <remarks>
        /// This function will detect misuse of keywords and built in type names the class definition signature used to define a type, even generic types, when used in
        /// <see cref="CSharpClassNameType.Declaration"/> mode.  <see cref="CSharpClassNameType.Initialization"/> mode allows for qualified name references using the '.' operator.
        /// </remarks>
        /// <param name="input">The input string containing the proposed type value.</param>
        /// <param name="signatureType">The context this signature is being used in, class declaration, or class initialization.</param>
        /// <returns><see cref="CSharpClassNameValidationResult"/></returns>
        public static CSharpClassNameValidationResult Validate(string input,
            CSharpClassNameType signatureType = CSharpClassNameType.Declaration)
        {
            return _Validate(input, signatureType, 0);
        }

        private class Qualification
        {
            public Qualification(StringBuilder builder, int parseIndex)
            {
                Builder = builder;
                ParseIndex = parseIndex;
            }

            public StringBuilder Builder { get; set; }
            public int ParseIndex { get; set; }
        }

        private static CSharpClassNameValidationResult _Validate(string input, CSharpClassNameType signatureType,
            int index)
        {
            string fullyQualifiedName = "";
            var genericArgs = new List<CSharpClassNameValidationResult>();

            var qualifications = new List<Qualification>()
            {
                new Qualification(new StringBuilder(), index)
            };


            string genericPart = "";
            bool isGeneric = false;


            int genericBrackets = 0;

            foreach (var c in input)
            {
                index++;
                qualifications.Last().ParseIndex = index;

                //enter generic arguments
                if (c == '<')
                {
                    isGeneric = true;
                    genericBrackets++;
                }

                //check if we have gotten to the generic part of the type signature yet (if any)
                if (!isGeneric)
                {
                    if (c == '.')
                    {
                        //qualifier operator is not allowed anywhere in declaration signatures because
                        //they only consist of a raw type name and generic argument specifications
                        if (signatureType == CSharpClassNameType.Declaration)
                        {
                            return new CSharpClassNameValidationResult
                            {
                                Success = false,
                                ErrorDescription =
                                    string.Format(
                                        "Index {0}: '.' name qualifier operator is not valid in a class name/generic " +
                                        "type parameter used in the declaration class.", index),
                                ErrorIndex = index,
                            };
                        }

                        //detect a missing qualifier section that's terminated with a dot.
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

                        qualifications.Add(new Qualification(new StringBuilder(), index));
                    }
                    else
                    {
                        qualifications.Last().Builder.Append(c);
                    }
                }

                if (genericBrackets == 0 && !isGeneric)
                {
                    //accumulate to our fully qualified name
                    fullyQualifiedName += c;
                }
                else if (genericBrackets == 0 && isGeneric)
                {
                    //we have exited even pairs of brackets and are on the
                    //other side of the generic arguments at the end of the signature
                    //there should not be anything here

                    return new CSharpClassNameValidationResult
                    {
                        Success = false,
                        ErrorDescription = string.Format("Index {0}: extra content after generic argument list.", index),
                        ErrorIndex = index
                    };
                }
                else if (!(genericBrackets == 1 && c == '<'))
                {
                    //passed the start of the first < in the signature by 1
                    if ((c == ',' || c == '>') && genericBrackets == 1)
                    {
                        //we have accumulated a type argument suitable for recursive decent
                        //validate it recursively
                        var validateGenericArgument = _Validate(genericPart.Trim(), signatureType, index);

                        //return immediately on failure
                        if (!validateGenericArgument.Success) return validateGenericArgument;

                        //add the validated 'tree'
                        genericArgs.Add(validateGenericArgument);

                        //reset the accumulator
                        genericPart = "";
                    }
                    else
                    {
                        //acumulate until we hit a comma or the > character
                        genericPart += c;
                    }
                }

                if (c == '>')
                {
                    //exit a generic type scope
                    genericBrackets--;
                }
            }

            
            if (genericBrackets > 0)
            {
                //something is amiss with bracket matching
                return new CSharpClassNameValidationResult
                {
                    Success = false,
                    ErrorDescription = string.Format("Index {0}: mismatched generic brackets.", index),
                    ErrorIndex = index
                };
            }

            //there may be only one qualification, in which case
            //the base name is also the fully qualified name.
            var baseName = qualifications.Last();

            if (qualifications.Count > 1)
            {
                //uses qualified names, this is definitely an initialization signature
                //because the qualifier '.' operator returns an error in declaration signatures
                //above the recursive call to validate

                foreach (var name in qualifications)
                {
                    if (string.IsNullOrWhiteSpace(name.Builder.ToString()))
                    {
                        return new CSharpClassNameValidationResult
                        {
                            Success = false,
                            ErrorDescription =
                                string.Format("Index {0}:  qualified type name '{1}' is incomplete.", name.ParseIndex,
                                    fullyQualifiedName),
                            ErrorIndex = name.ParseIndex
                        };
                    }
                }

                //is my fully qualified non generic type a known built in type?
                if ((Type.GetType(fullyQualifiedName, false) == null))
                {
                    //no, not build in type, check everything for syntax errors

                    foreach (var name in qualifications)
                    {
                        //check for syntax errors in the qualified name pieces, they need to be valid ID's and not keywords
                        //IsValidIdentifier takes care of both these criteria
                        if (!CSharpCompilerSingleton.Compiler.IsValidIdentifier(name.Builder.ToString()))
                        {
                            //sound something funky
                            return new CSharpClassNameValidationResult
                            {
                                Success = false,
                                ErrorDescription =
                                    string.Format(
                                        "Index {0}: '{1}' is not valid in the given qualified type name '{2}'.",
                                        name.ParseIndex, name.Builder, fullyQualifiedName),
                                ErrorIndex = name.ParseIndex
                            };
                        }
                    }
                }
            }
            else
            {
                //single type argument to a generic type
                if (string.IsNullOrWhiteSpace(baseName.Builder.ToString()))
                {
                    return new CSharpClassNameValidationResult
                    {
                        Success = false,
                        ErrorDescription = string.Format("Index {0}:  missing generic {1} name.",
                            baseName.ParseIndex,
                            signatureType == CSharpClassNameType.Declaration ? "argument" : "type"),
                        ErrorIndex = baseName.ParseIndex,
                    };
                }

                //allow built in types only in initialization signatures, always disallow keywords and non ID's
                //IsValidIdentifier takes care of the later
                if ((Type.GetType(baseName.Builder.ToString(), false) == null && signatureType == CSharpClassNameType.Declaration) &&
                    !CSharpCompilerSingleton.Compiler.IsValidIdentifier(baseName.Builder.ToString()))
                {
                    return new CSharpClassNameValidationResult
                    {
                        Success = false,
                        ErrorDescription =
                            string.Format("Index {0}: '{1}' is not a valid {2} name.", baseName.ParseIndex,
                                baseName.Builder,
                                signatureType == CSharpClassNameType.Declaration ? "generic argument" : "type"),
                        ErrorIndex = baseName.ParseIndex
                    };
                }
            }

            //success
            return new CSharpClassNameValidationResult()
            {
                QualifiedName = fullyQualifiedName,
                BaseName = baseName.Builder.ToString(),
                GenericArguments = genericArgs.ToArray(),
                IsGeneric = isGeneric,
                Success = true
            };
        }
    }
}