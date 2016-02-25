#region FileInfo

// 
// File: LSLListParser.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using LibLSLCC.AntlrParser;

#endregion

namespace LibLSLCC.Utility.ListParser
{
    /// <summary>
    ///     Tool for parsing LSL list from source code styled strings.
    /// </summary>
    public static class LSLListParser
    {
        /// <summary>
        ///     List parsing option flags.
        /// </summary>
        [Flags]
        public enum LSLListParsingFlags
        {
            /// <summary>
            ///     No options selected
            /// </summary>
            None = 0,

            /// <summary>
            ///     The allow variable references to appear in the list at the top level.
            /// </summary>
            AllowVariableReferencesInList = 2,

            /// <summary>
            ///     The allow variable references to appear in vectors that are elements of the list.
            /// </summary>
            AllowVariableReferencesInVectors = 8,

            /// <summary>
            ///     The allow variable references to appear in rotations that are elements of the list.
            /// </summary>
            AllowVariableReferencesInRotations = 4
        }


        private static IEnumerable<object> EnumerateExpressionList(LSLParser.ExpressionListContext context)
        {
            var expression = context.expression();
            if (expression == null)
            {
                yield break;
            }

            yield return expression;

            var tail = context.expressionListTail();

            foreach (var e in tail)
            {
                yield return e.expression();
            }
        }


        /// <summary>
        ///     Attempts to parse an LSL list from a string and returns true if the parse succeeded.
        /// </summary>
        /// <param name="list">The string containing the list.</param>
        /// <param name="expressions">
        ///     The resulting expression list will be put at this location if the parse succeeded, otherwise
        ///     it will be null.
        /// </param>
        /// <param name="parsingFlags">Optional parsing flags.</param>
        /// <returns></returns>
        public static bool TryParseList(string list, out List<ILSLListExpr> expressions,
            LSLListParsingFlags parsingFlags = LSLListParsingFlags.None)
        {
            try
            {
                expressions = ParseList(list, parsingFlags).ToList();
                return true;
            }
            catch (LSLListParserSyntaxException)
            {
                expressions = null;
                return false;
            }
        }


        /// <summary>
        ///     Formats the specified expressions into the string representation of an LSL list.
        /// </summary>
        /// <param name="expressions">The expressions to format.</param>
        /// <param name="brackets">
        ///     if set to <c>true</c> place brackets around the formated list, otherwise just return the CSV
        ///     list content.
        /// </param>
        /// <returns></returns>
        public static string Format(IEnumerable<ILSLListExpr> expressions, bool brackets = true)
        {
            return (brackets ? "[" : "") + string.Join(", ", expressions.Select(x => x.ValueString)) +
                   (brackets ? "]" : "");
        }


        /// <summary>
        ///     Formats the specified list string by parsing it with <see cref="ParseList" /> and passing the resulting enumerable
        ///     to <see cref="Format(IEnumerable{ILSLListExpr},bool)" />.
        /// </summary>
        /// <param name="listString">The string containing the list to parse and format.</param>
        /// <param name="brackets">
        ///     if set to <c>true</c> place brackets around the formated list, otherwise just return the CSV
        ///     list content.
        /// </param>
        /// <returns></returns>
        public static string Format(string listString, bool brackets = true)
        {
            return Format(ParseList(listString), brackets);
        }


        private static ILSLListExpr BasicAtomToExpr(LSLParser.Expr_AtomContext c, string numericPrefix = null)
        {
            if (c.string_literal != null)
            {
                if (numericPrefix != null)
                    throw new InvalidOperationException(
                        "LSLListParser.BasicAtomToExpr:  Numeric prefix cannot be added to a string literal node.");
                return (new LSLListStringExpr(c.GetText()));
            }
            if (c.float_literal != null)
            {
                numericPrefix = numericPrefix ?? "";
                return new LSLListFloatExpr(numericPrefix + c.GetText());
            }
            if (c.hex_literal != null)
            {
                if (numericPrefix != null)
                    throw new InvalidOperationException(
                        "LSLListParser.BasicAtomToExpr:  Numeric prefix cannot be added to a hex literal node.");
                return new LSLListFloatExpr(c.GetText(), true);
            }
            if (c.integer_literal != null)
            {
                numericPrefix = numericPrefix ?? "";
                return new LSLListFloatExpr(numericPrefix + c.GetText());
            }

            return null;
        }


