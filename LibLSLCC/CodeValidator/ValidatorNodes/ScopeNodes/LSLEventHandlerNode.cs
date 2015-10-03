#region FileInfo
// 
// File: LSLEventHandlerNode.cs
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
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes
{
    public class LSLEventHandlerNode : ILSLEventHandlerNode, ILSLSyntaxTreeNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLEventHandlerNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLEventHandlerNode(LSLParser.EventHandlerContext context, LSLParameterListNode parameterListNode,
            LSLCodeScopeNode eventBodyNode)
        {
            if (parameterListNode == null)
            {
                throw new ArgumentNullException("parameterListNode");
            }

            if (eventBodyNode == null)
            {
                throw new ArgumentNullException("eventBodyNode");
            }

            ParserContext = context;
            EventBodyNode = eventBodyNode;
            ParameterListNode = parameterListNode;

            EventBodyNode.Parent = this;
            ParameterListNode.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }

        internal LSLParser.EventHandlerContext ParserContext { get; private set; }

        public IReadOnlyList<LSLParameterNode> ParameterNodes
        {
            get { return ParameterListNode.Parameters; }
        }

        public LSLCodeScopeNode EventBodyNode { get; private set; }
        public LSLParameterListNode ParameterListNode { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public string Name
        {
            get { return ParserContext.handler_name.Text; }
        }

        public bool HasParameterNodes
        {
            get { return ParameterListNode.Parameters.Any(); }
        }

        IReadOnlyList<ILSLParameterNode> ILSLEventHandlerNode.ParameterNodes
        {
            get { return ParameterNodes; }
        }

        ILSLCodeScopeNode ILSLEventHandlerNode.EventBodyNode
        {
            get { return EventBodyNode; }
        }

        ILSLParameterListNode ILSLEventHandlerNode.ParameterListNode
        {
            get { return ParameterListNode; }
        }

        public LSLEventSignature ToSignature()
        {
            return new LSLEventSignature(Name,
                ParameterListNode.Parameters.Select(x => new LSLParameter(x.Type, x.Name, false)));
        }

        public static
            LSLEventHandlerNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLEventHandlerNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        public LSLSourceCodeRange SourceCodeRange { get; set; }


        public bool HasErrors { get; set; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitEventHandler(this);
        }


        public ILSLSyntaxTreeNode Parent { get; set; }

        #endregion
    }
}