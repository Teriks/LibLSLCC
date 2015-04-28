#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Exceptions;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.LoopNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;

#endregion

namespace LibLSLCC.CodeValidator.Visitor
{
    internal partial class LSLCodeValidationVisitor : LSLBaseVisitor<ILSLSyntaxTreeNode>
    {
        private readonly ILSLValidatorServiceProvider _validatorServices;
        private ILSLSyntaxErrorListener _syntaxErrorListenerOveride;
        private ILSLSyntaxWarningListener _syntaxWarningListenerOveride;


        public LSLCodeValidationVisitor()
        {
            _validatorServices = new LSLDefaultValidatorServiceProvider();

            ScopingManager = new LSLVisitorScopeTracker(_validatorServices);
        }


        public LSLCodeValidationVisitor(ILSLValidatorServiceProvider validatorServices)
        {
            if (!validatorServices.IsComplete())
            {
                throw new ArgumentException("An ILSLValidatorServiceProvider property was null", "validatorServices");
            }

            _validatorServices = validatorServices;

            ScopingManager = new LSLVisitorScopeTracker(_validatorServices);
        }


        private LSLVisitorScopeTracker ScopingManager { get; set; }


        public ILSLSyntaxWarningListener SyntaxWarningListener
        {
            get
            {
                if (_syntaxWarningListenerOveride == null)
                {
                    return _validatorServices.SyntaxWarningListener;
                }
                return _syntaxWarningListenerOveride;
            }
        }

        public ILSLExpressionValidator ExpressionValidator
        {
            get { return _validatorServices.ExpressionValidator; }
        }


        public ILSLSyntaxErrorListener SyntaxErrorListener
        {
            get
            {
                if (_syntaxErrorListenerOveride == null)
                {
                    return _validatorServices.SyntaxErrorListener;
                }
                return _syntaxErrorListenerOveride;
            }
        }

        public ILSLMainLibraryDataProvider MainLibraryDataProvider
        {
            get { return _validatorServices.MainLibraryDataProvider; }
        }

        public ILSLStringPreProcessor StringLiteralPreProcessor
        {
            get { return _validatorServices.StringLiteralPreProcessor; }
        }


        private bool InSingleStatementBlock
        {
            get { return ScopingManager.InSingleStatementBlock; }
        }


        private void OverrideSyntaxWarningListener(ILSLSyntaxWarningListener listener)
        {
            _syntaxWarningListenerOveride = listener;
        }


        private void RemoveSyntaxWarningListenerOverride()
        {
            _syntaxWarningListenerOveride = null;
        }


        private void OverrideSyntaxErrorListener(ILSLSyntaxErrorListener listener)
        {
            _syntaxErrorListenerOveride = listener;
        }


        private void RemoveSyntaxErrorListenerOverride()
        {
            _syntaxErrorListenerOveride = null;
        }


        public LSLCompilationUnitNode ValidateAndBuildTree(LSLParser.CompilationUnitContext tree)
        {
            LSLCompilationUnitNode x;
            try
            {
                x = VisitCompilationUnit(tree) as LSLCompilationUnitNode;
                if (x == null)
                {
                    throw LSLCodeValidatorInternalError.VisitReturnTypeException("VisitCompilationUnit",
                        typeof(LSLCompilationUnitNode));
                }
            }
            finally
            {
                Reset();
            }

            return x;
        }


        private void Reset()
        {
            _lastContextWithListAssign = null;
            _lastContextWithStringAssign = null;
            _referencesToNotYetDefinedFunctions.Clear();
            _syntaxErrorListenerOveride = null;
            _syntaxWarningListenerOveride = null;
            _multipleListAssignmentWarned.Clear();
            _multipleStringAssignmentWarned.Clear();

            ScopingManager.Reset();
            StringLiteralPreProcessor.Reset();
        }


        private ILSLSyntaxTreeNode VisitListLiteralInitializerList(LSLParser.OptionalExpressionListContext context)
        {
            if (context == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitListLiteralInitializerList");
            }

            var result = new LSLExpressionListNode(context, LSLExpressionListType.ListInitializer);

            var expressionList = context.expressionList();
            if (expressionList == null)
            {
                return result;
            }

            if (expressionList.children == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitListLiteralInitializerList");
            }


            IEnumerable<IParseTree> subtrees = new[] { (LSLParser.ExpressionContext)expressionList.children[0] };

            if (expressionList.children.Count > 1)
            {
                subtrees = subtrees.Concat(expressionList.children.Skip(1).Select(x =>
                {
                    var listTail = ((LSLParser.ExpressionListTailContext)x);
                    result.AddCommaRange(new LSLSourceCodeRange(listTail.comma));
                    return listTail.expression();
                }));
            }

            var expressionContexts = subtrees.ToList();


            for (var i = 0; i < expressionContexts.Count; i++)
            {
                var expression = VisitTopOfExpression((LSLParser.ExpressionContext)expressionContexts[i]);

                result.AddExpression(expression);

                if (!expression.HasErrors && !ExpressionValidator.ValidListContent(expression))
                {
                    SyntaxErrorListener.InvalidListContent(expression.SourceCodeRange, i, expression);

                    result.HasErrors = true;
                }
                else if (expression.HasErrors)
                {
                    result.HasErrors = true;
                }
            }


            return ReturnFromVisit(context, result);
        }


        private ILSLSyntaxTreeNode VisitForLoopAfterthoughts(LSLParser.OptionalExpressionListContext context)
        {
            if (context == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitForLoopAfterthoughts");
            }


            var result = new LSLExpressionListNode(context, LSLExpressionListType.ForLoopAfterthoughts);

            var expressionList = context.expressionList();
            if (expressionList == null)
            {
                return result;
            }

            if (expressionList.children == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitForLoopAfterthoughts");
            }

            IEnumerable<IParseTree> subtrees = new[] { (LSLParser.ExpressionContext)expressionList.children[0] };

            if (expressionList.children.Count > 1)
            {
                subtrees = subtrees.Concat(expressionList.children.Skip(1).Select(x =>
                {
                    var listTail = ((LSLParser.ExpressionListTailContext)x);
                    result.AddCommaRange(new LSLSourceCodeRange(listTail.comma));
                    return listTail.expression();
                }));
            }

            var expressionContexts = subtrees.ToList();


            var expressionIndex = 0;

            foreach (var expressionContext in expressionContexts)
            {
                var ctx = (LSLParser.ExpressionContext)expressionContext;
                var expressionHasEffect = DoesExpressionHaveEffect(ctx);

                if (!expressionHasEffect)
                {
                    SyntaxWarningListener.ForLoopAfterthoughtHasNoEffect(
                        new LSLSourceCodeRange(ctx),
                        expressionIndex, expressionContexts.Count);
                }

                var expression = VisitTopOfExpression(ctx);
                if (expression.HasErrors)
                {
                    result.HasErrors = true;
                }

                result.AddExpression(expression);

                expressionIndex++;
            }


            return ReturnFromVisit(context, result);
        }




        private ILSLSyntaxTreeNode VisitForLoopInitExpressions(LSLParser.OptionalExpressionListContext context)
        {
            if (context == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitForLoopInitExpressions");
            }


            var result = new LSLExpressionListNode(context, LSLExpressionListType.ForLoopInitExpressions);

            var expressionList = context.expressionList();
            if (expressionList == null)
            {
                return result;
            }

            if (expressionList.children == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitForLoopInitExpressions");
            }




            IEnumerable<IParseTree> subtrees = new[] { (LSLParser.ExpressionContext)expressionList.children[0] };

            if (expressionList.children.Count > 1)
            {
                subtrees = subtrees.Concat(expressionList.children.Skip(1).Select(x =>
                {
                    var listTail = ((LSLParser.ExpressionListTailContext)x);
                    result.AddCommaRange(new LSLSourceCodeRange(listTail.comma));
                    return listTail.expression();
                }));
            }

            var expressionContexts = subtrees.ToList();



            var expressionIndex = 0;

            foreach (var expressionContext in expressionContexts)
            {
                var ctx = (LSLParser.ExpressionContext)expressionContext;
                var expressionHasEffect = DoesExpressionHaveEffect(ctx);

                if (!expressionHasEffect)
                {
                    SyntaxWarningListener.ForLoopInitExpressionHasNoEffect(
                        new LSLSourceCodeRange(ctx),
                        expressionIndex, expressionContexts.Count);
                }

                var expression = VisitTopOfExpression(ctx);
                if (expression.HasErrors)
                {
                    result.HasErrors = true;
                }

                result.AddExpression(expression);

                expressionIndex++;
            }


            return ReturnFromVisit(context, result);
        }


