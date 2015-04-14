using System;
using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes
{
    public class LSLEventHandlerNode : ILSLEventHandlerNode, ILSLSyntaxTreeNode
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLEventHandlerNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLEventHandlerNode(LSLParser.EventHandlerContext context, LSLParameterListNode parameterListNode,
            LSLCodeScopeNode eventBodyNode)
        {
            if (parameterListNode == null)
            {
                throw new ArgumentNullException("parameterListNode");
            }

            if (eventBodyNode == null)
            {
                throw new ArgumentNullException("eventBodyNode");
            }

            ParserContext = context;
            EventBodyNode = eventBodyNode;
            ParameterListNode = parameterListNode;

            EventBodyNode.Parent = this;
            ParameterListNode.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.EventHandlerContext ParserContext { get; private set; }

        public IReadOnlyList<LSLParameterNode> ParameterNodes
        {
            get { return ParameterListNode.Parameters; }
        }

        public LSLCodeScopeNode EventBodyNode { get; private set; }
        public LSLParameterListNode ParameterListNode { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        public string Name
        {
            get { return ParserContext.handler_name.Text; }
        }


        public bool HasParameterNodes
        {
            get { return ParameterListNode.Parameters.Any(); }
        }


        IReadOnlyList<ILSLParameterNode> ILSLEventHandlerNode.ParameterNodes
        {
            get { return ParameterNodes; }
        }


        ILSLCodeScopeNode ILSLEventHandlerNode.EventBodyNode
        {
            get { return EventBodyNode; }
        }

        ILSLParameterListNode ILSLEventHandlerNode.ParameterListNode
        {
            get { return ParameterListNode; }
        }



        public LSLEventSignature ToSignature()
        {
            return new LSLEventSignature(Name,
                ParameterListNode.Parameters.Select(x => new LSLParameter(x.Type, x.Name,false)));
        }




        #region ILSLTreeNode Members


        public LSLSourceCodeRange SourceCodeRange { get; set; }


        public bool HasErrors { get; set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitEventHandler(this);
        }



        public ILSLSyntaxTreeNode Parent { get; set; }


        #endregion




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion




        public static
            LSLEventHandlerNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLEventHandlerNode(sourceRange, Err.Err);
        }
    }
}