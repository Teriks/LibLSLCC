using System;
using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.AntlrTreeUtilitys;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Exceptions;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;

namespace LibLSLCC.CodeValidator.Visitor
{
    internal interface ILSLTreePreePass
    {
        bool HasErrors { get; }
    }

    internal class LSLVisitorScopeTracker
    {
        private readonly Stack<LSLControlStatementNode> _controlStatementStack = new Stack<LSLControlStatementNode>();

        private readonly Dictionary<string, LSLStateScopeNode> _definedStates =
            new Dictionary<string, LSLStateScopeNode>();

        private readonly Dictionary<string, LSLPreDefinedFunctionSignature> _functionDefinitions =
            new Dictionary<string, LSLPreDefinedFunctionSignature>();

        private readonly Dictionary<string, LSLVariableDeclarationNode> _globalVariables =
            new Dictionary<string, LSLVariableDeclarationNode>();


        private readonly Dictionary<LSLParser.CodeScopeContext, Dictionary<string, LSLLabelStatementNode>> _labelScopes
            = new Dictionary<LSLParser.CodeScopeContext, Dictionary<string, LSLLabelStatementNode>>();


        private readonly Dictionary<string, LSLVariableDeclarationNode> _parameterScopeVariables =
            new Dictionary<string, LSLVariableDeclarationNode>();

        private readonly Stack<LSLParser.CodeScopeContext> _scopeStack = new Stack<LSLParser.CodeScopeContext>();
        private readonly Stack<LSLCodeScopeType> _scopeTypeStack = new Stack<LSLCodeScopeType>();

        private readonly Stack<Dictionary<string, LSLVariableDeclarationNode>> _scopeVariables =
            new Stack<Dictionary<string, LSLVariableDeclarationNode>>();

        private readonly Stack<bool> _singleBlockStatementTrackingStack = new Stack<bool>();



        public LSLVisitorScopeTracker(ILSLValidatorServiceProvider validatorServiceProvider)
        {
            ValidatorServiceProvider = validatorServiceProvider;
        }



        public UInt64 CurrentScopeId { get; private set; }


        public ILSLValidatorServiceProvider ValidatorServiceProvider { get; private set; }


        public bool InSingleStatementBlock
        {
            get
            {
                if (_singleBlockStatementTrackingStack.Count != 0)
                {
                    return _singleBlockStatementTrackingStack.Peek();
                }

                return false;
            }
        }


        public Dictionary<string, LSLStateScopeNode> DefinedStates
        {
            get { return _definedStates; }
        }

        public Dictionary<string, LSLPreDefinedFunctionSignature> FunctionDefinitions
        {
            get { return _functionDefinitions; }
        }

        public Dictionary<string, LSLVariableDeclarationNode> GlobalVariables
        {
            get { return _globalVariables; }
        }

        public bool InsideEventHandlerBody
        {
            get { return CurrentEventHandlerSignature != null; }
        }

        public bool InsideVoidFunction
        {
            get { return InsideFunctionBody && CurrentFunctionBodySignature.ReturnType == LSLType.Void; }
        }

        public LSLParsedEventHandlerSignature CurrentEventHandlerSignature { get; private set; }

        public LSLCodeScopeType CurrentCodeScopeType
        {
            get
            {
                if (!_scopeTypeStack.Any())
                {
                    throw new LSLCodeValidatorInternalError("Cannot resolve scope type, not inside a scope");
                }

                return _scopeTypeStack.Peek();
            }
        }

        public LSLParser.CodeScopeContext CurrentCodeScopeContext
        {
            get
            {
                if (!_scopeStack.Any())
                {
                    throw new LSLCodeValidatorInternalError("Cannot resolve current scope context, not in a scope");
                }

                return _scopeStack.Peek();
            }
        }

        public bool InsideFunctionBody
        {
            get { return CurrentFunctionBodySignature != null; }
        }

        public LSLPreDefinedFunctionSignature CurrentFunctionBodySignature { get; private set; }

        internal LSLParser.FunctionDeclarationContext CurrentFunctionContext { get; private set; }

        internal LSLParser.EventHandlerContext CurrentEventHandlerContext { get; private set; }

