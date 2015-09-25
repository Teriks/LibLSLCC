#region FileInfo

// 
// File: LSLEventHandlerNode.cs
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

        internal LSLParser.EventHandlerContext ParserContext { get; }

        public IReadOnlyList<LSLParameterNode> ParameterNodes
        {
            get { return ParameterListNode.Parameters; }
        }

        public LSLCodeScopeNode EventBodyNode { get; }
        public LSLParameterListNode ParameterListNode { get; }

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