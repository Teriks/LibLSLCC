using System;
using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes
{
    public class LSLFunctionDeclarationNode : ILSLFunctionDeclarationNode, ILSLSyntaxTreeNode
    {
        private readonly List<LSLFunctionCallNode> _references = new List<LSLFunctionCallNode>();
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLFunctionDeclarationNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLFunctionDeclarationNode(LSLParser.FunctionDeclarationContext context,
            LSLParameterListNode parameterListNode, LSLCodeScopeNode functionBodyNode)
        {
            if (parameterListNode == null)
            {
                throw new ArgumentNullException("parameterListNode");
            }

            if (functionBodyNode == null)
            {
                throw new ArgumentNullException("functionBodyNode");
            }

            ParserContext = context;
            ParameterListNode = parameterListNode;
            FunctionBodyNode = functionBodyNode;


            FunctionBodyNode.Parent = this;

            ParameterListNode.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.FunctionDeclarationContext ParserContext { get; private set; }


        public IReadOnlyList<LSLParameterNode> ParameterNodes
        {
            get { return ParameterListNode.Parameters; }
        }


        public IReadOnlyList<LSLFunctionCallNode> References { get { return _references; } }

        IReadOnlyList<ILSLFunctionCallNode> ILSLFunctionDeclarationNode.References { get { return _references; } }

        internal void AddReference(LSLFunctionCallNode reference)
        {
            _references.Add(reference);
        }

        public LSLParameterListNode ParameterListNode { get; set; }
        public LSLCodeScopeNode FunctionBodyNode { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public bool HasParameters
        {
            get { return ParameterListNode.Parameters.Any(); }
        }


        IReadOnlyList<ILSLParameterNode> ILSLFunctionDeclarationNode.ParameterNodes
        {
            get { return ParameterListNode.Parameters; }
        }


        public string ReturnTypeString
        {
            get
            {
                if (ParserContext.return_type == null) return "";

                return ParserContext.return_type.Text;
            }
        }

        public string Name
        {
            get { return ParserContext.function_name.Text; }
        }

        public LSLType ReturnType
        {
            get
            {
                if (ParserContext.return_type == null) return LSLType.Void;

                return LSLTypeTools.FromLSLTypeString(ReturnTypeString);
            }
        }

        ILSLParameterListNode ILSLFunctionDeclarationNode.ParameterListNode
        {
            get { return ParameterListNode; }
        }

        ILSLCodeScopeNode ILSLFunctionDeclarationNode.FunctionBodyNode
        {
            get { return FunctionBodyNode; }
        }




        #region ILSLTreeNode Members


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitFunctionDeclaration(this);
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
            LSLFunctionDeclarationNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLFunctionDeclarationNode(sourceRange, Err.Err);
        }



        public LSLFunctionSignature ToSignature()
        {
            return new LSLFunctionSignature(ReturnType, Name,
                ParameterListNode.Parameters.Select(x => new LSLParameter(x.Type, x.Name,false)));
        }
    }
}