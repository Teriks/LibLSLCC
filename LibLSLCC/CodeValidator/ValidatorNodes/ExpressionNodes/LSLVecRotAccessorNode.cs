#region FileInfo

// 
// File: LSLVecRotAccessorNode.cs
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
    public class LSLTupleAccessorNode : ILSLTupleAccessorNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLTupleAccessorNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLTupleAccessorNode(LSLParser.Expr_DotAccessorContext context, ILSLExprNode accessedExpression,
            LSLType accessedType,
            LSLTupleComponent accessedComponent)
        {
            if (accessedType != LSLType.Vector && accessedType != LSLType.Rotation)
            {
                throw new ArgumentException("accessedType can only be LSLType.Vector or LSLType.Rotation");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (accessedExpression == null)
            {
                throw new ArgumentNullException("context");
            }

            ParserContext = context;
            AccessedComponent = accessedComponent;
            AccessedType = accessedType;
            AccessedExpression = accessedExpression;
            AccessedExpression.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }

        internal LSLParser.Expr_DotAccessorContext ParserContext { get; }
        public ILSLExprNode AccessedExpression { get; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public string AccessedComponentString
        {
            get { return ParserContext.member.Text; }
        }

        public LSLTupleComponent AccessedComponent { get; }
        public LSLType AccessedType { get; }

        ILSLReadOnlyExprNode ILSLTupleAccessorNode.AccessedExpression
        {
            get { return AccessedExpression; }
        }

        public static
            LSLTupleAccessorNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLTupleAccessorNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region AccessType enum

        #endregion

        #region Component enum

        #endregion

        #region ILSLExprNode Members

        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLTupleAccessorNode(ParserContext, AccessedExpression.Clone(), AccessedType, AccessedComponent)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };
        }


        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("visitor");
            }

            return visitor.VisitVecRotAccessor(this);
        }


        public LSLType Type
        {
            get { return LSLType.Float; }
        }


        public LSLExpressionType ExpressionType
        {
            get
            {
                return AccessedType == LSLType.Vector
                    ? LSLExpressionType.VectorComponentAccess
                    : LSLExpressionType.RotationComponentAccess;
            }
        }


        public bool IsConstant
        {
            get { return AccessedExpression.IsConstant; }
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