        public LSLControlStatementNode CurrentControlStatement
        {
            get { return _controlStatementStack.Peek(); }
        }

        public bool InGlobalScope
        {
            get { return !InsideFunctionBody && !InsideEventHandlerBody; }
        }



        public void IncrementScopeId()
        {
            CurrentScopeId++;
        }



        public bool StatePreDefined(string name)
        {
            return DefinedStates.ContainsKey(name);
        }



        public void SetStateNode(string name, LSLStateScopeNode value)
        {
            DefinedStates[name] = value;
        }



        public bool FunctionIsPreDefined(string name)
        {
            return FunctionDefinitions.ContainsKey(name);
        }



        public LSLPreDefinedFunctionSignature ResolveFunctionPreDefine(string name)
        {
            return FunctionDefinitions[name];
        }



        public ILSLTreePreePass EnterFunctionScope(LSLParser.FunctionDeclarationContext context,
            LSLPreDefinedFunctionSignature functionSignature)
        {
            _parameterScopeVariables.Clear();

            CurrentEventHandlerContext = null;
            CurrentEventHandlerSignature = null;

            CurrentFunctionBodySignature = functionSignature;
            CurrentFunctionContext = context;

            if (functionSignature.ParameterListNode != null)
            {
                //define all the parameters in the local scope, they are not considered constant in the analysis
                foreach (var parameter in functionSignature.ParameterListNode.Parameters)
                {
                    //parameter references are implicitly not constant
                    var parameterRef = LSLVariableDeclarationNode.CreateParameter(parameter.ParserContext);


                    _parameterScopeVariables.Add(parameter.Name, parameterRef);
                }
            }

            var labelCollector = DoLabelCollectorPrePass(context);
            return labelCollector;
        }



        public void ExitFunctionScope()
        {
            CurrentFunctionBodySignature = null;

            _parameterScopeVariables.Clear();
            _labelScopes.Clear();
        }



        public ILSLTreePreePass EnterCompilationUnit(LSLParser.CompilationUnitContext context)
        {
            var compilationUnitPrepass = DoFunctionAndStateDefinitionPrePass(context);
            return compilationUnitPrepass;
        }



        public void ExitCompilationUnit()
        {
            Reset();
        }



        public ILSLTreePreePass EnterEventScope(LSLParser.EventHandlerContext context,
            LSLParsedEventHandlerSignature eventSig)
        {
            _parameterScopeVariables.Clear();
            CurrentFunctionBodySignature = null;
            CurrentFunctionContext = null;

            CurrentEventHandlerSignature = eventSig;
            CurrentEventHandlerContext = context;

            if (eventSig.ParameterListNode != null)
            {
                //define all the parameters in the local scope, they are not considered constant in the analysis
                foreach (var parameter in eventSig.ParameterListNode.Parameters)
                {
                    //parameter references are implicitly not constant
                    var parameterRef = LSLVariableDeclarationNode.CreateParameter(parameter.ParserContext);


                    _parameterScopeVariables.Add(parameter.Name, parameterRef);
                }
            }

            var labelCollector = DoLabelCollectorPrePass(context);
            return labelCollector;
        }



        public void EnterControlStatement(LSLControlStatementNode statement)
        {
            _controlStatementStack.Push(statement);
        }



        public void ExitControlStatement()
        {
            _controlStatementStack.Pop();
        }



        public void ExitEventScope()
        {
            CurrentEventHandlerSignature = null;
            CurrentEventHandlerContext = null;

            _parameterScopeVariables.Clear();

            _labelScopes.Clear();
        }



        public void EnterCodeScopeAfterPrePass(LSLParser.CodeScopeContext context)
        {
            _scopeTypeStack.Push(LSLAntlrTreeIntrospector.ResolveCodeScopeNodeType(context));
            _singleBlockStatementTrackingStack.Push(false);
            _scopeStack.Push(context);
            EnterLocalVariableScope();
        }



        public void ExitCodeScopeAfterPrePass()
        {
            _scopeTypeStack.Pop();
            _singleBlockStatementTrackingStack.Pop();
            _scopeStack.Pop();
            ExitLocalVariableScope();
        }


