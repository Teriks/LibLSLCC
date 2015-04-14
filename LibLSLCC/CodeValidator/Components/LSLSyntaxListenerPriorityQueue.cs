using System;
using System.Collections.Generic;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;
using LibLSLCC.Collections;

namespace LibLSLCC.CodeValidator.Components
{
    public sealed class LSLSyntaxListenerPriorityQueue : ILSLSyntaxErrorListener, ILSLSyntaxWarningListener
    {
        private readonly PriorityQueue<int, Action> _errorActionQueue = new PriorityQueue<int, Action>();
        private readonly PriorityQueue<int, Action> _warningActionQueue = new PriorityQueue<int, Action>();



        public LSLSyntaxListenerPriorityQueue(ILSLSyntaxErrorListener invokeErrorsOn,
            ILSLSyntaxWarningListener invokeWarningsOn)
        {
            SyntaxErrorListener = invokeErrorsOn;
            SyntaxWarningListener = invokeWarningsOn;
        }



        public ILSLSyntaxErrorListener SyntaxErrorListener { get; private set; }
        public ILSLSyntaxWarningListener SyntaxWarningListener { get; private set; }



        void ILSLSyntaxErrorListener.GrammarLevelSyntaxError(int line, int column, string message)
        {
            _errorActionQueue.Enqueue(line,
                () => SyntaxErrorListener.GrammarLevelSyntaxError(line, column, message));
        }