        /// <summary>
        ///     Parses an LSL list from a string and returns the simple expressions it contains as an enumerable.
        ///     <remarks>
        ///         Take note that parsing wont start to occur until you begin enumerating the returned value.
        ///     </remarks>
        /// </summary>
        /// <param name="list">The string containing the list.</param>
        /// <param name="parsingFlags">Optional parsing flags.</param>
        /// <returns></returns>
        /// <exception cref="LSLListParserOptionsConstraintException">
        ///     When an
        ///     <see cref="LSLListParsingFlags" /> constraint is violated.
        /// </exception>
        /// <exception cref="LSLListParserSyntaxException">
        ///     Rotations must contain only literal values to be parsed, and cannot contain these types of expressions: rotations,
        ///     vectors, lists or strings.
        ///     or
        ///     Vectors must contain only literal values to be parsed, and cannot contain these types of expressions: rotations,
        ///     vectors, lists or strings.
        ///     or
        ///     Lists cannot contain other lists.
        ///     or
        ///     Cast expressions can only be used to specify that a list element is a 'key' and not a 'string'
        ///     or
        ///     Encountered an un-parseable expression in the list, only literal values and possibly variable names are acceptable
        ///     when parsing.
        /// </exception>
        public static IEnumerable<ILSLListExpr> ParseList(string list,
            LSLListParsingFlags parsingFlags = LSLListParsingFlags.None)
        {
            var strm = new AntlrInputStream(new StringReader(list));
            var lex = new LSLLexer(strm);
            var parse = new LSLParser(new CommonTokenStream(lex));
            parse.ErrorListeners.Clear();
            lex.ErrorListeners.Clear();

            var errorListener = new ErrorListener();
            parse.AddErrorListener(errorListener);
            parse.AddErrorListener(errorListener);

            var production = parse.listLiteral();

            if (production.expression_list == null)
            {
                throw new LSLListParserSyntaxException("List rule missing expression list, parsing error");
            }

            var listProduction = production.expression_list.expressionList();

            if (listProduction == null) yield break;

            foreach (var child in EnumerateExpressionList(listProduction))
            {
                var atomToken = child as LSLParser.Expr_AtomContext;
                var castExpression = child as LSLParser.Expr_TypeCastContext;
                var negateOrPositive = child as LSLParser.Expr_PrefixOperationContext;

                if (atomToken != null)
                {
                    var maybeBasic = BasicAtomToExpr(atomToken);
                    if (maybeBasic != null)
                    {
                        yield return maybeBasic;
                    }

                    if (atomToken.variable != null)
                    {
                        if ((parsingFlags & LSLListParsingFlags.AllowVariableReferencesInList) ==
                            LSLListParsingFlags.AllowVariableReferencesInList)
                        {
                            yield return (new LSLListStringExpr(atomToken.GetText()));
                        }
                        else
                        {
                            throw new LSLListParserOptionsConstraintException(
                                "Variable references are not allowed in the list.");
                        }
                    }

                    if (atomToken.rotation_literal != null)
                    {
                        yield return ListExpressionFromRotation(parsingFlags, atomToken);
                    }

                    if (atomToken.vector_literal != null)
                    {
                        yield return ListExpressionFromVector(parsingFlags, atomToken);
                    }

                    if (atomToken.list_literal != null)
                    {
                        throw new LSLListParserSyntaxException("Lists cannot contain other lists.");
                    }
                }
                else if (castExpression != null)
                {
                    var stringLiteral = castExpression.expr_rvalue as LSLParser.Expr_AtomContext;
                    if (stringLiteral != null && stringLiteral.string_literal != null &&
                        castExpression.cast_type.Text == "key")
                    {
                        yield return (new LSLListKeyExpr(stringLiteral.GetText()));
                    }
                    else
                    {
                        throw new LSLListParserSyntaxException(
                            "Cast expressions can only be used to specify that a list element is a 'key' and not a 'string'");
                    }
                }
                else if (negateOrPositive != null)
                {
                    var floatOrInt = negateOrPositive.expr_rvalue as LSLParser.Expr_AtomContext;
                    var operation = negateOrPositive.operation.Text;

                    var validType = floatOrInt != null &&
                                    (floatOrInt.float_literal != null || floatOrInt.integer_literal != null);

                    if (validType && operation == "-" || operation == "+")
                    {
                        yield return BasicAtomToExpr(floatOrInt, operation);
                    }
                    else
                    {
                        throw new LSLListParserSyntaxException(
                            string.Format(
                                "The Negative and Positive prefix operator can only be used on Floats and Integer list elements, operator '{0}' is not valid.",
                                operation));
                    }
                }
                else
                {
                    throw new LSLListParserSyntaxException(
                        "Encountered an un-parseable expression in the list, only LSL literals and possibly variable names are acceptable list elements.");
                }
            }
        }