        /// <summary>
        /// Determines if given the current scoping state, can a variable be defined without causing a conflict
        /// </summary>
        /// <param name="name">name of the variable</param>
        /// <param name="scope">the scope level of the variable to be defined</param>
        /// <returns>whether or not the variable can be defined without conflict</returns>
        public bool CanVariableBeDefined(string name, LSLVariableScope scope)
        {
            if (scope == LSLVariableScope.Global)
            {
                return GlobalVariables.ContainsKey(name);
            }
            if (scope == LSLVariableScope.Local)
            {
                return _scopeVariables.Any(s => s.ContainsKey(name));
            }
            return false;
        }


        /// <summary>
        /// All local variables in the current scope, excluding parameters
        /// </summary>
        public IEnumerable<LSLVariableDeclarationNode> AllLocalVariablesInScope
        {
            get { return _scopeVariables.Peek().Values; }
        }

        /// <summary>
        /// All parameters defined in the current scope
        /// </summary>
        public IEnumerable<LSLVariableDeclarationNode> AllParametersInScope
        {
            get { return _parameterScopeVariables.Values; }
        }

        public void DefineVariable(LSLVariableDeclarationNode decl, LSLVariableScope scope)
        {
            //return a clone of the node into the global pool of variables, so if we modify it
            //it does not modify the tree node we put it
            if (scope == LSLVariableScope.Global)
            {
                GlobalVariables.Add(decl.Name, decl);
            }
            if (scope == LSLVariableScope.Local)
            {
                _scopeVariables.Peek().Add(decl.Name, decl);
            }
        }

        public bool GlobalVariableDefined(string name)
        {
            return _globalVariables.ContainsKey(name);
        }



        public bool ParameterDefined(string name)
        {
            return _parameterScopeVariables.ContainsKey(name);
        }



        public LSLVariableDeclarationNode ResolveParameter(string name)
        {
            return _parameterScopeVariables[name];
        }

        public LSLVariableDeclarationNode ResolveGlobalVariable(string name)
        {
            return _globalVariables[name];
        }

        public LSLVariableDeclarationNode ResolveVariable(string name)
        {
            var local = (from scope in _scopeVariables
                where scope.ContainsKey(name)
                select scope[name]).FirstOrDefault();

            if (local != null)
            {
                return local;
            }


            if (_parameterScopeVariables.ContainsKey(name))
            {
                var x = _parameterScopeVariables[name];
                return x;
            }


            if (GlobalVariables.ContainsKey(name))
            {
                return GlobalVariables[name];
            }

            return null;
        }



        public bool LabelPreDefinedAnywhere(string name)
        {
            return _labelScopes.Values.Any(x => x.ContainsKey(name));
        }



        public bool LabelPreDefinedInScope(string name)
        {
            return _scopeStack.Any(x => _labelScopes[x].ContainsKey(name));
        }



        public LSLLabelStatementNode ResolvePreDefinedLabelNode(string name)
        {
            return (from codeScopeContext in _scopeStack
                where _labelScopes[codeScopeContext].ContainsKey(name)
                select _labelScopes[codeScopeContext][name]).FirstOrDefault();
        }



        public void Reset()
        {
            CurrentEventHandlerSignature = null;
            CurrentEventHandlerContext = null;
            CurrentFunctionBodySignature = null;
            CurrentFunctionContext = null;
            _functionDefinitions.Clear();
            _globalVariables.Clear();
            _definedStates.Clear();
            _parameterScopeVariables.Clear();
            _scopeStack.Clear();
            _scopeTypeStack.Clear();
            _scopeVariables.Clear();
            _singleBlockStatementTrackingStack.Clear();
            _controlStatementStack.Clear();
            CurrentScopeId = 0;
        }



        public void PreDefineState(string name)
        {
            DefinedStates.Add(name, null);
        }



        public void PreDefineFunction(LSLPreDefinedFunctionSignature lslFunctionSignature)
        {
            FunctionDefinitions.Add(lslFunctionSignature.Name, lslFunctionSignature);
        }



