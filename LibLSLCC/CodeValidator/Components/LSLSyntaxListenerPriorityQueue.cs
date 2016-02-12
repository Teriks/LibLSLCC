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
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;
using LibLSLCC.LibraryData;

#endregion

namespace LibLSLCC.CodeValidator.Components
{

    /// <summary>
    /// A proxy object for <see cref="ILSLSyntaxErrorListener"/> and <see cref="ILSLSyntaxWarningListener"/> that queue's/sorts syntax warnings/errors by their starting index in source code.
    /// </summary>
    public sealed class LSLSyntaxListenerPriorityQueue : ILSLSyntaxErrorListener, ILSLSyntaxWarningListener
    {
        private readonly PriorityQueue<int, Action> _errorActionQueue = new PriorityQueue<int, Action>();
        private readonly PriorityQueue<int, Action> _warningActionQueue = new PriorityQueue<int, Action>();




        /// <summary>
        /// Gets the number of syntax warnings that have been queued.
        /// </summary>
        /// <value>
        /// The number of syntax warnings queued.
        /// </value>
        public int NumberOfSyntaxWarnings
        {
            get { return _warningActionQueue.Count; }
        }


        /// <summary>
        /// Gets the number of syntax errors that have been queued.
        /// </summary>
        /// <value>
        /// The number of syntax errors queued.
        /// </value>
        public int NumberOfSyntaxErrors
        {
            get { return _errorActionQueue.Count; }
        }


        /// <summary>
        /// Construct a <see cref="LSLSyntaxListenerPriorityQueue"/> by wrapping another <see cref="ILSLSyntaxErrorListener"/> and <see cref="ILSLSyntaxWarningListener"/>
        /// </summary>
        /// <param name="invokeErrorsOn">Syntax errors will be priority queued by their index in the source code and invoked on this object.</param>
        /// <param name="invokeWarningsOn">Syntax warnings will be priority queued by their index in the source code and invoked on this object.</param>
        public LSLSyntaxListenerPriorityQueue(
            ILSLSyntaxErrorListener invokeErrorsOn,
            ILSLSyntaxWarningListener invokeWarningsOn)
        {
            SyntaxErrorListener = invokeErrorsOn;
            SyntaxWarningListener = invokeWarningsOn;
        }

        /// <summary>
        /// The syntax error listener that this object delegates to.
        /// </summary>
        public ILSLSyntaxErrorListener SyntaxErrorListener { get; private set; }


        /// <summary>
        /// The syntax warning listener that this object delegates to.
        /// </summary>
        public ILSLSyntaxWarningListener SyntaxWarningListener { get; private set; }


        /// <summary>
        /// A parsing error at the grammar level has occurred somewhere in the source code.
        /// </summary>
        /// <param name="line">The line on which the error occurred.</param>
        /// <param name="column">The character column at which the error occurred.</param>
        /// <param name="offendingTokenText">The text representing the offending token.</param>
        /// <param name="message">The parsing error messaged passed along from the parsing back end.</param>
        /// <param name="offendingTokenRange">The source code range of the offending symbol.</param>
        void ILSLSyntaxErrorListener.GrammarLevelParserSyntaxError(int line, int column, LSLSourceCodeRange offendingTokenRange, string offendingTokenText, string message)
        {
            _errorActionQueue.Enqueue(line,
                () => SyntaxErrorListener.GrammarLevelParserSyntaxError(line, column, offendingTokenRange, offendingTokenText, message));
        }


