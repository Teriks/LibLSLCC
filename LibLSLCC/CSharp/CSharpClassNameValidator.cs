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
using System.Windows.Forms;

namespace LibLSLCC.CSharp
{
    public class CSharpParsedTypeValidationResult
    {
        public bool IsValid { get; private set; }
        public string ErrorMessage { get; private set; }

        public object Tag { get; set; }

        public CSharpParsedTypeValidationResult()
        {
            IsValid = true;
        }

        public CSharpParsedTypeValidationResult(string errorMessage)
        {
            IsValid = false;
            ErrorMessage = errorMessage;
        }
    }


    /// <summary>
    /// Class name validation result's produced by <see cref="CSharpClassNameValidator.ValidateDeclaration"/> 
    /// and <see cref="CSharpClassNameValidator.ValidateInitialization"/>.
    /// </summary>
    public class CSharpClassNameValidationResult
    {
        /// <summary>
        /// Gets a value indicating whether the given string passed to <see cref="CSharpClassNameValidator.ValidateDeclaration"/>
        /// or <see cref="CSharpClassNameValidator.ValidateInitialization"/> was syntactically valid.
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

        /// <summary>
        /// Gets the sub validation results for the generic arguments of the parsed type if it has any.
        /// The contents of this array will always be empty if <see cref="Success"/> is <c>false</c>.
        /// </summary>
        /// <value>
        /// The sub validation results for the generic arguments.
        /// </value>
        public CSharpClassNameValidationResult[] GenericArguments { get; internal set; }

        /// <summary>
        /// Gets the type validation result for this parsed type.  This is only ever non <c>null</c> if a callback for type validation
        /// is passed to <see cref="CSharpClassNameValidator.ValidateInitialization"/>.
        /// </summary>
        /// <value>
        /// The type validation result for this parsed class name.
        /// </value>
        public CSharpParsedTypeValidationResult TypeValidationResult { get; internal set; }


        /// <summary>
        /// Gets the full signature of the type/class, with spaces that exist in the input stripped out.
        /// </summary>
        /// <value>
        /// The full signature of the parsed type/class.
        /// </value>
        public string FullSignature { get; internal set; }
    }


    public delegate CSharpParsedTypeValidationResult CSharpParsedTypeValidateTypeCallback(
        CSharpClassNameValidationResult parsedTypeDescription);


    /// <summary>
    /// Tools to check the validity of raw class type names in strings
    /// </summary>
    public static class CSharpClassNameValidator
    {
        private static readonly Dictionary<string, Type> BuiltInTypeMap = new Dictionary<string, Type>()
        {
            {"bool", typeof (bool)},
            {"byte", typeof (byte)},
            {"sbyte", typeof (sbyte)},
            {"char", typeof (char)},
            {"decimal", typeof (decimal)},
            {"double", typeof (double)},
            {"float", typeof (float)},
            {"int", typeof (int)},
            {"uint", typeof (uint)},
            {"long", typeof (long)},
            {"ulong", typeof (ulong)},
            {"object", typeof (object)},
            {"short", typeof (short)},
            {"ushort", typeof (ushort)},
            {"string", typeof (string)}
        };


        /// <summary>
        /// Converts a CSharp type alias such as 'int', 'float' or object (etc..) to its corresponding <see cref="Type"/>.
        /// </summary>
        /// <param name="keywordTypeName">The keyword type alias to convert to an actual <see cref="Type"/>.</param>
        /// <returns>The type if there is a corresponding <see cref="Type"/>, otherwise <c>null</c>.</returns>
        public static Type KeywordTypetoType(string keywordTypeName)
        {
            Type t;
            if (BuiltInTypeMap.TryGetValue(keywordTypeName, out t))
            {
                return t;
            }
            return null;
        }


        /// <summary>
        /// Determines if a string is a built in type alias reference such as int.
        /// </summary>
        /// <returns><c>true</c> if the given string contains a built in type alias name; otherwise <c>false</c>.</returns>
        public static bool IsTypeAliasKeyword(string keywordTypeName)
        {
            return BuiltInTypeMap.ContainsKey(keywordTypeName);
        }


        /// <summary>
        /// Context of a class name signature
        /// </summary>
        private enum ClassSigType
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
        /// Validates that the specified input string is syntacticly valid C# class definition signature, including generic definition.
        /// </summary>
        /// <remarks>
        /// This function will detect misuse of keywords and built in type names the class definition signature used to define a type, even generic types.
        /// </remarks>
        /// <param name="input">The input string containing the proposed type value.</param>
        /// <returns><see cref="CSharpClassNameValidationResult"/></returns>
        public static CSharpClassNameValidationResult ValidateDeclaration(string input)
        {
            return _Validate(input, ClassSigType.Declaration, false, null, 0);
        }


        /// <summary>
        /// Validates that the specified input string is syntacticly valid C# type initialization signature, including generic types.
        /// </summary>
        /// <param name="input">The input string containing the proposed type value.</param>
        /// <param name="validateTypeCallback">A call back to allow you to verify the existence of the types in the type signature as they are parsed.</param>
        /// <param name="allowBuiltInAliases">Allow built in aliases such as 'int' or 'char' to pass as class names</param>
        /// <returns><see cref="CSharpClassNameValidationResult"/></returns>
        public static CSharpClassNameValidationResult ValidateInitialization(string input, bool allowBuiltInAliases,
            CSharpParsedTypeValidateTypeCallback validateTypeCallback = null)
        {
            return _Validate(input, ClassSigType.Initialization, allowBuiltInAliases, validateTypeCallback, 0);
        }


