#region FileInfo

// 
// File: LSLSyntaxListenerPriorityQueue.cs
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
using LibLSLCC.Collections;
using LibLSLCC.LibraryData;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     A proxy object for <see cref="ILSLSyntaxErrorListener" /> and <see cref="ILSLSyntaxWarningListener" /> that
    ///     queue's/sorts syntax warnings/errors by their starting index in source code.
    /// </summary>
    public sealed class LSLSyntaxListenerPriorityQueue : ILSLSyntaxErrorListener, ILSLSyntaxWarningListener
    {
        private readonly PriorityQueue<int, Action> _errorActionQueue = new PriorityQueue<int, Action>();
        private readonly PriorityQueue<int, Action> _warningActionQueue = new PriorityQueue<int, Action>();


        /// <summary>
        ///     Construct a <see cref="LSLSyntaxListenerPriorityQueue" /> by wrapping another
        ///     <see cref="ILSLSyntaxErrorListener" /> and <see cref="ILSLSyntaxWarningListener" />
        /// </summary>
        /// <param name="invokeErrorsOn">
        ///     Syntax errors will be priority queued by their index in the source code and invoked on
        ///     this object.
        /// </param>
        /// <param name="invokeWarningsOn">
        ///     Syntax warnings will be priority queued by their index in the source code and invoked on
        ///     this object.
        /// </param>
        public LSLSyntaxListenerPriorityQueue(
            ILSLSyntaxErrorListener invokeErrorsOn,
            ILSLSyntaxWarningListener invokeWarningsOn)
        {
            SyntaxErrorListener = invokeErrorsOn;
            SyntaxWarningListener = invokeWarningsOn;
        }


        /// <summary>
        ///     Gets the number of syntax warnings that have been queued.
        /// </summary>
        /// <value>
        ///     The number of syntax warnings queued.
        /// </value>
        public int NumberOfSyntaxWarnings
        {
            get { return _warningActionQueue.Count; }
        }

        /// <summary>
        ///     Gets the number of syntax errors that have been queued.
        /// </summary>
        /// <value>
        ///     The number of syntax errors queued.
        /// </value>
        public int NumberOfSyntaxErrors
        {
            get { return _errorActionQueue.Count; }
        }

        /// <summary>
        ///     The syntax error listener that this object delegates to.
        /// </summary>
        public ILSLSyntaxErrorListener SyntaxErrorListener { get; private set; }

        /// <summary>
        ///     The syntax warning listener that this object delegates to.
        /// </summary>
        public ILSLSyntaxWarningListener SyntaxWarningListener { get; private set; }


        void ILSLSyntaxErrorListener.GrammarLevelParserSyntaxError(int line, int column,
            LSLSourceCodeRange offendingTokenRange, string offendingTokenText, string message)
        {
            _errorActionQueue.Enqueue(line,
                () =>
                    SyntaxErrorListener.GrammarLevelParserSyntaxError(line, column, offendingTokenRange,
                        offendingTokenText, message));
        }


        void ILSLSyntaxErrorListener.UndefinedVariableReference(LSLSourceCodeRange location, string name)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.UndefinedVariableReference(location, name));
        }


        void ILSLSyntaxErrorListener.ParameterNameRedefined(LSLSourceCodeRange location,
            LSLParameterListType parameterListType, LSLType type, string name)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ParameterNameRedefined(location, parameterListType, type, name));
        }


        void ILSLSyntaxErrorListener.InvalidBinaryOperation(LSLSourceCodeRange location, ILSLReadOnlyExprNode left,
            string operation,
            ILSLReadOnlyExprNode right)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidBinaryOperation(location, left, operation, right));
        }


        void ILSLSyntaxErrorListener.InvalidPrefixOperation(LSLSourceCodeRange location, string operation,
            ILSLReadOnlyExprNode right)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidPrefixOperation(location, operation, right));
        }


        void ILSLSyntaxErrorListener.InvalidPostfixOperation(LSLSourceCodeRange location, ILSLReadOnlyExprNode left,
            string operation)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidPostfixOperation(location, left, operation));
        }


        void ILSLSyntaxErrorListener.InvalidCastOperation(LSLSourceCodeRange location, LSLType castTo,
            ILSLReadOnlyExprNode fromExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidCastOperation(location, castTo, fromExpression));
        }


        void ILSLSyntaxErrorListener.TypeMismatchInVariableDeclaration(LSLSourceCodeRange location, LSLType variableType,
            ILSLReadOnlyExprNode assignedExpression)
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
            ILSLReadOnlyExprNode invalidExpressionContent)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidVectorContent(location, component, invalidExpressionContent));
        }


        void ILSLSyntaxErrorListener.InvalidListContent(LSLSourceCodeRange location, int index,
            ILSLReadOnlyExprNode invalidExpressionContent)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidListContent(location, index, invalidExpressionContent));
        }


        void ILSLSyntaxErrorListener.InvalidRotationContent(LSLSourceCodeRange location, LSLRotationComponent component,
            ILSLReadOnlyExprNode invalidExpressionContent)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidRotationContent(location, component, invalidExpressionContent));
        }


        void ILSLSyntaxErrorListener.ReturnedValueFromVoidFunction(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature,
            ILSLReadOnlyExprNode attemptedReturnExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.ReturnedValueFromVoidFunction(location, functionSignature,
                        attemptedReturnExpression));
        }


        void ILSLSyntaxErrorListener.TypeMismatchInReturnValue(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature,
            ILSLReadOnlyExprNode attemptedReturnExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.TypeMismatchInReturnValue(location, functionSignature,
                        attemptedReturnExpression));
        }


        void ILSLSyntaxErrorListener.ReturnedVoidFromNonVoidFunction(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ReturnedVoidFromNonVoidFunction(location, functionSignature));
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
            ILSLReadOnlyExprNode[] parameterExpressionsGiven)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.ImproperParameterCountInFunctionCall(location, functionSignature,
                        parameterExpressionsGiven));
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


        void ILSLSyntaxErrorListener.InvalidTupleComponentAccessOperation(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode exprLvalue,
            string memberAccessed)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidTupleComponentAccessOperation(location, exprLvalue, memberAccessed));
        }


        void ILSLSyntaxErrorListener.IfConditionNotValidType(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.IfConditionNotValidType(location, attemptedConditionExpression));
        }


        void ILSLSyntaxErrorListener.ElseIfConditionNotValidType(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ElseIfConditionNotValidType(location, attemptedConditionExpression));
        }


        void ILSLSyntaxErrorListener.DoLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.DoLoopConditionNotValidType(location, attemptedConditionExpression));
        }


        void ILSLSyntaxErrorListener.WhileLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.WhileLoopConditionNotValidType(location, attemptedConditionExpression));
        }


        void ILSLSyntaxErrorListener.ForLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ForLoopConditionNotValidType(location, attemptedConditionExpression));
        }


        void ILSLSyntaxErrorListener.ParameterTypeMismatchInFunctionCall(LSLSourceCodeRange location,
            int parameterNumberWithError,
            LSLFunctionSignature calledFunction, ILSLReadOnlyExprNode[] parameterExpressionsGiven)
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
            IReadOnlyGenericArray<LSLLibraryFunctionSignature> libraryFunctionSignatureOverloads)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.RedefinedStandardLibraryFunction(location, functionName,
                        libraryFunctionSignatureOverloads));
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


        void ILSLSyntaxErrorListener.DeadCodeAfterReturnPath(LSLSourceCodeRange location,
            LSLFunctionSignature inFunction, LSLDeadCodeSegment deadSegment)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.DeadCodeAfterReturnPath(location, inFunction, deadSegment));
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


        void ILSLSyntaxErrorListener.DefinedVariableInBracelessScope(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.DefinedVariableInBracelessScope(location));
        }


        void ILSLSyntaxErrorListener.IllegalStringCharacter(LSLSourceCodeRange location, LSLStringCharacterError err)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.IllegalStringCharacter(location, err));
        }


        void ILSLSyntaxErrorListener.InvalidStringEscapeCode(LSLSourceCodeRange location, LSLStringCharacterError err)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidStringEscapeCode(location, err));
        }


        void ILSLSyntaxErrorListener.CallToFunctionInStaticContext(LSLSourceCodeRange location, string functionName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.CallToFunctionInStaticContext(location, functionName));
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


        void ILSLSyntaxErrorListener.NoSuitableLibraryFunctionOverloadFound(LSLSourceCodeRange location,
            string functionName,
            IReadOnlyGenericArray<ILSLReadOnlyExprNode> givenParameterExpressions)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.NoSuitableLibraryFunctionOverloadFound(location, functionName,
                        givenParameterExpressions));
        }


        void ILSLSyntaxErrorListener.CallToOverloadedLibraryFunctionIsAmbiguous(LSLSourceCodeRange location,
            string functionName,
            IReadOnlyGenericArray<LSLLibraryFunctionSignature> ambiguousMatches,
            IReadOnlyGenericArray<ILSLReadOnlyExprNode> givenParameterExpressions)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.CallToOverloadedLibraryFunctionIsAmbiguous(location, functionName,
                        ambiguousMatches, givenParameterExpressions));
        }


        void ILSLSyntaxErrorListener.TupleComponentAccessOnLibraryConstant(LSLSourceCodeRange location,
            ILSLVariableNode libraryConstantReferenceNode,
            LSLLibraryConstantSignature libraryConstantSignature, string accessedMember)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.TupleComponentAccessOnLibraryConstant(location, libraryConstantReferenceNode,
                        libraryConstantSignature, accessedMember));
        }


        void ILSLSyntaxErrorListener.BinaryOperatorInStaticContext(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.BinaryOperatorInStaticContext(location));
        }


        void ILSLSyntaxErrorListener.ParenthesizedExpressionInStaticContext(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.ParenthesizedExpressionInStaticContext(location));
        }


        void ILSLSyntaxErrorListener.PostfixOperationInStaticContext(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.PostfixOperationInStaticContext(location));
        }


        void ILSLSyntaxErrorListener.InvalidPrefixOperationUsedInStaticContext(LSLSourceCodeRange location,
            LSLPrefixOperationType type)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.InvalidPrefixOperationUsedInStaticContext(location, type));
        }


        void ILSLSyntaxErrorListener.PrefixOperationOnGlobalVariableInStaticContext(LSLSourceCodeRange location,
            LSLPrefixOperationType type)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.PrefixOperationOnGlobalVariableInStaticContext(location, type));
        }


        void ILSLSyntaxErrorListener.ModifyingPrefixOperationOnNonVariable(LSLSourceCodeRange location,
            LSLPrefixOperationType type)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ModifyingPrefixOperationOnNonVariable(location, type));
        }


        void ILSLSyntaxErrorListener.NegateOperationOnVectorLiteralInStaticContext(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.NegateOperationOnVectorLiteralInStaticContext(location));
        }


        void ILSLSyntaxErrorListener.NegateOperationOnRotationLiteralInStaticContext(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.NegateOperationOnRotationLiteralInStaticContext(location));
        }


        void ILSLSyntaxErrorListener.CastExpressionInStaticContext(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.CastExpressionInStaticContext(location));
        }


        void ILSLSyntaxErrorListener.AssignmentToNonassignableExpression(LSLSourceCodeRange location,
            string assignmentOperatorUsed)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.AssignmentToNonassignableExpression(location, assignmentOperatorUsed));
        }


        void ILSLSyntaxErrorListener.PostfixOperationOnNonVariable(LSLSourceCodeRange location,
            LSLPostfixOperationType type)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.PostfixOperationOnNonVariable(location, type));
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


        void ILSLSyntaxWarningListener.UselessSemicolon(LSLSourceCodeRange location)
        {
            _warningActionQueue.Enqueue(location.StartIndex, () => SyntaxWarningListener.UselessSemicolon(location));
        }


        void ILSLSyntaxWarningListener.ExpressionStatementHasNoEffect(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode statementExpression)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.ExpressionStatementHasNoEffect(location, statementExpression));
        }


        void ILSLSyntaxWarningListener.ForLoopAfterthoughtHasNoEffect(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode expression, int expressionIndex,
            int expressionCountTotal)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.ForLoopAfterthoughtHasNoEffect(location, expression, expressionIndex,
                        expressionCountTotal));
        }


        void ILSLSyntaxWarningListener.ForLoopInitExpressionHasNoEffect(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode expression, int expressionIndex,
            int expressionCountTotal)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.ForLoopInitExpressionHasNoEffect(location, expression, expressionIndex,
                        expressionCountTotal));
        }


        void ILSLSyntaxWarningListener.RedundantCast(LSLSourceCodeRange location, LSLType castType)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.RedundantCast(location, castType));
        }


        void ILSLSyntaxWarningListener.FunctionNeverUsed(LSLSourceCodeRange location,
            ILSLFunctionDeclarationNode functionDeclarationNode)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.FunctionNeverUsed(location, functionDeclarationNode));
        }


        void ILSLSyntaxWarningListener.GlobalVariableNeverUsed(LSLSourceCodeRange location,
            ILSLVariableDeclarationNode variable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.GlobalVariableNeverUsed(location, variable));
        }


        void ILSLSyntaxWarningListener.LocalVariableNeverUsed(LSLSourceCodeRange location,
            ILSLVariableDeclarationNode variable,
            LSLFunctionSignature inFunction)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.LocalVariableNeverUsed(location, variable, inFunction));
        }


        void ILSLSyntaxWarningListener.LocalVariableNeverUsed(LSLSourceCodeRange location,
            ILSLVariableDeclarationNode variable,
            LSLEventSignature inEvent)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.LocalVariableNeverUsed(location, variable, inEvent));
        }


        void ILSLSyntaxWarningListener.FunctionParameterNeverUsed(LSLSourceCodeRange location,
            ILSLVariableDeclarationNode parameter,
            LSLFunctionSignature inFunction)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.FunctionParameterNeverUsed(location, parameter, inFunction));
        }


        void ILSLSyntaxWarningListener.ParameterHidesGlobalVariable(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature,
            ILSLParameterNode parameter, ILSLVariableDeclarationNode globalVariable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.ParameterHidesGlobalVariable(location, functionSignature, parameter,
                        globalVariable));
        }


        void ILSLSyntaxWarningListener.ParameterHidesGlobalVariable(LSLSourceCodeRange location,
            LSLEventSignature eventHandlerSignature,
            ILSLParameterNode parameter, ILSLVariableDeclarationNode globalVariable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.ParameterHidesGlobalVariable(location, eventHandlerSignature, parameter,
                        globalVariable));
        }


        void ILSLSyntaxWarningListener.LocalVariableHidesParameter(LSLSourceCodeRange location,
            LSLPreDefinedFunctionSignature functionSignature,
            ILSLVariableDeclarationNode localVariable, ILSLParameterNode parameter)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.LocalVariableHidesParameter(location, functionSignature, localVariable,
                        parameter));
        }


        void ILSLSyntaxWarningListener.LocalVariableHidesParameter(LSLSourceCodeRange location,
            LSLParsedEventHandlerSignature eventHandlerSignature,
            ILSLVariableDeclarationNode localVariable, ILSLParameterNode parameter)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.LocalVariableHidesParameter(location, eventHandlerSignature, localVariable,
                        parameter));
        }


        void ILSLSyntaxWarningListener.LocalVariableHidesGlobalVariable(LSLSourceCodeRange location,
            LSLPreDefinedFunctionSignature functionSignature,
            ILSLVariableDeclarationNode localVariable, ILSLVariableDeclarationNode globalVariable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.LocalVariableHidesGlobalVariable(location, functionSignature, localVariable,
                        globalVariable));
        }


        void ILSLSyntaxWarningListener.LocalVariableHidesGlobalVariable(LSLSourceCodeRange location,
            LSLParsedEventHandlerSignature eventHandlerSignature,
            ILSLVariableDeclarationNode localVariable, ILSLVariableDeclarationNode globalVariable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.LocalVariableHidesGlobalVariable(location, eventHandlerSignature,
                        localVariable, globalVariable));
        }


        void ILSLSyntaxWarningListener.UseOfDeprecatedLibraryFunction(LSLSourceCodeRange location,
            LSLLibraryFunctionSignature functionSignature)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.UseOfDeprecatedLibraryFunction(location, functionSignature));
        }


        void ILSLSyntaxWarningListener.UseOfDeprecatedLibraryConstant(LSLSourceCodeRange location,
            LSLLibraryConstantSignature constantSignature)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.UseOfDeprecatedLibraryConstant(location, constantSignature));
        }


        void ILSLSyntaxWarningListener.UseOfDeprecatedLibraryEventHandler(LSLSourceCodeRange location,
            LSLLibraryEventSignature eventSignature)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.UseOfDeprecatedLibraryEventHandler(location, eventSignature));
        }


        void ILSLSyntaxWarningListener.VariableRedeclaredInInnerScope(LSLSourceCodeRange location,
            LSLFunctionSignature currentFunctionBodySignature,
            ILSLVariableDeclarationNode newDeclarationNode, ILSLVariableDeclarationNode previousDeclarationNode)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.VariableRedeclaredInInnerScope(location, currentFunctionBodySignature,
                        newDeclarationNode, previousDeclarationNode));
        }


        void ILSLSyntaxWarningListener.VariableRedeclaredInInnerScope(LSLSourceCodeRange location,
            LSLEventSignature currentEventBodySignature,
            ILSLVariableDeclarationNode newDeclarationNode, ILSLVariableDeclarationNode previousDeclarationNode)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.VariableRedeclaredInInnerScope(location, currentEventBodySignature,
                        newDeclarationNode, previousDeclarationNode));
        }


        void ILSLSyntaxWarningListener.IntegerLiteralOverflow(LSLSourceCodeRange location, string literalText)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.IntegerLiteralOverflow(location, literalText));
        }


        void ILSLSyntaxWarningListener.HexLiteralOverflow(LSLSourceCodeRange location, string literalText)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.HexLiteralOverflow(location, literalText));
        }


        void ILSLSyntaxWarningListener.ReturnedValueFromEventHandler(LSLSourceCodeRange location,
            LSLEventSignature eventSignature, ILSLReadOnlyExprNode returnExpression)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.ReturnedValueFromEventHandler(location, eventSignature, returnExpression));
        }


        void ILSLSyntaxWarningListener.ConditionalExpressionIsConstant(LSLSourceCodeRange location,
            ILSLReadOnlyExprNode expression,
            LSLConditionalStatementType conditionalStatementType)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.ConditionalExpressionIsConstant(location, expression, conditionalStatementType));
        }


        /// <summary>
        ///     Invoke all the queued errors on <see cref="SyntaxErrorListener" />
        ///     so that the syntax errors are reported in order by their position in source code.
        /// </summary>
        public void InvokeQueuedErrors()
        {
            while (!_errorActionQueue.IsEmpty)
            {
                _errorActionQueue.DequeueValue()();
            }
        }


        /// <summary>
        ///     Invoke all the queued errors on <see cref="SyntaxWarningListener" />
        ///     so that the syntax warnings are reported in order by their position in source code.
        /// </summary>
        public void InvokeQueuedWarnings()
        {
            while (!_warningActionQueue.IsEmpty)
            {
                _warningActionQueue.DequeueValue()();
            }
        }


        /// <summary>
        ///     Invoke all the queued warnings and errors on <see cref="SyntaxErrorListener" /> and
        ///     <see cref="SyntaxWarningListener" />
        ///     so that the syntax warnings and errors are reported in order by their position in source code.
        /// </summary>
        public void InvokeQueuedActions()
        {
            InvokeQueuedErrors();

            InvokeQueuedWarnings();
        }
    }
}