        private ILSLSyntaxTreeNode VisitFunctionCallParameters(LSLParser.OptionalExpressionListContext context,
            LSLExpressionListType type)
        {
            if (context == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitFunctionCallParameters");
            }


            if (type != LSLExpressionListType.UserFunctionCallParameters &&
                type != LSLExpressionListType.LibraryFunctionCallParameters)
            {
                throw new LSLCodeValidatorInternalError(
                    "VisitFunctionCallParameters LSLExpressionListType was an invalid type");
            }

            var result = new LSLExpressionListNode(context, type);

            var expressionList = context.expressionList();

            if (expressionList == null)
            {
                return result;
            }

            if (expressionList.children == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitFunctionCallParameters");
            }



            IEnumerable<IParseTree> subtrees = new[] { (LSLParser.ExpressionContext)expressionList.children[0] };

            if (expressionList.children.Count > 1)
            {
                subtrees = subtrees.Concat(expressionList.children.Skip(1).Select(x =>
                {
                    var listTail = ((LSLParser.ExpressionListTailContext)x);
                    result.AddCommaRange(new LSLSourceCodeRange(listTail.comma));
                    return listTail.expression();
                }));
            }

            var expressionContexts = subtrees.ToList();



            foreach (var expressionContext in expressionContexts)
            {
                var expression = VisitTopOfExpression((LSLParser.ExpressionContext)expressionContext);
                if (expression.HasErrors)
                {
                    result.HasErrors = true;
                }
                result.AddExpression(expression);
            }


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpressionList(LSLParser.ExpressionListContext context)
        {
            throw new LSLCodeValidatorInternalError("Expression list are not expected to be visited directly");
        }


        public override ILSLSyntaxTreeNode VisitTerminal(ITerminalNode node)
        {
            throw new LSLCodeValidatorInternalError("Terminals are not expected to be visited");
        }


        public override ILSLSyntaxTreeNode VisitCompilationUnit(LSLParser.CompilationUnitContext context)
        {
            if (context == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitCompilationUnit");
            }


            var compilationUnitPrePass = ScopingManager.EnterCompilationUnit(context);

            if (compilationUnitPrePass.HasErrors)
            {
                ScopingManager.ExitCompilationUnit();
                return ReturnFromVisit(context, LSLCompilationUnitNode
                    .GetError(new LSLSourceCodeRange(context)));
            }

            var result = new LSLCompilationUnitNode(context);


            var syntaxMessagePrioritizer = new LSLSyntaxListenerPriorityQueue(SyntaxErrorListener, SyntaxWarningListener);

            OverrideSyntaxErrorListener(syntaxMessagePrioritizer);
            OverrideSyntaxWarningListener(syntaxMessagePrioritizer);

            foreach (var global in context.globalVariableDeclaration())
            {
                var child = VisitGlobalVariableDeclaration(global) as LSLVariableDeclarationNode;

                if (child == null)
                {
                    throw LSLCodeValidatorInternalError
                        .VisitReturnTypeException("VisitGlobalVariableDeclaration", typeof(LSLVariableDeclarationNode));
                }

                if (child.HasErrors)
                {
                    result.HasErrors = true;
                }
                else
                {
                    //prevent global variable declarations from being added
                    //to the global variable pool, they are effectively undefined if there is an error
                    //in their definition
                    result.AddVariableDeclaration(child);
                }
            }


            foreach (var item in context.functionDeclaration())
            {
                var child = VisitFunctionDeclaration(item) as LSLFunctionDeclarationNode;

                if (child == null)
                {
                    throw LSLCodeValidatorInternalError
                        .VisitReturnTypeException("VisitFunctionDeclaration", typeof(LSLFunctionDeclarationNode));
                }

                if (child.HasErrors)
                {
                    result.HasErrors = true;
                }

                //function definitions are guaranteed not to have duplicate definitions in a pre-pass
                //also guaranteed to have syntacticly correct parameter definitions
                result.AddFunctionDeclaration(child);
            }


            var defaultState = VisitDefaultState(context.defaultState()) as LSLStateScopeNode;


            if (defaultState == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitReturnTypeException("VisitDefaultState", typeof(LSLStateScopeNode));
            }


            result.DefaultState = defaultState;

            if (result.DefaultState.HasErrors)
            {
                result.HasErrors = true;
            }


            foreach (var item in context.definedState())
            {
                var child = VisitDefinedState(item) as LSLStateScopeNode;

                if (child == null)
                {
                    throw LSLCodeValidatorInternalError
                        .VisitReturnTypeException("VisitDefinedState", typeof(LSLStateScopeNode));
                }


                if (child.HasErrors)
                {
                    result.HasErrors = true;
                }

                result.AddStateDeclaration(child);
            }

            ScopingManager.ExitCompilationUnit();


            foreach (var fun in result.FunctionDeclarations.Where(x => x.References.Count == 0))
            {
                SyntaxWarningListener.FunctionNeverUsed(fun.SourceCodeRange, fun);
            }

            foreach (var gvar in result.GlobalVariableDeclarations.Where(x => x.References.Count == 0))
            {
                SyntaxWarningListener.GlobalVariableNeverUsed(gvar.SourceCodeRange, gvar);
            }

            syntaxMessagePrioritizer.InvokeQueuedActions();

            RemoveSyntaxErrorListenerOverride();
            RemoveSyntaxWarningListenerOverride();

            return ReturnFromVisit(context, result);
        }

        #region VariableDeclarationVisitors

        public override ILSLSyntaxTreeNode VisitGlobalVariableDeclaration(
            LSLParser.GlobalVariableDeclarationContext context)
        {
            if (context == null || Utility.AnyNull(context.variable_type, context.variable_name))
            {
                throw new
                    LSLCodeValidatorInternalError(
                    "VisitGlobalVariableDeclaration context state pre-requisite error");
            }

            return ReturnFromVisit(context, VisitVariableDeclaration(context, context.variable_type,
                context.variable_name,
                context.variable_value,
                LSLVariableScope.Global));
        }


        public override ILSLSyntaxTreeNode VisitLocalVariableDeclaration(
            LSLParser.LocalVariableDeclarationContext context)
        {
            if (context == null || Utility.AnyNull(context.variable_type, context.variable_name))
            {
                throw new
                    LSLCodeValidatorInternalError(
                    "VisitLocalVariableDeclaration context state pre-requisite error");
            }

            return ReturnFromVisit(context, VisitVariableDeclaration(context, context.variable_type,
                context.variable_name,
                context.variable_value,
                LSLVariableScope.Local));
        }


        private ILSLSyntaxTreeNode VisitVariableDeclaration(ParserRuleContext context, IToken typeToken,
            IToken nameToken,
            LSLParser.ExpressionContext declarationExpression, LSLVariableScope declarationScope)
        {
            var variableType = LSLTypeTools.FromLSLTypeString(typeToken.Text);


            if (MainLibraryDataProvider.LibraryConstantExist(nameToken.Text))
            {
                SyntaxErrorListener.RedefinedStandardLibraryConstant(
                    new LSLSourceCodeRange(nameToken), variableType,
                    MainLibraryDataProvider.GetLibraryConstantSignature(nameToken.Text));


                return LSLVariableDeclarationNode.GetError(new LSLSourceCodeRange(context));
            }


            if (ScopingManager.CanVariableBeDefined(nameToken.Text, declarationScope))
            {
                SyntaxErrorListener.VariableRedefined(
                    new LSLSourceCodeRange(nameToken), variableType, nameToken.Text);

                return LSLVariableDeclarationNode.GetError(new LSLSourceCodeRange(context));
            }


            LSLVariableDeclarationNode variable;


            if (declarationExpression != null)
            {
                var expression = VisitTopOfExpression(declarationExpression);

                if (expression.HasErrors)
                {
                    return LSLVariableDeclarationNode.GetError(new LSLSourceCodeRange(context));
                }


                if (declarationScope == LSLVariableScope.Local)
                {
                    variable = LSLVariableDeclarationNode.CreateVar(
                        (LSLParser.LocalVariableDeclarationContext)context, expression);
                }
                else
                {
                    variable = LSLVariableDeclarationNode.CreateVar(
                        (LSLParser.GlobalVariableDeclarationContext)context, expression);
                }


                var valid = ExpressionValidator.ValidateBinaryOperation(
                    variable.VariableNode, LSLBinaryOperationType.Assign,
                    variable.DeclarationExpression);


                if (!valid.IsValid)
                {
                    SyntaxErrorListener.TypeMismatchInVariableDeclaration(
                        new LSLSourceCodeRange(typeToken),
                        variableType, variable.DeclarationExpression);

                    return LSLVariableDeclarationNode.GetError(new LSLSourceCodeRange(context));
                }
            }
            else
            {
                //variable created with no initializer expression
                if (declarationScope == LSLVariableScope.Local)
                {
                    variable = LSLVariableDeclarationNode.CreateVar(
                        (LSLParser.LocalVariableDeclarationContext)context);
                }
                else
                {
                    variable = LSLVariableDeclarationNode.CreateVar(
                        (LSLParser.GlobalVariableDeclarationContext)context);
                }
            }


            ScopingManager.DefineVariable(variable, declarationScope);


            if (declarationScope == LSLVariableScope.Local)
            {
                WarnIfLocalVariableHidesParameterOrGlobal(variable);
            }


            return variable;
        }


        private void WarnIfLocalVariableHidesParameterOrGlobal(LSLVariableDeclarationNode variable)
        {
            if (ScopingManager.ParameterDefined(variable.Name))
            {
                if (ScopingManager.InsideFunctionBody)
                {
                    var parameter =
                        ScopingManager.CurrentFunctionBodySignature.ParameterListNode.Parameters.Single(
                            x => (x.Name == variable.Name));


                    SyntaxWarningListener.LocalVariableHidesParameter(variable.SourceCodeRange,
                        ScopingManager.CurrentFunctionBodySignature, variable, parameter);
                }
                else
                {
                    var parameter =
                        ScopingManager.CurrentEventHandlerSignature
                            .ParameterListNode.Parameters.Single(x => (x.Name == variable.Name));

                    SyntaxWarningListener.LocalVariableHidesParameter(variable.SourceCodeRange,
                        ScopingManager.CurrentEventHandlerSignature, variable, parameter);
                }
            }


            if (ScopingManager.GlobalVariableDefined(variable.Name))
            {
                if (ScopingManager.InsideFunctionBody)
                {
                    SyntaxWarningListener.LocalVariableHidesGlobalVariable(variable.SourceCodeRange,
                        ScopingManager.CurrentFunctionBodySignature, variable,
                        ScopingManager.ResolveGlobalVariable(variable.Name));
                }
                else
                {
                    SyntaxWarningListener.LocalVariableHidesGlobalVariable(variable.SourceCodeRange,
                        ScopingManager.CurrentEventHandlerSignature, variable,
                        ScopingManager.ResolveGlobalVariable(variable.Name));
                }
            }
        }

        #endregion

        #region StateBodyVisitors

        public override ILSLSyntaxTreeNode VisitDefaultState(LSLParser.DefaultStateContext context)
        {
            if (context == null || context.children == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitDefaultState");
            }

            var result = new LSLStateScopeNode(context);


            foreach (var x in context.eventHandler())
            {
                var child = VisitEventHandler(x) as LSLEventHandlerNode;

                if (child == null)
                {
                    throw LSLCodeValidatorInternalError.
                        VisitReturnTypeException("VisitEventHandler", typeof(LSLEventHandlerNode));
                }

                if (child.HasErrors)
                {
                    result.HasErrors = true;
                }


                result.AddEventHandler(child);
            }

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitDefinedState(LSLParser.DefinedStateContext context)
        {
            if (context == null || Utility.AnyNull(context.children, context.state_name))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitDefinedState");
            }


            if (!ScopingManager.StatePreDefined(context.state_name.Text))
            {
                //state was not defined in the pre-pass the predefines all states
                //in the compilation unit, this is an internal error and should never happen.
                throw new LSLCodeValidatorInternalError(
                    "VisitDefinedState, ScopingManager.StateDefined returned that state was undefined " +
                    "when it should have been pre-defined in a pre-pass");
            }


            var result = new LSLStateScopeNode(context);


            foreach (var x in context.eventHandler())
            {
                var child = VisitEventHandler(x) as LSLEventHandlerNode;

                if (child == null)
                {
                    throw LSLCodeValidatorInternalError.
                        VisitReturnTypeException("VisitEventHandler", typeof(LSLEventHandlerNode));
                }


                if (child.HasErrors)
                {
                    result.HasErrors = true;
                }


                result.AddEventHandler(child);
            }


            ScopingManager.SetStateNode(context.state_name.Text, result);

            return ReturnFromVisit(context, result);
        }

        #endregion

        #region BranchStructureVisitors

        public override ILSLSyntaxTreeNode VisitIfStatement(LSLParser.IfStatementContext context)
        {
            if (context == null || context.code == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitIfStatement");
            }

            var isError = false;
            ILSLExprNode expression;

            if (context.condition == null)
            {
                //creating a valid if statement node even if the condition is null
                //allows return path verification to continue, also various other error checks
                //make a dummy expression value for the condition node, a constant integer literal

                SyntaxErrorListener.MissingConditionalExpression(
                    new LSLSourceCodeRange(context),
                    LSLConditionalStatementType.If);

                isError = true;

                expression = new LSLDummyExpr
                {
                    Type = LSLType.Integer,
                    ExpressionType = LSLExpressionType.Literal,
                    IsConstant = true
                };
            }
            else
            {
                expression = VisitTopOfExpression(context.condition);

                if (expression.HasErrors)
                {
                    isError = true;
                }
                else if (!ExpressionValidator.ValidBooleanConditional(expression))
                {
                    SyntaxErrorListener.IfConditionNotValidType(
                        new LSLSourceCodeRange(context.condition),
                        expression
                        );


                    isError = true;
                }

                if (!isError && expression.IsConstant)
                {
                    SyntaxWarningListener.ConditionalExpressionIsConstant(new LSLSourceCodeRange(context.condition), LSLConditionalStatementType.ElseIf);
                }
            }


            //if (!expression.HasErrors)
            //    EnterBranchStatement(expression.IsConstant);

            var code = VisitCodeScopeOrSingleBlockStatement(context.code) as LSLCodeScopeNode;

            //if (!expression.HasErrors)
            //    ExitBranchStatement();


            if (code == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitReturnTypeException("VisitCodeScopeOrSingleBlockStatement",
                        typeof(LSLCodeScopeNode));
            }


            if (code.HasErrors)
            {
                isError = true;
            }


            var result = new LSLIfStatementNode(context, code, expression)
            {
                HasErrors = isError
            };

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitElseIfStatement(LSLParser.ElseIfStatementContext context)
        {
            if (context == null || context.code == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitElseIfStatement");
            }


            var isError = false;
            ILSLExprNode expression;

            if (context.condition == null)
            {
                //creating a valid if statement node even if the condition is null
                //allows return path verification to continue, also various other error checks
                //make a dummy expression value for the condition node, a constant integer literal

                SyntaxErrorListener.MissingConditionalExpression(new LSLSourceCodeRange(context),
                    LSLConditionalStatementType.ElseIf);

                isError = true;

                expression = new LSLDummyExpr
                {
                    Type = LSLType.Integer,
                    ExpressionType = LSLExpressionType.Literal,
                    IsConstant = true
                };
            }
            else
            {
                expression = VisitTopOfExpression(context.condition);


                if (expression.HasErrors)
                {
                    isError = true;
                }
                else if (!ExpressionValidator.ValidBooleanConditional(expression))
                {
                    SyntaxErrorListener.ElseIfConditionNotValidType(
                        new LSLSourceCodeRange(context.condition),
                        expression
                        );


                    isError = true;
                }

                
                if (!isError && expression.IsConstant)
                {
                    SyntaxWarningListener.ConditionalExpressionIsConstant(new LSLSourceCodeRange(context.condition), LSLConditionalStatementType.ElseIf);
                }
            }


            //if (!expression.HasErrors)
            //    EnterBranchStatement(expression.IsConstant);

            var code = VisitCodeScopeOrSingleBlockStatement(context.code) as LSLCodeScopeNode;

            //if (!expression.HasErrors)
            //    ExitBranchStatement();


            if (code == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitReturnTypeException("VisitCodeScopeOrSingleBlockStatement",
                        typeof(LSLCodeScopeNode));
            }


            if (code.HasErrors)
            {
                isError = true;
            }


            var result = new LSLElseIfStatementNode(context, code, expression)
            {
                HasErrors = isError
            };


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitElseStatement(LSLParser.ElseStatementContext context)
        {
            if (context == null || context.code == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitElseStatement");
            }

            var currentControlStatement = ScopingManager.CurrentControlStatement;

            var elseBranchIsConstant =
                currentControlStatement.IfStatement.IsConstantBranch &&
                currentControlStatement.ElseIfStatements.All(x => x.IsConstantBranch);

            //EnterBranchStatement(elseBranchIsConstant);

            var code = VisitCodeScopeOrSingleBlockStatement(context.code) as LSLCodeScopeNode;

            //ExitBranchStatement();

            if (code == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitReturnTypeException("VisitCodeScopeOrSingleBlockStatement",
                        typeof(LSLCodeScopeNode));
            }


            var result = new LSLElseStatementNode(context, code, elseBranchIsConstant)
            {
                HasErrors = code.HasErrors
            };


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitControlStructure(LSLParser.ControlStructureContext context)
        {
            if (context == null || context.children == null)
            {
                throw LSLCodeValidatorInternalError.VisitContextInvalidState("VisitControlStructure");
            }


            var result = new LSLControlStatementNode(context, InSingleStatementBlock);

            ScopingManager.EnterControlStatement(result);


            foreach (var child in context.children.Select(x => Visit(x) as ILSLBranchStatementNode))
            {
                if (child == null)
                {
                    throw new LSLCodeValidatorInternalError(
                        "Child visit in VisitControlStructure did not return an ILSLBranchStatementNode type");
                }

                if (child.HasErrors)
                {
                    result.HasErrors = true;
                }


                var ifNode = child as LSLIfStatementNode;
                if (ifNode != null)
                {
                    result.IfStatement = ifNode;
                    continue;
                }
                var elseIfNode = child as LSLElseIfStatementNode;
                if (elseIfNode != null)
                {
                    result.AddElseIfStatement(elseIfNode);
                    continue;
                }
                var elseNode = child as LSLElseStatementNode;
                if (elseNode != null)
                {
                    result.ElseStatement = elseNode;
                    continue;
                }

                throw new
                    LSLCodeValidatorInternalError(
                    "VisitControlStructure child node Visit did not return proper type");
            }

            ScopingManager.ExitControlStatement();


            return ReturnFromVisit(context, result);
        }

        #endregion

        #region CodeScopeVisitors

        private bool ValidateEventHandlerReturnPath(LSLCodeScopeNode codeScope)
        {
            if (codeScope.HasDeadStatementNodes)
            {
                foreach (var deadSegment in codeScope.DeadCodeSegments)
                {
                    SyntaxWarningListener.DeadCodeDetected(
                        deadSegment.SourceCodeRange,
                        ScopingManager.CurrentEventHandlerSignature, deadSegment);
                }
            }

            return false;
        }


        private bool ValidateCommonCodeScopeReturnPath(LSLCodeScopeNode codeScope)
        {
            if (codeScope.HasDeadStatementNodes && ScopingManager.InsideEventHandlerBody)
            {
                foreach (var deadSegment in codeScope.DeadCodeSegments)
                {
                    SyntaxWarningListener.DeadCodeDetected(
                        deadSegment.SourceCodeRange,
                        ScopingManager.CurrentEventHandlerSignature, deadSegment);
                }
            }

            if (codeScope.HasDeadStatementNodes && ScopingManager.InsideFunctionBody)
            {
                foreach (var deadSegment in codeScope.DeadCodeSegments)
                {
                    SyntaxWarningListener.DeadCodeDetected(
                        deadSegment.SourceCodeRange,
                        ScopingManager.CurrentFunctionBodySignature, deadSegment);
                }
            }

            return false;
        }


        private bool ValidateFunctionReturnPath(LSLCodeScopeNode codeScope)
        {
            var currentFunctionPredefinition = ScopingManager.CurrentFunctionBodySignature;

            var isError = false;

            var context = ScopingManager.CurrentFunctionContext;

            if (!codeScope.HasReturnPath && currentFunctionPredefinition.ReturnType != LSLType.Void)
            {
                //if the function is not a void function and it has no return path, then its an error
                SyntaxErrorListener.NotAllCodePathsReturnAValue(
                    new LSLSourceCodeRange(context),
                    currentFunctionPredefinition);

                isError = true;
            }


            if (codeScope.HasDeadStatementNodes && currentFunctionPredefinition.ReturnType != LSLType.Void)
            {
                foreach (var deadSegment in codeScope.DeadCodeSegments)
                {
                    if (deadSegment.DeadCodeType == LSLDeadCodeType.AfterReturnPath)
                    {
                        SyntaxErrorListener.DeadCodeAfterReturnPathDetected(
                            deadSegment.SourceCodeRange, currentFunctionPredefinition, deadSegment);
                        isError = true;
                    }
                    else
                    {
                        SyntaxWarningListener.DeadCodeDetected(
                            deadSegment.SourceCodeRange,
                            currentFunctionPredefinition, deadSegment);
                    }
                }
            }
            else if (codeScope.HasDeadStatementNodes)
            {
                //if the function has dead code somewhere in the function 
                //(void functions have jump statement analysis enabled), and its a void function
                //then its always just a warning
                foreach (var deadSegment in codeScope.DeadCodeSegments)
                {
                    SyntaxWarningListener.DeadCodeDetected(
                        deadSegment.SourceCodeRange,
                        currentFunctionPredefinition, deadSegment);
                }
            }

            return isError;
        }


        private void WarnUnusedVariablesInFunction()
        {
            foreach (var v in ScopingManager.AllParametersInScope.Where(x => x.References.Count == 0))
            {
                SyntaxWarningListener.FunctionParameterNeverUsed(v.SourceCodeRange, v,
                    ScopingManager.CurrentFunctionBodySignature);
            }

            WarnUnusedVariablesInScope();
        }


        private void WarnUnusedVariablesInScope()
        {
            if (ScopingManager.InsideFunctionBody)
            {
                foreach (var v in ScopingManager.AllLocalVariablesInScope.Where(x => x.References.Count == 0))
                {
                    SyntaxWarningListener.LocalVariableNeverUsed(v.SourceCodeRange, v,
                        ScopingManager.CurrentFunctionBodySignature);
                }
            }
            else
            {
                foreach (var v in ScopingManager.AllLocalVariablesInScope.Where(x => x.References.Count == 0))
                {
                    SyntaxWarningListener.LocalVariableNeverUsed(v.SourceCodeRange, v,
                        ScopingManager.CurrentEventHandlerSignature);
                }
            }
        }


        public override ILSLSyntaxTreeNode VisitCodeScope(LSLParser.CodeScopeContext context)
        {
            if (context == null || context.children == null)
            {
                throw new
                    LSLCodeValidatorInternalError("VisitCodeScope did not meet context state pre-requisites");
            }


            ScopingManager.EnterCodeScopeAfterPrePass(context);


            ScopingManager.IncrementScopeId();


            var result = new LSLCodeScopeNode(context, ScopingManager.CurrentScopeId, ScopingManager.CurrentCodeScopeType);


            var codeStatementContexts = context.codeStatement();


            foreach (var codeStatementContext in codeStatementContexts)
            {
                var child = VisitCodeStatement(codeStatementContext) as ILSLCodeStatement;

                if (child == null)
                {
                    throw LSLCodeValidatorInternalError
                        .VisitReturnTypeException("VisitCodeStatement", typeof(ILSLCodeStatement));
                }

                if (child.HasErrors)
                {
                    result.HasErrors = true;
                }

                result.AddCodeStatement(child, codeStatementContext);
            }

            result.EndScope();


            if (ScopingManager.CurrentCodeScopeType == LSLCodeScopeType.FunctionCodeRoot)
            {
                WarnUnusedVariablesInFunction();
                result.HasErrors = ValidateFunctionReturnPath(result) || result.HasErrors;
            }
            else if (ScopingManager.CurrentCodeScopeType == LSLCodeScopeType.EventHandlerCodeRoot)
            {
                WarnUnusedVariablesInScope();
                result.HasErrors = ValidateEventHandlerReturnPath(result) || result.HasErrors;
            }
            else
            {
                WarnUnusedVariablesInScope();
                result.HasErrors = ValidateCommonCodeScopeReturnPath(result) || result.HasErrors;
            }


            ScopingManager.ExitCodeScopeAfterPrePass();


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitCodeScopeOrSingleBlockStatement(
            LSLParser.CodeScopeOrSingleBlockStatementContext context)
        {
            if (context == null || !Utility.OnlyOneNotNull(context.code, context.statement))
            {
                throw
                    LSLCodeValidatorInternalError.VisitContextInvalidState("VisitCodeScopeOrSingleBlockStatement");
            }


            if (context.code != null)
            {
                var code = VisitCodeScope(context.code) as LSLCodeScopeNode;

                if (code == null)
                {
                    throw LSLCodeValidatorInternalError
                        .VisitReturnTypeException("VisitCodeScope", typeof(LSLCodeScopeNode));
                }

                return ReturnFromVisit(context, code);
            }

            if (context.statement != null)
            {
                ScopingManager.EnterSingleStatementBlock(context.statement);

                var code = VisitCodeStatement(context.statement) as ILSLCodeStatement;

                ScopingManager.ExitSingleStatementBlock();


                if (code == null)
                {
                    throw LSLCodeValidatorInternalError
                        .VisitReturnTypeException("VisitCodeStatement", typeof(ILSLCodeStatement));
                }


                ScopingManager.IncrementScopeId();


                var result = new LSLCodeScopeNode(context.statement, ScopingManager.CurrentScopeId, ScopingManager.CurrentCodeScopeType);

            
                result.AddCodeStatement(code, context.statement);
                result.EndScope();


                return ReturnFromVisit(context, result);
            }

            //it should never get here
            throw new LSLCodeValidatorInternalError("VisitCodeScopeOrSingleBlockStatement ended in invalid state");
        }

        #endregion

        #region EventHandlerAndFunctionDeclarationVisitors

        public override ILSLSyntaxTreeNode VisitEventHandler(LSLParser.EventHandlerContext context)
        {
            if (context == null || context.code == null)
            {
                throw LSLCodeValidatorInternalError.VisitContextInvalidState("VisitEventHandler");
            }


            var parameterList = LSLParameterListNode.BuildDirectlyFromContext(context.parameters);


            var isError = parameterList.HasErrors;


            var eventHandlerSignature = new LSLParsedEventHandlerSignature(context.handler_name.Text, parameterList);


            //Warn about parameters that hide global variables
            foreach (var parameter in parameterList.Parameters)
            {
                if (ScopingManager.GlobalVariableDefined(parameter.Name))
                {
                    SyntaxWarningListener.ParameterHidesGlobalVariable(parameter.SourceCodeRange,
                        eventHandlerSignature,
                        parameter,
                        ScopingManager.ResolveVariable(parameter.Name));
                }
            }


            if (!MainLibraryDataProvider.EventHandlerExist(context.handler_name.Text))
            {
                var location = new LSLSourceCodeRange(context.handler_name);

                SyntaxErrorListener.UnknownEventHandlerDeclared(location, eventHandlerSignature);

                isError = true;
            }

            var librarySignature = MainLibraryDataProvider.GetEventHandlerSignature(context.handler_name.Text);


            if (!eventHandlerSignature.SignatureMatches(librarySignature))
            {
                var location = new LSLSourceCodeRange(context.handler_name);

                SyntaxErrorListener.IncorrectEventHandlerSignature(
                    location,
                    eventHandlerSignature,
                    librarySignature
                    );

                isError = true;
            }


            var eventPrePass = ScopingManager.EnterEventScope(context, eventHandlerSignature);

            if (eventPrePass.HasErrors)
            {
                ScopingManager.ExitEventScope();
                return ReturnFromVisit(context, LSLEventHandlerNode
                    .GetError(new LSLSourceCodeRange(context)));
            }


            var codeScope = VisitCodeScope(context.codeScope()) as LSLCodeScopeNode;


            ScopingManager.ExitEventScope();


            if (codeScope == null)
            {
                throw LSLCodeValidatorInternalError.VisitReturnTypeException("VisitCodeScope",
                    typeof(LSLCodeScopeNode));
            }


            isError = codeScope.HasErrors || isError;


            var result = new LSLEventHandlerNode(context, parameterList, codeScope) { HasErrors = isError };
            ScopingManager.ResetScopeId();

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitFunctionDeclaration(LSLParser.FunctionDeclarationContext context)
        {
            if (context == null || Utility.AnyNull(context.function_name, context.code))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitFunctionDeclaration");
            }


            if (!ScopingManager.FunctionIsPreDefined(context.function_name.Text))
            {
                //function was not defined in the pre-pass that parses all function and state declarations,
                //this should never happen at this point but check anyway when we enter the function
                //declaration a second time
                return ReturnFromVisit(context, LSLFunctionDeclarationNode
                    .GetError(new LSLSourceCodeRange(context)));
            }


            var currentFunctionPredefinition = ScopingManager.ResolveFunctionPreDefine(context.function_name.Text);


            //Warn about parameters that hide global variables
            foreach (var parameter in currentFunctionPredefinition.ParameterListNode.Parameters)
            {
                if (ScopingManager.GlobalVariableDefined(parameter.Name))
                {
                    SyntaxWarningListener.ParameterHidesGlobalVariable(parameter.SourceCodeRange,
                        currentFunctionPredefinition,
                        parameter,
                        ScopingManager.ResolveVariable(parameter.Name));
                }
            }


            var functionPrePass = ScopingManager.EnterFunctionScope(context, currentFunctionPredefinition);

            if (functionPrePass.HasErrors)
            {
                ScopingManager.ExitFunctionScope();
                return ReturnFromVisit(context, LSLFunctionDeclarationNode
                    .GetError(new LSLSourceCodeRange(context)));
            }


            var codeScope = VisitCodeScope(context.codeScope()) as LSLCodeScopeNode;


            ScopingManager.ExitFunctionScope();


            if (codeScope == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitReturnTypeException("VisitCodeScope", typeof(LSLCodeScopeNode));
            }


            var result = new LSLFunctionDeclarationNode(context, currentFunctionPredefinition.ParameterListNode,
                codeScope)
            {
                HasErrors = codeScope.HasErrors
            };

            currentFunctionPredefinition.GiveDefinition(result);


            AddReferencesWhereFunctionNotYetDefined(result);


            ScopingManager.ResetScopeId();


            return ReturnFromVisit(context, result);
        }


        /// <summary>
        ///     Calls .AddReference(result) on functionSignature.DefinitionNode if it is not null (the function was defined before
        ///     the call was encountered)
        ///     Otherwise, appends the function call node to the dictionary of list _referencesToNotYetDefinedFunctions
        ///     The key is the function name, the list associated with the key is all the references to it before it was defined.
        ///     Function signatures are defined in a pre-pass, and later there body's are added;
        ///     If the function is undefined anywhere, this function is never utilized
        /// </summary>
        /// <param name="functionSignature">
        ///     The function signature that was pre-defined in the pre-pass for the function we are
        ///     referencing
        /// </param>
        /// <param name="node">The function call node that was constructed for this function reference</param>
        private void AddReferenceToFunctionDefinition(LSLPreDefinedFunctionSignature functionSignature,
            LSLFunctionCallNode node)
        {
            //if the definition node is not null, then this pre-definition has already been given
            //a definition by the time we referenced it
            if (functionSignature.DefinitionNode != null)
            {
                functionSignature.DefinitionNode.AddReference(node);
            }
            else
            {
                //otherwise, store it for later, so that when the function is defined
                //we can add it as a reference to the function definition node
                if (_referencesToNotYetDefinedFunctions.ContainsKey(functionSignature.Name))
                {
                    _referencesToNotYetDefinedFunctions[functionSignature.Name].Add(node);
                }
                else
                {
                    _referencesToNotYetDefinedFunctions.Add(functionSignature.Name, new List<LSLFunctionCallNode> { node });
                }
            }
        }


        /// <summary>
        ///     Called when a function is finally defined, if there are any references to it that were made before
        ///     its node was constructed (before it was defined in source) we add them here from the
        ///     _referencesToNotYetDefinedFunctions
        ///     dictionary.  Its looked up by name and all the references in the associated list (if its an entry in the
        ///     dictionary) are
        ///     added to the definition via node.AddReference
        /// </summary>
        /// <param name="node">The function declaration node to add possible pre-definition references to</param>
        private void AddReferencesWhereFunctionNotYetDefined(LSLFunctionDeclarationNode node)
        {
            //add all the references to the node object once we are defined
            //this only occurs when this function was referenced before its definition
            //like in a function defined above it
            if (_referencesToNotYetDefinedFunctions.ContainsKey(node.Name))
            {
                var references = _referencesToNotYetDefinedFunctions[node.Name];
                foreach (var lslFunctionCallNode in references)
                {
                    node.AddReference(lslFunctionCallNode);
                }
            }
        }

        #endregion

        #region LoopStructureVisitors

        public override ILSLSyntaxTreeNode VisitLoopStructure(LSLParser.LoopStructureContext context)
        {
            if (context == null ||
                context.children == null ||
                context.ChildCount == 0 ||
                context.children[0] == null)
            {
                throw LSLCodeValidatorInternalError.VisitContextInvalidState("VisitLoopStructure");
            }

            return ReturnFromVisit(context, Visit(context.children[0]));
        }


        public override ILSLSyntaxTreeNode VisitDoLoop(LSLParser.DoLoopContext context)
        {
            if (context == null || Utility.AnyNull(context.loop_keyword, context.code))
            {
                throw LSLCodeValidatorInternalError.VisitContextInvalidState("VisitDoLoop");
            }


            var isError = false;


            ILSLExprNode loopCondition;
            if (context.loop_condition != null)
            {
                loopCondition = VisitTopOfExpression(context.loop_condition);

                if (loopCondition.HasErrors)
                {
                    isError = true;
                }
                else if (!ExpressionValidator.ValidBooleanConditional(loopCondition))
                {
                    SyntaxErrorListener.DoLoopConditionNotValidType(
                        new LSLSourceCodeRange(context.loop_condition),
                        loopCondition);

                    isError = true;
                }

                if (!isError && loopCondition.IsConstant)
                {
                    SyntaxWarningListener.ConditionalExpressionIsConstant(loopCondition.SourceCodeRange, LSLConditionalStatementType.For);
                }
            }
            else
            {
                SyntaxErrorListener.MissingConditionalExpression(new LSLSourceCodeRange(context),
                    LSLConditionalStatementType.DoWhile);

                //make a dummy expression, just treat it as a constant integer literal
                //so return path verification can continue, and other error detections

                loopCondition = new LSLDummyExpr
                {
                    Type = LSLType.Integer,
                    ExpressionType = LSLExpressionType.Literal,
                    IsConstant = true
                };

                isError = true;
            }


            //if (!loopCondition.HasErrors)
            //    EnterLoopStatement(loopCondition.IsConstant);

            var code = VisitCodeScopeOrSingleBlockStatement(context.code) as LSLCodeScopeNode;

            //if (!loopCondition.HasErrors)
            //    ExitLoopStatement();

            if (code == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitReturnTypeException("VisitCodeScopeOrSingleBlockStatement", typeof(LSLCodeScopeNode));
            }


            var result = new LSLDoLoopNode(
                context,
                code,
                loopCondition,
                InSingleStatementBlock)
            {
                HasErrors = (isError || code.HasErrors)
            };


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitWhileLoop(LSLParser.WhileLoopContext context)
        {
            if (context == null || Utility.AnyNull(context.loop_keyword, context.code))
            {
                throw LSLCodeValidatorInternalError.VisitContextInvalidState("VisitWhileLoop");
            }


            var isError = false;

            ILSLExprNode loopCondition;
            if (context.loop_condition != null)
            {
                loopCondition = VisitTopOfExpression(context.loop_condition);

                if (loopCondition.HasErrors)
                {
                    isError = true;
                }
                else if (!ExpressionValidator.ValidBooleanConditional(loopCondition))
                {
                    SyntaxErrorListener.WhileLoopConditionNotValidType(
                        new LSLSourceCodeRange(context.loop_condition),
                        loopCondition);

                    isError = true;
                }


                if (!isError && loopCondition.IsConstant)
                {
                    SyntaxWarningListener.ConditionalExpressionIsConstant(loopCondition.SourceCodeRange, LSLConditionalStatementType.While);
                }
            }
            else
            {
                SyntaxErrorListener.MissingConditionalExpression(new LSLSourceCodeRange(context),
                    LSLConditionalStatementType.While);

                //make a dummy expression, just treat it as a constant integer literal
                //so return path verification can continue, and other error detections

                loopCondition = new LSLDummyExpr
                {
                    Type = LSLType.Integer,
                    ExpressionType = LSLExpressionType.Literal,
                    IsConstant = true
                };

                isError = true;
            }


            //if (!isError)
            //    EnterLoopStatement(loopCondition.IsConstant);

            var code = VisitCodeScopeOrSingleBlockStatement(context.code) as LSLCodeScopeNode;

            //if (!isError)
            //    ExitLoopStatement();


            if (code == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitReturnTypeException("VisitCodeScopeOrSingleBlockStatement",
                        typeof(LSLCodeScopeNode));
            }


            var result = new LSLWhileLoopNode(
                context,
                loopCondition,
                code,
                InSingleStatementBlock)
            {
                HasErrors = (code.HasErrors || isError)
            };

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitForLoop(LSLParser.ForLoopContext context)
        {
            if (context == null || Utility.AnyNull(context.loop_keyword, context.code))
            {
                throw LSLCodeValidatorInternalError.VisitContextInvalidState("VisitForLoop");
            }


            var isError = false;


            ILSLExpressionListNode loopInit = null;
            if (context.loop_init != null)
            {
                loopInit = VisitForLoopInitExpressions(context.loop_init) as LSLExpressionListNode;

                if (loopInit == null)
                {
                    throw LSLCodeValidatorInternalError
                        .VisitReturnTypeException("VisitOptionalExpressionList",
                            typeof(LSLExpressionListNode));
                }

                if (loopInit.HasErrors)
                {
                    isError = true;
                }
            }


            ILSLExprNode loopCondition = null;
            if (context.loop_condition != null)
            {
                loopCondition = VisitTopOfExpression(context.loop_condition);

                if (loopCondition.HasErrors)
                {
                    isError = true;
                }
                else if (!ExpressionValidator.ValidBooleanConditional(loopCondition))
                {
                    SyntaxErrorListener.ForLoopConditionNotValidType(
                        new LSLSourceCodeRange(context.loop_condition),
                        loopCondition);

                    isError = true;
                }

                if (!isError && loopCondition.IsConstant)
                {
                    SyntaxWarningListener.ConditionalExpressionIsConstant(loopCondition.SourceCodeRange, LSLConditionalStatementType.For);
                }
            }


            var expressionListRule = context.expression_list;


            var expressionList = VisitForLoopAfterthoughts(expressionListRule) as LSLExpressionListNode;

            if (expressionList == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitReturnTypeException("VisitForLoopAfterthoughts",
                        typeof(LSLExpressionListNode));
            }

            if (expressionList.HasErrors)
            {
                isError = true;
            }


            //if (!isError)
            //    EnterLoopStatement(false);

            var code = VisitCodeScopeOrSingleBlockStatement(context.code) as LSLCodeScopeNode;

            //if (!isError)
            //    ExitLoopStatement();


            if (code == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitReturnTypeException("VisitCodeScopeOrSingleBlockStatement",
                        typeof(LSLCodeScopeNode));
            }


            var result = new LSLForLoopNode(
                context,
                loopInit,
                loopCondition,
                expressionList,
                code,
                InSingleStatementBlock)
            {
                HasErrors = (isError || code.HasErrors)
            };

            return ReturnFromVisit(context, result);
        }

        #endregion

        #region StatementVisitors

        public override ILSLSyntaxTreeNode VisitExpressionStatement(LSLParser.ExpressionStatementContext context)
        {
            if (context == null || context.expression_rule == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpressionStatement");
            }


            var expressionHasEffect = DoesExpressionHaveEffect(context.expression_rule);

            if (!expressionHasEffect)
            {
                SyntaxWarningListener.ExpressionStatementHasNoEffect(new LSLSourceCodeRange(context));
            }

            var expression = VisitTopOfExpression(context.expression_rule);


            var result = new LSLExpressionStatementNode(context, expression, InSingleStatementBlock, expressionHasEffect)
            {
                HasErrors = expression.HasErrors
            };

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitReturnStatement(LSLParser.ReturnStatementContext context)
        {
            if (context == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitReturnStatement");
            }


            var containingFunction = ScopingManager.CurrentFunctionBodySignature;

            LSLReturnStatementNode result;

            if (context.return_expression != null)
            {
                var returnExpression = VisitTopOfExpression(context.return_expression);


                if (containingFunction == null)
                {
                    SyntaxErrorListener
                        .ReturnedValueFromEventHandler(new LSLSourceCodeRange(context),
                            returnExpression);

                    return ReturnFromVisit(context, LSLReturnStatementNode
                        .GetError(new LSLSourceCodeRange(context)));
                }

                if (containingFunction.ReturnType == LSLType.Void)
                {
                    SyntaxErrorListener.ReturnedValueFromVoidFunction(
                        new LSLSourceCodeRange(context),
                        containingFunction,
                        returnExpression);

                    return ReturnFromVisit(context, LSLReturnStatementNode
                        .GetError(new LSLSourceCodeRange(context)));
                }


                if (returnExpression.HasErrors)
                {
                    return ReturnFromVisit(context, LSLReturnStatementNode
                        .GetError(new LSLSourceCodeRange(context)));
                }


                var valid = ExpressionValidator.
                    ValidateReturnTypeMatch(containingFunction.ReturnType,
                        returnExpression);


                if (!valid)
                {
                    SyntaxErrorListener.TypeMismatchInReturnValue(
                        new LSLSourceCodeRange(context),
                        containingFunction,
                        returnExpression);
                    return ReturnFromVisit(context, LSLReturnStatementNode
                        .GetError(new LSLSourceCodeRange(context)));
                }

                result = new LSLReturnStatementNode(context, returnExpression, InSingleStatementBlock);

                return ReturnFromVisit(context, result);
            }


            if (containingFunction != null && containingFunction.ReturnType != LSLType.Void)
            {
                SyntaxErrorListener.ReturnedVoidFromANonVoidFunction(
                    new LSLSourceCodeRange(context),
                    containingFunction);


                return ReturnFromVisit(context, LSLReturnStatementNode
                    .GetError(new LSLSourceCodeRange(context)));
            }

            result = new LSLReturnStatementNode(context, InSingleStatementBlock);

            return ReturnFromVisit(context, result);
        }


        private bool ValidateCodeStatementTypeValidInScope(LSLParser.CodeStatementContext context)
        {
            if (InSingleStatementBlock && context.children[0] is LSLParser.LocalVariableDeclarationContext)
            {
                var ctx = (LSLParser.LocalVariableDeclarationContext)context.children[0];

                SyntaxErrorListener.DefinedVariableInNonScopeBlock(new LSLSourceCodeRange(ctx));
                return false;
            }

            return true;
        }


        public override ILSLSyntaxTreeNode VisitCodeStatement(LSLParser.CodeStatementContext context)
        {
            if (context == null ||
                context.children == null ||
                context.ChildCount == 0 ||
                context.children[0] == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitCodeStatement");
            }


            if (context.semi_colon != null)
            {
                if (!ScopingManager.InSingleStatementBlock)
                {
                    SyntaxWarningListener.UselessSemiColon(new LSLSourceCodeRange(context.semi_colon));
                }

                var result = new LSLSemiColonStatement(context, InSingleStatementBlock);
                return ReturnFromVisit(context, result);
            }

            if (!ValidateCodeStatementTypeValidInScope(context))
            {
                var result = new LSLCodeStatementError(context, InSingleStatementBlock);

                return ReturnFromVisit(context, result);
            }


            var statement = Visit(context.children[0]) as ILSLCodeStatement;


            if (statement == null)
            {
                throw new LSLCodeValidatorInternalError(
                    "VisitCodeStatement Visit(context.children[0]) did not return an ILSLCodeStatement");
            }

            return ReturnFromVisit(context, statement);
        }


        public override ILSLSyntaxTreeNode VisitStateChangeStatement(LSLParser.StateChangeStatementContext context)
        {
            if (context == null || context.state_target == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitStateChangeStatement");
            }


            if (!ScopingManager.StatePreDefined(context.state_target.Text))
            {
                var location = new LSLSourceCodeRange(context);

                SyntaxErrorListener.ChangeToUndefinedState(location, context.state_target.Text);

                return ReturnFromVisit(context,
                    LSLStateChangeStatementNode.GetError(new LSLSourceCodeRange(context)));
            }

            var result = new LSLStateChangeStatementNode(context, InSingleStatementBlock);


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitJumpStatement(LSLParser.JumpStatementContext context)
        {
            if (context == null || context.jump_target == null || context.jump_target.Text == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitJumpStatement");
            }


            if (!ScopingManager.LabelPreDefinedInScope(context.jump_target.Text))
            {
                SyntaxErrorListener.JumpToUndefinedLabel(
                    new LSLSourceCodeRange(context),
                    context.jump_target.Text);

                return ReturnFromVisit(context, LSLJumpStatementNode
                    .GetError(new LSLSourceCodeRange(context)));
            }

            var result = new LSLJumpStatementNode(
                context,
                ScopingManager.ResolvePreDefinedLabelNode(context.jump_target.Text),
                InSingleStatementBlock);


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitLabelStatement(LSLParser.LabelStatementContext context)
        {
            if (context == null || context.label_name == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitLabelStatement");
            }

            var label = ScopingManager.ResolvePreDefinedLabelNode(context.label_name.Text);
            if (label == null)
            {
                throw new LSLCodeValidatorInternalError(
                    "ScopingManager could not resolve label node that should have been defined in pre-pass");
            }


            return ReturnFromVisit(context, label);
        }

        #endregion

        #region ExpressionVisitors

        private ILSLExprNode VisitTopOfExpression(LSLParser.ExpressionContext tree)
        {
            var result = Visit(tree) as ILSLExprNode;

            if (result != null)
            {
                ResetMultipleAssignmentWarningMemory();
                return result;
            }


            throw new InvalidOperationException(
                "Internal error VisitExpressionContent in LSLCodeValidationVisitor returned a non ILSLExprNode type");
        }


        private ILSLExprNode VisitExpressionContent(LSLParser.ExpressionContext tree)
        {
            var result = Visit(tree) as ILSLExprNode;

            if (result != null)
            {
                return result;
            }

            throw new InvalidOperationException(
                "Internal error VisitExpressionContent in LSLCodeValidationVisitor returned a non ILSLExprNode type");
        }


        public override ILSLSyntaxTreeNode VisitExpr_Assignment(LSLParser.Expr_AssignmentContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_Assignment");
            }


            var result = VisitBinaryExpression(
                new BinaryExpressionContext(context.expr_lvalue, context.operation, context.expr_rvalue, context, true));

            if (result.HasErrors)
            {
                return ReturnFromVisit(context, result);
            }


            CheckForMultipleListAssignment(context, result);
            CheckForMultipleStringExpression(context, result);


            var location = new LSLSourceCodeRange(context);


            //we preformed a modifying operation on a library constant, thats an error
            if (result.LeftExpression.ExpressionType == LSLExpressionType.LibraryConstant)
            {
                SyntaxErrorListener.ModifiedLibraryConstant(location,
                    context.expr_lvalue.GetText());
                result.HasErrors = true;
            }

            else if (result.LeftExpression.IsCompoundExpression())
            {
                //allow chaining assignment, and modifying assignment operations when the left operand is a binary expression involving assignment, but nothing else
                var checkAssign = result.LeftExpression as LSLBinaryExpressionNode;
                if ((checkAssign != null && !checkAssign.Operation.IsAssignOrModifyAssign()) || checkAssign == null)
                {

                    SyntaxErrorListener.
                        AssignmentToCompoundExpression(location);

                    result.HasErrors = true;
                }
            }

            else if (result.LeftExpression.IsLiteral())
            {
                SyntaxErrorListener.
                    AssignmentToLiteral(location);

                result.HasErrors = true;
            }


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_ModifyingAssignment(
            LSLParser.Expr_ModifyingAssignmentContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_ModifyingAssignment");
            }

           
            var result = VisitBinaryExpression(
                new BinaryExpressionContext(context.expr_lvalue, context.operation, context.expr_rvalue, context,
                    true));


            if (result.HasErrors)
            {
                return ReturnFromVisit(context, result);
            }

            var location = new LSLSourceCodeRange(context.operation);

            //we preformed a modifying operation on a library constant, thats an error
            if (result.LeftExpression.ExpressionType == LSLExpressionType.LibraryConstant)
            {
                SyntaxErrorListener.ModifiedLibraryConstant(location,
                    context.expr_lvalue.GetText());

                result.HasErrors = true;
            }

            else if (result.LeftExpression.IsCompoundExpression())
            {
                //allow chaining assignment, and modifying assignment operations when the left operand is a binary expression involving assignment, but nothing else
                var checkAssign = result.LeftExpression as LSLBinaryExpressionNode;
                if ((checkAssign != null && !checkAssign.Operation.IsAssignOrModifyAssign()) || checkAssign == null)
                {
                    SyntaxErrorListener.
                        ModifyingAssignmentToCompoundExpression(location,
                            context.operation.Text);

                    result.HasErrors = true;
                }
            }

            else if (result.LeftExpression.IsLiteral())
            {
                SyntaxErrorListener.
                    ModifyingAssignmentToLiteral(location,
                        context.operation.Text);

                result.HasErrors = true;
            }


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_MultDivMod(LSLParser.Expr_MultDivModContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_MultDivMod");
            }


            var result = VisitBinaryExpression(
                new BinaryExpressionContext(context.expr_lvalue, context.operation, context.expr_rvalue, context));

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_AddSub(LSLParser.Expr_AddSubContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_AddSub");
            }


            var result = VisitBinaryExpression(
                new BinaryExpressionContext(context.expr_lvalue, context.operation, context.expr_rvalue, context));


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_BitwiseAnd(LSLParser.Expr_BitwiseAndContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_BitwiseAnd");
            }


            var result = VisitBinaryExpression(
                new BinaryExpressionContext(context.expr_lvalue, context.operation, context.expr_rvalue, context));

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_BitwiseOr(LSLParser.Expr_BitwiseOrContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_BitwiseOr");
            }

            var result = VisitBinaryExpression(
                new BinaryExpressionContext(context.expr_lvalue, context.operation, context.expr_rvalue, context));

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_BitwiseXor(LSLParser.Expr_BitwiseXorContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_BitwiseXor");
            }

            var result = VisitBinaryExpression(
                new BinaryExpressionContext(context.expr_lvalue, context.operation, context.expr_rvalue, context));


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_Atom(LSLParser.Expr_AtomContext context)
        {
            if (context == null ||
                context.children == null ||
                context.children[0] == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_Atom");
            }


            var location = new LSLSourceCodeRange(context);

            if (context.variable != null)
            {
                var idText = context.variable.Text;

                LSLVariableDeclarationNode declaration;
                if (MainLibraryDataProvider.LibraryConstantExist(idText))
                {
                    declaration =
                        LSLVariableDeclarationNode.CreateLibraryConstant(
                            MainLibraryDataProvider.GetLibraryConstantSignature(idText).Type, idText);
                }
                else
                {
                    declaration = ScopingManager.ResolveVariable(idText);
                    if (declaration == null)
                    {
                        SyntaxErrorListener.UndefinedVariableReference(location,
                            idText);

                        return ReturnFromVisit(context,
                            LSLVariableNode.GetError(new LSLSourceCodeRange(context.variable)));
                    }
                }

                var v = declaration.CreateReference(context.variable);

                //return a clone of the node into the tree, so its independent
                //from the definition if modified
                return ReturnFromVisit(context, v);
            }
            if (context.integer_literal != null)
            {
                return ReturnFromVisit(context, new LSLIntegerLiteralNode(context));
            }
            if (context.float_literal != null)
            {
                return ReturnFromVisit(context, new LSLFloatLiteralNode(context));
            }
            if (context.hex_literal != null)
            {
                return ReturnFromVisit(context, new LSLHexLiteralNode(context));
            }
            if (context.string_literal != null)
            {
                StringLiteralPreProcessor.ProcessString(context.string_literal.Text);

                if (StringLiteralPreProcessor.HasErrors)
                {
                    foreach (var code in StringLiteralPreProcessor.InvalidEscapeCodes)
                    {
                        SyntaxErrorListener.InvalidStringEscapeCode(location, code);
                    }

                    foreach (var chr in StringLiteralPreProcessor.IllegalCharacters)
                    {
                        SyntaxErrorListener.IllegalStringCharacter(location, chr);
                    }

                    StringLiteralPreProcessor.Reset();

                    return ReturnFromVisit(context, LSLStringLiteralNode.GetError(
                        new LSLSourceCodeRange(context.string_literal)));
                }

                var preProcessedString = StringLiteralPreProcessor.Result;

                StringLiteralPreProcessor.Reset();

                return ReturnFromVisit(context, new LSLStringLiteralNode(context, preProcessedString));
            }

            if (context.list_literal != null)
            {
                return ReturnFromVisit(context, VisitListLiteral(context.list_literal));
            }

            if (context.vector_literal != null)
            {
                return ReturnFromVisit(context, VisitVectorLiteral(context.vector_literal));
            }

            if (context.rotation_literal != null)
            {
                return ReturnFromVisit(context, VisitRotationLiteral(context.rotation_literal));
            }


            throw new LSLCodeValidatorInternalError("VisitExpr_Atom unexpected context state");
        }


        public override ILSLSyntaxTreeNode VisitExpr_DotAccessor(LSLParser.Expr_DotAccessorContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.operation, context.member))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_DotAccessor");
            }


            var exprLvalue = VisitExpressionContent(context.expr_lvalue);


            if (exprLvalue.HasErrors)
            {
                return ReturnFromVisit(context, LSLTupleAccessorNode.GetError(
                    new LSLSourceCodeRange(context)));
            }


            var location = new LSLSourceCodeRange(context.operation);

            var accessedMember = context.member.Text;

            var isTupleAccess = accessedMember.EqualsOneOf("x", "y", "z", "s");

            if (isTupleAccess && (exprLvalue.Type == LSLType.Vector || exprLvalue.Type == LSLType.Rotation))
            {
                //only give these errors if we are dealing with a vector or rotation type
                //accesses to non vectors and rotations are handled near the bottom of this function

                if (exprLvalue.IsLiteral())
                {
                    SyntaxErrorListener.TupleAccessorOnLiteral(
                        location,
                        exprLvalue, accessedMember);

                    return ReturnFromVisit(context, LSLTupleAccessorNode.GetError(
                        new LSLSourceCodeRange(context)));
                }


                if (exprLvalue.IsCompoundExpression())
                {
                    SyntaxErrorListener.TupleAccessorOnCompoundExpression(
                        location,
                        exprLvalue, accessedMember);

                    return ReturnFromVisit(context, LSLTupleAccessorNode.GetError(
                        new LSLSourceCodeRange(context)));
                }
            }
            else
            {
                SyntaxErrorListener.InvalidComponentAccessorOperation(location, exprLvalue, accessedMember);


                return ReturnFromVisit(context, LSLTupleAccessorNode.GetError(
                    new LSLSourceCodeRange(context)));
            }


            var accessedComponent = LSLTupleComponentTools.ParseComponentName(accessedMember);


            if (exprLvalue.Type == LSLType.Vector)
            {
                if (accessedMember == "s")
                {
                    SyntaxErrorListener.InvalidComponentAccessorOperation(
                        location, exprLvalue, accessedMember);

                    return ReturnFromVisit(context, LSLTupleAccessorNode.GetError(
                        new LSLSourceCodeRange(context)));
                }
            }


            var result = new LSLTupleAccessorNode(context, exprLvalue,
                exprLvalue.Type, accessedComponent);


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitParenthesizedExpression(LSLParser.ParenthesizedExpressionContext context)
        {
            if (context == null || context.expr_value == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitParenthisizedExpression");
            }


            var result = VisitExpressionContent(context.expr_value);

            if (result.HasErrors)
            {
                return ReturnFromVisit(context,
                    LSLParenthesizedExpressionNode.GetError(new LSLSourceCodeRange(context)));
            }


            return ReturnFromVisit(context, new LSLParenthesizedExpressionNode(context, result));
        }


        private LSLBinaryExpressionNode VisitBinaryExpression(BinaryExpressionContext context)
        {
            var exprLvalue = VisitExpressionContent(context.LeftContext);
            var exprRvalue = VisitExpressionContent(context.RightContext);

            var operationString = context.OperationToken.Text;

            var validate = ValidateBinaryOperation(exprLvalue, context.OperationToken.Text, exprRvalue,
                new LSLSourceCodeRange(context.OperationToken));


            var result = new LSLBinaryExpressionNode(
                context.OriginalContext, 
                context.OperationToken, 
                exprLvalue,
                exprRvalue, 
                validate.ResultType, 
                operationString)
            {
                HasErrors = !validate.IsValid
            };


            return result;
        }


        public override ILSLSyntaxTreeNode VisitExpr_LogicalEquality(LSLParser.Expr_LogicalEqualityContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_LogicalEquality");
            }


            var result = VisitBinaryExpression(
                new BinaryExpressionContext(context.expr_lvalue, context.operation, context.expr_rvalue, context));

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_LogicalCompare(LSLParser.Expr_LogicalCompareContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_LogicalCompare");
            }


            var result = VisitBinaryExpression(
                new BinaryExpressionContext(context.expr_lvalue, context.operation, context.expr_rvalue, context));

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_TypeCast(LSLParser.Expr_TypeCastContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_rvalue, context.cast_type))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_TypeCast");
            }

            var exprRvalue = VisitExpressionContent(context.expr_rvalue);


            var castType = LSLTypeTools.FromLSLTypeString(context.cast_type.Text);


            var validate = ValidateCastOperation(castType, exprRvalue, new LSLSourceCodeRange(context.cast_type));

            if (validate.IsValid && exprRvalue.Type == castType)
            {
                SyntaxWarningListener.RedundantCast(new LSLSourceCodeRange(context), castType);
            }


            var result = new LSLTypecastExprNode(context, validate.ResultType, exprRvalue)
            {
                HasErrors = !validate.IsValid
            };


            return ReturnFromVisit(context, result);
        }


        private bool ValidateFunctionCallSignatureMatch(
            LSLParser.Expr_FunctionCallContext context,
            LSLFunctionSignature functionSignature,
            IReadOnlyList<ILSLExprNode> expressions)
        {
            var parameterTypeMismatch = false;
            var parameterNumber = 0;


            var location = new LSLSourceCodeRange(context);

            if (!functionSignature.HasVariadicParameter)
            {
                if (expressions.Count() != functionSignature.ParameterCount)
                {
                    SyntaxErrorListener.ImproperParameterCountInFunctionCall(
                        location,
                        functionSignature,
                        expressions.ToArray());

                    return false;
                }
            }
            else
            {
                if (expressions.Count < functionSignature.ConcreteParameterCount)
                {
                    SyntaxErrorListener.ImproperParameterCountInFunctionCall(
                        location,
                        functionSignature,
                        expressions.ToArray());

                    return false;
                }
            }

            var checkUpToIndex = functionSignature.HasVariadicParameter ? 
                functionSignature.VariadicParameterIndex : expressions.Count;

            for (; parameterNumber < checkUpToIndex; parameterNumber++)
            {
                if (!ExpressionValidator.ValidFunctionParameter(
                    functionSignature,
                    parameterNumber,
                    expressions[parameterNumber]))
                {
                    parameterTypeMismatch = true;
                    break;
                }
            }


            if (parameterTypeMismatch)
            {
                SyntaxErrorListener.ParameterTypeMismatchInFunctionCall(
                    location,
                    parameterNumber,
                    functionSignature,
                    expressions.ToArray());


                return false;
            }

            return true;
        }


        public override ILSLSyntaxTreeNode VisitExpr_FunctionCall(LSLParser.Expr_FunctionCallContext context)
        {
            if (context == null || context.function_name == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_FunctionCall");
            }


            var location = new LSLSourceCodeRange(context);

            if (ScopingManager.InGlobalScope)
            {
                SyntaxErrorListener.
                    CallToFunctionInStaticContext(location);

                return ReturnFromVisit(context, LSLFunctionCallNode.GetError(
                    new LSLSourceCodeRange(context)));
            }


            var expressionListRule = context.expression_list;


            LSLFunctionCallNode result;
            var functionName = context.function_name.Text;


            if (MainLibraryDataProvider.LibraryFunctionExist(functionName))
            {
                var expressionList = VisitFunctionCallParameters(expressionListRule,
                    LSLExpressionListType.LibraryFunctionCallParameters) as LSLExpressionListNode;

                if (expressionList == null)
                {
                    throw LSLCodeValidatorInternalError
                        .VisitReturnTypeException("VisitFunctionCallParameters", typeof(LSLExpressionListNode));
                }


                var functionSignatures = MainLibraryDataProvider.GetLibraryFunctionSignatures(functionName);


                LSLFunctionSignature functionSignature = null;
                if (!expressionList.HasErrors)
                {
                    functionSignature = ValidateLibraryFunctionCallSignatureMatch(context, functionSignatures,
                        expressionList.ExpressionNodes);

                    if (functionSignature == null)
                    {
                        return LSLFunctionCallNode.GetError(new LSLSourceCodeRange(context));
                    }
                }

                result = new LSLFunctionCallNode(context, functionSignature, expressionList)
                {
                    HasErrors = expressionList.HasErrors
                };
            }
            else if (ScopingManager.FunctionIsPreDefined(functionName))
            {
                var expressionList = VisitFunctionCallParameters(expressionListRule,
                    LSLExpressionListType.UserFunctionCallParameters) as LSLExpressionListNode;

                if (expressionList == null)
                {
                    throw LSLCodeValidatorInternalError
                        .VisitReturnTypeException("VisitFunctionCallParameters", typeof(LSLExpressionListNode));
                }


                var functionSignature = ScopingManager.ResolveFunctionPreDefine(functionName);

                var match = false;
                if (!expressionList.HasErrors)
                {
                    match = ValidateFunctionCallSignatureMatch(context, functionSignature,
                        expressionList.ExpressionNodes);
                }

                result = new LSLFunctionCallNode(context, functionSignature, expressionList)
                {
                    HasErrors = !match | expressionList.HasErrors
                };


                AddReferenceToFunctionDefinition(functionSignature, result);
            }
            else
            {
                SyntaxErrorListener.CallToUndefinedFunction(location, functionName);

                return ReturnFromVisit(context, LSLFunctionCallNode.GetError(
                    new LSLSourceCodeRange(context)));
            }


            return ReturnFromVisit(context, result);
        }


        private LSLFunctionSignature ValidateLibraryFunctionCallSignatureMatch(
            LSLParser.Expr_FunctionCallContext context, IReadOnlyList<LSLLibraryFunctionSignature> functionSignatures,
            IReadOnlyList<ILSLExprNode> expressionNodes)
        {
            LSLFunctionSignature functionSignature = null;


            if (functionSignatures.Count() == 1)
            {
                var signature = functionSignatures.First();
                bool match = ValidateFunctionCallSignatureMatch(context, signature, expressionNodes);

                if (match)
                {
                    return signature;
                }
                return null;
            }

            foreach (var lslLibraryFunctionSignature in functionSignatures)
            {
                bool overloadMatch = true;
                if (lslLibraryFunctionSignature.ParameterCount == expressionNodes.Count)
                {
                    int pindex = 0;
                    foreach (var expression in expressionNodes)
                    {
                        if (ExpressionValidator.ValidFunctionParameter(lslLibraryFunctionSignature, pindex, expression))
                        {
                            overloadMatch = false;
                            break;
                        }
                        pindex++;
                    }
                }
                else
                {
                    overloadMatch = false;
                }

                if (overloadMatch)
                {
                    functionSignature = lslLibraryFunctionSignature;
                    break;
                }
            }

            if (functionSignature == null)
            {
                SyntaxErrorListener.NoSuitableLibraryFunctionOverloadFound(new LSLSourceCodeRange(context),
                    context.function_name.Text, expressionNodes);
            }

            return functionSignature;
        }


        public override ILSLSyntaxTreeNode VisitExpr_LogicalAnd(LSLParser.Expr_LogicalAndContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_LogicalAnd");
            }


            var result = VisitBinaryExpression(
                new BinaryExpressionContext(context.expr_lvalue, context.operation, context.expr_rvalue, context));

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_PrefixOperation(LSLParser.Expr_PrefixOperationContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_rvalue, context.operation))
            {
                return ReturnFromVisit(context, LSLPrefixOperationNode
                    .GetError(new LSLSourceCodeRange(context)));
            }


            var exprRvalue = VisitExpressionContent(context.expr_rvalue);


            var validate = ValidatePrefixOperation(context.operation.Text, exprRvalue,
                new LSLSourceCodeRange(context.operation));


            var result =
                new LSLPrefixOperationNode(context, validate.ResultType, exprRvalue)
                {
                    HasErrors = !validate.IsValid
                };


            if (exprRvalue.ExpressionType == LSLExpressionType.LibraryConstant &&
                (context.operation.Text == "++" || context.operation.Text == "--"))
            {
                SyntaxErrorListener.ModifiedLibraryConstant(
                    exprRvalue.SourceCodeRange, context.expr_rvalue.GetText());
                result.HasErrors = true;
            }


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_PostfixOperation(LSLParser.Expr_PostfixOperationContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.operation))
            {
                return ReturnFromVisit(context, LSLPostfixOperationNode
                    .GetError(new LSLSourceCodeRange(context)));
            }

            var exprLvalue = VisitExpressionContent(context.expr_lvalue);


            var validate = ValidatePostfixOperation(exprLvalue, context.operation.Text,
                new LSLSourceCodeRange(context.operation));


            var result = new LSLPostfixOperationNode(context, validate.ResultType, exprLvalue)
            {
                HasErrors = !validate.IsValid
            };


            if (exprLvalue.ExpressionType == LSLExpressionType.LibraryConstant &&
                (context.operation.Text == "++" || context.operation.Text == "--")
                )
            {
                SyntaxErrorListener.ModifiedLibraryConstant(
                    exprLvalue.SourceCodeRange, context.expr_lvalue.GetText());
                result.HasErrors = true;
            }


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_LogicalOr(LSLParser.Expr_LogicalOrContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_LogicalOr");
            }

            var result = VisitBinaryExpression(
                new BinaryExpressionContext(context.expr_lvalue, context.operation, context.expr_rvalue, context));

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_BitwiseShift(LSLParser.Expr_BitwiseShiftContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitExpr_BitwiseShift");
            }


            var result = VisitBinaryExpression(
                new BinaryExpressionContext(context.expr_lvalue, context.operation, context.expr_rvalue, context));
            return ReturnFromVisit(context, result);
        }

        #endregion

        #region CompoundLiteralVisitors

        public override ILSLSyntaxTreeNode VisitVectorLiteral(LSLParser.VectorLiteralContext context)
        {
            if (context == null ||
                Utility.AnyNull(context.vector_x, context.vector_y, context.vector_z))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitVectorLiteral");
            }


            var x = VisitTopOfExpression(context.vector_x);
            var y = VisitTopOfExpression(context.vector_y);
            var z = VisitTopOfExpression(context.vector_z);


            var result = new LSLVectorLiteralNode(context, x, y, z)
            {
                HasErrors = x.HasErrors || y.HasErrors || z.HasErrors
            };


            if (result.HasErrors)
            {
                return ReturnFromVisit(context, result);
            }


            if (!ExpressionValidator.ValidVectorContent(x))
            {
                SyntaxErrorListener.InvalidVectorContent(
                    new LSLSourceCodeRange(context.vector_x), LSLVectorComponent.X, x);
                result.HasErrors = true;
            }

            if (!ExpressionValidator.ValidVectorContent(y))
            {
                SyntaxErrorListener.InvalidVectorContent(
                    new LSLSourceCodeRange(context.vector_y), LSLVectorComponent.Y, y);
                result.HasErrors = true;
            }

            if (!ExpressionValidator.ValidVectorContent(z))
            {
                SyntaxErrorListener.InvalidVectorContent(
                    new LSLSourceCodeRange(context.vector_z), LSLVectorComponent.Z, z);
                result.HasErrors = true;
            }


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitListLiteral(LSLParser.ListLiteralContext context)
        {
            if (context == null)
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitListLiteral");
            }


            var expressionList = VisitListLiteralInitializerList(context.expression_list) as LSLExpressionListNode;

            if (expressionList == null)
            {
                throw LSLCodeValidatorInternalError.VisitReturnTypeException(
                    "VisitListLiteralExpressionList",
                    typeof(LSLExpressionListNode));
            }

            var result = new LSLListLiteralNode(context, expressionList)
            {
                HasErrors = expressionList.HasErrors
            };


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitRotationLiteral(LSLParser.RotationLiteralContext context)
        {
            if (context == null ||
                Utility.AnyNull(context.rotation_x, context.rotation_y, context.rotation_z, context.rotation_s))
            {
                throw LSLCodeValidatorInternalError
                    .VisitContextInvalidState("VisitRotationLiteral");
            }


            var x = VisitTopOfExpression(context.rotation_x);
            var y = VisitTopOfExpression(context.rotation_y);
            var z = VisitTopOfExpression(context.rotation_z);
            var s = VisitTopOfExpression(context.rotation_s);


            var result = new LSLRotationLiteralNode(context, x, y, z, s)
            {
                HasErrors = x.HasErrors || y.HasErrors || z.HasErrors || s.HasErrors
            };

            if (result.HasErrors)
            {
                return ReturnFromVisit(context, result);
            }


            if (!ExpressionValidator.ValidRotationContent(x))
            {
                SyntaxErrorListener.InvalidRotationContent(
                    new LSLSourceCodeRange(context.rotation_x), LSLRotationComponent.X, x);
                result.HasErrors = true;
            }

            if (!ExpressionValidator.ValidRotationContent(y))
            {
                SyntaxErrorListener.InvalidRotationContent(
                    new LSLSourceCodeRange(context.rotation_y), LSLRotationComponent.Y, y);
                result.HasErrors = true;
            }

            if (!ExpressionValidator.ValidRotationContent(z))
            {
                SyntaxErrorListener.InvalidRotationContent(
                    new LSLSourceCodeRange(context.rotation_z), LSLRotationComponent.Z, z);
                result.HasErrors = true;
            }

            if (!ExpressionValidator.ValidRotationContent(s))
            {
                SyntaxErrorListener.InvalidRotationContent(
                    new LSLSourceCodeRange(context.rotation_s), LSLRotationComponent.S, s);
                result.HasErrors = true;
            }


            return ReturnFromVisit(context, result);
        }

        #endregion

        #region GeneralUtilitys

        private static bool DoesExpressionHaveEffect(LSLParser.ExpressionContext context)
        {
            var postfixOperationContext = context as LSLParser.Expr_PostfixOperationContext;

            if (postfixOperationContext != null)
            {
                var opType =
                    LSLPostfixOperationTypeTools.
                        ParseFromOperator(postfixOperationContext.operation.Text);

                return opType == LSLPostfixOperationType.Increment || opType == LSLPostfixOperationType.Decrement;
            }

            var prefixOperationContext = context as LSLParser.Expr_PrefixOperationContext;

            if (prefixOperationContext != null)
            {
                var opType =
                    LSLPrefixOperationTypeTools.
                        ParseFromOperator(prefixOperationContext.operation.Text);

                return opType == LSLPrefixOperationType.Increment || opType == LSLPrefixOperationType.Decrement;
            }

            if (context is LSLParser.Expr_ModifyingAssignmentContext)
            {
                return true;
            }
            if (context is LSLParser.Expr_AssignmentContext)
            {
                return true;
            }
            if (context is LSLParser.Expr_FunctionCallContext)
            {
                return true;
            }
            return false;
        }



        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "context")]
        private static ILSLSyntaxTreeNode ReturnFromVisit(IParseTree context, ILSLSyntaxTreeNode nodeData)
        {
            //mostly a debugging hook
            return nodeData;
        }