        void ILSLSyntaxErrorListener.UndefinedVariableReference(LSLSourceCodeRange location, string name)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.UndefinedVariableReference(location, name));
        }



        void ILSLSyntaxErrorListener.ParameterNameRedefined(LSLSourceCodeRange location, LSLType type, string name)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ParameterNameRedefined(location, type, name));
        }



        void ILSLSyntaxErrorListener.InvalidBinaryOperation(LSLSourceCodeRange location, ILSLExprNode left,
            string operation,
            ILSLExprNode right)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidBinaryOperation(location, left, operation, right));
        }



        void ILSLSyntaxErrorListener.InvalidPrefixOperation(LSLSourceCodeRange location, string operation,
            ILSLExprNode right)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidPrefixOperation(location, operation, right));
        }



        void ILSLSyntaxErrorListener.InvalidPostfixOperation(LSLSourceCodeRange location, ILSLExprNode left,
            string operation)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidPostfixOperation(location, left, operation));
        }



        void ILSLSyntaxErrorListener.InvalidCastOperation(LSLSourceCodeRange location, LSLType castTo,
            ILSLExprNode fromExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidCastOperation(location, castTo, fromExpression));
        }



        void ILSLSyntaxErrorListener.TypeMismatchInVariableDeclaration(LSLSourceCodeRange location, LSLType variableType,
            ILSLExprNode assignedExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.TypeMismatchInVariableDeclaration(location, variableType, assignedExpression));
        }



        void ILSLSyntaxErrorListener.VariableRedefined(LSLSourceCodeRange location, LSLType variableType,
            string variableName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.VariableRedefined(location, variableType, variableName));
        }



        void ILSLSyntaxErrorListener.InvalidVectorContent(LSLSourceCodeRange location, LSLVectorComponent component,
            ILSLExprNode invalidExpressionContent)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidVectorContent(location, component, invalidExpressionContent));
        }



        void ILSLSyntaxErrorListener.InvalidListContent(LSLSourceCodeRange location, int index,
            ILSLExprNode invalidExpressionContent)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidListContent(location, index, invalidExpressionContent));
        }



        void ILSLSyntaxErrorListener.InvalidRotationContent(LSLSourceCodeRange location, LSLRotationComponent component,
            ILSLExprNode invalidExpressionContent)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidRotationContent(location, component, invalidExpressionContent));
        }



        void ILSLSyntaxErrorListener.ReturnedValueFromVoidFunction(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature,
            ILSLExprNode attemptedReturnExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.ReturnedValueFromVoidFunction(location, functionSignature,
                        attemptedReturnExpression));
        }



        void ILSLSyntaxErrorListener.TypeMismatchInReturnValue(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature,
            ILSLExprNode attemptedReturnExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.TypeMismatchInReturnValue(location, functionSignature,
                        attemptedReturnExpression));
        }



        void ILSLSyntaxErrorListener.ReturnedVoidFromANonVoidFunction(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ReturnedVoidFromANonVoidFunction(location, functionSignature));
        }



        void ILSLSyntaxErrorListener.JumpToUndefinedLabel(LSLSourceCodeRange location, string labelName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.JumpToUndefinedLabel(location, labelName));
        }



        void ILSLSyntaxErrorListener.CallToUndefinedFunction(LSLSourceCodeRange location, string functionName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.CallToUndefinedFunction(location, functionName));
        }



        void ILSLSyntaxErrorListener.ImproperParameterCountInFunctionCall(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature,
            ILSLExprNode[] parameterExpressionsGiven)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.ImproperParameterCountInFunctionCall(location, functionSignature,
                        parameterExpressionsGiven));
        }



        void ILSLSyntaxErrorListener.ReturnedValueFromEventHandler(LSLSourceCodeRange location,
            ILSLExprNode attemptedReturnExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ReturnedValueFromEventHandler(location, attemptedReturnExpression));
        }



        void ILSLSyntaxErrorListener.RedefinedFunction(LSLSourceCodeRange location,
            LSLFunctionSignature previouslyDefinedSignature)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.RedefinedFunction(location, previouslyDefinedSignature));
        }



        void ILSLSyntaxErrorListener.RedefinedLabel(LSLSourceCodeRange location, string labelName)
        {
            _errorActionQueue.Enqueue(location.StartIndex, () => SyntaxErrorListener.RedefinedLabel(location, labelName));
        }



        void ILSLSyntaxErrorListener.TupleAccessorOnLiteral(LSLSourceCodeRange location, ILSLExprNode lvalueLiteral,
            string operationText)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.TupleAccessorOnLiteral(location, lvalueLiteral, operationText));
        }



        void ILSLSyntaxErrorListener.TupleAccessorOnCompoundExpression(LSLSourceCodeRange location,
            ILSLExprNode lvalueCompound,
            string operationText)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.TupleAccessorOnCompoundExpression(location, lvalueCompound, operationText));
        }



        void ILSLSyntaxErrorListener.InvalidComponentAccessorOperation(LSLSourceCodeRange location,
            ILSLExprNode exprLvalue,
            string componentAccessed)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidComponentAccessorOperation(location, exprLvalue, componentAccessed));
        }



        void ILSLSyntaxErrorListener.IfConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.IfConditionNotValidType(location, attemptedConditionExpression));
        }



        void ILSLSyntaxErrorListener.ElseIfConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ElseIfConditionNotValidType(location, attemptedConditionExpression));
        }



        void ILSLSyntaxErrorListener.DoLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.DoLoopConditionNotValidType(location, attemptedConditionExpression));
        }



        void ILSLSyntaxErrorListener.WhileLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.WhileLoopConditionNotValidType(location, attemptedConditionExpression));
        }



        void ILSLSyntaxErrorListener.ForLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ForLoopConditionNotValidType(location, attemptedConditionExpression));
        }



        void ILSLSyntaxErrorListener.ParameterTypeMismatchInFunctionCall(LSLSourceCodeRange location,
            int parameterNumberWithError,
            LSLFunctionSignature calledFunction, ILSLExprNode[] parameterExpressionsGiven)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.ParameterTypeMismatchInFunctionCall(location, parameterNumberWithError,
                        calledFunction, parameterExpressionsGiven));
        }



        void ILSLSyntaxErrorListener.RedefinedStateName(LSLSourceCodeRange location, string stateName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.RedefinedStateName(location, stateName));
        }



        void ILSLSyntaxErrorListener.UnknownEventHandlerDeclared(LSLSourceCodeRange location,
            LSLEventSignature givenEventHandlerSignature)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.UnknownEventHandlerDeclared(location, givenEventHandlerSignature));
        }



        void ILSLSyntaxErrorListener.IncorrectEventHandlerSignature(LSLSourceCodeRange location,
            LSLEventSignature givenEventHandlerSignature,
            LSLLibraryEventSignature correctEventHandlerSignature)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.IncorrectEventHandlerSignature(location, givenEventHandlerSignature,
                        correctEventHandlerSignature));
        }



        void ILSLSyntaxErrorListener.RedefinedStandardLibraryConstant(LSLSourceCodeRange location,
            LSLType redefinitionType,
            LSLLibraryConstantSignature originalSignature)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.RedefinedStandardLibraryConstant(location, redefinitionType,
                        originalSignature));
        }



        void ILSLSyntaxErrorListener.RedefinedStandardLibraryFunction(LSLSourceCodeRange location, string functionName,
            IReadOnlyList<LSLLibraryFunctionSignature> libraryFunctionSignatureOverloads)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.RedefinedStandardLibraryFunction(location, functionName,libraryFunctionSignatureOverloads));
        }



        void ILSLSyntaxErrorListener.ChangeToUndefinedState(LSLSourceCodeRange location, string stateName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ChangeToUndefinedState(location, stateName));
        }



        void ILSLSyntaxErrorListener.ModifiedLibraryConstant(LSLSourceCodeRange location, string constantName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ModifiedLibraryConstant(location, constantName));
        }



        void ILSLSyntaxErrorListener.RedefinedDefaultState(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex, () => SyntaxErrorListener.RedefinedDefaultState(location));
        }



        void ILSLSyntaxErrorListener.InvalidStatementExpression(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex, () => SyntaxErrorListener.InvalidStatementExpression(location));
        }



        void ILSLSyntaxErrorListener.DeadCodeAfterReturnPathDetected(LSLSourceCodeRange location,
            LSLFunctionSignature inFunction, LSLDeadCodeSegment deadSegment)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.DeadCodeAfterReturnPathDetected(location, inFunction, deadSegment));
        }



        void ILSLSyntaxErrorListener.NotAllCodePathsReturnAValue(LSLSourceCodeRange location,
            LSLFunctionSignature inFunction)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.NotAllCodePathsReturnAValue(location, inFunction));
        }



        void ILSLSyntaxErrorListener.StateHasNoEventHandlers(LSLSourceCodeRange location, string stateName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.StateHasNoEventHandlers(location, stateName));
        }



        void ILSLSyntaxErrorListener.MissingConditionalExpression(LSLSourceCodeRange location,
            LSLConditionalStatementType statementType)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.MissingConditionalExpression(location, statementType));
        }



        void ILSLSyntaxErrorListener.DefinedVariableInNonScopeBlock(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.DefinedVariableInNonScopeBlock(location));
        }



        void ILSLSyntaxErrorListener.IllegalStringCharacter(LSLSourceCodeRange location, LSLStringCharacterError chr)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.IllegalStringCharacter(location, chr));
        }



        void ILSLSyntaxErrorListener.InvalidStringEscapeCode(LSLSourceCodeRange location, LSLStringCharacterError code)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidStringEscapeCode(location, code));
        }



        void ILSLSyntaxErrorListener.CallToFunctionInStaticContext(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.CallToFunctionInStaticContext(location));
        }



        void ILSLSyntaxErrorListener.ModifyingAssignmentToCompoundExpression(LSLSourceCodeRange location,
            string operation)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ModifyingAssignmentToCompoundExpression(location, operation));
        }



        void ILSLSyntaxErrorListener.AssignmentToCompoundExpression(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.AssignmentToCompoundExpression(location));
        }



        void ILSLSyntaxErrorListener.AssignmentToLiteral(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex, () => SyntaxErrorListener.AssignmentToLiteral(location));
        }



        void ILSLSyntaxErrorListener.ModifyingAssignmentToLiteral(LSLSourceCodeRange location, string operation)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ModifyingAssignmentToLiteral(location, operation));
        }



        void ILSLSyntaxErrorListener.RedefinedEventHandler(LSLSourceCodeRange location, string eventHandlerName,
            string stateName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.RedefinedEventHandler(location, eventHandlerName, stateName));
        }



        void ILSLSyntaxErrorListener.MissingDefaultState()
        {
            _errorActionQueue.Enqueue(0, () => SyntaxErrorListener.MissingDefaultState());
        }



        public void NoSuitableLibraryFunctionOverloadFound(LSLSourceCodeRange location, string functionName,
            IReadOnlyList<ILSLExprNode> givenParameters)
        {
            _errorActionQueue.Enqueue(location.StartIndex, () => SyntaxErrorListener.NoSuitableLibraryFunctionOverloadFound(location, functionName, givenParameters));
        }



        void ILSLSyntaxWarningListener.MultipleListAssignmentsInExpression(LSLSourceCodeRange location)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.MultipleListAssignmentsInExpression(location));
        }



        void ILSLSyntaxWarningListener.MultipleStringAssignmentsInExpression(LSLSourceCodeRange location)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.MultipleStringAssignmentsInExpression(location));
        }



        void ILSLSyntaxWarningListener.OnWarning(LSLSourceCodeRange location, string message)
        {
            _warningActionQueue.Enqueue(location.StartIndex, () => SyntaxWarningListener.OnWarning(location, message));
        }



        void ILSLSyntaxWarningListener.DeadCodeDetected(LSLSourceCodeRange location,
            LSLFunctionSignature currentFunction, LSLDeadCodeSegment deadSegment)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.DeadCodeDetected(location, currentFunction, deadSegment));
        }



        void ILSLSyntaxWarningListener.DeadCodeDetected(LSLSourceCodeRange location,
            LSLEventSignature currentEvent, LSLDeadCodeSegment deadSegment)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.DeadCodeDetected(location, currentEvent, deadSegment));
        }



        void ILSLSyntaxWarningListener.UselessSemiColon(LSLSourceCodeRange location)
        {
            _warningActionQueue.Enqueue(location.StartIndex, () => SyntaxWarningListener.UselessSemiColon(location));
        }



        void ILSLSyntaxWarningListener.ExpressionStatementHasNoEffect(LSLSourceCodeRange location)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.ExpressionStatementHasNoEffect(location));
        }



        void ILSLSyntaxWarningListener.ForLoopAfterthoughtHasNoEffect(LSLSourceCodeRange location, int expressionIndex,
            int expressionCountTotal)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.ForLoopAfterthoughtHasNoEffect(location, expressionIndex,
                        expressionCountTotal));
        }



        void ILSLSyntaxWarningListener.RedundantCast(LSLSourceCodeRange location, LSLType castType)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.RedundantCast(location, castType));
        }



        public void FunctionNeverUsed(LSLSourceCodeRange location, ILSLFunctionDeclarationNode function)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
               () => SyntaxWarningListener.FunctionNeverUsed(location, function));
        }



        public void GlobalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
               () => SyntaxWarningListener.GlobalVariableNeverUsed(location, variable));
        }



        public void LocalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable,
            LSLFunctionSignature inFunction)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
               () => SyntaxWarningListener.LocalVariableNeverUsed(location, variable, inFunction));
        }



        public void LocalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable,
            LSLEventSignature inEvent)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
               () => SyntaxWarningListener.LocalVariableNeverUsed(location, variable, inEvent));
        }



        public void FunctionParameterNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode parameter,LSLFunctionSignature inFunction)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
               () => SyntaxWarningListener.FunctionParameterNeverUsed(location, parameter, inFunction));
        }



        public void ParameterHidesGlobalVariable(LSLSourceCodeRange location, LSLFunctionSignature functionSignature,
            LSLParameterNode parameter, LSLVariableDeclarationNode globalVariable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
               () => SyntaxWarningListener.ParameterHidesGlobalVariable(location, functionSignature, parameter, globalVariable));
        }



        public void ParameterHidesGlobalVariable(LSLSourceCodeRange location, LSLEventSignature eventHandlerSignature,
            LSLParameterNode parameter, LSLVariableDeclarationNode globalVariable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
               () => SyntaxWarningListener.ParameterHidesGlobalVariable(location, eventHandlerSignature, parameter, globalVariable));
        }



        public void LocalVariableHidesParameter(LSLSourceCodeRange location, LSLPreDefinedFunctionSignature functionSignature,
            LSLVariableDeclarationNode localVariable, LSLParameterNode parameter)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
              () => SyntaxWarningListener.LocalVariableHidesParameter(location,functionSignature,localVariable,parameter));
        }



        public void LocalVariableHidesParameter(LSLSourceCodeRange location, LSLParsedEventHandlerSignature eventHandlerSignature,
            LSLVariableDeclarationNode localVariable, LSLParameterNode parameter)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
              () => SyntaxWarningListener.LocalVariableHidesParameter(location, eventHandlerSignature, localVariable, parameter));
        }



        public void LocalVariableHidesGlobalVariable(LSLSourceCodeRange location, LSLPreDefinedFunctionSignature functionSignature,
            LSLVariableDeclarationNode localVariable, LSLVariableDeclarationNode globalVariable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
              () => SyntaxWarningListener.LocalVariableHidesGlobalVariable(location,functionSignature,localVariable,globalVariable));
        }



        public void LocalVariableHidesGlobalVariable(LSLSourceCodeRange location, LSLParsedEventHandlerSignature eventHandlerSignature,
            LSLVariableDeclarationNode localVariable, LSLVariableDeclarationNode globalVariable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
              () => SyntaxWarningListener.LocalVariableHidesGlobalVariable(location, eventHandlerSignature, localVariable, globalVariable));
        }



        public void InvokeQueuedActions()
        {
            while (!_errorActionQueue.IsEmpty)
            {
                _errorActionQueue.DequeueValue()();
            }

            while (!_warningActionQueue.IsEmpty)
            {
                _warningActionQueue.DequeueValue()();
            }
        }
    }
}