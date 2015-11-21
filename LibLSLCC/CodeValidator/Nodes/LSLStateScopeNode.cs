#region FileInfo
// 
// File: LSLStateScopeNode.cs
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
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Collections;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    public class LSLStateScopeNode : ILSLStateScopeNode, ILSLSyntaxTreeNode
    {
        private readonly GenericArray<LSLEventHandlerNode> _eventHandlers = new GenericArray<LSLEventHandlerNode>();

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        // ReSharper disable UnusedParameter.Local
        protected LSLStateScopeNode(LSLSourceCodeRange sourceRange, Err err)
            // ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLStateScopeNode(LSLParser.DefaultStateContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }


            StateName = "default";
            DefaultStateContext = context;
            IsDefaultState = true;
            SourceCodeRange = new LSLSourceCodeRange(context);

            OpenBraceSourceCodeRange = new LSLSourceCodeRange(context.open_brace);
            CloseBraceSourceCodeRange = new LSLSourceCodeRange(context.close_brace);
            StateNameSourceCodeRange = new LSLSourceCodeRange(context.state_name);
            StateKeywordSourceCodeRange = new LSLSourceCodeRange(context.state_name);

            SourceCodeRangesAvailable = true;
        }

        internal LSLStateScopeNode(LSLParser.DefinedStateContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }


            StateName = context.state_name.Text;
            IsDefinedState = true;
            DefinedStateContext = context;
            SourceCodeRange = new LSLSourceCodeRange(context);

            OpenBraceSourceCodeRange = new LSLSourceCodeRange(context.open_brace);
            CloseBraceSourceCodeRange = new LSLSourceCodeRange(context.close_brace);
            StateNameSourceCodeRange = new LSLSourceCodeRange(context.state_name);
            StateKeywordSourceCodeRange = new LSLSourceCodeRange(context.state_keyword);

            SourceCodeRangesAvailable = true;
        }

        internal LSLStateScopeNode(LSLParser.DefaultStateContext context, IEnumerable<LSLEventHandlerNode> eventHandlers)
            : this(context)

        {
            if (eventHandlers == null)
            {
                throw new ArgumentNullException("eventHandlers");
            }


            foreach (var lslEventHandlerNode in eventHandlers)
            {
                AddEventHandler(lslEventHandlerNode);
            }

            SourceCodeRange = new LSLSourceCodeRange(context);

            OpenBraceSourceCodeRange = new LSLSourceCodeRange(context.open_brace);
            CloseBraceSourceCodeRange = new LSLSourceCodeRange(context.close_brace);
            StateNameSourceCodeRange = new LSLSourceCodeRange(context.state_name);
            StateKeywordSourceCodeRange = new LSLSourceCodeRange(context.state_name);

            SourceCodeRangesAvailable = true;
        }

        internal LSLStateScopeNode(LSLParser.DefinedStateContext context, IEnumerable<LSLEventHandlerNode> eventHandlers)
            : this(context)
        {
            if (eventHandlers == null)
            {
                throw new ArgumentNullException("eventHandlers");
            }


            foreach (var lslEventHandlerNode in eventHandlers)
            {
                AddEventHandler(lslEventHandlerNode);
            }

            SourceCodeRange = new LSLSourceCodeRange(context);

            OpenBraceSourceCodeRange = new LSLSourceCodeRange(context.open_brace);
            CloseBraceSourceCodeRange = new LSLSourceCodeRange(context.close_brace);
            StateNameSourceCodeRange = new LSLSourceCodeRange(context.state_name);
            StateKeywordSourceCodeRange = new LSLSourceCodeRange(context.state_keyword);

            SourceCodeRangesAvailable = true;
        }

        internal LSLParser.DefinedStateContext DefinedStateContext { get; private set; }
        internal LSLParser.DefaultStateContext DefaultStateContext { get; private set; }

        public IReadOnlyGenericArray<LSLEventHandlerNode> EventHandlers
        {
            get { return _eventHandlers; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        /// The name of the state block in the source code.
        /// 'default' should be returned if the node represents the default state.
        /// </summary>
        public string StateName { get; private set; }


        /// <summary>
        /// True if this state scope node represents a user defined state,  False if it is the 'default' state.
        /// </summary>
        public bool IsDefinedState { get; private set; }


        /// <summary>
        /// True if this state scope node represents the 'default' state,  False if it is a user defined state.
        /// </summary>
        public bool IsDefaultState { get; private set; }



        IReadOnlyGenericArray<ILSLEventHandlerNode> ILSLStateScopeNode.EventHandlers
        {
            get { return _eventHandlers; }
        }

        public static
            LSLStateScopeNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLStateScopeNode(sourceRange, Err.Err);
        }

        public void AddEventHandler(LSLEventHandlerNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Parent = this;
            _eventHandlers.Add(node);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; set; }


        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceCodeRange { get; private set; }


        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceCodeRangesAvailable { get; private set; }


        /// <summary>
        /// The source code range of the opening brace of the state block's scope.
        /// </summary>
        public LSLSourceCodeRange OpenBraceSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range of the closing brace of the state block's scope.
        /// </summary>
        public LSLSourceCodeRange CloseBraceSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range where the name of the state is located.
        /// For the default state, this will be the location of the 'default' keyword.
        /// </summary>
        public LSLSourceCodeRange StateNameSourceCodeRange { get; private set; }

        public LSLSourceCodeRange StateKeywordSourceCodeRange { get; private set; }


        /// <summary>
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (IsDefaultState)
            {
                return visitor.VisitDefaultState(this);
            }

            return visitor.VisitDefinedState(this);
        }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }

        #endregion
    }
}