#region FileInfo
// 
// 
// File: LSLStateScopeNode.cs
// 
// Last Compile: 25/09/2015 @ 5:46 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// ============================================================
// ============================================================
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// Copyright (c) 2015, Teriks
// All rights reserved.
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
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes
{
    public class LSLStateScopeNode : ILSLStateScopeNode, ILSLSyntaxTreeNode
    {
        private readonly List<LSLEventHandlerNode> _eventHandlers = new List<LSLEventHandlerNode>();

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
        }

        internal LSLParser.DefinedStateContext DefinedStateContext { get; private set; }
        internal LSLParser.DefaultStateContext DefaultStateContext { get; private set; }

        public IReadOnlyList<LSLEventHandlerNode> EventHandlers
        {
            get { return _eventHandlers; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public string StateName { get; }
        public bool IsDefinedState { get; }
        public bool IsDefaultState { get; }

        IReadOnlyList<ILSLEventHandlerNode> ILSLStateScopeNode.EventHandlers
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

        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }

        public LSLSourceCodeRange OpenBraceSourceCodeRange { get; }

        public LSLSourceCodeRange CloseBraceSourceCodeRange { get; }

        public LSLSourceCodeRange StateNameSourceCodeRange { get; }

        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (IsDefaultState)
            {
                return visitor.VisitDefaultState(this);
            }

            return visitor.VisitDefinedState(this);
        }


        public ILSLSyntaxTreeNode Parent { get; set; }

        #endregion
    }
}