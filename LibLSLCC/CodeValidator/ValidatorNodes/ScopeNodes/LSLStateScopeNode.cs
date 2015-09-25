#region FileInfo

// 
// File: LSLStateScopeNode.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:24 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
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