#region FileInfo

// 
// File: LSLCodeValidationVisitor.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 

#endregion

#region Imports

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;
using LibLSLCC.LibraryData;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Visitor
{
    internal partial class LSLCodeValidationVisitor : LSLBaseVisitor<ILSLSyntaxTreeNode>
    {
        private readonly ILSLValidatorServiceProvider _validatorServices;
        private ILSLSyntaxErrorListener _syntaxErrorListenerOveride;
        private ILSLSyntaxWarningListener _syntaxWarningListenerOveride;


        public LSLCodeValidationVisitor(ILSLValidatorServiceProvider validatorServices)
        {
            if (!validatorServices.IsComplete())
            {
                throw new ArgumentException("An ILSLValidatorServiceProvider property was null", "validatorServices");
            }

            _validatorServices = validatorServices;

            ScopingManager = new LSLVisitorScopeTracker(_validatorServices);
        }


        /// <summary>
        ///     Gets a value indicating if syntax warnings were present after a visit.
        /// </summary>
        /// <value>
        ///     <c>true</c> if syntax warnings are present; otherwise, <c>false</c>.
        /// </value>
        public bool HasSyntaxWarnings { get; private set; }

        /// <summary>
        ///     Gets a value indicating if syntax errors were present after a visit.
        /// </summary>
        /// <value>
        ///     <c>true</c> if syntax errors are present; otherwise, <c>false</c>.
        /// </value>
        public bool HasSyntaxErrors { get; private set; }

        private LSLVisitorScopeTracker ScopingManager { get; set; }

        /// <summary>
        ///     Gets the syntax warning listener.  this property should NOT be used to generate warning events,
        ///     use <see cref="GenSyntaxWarning" /> for that instead.
        /// </summary>
        /// <value>
        ///     The syntax warning listener.
        /// </value>
        private ILSLSyntaxWarningListener SyntaxWarningListener
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

        /// <summary>
        ///     Gets the syntax error listener.  this property should NOT be used to generate error events,
        ///     use <see cref="GenSyntaxError" /> for that instead.
        /// </summary>
        /// <value>
        ///     The syntax error listener.
        /// </value>
        private ILSLSyntaxErrorListener SyntaxErrorListener
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

        public ILSLExpressionValidator ExpressionValidator
        {
            get { return _validatorServices.ExpressionValidator; }
        }

        public ILSLLibraryDataProvider LibraryDataProvider
        {
            get { return _validatorServices.LibraryDataProvider; }
        }

        public ILSLStringPreProcessor StringLiteralPreProcessor
        {
            get { return _validatorServices.StringLiteralPreProcessor; }
        }

        private bool InSingleStatementBlock
        {
            get { return ScopingManager.InSingleStatementBlock; }
        }


        /// <summary>
        ///     Returns a reference to <see cref="SyntaxWarningListener" /> and sets <see cref="HasSyntaxWarnings" /> to
        ///     <c>true</c>.
        /// </summary>
        /// <returns>
        ///     <see cref="SyntaxWarningListener" />
        /// </returns>
        private ILSLSyntaxWarningListener GenSyntaxWarning()
        {
            HasSyntaxWarnings = true;
            return SyntaxWarningListener;
        }


        /// <summary>
        ///     Returns a reference to <see cref="SyntaxErrorListener" /> and sets <see cref="HasSyntaxErrors" /> to <c>true</c>.
        /// </summary>
        /// <returns>
        ///     <see cref="SyntaxErrorListener" />
        /// </returns>
        private ILSLSyntaxErrorListener GenSyntaxError()
        {
            HasSyntaxErrors = true;
            return SyntaxErrorListener;
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
            var x = VisitCompilationUnit(tree) as LSLCompilationUnitNode;

            if (x == null)
            {
                throw LSLCodeValidatorInternalException.VisitReturnTypeException("VisitCompilationUnit",
                    typeof (LSLCompilationUnitNode));
            }

            return x;
        }


        public void Reset()
        {
            HasSyntaxWarnings = false;
            HasSyntaxErrors = false;


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
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitListLiteralInitializerList");
            }

            var result = new LSLExpressionListNode(context, LSLExpressionListType.ListInitializer);

            var expressionList = context.expressionList();
            if (expressionList == null)
            {
                return ReturnFromVisit(context, result);
            }

            if (expressionList.children == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitListLiteralInitializerList");
            }


            IEnumerable<IParseTree> subtrees = new[] {(LSLParser.ExpressionContext) expressionList.children[0]};

            if (expressionList.children.Count > 1)
            {
                subtrees = subtrees.Concat(expressionList.children.Skip(1).Select(x =>
                {
                    var listTail = ((LSLParser.ExpressionListTailContext) x);
                    result.AddCommaRange(new LSLSourceCodeRange(listTail.comma));
                    return listTail.expression();
                }));
            }

            var expressionContexts = subtrees.ToList();


            for (var i = 0; i < expressionContexts.Count; i++)
            {
                var expression = VisitTopOfExpression((LSLParser.ExpressionContext) expressionContexts[i]);

                result.AddExpression(expression);

                if (!expression.HasErrors && !ExpressionValidator.ValidateListContent(expression))
                {
                    GenSyntaxError().InvalidListContent(expression.SourceCodeRange, i, expression);

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
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitForLoopAfterthoughts");
            }


            var result = new LSLExpressionListNode(context, LSLExpressionListType.ForLoopAfterthoughts);

            var expressionList = context.expressionList();
            if (expressionList == null)
            {
                return ReturnFromVisit(context, result);
            }

            if (expressionList.children == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitForLoopAfterthoughts");
            }

            IEnumerable<IParseTree> subtrees = new[] {(LSLParser.ExpressionContext) expressionList.children[0]};

            if (expressionList.children.Count > 1)
            {
                subtrees = subtrees.Concat(expressionList.children.Skip(1).Select(x =>
                {
                    var listTail = ((LSLParser.ExpressionListTailContext) x);
                    result.AddCommaRange(new LSLSourceCodeRange(listTail.comma));
                    return listTail.expression();
                }));
            }

            var expressionContexts = subtrees.ToList();


            var expressionIndex = 0;

            foreach (var expressionContext in expressionContexts)
            {
                var ctx = (LSLParser.ExpressionContext) expressionContext;

                var expression = VisitTopOfExpression(ctx);
                if (expression.HasErrors)
                {
                    result.HasErrors = true;
                }
                else
                {
                    if (!expression.HasPossibleSideEffects)
                    {
                        GenSyntaxWarning().ForLoopAfterthoughtHasNoEffect(
                            new LSLSourceCodeRange(ctx),
                            expressionIndex, expressionContexts.Count);
                    }
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
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitForLoopInitExpressions");
            }


            var result = new LSLExpressionListNode(context, LSLExpressionListType.ForLoopInitExpressions);

            var expressionList = context.expressionList();
            if (expressionList == null)
            {
                return ReturnFromVisit(context, result);
            }

            if (expressionList.children == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitForLoopInitExpressions");
            }


            IEnumerable<IParseTree> subtrees = new[] {(LSLParser.ExpressionContext) expressionList.children[0]};

            if (expressionList.children.Count > 1)
            {
                subtrees = subtrees.Concat(expressionList.children.Skip(1).Select(x =>
                {
                    var listTail = ((LSLParser.ExpressionListTailContext) x);
                    result.AddCommaRange(new LSLSourceCodeRange(listTail.comma));
                    return listTail.expression();
                }));
            }

            var expressionContexts = subtrees.ToList();


            var expressionIndex = 0;

            foreach (var expressionContext in expressionContexts)
            {
                var ctx = (LSLParser.ExpressionContext) expressionContext;

                var expression = VisitTopOfExpression(ctx);

                if (expression.HasErrors)
                {
                    result.HasErrors = true;
                }
                else
                {
                    if (!expression.HasPossibleSideEffects)
                    {
                        GenSyntaxWarning().ForLoopInitExpressionHasNoEffect(
                            new LSLSourceCodeRange(ctx),
                            expressionIndex, expressionContexts.Count);
                    }
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
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitFunctionCallParameters");
            }


            if (type != LSLExpressionListType.UserFunctionCallParameters &&
                type != LSLExpressionListType.LibraryFunctionCallParameters)
            {
                throw new LSLCodeValidatorInternalException(
                    "VisitFunctionCallParameters LSLExpressionListType was an invalid type");
            }

            var result = new LSLExpressionListNode(context, type);

            var expressionList = context.expressionList();

            if (expressionList == null)
            {
                return ReturnFromVisit(context, result);
            }

            if (expressionList.children == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitFunctionCallParameters");
            }


            IEnumerable<IParseTree> subtrees = new[] {(LSLParser.ExpressionContext) expressionList.children[0]};

            if (expressionList.children.Count > 1)
            {
                subtrees = subtrees.Concat(expressionList.children.Skip(1).Select(x =>
                {
                    var listTail = ((LSLParser.ExpressionListTailContext) x);
                    result.AddCommaRange(new LSLSourceCodeRange(listTail.comma));
                    return listTail.expression();
                }));
            }

            var expressionContexts = subtrees.ToList();


            foreach (var expressionContext in expressionContexts)
            {
                var expression = VisitTopOfExpression((LSLParser.ExpressionContext) expressionContext);
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
            throw new LSLCodeValidatorInternalException("Expression list are not expected to be visited directly");
        }


        public override ILSLSyntaxTreeNode VisitTerminal(ITerminalNode node)
        {
            throw new LSLCodeValidatorInternalException("Terminals are not expected to be visited");
        }


        public override ILSLSyntaxTreeNode VisitCompilationUnit(LSLParser.CompilationUnitContext context)
        {
            if (context == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitCompilationUnit");
            }


            var compilationUnitPrePass = ScopingManager.EnterCompilationUnit(context);

            if (compilationUnitPrePass.HasSyntaxErrors)
            {
                HasSyntaxErrors = true;
                ScopingManager.ExitCompilationUnit();
                return ReturnFromVisit(context, LSLCompilationUnitNode.GetError(new LSLSourceCodeRange(context)));
            }

            if (compilationUnitPrePass.HasSyntaxWarnings)
            {
                HasSyntaxWarnings = true;
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
                    throw LSLCodeValidatorInternalException
                        .VisitReturnTypeException("VisitGlobalVariableDeclaration", typeof (LSLVariableDeclarationNode));
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
                    throw LSLCodeValidatorInternalException
                        .VisitReturnTypeException("VisitFunctionDeclaration", typeof (LSLFunctionDeclarationNode));
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
                throw LSLCodeValidatorInternalException
                    .VisitReturnTypeException("VisitDefaultState", typeof (LSLStateScopeNode));
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
                    throw LSLCodeValidatorInternalException
                        .VisitReturnTypeException("VisitDefinedState", typeof (LSLStateScopeNode));
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
                GenSyntaxWarning().FunctionNeverUsed(fun.SourceCodeRange, fun);
            }

            foreach (var gvar in result.GlobalVariableDeclarations.Where(x => x.References.Count == 0))
            {
                GenSyntaxWarning().GlobalVariableNeverUsed(gvar.SourceCodeRange, gvar);
            }

            syntaxMessagePrioritizer.InvokeQueuedActions();

            RemoveSyntaxErrorListenerOverride();
            RemoveSyntaxWarningListenerOverride();

            return ReturnFromVisit(context, result);
        }

        #region GeneralUtilitys

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "context")]
        // ReSharper disable once UnusedParameter.Local
        private static ILSLSyntaxTreeNode ReturnFromVisit(IParseTree context, ILSLSyntaxTreeNode nodeData)
        {
            //mostly a debugging hook
            return nodeData;
        }

        #endregion

        #region VariableDeclarationVisitors

        public override ILSLSyntaxTreeNode VisitGlobalVariableDeclaration(
            LSLParser.GlobalVariableDeclarationContext context)
        {
            if (context == null || Utility.AnyNull(context.variable_type, context.variable_name))
            {
                throw new
                    LSLCodeValidatorInternalException(
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
                    LSLCodeValidatorInternalException(
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


            if (LibraryDataProvider.LibraryConstantExist(nameToken.Text))
            {
                GenSyntaxError().RedefinedStandardLibraryConstant(
                    new LSLSourceCodeRange(nameToken), variableType,
                    LibraryDataProvider.GetLibraryConstantSignature(nameToken.Text));


                return LSLVariableDeclarationNode.GetError(new LSLSourceCodeRange(context));
            }


            if (!ScopingManager.CanVariableBeDefined(nameToken.Text, declarationScope))
            {
                GenSyntaxError().VariableRedefined(
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
                        (LSLParser.LocalVariableDeclarationContext) context, expression);
                }
                else
                {
                    variable = LSLVariableDeclarationNode.CreateVar(
                        (LSLParser.GlobalVariableDeclarationContext) context, expression);
                }


                var valid = ExpressionValidator.ValidateBinaryOperation(
                    variable.VariableNode, LSLBinaryOperationType.Assign,
                    variable.DeclarationExpression);


                if (!valid.IsValid)
                {
                    GenSyntaxError().TypeMismatchInVariableDeclaration(
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
                        (LSLParser.LocalVariableDeclarationContext) context);
                }
                else
                {
                    variable = LSLVariableDeclarationNode.CreateVar(
                        (LSLParser.GlobalVariableDeclarationContext) context);
                }
            }

            if (declarationScope == LSLVariableScope.Local)
            {
                WarnIfLocalVariableHidesDeclarationInOuterScope(variable);

                ScopingManager.DefineVariable(variable, declarationScope);

                WarnIfLocalVariableHidesParameterOrGlobal(variable);
            }
            else
            {
                ScopingManager.DefineVariable(variable, declarationScope);
            }


            return variable;
        }


        private void WarnIfLocalVariableHidesDeclarationInOuterScope(LSLVariableDeclarationNode variable)
        {
            if (ScopingManager.LocalVariableDefined(variable.Name))
            {
                if (ScopingManager.InsideFunctionBody)
                {
                    GenSyntaxWarning().VariableRedeclaredInInnerScope(variable.SourceCodeRange,
                        ScopingManager.CurrentFunctionBodySignature, variable,
                        ScopingManager.ResolveVariable(variable.Name));
                }
                else
                {
                    GenSyntaxWarning().VariableRedeclaredInInnerScope(variable.SourceCodeRange,
                        ScopingManager.CurrentEventHandlerSignature, variable,
                        ScopingManager.ResolveVariable(variable.Name));
                }
            }
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


                    GenSyntaxWarning().LocalVariableHidesParameter(variable.SourceCodeRange,
                        ScopingManager.CurrentFunctionBodySignature, variable, parameter);
                }
                else
                {
                    var parameter =
                        ScopingManager.CurrentEventHandlerSignature
                            .ParameterListNode.Parameters.Single(x => (x.Name == variable.Name));

                    GenSyntaxWarning().LocalVariableHidesParameter(variable.SourceCodeRange,
                        ScopingManager.CurrentEventHandlerSignature, variable, parameter);
                }
            }


            if (ScopingManager.GlobalVariableDefined(variable.Name))
            {
                if (ScopingManager.InsideFunctionBody)
                {
                    GenSyntaxWarning().LocalVariableHidesGlobalVariable(variable.SourceCodeRange,
                        ScopingManager.CurrentFunctionBodySignature, variable,
                        ScopingManager.ResolveGlobalVariable(variable.Name));
                }
                else
                {
                    GenSyntaxWarning().LocalVariableHidesGlobalVariable(variable.SourceCodeRange,
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
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitDefaultState");
            }

            var result = new LSLStateScopeNode(context);


            foreach (var x in context.eventHandler())
            {
                var child = VisitEventHandler(x) as LSLEventHandlerNode;

                if (child == null)
                {
                    throw LSLCodeValidatorInternalException.
                        VisitReturnTypeException("VisitEventHandler", typeof (LSLEventHandlerNode));
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
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitDefinedState");
            }


            if (!ScopingManager.StatePreDefined(context.state_name.Text))
            {
                //state was not defined in the pre-pass that predefines all states
                //in the compilation unit, this is an internal error and should never happen.
                throw new LSLCodeValidatorInternalException(
                    "VisitDefinedState, ScopingManager.StateDefined returned that state was undefined " +
                    "when it should have been pre-defined in a pre-pass");
            }


            var result = new LSLStateScopeNode(context);


            foreach (var x in context.eventHandler())
            {
                var child = VisitEventHandler(x) as LSLEventHandlerNode;

                if (child == null)
                {
                    throw LSLCodeValidatorInternalException.
                        VisitReturnTypeException("VisitEventHandler", typeof (LSLEventHandlerNode));
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

        private ILSLSyntaxTreeNode VisitElseIfStatement(LSLParser.ElseStatementContext context)
        {
            ILSLExprNode expression;
            var isError = false;

            if (context.code.control_structure.condition == null)
            {
                //creating a valid else if statement node even if the condition is null
                //allows return path verification to continue, also various other error checks
                //make a dummy expression value for the condition node, a constant integer literal


                LSLSourceCodeRange errorLocation;

                if (
                    context.code.control_structure.open_parenth == null ||
                    context.code.control_structure.close_parenth == null
                    )
                {
                    errorLocation = new LSLSourceCodeRange(context);
                }
                else
                {
                    errorLocation =
                        new LSLSourceCodeRange(
                            context.code.control_structure.open_parenth,
                            context.code.control_structure.close_parenth);
                }

                GenSyntaxError().MissingConditionalExpression(errorLocation, LSLConditionalStatementType.ElseIf);


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
                expression = VisitTopOfExpression(context.code.control_structure.condition);


                if (expression.HasErrors)
                {
                    isError = true;
                }
                else if (!ExpressionValidator.ValidBooleanConditional(expression))
                {
                    GenSyntaxError().ElseIfConditionNotValidType(
                        new LSLSourceCodeRange(context.code.control_structure.condition),
                        expression
                        );


                    isError = true;
                }


                if (!isError && expression.IsConstant)
                {
                    GenSyntaxWarning().ConditionalExpressionIsConstant(
                        new LSLSourceCodeRange(context.code.control_structure.condition),
                        LSLConditionalStatementType.ElseIf);
                }
            }


            var code =
                VisitCodeScopeOrSingleBlockStatement(context.code.control_structure.code) as LSLCodeScopeNode;


            if (code == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitReturnTypeException("VisitCodeScopeOrSingleBlockStatement",
                        typeof (LSLCodeScopeNode));
            }


            if (code.HasErrors)
            {
                isError = true;
            }


            var result = new LSLElseIfStatementNode(
                context.else_keyword,
                context.code.control_structure,
                code,
                expression)
            {
                HasErrors = isError
            };

            return result;
        }


        public override ILSLSyntaxTreeNode VisitElseStatement(LSLParser.ElseStatementContext context)
        {
            if (context == null || context.code == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitElseStatement");
            }

            var currentControlStatement = ScopingManager.CurrentControlStatement;

            var elseBranchIsConstant =
                currentControlStatement.IfStatement.IsConstantBranch &&
                currentControlStatement.ElseIfStatements.All(x => x.IsConstantBranch);


            var code = VisitCodeScopeOrSingleBlockStatement(context.code) as LSLCodeScopeNode;

            if (code == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitReturnTypeException("VisitCodeScopeOrSingleBlockStatement",
                        typeof (LSLCodeScopeNode));
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
                throw LSLCodeValidatorInternalException.VisitContextInvalidState("VisitControlStructure");
            }


            var result = new LSLControlStatementNode(context, InSingleStatementBlock);

            ScopingManager.EnterControlStatement(result);


            ILSLExprNode expression;

            if (context.condition == null)
            {
                //creating a valid if statement node even if the condition is null
                //allows return path verification to continue, also various other error checks
                //make a dummy expression value for the condition node, a constant integer literal


                LSLSourceCodeRange errorLocation;

                if (
                    context.code.control_structure.open_parenth == null ||
                    context.code.control_structure.close_parenth == null
                    )
                {
                    errorLocation = new LSLSourceCodeRange(context);
                }
                else
                {
                    errorLocation =
                        new LSLSourceCodeRange(
                            context.code.control_structure.open_parenth,
                            context.code.control_structure.close_parenth);
                }

                GenSyntaxError().MissingConditionalExpression(errorLocation, LSLConditionalStatementType.If);

                result.HasErrors = true;

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
                    result.HasErrors = true;
                }
                else if (!ExpressionValidator.ValidBooleanConditional(expression))
                {
                    GenSyntaxError().IfConditionNotValidType(
                        new LSLSourceCodeRange(context.condition),
                        expression
                        );


                    result.HasErrors = true;
                }

                if (!result.HasErrors && expression.IsConstant)
                {
                    GenSyntaxWarning().ConditionalExpressionIsConstant(new LSLSourceCodeRange(context.condition),
                        LSLConditionalStatementType.If);
                }
            }

            var code = VisitCodeScopeOrSingleBlockStatement(context.code) as LSLCodeScopeNode;


            if (code == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitReturnTypeException("VisitCodeScopeOrSingleBlockStatement",
                        typeof (LSLCodeScopeNode));
            }

            if (code.HasErrors)
            {
                result.HasErrors = true;
            }

            result.IfStatement = new LSLIfStatementNode(context, code, expression)
            {
                HasErrors = result.HasErrors
            };



            //traverse down the right side of the syntax tree if needed

            var elseStatement = context.else_statement;


            if (elseStatement == null)
            {
                ScopingManager.ExitControlStatement();
                return ReturnFromVisit(context, result);
            }

            
            do
            {
                if (elseStatement.code != null && elseStatement.code.control_structure != null)
                {
                    var createdElseIfStatement = VisitElseIfStatement(elseStatement) as LSLElseIfStatementNode;

                    if (createdElseIfStatement == null)
                    {
                        throw new
                            LSLCodeValidatorInternalException(
                            "VisitControlStructure child node Visit did not return proper type");
                    }

                    if (createdElseIfStatement.HasErrors)
                    {
                        result.HasErrors = true;
                    }

                    result.AddElseIfStatement(createdElseIfStatement);


                    elseStatement = elseStatement.code.control_structure.else_statement;
                }
                else
                {
                    var createdElseStatement = VisitElseStatement(elseStatement) as LSLElseStatementNode;

                    if (createdElseStatement == null)
                    {
                        throw new
                            LSLCodeValidatorInternalException(
                            "VisitControlStructure child node Visit did not return proper type");
                    }

                    if (createdElseStatement.HasErrors)
                    {
                        result.HasErrors = true;
                    }

                    result.ElseStatement = createdElseStatement;

                    elseStatement = null;
                }

            } while (elseStatement != null);


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
                    GenSyntaxWarning().DeadCodeDetected(
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
                    GenSyntaxWarning().DeadCodeDetected(
                        deadSegment.SourceCodeRange,
                        ScopingManager.CurrentEventHandlerSignature, deadSegment);
                }
            }

            if (codeScope.HasDeadStatementNodes && ScopingManager.InsideFunctionBody)
            {
                foreach (var deadSegment in codeScope.DeadCodeSegments)
                {
                    GenSyntaxWarning().DeadCodeDetected(
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
                GenSyntaxError().NotAllCodePathsReturnAValue(
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
                        GenSyntaxError().DeadCodeAfterReturnPathDetected(
                            deadSegment.SourceCodeRange, currentFunctionPredefinition, deadSegment);
                        isError = true;
                    }
                    else
                    {
                        GenSyntaxWarning().DeadCodeDetected(
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
                    GenSyntaxWarning().DeadCodeDetected(
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
                GenSyntaxWarning().FunctionParameterNeverUsed(v.SourceCodeRange, v,
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
                    GenSyntaxWarning().LocalVariableNeverUsed(v.SourceCodeRange, v,
                        ScopingManager.CurrentFunctionBodySignature);
                }
            }
            else
            {
                foreach (var v in ScopingManager.AllLocalVariablesInScope.Where(x => x.References.Count == 0))
                {
                    GenSyntaxWarning().LocalVariableNeverUsed(v.SourceCodeRange, v,
                        ScopingManager.CurrentEventHandlerSignature);
                }
            }
        }


        public override ILSLSyntaxTreeNode VisitCodeScope(LSLParser.CodeScopeContext context)
        {
            if (context == null || context.children == null)
            {
                throw new
                    LSLCodeValidatorInternalException("VisitCodeScope did not meet context state pre-requisites");
            }


            ScopingManager.EnterCodeScopeAfterPrePass(context);


            ScopingManager.IncrementScopeId();


            var result = new LSLCodeScopeNode(context, ScopingManager.CurrentScopeId,
                ScopingManager.CurrentCodeScopeType);


            var codeStatementContexts = context.codeStatement();


            foreach (var codeStatementContext in codeStatementContexts)
            {
                var child = VisitCodeStatement(codeStatementContext) as ILSLCodeStatement;

                if (child == null)
                {
                    throw LSLCodeValidatorInternalException
                        .VisitReturnTypeException("VisitCodeStatement", typeof (ILSLCodeStatement));
                }

                if (child.HasErrors)
                {
                    result.HasErrors = true;
                }

                result.AddCodeStatement(child);
            }

            result.EndScope();


            if (ScopingManager.CurrentCodeScopeType == LSLCodeScopeType.Function)
            {
                WarnUnusedVariablesInFunction();
                result.HasErrors = ValidateFunctionReturnPath(result) || result.HasErrors;
            }
            else if (ScopingManager.CurrentCodeScopeType == LSLCodeScopeType.EventHandler)
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


        public ILSLSyntaxTreeNode VisitCodeScopeOrSingleBlockStatement(
            LSLParser.CodeStatementContext context)
        {
            if (context == null)
            {
                throw
                    LSLCodeValidatorInternalException.VisitContextInvalidState("VisitCodeScopeOrSingleBlockStatement");
            }


            if (context.code_scope != null)
            {
                var codeScope = VisitCodeScope(context.code_scope) as LSLCodeScopeNode;

                if (codeScope == null)
                {
                    throw LSLCodeValidatorInternalException
                        .VisitReturnTypeException("VisitCodeScope", typeof (LSLCodeScopeNode));
                }

                return ReturnFromVisit(context, codeScope);
            }


            ScopingManager.EnterSingleStatementBlock(context);

            var codeStatement = VisitCodeStatement(context) as ILSLCodeStatement;

            ScopingManager.ExitSingleStatementBlock();


            if (codeStatement == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitReturnTypeException("VisitCodeStatement", typeof (ILSLCodeStatement));
            }


            ScopingManager.IncrementScopeId();

            LSLCodeScopeNode result;
            if (!codeStatement.HasErrors)
            {
                result = new LSLCodeScopeNode(context, ScopingManager.CurrentScopeId,
                    ScopingManager.CurrentCodeScopeType);
            }
            else
            {
                result = LSLCodeScopeNode.GetError(new LSLSourceCodeRange(context));
            }


            result.AddCodeStatement(codeStatement);
            result.EndScope();


            return ReturnFromVisit(context, result);
        }

        #endregion

        #region EventHandlerAndFunctionDeclarationVisitors

        public override ILSLSyntaxTreeNode VisitEventHandler(LSLParser.EventHandlerContext context)
        {
            if (context == null || context.code == null)
            {
                throw LSLCodeValidatorInternalException.VisitContextInvalidState("VisitEventHandler");
            }


            var parameterList = LSLParameterListNode.BuildDirectlyFromContext(context.parameters);


            var isError = parameterList.HasErrors;


            var eventHandlerSignature = new LSLParsedEventHandlerSignature(context.handler_name.Text, parameterList);


            if (!LibraryDataProvider.EventHandlerExist(context.handler_name.Text))
            {
                var location = new LSLSourceCodeRange(context.handler_name);

                GenSyntaxError().UnknownEventHandlerDeclared(location, eventHandlerSignature);


                isError = true;
            }


            var librarySignature = LibraryDataProvider.GetEventHandlerSignature(context.handler_name.Text);


            if (librarySignature != null && librarySignature.Deprecated)
            {
                GenSyntaxWarning()
                    .UseOfDeprecatedLibraryEventHandler(new LSLSourceCodeRange(context.handler_name), librarySignature);
            }


            //the library signature may not have been defined, see above
            //but we want to continue processing errors in the code body of the event handler.
            //so instead of returning after it is determined that there is no event handler with the given
            //name, just skip the signature match check since librarySignature will be null (event handler is undefined)
            //and continue down the tree into the events code scope.
            if (librarySignature != null && !eventHandlerSignature.SignatureMatches(librarySignature))
            {
                var location = new LSLSourceCodeRange(context.handler_name);

                GenSyntaxError().IncorrectEventHandlerSignature(
                    location,
                    eventHandlerSignature,
                    librarySignature
                    );

                isError = true;
            }


            //Warn about parameters that hide global variables
            foreach (var parameter in parameterList.Parameters)
            {
                if (ScopingManager.GlobalVariableDefined(parameter.Name))
                {
                    GenSyntaxWarning().ParameterHidesGlobalVariable(parameter.SourceCodeRange,
                        eventHandlerSignature,
                        parameter,
                        ScopingManager.ResolveVariable(parameter.Name));
                }
            }


            var eventPrePass = ScopingManager.EnterEventScope(context, eventHandlerSignature);

            if (eventPrePass.HasSyntaxErrors)
            {
                HasSyntaxErrors = true;
                ScopingManager.ExitEventScope();
                return ReturnFromVisit(context, LSLEventHandlerNode
                    .GetError(new LSLSourceCodeRange(context)));
            }

            if (eventPrePass.HasSyntaxWarnings)
            {
                HasSyntaxWarnings = true;
            }


            var codeScope = VisitCodeScope(context.codeScope()) as LSLCodeScopeNode;


            ScopingManager.ExitEventScope();


            if (codeScope == null)
            {
                throw LSLCodeValidatorInternalException.VisitReturnTypeException("VisitCodeScope",
                    typeof (LSLCodeScopeNode));
            }


            isError = codeScope.HasErrors || isError;


            var result = new LSLEventHandlerNode(context, parameterList, codeScope) {HasErrors = isError};
            ScopingManager.ResetScopeId();

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitFunctionDeclaration(LSLParser.FunctionDeclarationContext context)
        {
            if (context == null || Utility.AnyNull(context.function_name, context.code))
            {
                throw LSLCodeValidatorInternalException
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
                    GenSyntaxWarning().ParameterHidesGlobalVariable(parameter.SourceCodeRange,
                        currentFunctionPredefinition,
                        parameter,
                        ScopingManager.ResolveVariable(parameter.Name));
                }
            }


            var functionPrePass = ScopingManager.EnterFunctionScope(context, currentFunctionPredefinition);


            if (functionPrePass.HasSyntaxErrors)
            {
                HasSyntaxErrors = true;
                ScopingManager.ExitFunctionScope();
                return ReturnFromVisit(context, LSLFunctionDeclarationNode
                    .GetError(new LSLSourceCodeRange(context)));
            }

            if (functionPrePass.HasSyntaxWarnings)
            {
                HasSyntaxWarnings = true;
            }


            var codeScope = VisitCodeScope(context.codeScope()) as LSLCodeScopeNode;


            ScopingManager.ExitFunctionScope();


            if (codeScope == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitReturnTypeException("VisitCodeScope", typeof (LSLCodeScopeNode));
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
                    _referencesToNotYetDefinedFunctions.Add(functionSignature.Name, new List<LSLFunctionCallNode> {node});
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
                throw LSLCodeValidatorInternalException.VisitContextInvalidState("VisitLoopStructure");
            }

            return ReturnFromVisit(context, Visit(context.children[0]));
        }


        public override ILSLSyntaxTreeNode VisitDoLoop(LSLParser.DoLoopContext context)
        {
            if (context == null || Utility.AnyNull(context.loop_keyword, context.code))
            {
                throw LSLCodeValidatorInternalException.VisitContextInvalidState("VisitDoLoop");
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
                    GenSyntaxError().DoLoopConditionNotValidType(
                        new LSLSourceCodeRange(context.loop_condition),
                        loopCondition);

                    isError = true;
                }

                if (!isError && loopCondition.IsConstant)
                {
                    GenSyntaxWarning().ConditionalExpressionIsConstant(loopCondition.SourceCodeRange,
                        LSLConditionalStatementType.DoWhile);
                }
            }
            else
            {

                LSLSourceCodeRange errorLocation;

                if (context.open_parenth == null || context.close_parenth == null)
                {
                    errorLocation = new LSLSourceCodeRange(context);
                }
                else
                {
                    errorLocation = new LSLSourceCodeRange(context.open_parenth, context.close_parenth);
                }

                GenSyntaxError().MissingConditionalExpression(errorLocation, LSLConditionalStatementType.DoWhile);

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
                throw LSLCodeValidatorInternalException
                    .VisitReturnTypeException("VisitCodeScopeOrSingleBlockStatement", typeof (LSLCodeScopeNode));
            }


            var result = new LSLDoLoopNode(
                context,
                code,
                loopCondition, InSingleStatementBlock)
            {
                HasErrors = (isError || code.HasErrors)
            };


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitWhileLoop(LSLParser.WhileLoopContext context)
        {
            if (context == null || Utility.AnyNull(context.loop_keyword, context.code))
            {
                throw LSLCodeValidatorInternalException.VisitContextInvalidState("VisitWhileLoop");
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
                    GenSyntaxError().WhileLoopConditionNotValidType(
                        new LSLSourceCodeRange(context.loop_condition),
                        loopCondition);

                    isError = true;
                }


                if (!isError && loopCondition.IsConstant)
                {
                    GenSyntaxWarning().ConditionalExpressionIsConstant(loopCondition.SourceCodeRange,
                        LSLConditionalStatementType.While);
                }
            }
            else
            {

                LSLSourceCodeRange errorLocation;

                if (context.open_parenth == null || context.close_parenth == null)
                {
                    errorLocation = new LSLSourceCodeRange(context);
                }
                else
                {
                    errorLocation = new LSLSourceCodeRange(context.open_parenth, context.close_parenth);
                }

                GenSyntaxError().MissingConditionalExpression(errorLocation, LSLConditionalStatementType.While);

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
                throw LSLCodeValidatorInternalException
                    .VisitReturnTypeException("VisitCodeScopeOrSingleBlockStatement",
                        typeof (LSLCodeScopeNode));
            }


            var result = new LSLWhileLoopNode(
                context,
                loopCondition,
                code, InSingleStatementBlock)
            {
                HasErrors = (code.HasErrors || isError)
            };

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitForLoop(LSLParser.ForLoopContext context)
        {
            if (context == null || Utility.AnyNull(context.loop_keyword, context.code))
            {
                throw LSLCodeValidatorInternalException.VisitContextInvalidState("VisitForLoop");
            }


            var isError = false;


            ILSLExpressionListNode loopInit = null;
            if (context.loop_init != null)
            {
                loopInit = VisitForLoopInitExpressions(context.loop_init) as LSLExpressionListNode;

                if (loopInit == null)
                {
                    throw LSLCodeValidatorInternalException
                        .VisitReturnTypeException("VisitOptionalExpressionList",
                            typeof (LSLExpressionListNode));
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
                    GenSyntaxError().ForLoopConditionNotValidType(
                        new LSLSourceCodeRange(context.loop_condition),
                        loopCondition);

                    isError = true;
                }

                if (!isError && loopCondition.IsConstant)
                {
                    GenSyntaxWarning().ConditionalExpressionIsConstant(loopCondition.SourceCodeRange,
                        LSLConditionalStatementType.For);
                }
            }


            var expressionListRule = context.expression_list;


            var expressionList = VisitForLoopAfterthoughts(expressionListRule) as LSLExpressionListNode;

            if (expressionList == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitReturnTypeException("VisitForLoopAfterthoughts",
                        typeof (LSLExpressionListNode));
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
                throw LSLCodeValidatorInternalException
                    .VisitReturnTypeException("VisitCodeScopeOrSingleBlockStatement",
                        typeof (LSLCodeScopeNode));
            }


            var result = new LSLForLoopNode(
                context,
                loopInit,
                loopCondition,
                expressionList,
                code, InSingleStatementBlock)
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
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitExpressionStatement");
            }

            var expression = VisitTopOfExpression(context.expression_rule);


            if (!expression.HasErrors && !expression.HasPossibleSideEffects)
            {
                GenSyntaxWarning().ExpressionStatementHasNoEffect(new LSLSourceCodeRange(context));
            }


            var result = new LSLExpressionStatementNode(context, expression, InSingleStatementBlock)
            {
                HasErrors = expression.HasErrors
            };

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitReturnStatement(LSLParser.ReturnStatementContext context)
        {
            if (context == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitReturnStatement");
            }


            var containingFunction = ScopingManager.CurrentFunctionBodySignature;

            LSLReturnStatementNode result;

            if (context.return_expression != null)
            {
                var returnExpression = VisitTopOfExpression(context.return_expression);


                if (containingFunction == null)
                {
                    GenSyntaxWarning().ReturnedValueFromEventHandler(
                        new LSLSourceCodeRange(context),
                        ScopingManager.CurrentEventHandlerSignature,
                        returnExpression);
                }
                else if (containingFunction.ReturnType == LSLType.Void)
                {
                    GenSyntaxError().ReturnedValueFromVoidFunction(
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

                //You can return anything from an event handler.
                //If its not inside a function, than it's inside an event.
                var valid = containingFunction == null ||
                    ExpressionValidator.ValidateReturnTypeMatch(containingFunction.ReturnType, returnExpression);


                if (!valid)
                {
                    GenSyntaxError().TypeMismatchInReturnValue(
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
                GenSyntaxError().ReturnedVoidFromANonVoidFunction(
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
                var ctx = (LSLParser.LocalVariableDeclarationContext) context.children[0];

                GenSyntaxError().DefinedVariableInNonScopeBlock(new LSLSourceCodeRange(ctx));
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
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitCodeStatement");
            }


            if (context.semi_colon != null)
            {
                if (!ScopingManager.InSingleStatementBlock)
                {
                    GenSyntaxWarning().UselessSemiColon(new LSLSourceCodeRange(context.semi_colon));
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
                throw new LSLCodeValidatorInternalException(
                    "VisitCodeStatement Visit(context.children[0]) did not return an ILSLCodeStatement");
            }

            return ReturnFromVisit(context, statement);
        }


        public override ILSLSyntaxTreeNode VisitStateChangeStatement(LSLParser.StateChangeStatementContext context)
        {
            if (context == null || context.state_target == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitStateChangeStatement");
            }


            if (!ScopingManager.StatePreDefined(context.state_target.Text))
            {
                var location = new LSLSourceCodeRange(context);

                GenSyntaxError().ChangeToUndefinedState(location, context.state_target.Text);

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
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitJumpStatement");
            }


            if (!ScopingManager.LabelPreDefinedInScope(context.jump_target.Text))
            {
                GenSyntaxError().JumpToUndefinedLabel(
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
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitLabelStatement");
            }

            var label = ScopingManager.ResolvePreDefinedLabelNode(context.label_name.Text);
            if (label == null)
            {
                throw new LSLCodeValidatorInternalException(
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
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitExpr_Assignment");
            }


            ILSLExprNode leftVariable = (ILSLExprNode) Visit(context.expr_lvalue);


            var result = VisitAssignment(
                new AssignmentExpressionContext(leftVariable, context.operation, context.expr_rvalue, context, true));

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
                GenSyntaxError().ModifiedLibraryConstant(location,
                    context.expr_lvalue.GetText());
                result.HasErrors = true;
            }


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitModifiableLeftValue(LSLParser.ModifiableLeftValueContext context)
        {
            if (context.variable != null)
            {
                return GetClonedReferenceToVariableNode(
                    new LSLSourceCodeRange(context.variable),
                    context.variable.Text);
            }

            return Visit(context.dotAccessorExpr());
        }


        public override ILSLSyntaxTreeNode VisitExpr_ModifyingAssignment(
            LSLParser.Expr_ModifyingAssignmentContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitExpr_ModifyingAssignment");
            }


            ILSLExprNode leftVariable = (ILSLExprNode) Visit(context.expr_lvalue);


            var result = VisitAssignment(
                new AssignmentExpressionContext(leftVariable, context.operation, context.expr_rvalue, context,
                    true));


            if (result.HasErrors)
            {
                return ReturnFromVisit(context, result);
            }

            var location = new LSLSourceCodeRange(context.operation);

            //we preformed a modifying operation on a library constant, thats an error
            if (result.LeftExpression.ExpressionType == LSLExpressionType.LibraryConstant)
            {
                GenSyntaxError().ModifiedLibraryConstant(location,
                    context.expr_lvalue.GetText());

                result.HasErrors = true;
            }

            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_MultDivMod(LSLParser.Expr_MultDivModContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalException
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
                throw LSLCodeValidatorInternalException
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
                throw LSLCodeValidatorInternalException
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
                throw LSLCodeValidatorInternalException
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
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitExpr_BitwiseXor");
            }

            var result = VisitBinaryExpression(
                new BinaryExpressionContext(context.expr_lvalue, context.operation, context.expr_rvalue, context));


            return ReturnFromVisit(context, result);
        }


        private LSLVariableNode GetClonedReferenceToVariableNode(LSLSourceCodeRange variableCodeRange, string idText)
        {
            LSLVariableDeclarationNode declaration;
            if (LibraryDataProvider.LibraryConstantExist(idText))
            {
                var librarySignature = LibraryDataProvider.GetLibraryConstantSignature(idText);

                declaration =
                    LSLVariableDeclarationNode.CreateLibraryConstant(librarySignature.Type, idText);

                if (librarySignature.Deprecated)
                {
                    GenSyntaxWarning().UseOfDeprecatedLibraryConstant(
                        variableCodeRange, librarySignature);
                }
            }
            else
            {
                declaration = ScopingManager.ResolveVariable(idText);

                if (declaration != null) return declaration.CreateReference(variableCodeRange);

                GenSyntaxError().UndefinedVariableReference(variableCodeRange.Clone(), idText);

                return LSLVariableNode.GetError(variableCodeRange);
            }

            return declaration.CreateReference(variableCodeRange);
        }


        public override ILSLSyntaxTreeNode VisitExpr_Atom(LSLParser.Expr_AtomContext context)
        {
            if (context == null ||
                context.children == null ||
                context.children[0] == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitExpr_Atom");
            }


            var location = new LSLSourceCodeRange(context);

            if (context.variable != null)
            {
                return ReturnFromVisit(context,
                    GetClonedReferenceToVariableNode(new LSLSourceCodeRange(context.variable), context.variable.Text));
            }
            if (context.integer_literal != null)
            {
                var intLiteralNode = new LSLIntegerLiteralNode(context);

                if (intLiteralNode.IsIntegerLiteralOverflowed())
                {
                    GenSyntaxWarning()
                        .IntegerLiteralOverflow(new LSLSourceCodeRange(context.integer_literal),
                            context.integer_literal.Text);
                }


                return ReturnFromVisit(context, intLiteralNode);
            }
            if (context.float_literal != null)
            {
                return ReturnFromVisit(context, new LSLFloatLiteralNode(context));
            }
            if (context.hex_literal != null)
            {
                var hexLiteralNode = new LSLHexLiteralNode(context);

                if (hexLiteralNode.IsHexLiteralOverflowed())
                {
                    GenSyntaxWarning()
                        .HexLiteralOverflow(new LSLSourceCodeRange(context.hex_literal),
                            context.hex_literal.Text);
                }

                return ReturnFromVisit(context, hexLiteralNode);
            }
            if (context.string_literal != null)
            {
                StringLiteralPreProcessor.ProcessString(context.string_literal.Text);

                if (StringLiteralPreProcessor.HasErrors)
                {
                    foreach (var code in StringLiteralPreProcessor.InvalidEscapeCodes)
                    {
                        GenSyntaxError().InvalidStringEscapeCode(location, code);
                    }

                    foreach (var chr in StringLiteralPreProcessor.IllegalCharacters)
                    {
                        GenSyntaxError().IllegalStringCharacter(location, chr);
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


            throw new LSLCodeValidatorInternalException("VisitExpr_Atom unexpected context state");
        }


        public override ILSLSyntaxTreeNode VisitDotAccessorExpr(LSLParser.DotAccessorExprContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.operation, context.member))
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitExpr_DotAccessorGroup");
            }

            var operatorLocation = new LSLSourceCodeRange(context.operation);

            var variableAccessedLocation = new LSLSourceCodeRange(context.expr_lvalue);

            var accessedMember = context.member.Text;

            var variableReferenceOnLeft = GetClonedReferenceToVariableNode(variableAccessedLocation,
                context.expr_lvalue.Text);


            if (variableReferenceOnLeft.HasErrors)
            {
                return ReturnFromVisit(context, LSLTupleAccessorNode.GetError(variableAccessedLocation));
            }


            if (variableReferenceOnLeft.IsLibraryConstant)
            {
                var libraryConstantReferenced =
                    LibraryDataProvider.GetLibraryConstantSignature(variableReferenceOnLeft.Name);

                GenSyntaxError()
                    .TupleAccessorOnLibraryConstant(operatorLocation, variableReferenceOnLeft, libraryConstantReferenced,
                        accessedMember);

                return ReturnFromVisit(context, LSLTupleAccessorNode.GetError(new LSLSourceCodeRange(context)));
            }


            var isTupleAccess = accessedMember.EqualsOneOf("x", "y", "z", "s");


            if (
                !(isTupleAccess &&
                  (variableReferenceOnLeft.Type == LSLType.Vector || variableReferenceOnLeft.Type == LSLType.Rotation)))
            {
                GenSyntaxError()
                    .InvalidTupleComponentAccessorOperation(operatorLocation, variableReferenceOnLeft, accessedMember);


                return ReturnFromVisit(context, LSLTupleAccessorNode.GetError(
                    new LSLSourceCodeRange(context)));
            }


            var accessedComponent = LSLTupleComponentTools.ParseComponentName(accessedMember);


            if (variableReferenceOnLeft.Type == LSLType.Vector)
            {
                if (accessedMember == "s")
                {
                    GenSyntaxError().InvalidTupleComponentAccessorOperation(
                        operatorLocation, variableReferenceOnLeft, accessedMember);

                    return ReturnFromVisit(context, LSLTupleAccessorNode.GetError(
                        new LSLSourceCodeRange(context)));
                }
            }


            var result = new LSLTupleAccessorNode(context, variableReferenceOnLeft,
                variableReferenceOnLeft.Type, accessedComponent);


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_DotAccessorExpr(LSLParser.Expr_DotAccessorExprContext context)
        {
            return ReturnFromVisit(context, Visit(context.dotAccessorExpr()));
        }


        public override ILSLSyntaxTreeNode VisitParenthesizedExpression(LSLParser.ParenthesizedExpressionContext context)
        {
            if (context == null || context.expr_value == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitParenthisizedExpression");
            }


            if (ScopingManager.InGlobalScope)
            {
                GenSyntaxError().ParenthesizedExpressionUsedInStaticContext(new LSLSourceCodeRange(context));

                return ReturnFromVisit(context,
                    LSLParenthesizedExpressionNode.GetError(new LSLSourceCodeRange(context)));
            }


            var result = VisitExpressionContent(context.expr_value);

            if (result.HasErrors)
            {
                return ReturnFromVisit(context,
                    LSLParenthesizedExpressionNode.GetError(new LSLSourceCodeRange(context)));
            }


            return ReturnFromVisit(context, new LSLParenthesizedExpressionNode(context, result));
        }


        private LSLBinaryExpressionNode VisitAssignment(AssignmentExpressionContext context)
        {
            bool usedInStaticContext = ScopingManager.InGlobalScope;

            if (usedInStaticContext)
            {
                var sourceRange = new LSLSourceCodeRange(context.OperationToken);

                GenSyntaxError().BinaryOperatorUsedInStaticContext(sourceRange);

                return LSLBinaryExpressionNode.GetError(sourceRange);
            }


            var exprLvalue = context.LeftExpr;
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


        private LSLBinaryExpressionNode VisitBinaryExpression(BinaryExpressionContext context)
        {
            bool usedInStaticContext = ScopingManager.InGlobalScope;

            if (usedInStaticContext)
            {
                var sourceRange = new LSLSourceCodeRange(context.OperationToken);

                GenSyntaxError().BinaryOperatorUsedInStaticContext(sourceRange);

                return LSLBinaryExpressionNode.GetError(sourceRange);
            }


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
                throw LSLCodeValidatorInternalException
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
                throw LSLCodeValidatorInternalException
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
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitExpr_TypeCast");
            }


            if (ScopingManager.InGlobalScope)
            {
                var sourceRange = new LSLSourceCodeRange(context);

                GenSyntaxError().CastExpressionUsedInStaticContext(sourceRange);

                return LSLTypecastExprNode.GetError(sourceRange);
            }


            var exprRvalue = VisitExpressionContent(context.expr_rvalue);


            var castType = LSLTypeTools.FromLSLTypeString(context.cast_type.Text);


            var validate = ValidateCastOperation(castType, exprRvalue, new LSLSourceCodeRange(context.cast_type));

            if (validate.IsValid && exprRvalue.Type == castType)
            {
                GenSyntaxWarning().RedundantCast(new LSLSourceCodeRange(context), castType);
            }


            var result = new LSLTypecastExprNode(context, validate.ResultType, exprRvalue)
            {
                HasErrors = !validate.IsValid
            };


            return ReturnFromVisit(context, result);
        }


        /// <summary>
        ///     Validate that a <see cref="LSLFunctionSignature" /> matches up with the parameters that are attempting to be passed
        ///     into it.
        ///     This function generates <see cref="SyntaxErrorListener" /> events
        /// </summary>
        /// <param name="context">The ANTLR context for the parsed function call.</param>
        /// <param name="functionSignature">The function signature of the function call being tested.</param>
        /// <param name="expressions">The expressions that are proposed to be passed into the function with the given signature.</param>
        /// <returns>True if the function call signature matches the provided arguments,  False if it does not.</returns>
        private bool ValidateFunctionCallSignatureMatch(
            LSLParser.Expr_FunctionCallContext context,
            LSLFunctionSignature functionSignature,
            IReadOnlyGenericArray<ILSLExprNode> expressions)
        {
            var location = new LSLSourceCodeRange(context);


            var match = LSLFunctionSignatureMatcher.TryMatch(functionSignature, expressions, ExpressionValidator);

            if (match.ImproperParameterCount)
            {
                GenSyntaxError().ImproperParameterCountInFunctionCall(
                    location,
                    functionSignature,
                    expressions.ToArray());

                return false;
            }


            if (!match.TypeMismatch) return true;


            GenSyntaxError().ParameterTypeMismatchInFunctionCall(
                location,
                match.TypeMismatchIndex,
                functionSignature,
                expressions.ToArray());


            return false;
        }


        /// <summary>
        ///     Find a matching overload from a list of function signatures, generate SyntaxErrors using the
        ///     <see cref="SyntaxErrorListener" /> if none is found.
        ///     Returns null if no overload is found.
        /// </summary>
        /// <param name="context">The ANTLR context for the function call expression.</param>
        /// <param name="functionSignatures">Function signatures to check for overload matches in.</param>
        /// <param name="expressions">The expressions that are proposed to be passed into the function with the given signature.</param>
        /// <returns>A matching LSLLibraryFunctionSignature overload or null</returns>
        private LSLLibraryFunctionSignature ValidateLibraryFunctionCallSignatureMatch(
            LSLParser.Expr_FunctionCallContext context,
            IReadOnlyGenericArray<LSLLibraryFunctionSignature> functionSignatures,
            IReadOnlyGenericArray<ILSLExprNode> expressions)
        {
            if (functionSignatures.Count() == 1)
            {
                var signature = functionSignatures.First();
                var match = ValidateFunctionCallSignatureMatch(context, signature, expressions);

                return match ? signature : null;
            }

            var matchResults = LSLFunctionSignatureMatcher.MatchOverloads(functionSignatures, expressions,
                ExpressionValidator);


            if (matchResults.Success) return matchResults.MatchingOverload;

            if (matchResults.Ambiguous)
            {
                GenSyntaxError().CallToOverloadedLibraryFunctionIsAmbigious(new LSLSourceCodeRange(context),
                    context.function_name.Text, matchResults.Matches, expressions);
            }
            else
            {
                GenSyntaxError().NoSuitableLibraryFunctionOverloadFound(new LSLSourceCodeRange(context),
                    context.function_name.Text, expressions);
            }

            return null;
        }


        public override ILSLSyntaxTreeNode VisitExpr_FunctionCall(LSLParser.Expr_FunctionCallContext context)
        {
            if (context == null || context.function_name == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitExpr_FunctionCall");
            }


            var location = new LSLSourceCodeRange(context);

            if (ScopingManager.InGlobalScope)
            {
                GenSyntaxError().CallToFunctionInStaticContext(location.Clone());

                return ReturnFromVisit(context, LSLFunctionCallNode.GetError(location));
            }


            var expressionListRule = context.expression_list;


            LSLFunctionCallNode result;
            var functionName = context.function_name.Text;


            if (LibraryDataProvider.LibraryFunctionExist(functionName))
            {
                var expressionList = VisitFunctionCallParameters(expressionListRule,
                    LSLExpressionListType.LibraryFunctionCallParameters) as LSLExpressionListNode;

                if (expressionList == null)
                {
                    throw LSLCodeValidatorInternalException
                        .VisitReturnTypeException("VisitFunctionCallParameters", typeof(LSLExpressionListNode));
                }


                var functionSignatures = LibraryDataProvider.GetLibraryFunctionSignatures(functionName);


                LSLLibraryFunctionSignature functionSignature = null;
                if (!expressionList.HasErrors)
                {
                    functionSignature = ValidateLibraryFunctionCallSignatureMatch(context, functionSignatures,
                        expressionList.ExpressionNodes);

                    if (functionSignature == null)
                    {
                        return LSLFunctionCallNode.GetError(location);
                    }

#if DEBUG_OVERLOAD_MATCHES

                    Debug.WriteLine("Overload match: "+functionSignature.SignatureString);
#endif


                    if (functionSignature.Deprecated)
                    {
                        GenSyntaxWarning()
                            .UseOfDeprecatedLibraryFunction(location.Clone(), functionSignature);
                    }
                }

                if (functionSignature == null)
                {
                    //Just make a dummy.
                    //Theres a syntax error in the parameters but we still want this function to count as referenced.
                    //LSLFunctionCallNode's constructor will throw if it's given a null signature
                    functionSignature = new LSLLibraryFunctionSignature(LSLType.Void, functionName);
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
                    throw LSLCodeValidatorInternalException
                        .VisitReturnTypeException("VisitFunctionCallParameters", typeof(LSLExpressionListNode));
                }

                //Guaranteed to return non null, checked first with:
                //
                // ScopingManager.FunctionIsPreDefined(functionName)
                //
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
                GenSyntaxError().CallToUndefinedFunction(location.Clone(), functionName);

                return ReturnFromVisit(context, LSLFunctionCallNode.GetError(location));
            }


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_Logical_And_Or(LSLParser.Expr_Logical_And_OrContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitExpr_Logical_And_Or");
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


            if (exprRvalue.ExpressionType == LSLExpressionType.LibraryConstant && result.Operation.IsModifying())
            {
                GenSyntaxError().ModifiedLibraryConstant(
                    exprRvalue.SourceCodeRange, context.expr_rvalue.GetText());

                result.HasErrors = true;
                return ReturnFromVisit(context, result);
            }


            if (ScopingManager.InGlobalScope)
            {
                if (result.RightExpression.ExpressionType == LSLExpressionType.GlobalVariable)
                {
                    GenSyntaxError()
                        .InvalidPrefixOperationUsedGlobalVariableInStaticContext(new LSLSourceCodeRange(context),
                            result.Operation);

                    result.HasErrors = true;
                    return ReturnFromVisit(context, result);
                }
                if (result.Operation != LSLPrefixOperationType.Negative)
                {
                    GenSyntaxError()
                        .InvalidPrefixOperationUsedInStaticContext(new LSLSourceCodeRange(context), result.Operation);

                    result.HasErrors = true;
                    return ReturnFromVisit(context, result);
                }
                if (result.RightExpression.Type == LSLType.Vector)
                {
                    GenSyntaxError().NegateOperationOnVectorLiteralInStaticContext(
                        new LSLSourceCodeRange(context));

                    result.HasErrors = true;
                    return ReturnFromVisit(context, result);
                }
                if (result.RightExpression.Type == LSLType.Rotation)
                {
                    GenSyntaxError().NegateOperationOnRotationLiteralInStaticContext(
                        new LSLSourceCodeRange(context));

                    result.HasErrors = true;
                    return ReturnFromVisit(context, result);
                }
            }


            if (result.Operation.IsModifying() && !result.RightExpression.IsVariableOrParameter())
            {
                GenSyntaxError()
                    .ModifyingPrefixOperationOnNonVariable(new LSLSourceCodeRange(context), result.Operation);
                result.HasErrors = true;
                return ReturnFromVisit(context, result);
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


            if (ScopingManager.InGlobalScope)
            {
                GenSyntaxError().PostfixOperationUsedInStaticContext(new LSLSourceCodeRange(context));

                return ReturnFromVisit(context, LSLPostfixOperationNode.GetError(new LSLSourceCodeRange(context)));
            }


            var exprLvalue = VisitExpressionContent(context.expr_lvalue);


            var validate = ValidatePostfixOperation(exprLvalue, context.operation.Text,
                new LSLSourceCodeRange(context.operation));


            var result = new LSLPostfixOperationNode(context, validate.ResultType, exprLvalue)
            {
                HasErrors = !validate.IsValid
            };


            if (exprLvalue.ExpressionType == LSLExpressionType.LibraryConstant && result.Operation.IsModifying())
            {
                GenSyntaxError().ModifiedLibraryConstant(
                    exprLvalue.SourceCodeRange, context.expr_lvalue.GetText());
                result.HasErrors = true;
                return ReturnFromVisit(context, result);
            }
            if (!exprLvalue.IsVariableOrParameter())
            {
                GenSyntaxError().PostfixOperationOnNonVariable(new LSLSourceCodeRange(context), result.Operation);
                result.HasErrors = true;
                return ReturnFromVisit(context, result);
            }


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitExpr_BitwiseShift(LSLParser.Expr_BitwiseShiftContext context)
        {
            if (context == null || Utility.AnyNull(context.expr_lvalue, context.expr_rvalue, context.operation))
            {
                throw LSLCodeValidatorInternalException
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
                throw LSLCodeValidatorInternalException
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


            if (!ExpressionValidator.ValidateVectorContent(x))
            {
                GenSyntaxError().InvalidVectorContent(
                    new LSLSourceCodeRange(context.vector_x), LSLVectorComponent.X, x);
                result.HasErrors = true;
            }

            if (!ExpressionValidator.ValidateVectorContent(y))
            {
                GenSyntaxError().InvalidVectorContent(
                    new LSLSourceCodeRange(context.vector_y), LSLVectorComponent.Y, y);
                result.HasErrors = true;
            }

            if (!ExpressionValidator.ValidateVectorContent(z))
            {
                GenSyntaxError().InvalidVectorContent(
                    new LSLSourceCodeRange(context.vector_z), LSLVectorComponent.Z, z);
                result.HasErrors = true;
            }


            return ReturnFromVisit(context, result);
        }


        public override ILSLSyntaxTreeNode VisitListLiteral(LSLParser.ListLiteralContext context)
        {
            if (context == null)
            {
                throw LSLCodeValidatorInternalException
                    .VisitContextInvalidState("VisitListLiteral");
            }


            var expressionList = VisitListLiteralInitializerList(context.expression_list) as LSLExpressionListNode;

            if (expressionList == null)
            {
                throw LSLCodeValidatorInternalException.VisitReturnTypeException(
                    "VisitListLiteralExpressionList",
                    typeof (LSLExpressionListNode));
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
                throw LSLCodeValidatorInternalException
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


            if (!ExpressionValidator.ValidateRotationContent(x))
            {
                GenSyntaxError().InvalidRotationContent(
                    new LSLSourceCodeRange(context.rotation_x), LSLRotationComponent.X, x);
                result.HasErrors = true;
            }

            if (!ExpressionValidator.ValidateRotationContent(y))
            {
                GenSyntaxError().InvalidRotationContent(
                    new LSLSourceCodeRange(context.rotation_y), LSLRotationComponent.Y, y);
                result.HasErrors = true;
            }

            if (!ExpressionValidator.ValidateRotationContent(z))
            {
                GenSyntaxError().InvalidRotationContent(
                    new LSLSourceCodeRange(context.rotation_z), LSLRotationComponent.Z, z);
                result.HasErrors = true;
            }

            if (!ExpressionValidator.ValidateRotationContent(s))
            {
                GenSyntaxError().InvalidRotationContent(
                    new LSLSourceCodeRange(context.rotation_s), LSLRotationComponent.S, s);
                result.HasErrors = true;
            }


            return ReturnFromVisit(context, result);
        }

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
                GenSyntaxError().InvalidBinaryOperation(
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
                GenSyntaxError().InvalidPrefixOperation(
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
                GenSyntaxError().InvalidPostfixOperation(
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
                GenSyntaxError().InvalidCastOperation(
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
        // Mono/Net Byte-code, and warn that they are invalid

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
                        GenSyntaxWarning().MultipleListAssignmentsInExpression(operationLocation);
                        _multipleListAssignmentWarned.Add(warnedNode);
                        _lastContextWithListAssign = null;
                        return;
                    }

                    parent = parent.Parent;
                }

                if (parent is LSLParser.LocalVariableDeclarationContext)
                {
                    GenSyntaxWarning().MultipleListAssignmentsInExpression(operationLocation);
                    _multipleListAssignmentWarned.Add(warnedNode);
                    _lastContextWithListAssign = null;
                    return;
                }

                if (parent is LSLParser.GlobalVariableDeclarationContext)
                {
                    GenSyntaxWarning().MultipleListAssignmentsInExpression(operationLocation);
                    _multipleListAssignmentWarned.Add(warnedNode);
                    _lastContextWithListAssign = null;
                    return;
                }

                if (_lastContextWithListAssign != null)
                {
                    if (ReferenceEquals(_lastContextWithListAssign, parent))
                    {
                        GenSyntaxWarning().MultipleListAssignmentsInExpression(operationLocation);
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
                        GenSyntaxWarning().MultipleStringAssignmentsInExpression(operationLocation);
                        _multipleStringAssignmentWarned.Add(warnedNode);
                        _lastContextWithStringAssign = null;
                        return;
                    }

                    parent = parent.Parent;
                }

                if (parent is LSLParser.LocalVariableDeclarationContext)
                {
                    GenSyntaxWarning().MultipleStringAssignmentsInExpression(operationLocation);
                    _multipleStringAssignmentWarned.Add(warnedNode);
                    _lastContextWithStringAssign = null;
                    return;
                }

                if (parent is LSLParser.GlobalVariableDeclarationContext)
                {
                    GenSyntaxWarning().MultipleStringAssignmentsInExpression(operationLocation);
                    _multipleStringAssignmentWarned.Add(warnedNode);
                    _lastContextWithStringAssign = null;
                    return;
                }

                if (_lastContextWithStringAssign != null)
                {
                    if (ReferenceEquals(_lastContextWithStringAssign, parent))
                    {
                        GenSyntaxWarning().MultipleStringAssignmentsInExpression(operationLocation);
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