        #endregion

        #region ConstantAnalysisUtilitys

        /*private readonly Stack<bool> _branchScopeStack = new Stack<bool>();


        private bool InConstantBranch()
        {
            return !_branchScopeStack.Any() || _branchScopeStack.Peek();
        }


        private void EnterBranchStatement(bool branchConditionConstant)
        {
            _branchScopeStack.Push(branchConditionConstant);
        }


        private void ExitBranchStatement()
        {
            _branchScopeStack.Pop();
        }


        private void EnterLoopStatement(bool loopConditionConstant)
        {
            _branchScopeStack.Push(loopConditionConstant);
        }


        private void ExitLoopStatement()
        {
            _branchScopeStack.Pop();
        }*/

        #endregion

        #region ExpressionValidationStubs

        private LSLExpressionValidatorResult ValidateBinaryOperation(ILSLExprNode left, string operation,
            ILSLExprNode right, LSLSourceCodeRange location)
        {
            if (left.HasErrors || right.HasErrors)
            {
                return new LSLExpressionValidatorResult(LSLType.Void, false);
            }

            var validate = ExpressionValidator.ValidateBinaryOperation(left,
                LSLBinaryOperationTypeTools.ParseFromOperator(operation), right);


            if (!validate.IsValid)
            {
                SyntaxErrorListener.InvalidBinaryOperation(
                    location, left, operation, right);
            }

            return validate;
        }


