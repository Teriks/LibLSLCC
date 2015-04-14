using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLHexLiteralNode : LSLConstantLiteralNode, ILSLHexLiteralNode
    {
        // ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLHexLiteralNode(LSLSourceCodeRange sourceRange, Err err)
            : base(sourceRange, Err.Err)
            // ReSharper restore UnusedParameter.Local
        {
        }



        internal LSLHexLiteralNode(LSLParser.Expr_AtomContext context)
            : base(context, LSLType.Integer)
        {
        }



        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }



        public override T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitHexLiteral(this);
        }



        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }



        public static LSLHexLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLHexLiteralNode(sourceRange, Err.Err);
        }



        public override ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var x = new LSLHexLiteralNode(ParserContext)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };

            return x;
        }
    }
}