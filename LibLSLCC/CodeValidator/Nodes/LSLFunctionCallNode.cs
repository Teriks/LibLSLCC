#region FileInfo

// 
// File: LSLFunctionCallNode.cs
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
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.AntlrParser;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLFunctionCallNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLFunctionCallNode : ILSLFunctionCallNode, ILSLExprNode
    {
        private readonly bool _libraryFunction;
        private ILSLSyntaxTreeNode _parent;
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLFunctionCallNode(LSLSourceCodeRange sourceCodeRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceCodeRange;
            HasErrors = true;
        }


        /// <summary>
        ///     Construct an <see cref="LSLFunctionCallNode" /> with an arguments list and definition reference.
        ///     This represents a call to a user defined function.  <paramref name="definition" /> receives this node
        ///     as a new reference via <see cref="LSLFunctionDeclarationNode.AddReference" />.
        /// </summary>
        /// <param name="argumentList">The argument list node.</param>
        /// <param name="definition">The function definition node.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="definition" /> or <paramref name="argumentList" /> is
        ///     <c>null</c>.
        /// </exception>
        public LSLFunctionCallNode(LSLFunctionDeclarationNode definition, LSLExpressionListNode argumentList)
        {
            if (definition == null) throw new ArgumentNullException("definition");
            if (argumentList == null) throw new ArgumentNullException("argumentList");


            Name = definition.Name;

            ArgumentExpressionList = argumentList;
            ArgumentExpressionList.Parent = this;

            Definition = definition;
            Definition.AddReference(this);

            Signature = definition.CreateSignature();
        }


        /// <summary>
        ///     Construct an <see cref="LSLFunctionCallNode" /> with an arguments list and definition reference.
        ///     This represents a call to a user defined function.  <paramref name="definition" /> receives this node
        ///     as a new reference via <see cref="LSLFunctionDeclarationNode.AddReference" />.
        /// </summary>
        /// <param name="argumentList">The list of expression arguments.</param>
        /// <param name="definition">The function definition node.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="definition" /> or <paramref name="argumentList" /> is
        ///     <c>null</c>.
        /// </exception>
        public LSLFunctionCallNode(LSLFunctionDeclarationNode definition, params ILSLExprNode[] argumentList)
            : this(definition, new LSLExpressionListNode(argumentList))
        {
            
        }


        /// <summary>
        ///     Construct an <see cref="LSLFunctionCallNode" /> with an arguments list.
        ///     This represents a call to a library function, since it has no definition node.
        /// </summary>
        /// <param name="functionSignature">The signature of the library function.</param>
        /// <param name="argumentList">The argument list node.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="functionSignature" /> or <paramref name="argumentList" /> is
        ///     <c>null</c>.
        /// </exception>
        public LSLFunctionCallNode(LSLFunctionSignature functionSignature, LSLExpressionListNode argumentList)
        {
            if (functionSignature == null) throw new ArgumentNullException("functionSignature");
            if (argumentList == null) throw new ArgumentNullException("argumentList");


            Signature = new LSLFunctionSignature(functionSignature);

            Name = functionSignature.Name;

            ArgumentExpressionList = argumentList;
            ArgumentExpressionList.Parent = this;

            _libraryFunction = true;
        }




        /// <summary>
        ///     Construct an <see cref="LSLFunctionCallNode" /> with an arguments list.
        ///     This represents a call to a library function, since it has no definition node.
        /// </summary>
        /// <param name="functionSignature">The signature of the library function.</param>
        /// <param name="argumentList">The list of expression arguments.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="functionSignature" /> or <paramref name="argumentList" /> is
        ///     <c>null</c>.
        /// </exception>
        public LSLFunctionCallNode(LSLFunctionSignature functionSignature, params ILSLExprNode[] argumentList)
            : this(functionSignature, new LSLExpressionListNode(argumentList))
        {
        }


        /// <exception cref="ArgumentNullException">
        ///     <paramref name="context" /> or <paramref name="preDefinition" /> or
        ///     <paramref name="argumentExpressionList" /> is <c>null</c>.
        /// </exception>
        internal LSLFunctionCallNode(
            LSLParser.Expr_FunctionCallContext context,
            LSLPreDefinedFunctionSignature preDefinition,
            LSLExpressionListNode argumentExpressionList)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (preDefinition == null)
            {
                throw new ArgumentNullException("preDefinition");
            }

            if (argumentExpressionList == null)
            {
                throw new ArgumentNullException("argumentExpressionList");
            }

            Definition = preDefinition.DefinitionNode;
            Signature = preDefinition;

            Name = context.function_name.Text;

            ArgumentExpressionList = argumentExpressionList;

            argumentExpressionList.Parent = this;

            SourceRange = new LSLSourceCodeRange(context);
            SourceRangeOpenParenth = new LSLSourceCodeRange(context.open_parenth);
            SourceRangeCloseParenth = new LSLSourceCodeRange(context.close_parenth);
            SourceRangeName = new LSLSourceCodeRange(context.function_name);

            SourceRangesAvailable = true;
        }


        /// <exception cref="ArgumentNullException">
        ///     <paramref name="context" /> or <paramref name="signature" /> or
        ///     <paramref name="argumentExpressionList" /> is <c>null</c>.
        /// </exception>
        internal LSLFunctionCallNode(
            LSLParser.Expr_FunctionCallContext context,
            LSLFunctionSignature signature,
            LSLExpressionListNode argumentExpressionList)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (signature == null)
            {
                throw new ArgumentNullException("signature");
            }

            if (argumentExpressionList == null)
            {
                throw new ArgumentNullException("argumentExpressionList");
            }


            Signature = signature;

            Name = context.function_name.Text;

            _libraryFunction = true;
            ArgumentExpressionList = argumentExpressionList;
            argumentExpressionList.Parent = this;

            SourceRange = new LSLSourceCodeRange(context);
            SourceRangeOpenParenth = new LSLSourceCodeRange(context.open_parenth);
            SourceRangeCloseParenth = new LSLSourceCodeRange(context.close_parenth);
            SourceRangeName = new LSLSourceCodeRange(context.function_name);

            SourceRangesAvailable = true;
        }


        /// <summary>
        ///     Create an <see cref="LSLFunctionCallNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        public LSLFunctionCallNode(LSLFunctionCallNode other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }


            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
                SourceRangeOpenParenth = other.SourceRangeOpenParenth;
                SourceRangeCloseParenth = other.SourceRangeCloseParenth;
                SourceRangeName = other.SourceRangeName;
            }

            Name = other.Name;

            Definition = other.Definition;
            Signature = new LSLFunctionSignature(other.Signature);

            _libraryFunction = other._libraryFunction;


            ArgumentExpressionList = other.ArgumentExpressionList.Clone();
            ArgumentExpressionList.Parent = this;

            HasErrors = other.HasErrors;
        }


        /// <summary>
        ///     The parameter list node containing the expressions used to call this function, this will never be null even if the
        ///     parameter list is empty.
        /// </summary>
        public LSLExpressionListNode ArgumentExpressionList { get; private set; }

        /// <summary>
        ///     The syntax tree node where the function was defined if it is a user defined function.  If the function call is to a
        ///     library function this will be null.
        /// </summary>
        public LSLFunctionDeclarationNode Definition { get; private set; }

        /// <summary>
        ///     True if the function that was called is a library function call, false if it was a call to a user defined function.
        /// </summary>
        public bool IsLibraryFunctionCall
        {
            get { return _libraryFunction; }
        }

        /// <summary>
        ///     The source code range of the opening parentheses where the parameters of the function start.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeOpenParenth { get; private set; }

        /// <summary>
        ///     The source code range of the closing parentheses where the parameters of the function end.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeCloseParenth { get; private set; }

        /// <summary>
        ///     The source code range of the function name in the function call expression.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeName { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        ///     The name of the function that was called.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     The function signature of the function that was called, as it was defined by either the user or library.
        /// </summary>
        public LSLFunctionSignature Signature { get; private set; }

        ILSLExpressionListNode ILSLFunctionCallNode.ArgumentExpressionList
        {
            get { return ArgumentExpressionList; }
        }

        ILSLFunctionDeclarationNode ILSLFunctionCallNode.Definition
        {
            get { return Definition; }
        }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceCodeRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static LSLFunctionCallNode GetError(LSLSourceCodeRange sourceCodeRange)
        {
            return new LSLFunctionCallNode(sourceCodeRange, Err.Err);
        }

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLExprNode Members

        /// <summary>
        ///     Deep clones the expression node.  It should clone the node and all of its children and cloneable properties, except
        ///     the parent.
        ///     When cloned, the parent node reference should be left <c>null</c>.
        /// </summary>
        /// <returns>A deep clone of this expression tree node.</returns>
        public LSLFunctionCallNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLFunctionCallNode(this);
        }


        ILSLExprNode ILSLExprNode.Clone()
        {
            return Clone();
        }


        /// <summary>
        ///     The parent node of this syntax tree node.
        /// </summary>
        /// <exception cref="InvalidOperationException" accessor="set">If Parent has already been set.</exception>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value" /> is <see langword="null" />.</exception>
        public ILSLSyntaxTreeNode Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                {
                    throw new InvalidOperationException(GetType().Name +
                                                        ": Parent node already set, it can only be set once.");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value", GetType().Name + ": Parent cannot be set to null.");
                }

                _parent = value;
            }
        }


        /// <summary>
        ///     True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; internal set; }


        /// <summary>
        ///     The source code range that this syntax tree node occupies.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRange { get; private set; }


        /// <summary>
        ///     Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceRangesAvailable { get; private set; }


        /// <summary>
        ///     Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}" />
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="visitor"/> is <see langword="null" />.</exception>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");

            if (ExpressionType == LSLExpressionType.LibraryFunction)
            {
                return visitor.VisitLibraryFunctionCall(this);
            }

            if (ExpressionType == LSLExpressionType.UserFunction)
            {
                return visitor.VisitUserFunctionCall(this);
            }

            throw new InvalidOperationException(
                typeof (LSLFunctionCallNode).Name + " could not be visited, object is in an invalid state");
        }


        /// <summary>
        ///     The return type of the expression. see: <see cref="LSLType" />
        /// </summary>
        public LSLType Type
        {
            get { return Signature.ReturnType; }
        }


        /// <summary>
        ///     The expression type/classification of the expression. see: <see cref="LSLExpressionType" />
        /// </summary>
        /// <value>
        ///     The type of the expression.
        /// </value>
        public LSLExpressionType ExpressionType
        {
            get { return _libraryFunction ? LSLExpressionType.LibraryFunction : LSLExpressionType.UserFunction; }
        }

        /// <summary>
        ///     True if the expression is constant and can be calculated at compile time.
        /// </summary>
        public bool IsConstant
        {
            get { return false; }
        }


        /// <summary>
        ///     True if the expression statement has some modifying effect on a local parameter or global/local variable;  or is a
        ///     function call.  False otherwise.
        /// </summary>
        public bool HasPossibleSideEffects
        {
            get
            {
                //function calls can ALWAYS have some sort of side effect on the program state.
                return true;
            }
        }


        /// <summary>
        ///     Should produce a user friendly description of the expressions return type.
        ///     This is used in some syntax error messages, Ideally you should enclose your description in
        ///     parenthesis or something that will make it stand out in a string.
        /// </summary>
        /// <returns></returns>
        public string DescribeType()
        {
            return "(" + Type + (this.IsLiteral() ? " Literal)" : ")");
        }


        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }

        #endregion
    }
}