        private LSLExpressionValidatorResult ValidatePrefixOperation(string operation, ILSLExprNode right,
            LSLSourceCodeRange location)
        {
            if (right.HasErrors)
            {
                return new LSLExpressionValidatorResult(LSLType.Void, false);
            }

            var validate = ExpressionValidator.ValidatePrefixOperation(
                LSLPrefixOperationTypeTools.ParseFromOperator(operation),
                right);

            if (!validate.IsValid)
            {
                SyntaxErrorListener.InvalidPrefixOperation(
                    location, operation, right);
            }


            return validate;
        }


        private LSLExpressionValidatorResult ValidatePostfixOperation(ILSLExprNode left, string operation,
            LSLSourceCodeRange location)
        {
            if (left.HasErrors)
            {
                return new LSLExpressionValidatorResult(LSLType.Void, false);
            }

            var validate = ExpressionValidator.ValidatePostfixOperation(left,
                LSLPostfixOperationTypeTools.ParseFromOperator(operation));

            if (!validate.IsValid)
            {
                SyntaxErrorListener.InvalidPostfixOperation(
                    location, left, operation);
            }


            return validate;
        }


        private LSLExpressionValidatorResult ValidateCastOperation(LSLType castTo, ILSLExprNode from,
            LSLSourceCodeRange location)
        {
            if (from.HasErrors)
            {
                return new LSLExpressionValidatorResult(LSLType.Void, false);
            }

            var validate = ExpressionValidator.ValidateCastOperation(castTo, from);

            if (!validate.IsValid)
            {
                SyntaxErrorListener.InvalidCastOperation(
                    location, castTo, from);
            }

            return validate;
        }

