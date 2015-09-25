#region FileInfo

// 
// File: LSLRotationLiteralNode.cs
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
    public class LSLRotationLiteralNode : ILSLRotationLiteralNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLRotationLiteralNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLRotationLiteralNode(LSLParser.RotationLiteralContext context, ILSLExprNode x, ILSLExprNode y,
            ILSLExprNode z, ILSLExprNode s)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }
            x.Parent = this;

            if (y == null)
            {
                throw new ArgumentNullException("y");
            }
            y.Parent = this;

            if (z == null)
            {
                throw new ArgumentNullException("z");
            }
            z.Parent = this;

            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            s.Parent = this;

            ParserContext = context;
            XExpression = x;
            YExpression = y;
            ZExpression = z;
            SExpression = s;

            SourceCodeRange = new LSLSourceCodeRange(context);

            CommaOneSourceCodeRange = new LSLSourceCodeRange(context.comma_one);
            CommaTwoSourceCodeRange = new LSLSourceCodeRange(context.comma_two);
            CommaThreeSourceCodeRange = new LSLSourceCodeRange(context.comma_three);
        }

        internal LSLParser.RotationLiteralContext ParserContext { get; }
        public ILSLExprNode XExpression { get; }
        public ILSLExprNode YExpression { get; }
        public ILSLExprNode ZExpression { get; }
        public ILSLExprNode SExpression { get; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLReadOnlyExprNode ILSLRotationLiteralNode.XExpression
        {
            get { return XExpression; }
        }

        ILSLReadOnlyExprNode ILSLRotationLiteralNode.YExpression
        {
            get { return YExpression; }
        }

        ILSLReadOnlyExprNode ILSLRotationLiteralNode.ZExpression
        {
            get { return ZExpression; }
        }

        ILSLReadOnlyExprNode ILSLRotationLiteralNode.SExpression
        {
            get { return SExpression; }
        }

        public static
            LSLRotationLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLRotationLiteralNode(sourceRange, Err.Err);
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

            return new LSLRotationLiteralNode(ParserContext, XExpression.Clone(), YExpression.Clone(),
                ZExpression.Clone(),
                SExpression.Clone())
            {
                HasErrors = HasErrors,
                Parent = Parent
            };
        }


        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }

        public LSLSourceCodeRange CommaOneSourceCodeRange { get; }
        public LSLSourceCodeRange CommaTwoSourceCodeRange { get; }
        public LSLSourceCodeRange CommaThreeSourceCodeRange { get; }

        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitRotationLiteral(this);
        }


        public LSLType Type
        {
            get { return LSLType.Rotation; }
        }


        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.Literal; }
        }


        public bool IsConstant
        {
            get
            {
                var precondition =
                    XExpression != null &&
                    YExpression != null &&
                    ZExpression != null &&
                    SExpression != null;

                return precondition &&
                       XExpression.IsConstant &&
                       YExpression.IsConstant &&
                       ZExpression.IsConstant &&
                       SExpression.IsConstant;
            }
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