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
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Collections;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    /// <summary>
    /// Default <see cref="ILSLFunctionDeclarationNode"/> implementation used by <see cref="LSLCodeValidator"/>
    /// </summary>
    public sealed class LSLFunctionDeclarationNode : ILSLFunctionDeclarationNode, ILSLSyntaxTreeNode
    {
        private readonly GenericArray<LSLFunctionCallNode> _references = new GenericArray<LSLFunctionCallNode>();
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLFunctionDeclarationNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }

        /// <exception cref="ArgumentNullException"><paramref name="parameterListNode"/> or <paramref name="functionBodyNode"/> is <c>null</c>.</exception>
        internal LSLFunctionDeclarationNode(LSLParser.FunctionDeclarationContext context,
            LSLParameterListNode parameterListNode, LSLCodeScopeNode functionBodyNode)
        {
            if (parameterListNode == null)
            {
                throw new ArgumentNullException("parameterListNode");
            }

            if (functionBodyNode == null)
            {
                throw new ArgumentNullException("functionBodyNode");
            }

            if (context.return_type != null)
            {
                ReturnTypeString = context.return_type.Text;
                ReturnType = LSLTypeTools.FromLSLTypeString(ReturnTypeString);
            }
            else
            {
                ReturnTypeString = "";
                ReturnType = LSLType.Void;
            }

            Name = context.function_name.Text;


            ParameterListNode = parameterListNode;
            ParameterListNode.Parent = this;

            FunctionBodyNode = functionBodyNode;
            FunctionBodyNode.Parent = this;

            SourceRange = new LSLSourceCodeRange(context);
            SourceRangeName = new LSLSourceCodeRange(context.function_name);

            SourceRangesAvailable = true;
        }


        /// <summary>
        /// The source code range of the function name.
        /// </summary>
        public LSLSourceCodeRange SourceRangeName { get; private set; }

        /// <summary>
        /// <see cref="ILSLParameterListNode.Parameters"/> taken from <see cref="ParameterListNode"/>
        /// </summary>
        public IReadOnlyGenericArray<LSLParameterNode> ParameterNodes
        {
            get { return ParameterListNode.Parameters; }
        }


        /// <summary>
        /// A list of function call nodes that reference this function definition, or an empty list.
        /// </summary>
        public IReadOnlyGenericArray<LSLFunctionCallNode> References
        {
            get { return _references; }
        }

        /// <summary>
        /// The parameter list node that contains the parameter list definitions for this function.
        /// It should never be null, even if the function definition contains no parameter definitions.
        /// </summary>
        public LSLParameterListNode ParameterListNode { get; set; }

        /// <summary>
        /// The code scope node that represents the code body of the function definition.
        /// </summary>
        public LSLCodeScopeNode FunctionBodyNode { get; private set; }

        IReadOnlyGenericArray<ILSLFunctionCallNode> ILSLFunctionDeclarationNode.References
        {
            get { return _references; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        /// The string from the source code that represents the return type assigned to the function definition,
        /// or an empty string if no return type was assigned.
        /// </summary>
        public string ReturnTypeString { get; private set; }

        /// <summary>
        /// The name of the function.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The return type assigned to the function definition, it will be <see cref="LSLType.Void"/> if no return type was given.
        /// </summary>
        public LSLType ReturnType { get; private set; }

        ILSLParameterListNode ILSLFunctionDeclarationNode.ParameterListNode
        {
            get { return ParameterListNode; }
        }

        ILSLCodeScopeNode ILSLFunctionDeclarationNode.FunctionBodyNode
        {
            get { return FunctionBodyNode; }
        }

        internal void AddReference(LSLFunctionCallNode reference)
        {
            _references.Add(reference);
        }


        /// <summary>
        /// Returns a version of this node type that represents its error state;  in case of a syntax error
        /// in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLFunctionDeclarationNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLFunctionDeclarationNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; internal set; }


        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceRange { get; private set; }

        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceRangesAvailable { get; private set; }


        /// <summary>
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitFunctionDeclaration(this);
        }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }

        #endregion
    }
}