        #endregion

        #region MultipleListStringAssignmentWarnings

        // The following algorithms for detecting multiple assignments to lists and strings work
        // by traversing up the tree from assignment operations, which may be leaves in
        // the syntax tree, they assume the assignment operation they start at is valid.
        //
        //
        // only these sort of assignments end up being checked from the bottom up
        // for multiple assignments, then appends; which may cause unexpected behavior
        // due to this compilers left to right evaluation order 
        //
        // (as opposed to the weird and suboptimal right to left evaluation order of Linden Labs LSL compiler)
        //
        // The goal is to help detect list and string optimization hacks that date back to before LSL compiled into 
        // Mono/Net Bytecode, and warn that they are invalid

        private readonly HashSet<RuleContext> _multipleListAssignmentWarned = new HashSet<RuleContext>();


        private readonly HashSet<RuleContext> _multipleStringAssignmentWarned = new HashSet<RuleContext>();

        private readonly Dictionary<string, List<LSLFunctionCallNode>> _referencesToNotYetDefinedFunctions =
            new Dictionary<string, List<LSLFunctionCallNode>>();

        /// <summary>
        ///     Only used by CheckForMultipleListAssignment
        /// </summary>
        private RuleContext _lastContextWithListAssign;

        /// <summary>
        ///     Only used by CheckForMultipleStringExpression
        /// </summary>
        private RuleContext _lastContextWithStringAssign;


