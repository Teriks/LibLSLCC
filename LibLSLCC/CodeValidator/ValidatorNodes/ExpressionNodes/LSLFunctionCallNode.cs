using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLFunctionCallNode : ILSLFunctionCallNode, ILSLExprNode
    {
        private readonly bool _libraryFunction;
        private readonly LSLFunctionSignature _librarySignature;
        private readonly LSLPreDefinedFunctionSignature _preDefinition;


// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLFunctionCallNode(LSLSourceCodeRange sourceCodeRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceCodeRange;
            HasErrors = true;
        }



        internal LSLFunctionCallNode(LSLParser.Expr_FunctionCallContext context,
            LSLPreDefinedFunctionSignature preDefinition,
            LSLExpressionListNode parameterList)
        {
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }


            ParserContext = context;

            _preDefinition = preDefinition;
            _libraryFunction = false;
            ParameterListNode = parameterList;

            parameterList.Parent = this;

            SourceCodeRange = new LSLSourceCodeRange(context);
            OpenParenthSourceCodeRange = new LSLSourceCodeRange(context.open_parenth);
            CloseParenthSourceCodeRange = new LSLSourceCodeRange(context.close_parenth);
            FunctionNameSourceCodeRange = new LSLSourceCodeRange(context.function_name);
        }



        internal LSLFunctionCallNode(LSLParser.Expr_FunctionCallContext context,
            LSLFunctionSignature signature,
            LSLExpressionListNode parameterList)
        {
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }


            ParserContext = context;
            _librarySignature = signature;
            _preDefinition = null;
            _libraryFunction = true;
            ParameterListNode = parameterList;
            parameterList.Parent = this;

            SourceCodeRange = new LSLSourceCodeRange(context);
            OpenParenthSourceCodeRange = new LSLSourceCodeRange(context.open_parenth);
            CloseParenthSourceCodeRange = new LSLSourceCodeRange(context.close_parenth);
            FunctionNameSourceCodeRange = new LSLSourceCodeRange(context.function_name);
        }



        internal LSLParser.Expr_FunctionCallContext ParserContext { get; private set; }


        public IReadOnlyList<ILSLExprNode> ParameterExpressions
        {
            get { return ParameterListNode.ExpressionNodes; }
        }

        public LSLExpressionListNode ParameterListNode { get; private set; }

        public LSLFunctionDeclarationNode DefinitionNode
        {
            get
            {
                if (!_libraryFunction)
                {
                    return _preDefinition.DefinitionNode;
                }

                throw new InvalidOperationException("Cannot get a definition node for a library function call.");
            }
        }

        public LSLSourceCodeRange OpenParenthSourceCodeRange { get; private set; }

        public LSLSourceCodeRange CloseParenthSourceCodeRange
        {
            get;
            private set;
        }

        public LSLSourceCodeRange FunctionNameSourceCodeRange
        {
            get;
            private set;
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public string Name
        {
            get { return ParserContext.function_name.Text; }
        }


        IReadOnlyList<ILSLReadOnlyExprNode> ILSLFunctionCallNode.ParameterExpressions
        {
            get { return ParameterExpressions; }
        }


        public LSLFunctionSignature Signature
        {
            get
            {
                if (_libraryFunction)
                {
                    return _librarySignature;
                }
                return _preDefinition;
            }
        }


        ILSLExpressionListNode ILSLFunctionCallNode.ParameterListNode
        {
            get { return ParameterListNode; }
        }


        ILSLFunctionDeclarationNode ILSLFunctionCallNode.DefinitionNode
        {
            get { return DefinitionNode; }
        }




        #region ILSLExprNode Members


        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var parameterList = ParameterListNode == null ? null : ParameterListNode.Clone();

            if (_libraryFunction)
            {
                return new LSLFunctionCallNode(ParserContext, _librarySignature, parameterList)
                {
                    HasErrors = HasErrors,
                    Parent = Parent
                };
            }

            return new LSLFunctionCallNode(ParserContext, _preDefinition, parameterList)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };
        }



        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (ExpressionType == LSLExpressionType.LibraryFunction)
            {
                return visitor.VisitLibraryFunctionCall(this);
            }

            if (ExpressionType == LSLExpressionType.UserFunction)
            {
                return visitor.VisitUserFunctionCall(this);
            }

            throw new InvalidOperationException(
                "LSLFunctionCallNode could not be visited, object is in an invalid state");
        }



        public LSLType Type
        {
            get { return Signature.ReturnType; }
        }

        public LSLExpressionType ExpressionType
        {
            get { return _libraryFunction ? LSLExpressionType.LibraryFunction : LSLExpressionType.UserFunction; }
        }

        public bool IsConstant
        {
            get { return false; }
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




        public static LSLFunctionCallNode GetError(LSLSourceCodeRange sourceCodeRange)
        {
            return new LSLFunctionCallNode(sourceCodeRange, Err.Err);
        }
    }
}