        private class Qualification
        {
            public Qualification(StringBuilder builder, int stopIndex)
            {
                Builder = builder;
                StopIndex = stopIndex;
            }

            public StringBuilder Builder { get; private set; }
            public int StopIndex { get; private set; }

            public int StartIndex
            {
                get
                {
                    if (StopIndex == 0) return 0;
                    return StopIndex - Builder.Length;
                }
            }
        }

        private static CSharpClassNameValidationResult _Validate(string input, ClassSigType signatureType,
            bool allowBuiltinAliases, CSharpParsedTypeValidateTypeCallback validateTypeCallback, int index)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input", "Class name/signature string cannot be null!");
            }


            if (string.IsNullOrWhiteSpace(input))
            {
                return new CSharpClassNameValidationResult
                {
                    Success = false,
                    ErrorDescription =
                        "Class name/signature cannot be whitespace.",
                    ErrorIndex = 0,
                };
            }


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
                        if (signatureType == ClassSigType.Declaration)
                        {
                            return new CSharpClassNameValidationResult
                            {
                                Success = false,
                                ErrorDescription =
                                    string.Format(
                                        "'.' name qualifier operator is not valid in a class declaration/generic " +
                                        "type placeholder name."),
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
                                    "\'..\' is an invalid use of the qualification operator.",
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
                        ErrorDescription = "extra content found after generic argument list.",
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
                        var validateGenericArgument = _Validate(genericPart.Trim(), signatureType, allowBuiltinAliases,
                            validateTypeCallback, index);

                        //return immediately on failure
                        if (!validateGenericArgument.Success) return validateGenericArgument;

                        //add the validated 'tree'
                        genericArgs.Add(validateGenericArgument);

                        //reset the accumulator
                        genericPart = "";
                    }
                    else
                    {
                        //accumulate until we hit a comma or the > character
                        genericPart += c;
                    }
                }

                if (c == '>')
                {
                    //exit a generic type scope
                    genericBrackets--;
                }

                index++;
            }


            if (genericBrackets > 0)
            {
                //something is amiss with bracket matching
                return new CSharpClassNameValidationResult
                {
                    Success = false,
                    ErrorDescription = "mismatched generic type brackets.",
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
                                string.Format("qualified type name '{0}' is incomplete.",
                                    fullyQualifiedName),
                            ErrorIndex = name.StartIndex
                        };
                    }
                }


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
                                    "'{0}' is not valid in the given qualified type name '{1}'.", name.Builder,
                                    fullyQualifiedName),
                            ErrorIndex = name.StartIndex
                        };
                    }
                }
            }
            else
            {
                var shortName = baseName.Builder.ToString();

                //single type argument to a generic type
                if (string.IsNullOrWhiteSpace(shortName))
                {
                    return new CSharpClassNameValidationResult
                    {
                        Success = false,
                        ErrorDescription = string.Format("missing generic {0} name.",
                            signatureType == ClassSigType.Initialization ? "type argument" : "placeholder type"),
                        ErrorIndex = baseName.StartIndex,
                    };
                }


                bool aliasInitialization = allowBuiltinAliases && BuiltInTypeMap.ContainsKey(shortName) &&
                                           signatureType == ClassSigType.Initialization;

                if (!aliasInitialization &&
                    !CSharpCompilerSingleton.Compiler.IsValidIdentifier(baseName.Builder.ToString()))
                {
                    return new CSharpClassNameValidationResult
                    {
                        Success = false,
                        ErrorDescription =
                            string.Format("'{0}' is not an allowed CSharp identifier.",
                                baseName.Builder),
                        ErrorIndex = baseName.StartIndex
                    };
                }
            }


            var baseNameString = baseName.Builder.ToString();


            if (isGeneric && IsTypeAliasKeyword(fullyQualifiedName))
            {
                return new CSharpClassNameValidationResult
                {
                    Success = false,
                    ErrorDescription =
                        string.Format("Built in type alias '{0}' is not a generic type.",
                            baseName.Builder),
                    ErrorIndex = baseName.StartIndex
                };
            }


            //success
            var classDescription = new CSharpClassNameValidationResult()
            {
                QualifiedName = fullyQualifiedName,
                BaseName = baseNameString,
                GenericArguments = genericArgs.ToArray(),
                IsGeneric = isGeneric,
                Success = true
            };


            if (isGeneric)
            {
                classDescription.FullSignature = fullyQualifiedName + "<" +
                                                 string.Join(", ",
                                                     classDescription.GenericArguments.Select(x => x.FullSignature)) +
                                                 ">";
            }
            else
            {
                classDescription.FullSignature = fullyQualifiedName;
            }


            if (validateTypeCallback == null || signatureType != ClassSigType.Initialization) return classDescription;

            var typeCheckResult = validateTypeCallback(classDescription);

            if (typeCheckResult.IsValid) return classDescription;

            classDescription.FullSignature = null;
            classDescription.ErrorIndex = qualifications.First().StartIndex;
            classDescription.ErrorDescription = typeCheckResult.ErrorMessage;
            classDescription.Success = false;


            return classDescription;
        }
    }
}