        /// <summary>
        /// A reference to an undefined variable was encountered.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="name">Name of the variable attempting to be referenced.</param>
        void ILSLSyntaxErrorListener.UndefinedVariableReference(LSLSourceCodeRange location, string name)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.UndefinedVariableReference(location, name));
        }

        /// <summary>
        /// A parameter name for a function or event handler was used more than once.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="type">The type of the new parameter who's name was duplicate.</param>
        /// <param name="name">The name of the new parameter, which was duplicate.</param>
        void ILSLSyntaxErrorListener.ParameterNameRedefined(LSLSourceCodeRange location, LSLType type, string name)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ParameterNameRedefined(location, type, name));
        }

        /// <summary>
        /// A binary operation was encountered that had incorrect expression types on either or both sides.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="left">The left expression.</param>
        /// <param name="operation">The binary operation that was attempted on the two expressions.</param>
        /// <param name="right">The right expression.</param>
        void ILSLSyntaxErrorListener.InvalidBinaryOperation(LSLSourceCodeRange location, ILSLExprNode left,
            string operation,
            ILSLExprNode right)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidBinaryOperation(location, left, operation, right));
        }

        /// <summary>
        /// A prefix operation was attempted on an invalid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="operation">The prefix operation that was attempted on the expression.</param>
        /// <param name="right">The expression the prefix operation was used on.</param>
        void ILSLSyntaxErrorListener.InvalidPrefixOperation(LSLSourceCodeRange location, string operation,
            ILSLExprNode right)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidPrefixOperation(location, operation, right));
        }

        /// <summary>
        /// A postfix operation was attempted on an invalid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="operation">The postfix operation that was attempted on the expression.</param>
        /// <param name="left">The expression the postfix operation was used on.</param>
        void ILSLSyntaxErrorListener.InvalidPostfixOperation(LSLSourceCodeRange location, ILSLExprNode left,
            string operation)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidPostfixOperation(location, left, operation));
        }

        /// <summary>
        /// An invalid cast was preformed on some expression
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="castTo">The type that the cast operation attempted to cast the expression to.</param>
        /// <param name="fromExpression">The expression that the cast was attempted on.</param>
        void ILSLSyntaxErrorListener.InvalidCastOperation(LSLSourceCodeRange location, LSLType castTo,
            ILSLExprNode fromExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidCastOperation(location, castTo, fromExpression));
        }

        /// <summary>
        /// A variable declaration was initialized with an invalid type on the right.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="variableType">The actual type of the variable attempting to be initialized.</param>
        /// <param name="assignedExpression">The invalid expression that was assigned in the variable declaration.</param>
        void ILSLSyntaxErrorListener.TypeMismatchInVariableDeclaration(LSLSourceCodeRange location, LSLType variableType,
            ILSLExprNode assignedExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.TypeMismatchInVariableDeclaration(location, variableType, assignedExpression));
        }

        /// <summary>
        /// A variable was redefined.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="variableType">The type of the variable that was considered a re-definition.</param>
        /// <param name="variableName">The name of the variable that was considered a re-definition.</param>
        void ILSLSyntaxErrorListener.VariableRedefined(LSLSourceCodeRange location, LSLType variableType,
            string variableName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.VariableRedefined(location, variableType, variableName));
        }

        /// <summary>
        /// A vector literal contained an invalid expression in its initializer list.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="component">The vector component of the initializer that contained the invalid expression.</param>
        /// <param name="invalidExpressionContent">The expression that was considered to be invalid vector initializer content.</param>
        void ILSLSyntaxErrorListener.InvalidVectorContent(LSLSourceCodeRange location, LSLVectorComponent component,
            ILSLExprNode invalidExpressionContent)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidVectorContent(location, component, invalidExpressionContent));
        }

        /// <summary>
        /// A list literal contained an invalid expression in its initializer list.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="index">The index in the initializer list that contained the invalid expression.</param>
        /// <param name="invalidExpressionContent">The expression that was considered to be invalid list initializer content.</param>
        void ILSLSyntaxErrorListener.InvalidListContent(LSLSourceCodeRange location, int index,
            ILSLExprNode invalidExpressionContent)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidListContent(location, index, invalidExpressionContent));
        }

        /// <summary>
        /// A rotation literal contained an invalid expression in its initializer list.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="component">The rotation component of the initializer that contained the invalid expression.</param>
        /// <param name="invalidExpressionContent">The expression that was considered to be invalid rotation initializer content.</param>
        void ILSLSyntaxErrorListener.InvalidRotationContent(LSLSourceCodeRange location, LSLRotationComponent component,
            ILSLExprNode invalidExpressionContent)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidRotationContent(location, component, invalidExpressionContent));
        }

        /// <summary>
        /// Attempted to return a value from a function with no return type. (A void function)
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The signature of the function the return was attempted from.</param>
        /// <param name="attemptedReturnExpression">The expression that was attempted to be returned.</param>
        void ILSLSyntaxErrorListener.ReturnedValueFromVoidFunction(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature,
            ILSLExprNode attemptedReturnExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.ReturnedValueFromVoidFunction(location, functionSignature,
                        attemptedReturnExpression));
        }

        /// <summary>
        /// Attempted to return an invalid expression type from a function with a defined return type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The signature of the function the return was attempted from.</param>
        /// <param name="attemptedReturnExpression">The expression that was attempted to be returned.</param>
        void ILSLSyntaxErrorListener.TypeMismatchInReturnValue(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature,
            ILSLExprNode attemptedReturnExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.TypeMismatchInReturnValue(location, functionSignature,
                        attemptedReturnExpression));
        }

        /// <summary>
        /// An empty return statement was encountered in a non void function. (A function with a defined return type)
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The signature of the function the return was attempted from.</param>
        void ILSLSyntaxErrorListener.ReturnedVoidFromANonVoidFunction(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ReturnedVoidFromANonVoidFunction(location, functionSignature));
        }

        /// <summary>
        /// A jump statement to an undefined label name was encountered.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="labelName">The name of the label that was given to the jump statement.</param>
        void ILSLSyntaxErrorListener.JumpToUndefinedLabel(LSLSourceCodeRange location, string labelName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.JumpToUndefinedLabel(location, labelName));
        }

        /// <summary>
        /// A call to an undefined function was encountered.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the function used in the call.</param>
        void ILSLSyntaxErrorListener.CallToUndefinedFunction(LSLSourceCodeRange location, string functionName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.CallToUndefinedFunction(location, functionName));
        }

        /// <summary>
        /// A user defined or library function was called with the wrong number of parameters.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The function signature of the defined function attempting to be called.</param>
        /// <param name="parameterExpressionsGiven">The expressions given to the function call.</param>
        void ILSLSyntaxErrorListener.ImproperParameterCountInFunctionCall(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature,
            ILSLExprNode[] parameterExpressionsGiven)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.ImproperParameterCountInFunctionCall(location, functionSignature,
                        parameterExpressionsGiven));
        }



        /// <summary>
        /// A user defined function was re-defined.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="previouslyDefinedSignature">The signature of the previously defined function that is considered a duplicated to the new definition.</param>
        void ILSLSyntaxErrorListener.RedefinedFunction(LSLSourceCodeRange location,
            LSLFunctionSignature previouslyDefinedSignature)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.RedefinedFunction(location, previouslyDefinedSignature));
        }

        /// <summary>
        /// A code label was considered a redefinition of an already defined code label, given the scope of the new definition.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="labelName">The name of the label being redefined.</param>
        void ILSLSyntaxErrorListener.RedefinedLabel(LSLSourceCodeRange location, string labelName)
        {
            _errorActionQueue.Enqueue(location.StartIndex, () => SyntaxErrorListener.RedefinedLabel(location, labelName));
        }

        /// <summary>
        /// A '.' member access was attempted on an invalid variable type, or the variable type did not contain the given component.
        /// Valid component names for vectors are:  x,y and z
        /// Valid component names for rotations are:  x,y,z and s
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="exprLvalue">The variable expression on the left side of the dot operator.</param>
        /// <param name="memberAccessed">The member/component name on the right side of the dot operator.</param>
        void ILSLSyntaxErrorListener.InvalidTupleComponentAccessorOperation(LSLSourceCodeRange location,
            ILSLExprNode exprLvalue,
            string memberAccessed)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidTupleComponentAccessorOperation(location, exprLvalue, memberAccessed));
        }

        /// <summary>
        /// The return type of the expression present in an if statements condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the if statement.</param>
        void ILSLSyntaxErrorListener.IfConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.IfConditionNotValidType(location, attemptedConditionExpression));
        }

        /// <summary>
        /// The return type of the expression present in an else-if statements condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the else-if statement.</param>
        void ILSLSyntaxErrorListener.ElseIfConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ElseIfConditionNotValidType(location, attemptedConditionExpression));
        }

        /// <summary>
        /// The return type of the expression present in a do-loops condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the do-loop.</param>
        void ILSLSyntaxErrorListener.DoLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.DoLoopConditionNotValidType(location, attemptedConditionExpression));
        }

        /// <summary>
        /// The return type of the expression present in a while-loops condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the while-loop.</param>
        void ILSLSyntaxErrorListener.WhileLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.WhileLoopConditionNotValidType(location, attemptedConditionExpression));
        }

        /// <summary>
        /// The return type of the expression present in a for-loops condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the for-loop.</param>
        void ILSLSyntaxErrorListener.ForLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ForLoopConditionNotValidType(location, attemptedConditionExpression));
        }

        /// <summary>
        /// A parameter type mismatch was encountered when trying to call a user defined or library function.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="parameterNumberWithError">The index of the parameter with the type mismatch. (Zero based)</param>
        /// <param name="calledFunction">The defined/library function that was attempting to be called.</param>
        /// <param name="parameterExpressionsGiven">The parameter expressions given for the function call.</param>
        void ILSLSyntaxErrorListener.ParameterTypeMismatchInFunctionCall(LSLSourceCodeRange location,
            int parameterNumberWithError,
            LSLFunctionSignature calledFunction, ILSLExprNode[] parameterExpressionsGiven)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.ParameterTypeMismatchInFunctionCall(location, parameterNumberWithError,
                        calledFunction, parameterExpressionsGiven));
        }

        /// <summary>
        /// A state name was re-defined.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="stateName">The name of the state being redefined.</param>
        void ILSLSyntaxErrorListener.RedefinedStateName(LSLSourceCodeRange location, string stateName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.RedefinedStateName(location, stateName));
        }

        /// <summary>
        /// An event handler which was not defined in the library data provider was used in the program.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="givenEventHandlerSignature">The signature of the event handler attempting to be used.</param>
        void ILSLSyntaxErrorListener.UnknownEventHandlerDeclared(LSLSourceCodeRange location,
            LSLEventSignature givenEventHandlerSignature)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.UnknownEventHandlerDeclared(location, givenEventHandlerSignature));
        }

        /// <summary>
        /// An event handler was used in the program which was defined in the library data provider, but the given call signature in the program
        /// was incorrect.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="givenEventHandlerSignature">The invalid signature used for the event handler in the source code.</param>
        /// <param name="correctEventHandlerSignature">The actual valid signature for the event handler from the library data provider.</param>
        void ILSLSyntaxErrorListener.IncorrectEventHandlerSignature(LSLSourceCodeRange location,
            LSLEventSignature givenEventHandlerSignature,
            LSLLibraryEventSignature correctEventHandlerSignature)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.IncorrectEventHandlerSignature(location, givenEventHandlerSignature,
                        correctEventHandlerSignature));
        }

        /// <summary>
        /// A standard library constant defined in the library data provider was redefined in source code as a global or local variable.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="redefinitionType">The type used in the re-definition.</param>
        /// <param name="originalSignature">The original signature of the constant taken from the library data provider.</param>
        void ILSLSyntaxErrorListener.RedefinedStandardLibraryConstant(LSLSourceCodeRange location,
            LSLType redefinitionType,
            LSLLibraryConstantSignature originalSignature)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.RedefinedStandardLibraryConstant(location, redefinitionType,
                        originalSignature));
        }

        /// <summary>
        /// A library function that exist in the library data provider was redefined by the user as a user defined function.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the library function the user attempted to redefine.</param>
        /// <param name="libraryFunctionSignatureOverloads">All of the overloads for the library function, there may only be one if no overloads actually exist.</param>
        void ILSLSyntaxErrorListener.RedefinedStandardLibraryFunction(LSLSourceCodeRange location, string functionName,
            IReadOnlyGenericArray<LSLLibraryFunctionSignature> libraryFunctionSignatureOverloads)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.RedefinedStandardLibraryFunction(location, functionName,
                        libraryFunctionSignatureOverloads));
        }

        /// <summary>
        /// A state change statement was encountered that attempted to change states to an undefined state name.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="stateName">The undefined state name referenced.</param>
        void ILSLSyntaxErrorListener.ChangeToUndefinedState(LSLSourceCodeRange location, string stateName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ChangeToUndefinedState(location, stateName));
        }

        /// <summary>
        /// An attempt to modify a library constant defined in the library data provider was made.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="constantName">The name of the constant the user attempted to modified.</param>
        void ILSLSyntaxErrorListener.ModifiedLibraryConstant(LSLSourceCodeRange location, string constantName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.ModifiedLibraryConstant(location, constantName));
        }

        /// <summary>
        /// The user attempted to use 'default' for a user defined state name.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        void ILSLSyntaxErrorListener.RedefinedDefaultState(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex, () => SyntaxErrorListener.RedefinedDefaultState(location));
        }

        /// TODO check necessity
        /// <summary>
        /// The given expression was not valid as a statement in a code scope.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        void ILSLSyntaxErrorListener.InvalidStatementExpression(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidStatementExpression(location));
        }

        /// <summary>
        /// Dead code after a return path was detected in a function with a non-void return type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="inFunction">The signature of the function the dead code was detected in.</param>
        /// <param name="deadSegment">An object describing the location an span of the dead code segment.</param>
        void ILSLSyntaxErrorListener.DeadCodeAfterReturnPathDetected(LSLSourceCodeRange location,
            LSLFunctionSignature inFunction, LSLDeadCodeSegment deadSegment)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.DeadCodeAfterReturnPathDetected(location, inFunction, deadSegment));
        }

        /// <summary>
        /// A function with a non-void return type lacks a necessary return statement.  
        /// Not all code paths return a value.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="inFunction">The signature of the function in question.</param>
        void ILSLSyntaxErrorListener.NotAllCodePathsReturnAValue(LSLSourceCodeRange location,
            LSLFunctionSignature inFunction)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.NotAllCodePathsReturnAValue(location, inFunction));
        }

        /// <summary>
        /// A code state does not declare the use of any event handlers at all. (This is not allowed)
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="stateName">The name of the state in which this error occurred.</param>
        void ILSLSyntaxErrorListener.StateHasNoEventHandlers(LSLSourceCodeRange location, string stateName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.StateHasNoEventHandlers(location, stateName));
        }

        /// <summary>
        /// A conditional expression is missing from an IF, ELSE IF, or WHILE/DO-WHILE statement;
        /// FOR loops can have a missing condition expression, but other control statements cannot.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="statementType">The type of branch/loop statement the condition was missing from.</param>
        void ILSLSyntaxErrorListener.MissingConditionalExpression(LSLSourceCodeRange location,
            LSLConditionalStatementType statementType)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.MissingConditionalExpression(location, statementType));
        }

        /// <summary>
        /// Attempted to define a variable inside of a single statement block.  Such as inside of an IF statement which does not
        /// use braces.  This applies to other statements that can use brace-less single statement blocks as well.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        void ILSLSyntaxErrorListener.DefinedVariableInNonScopeBlock(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.DefinedVariableInNonScopeBlock(location));
        }

        /// <summary>
        /// An illegal character was found inside of a string literal according to the current ILSLStringPreProccessor instance.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="err">The generated character error object from the ILSLStringPreProccessor instance.</param>
        void ILSLSyntaxErrorListener.IllegalStringCharacter(LSLSourceCodeRange location, LSLStringCharacterError err)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.IllegalStringCharacter(location, err));
        }

        /// <summary>
        /// An invalid escape sequence was found inside of a string literal according to the current <see cref="ILSLStringPreProcessor"/> instance.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="err">The generated character error object from the ILSLStringPreProccessor instance.</param>
        void ILSLSyntaxErrorListener.InvalidStringEscapeCode(LSLSourceCodeRange location, LSLStringCharacterError err)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.InvalidStringEscapeCode(location, err));
        }

        /// <summary>
        /// A call to function was attempted in a static context.  For example, inside of a global variables declaration expression.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        void ILSLSyntaxErrorListener.CallToFunctionInStaticContext(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.CallToFunctionInStaticContext(location));
        }

        /// <summary>
        /// A library defined event handler was used more than once in the same state.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="eventHandlerName">The name of the event handler which was used more than once.</param>
        /// <param name="stateName">The name of the code state in which the error occurred.</param>
        void ILSLSyntaxErrorListener.RedefinedEventHandler(LSLSourceCodeRange location, string eventHandlerName,
            string stateName)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.RedefinedEventHandler(location, eventHandlerName, stateName));
        }

        /// <summary>
        /// The default code state was missing from the program.
        /// </summary>
        void ILSLSyntaxErrorListener.MissingDefaultState()
        {
            _errorActionQueue.Enqueue(0, () => SyntaxErrorListener.MissingDefaultState());
        }

        /// <summary>
        /// An overload could not be resolved for an attempted call to an overloaded library function.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the overloaded library function that the user attempted to call.</param>
        /// <param name="givenParameterExpressions">The parameter expressions the user attempted to pass to the overloaded library function.</param>
        void ILSLSyntaxErrorListener.NoSuitableLibraryFunctionOverloadFound(LSLSourceCodeRange location, string functionName,
            IReadOnlyGenericArray<ILSLExprNode> givenParameterExpressions)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.NoSuitableLibraryFunctionOverloadFound(location, functionName, givenParameterExpressions));
        }


        /// <summary>
        /// A call to an overloaded library function matches up with one or more overloads.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the overloaded library function that the user attempted to call.</param>
        /// <param name="ambigiousMatches">All of the function overloads the call to the library function matched up with.</param>
        /// <param name="givenParameterExpressions">The parameter expressions the user attempted to pass to the overloaded library function.</param>
        void ILSLSyntaxErrorListener.CallToOverloadedLibraryFunctionIsAmbigious(LSLSourceCodeRange location, string functionName,
            IReadOnlyGenericArray<LSLLibraryFunctionSignature> ambigiousMatches, IReadOnlyGenericArray<ILSLExprNode> givenParameterExpressions)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.CallToOverloadedLibraryFunctionIsAmbigious(location, functionName, ambigiousMatches, givenParameterExpressions));
        }

        /// <summary>
        /// The dot operator used to access the tuple component members of vectors and rotations was used on a library constant.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="libraryConstantReferenceNode">The variable reference node on the left side of the dot operator.</param>
        /// <param name="libraryConstantSignature">The library constant signature that was referenced, retrieved from the library data provider.</param>
        /// <param name="accessedMember">The member the user attempted to access.</param>
        void ILSLSyntaxErrorListener.TupleAccessorOnLibraryConstant(LSLSourceCodeRange location, ILSLVariableNode libraryConstantReferenceNode,
            LSLLibraryConstantSignature libraryConstantSignature, string accessedMember)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.TupleAccessorOnLibraryConstant(location, libraryConstantReferenceNode, libraryConstantSignature, accessedMember));
        }

        void ILSLSyntaxErrorListener.BinaryOperatorUsedInStaticContext(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.BinaryOperatorUsedInStaticContext(location));
        }

        void ILSLSyntaxErrorListener.ParenthesizedExpressionUsedInStaticContext(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.ParenthesizedExpressionUsedInStaticContext(location));
        }

        /// <summary>
        /// A postfix expression was used in a static context (a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        void ILSLSyntaxErrorListener.PostfixOperationUsedInStaticContext(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.PostfixOperationUsedInStaticContext(location));
        }

        /// <summary>
        /// A prefix expression was used in a static context (a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        /// <param name="type">The prefix operation type.</param>
        void ILSLSyntaxErrorListener.InvalidPrefixOperationUsedInStaticContext(LSLSourceCodeRange location, LSLPrefixOperationType type)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.InvalidPrefixOperationUsedInStaticContext(location, type));
        }

        void ILSLSyntaxErrorListener.InvalidPrefixOperationUsedGlobalVariableInStaticContext(LSLSourceCodeRange location, LSLPrefixOperationType type)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxErrorListener.InvalidPrefixOperationUsedGlobalVariableInStaticContext(location, type));
        }

        void ILSLSyntaxErrorListener.ModifyingPrefixOperationOnNonVariable(LSLSourceCodeRange location, LSLPrefixOperationType type)
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

        void ILSLSyntaxErrorListener.CastExpressionUsedInStaticContext(LSLSourceCodeRange location)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.CastExpressionUsedInStaticContext(location));
        }


        void ILSLSyntaxErrorListener.AssignmentToUnassignableExpression(LSLSourceCodeRange location, string assignmentOperatorUsed)
        {
            _errorActionQueue.Enqueue(location.StartIndex,
                () => SyntaxErrorListener.AssignmentToUnassignableExpression(location, assignmentOperatorUsed));
        }


        void ILSLSyntaxErrorListener.PostfixOperationOnNonVariable(LSLSourceCodeRange location, LSLPostfixOperationType type)
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

        void ILSLSyntaxWarningListener.ForLoopInitExpressionHasNoEffect(LSLSourceCodeRange location, int expressionIndex,
            int expressionCountTotal)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.ForLoopInitExpressionHasNoEffect(location, expressionIndex,
                        expressionCountTotal));
        }

        void ILSLSyntaxWarningListener.RedundantCast(LSLSourceCodeRange location, LSLType castType)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.RedundantCast(location, castType));
        }

        void ILSLSyntaxWarningListener.FunctionNeverUsed(LSLSourceCodeRange location, ILSLFunctionDeclarationNode functionDeclarationNode)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.FunctionNeverUsed(location, functionDeclarationNode));
        }

        void ILSLSyntaxWarningListener.GlobalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.GlobalVariableNeverUsed(location, variable));
        }

        void ILSLSyntaxWarningListener.LocalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable,
            LSLFunctionSignature inFunction)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.LocalVariableNeverUsed(location, variable, inFunction));
        }

        void ILSLSyntaxWarningListener.LocalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable,
            LSLEventSignature inEvent)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.LocalVariableNeverUsed(location, variable, inEvent));
        }

        void ILSLSyntaxWarningListener.FunctionParameterNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode parameter,
            LSLFunctionSignature inFunction)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () => SyntaxWarningListener.FunctionParameterNeverUsed(location, parameter, inFunction));
        }

        void ILSLSyntaxWarningListener.ParameterHidesGlobalVariable(LSLSourceCodeRange location, LSLFunctionSignature functionSignature,
            LSLParameterNode parameter, LSLVariableDeclarationNode globalVariable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.ParameterHidesGlobalVariable(location, functionSignature, parameter,
                        globalVariable));
        }

        void ILSLSyntaxWarningListener.ParameterHidesGlobalVariable(LSLSourceCodeRange location, LSLEventSignature eventHandlerSignature,
            LSLParameterNode parameter, LSLVariableDeclarationNode globalVariable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.ParameterHidesGlobalVariable(location, eventHandlerSignature, parameter,
                        globalVariable));
        }

        void ILSLSyntaxWarningListener.LocalVariableHidesParameter(LSLSourceCodeRange location,
            LSLPreDefinedFunctionSignature functionSignature,
            LSLVariableDeclarationNode localVariable, LSLParameterNode parameter)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.LocalVariableHidesParameter(location, functionSignature, localVariable,
                        parameter));
        }

        void ILSLSyntaxWarningListener.LocalVariableHidesParameter(LSLSourceCodeRange location,
            LSLParsedEventHandlerSignature eventHandlerSignature,
            LSLVariableDeclarationNode localVariable, LSLParameterNode parameter)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.LocalVariableHidesParameter(location, eventHandlerSignature, localVariable,
                        parameter));
        }

        void ILSLSyntaxWarningListener.LocalVariableHidesGlobalVariable(LSLSourceCodeRange location,
            LSLPreDefinedFunctionSignature functionSignature,
            LSLVariableDeclarationNode localVariable, LSLVariableDeclarationNode globalVariable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.LocalVariableHidesGlobalVariable(location, functionSignature, localVariable,
                        globalVariable));
        }

        void ILSLSyntaxWarningListener.LocalVariableHidesGlobalVariable(LSLSourceCodeRange location,
            LSLParsedEventHandlerSignature eventHandlerSignature,
            LSLVariableDeclarationNode localVariable, LSLVariableDeclarationNode globalVariable)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.LocalVariableHidesGlobalVariable(location, eventHandlerSignature,
                        localVariable, globalVariable));
        }

        void ILSLSyntaxWarningListener.UseOfDeprecatedLibraryFunction(LSLSourceCodeRange location, LSLLibraryFunctionSignature functionSignature)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.UseOfDeprecatedLibraryFunction(location, functionSignature));
        }

        void ILSLSyntaxWarningListener.UseOfDeprecatedLibraryConstant(LSLSourceCodeRange location, LSLLibraryConstantSignature constantSignature)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.UseOfDeprecatedLibraryConstant(location, constantSignature));
        }

        void ILSLSyntaxWarningListener.UseOfDeprecatedLibraryEventHandler(LSLSourceCodeRange location, LSLLibraryEventSignature eventSignature)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.UseOfDeprecatedLibraryEventHandler(location, eventSignature));
        }

        void ILSLSyntaxWarningListener.VariableRedeclaredInInnerScope(LSLSourceCodeRange location, LSLFunctionSignature currentFunctionBodySignature,
            LSLVariableDeclarationNode newDeclarationNode, LSLVariableDeclarationNode previousDeclarationNode)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.VariableRedeclaredInInnerScope(location, currentFunctionBodySignature,
                    newDeclarationNode, previousDeclarationNode));
        }


        void ILSLSyntaxWarningListener.VariableRedeclaredInInnerScope(LSLSourceCodeRange location, LSLEventSignature currentEventBodySignature,
            LSLVariableDeclarationNode newDeclarationNode, LSLVariableDeclarationNode previousDeclarationNode)
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


        void ILSLSyntaxWarningListener.ReturnedValueFromEventHandler(LSLSourceCodeRange location, LSLEventSignature eventSignature, ILSLExprNode returnExpression)
        {
            _warningActionQueue.Enqueue(location.StartIndex,
                () =>
                    SyntaxWarningListener.ReturnedValueFromEventHandler(location, eventSignature, returnExpression));
        }


        void ILSLSyntaxWarningListener.ConditionalExpressionIsConstant(LSLSourceCodeRange expression,
            LSLConditionalStatementType conditionalStatementType)
        {
            _warningActionQueue.Enqueue(expression.StartIndex,
                () => SyntaxWarningListener.ConditionalExpressionIsConstant(expression, conditionalStatementType));
        }


        /// <summary>
        /// Invoke all the queued errors on <see cref="SyntaxErrorListener"/>
        /// so that the syntax errors are reported in order by their position in source code.
        /// </summary>
        public void InvokeQueuedErrors()
        {
            while (!_errorActionQueue.IsEmpty)
            {
                _errorActionQueue.DequeueValue()();
            }
        }


        /// <summary>
        /// Invoke all the queued errors on <see cref="SyntaxWarningListener"/>
        /// so that the syntax warnings are reported in order by their position in source code.
        /// </summary>
        public void InvokeQueuedWarnings()
        {
            while (!_warningActionQueue.IsEmpty)
            {
                _warningActionQueue.DequeueValue()();
            }
        }

        /// <summary>
        /// Invoke all the queued warnings and errors on <see cref="SyntaxErrorListener"/> and <see cref="SyntaxWarningListener"/>
        /// so that the syntax warnings and errors are reported in order by their position in source code.
        /// </summary>
        public void InvokeQueuedActions()
        {
            InvokeQueuedErrors();

            InvokeQueuedWarnings();
        }


    }
}