        private void CheckForMultipleListAssignment(LSLParser.Expr_AssignmentContext context,
            LSLBinaryExpressionNode nodeData)
        {
            var leftData = nodeData.LeftExpression;
            var rightData = nodeData.RightExpression;

            var warnedNode = context.Parent;
            while (warnedNode is LSLParser.ExpressionContext)
            {
                warnedNode = warnedNode.Parent;
            }


            if (leftData.Type == LSLType.List &&
                rightData.Type == LSLType.List &&
                !_multipleListAssignmentWarned.Contains(warnedNode))
            {
                var operationLocation = new LSLSourceCodeRange(context.operation);

                var parent = context.Parent;
                while (parent is LSLParser.ExpressionContext)
                {
                    if (parent is LSLParser.Expr_AddSubContext)
                    {
                        SyntaxWarningListener.MultipleListAssignmentsInExpression(operationLocation);
                        _multipleListAssignmentWarned.Add(warnedNode);
                        _lastContextWithListAssign = null;
                        return;
                    }

                    parent = parent.Parent;
                }

                if (parent is LSLParser.LocalVariableDeclarationContext)
                {
                    SyntaxWarningListener.MultipleListAssignmentsInExpression(operationLocation);
                    _multipleListAssignmentWarned.Add(warnedNode);
                    _lastContextWithListAssign = null;
                    return;
                }

                if (parent is LSLParser.GlobalVariableDeclarationContext)
                {
                    SyntaxWarningListener.MultipleListAssignmentsInExpression(operationLocation);
                    _multipleListAssignmentWarned.Add(warnedNode);
                    _lastContextWithListAssign = null;
                    return;
                }

                if (_lastContextWithListAssign != null)
                {
                    if (ReferenceEquals(_lastContextWithListAssign, parent))
                    {
                        SyntaxWarningListener.MultipleListAssignmentsInExpression(operationLocation);
                        _multipleListAssignmentWarned.Add(warnedNode);
                        _lastContextWithListAssign = null;
                        return;
                    }
                }

                _lastContextWithListAssign = parent;
            }
        }


