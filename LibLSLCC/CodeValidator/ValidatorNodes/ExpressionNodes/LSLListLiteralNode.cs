#region FileInfo

// 
// File: LSLListLiteralNode.cs
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
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLListLiteralNode : ILSLListLiteralNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLListLiteralNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLListLiteralNode(LSLParser.ListLiteralContext context, LSLExpressionListNode expressionListNode)
        {
            if (expressionListNode == null)
            {
                throw new ArgumentNullException("expressionListNode");
            }


            IsConstant = expressionListNode.AllExpressionsConstant;
            ParserContext = context;
            ExpressionListNode = expressionListNode;


            ExpressionListNode.Parent = this;

            SourceCodeRange = new LSLSourceCodeRange(context);
        }

        internal LSLParser.ListLiteralContext ParserContext { get; private set; }

        public IReadOnlyList<ILSLExprNode> ListEntryExpressions
        {
            get { return ExpressionListNode.ExpressionNodes; }
        }

        public LSLExpressionListNode ExpressionListNode { get; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        IReadOnlyList<ILSLReadOnlyExprNode> ILSLListLiteralNode.ListEntryExpressions
        {
            get { return ListEntryExpressions; }
        }

        ILSLExpressionListNode ILSLListLiteralNode.ExpressionListNode
        {
            get { return ExpressionListNode; }
        }

        public static
            LSLListLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLListLiteralNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLExprNode Members

        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLListLiteralNode(ParserContext, ExpressionListNode.Clone())
            {
                HasErrors = HasErrors,
                IsConstant = IsConstant,
                Parent = Parent,
                ParserContext = ParserContext
            };
        }


        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitListLiteral(this);
        }


        public LSLType Type
        {
            get { return LSLType.List; }
        }

        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.Literal; }
        }

        public bool IsConstant { get; private set; }


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