#region FileInfo

// 
// File: LSLFunctionDeclarationNode.cs
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
using System.Linq;
using LibLSLCC.AntlrParser;
using LibLSLCC.Collections;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLFunctionDeclarationNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLFunctionDeclarationNode : ILSLFunctionDeclarationNode, ILSLSyntaxTreeNode
    {
        private readonly GenericArray<LSLFunctionCallNode> _references = new GenericArray<LSLFunctionCallNode>();
        private ILSLSyntaxTreeNode _parent;
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLFunctionDeclarationNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /*
        /// <summary>
        ///     Create an <see cref="LSLFunctionDeclarationNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        public LSLFunctionDeclarationNode(LSLFunctionDeclarationNode other)
        {
            if (other == null) throw new ArgumentNullException("other");


            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
                SourceRangeName = other.SourceRangeName;
                SourceRangeReturnType = other.SourceRangeReturnType;
                SourceRangesAvailable = other.SourceRangesAvailable;
            }


            Name = other.Name;

            ParameterList = other.ParameterList.Clone();
            ParameterList.Parent = this;

            Code = other.Code.Clone();
            Code.Parent = this;


            HasErrors = other.HasErrors;
        }*/


        /// <summary>
        ///     Construct an <see cref="LSLFunctionDeclarationNode" /> with the given return type, and code body.
        ///     The function declaration will have an empty parameter list node.
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     if <paramref name="functionName" /> contains invalid characters for an LSL ID
        ///     token.
        /// </exception>
        public LSLFunctionDeclarationNode(LSLType returnType, string functionName, LSLCodeScopeNode code)
            : this(returnType, functionName, new LSLParameterListNode(), code)
        {
        }


        /// <summary>
        ///     Construct an <see cref="LSLFunctionDeclarationNode" /> with the given return type, parameter list, and code body.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="functionName" /> or <paramref name="parameterList" /> or <paramref name="code" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="LSLInvalidSymbolNameException">
        ///     if <paramref name="functionName" /> contains invalid characters for an LSL ID
        ///     token.
        /// </exception>
        public LSLFunctionDeclarationNode(LSLType returnType, string functionName,
            LSLParameterListNode parameterList, LSLCodeScopeNode code)
        {
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            if (code == null)
            {
                throw new ArgumentNullException("code");
            }
            if (functionName == null)
            {
                throw new ArgumentNullException("functionName");
            }

            if (!LSLTokenTools.IDRegexAnchored.IsMatch(functionName))
            {
                throw new LSLInvalidSymbolNameException(
                    "functionName provided contained characters not allowed in an LSL ID token.");
            }

            Name = functionName;


            ReturnTypeName = returnType == LSLType.Void ? "" : LSLTypeTools.ToLSLTypeName(returnType);
            ReturnType = returnType;


            ParameterList = parameterList;
            ParameterList.Parent = this;

            Code = code;
            Code.Parent = this;
            Code.CodeScopeType = LSLCodeScopeType.Function;
        }


        /// <exception cref="ArgumentNullException">
        ///     <paramref name="parameterList" /> or <paramref name="code" />
        ///     is <c>null</c>.
        /// </exception>
        internal LSLFunctionDeclarationNode(LSLParser.FunctionDeclarationContext context,
            LSLParameterListNode parameterList, LSLCodeScopeNode code)
        {
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }

            if (code == null)
            {
                throw new ArgumentNullException("code");
            }

            if (context.return_type != null)
            {
                ReturnTypeName = context.return_type.Text;
                ReturnType = LSLTypeTools.FromLSLTypeName(ReturnTypeName);
                SourceRangeReturnType = new LSLSourceCodeRange(context.return_type);
            }
            else
            {
                ReturnTypeName = "";
                ReturnType = LSLType.Void;
            }

            Name = context.function_name.Text;


            ParameterList = parameterList;
            ParameterList.Parent = this;

            Code = code;
            Code.Parent = this;
            Code.CodeScopeType = LSLCodeScopeType.Function;

            SourceRange = new LSLSourceCodeRange(context);
            SourceRangeName = new LSLSourceCodeRange(context.function_name);

            SourceRangesAvailable = true;
        }


        /// <summary>
        ///     A list of function call nodes that reference this function definition, or an empty list.
        /// </summary>
        public IReadOnlyGenericArray<LSLFunctionCallNode> References
        {
            get { return _references; }
        }

        /// <summary>
        ///     The parameter list node that contains the parameter list definitions for this function.
        ///     It should never be null, even if the function definition contains no parameter definitions.
        /// </summary>
        public LSLParameterListNode ParameterList { get; set; }

        /// <summary>
        ///     The code scope node that represents the code body of the function definition.
        /// </summary>
        public LSLCodeScopeNode Code { get; private set; }

        /// <summary>
        ///     The source code range of the function name.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeName { get; private set; }

        /// <summary>
        ///     The source code range of the function return type, or <c>null</c> if no return type was specified.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeReturnType { get; private set; }

        IReadOnlyGenericArray<ILSLFunctionCallNode> ILSLFunctionDeclarationNode.References
        {
            get { return _references; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        ///     The string from the source code that represents the return type assigned to the function definition,
        ///     or an empty string if no return type was assigned.
        /// </summary>
        public string ReturnTypeName { get; private set; }

        /// <summary>
        ///     The name of the function.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     The return type assigned to the function definition, it will be <see cref="LSLType.Void" /> if no return type was
        ///     given.
        /// </summary>
        public LSLType ReturnType { get; private set; }

        ILSLParameterListNode ILSLFunctionDeclarationNode.ParameterList
        {
            get { return ParameterList; }
        }

        ILSLCodeScopeNode ILSLFunctionDeclarationNode.Code
        {
            get { return Code; }
        }


        internal void AddReference(LSLFunctionCallNode reference)
        {
            _references.Add(reference);
        }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLFunctionDeclarationNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLFunctionDeclarationNode(sourceRange, Err.Err);
        }


        /// <summary>
        ///     Build a <see cref="LSLFunctionSignature" /> object based off the signature of this function declaration node.
        /// </summary>
        /// <returns>The created <see cref="LSLFunctionSignature" />.</returns>
        public LSLFunctionSignature CreateSignature()
        {
            return new LSLFunctionSignature(ReturnType, Name,
                ParameterList.Parameters.Select(x => new LSLParameterSignature(x.Type, x.Name, false)));
        }

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        /// <summary>
        ///     True if this syntax tree node contains syntax errors. <para/>
        ///     <see cref="SourceRange"/> should point to a more specific error location when this is <c>true</c>. <para/>
        ///     Other source ranges will not be available.
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

            return visitor.VisitFunctionDeclaration(this);
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

        #endregion
    }
}