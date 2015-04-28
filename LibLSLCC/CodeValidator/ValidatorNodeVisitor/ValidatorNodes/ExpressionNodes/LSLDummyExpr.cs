using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLDummyExpr : ILSLExprNode
    {


// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLDummyExpr(Err err)
// ReSharper restore UnusedParameter.Local
        {
        }



        public LSLDummyExpr()
        {
        }



        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }




        #region ILSLExprNode Members


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange
        {
            get { return new LSLSourceCodeRange(); }
        }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            throw new NotImplementedException("Visited LSLDummyExpr");
        }



        public ILSLSyntaxTreeNode Parent { get; set; }


        public LSLType Type { get; set; }


        public LSLExpressionType ExpressionType { get; set; }


        public bool IsConstant { get; set; }



        public ILSLExprNode Clone()
        {

            return new LSLDummyExpr
            {
                Parent = Parent,
                ExpressionType = ExpressionType,
                Type = Type,
                HasErrors = HasErrors,
                IsConstant = IsConstant,
            };
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




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion
    }
}