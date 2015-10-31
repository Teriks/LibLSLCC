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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.Collections;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.Utility
{
    /// <summary>
    ///     Thrown when an <see cref="LSLListParser" /> encounters a syntax error while parsing.
    /// </summary>
    [Serializable]
    public class LSLListParserSyntaxException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLListParserSyntaxException" /> class.
        /// </summary>
        public LSLListParserSyntaxException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLListParserSyntaxException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LSLListParserSyntaxException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLListParserSyntaxException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public LSLListParserSyntaxException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLListParserSyntaxException" /> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual
        ///     information about the source or destination.
        /// </param>
        protected LSLListParserSyntaxException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }


    /// <summary>
    ///     Thrown when an <see cref="LSLListParser.LSLListParsingFlags" /> constraint is violated.
    /// </summary>
    [Serializable]
    public class LSLListParserOptionsConstraintViolationException : LSLListParserSyntaxException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLListParserOptionsConstraintViolationException" /> class.
        /// </summary>
        public LSLListParserOptionsConstraintViolationException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLListParserOptionsConstraintViolationException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LSLListParserOptionsConstraintViolationException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLListParserOptionsConstraintViolationException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public LSLListParserOptionsConstraintViolationException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLListParserOptionsConstraintViolationException" /> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual
        ///     information about the source or destination.
        /// </param>
        protected LSLListParserOptionsConstraintViolationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }


    /// <summary>
    ///     Tool for parsing LSL list from source code styled strings.
    /// </summary>
    public class LSLListParser
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
        ///     Parses an LSL list from a string and returns the simple expressions it contains.
        /// </summary>
        /// <param name="list">The string containing the list.</param>
        /// <param name="parsingFlags">Optional parsing flags.</param>
        /// <returns></returns>
        /// <exception cref="LSLListParserOptionsConstraintViolationException">
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
        public static List<ILSLListExpr> ParseList(string list,
            LSLListParsingFlags parsingFlags = LSLListParsingFlags.None)
        {
            return _ParseList(list, parsingFlags).ToList();
        }



        /// <summary>
        /// Attempts to parse an LSL list from a string and returns true if the parse succeeded.
        /// </summary>
        /// <param name="list">The string containing the list.</param>
        /// <param name="expressions">The resulting expression list will be put at this location if the parse succeeded, otherwise it will be null.</param>
        /// <param name="parsingFlags">Optional parsing flags.</param>
        /// <returns></returns>
        public static bool TryParseList(string list, out List<ILSLListExpr> expressions, LSLListParsingFlags parsingFlags = LSLListParsingFlags.None)
        {
            try
            {
                expressions = ParseList(list, parsingFlags);
                return true;
            }
            catch (LSLListParserSyntaxException)
            {
                expressions = null;
                return false;
            }
        }


        /// <summary>
        /// Formats the specified expressions into the string representation of an LSL list.
        /// </summary>
        /// <param name="expressions">The expressions to format.</param>
        /// <param name="brackets">if set to <c>true</c> place brackets around the formated list, otherwise just return the CSV list content.</param>
        /// <returns></returns>
        public static string Format(IEnumerable<ILSLListExpr> expressions, bool brackets=true)
        {
            return  (brackets ? "[" : "") + string.Join(", ", expressions.Select(x => x.ValueString)) + (brackets ? "]" : "");
        }


        /// <summary>
        /// Formats the specified list string by parsing it with <see cref="ParseListAsEnumerable"/> and passing the resulting enumerable to <see cref="Format(IEnumerable{ILSLListExpr},bool)"/>.
        /// </summary>
        /// <param name="listString">The string containing the list to parse and format.</param>
        /// <param name="brackets">if set to <c>true</c> place brackets around the formated list, otherwise just return the CSV list content.</param>
        /// <returns></returns>
        public static string Format(string listString, bool brackets = true)
        {
            return Format(ParseListAsEnumerable(listString), brackets);
        }


        /// <summary>
        /// Parses an LSL list from a string and returns the simple expressions it contains as an enumerable.
        /// <remarks>
        /// Take note that parsing wont start to occur until you begin enumerating the returned value.
        /// </remarks>
        /// </summary>
        /// <param name="list">The string containing the list.</param>
        /// <param name="parsingFlags">Optional parsing flags.</param>
        /// <returns></returns>
        /// <exception cref="LSLListParserOptionsConstraintViolationException">
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
        public static IEnumerable<ILSLListExpr> ParseListAsEnumerable(string list,
            LSLListParsingFlags parsingFlags = LSLListParsingFlags.None)
        {
            return _ParseList(list, parsingFlags);
        }


        private static ILSLListExpr BasicAtomToExpr(LSLParser.Expr_AtomContext c, string numericPrefix = null)
        {
            if (c.string_literal != null)
            {
                if(numericPrefix!=null) throw new InvalidOperationException("LSLListParser.BasicAtomToExpr:  Numeric prefix cannot be added to a string literal node.");
                return (new LSLString(c.GetText()));
            }
            if (c.float_literal != null)
            {
                numericPrefix = numericPrefix ?? "";
                return new LSLFloat(numericPrefix + c.GetText());
            }
            if (c.hex_literal != null)
            {
                if (numericPrefix != null) throw new InvalidOperationException("LSLListParser.BasicAtomToExpr:  Numeric prefix cannot be added to a hex literal node.");
                return new LSLFloat(c.GetText(), true);
            }
            if (c.integer_literal != null)
            {
                numericPrefix = numericPrefix ?? "";
                return new LSLFloat(numericPrefix + c.GetText());
            }

            return null;
        }

        private static IEnumerable<ILSLListExpr> _ParseList(string list,
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
                            yield return (new LSLString(atomToken.GetText()));
                        }
                        else
                        {
                            throw new LSLListParserOptionsConstraintViolationException(
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
                        yield return (new LSLKey(stringLiteral.GetText()));
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
                            string.Format("The Negative and Positive prefix operator can only be used on Floats and Integer list elements, operator '{0}' is not valid.", operation));
                    }

                }
                else
                {
                    throw new LSLListParserSyntaxException(
                        "Encountered an un-parseable expression in the list, only LSL literals and possibly variable names are acceptable list elements.");
                }
            }
        }

        private static ILSLListExpr ListExpressionFromVector(LSLListParsingFlags parsingFlags, LSLParser.Expr_AtomContext atomToken)
        {

            object[] xe =
            {
                atomToken.vector_literal.vector_x,
                atomToken.vector_literal.vector_y,
                atomToken.vector_literal.vector_z,
            };

            for (int i = 0; i < xe.Length; i++)
            {
                var atom = xe[i] as LSLParser.Expr_AtomContext;
                var prefix = xe[i] as LSLParser.Expr_PrefixOperationContext;

                if (atom != null)
                {
                    if (atom.float_literal != null || atom.integer_literal != null || atom.hex_literal != null)
                    {
                        xe[i] = BasicAtomToExpr(atom);
                    }
                    else if (atom.variable != null)
                    {
                        if ((parsingFlags & LSLListParsingFlags.AllowVariableReferencesInVectors) ==LSLListParsingFlags.AllowVariableReferencesInVectors)
                        {
                            xe[i] = new LSLVariable(atomToken.GetText());
                        }
                        throw new LSLListParserOptionsConstraintViolationException("Variable references are not allowed in Vector literals.");
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
                        xe[i] = BasicAtomToExpr(floatOrInt, operation);
                    }
                    else
                    {
                        throw new LSLListParserSyntaxException(
                            string.Format("The Negative and Positive prefix operator can only be used on Floats and Integers inside of a Vector, operator '{0}' is not valid.", operation));
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

            return new LSLVector((ILSLListExpr)xe[0], (ILSLListExpr)xe[1], (ILSLListExpr)xe[2]);
        }



        private static ILSLListExpr ListExpressionFromRotation(LSLListParsingFlags parsingFlags, LSLParser.Expr_AtomContext atomToken)
        {

            object[] xe =
            {
                atomToken.rotation_literal.rotation_x,
                atomToken.rotation_literal.rotation_y,
                atomToken.rotation_literal.rotation_z,
                atomToken.rotation_literal.rotation_s
            };

            for (int i = 0; i < xe.Length; i++)
            {
                var atom = xe[i] as LSLParser.Expr_AtomContext;
                var prefix = xe[i] as LSLParser.Expr_PrefixOperationContext;

                if (atom != null)
                {
                    if (atom.float_literal != null || atom.integer_literal != null || atom.hex_literal != null)
                    {
                        xe[i] = BasicAtomToExpr(atom);
                    }
                    else if (atom.variable != null)
                    {
                        if ((parsingFlags & LSLListParsingFlags.AllowVariableReferencesInRotations) == LSLListParsingFlags.AllowVariableReferencesInRotations)
                        {
                            xe[i] = new LSLVariable(atomToken.GetText());
                        }
                        throw new LSLListParserOptionsConstraintViolationException("Variable references are not allowed in Rotation literals.");
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
                        xe[i] = BasicAtomToExpr(floatOrInt, operation);
                    }
                    else
                    {
                        throw new LSLListParserSyntaxException(
                            string.Format("The Negative and Positive prefix operator can only be used on Floats and Integers inside of a Rotation, operator '{0}' is not valid.", operation));
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

            return new LSLRotation((ILSLListExpr)xe[0], (ILSLListExpr)xe[1], (ILSLListExpr)xe[2], (ILSLListExpr)xe[3]);
        }




        /// <summary>
        ///     Interface for parsed list elements.
        /// </summary>
        public interface ILSLListExpr
        {
            /// <summary>
            ///     True if this list item represents a variable reference.
            /// </summary>
            bool IsVariableReference { get; }

            /// <summary>
            ///     The list item type, it will be void if its a variable reference
            /// </summary>
            LSLType Type { get; }

            /// <summary>
            ///     Gets string representing the element, with quoting characters for the type.
            /// </summary>
            /// <value>
            ///     The value string.
            /// </value>
            string ValueString { get; }
        }

        /// <summary>
        ///     Vector list item.
        /// </summary>
        public class LSLVector : ILSLListExpr
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="LSLVector" /> class.
            /// </summary>
            /// <param name="x">The x component.</param>
            /// <param name="y">The y component.</param>
            /// <param name="z">The z component.</param>
            /// <exception cref="System.ArgumentException">
            ///     X is not an LSLFloat or LSLVariable.;x
            ///     or
            ///     Y is not an LSLFloat or LSLVariable.;y
            ///     or
            ///     Z is not an LSLFloat or LSLVariable.;z
            /// </exception>
            public LSLVector(ILSLListExpr x, ILSLListExpr y, ILSLListExpr z)
            {
                if (!(x is LSLFloat || x is LSLVariable))
                {
                    throw new ArgumentException("X is not an LSLFloat or LSLVariable.", "x");
                }

                if (!(y is LSLFloat || y is LSLVariable))
                {
                    throw new ArgumentException("Y is not an LSLFloat or LSLVariable.", "y");
                }

                if (!(z is LSLFloat || z is LSLVariable))
                {
                    throw new ArgumentException("Z is not an LSLFloat or LSLVariable.", "z");
                }

                X = x;
                Y = y;
                Z = z;
            }

            /// <summary>
            ///     The X component.
            /// </summary>
            public ILSLListExpr X { get; private set; }

            /// <summary>
            ///     The Y component.
            /// </summary>
            public ILSLListExpr Y { get; private set; }

            /// <summary>
            ///     The Z component.
            /// </summary>
            public ILSLListExpr Z { get; private set; }

            /// <summary>
            ///     True if this list item represents a variable reference.
            /// </summary>
            public bool IsVariableReference
            {
                get { return false; }
            }

            /// <summary>
            ///     The list item type, it will be void if its a variable reference
            /// </summary>
            public LSLType Type
            {
                get { return LSLType.Vector; }
            }

            /// <summary>
            ///     Gets string representing the element, with quoting characters for the type.
            /// </summary>
            /// <value>
            ///     The value string.
            /// </value>
            public string ValueString
            {
                get { return "<" + X.ValueString + ", " + Y.ValueString + ", " + Z.ValueString + ">"; }
            }
        }

        /// <summary>
        ///     Rotation list item.
        /// </summary>
        public class LSLRotation : ILSLListExpr
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="LSLRotation" /> class.
            /// </summary>
            /// <param name="x">The x component.</param>
            /// <param name="y">The y component.</param>
            /// <param name="z">The z component.</param>
            /// <param name="s">The s component.</param>
            /// <exception cref="System.ArgumentException">
            ///     X is not an LSLFloat or LSLVariable.;x
            ///     or
            ///     Y is not an LSLFloat or LSLVariable.;y
            ///     or
            ///     Z is not an LSLFloat or LSLVariable.;z
            ///     or
            ///     S is not an LSLFloat or LSLVariable.;s
            /// </exception>
            public LSLRotation(ILSLListExpr x, ILSLListExpr y, ILSLListExpr z, ILSLListExpr s)
            {
                if (!(x is LSLFloat || x is LSLVariable))
                {
                    throw new ArgumentException("X is not an LSLFloat or LSLVariable.", "x");
                }

                if (!(y is LSLFloat || y is LSLVariable))
                {
                    throw new ArgumentException("Y is not an LSLFloat or LSLVariable.", "y");
                }

                if (!(z is LSLFloat || z is LSLVariable))
                {
                    throw new ArgumentException("Z is not an LSLFloat or LSLVariable.", "z");
                }

                if (!(s is LSLFloat || s is LSLVariable))
                {
                    throw new ArgumentException("S is not an LSLFloat or LSLVariable.", "s");
                }


                X = x;
                Y = y;
                Z = z;
                S = s;
            }

            /// <summary>
            ///     The X component.
            /// </summary>
            public ILSLListExpr X { get; private set; }

            /// <summary>
            ///     The Y component.
            /// </summary>
            public ILSLListExpr Y { get; private set; }

            /// <summary>
            ///     The Z component.
            /// </summary>
            public ILSLListExpr Z { get; private set; }

            /// <summary>
            ///     The S component.
            /// </summary>
            public ILSLListExpr S { get; private set; }

            /// <summary>
            ///     True if this list item represents a variable reference.
            /// </summary>
            public bool IsVariableReference
            {
                get { return false; }
            }

            /// <summary>
            ///     The list item type, it will be void if its a variable reference
            /// </summary>
            public LSLType Type
            {
                get { return LSLType.Rotation; }
            }

            /// <summary>
            ///     Gets string representing the element, with quoting characters for the type.
            /// </summary>
            /// <value>
            ///     The value string.
            /// </value>
            public string ValueString
            {
                get { return "<" + X.ValueString + ", " + Y.ValueString + ", " + Z.ValueString + ", " + S.ValueString+ ">"; }
            }
        }

        /// <summary>
        ///     Float list item.
        /// </summary>
        public class LSLFloat : ILSLListExpr
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="LSLFloat" /> class.
            /// </summary>
            /// <param name="val">The value.</param>
            /// <param name="hex">if set to <c>true</c> val is parsed from hexadecimal notation.</param>
            public LSLFloat(string val, bool hex = false)
            {
                Value = hex ? Convert.ToInt32(val, 16) : float.Parse(val);
            }

            /// <summary>
            ///     The float value
            /// </summary>
            public float Value { get; private set; }

            /// <summary>
            ///     True if this list item represents a variable reference.
            /// </summary>
            public bool IsVariableReference
            {
                get { return false; }
            }

            /// <summary>
            ///     The list item type, it will be void if its a variable reference
            /// </summary>
            public LSLType Type
            {
                get { return LSLType.Float; }
            }

            /// <summary>
            ///     Gets string representing the element, with quoting characters for the type.
            /// </summary>
            /// <value>
            ///     The value string.
            /// </value>
            public string ValueString
            {
                get { return Value.ToString(CultureInfo.InvariantCulture); }
            }
        }

        /// <summary>
        ///     Integer list item.
        /// </summary>
        public class LSLInteger : ILSLListExpr
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="LSLInteger" /> class.
            /// </summary>
            /// <param name="val">The value.</param>
            /// <param name="hex">if set to <c>true</c> [hexadecimal].</param>
            public LSLInteger(string val, bool hex = false)
            {
                Value = hex ? Convert.ToInt32(val, 16) : int.Parse(val);
            }

            /// <summary>
            ///     The integer value.
            /// </summary>
            public int Value { get; private set; }

            /// <summary>
            ///     True if this list item represents a variable reference.
            /// </summary>
            public bool IsVariableReference
            {
                get { return false; }
            }

            /// <summary>
            ///     The list item type, it will be void if its a variable reference
            /// </summary>
            public LSLType Type
            {
                get { return LSLType.Integer; }
            }

            /// <summary>
            ///     Gets string representing the element, with quoting characters for the type.
            /// </summary>
            /// <value>
            ///     The value string.
            /// </value>
            public string ValueString
            {
                get { return Value.ToString(CultureInfo.InvariantCulture); }
            }
        }

        /// <summary>
        ///     Key List item, they can be created by specifying '(key)""' as  list item, using a cast expression.
        /// </summary>
        public class LSLKey : ILSLListExpr
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="LSLKey" /> class.
            /// </summary>
            /// <param name="val">The value.</param>
            public LSLKey(string val)
            {
                Value = val;
            }

            /// <summary>
            ///     The raw value of the key, without quotes.
            /// </summary>
            public string Value { get; private set; }

            /// <summary>
            ///     True if this list item represents a variable reference.
            /// </summary>
            public bool IsVariableReference
            {
                get { return false; }
            }

            /// <summary>
            ///     The list item type, it will be void if its a variable reference
            /// </summary>
            public LSLType Type
            {
                get { return LSLType.Key; }
            }

            /// <summary>
            ///     Gets string representing the element, with quoting characters for the type.
            /// </summary>
            /// <value>
            ///     The value string.
            /// </value>
            public string ValueString
            {
                get { return Value; }
            }
        }

        /// <summary>
        ///     String list item.
        /// </summary>
        public class LSLString : ILSLListExpr
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="LSLString" /> class.
            /// </summary>
            /// <param name="val">The value.</param>
            public LSLString(string val)
            {
                Value = val;
            }

            /// <summary>
            ///     The raw value of the string, without quotes.
            /// </summary>
            public string Value { get; private set; }

            /// <summary>
            ///     True if this list item represents a variable reference.
            /// </summary>
            public bool IsVariableReference
            {
                get { return false; }
            }

            /// <summary>
            ///     The list item type, it will be void if its a variable reference
            /// </summary>
            public LSLType Type
            {
                get { return LSLType.String; }
            }

            /// <summary>
            ///     Gets string representing the element, with quoting characters for the type.
            /// </summary>
            /// <value>
            ///     The value string.
            /// </value>
            public string ValueString
            {
                get { return Value; }
            }
        }

        /// <summary>
        ///     Variable reference list item.
        /// </summary>
        public class LSLVariable : ILSLListExpr
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="LSLVariable" /> class.
            /// </summary>
            /// <param name="name">The name.</param>
            public LSLVariable(string name)
            {
                Name = name;
            }

            /// <summary>
            ///     Name of the variable referenced
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            ///     True if this list item represents a variable reference.
            /// </summary>
            public bool IsVariableReference
            {
                get { return true; }
            }

            /// <summary>
            ///     The list item type, it will be void if its a variable reference
            /// </summary>
            public LSLType Type
            {
                get { return LSLType.Void; }
            }

            /// <summary>
            ///     Gets string representing the element, with quoting characters for the type.
            /// </summary>
            /// <value>
            ///     The value string.
            /// </value>
            public string ValueString
            {
                get { return Name; }
            }
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