        public void EnterCodeScopeDuringPrePass(LSLParser.CodeScopeContext context)
        {
            _scopeTypeStack.Push(LSLAntlrTreeIntrospector.ResolveCodeScopeNodeType(context));
            _singleBlockStatementTrackingStack.Push(false);
            _scopeStack.Push(context);
            _labelScopes.Add(context, new Dictionary<string, LSLLabelStatementNode>());
        }



        public void EnterSingleStatementBlock(LSLParser.CodeStatementContext statement)
        {
            _scopeTypeStack.Push(LSLAntlrTreeIntrospector.ResolveCodeScopeNodeType(statement));
            _singleBlockStatementTrackingStack.Push(true);
        }



        public void ExitSingleStatementBlock()
        {
            _scopeTypeStack.Pop();
            _singleBlockStatementTrackingStack.Pop();
        }



        public void ExitCodeScopeDuringPrePass()
        {
            _scopeTypeStack.Pop();
            _singleBlockStatementTrackingStack.Pop();
            _scopeStack.Pop();
        }



        private void EnterLocalVariableScope()
        {
            _scopeVariables.Push(new Dictionary<string, LSLVariableDeclarationNode>());
        }



        private void ExitLocalVariableScope()
        {
            _scopeVariables.Pop();
        }



        public void PreDefineLabel(string name, LSLLabelStatementNode statement)
        {
            _labelScopes[CurrentCodeScopeContext][name] = statement;
        }



        private FunctionAndStateDefinitionPrePass DoFunctionAndStateDefinitionPrePass(
            LSLParser.CompilationUnitContext context)
        {
            var r = new FunctionAndStateDefinitionPrePass(this, ValidatorServiceProvider);
            r.Visit(context);
            return r;
        }



        private LabelCollectorPrePass DoLabelCollectorPrePass(
            LSLParser.EventHandlerContext context)
        {
            var r = new LabelCollectorPrePass(this, ValidatorServiceProvider);
            r.Visit(context);
            return r;
        }



        private LabelCollectorPrePass DoLabelCollectorPrePass(
            LSLParser.FunctionDeclarationContext context)
        {
            var r = new LabelCollectorPrePass(this,
                ValidatorServiceProvider);
            r.Visit(context);
            return r;
        }



        public void ResetScopeId()
        {
            CurrentScopeId = 0;
        }



        public class FunctionAndStateDefinitionPrePass : LSLBaseVisitor<bool>, ILSLTreePreePass
        {
            private readonly LSLVisitorScopeTracker _scopingManager;

            private readonly ILSLValidatorServiceProvider _validatorServiceProvider;



            public FunctionAndStateDefinitionPrePass(LSLVisitorScopeTracker scopingManager,
                ILSLValidatorServiceProvider validatorServiceProvider)
            {
                _scopingManager = scopingManager;
                _validatorServiceProvider = validatorServiceProvider;
            }



            public bool HasErrors { get; private set; }



            public override bool VisitCompilationUnit(LSLParser.CompilationUnitContext context)
            {
                if (context == null)
                {
                    throw LSLCodeValidatorInternalError.VisitContextInvalidState("VisitCompilationUnit", true);
                }


                var defaultStateRule = context.defaultState();

                if (defaultStateRule == null)
                {
                    _validatorServiceProvider.SyntaxErrorListener.MissingDefaultState();
                    HasErrors = true;
                }
                else
                {
                    Visit(defaultStateRule);
                }

                foreach (var func in context.functionDeclaration())
                {
                    Visit(func);
                }

                foreach (var state in context.definedState())
                {
                    Visit(state);
                }

                return true;
            }



