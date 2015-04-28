using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLParameterNode : ILSLParameterNode, ILSLSyntaxTreeNode
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLParameterNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        public LSLParameterNode(LSLParser.ParameterDefinitionContext context)
        {
            ParserContext = context;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.ParameterDefinitionContext ParserContext { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        public string Name
        {
            get { return ParserContext.ID().GetText(); }
        }


        public LSLType Type
        {
            get { return LSLTypeTools.FromLSLTypeString(ParserContext.TYPE().GetText()); }
        }


        public string TypeString
        {
            get { return ParserContext.TYPE().GetText(); }
        }

        public int ParameterIndex { get; set; }

        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitParameterDefinition(this);
        }



        public ILSLSyntaxTreeNode Parent { get; set; }



        public static
            LSLParameterNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLParameterNode(sourceRange, Err.Err);
        }



        protected enum Err
        {
            Err
        }
    }
}