        private void CheckForMultipleStringExpression(LSLParser.Expr_AssignmentContext context,
            LSLBinaryExpressionNode nodeData)
        {
            var leftData = nodeData.LeftExpression;
            var rightData = nodeData.RightExpression;

            var warnedNode = context.Parent;
            while (warnedNode is LSLParser.ExpressionContext)
            {
                warnedNode = warnedNode.Parent;
            }

            if (leftData.Type == LSLType.String && rightData.Type == LSLType.String &&
                !_multipleStringAssignmentWarned.Contains(warnedNode))
            {
                var operationLocation = new LSLSourceCodeRange(context.operation);


                var parent = context.Parent;
                while (parent is LSLParser.ExpressionContext)
                {
                    if (parent is LSLParser.Expr_AddSubContext)
                    {
                        SyntaxWarningListener.MultipleStringAssignmentsInExpression(operationLocation);
                        _multipleStringAssignmentWarned.Add(warnedNode);
                        _lastContextWithStringAssign = null;
                        return;
                    }

                    parent = parent.Parent;
                }

                if (parent is LSLParser.LocalVariableDeclarationContext)
                {
                    SyntaxWarningListener.MultipleStringAssignmentsInExpression(operationLocation);
                    _multipleStringAssignmentWarned.Add(warnedNode);
                    _lastContextWithStringAssign = null;
                    return;
                }

                if (parent is LSLParser.GlobalVariableDeclarationContext)
                {
                    SyntaxWarningListener.MultipleStringAssignmentsInExpression(operationLocation);
                    _multipleStringAssignmentWarned.Add(warnedNode);
                    _lastContextWithStringAssign = null;
                    return;
                }

                if (_lastContextWithStringAssign != null)
                {
                    if (ReferenceEquals(_lastContextWithStringAssign, parent))
                    {
                        SyntaxWarningListener.MultipleStringAssignmentsInExpression(operationLocation);
                        _multipleStringAssignmentWarned.Add(warnedNode);
                        _lastContextWithStringAssign = null;
                        return;
                    }
                }
                _lastContextWithStringAssign = parent;
            }
        }


        /// <summary>
        ///     Resets data used to prevent multiple assignment warnings for lists and strings from appearing twice
        ///     for the same expression node
        ///     This data is currently reset at the end of code scope nodes (event handler and function body's)
        ///     which can contain multiple expression statements, instead of after every top level expression node visit,
        ///     this is for simplicity
        ///     the data is also reset after visits to global variable declarations, since there can be an expression on
        ///     the right side of the assignment
        /// </summary>
        private void ResetMultipleAssignmentWarningMemory()
        {
            _multipleStringAssignmentWarned.Clear();
            _multipleListAssignmentWarned.Clear();
        }

        #endregion
    }
}