            public override bool VisitDefaultState(LSLParser.DefaultStateContext context)
            {
                if (context == null || context.children == null)
                {
                    throw LSLCodeValidatorInternalError.VisitContextInvalidState("VisitDefaultState", true);
                }


                _scopingManager.PreDefineState("default");


                if (!context.eventHandler().Any())
                {
                    _validatorServiceProvider.SyntaxErrorListener.StateHasNoEventHandlers(
                        new LSLSourceCodeRange(context.state_name),
                        "default");
                    HasErrors = true;
                    return false;
                }


                var eventHandlerNames = new HashSet<string>();

                foreach (var ctx in context.eventHandler())
                {
                    if (ctx.handler_name == null)
                    {
                        throw LSLCodeValidatorInternalError
                            .ContextInInvalidState("VisitDefaultState", typeof (LSLParser.EventHandlerContext), true);
                    }

                    if (eventHandlerNames.Contains(ctx.handler_name.Text))
                    {
                        _validatorServiceProvider.SyntaxErrorListener.RedefinedEventHandler(
                            new LSLSourceCodeRange(ctx.handler_name),
                            ctx.handler_name.Text,
                            context.state_name.Text);
                        HasErrors = true;
                        return false;
                    }
                    eventHandlerNames.Add(ctx.handler_name.Text);
                }


                return true;
            }



            public override bool VisitDefinedState(LSLParser.DefinedStateContext context)
            {
                if (context == null || Utility.AnyNull(context.children, context.state_name))
                {
                    throw LSLCodeValidatorInternalError.VisitContextInvalidState("VisitDefinedState", true);
                }


                if (context.state_name.Text == "default")
                {
                    _validatorServiceProvider.SyntaxErrorListener.RedefinedDefaultState(
                        new LSLSourceCodeRange(context.state_name));
                    HasErrors = true;
                    return false;
                }


                if (_scopingManager.StatePreDefined(context.state_name.Text))
                {
                    _validatorServiceProvider.SyntaxErrorListener.RedefinedStateName(
                        new LSLSourceCodeRange(context.state_name),
                        context.state_name.Text);
                    HasErrors = true;
                    return false;
                }


                _scopingManager.PreDefineState(context.state_name.Text);


                if (!context.eventHandler().Any())
                {
                    _validatorServiceProvider.SyntaxErrorListener.StateHasNoEventHandlers(
                        new LSLSourceCodeRange(context.state_name),
                        context.state_name.Text);
                    HasErrors = true;
                    return false;
                }


                var eventHandlerNames = new HashSet<string>();

                foreach (var ctx in context.eventHandler())
                {
                    if (ctx.handler_name == null)
                    {
                        throw LSLCodeValidatorInternalError
                            .ContextInInvalidState("VisitDefinedState",
                                typeof (LSLParser.EventHandlerContext), true);
                    }

                    if (eventHandlerNames.Contains(ctx.handler_name.Text))
                    {
                        _validatorServiceProvider.SyntaxErrorListener.RedefinedEventHandler(
                            new LSLSourceCodeRange(ctx.handler_name),
                            ctx.handler_name.Text,
                            context.state_name.Text);

                        HasErrors = true;
                        return false;
                    }
                    eventHandlerNames.Add(ctx.handler_name.Text);
                }


                return true;
            }



            public override bool VisitFunctionDeclaration(LSLParser.FunctionDeclarationContext context)
            {
                if (context == null || Utility.AnyNull(context.function_name, context.code))
                {
                    throw LSLCodeValidatorInternalError.VisitContextInvalidState("VisitFunctionDeclaration", true);
                }

                if (_validatorServiceProvider.MainLibraryDataProvider.LibraryFunctionExist(context.function_name.Text))
                {
                    _validatorServiceProvider.SyntaxErrorListener.RedefinedStandardLibraryFunction(
                        new LSLSourceCodeRange(context.function_name),context.function_name.Text,
                        _validatorServiceProvider.MainLibraryDataProvider.GetLibraryFunctionSignatures(
                            context.function_name.Text));
                    HasErrors = true;
                    return false;
                }


                if (_scopingManager.FunctionIsPreDefined(context.function_name.Text))
                {
                    _validatorServiceProvider.SyntaxErrorListener.RedefinedFunction(
                        new LSLSourceCodeRange(context.function_name),
                        _scopingManager.ResolveFunctionPreDefine(context.function_name.Text));
                    HasErrors = true;
                    return false;
                }


                var parameterListNode = LSLParameterListNode.BuildDirectlyFromContext(
                    context.parameters,
                    _validatorServiceProvider);

                if (parameterListNode.HasErrors)
                {
                    HasErrors = true;
                    return false;
                }

                var returnType = LSLType.Void;

                if (context.return_type != null)
                {
                    returnType =
                        LSLTypeTools.FromLSLTypeString(context.return_type.Text);
                }


                var func = new LSLPreDefinedFunctionSignature(returnType,
                    context.function_name.Text,
                    parameterListNode);

                _scopingManager.PreDefineFunction(func);


                base.VisitFunctionDeclaration(context);

                return true;
            }
        }


