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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Collections;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    /// <summary>
    /// Default <see cref="ILSLEventHandlerNode"/> implementation used by <see cref="LSLCodeValidator"/>
    /// </summary>
    public sealed class LSLEventHandlerNode : ILSLEventHandlerNode, ILSLSyntaxTreeNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLEventHandlerNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
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

            Name = context.handler_name.Text;

            EventBodyNode = eventBodyNode;
            EventBodyNode.Parent = this;

            ParameterListNode = parameterListNode;
            ParameterListNode.Parent = this;


            SourceRange = new LSLSourceCodeRange(context);
            SourceRangeName = new LSLSourceCodeRange(context.handler_name);

            SourceRangesAvailable = true;
        }

        /// <summary>
        /// An in order list of parameter nodes that belong to the event handler, or an empty enumerable if none exist.
        /// </summary>
        public IReadOnlyGenericArray<LSLParameterNode> ParameterNodes
        {
            get { return ParameterListNode.Parameters; }
        }

        /// <summary>
        /// The code scope node that represents the code body of the event handler.
        /// </summary>
        public LSLCodeScopeNode EventBodyNode { get; private set; }


        /// <summary>
        /// The parameter list node for the parameters of the event handler.  This is not null even when no parameters exist.
        /// It can be null if there are errors in the event handler node that prevent the parameters from being parsed.
        /// Ideally you should not be handling a syntax tree with syntax errors in it.
        /// </summary>
        public LSLParameterListNode ParameterListNode { get; private set; }

        /// <summary>
        /// The source code range of the event handler name.
        /// </summary>
        public LSLSourceCodeRange SourceRangeName { get; private set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        /// The name of the event handler.
        /// </summary>
        public string Name { get; private set; }


        ILSLCodeScopeNode ILSLEventHandlerNode.EventBodyNode
        {
            get { return EventBodyNode; }
        }

        ILSLParameterListNode ILSLEventHandlerNode.ParameterListNode
        {
            get { return ParameterListNode; }
        }


        /// <summary>
        /// Returns a version of this node type that represents its error state;  in case of a syntax error
        /// in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLEventHandlerNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLEventHandlerNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members


        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceRange { get; private set; }



        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceRangesAvailable { get; private set; }


        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; internal set; }


        /// <summary>
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitEventHandler(this);
        }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }

        #endregion
    }
}