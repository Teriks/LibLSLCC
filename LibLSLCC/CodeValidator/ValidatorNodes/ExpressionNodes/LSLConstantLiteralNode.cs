using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public abstract class LSLConstantLiteralNode : ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLConstantLiteralNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        protected internal LSLConstantLiteralNode(LSLParser.Expr_AtomContext context, LSLType type)
        {
            ParserContext = context;
            Type = type;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.Expr_AtomContext ParserContext { get; set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }




        #region ILSLExprNode Members


        public string RawText
        {
            get { return ParserContext.children[0].GetText(); }
        }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; set; }

        public abstract T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor);


        public ILSLSyntaxTreeNode Parent { get; set; }


        public LSLType Type { get; private set; }


        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.Literal; }
        }


        public bool IsConstant
        {
            get { return true; }
        }


        public abstract ILSLExprNode Clone();



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