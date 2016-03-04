#region FileInfo

// 
// File: ILSLSyntaxErrorListener.cs
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

using LibLSLCC.Collections;
using LibLSLCC.LibraryData;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Interface for a syntax error listener, this is called into by the code validator
    ///     when syntax errors occur.
    /// </summary>
    public interface ILSLSyntaxErrorListener
    {
        /// <summary>
        ///     A parsing error at the grammar level has occurred somewhere in the source code.
        /// </summary>
        /// <param name="line">The line on which the error occurred.</param>
        /// <param name="column">The character column at which the error occurred.</param>
        /// <param name="offendingTokenText">The text representing the offending token.</param>
        /// <param name="message">The parsing error messaged passed along from the parsing back end.</param>
        /// <param name="offendingTokenRange">The source code range of the offending symbol.</param>
        void GrammarLevelParserSyntaxError(int line, int column, LSLSourceCodeRange offendingTokenRange,
            string offendingTokenText, string message);


        /// <summary>
        ///     A reference to an undefined variable was encountered.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="name">Name of the variable attempting to be referenced.</param>
        void UndefinedVariableReference(LSLSourceCodeRange location, string name);


        /// <summary>
        ///     A parameter name for a function or event handler was used more than once.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="parameterListType">The type of parameter list the duplicate parameter name was found in.</param>
        /// <param name="type">The type of the new parameter who's name was duplicate.</param>
        /// <param name="name">The name of the new parameter, which was duplicate.</param>
        void ParameterNameRedefined(LSLSourceCodeRange location, LSLParameterListType parameterListType, LSLType type,
            string name);


        /// <summary>
        ///     A binary operation was encountered that had expressions with incorrect return types on either or both sides.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="left">The left expression.</param>
        /// <param name="operation">The binary operation that was attempted on the two expressions.</param>
        /// <param name="right">The right expression.</param>
        void InvalidBinaryOperation(LSLSourceCodeRange location, ILSLReadOnlyExprNode left, string operation, ILSLReadOnlyExprNode right);


        /// <summary>
        ///     A prefix operation was attempted on expression with an invalid return type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="operation">The prefix operation that was attempted on the expression.</param>
        /// <param name="right">The expression the prefix operation was used on.</param>
        void InvalidPrefixOperation(LSLSourceCodeRange location, string operation, ILSLReadOnlyExprNode right);


        /// <summary>
        ///     A postfix operation was attempted on expression with an invalid return type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="operation">The postfix operation that was attempted on the expression.</param>
        /// <param name="left">The expression the postfix operation was used on.</param>
        void InvalidPostfixOperation(LSLSourceCodeRange location, ILSLReadOnlyExprNode left, string operation);


        /// <summary>
        ///     An invalid cast was preformed on an expression.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="castTo">The type that the cast operation attempted to cast the expression to.</param>
        /// <param name="fromExpression">The expression that the cast was attempted on.</param>
        void InvalidCastOperation(LSLSourceCodeRange location, LSLType castTo, ILSLReadOnlyExprNode fromExpression);


        /// <summary>
        ///     A variable declaration was initialized with an invalid type on the right.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="variableType">The actual type of the variable attempting to be initialized.</param>
        /// <param name="assignedExpression">The invalid expression that was assigned in the variable declaration.</param>
        void TypeMismatchInVariableDeclaration(LSLSourceCodeRange location, LSLType variableType,
            ILSLReadOnlyExprNode assignedExpression);


        /// <summary>
        ///     A variable was redefined.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="variableType">The type of the variable that was considered a re-definition.</param>
        /// <param name="variableName">The name of the variable that was considered a re-definition.</param>
        void VariableRedefined(LSLSourceCodeRange location, LSLType variableType, string variableName);


        /// <summary>
        ///     A vector literal contained an invalid expression in its initializer list.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="component">The vector component of the initializer that contained the invalid expression.</param>
        /// <param name="invalidExpressionContent">The expression that was considered to be invalid vector initializer content.</param>
        void InvalidVectorContent(LSLSourceCodeRange location, LSLVectorComponent component,
            ILSLReadOnlyExprNode invalidExpressionContent);


        /// <summary>
        ///     A list literal contained an invalid expression in its initializer list.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="index">The index in the initializer list that contained the invalid expression.</param>
        /// <param name="invalidExpressionContent">The expression that was considered to be invalid list initializer content.</param>
        void InvalidListContent(LSLSourceCodeRange location, int index, ILSLReadOnlyExprNode invalidExpressionContent);


        /// <summary>
        ///     A rotation literal contained an invalid expression in its initializer list.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="component">The rotation component of the initializer that contained the invalid expression.</param>
        /// <param name="invalidExpressionContent">The expression that was considered to be invalid rotation initializer content.</param>
        void InvalidRotationContent(LSLSourceCodeRange location, LSLRotationComponent component,
            ILSLReadOnlyExprNode invalidExpressionContent);


        /// <summary>
        ///     Attempted to return a value from a function with no return type. (A void function)
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The signature of the function the return was attempted from.</param>
        /// <param name="attemptedReturnExpression">The expression that was attempted to be returned.</param>
        void ReturnedValueFromVoidFunction(LSLSourceCodeRange location, LSLFunctionSignature functionSignature,
            ILSLReadOnlyExprNode attemptedReturnExpression);


        /// <summary>
        ///     Type mismatch between the expression returned from a function and the functions actual return type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The signature of the function the return was attempted from.</param>
        /// <param name="attemptedReturnExpression">The expression that was attempted to be returned.</param>
        void TypeMismatchInReturnValue(LSLSourceCodeRange location, LSLFunctionSignature functionSignature,
            ILSLReadOnlyExprNode attemptedReturnExpression);


        /// <summary>
        ///     An empty return statement was encountered in a non void function. <para/>
        ///     In other words, the function required an expression to be returned but none was provided.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The signature of the function the return was attempted from.</param>
        void ReturnedVoidFromNonVoidFunction(LSLSourceCodeRange location, LSLFunctionSignature functionSignature);


        /// <summary>
        ///     A jump statement to an undefined label name was encountered.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="labelName">The name of the label that was given to the jump statement.</param>
        void JumpToUndefinedLabel(LSLSourceCodeRange location, string labelName);


        /// <summary>
        ///     A call to an undefined function was encountered.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the function used in the call.</param>
        void CallToUndefinedFunction(LSLSourceCodeRange location, string functionName);


        /// <summary>
        ///     A user defined or library function was called with the wrong number of parameters.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The function signature of the defined function attempting to be called.</param>
        /// <param name="parameterExpressionsGiven">The expressions given to the function call.</param>
        void ImproperParameterCountInFunctionCall(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature, ILSLReadOnlyExprNode[] parameterExpressionsGiven);


        /// <summary>
        ///     A user defined function was re-defined.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="previouslyDefinedSignature">
        ///     The signature of the previously defined function that is considered a
        ///     duplicated to the new definition.
        /// </param>
        void RedefinedFunction(LSLSourceCodeRange location, LSLFunctionSignature previouslyDefinedSignature);


        /// <summary>
        ///     A code label was considered a redefinition, given the scope of the new definition.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="labelName">The name of the label being redefined.</param>
        void RedefinedLabel(LSLSourceCodeRange location, string labelName);


        /// <summary>
        ///     A '.' member access operation was attempted on an invalid variable type, or the variable type did not contain the given member.  <para/>
        ///     Valid member names for vectors are:  "x", "y" and "z" <para/>
        ///     Valid member names for rotations are:  "x", "y", "z" and "s" <para/>
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="exprLvalue">The variable expression on the left side of the dot operator.</param>
        /// <param name="memberAccessed">The member/component name on the right side of the dot operator.</param>
        void InvalidTupleComponentAccessOperation(LSLSourceCodeRange location, ILSLReadOnlyExprNode exprLvalue,
            string memberAccessed);


        /// <summary>
        ///     The return type of an expression used in an if statement condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the if statement.</param>
        void IfConditionNotValidType(LSLSourceCodeRange location, ILSLReadOnlyExprNode attemptedConditionExpression);


        /// <summary>
        ///     The return type of an expression used in an else-if statement condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the else-if statement.</param>
        void ElseIfConditionNotValidType(LSLSourceCodeRange location, ILSLReadOnlyExprNode attemptedConditionExpression);


        /// <summary>
        ///     The return type of an expression used in a do-loops condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the do-loop.</param>
        void DoLoopConditionNotValidType(LSLSourceCodeRange location, ILSLReadOnlyExprNode attemptedConditionExpression);


        /// <summary>
        ///     The return type of an expression used in a while-loops condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the while-loop.</param>
        void WhileLoopConditionNotValidType(LSLSourceCodeRange location, ILSLReadOnlyExprNode attemptedConditionExpression);


        /// <summary>
        ///     The return type of an expression used in a for-loops condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the for-loop.</param>
        void ForLoopConditionNotValidType(LSLSourceCodeRange location, ILSLReadOnlyExprNode attemptedConditionExpression);


        /// <summary>
        ///     A parameter type mismatch occured when trying to call a user defined or library function.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="parameterNumberWithError">The index of the parameter with the type mismatch. (Zero based)</param>
        /// <param name="calledFunction">The defined/library function that was attempting to be called.</param>
        /// <param name="parameterExpressionsGiven">The parameter expressions given for the function call.</param>
        void ParameterTypeMismatchInFunctionCall(LSLSourceCodeRange location, int parameterNumberWithError,
            LSLFunctionSignature calledFunction, ILSLReadOnlyExprNode[] parameterExpressionsGiven);


        /// <summary>
        ///     A state name was re-defined.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="stateName">The name of the state being redefined.</param>
        void RedefinedStateName(LSLSourceCodeRange location, string stateName);


        /// <summary>
        ///     An event handler was declared that was not defined in the library data provider.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="givenEventHandlerSignature">The signature of the event handler attempting to be used.</param>
        void UnknownEventHandlerDeclared(LSLSourceCodeRange location,
            LSLEventSignature givenEventHandlerSignature);


        /// <summary>
        ///     An event handler was declared in the program with a call signature differing from its definition in the library data provider.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="givenEventHandlerSignature">The invalid signature used for the event handler in the source code.</param>
        /// <param name="correctEventHandlerSignature">
        ///     The actual valid signature for the event handler from the library data
        ///     provider.
        /// </param>
        void IncorrectEventHandlerSignature(LSLSourceCodeRange location,
            LSLEventSignature givenEventHandlerSignature,
            LSLLibraryEventSignature correctEventHandlerSignature);


        /// <summary>
        ///     A standard library constant defined in the library data provider was redefined in source code as a global or local
        ///     variable.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="redefinitionType">The type used in the re-definition.</param>
        /// <param name="originalSignature">The original signature of the constant taken from the library data provider.</param>
        void RedefinedStandardLibraryConstant(LSLSourceCodeRange location, LSLType redefinitionType,
            LSLLibraryConstantSignature originalSignature);


        /// <summary>
        ///     A library function that exist in the library data provider was redefined by the user.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the library function the user attempted to redefine.</param>
        /// <param name="libraryFunctionSignatureOverloads">
        ///     All of the overloads for the library function, there may only be one if
        ///     no overloads actually exist.
        /// </param>
        void RedefinedStandardLibraryFunction(LSLSourceCodeRange location, string functionName,
            IReadOnlyGenericArray<LSLLibraryFunctionSignature> libraryFunctionSignatureOverloads);


        /// <summary>
        ///     A state change statement was encountered that referenced an undefined state name.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="stateName">The undefined state name referenced.</param>
        void ChangeToUndefinedState(LSLSourceCodeRange location, string stateName);


        /// <summary>
        ///     An attempt to modify a library constant defined in the library data provider was made.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="constantName">The name of the constant the user attempted to modified.</param>
        void ModifiedLibraryConstant(LSLSourceCodeRange location, string constantName);


        /// <summary>
        ///     The user attempted to use 'default' for a user defined state name.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        void RedefinedDefaultState(LSLSourceCodeRange location);


        /// <summary>
        ///     Dead code after a return path was detected in a function with a non-void return type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="inFunction">The signature of the function the dead code was detected in.</param>
        /// <param name="deadSegment">An object describing the location an span of the dead code segment.</param>
        void DeadCodeAfterReturnPath(LSLSourceCodeRange location, LSLFunctionSignature inFunction,
            LSLDeadCodeSegment deadSegment);


        /// <summary>
        ///     A function with a non-void return type lacks a necessary return statement. <para/>
        ///     There is a path that leads to the end of the function without it returning a value.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="inFunction">The signature of the function in question.</param>
        void NotAllCodePathsReturnAValue(LSLSourceCodeRange location, LSLFunctionSignature inFunction);


        /// <summary>
        ///     A code state does not declare any event handlers at all; this is not allowed.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="stateName">The name of the state in which this error occurred.</param>
        void StateHasNoEventHandlers(LSLSourceCodeRange location, string stateName);


        /// <summary>
        ///     A conditional expression is missing from an IF, ELSE IF, or WHILE/DO-WHILE statement.  <para/>
        ///     FOR loops can have a missing condition expression, but other control statements cannot.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="statementType">The type of branch/loop statement the condition was missing from.</param>
        void MissingConditionalExpression(LSLSourceCodeRange location, LSLConditionalStatementType statementType);


        /// <summary>
        ///     Attempted to define a variable inside of a braceless scope. <para/>
        ///     For example, inside of an IF statement which does not use braces in its code area. (a single statement is used) <para/>
        ///     This applies to other control/loop statements that can use braceless scopes as well.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        void DefinedVariableInBracelessScope(LSLSourceCodeRange location);


        /// <summary>
        ///     An illegal character was found inside of a string literal according to the current
        ///     <see cref="ILSLStringPreProcessor" /> instance.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="err">The generated character error object from the <see cref="ILSLStringPreProcessor" /> instance.</param>
        void IllegalStringCharacter(LSLSourceCodeRange location, LSLStringCharacterError err);


        /// <summary>
        ///     An invalid escape sequence was found inside of a string literal according to the current
        ///     <see cref="ILSLStringPreProcessor" /> instance.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="err">The generated character error object from the <see cref="ILSLStringPreProcessor" /> instance.</param>
        void InvalidStringEscapeCode(LSLSourceCodeRange location, LSLStringCharacterError err);


        /// <summary>
        ///     A call to function was attempted in a static context.  <para/>
        ///     (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">Location in source code.</param>
        void CallToFunctionInStaticContext(LSLSourceCodeRange location);


        /// <summary>
        ///     A library defined event handler was used more than once in the same state.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="eventHandlerName">The name of the event handler which was used more than once.</param>
        /// <param name="stateName">The name of the code state in which the error occured.</param>
        void RedefinedEventHandler(LSLSourceCodeRange location, string eventHandlerName, string stateName);


        /// <summary>
        ///     The default code state was missing from the program.
        /// </summary>
        void MissingDefaultState();


        /// <summary>
        ///     An overload could not be resolved for an attempted call to an overloaded library function.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the overloaded library function that the user attempted to call.</param>
        /// <param name="givenParameterExpressions">
        ///     The parameter expressions the user attempted to pass to the overloaded library
        ///     function.
        /// </param>
        void NoSuitableLibraryFunctionOverloadFound(LSLSourceCodeRange location, string functionName,
            IReadOnlyGenericArray<ILSLReadOnlyExprNode> givenParameterExpressions);


        /// <summary>
        ///     The arguments passed to an overloaded library function match up with more than one defined overload.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the overloaded library function that the user attempted to call.</param>
        /// <param name="ambiguousMatches">All of the function overloads the call to the library function matched up with.</param>
        /// <param name="givenParameterExpressions">
        ///     The parameter expressions the user attempted to pass to the overloaded library
        ///     function.
        /// </param>
        void CallToOverloadedLibraryFunctionIsAmbiguous(LSLSourceCodeRange location, string functionName,
            IReadOnlyGenericArray<LSLLibraryFunctionSignature> ambiguousMatches,
            IReadOnlyGenericArray<ILSLReadOnlyExprNode> givenParameterExpressions);


        /// <summary>
        ///     The dot operator used to access the components of vectors and rotations was used on a library constant.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="libraryConstantReferenceNode">The variable reference node on the left side of the dot operator.</param>
        /// <param name="libraryConstantSignature">
        ///     The library constant signature that was referenced, retrieved from the library
        ///     data provider.
        /// </param>
        /// <param name="accessedMember">The member the user attempted to access.</param>
        void TupleComponentAccessOnLibraryConstant(LSLSourceCodeRange location,
            ILSLVariableNode libraryConstantReferenceNode,
            LSLLibraryConstantSignature libraryConstantSignature,
            string accessedMember);


        /// <summary>
        ///     A binary operator was used in a static context.  <para/>
        ///     (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        void BinaryOperatorInStaticContext(LSLSourceCodeRange location);


        /// <summary>
        ///     A parenthesized expression was used in a static context.  <para/>
        ///     (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        void ParenthesizedExpressionInStaticContext(LSLSourceCodeRange location);


        /// <summary>
        ///     A postfix expression was used in a static context.  <para/>
        ///     (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        void PostfixOperationInStaticContext(LSLSourceCodeRange location);


        /// <summary>
        ///     An invalid prefix expression was used in a static context (a global variable declaration expression) <para/>
        ///     Negate is the only prefix operator allowed in a static context, and only on literals and not variables.
        /// </summary>
        /// <param name="location">The location of the error.</param>
        /// <param name="type">The operation type.</param>
        void InvalidPrefixOperationUsedInStaticContext(LSLSourceCodeRange location, LSLPrefixOperationType type);


        /// <summary>
        ///     A prefix operator was used on a global variable reference in a static context.  <para/>
        ///     (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        /// <param name="type">The operation type.</param>
        void PrefixOperationOnGlobalVariableInStaticContext(LSLSourceCodeRange location,
            LSLPrefixOperationType type);


        /// <summary>
        ///     A postfix operation was applied to an expression that was not a variable.
        /// </summary>
        /// <param name="location">The location of the error.</param>
        /// <param name="type">The operation type.</param>
        void PostfixOperationOnNonVariable(LSLSourceCodeRange location, LSLPostfixOperationType type);


        /// <summary>
        ///     A modifying prefix operation was applied to an expression that was not a variable.
        /// </summary>
        /// <param name="location">The location of the error.</param>
        /// <param name="type">The operation type.</param>
        void ModifyingPrefixOperationOnNonVariable(LSLSourceCodeRange location, LSLPrefixOperationType type);


        /// <summary>
        ///     The negate prefix operator was used on a vector literal in a static context. <para/>
        ///      (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        void NegateOperationOnVectorLiteralInStaticContext(LSLSourceCodeRange location);


        /// <summary>
        ///     The negate prefix operator was used on a rotation literal in a static context. <para/>
        ///     (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        void NegateOperationOnRotationLiteralInStaticContext(LSLSourceCodeRange location);


        /// <summary>
        ///     A cast expression was used inside of a static context. <para/>
        ///     (in a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        void CastExpressionInStaticContext(LSLSourceCodeRange location);


        /// <summary>
        ///     Occurs with an expression that is left of an assignment type operator is not assignable.
        ///     This includes compound assignment operators such as: += ... <para/>
        ///     This error occurs only for left expressions that are not library constants.
        ///     There is a separate error for library constants, see <see cref="ModifiedLibraryConstant" />.
        /// </summary>
        /// <param name="location">The source code range of the assignment operator used.</param>
        /// <param name="assignmentOperatorUsed">The assignment operator used.</param>
        void AssignmentToNonassignableExpression(LSLSourceCodeRange location, string assignmentOperatorUsed);
    }
}