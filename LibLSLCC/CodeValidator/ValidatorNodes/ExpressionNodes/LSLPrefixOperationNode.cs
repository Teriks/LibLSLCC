#region FileInfo

// 
// File: LSLPrefixOperationNode.cs
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
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLPrefixOperationNode : ILSLPrefixOperationNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLPrefixOperationNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLPrefixOperationNode(LSLParser.Expr_PrefixOperationContext context, LSLType resultType,
            ILSLExprNode rightExpression)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (rightExpression == null)
            {
                throw new ArgumentNullException("rightExpression");
            }


            ParserContext = context;
            Type = resultType;
            RightExpression = rightExpression;
            RightExpression.Parent = this;

            ParseAndSetOperation(context.operation.Text);

            SourceCodeRange = new LSLSourceCodeRange(context);

            OperationSourceCodeRange = new LSLSourceCodeRange(context.operation);
        }

        internal LSLParser.Expr_PrefixOperationContext ParserContext { get; }
        public ILSLExprNode RightExpression { get; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public LSLPrefixOperationType Operation { get; private set; }

        public string OperationString
        {
            get { return ParserContext.operation.Text; }
        }

        ILSLReadOnlyExprNode ILSLPrefixOperationNode.RightExpression
        {
            get { return RightExpression; }
        }

        public static
            LSLPrefixOperationNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLPrefixOperationNode(sourceRange, Err.Err);
        }

        private void ParseAndSetOperation(string operationString)
        {
            Operation = LSLPrefixOperationTypeTools.ParseFromOperator(operationString);
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

            return new LSLPrefixOperationNode(ParserContext, Type, RightExpression.Clone())
            {
                HasErrors = HasErrors,
                Parent = Parent
            };
        }


        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }

        public LSLSourceCodeRange OperationSourceCodeRange { get; }

        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitPrefixOperation(this);
        }


        public LSLType Type { get; }

        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.PrefixExpression; }
        }

        public bool IsConstant
        {
            get { return RightExpression.IsConstant; }
        }


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