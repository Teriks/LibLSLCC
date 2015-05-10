using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLIntegerLiteralNode : LSLConstantLiteralNode, ILSLIntegerLiteralNode
    {
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLIntegerLiteralNode(LSLSourceCodeRange sourceRange, Err err)
            : base(sourceRange, Err.Err)
            // ReSharper restore UnusedParameter.Local
        {
        }



        internal LSLIntegerLiteralNode(LSLParser.Expr_AtomContext context)
            : base(context, LSLType.Integer)
        {
        }



        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }



        public override T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitIntegerLiteral(this);
        }



        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }



        public static LSLIntegerLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLIntegerLiteralNode(sourceRange, Err.Err);
        }



        public override ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var x = new LSLIntegerLiteralNode(ParserContext)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };

            return x;
        }
    }
}