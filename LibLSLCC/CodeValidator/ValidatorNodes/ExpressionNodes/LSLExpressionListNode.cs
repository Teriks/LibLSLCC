#region FileInfo

// 
// File: LSLExpressionListNode.cs
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
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public enum LSLExpressionListType
    {
        ListInitializer,
        UserFunctionCallParameters,
        LibraryFunctionCallParameters,
        ForLoopAfterthoughts,
        ForLoopInitExpressions
    }


    public class LSLExpressionListNode : ILSLExpressionListNode
    {
        private readonly List<LSLSourceCodeRange> _commaSourceCodeRanges = new List<LSLSourceCodeRange>();
        private readonly List<ILSLExprNode> _expressionNodes = new List<ILSLExprNode>();
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLExpressionListNode(LSLSourceCodeRange sourceRange, Err err)

            // ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLExpressionListNode(LSLParser.OptionalExpressionListContext parserContext,
            LSLExpressionListType listType)
        {
            ParserContext = parserContext;
            ListType = listType;
            SourceCodeRange = new LSLSourceCodeRange(parserContext);
        }

        internal LSLExpressionListNode(LSLParser.OptionalExpressionListContext parserContext,
            IEnumerable<ILSLExprNode> expressions,
            LSLExpressionListType listType)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException("expressions");
            }

            ListType = listType;

            foreach (var lslExprNode in expressions)
            {
                AddExpression(lslExprNode);
            }
            SourceCodeRange = new LSLSourceCodeRange(parserContext);
        }

        internal LSLParser.OptionalExpressionListContext ParserContext { get; set; }

        public IReadOnlyList<ILSLExprNode> ExpressionNodes
        {
            get { return _expressionNodes; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public LSLExpressionListType ListType { get; }

        IReadOnlyList<ILSLReadOnlyExprNode> ILSLExpressionListNode.ExpressionNodes
        {
            get { return _expressionNodes; }
        }

        public bool AllExpressionsConstant
        {
            get { return ExpressionNodes.Count == 0 || ExpressionNodes.All(lslExprNode => lslExprNode.IsConstant); }
        }

        public bool HasExpressionNodes
        {
            get { return ExpressionNodes.Count > 0; }
        }

        public IReadOnlyList<LSLSourceCodeRange> CommaSourceCodeRanges
        {
            get { return _commaSourceCodeRanges; }
        }

        public static LSLExpressionListNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLExpressionListNode(sourceRange, Err.Err);
        }

        public void AddExpression(ILSLExprNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Parent = this;

            _expressionNodes.Add(node);
        }

        public LSLExpressionListNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var r = new LSLExpressionListNode(ParserContext, ListType)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };


            foreach (var expressionNode in _expressionNodes)
            {
                r.AddExpression(expressionNode);
            }

            return r;
        }

        public void AddCommaRange(LSLSourceCodeRange range)
        {
            _commaSourceCodeRanges.Add(range);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        public LSLSourceCodeRange SourceCodeRange { get; }


        public bool HasErrors { get; set; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (ListType == LSLExpressionListType.ForLoopAfterthoughts)
            {
                return visitor.VisitForLoopAfterthoughts(this);
            }

            if (ListType == LSLExpressionListType.LibraryFunctionCallParameters)
            {
                return visitor.VisitLibraryFunctionCallParameters(this);
            }

            if (ListType == LSLExpressionListType.UserFunctionCallParameters)
            {
                return visitor.VisitUserFunctionCallParameters(this);
            }

            if (ListType == LSLExpressionListType.ListInitializer)
            {
                return visitor.VisitListLiteralInitializerList(this);
            }

            if (ListType == LSLExpressionListType.ForLoopInitExpressions)
            {
                return visitor.VisitForLoopInitExpressions(this);
            }

            throw new InvalidOperationException("Visit LSLExpressionList, unknown ListTypes");
        }


        public ILSLSyntaxTreeNode Parent { get; set; }

        #endregion
    }
}