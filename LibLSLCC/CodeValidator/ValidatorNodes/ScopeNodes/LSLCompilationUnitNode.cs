using System;
using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes
{
    public class LSLCompilationUnitNode : ILSLCompilationUnitNode, ILSLSyntaxTreeNode
    {
        private readonly List<LSLFunctionDeclarationNode> _functionDeclarations = new List<LSLFunctionDeclarationNode>();

        private readonly List<LSLVariableDeclarationNode> _globalVariableDeclarations =
            new List<LSLVariableDeclarationNode>();

        private readonly List<LSLStateScopeNode> _stateDeclarations = new List<LSLStateScopeNode>();


        private int _addCounter;
        private LSLStateScopeNode _defaultState;


// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLCompilationUnitNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        public LSLCompilationUnitNode(LSLParser.CompilationUnitContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ParserContext = context;

            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.CompilationUnitContext ParserContext { get; private set; }


        public IReadOnlyList<LSLVariableDeclarationNode> GlobalVariableDeclarations
        {
            get { return _globalVariableDeclarations; }
        }

        public IReadOnlyList<LSLFunctionDeclarationNode> FunctionDeclarations
        {
            get { return _functionDeclarations; }
        }

        public IReadOnlyList<LSLStateScopeNode> StateDeclarations
        {
            get { return _stateDeclarations; }
        }


        public LSLStateScopeNode DefaultState
        {
            get { return _defaultState; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (_defaultState == null)
                {
                    if (_globalVariableDeclarations.Count > 0)
                    {
                        _globalVariableDeclarations.Last().IsLastStatementInScope = false;
                    }

                    _addCounter++;
                }

                _defaultState = value;
                _defaultState.Parent = this;
            }
        }




        #region ILSLTreeNode Members


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitCompilationUnit(this);
        }



        public ILSLSyntaxTreeNode Parent { get; set; }


        #endregion




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion




        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        IReadOnlyList<ILSLVariableDeclarationNode> ILSLCompilationUnitNode.GlobalVariableDeclarations
        {
            get { return _globalVariableDeclarations; }
        }

        IReadOnlyList<ILSLFunctionDeclarationNode> ILSLCompilationUnitNode.FunctionDeclarations
        {
            get { return _functionDeclarations; }
        }

        IReadOnlyList<ILSLStateScopeNode> ILSLCompilationUnitNode.StateDeclarations
        {
            get { return _stateDeclarations; }
        }

        ILSLStateScopeNode ILSLCompilationUnitNode.DefaultState
        {
            get { return DefaultState; }
        }



        public static
            LSLCompilationUnitNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLCompilationUnitNode(sourceRange, Err.Err);
        }



        public void AddVariableDeclaration(LSLVariableDeclarationNode declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }

            declaration.Parent = this;
            declaration.StatementIndex = _addCounter;
            declaration.IsLastStatementInScope = true;

            if (_globalVariableDeclarations.Count > 0)
            {
                _globalVariableDeclarations.Last().IsLastStatementInScope = false;
            }

            _addCounter++;
            _globalVariableDeclarations.Add(declaration);
        }



        public void AddFunctionDeclaration(LSLFunctionDeclarationNode declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }

            declaration.Parent = this;

            if (_globalVariableDeclarations.Count > 0)
            {
                _globalVariableDeclarations.Last().IsLastStatementInScope = false;
            }
            _addCounter++;
            _functionDeclarations.Add(declaration);
        }



        public void AddStateDeclaration(LSLStateScopeNode declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }

            declaration.Parent = this;

            if (_globalVariableDeclarations.Count > 0)
            {
                _globalVariableDeclarations.Last().IsLastStatementInScope = false;
            }

            _addCounter++;
            _stateDeclarations.Add(declaration);
        }
    }
}