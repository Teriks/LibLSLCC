using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLFloatLiteralNode : LSLConstantLiteralNode, ILSLFloatLiteralNode
    {
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLFloatLiteralNode(LSLSourceCodeRange sourceRange, Err err)
            : base(sourceRange, Err.Err)
            // ReSharper restore UnusedParameter.Local
        {
        }



        internal LSLFloatLiteralNode(LSLParser.Expr_AtomContext context)
            : base(context, LSLType.Float)
        {
        }



        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }



        public override T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitFloatLiteral(this);
        }



        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }



        public static LSLFloatLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLFloatLiteralNode(sourceRange, Err.Err);
        }



        public override ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var x = new LSLFloatLiteralNode(ParserContext)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };

            return x;
        }
    }
}