        public class LabelCollectorPrePass : LSLBaseVisitor<bool>, ILSLTreePreePass
        {
            private readonly LSLVisitorScopeTracker _scopingManager;

            private readonly ILSLValidatorServiceProvider _validatorServiceProvider;

            private UInt64 _currentScopeId;
            private readonly Stack<StatementIndexContainer> _statementIndexStack =
                new Stack<StatementIndexContainer>();



            public LabelCollectorPrePass(LSLVisitorScopeTracker scopingManager,
                ILSLValidatorServiceProvider validatorServiceProvider)
            {
                _scopingManager = scopingManager;
                _validatorServiceProvider = validatorServiceProvider;
                _statementIndexStack.Push(new StatementIndexContainer {Index = 0, ScopeId = 0});
            }



            public bool HasErrors { get; private set; }



            public override bool VisitCodeStatement(LSLParser.CodeStatementContext context)
            {
                base.VisitCodeStatement(context);
                _statementIndexStack.Peek().Index++;
                return false;
            }



            public override bool VisitCodeScopeOrSingleBlockStatement(
                LSLParser.CodeScopeOrSingleBlockStatementContext context)
            {
                if (context == null || !Utility.OnlyOneNotNull(context.code, context.statement))
                {
                    throw
                        LSLCodeValidatorInternalError.
                            VisitContextInvalidState("VisitCodeScopeOrSingleBlockStatement",
                                true);
                }


                if (context.statement != null)
                {
                    _scopingManager.EnterSingleStatementBlock(context.statement);

                    _currentScopeId++;
                    _statementIndexStack.Push(new StatementIndexContainer {Index = 0, ScopeId = _currentScopeId});
                    base.VisitCodeScopeOrSingleBlockStatement(context);
                    _statementIndexStack.Pop();
                    _scopingManager.ExitSingleStatementBlock();
                }
                if (context.code != null)
                {
                    base.VisitCodeScopeOrSingleBlockStatement(context);
                }

                return true;
            }



            public override bool VisitCodeScope(LSLParser.CodeScopeContext context)
            {
                if (context == null || context.children == null)
                {
                    throw LSLCodeValidatorInternalError.VisitContextInvalidState("VisitCodeScope", true);
                }

                _scopingManager.EnterCodeScopeDuringPrePass(context);
                _currentScopeId++;
                _statementIndexStack.Push(new StatementIndexContainer {Index = 0, ScopeId = _currentScopeId});
                var result = base.VisitCodeScope(context);
                _statementIndexStack.Pop();
                _scopingManager.ExitCodeScopeDuringPrePass();
                return result;
            }



            public override bool VisitLabelStatement(LSLParser.LabelStatementContext context)
            {
                if (context == null || context.label_name == null)
                {
                    throw LSLCodeValidatorInternalError.VisitContextInvalidState("VisitLabelStatement", true);
                }


                if (_scopingManager.LabelPreDefinedAnywhere(context.label_name.Text))
                {
                    _validatorServiceProvider.SyntaxErrorListener.RedefinedLabel(
                        new LSLSourceCodeRange(context), 
                        context.label_name.Text);
                    HasErrors = true;
                    return false;
                }


                var ctx = new LSLLabelStatementNode(context, _scopingManager.InSingleStatementBlock);

                var statementIndexInfo = _statementIndexStack.Peek();

                ctx.ScopeId = statementIndexInfo.ScopeId;

                ctx.StatementIndex = statementIndexInfo.Index;

                _scopingManager.PreDefineLabel(context.label_name.Text, ctx);


                return base.VisitLabelStatement(context);
            }



            private class StatementIndexContainer
            {
                public int Index;
                public UInt64 ScopeId;
            }
        }
    }
}