        private static ILSLListExpr ListExpressionFromVector(LSLListParsingFlags parsingFlags,
            LSLParser.Expr_AtomContext atomToken)
        {
            object[] vecComponents =
            {
                atomToken.vector_literal.vector_x,
                atomToken.vector_literal.vector_y,
                atomToken.vector_literal.vector_z
            };

            for (var i = 0; i < vecComponents.Length; i++)
            {
                var atom = vecComponents[i] as LSLParser.Expr_AtomContext;
                var prefix = vecComponents[i] as LSLParser.Expr_PrefixOperationContext;

                if (atom != null)
                {
                    if (atom.float_literal != null || atom.integer_literal != null || atom.hex_literal != null)
                    {
                        vecComponents[i] = BasicAtomToExpr(atom);
                    }
                    else if (atom.variable != null)
                    {
                        if ((parsingFlags & LSLListParsingFlags.AllowVariableReferencesInVectors) ==
                            LSLListParsingFlags.AllowVariableReferencesInVectors)
                        {
                            vecComponents[i] = new LSLListVariableExpr(atomToken.GetText());
                        }
                        throw new LSLListParserOptionsConstraintException(
                            "Variable references are not allowed in Vector literals.");
                    }
                    else
                    {
                        goto throw_type_error;
                    }
                }
                else if (prefix != null)
                {
                    var floatOrInt = prefix.expr_rvalue as LSLParser.Expr_AtomContext;
                    var operation = prefix.operation.Text;

                    var validType = floatOrInt != null &&
                                    (floatOrInt.float_literal != null || floatOrInt.integer_literal != null);

                    if (validType && (operation == "-" || operation == "+"))
                    {
                        vecComponents[i] = BasicAtomToExpr(floatOrInt, operation);
                    }
                    else
                    {
                        throw new LSLListParserSyntaxException(
                            string.Format(
                                "The Negative and Positive prefix operator can only be used on Floats and Integers inside of a Vector, operator '{0}' is not valid.",
                                operation));
                    }
                }
                else
                {
                    goto throw_type_error;
                }

                continue;
                throw_type_error:

                throw new LSLListParserSyntaxException(
                    "Vectors must contain only Float and Integer literal values.");
            }

            return new LSLListVectorExpr((ILSLListExpr) vecComponents[0], (ILSLListExpr) vecComponents[1],
                (ILSLListExpr) vecComponents[2]);
        }


        private static ILSLListExpr ListExpressionFromRotation(LSLListParsingFlags parsingFlags,
            LSLParser.Expr_AtomContext atomToken)
        {
            object[] rotComponents =
            {
                atomToken.rotation_literal.rotation_x,
                atomToken.rotation_literal.rotation_y,
                atomToken.rotation_literal.rotation_z,
                atomToken.rotation_literal.rotation_s
            };

            for (int i = 0; i < rotComponents.Length; i++)
            {
                var atom = rotComponents[i] as LSLParser.Expr_AtomContext;
                var prefix = rotComponents[i] as LSLParser.Expr_PrefixOperationContext;

                if (atom != null)
                {
                    if (atom.float_literal != null || atom.integer_literal != null || atom.hex_literal != null)
                    {
                        rotComponents[i] = BasicAtomToExpr(atom);
                    }
                    else if (atom.variable != null)
                    {
                        if ((parsingFlags & LSLListParsingFlags.AllowVariableReferencesInRotations) ==
                            LSLListParsingFlags.AllowVariableReferencesInRotations)
                        {
                            rotComponents[i] = new LSLListVariableExpr(atomToken.GetText());
                        }
                        throw new LSLListParserOptionsConstraintException(
                            "Variable references are not allowed in Rotation literals.");
                    }
                    else
                    {
                        goto throw_type_error;
                    }
                }
                else if (prefix != null)
                {
                    var floatOrInt = prefix.expr_rvalue as LSLParser.Expr_AtomContext;
                    var operation = prefix.operation.Text;

                    var validType = floatOrInt != null &&
                                    (floatOrInt.float_literal != null || floatOrInt.integer_literal != null);

                    if (validType && (operation == "-" || operation == "+"))
                    {
                        rotComponents[i] = BasicAtomToExpr(floatOrInt, operation);
                    }
                    else
                    {
                        throw new LSLListParserSyntaxException(
                            string.Format(
                                "The Negative and Positive prefix operator can only be used on Floats and Integers inside of a Rotation, operator '{0}' is not valid.",
                                operation));
                    }
                }
                else
                {
                    goto throw_type_error;
                }

                continue;
                throw_type_error:

                throw new LSLListParserSyntaxException(
                    "Rotations must contain only Float and Integer literal values.");
            }

            return new LSLListRotationExpr((ILSLListExpr) rotComponents[0], (ILSLListExpr) rotComponents[1],
                (ILSLListExpr) rotComponents[2], (ILSLListExpr) rotComponents[3]);
        }


        private class ErrorListener : BaseErrorListener
        {
            public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line,
                int charPositionInLine, string msg,
                RecognitionException e)
            {
                throw new LSLListParserSyntaxException(msg);
            }
        }
    }
}