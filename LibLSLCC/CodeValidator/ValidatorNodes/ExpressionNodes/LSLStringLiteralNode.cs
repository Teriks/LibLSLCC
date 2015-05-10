using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLStringLiteralNode : LSLConstantLiteralNode, ILSLStringLiteralNode
    {
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLStringLiteralNode(LSLSourceCodeRange sourceRange, Err err)
            : base(sourceRange, Err.Err)
            // ReSharper restore UnusedParameter.Local
        {
        }



        internal LSLStringLiteralNode(LSLParser.Expr_AtomContext context, string preProccessedText)
            : base(context, LSLType.String)
        {
            PreProccessedText = preProccessedText;
        }



        public string PreProccessedText { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }



        public override T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitStringLiteral(this);
        }



        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }



        public static LSLStringLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLStringLiteralNode(sourceRange, Err.Err);
        }



        public override ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var x = new LSLStringLiteralNode(ParserContext, PreProccessedText)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };

            return x;
        }
    }
}