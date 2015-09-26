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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes
{
    public class LSLFunctionDeclarationNode : ILSLFunctionDeclarationNode, ILSLSyntaxTreeNode
    {
        private readonly List<LSLFunctionCallNode> _references = new List<LSLFunctionCallNode>();
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLFunctionDeclarationNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

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

            ParserContext = context;
            ParameterListNode = parameterListNode;
            FunctionBodyNode = functionBodyNode;


            FunctionBodyNode.Parent = this;

            ParameterListNode.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }

        internal LSLParser.FunctionDeclarationContext ParserContext { get; }

        public IReadOnlyList<LSLParameterNode> ParameterNodes
        {
            get { return ParameterListNode.Parameters; }
        }

        public IReadOnlyList<LSLFunctionCallNode> References
        {
            get { return _references; }
        }

        public LSLParameterListNode ParameterListNode { get; set; }
        public LSLCodeScopeNode FunctionBodyNode { get; }

        IReadOnlyList<ILSLFunctionCallNode> ILSLFunctionDeclarationNode.References
        {
            get { return _references; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public bool HasParameters
        {
            get { return ParameterListNode.Parameters.Any(); }
        }

        IReadOnlyList<ILSLParameterNode> ILSLFunctionDeclarationNode.ParameterNodes
        {
            get { return ParameterListNode.Parameters; }
        }

        public string ReturnTypeString
        {
            get
            {
                if (ParserContext.return_type == null) return "";

                return ParserContext.return_type.Text;
            }
        }

        public string Name
        {
            get { return ParserContext.function_name.Text; }
        }

        public LSLType ReturnType
        {
            get
            {
                if (ParserContext.return_type == null) return LSLType.Void;

                return LSLTypeTools.FromLSLTypeString(ReturnTypeString);
            }
        }

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

        public static
            LSLFunctionDeclarationNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLFunctionDeclarationNode(sourceRange, Err.Err);
        }

        public LSLFunctionSignature ToSignature()
        {
            return new LSLFunctionSignature(ReturnType, Name,
                ParameterListNode.Parameters.Select(x => new LSLParameter(x.Type, x.Name, false)));
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitFunctionDeclaration(this);
        }


        public ILSLSyntaxTreeNode Parent { get; set; }

        #endregion
    }
}