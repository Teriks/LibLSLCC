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

using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;
using LibLSLCC.LibraryData;

#endregion

namespace LibLSLCC.CodeValidator.Components.Interfaces
{
    /// <summary>
    /// Interface for a syntax error listener, this called into by the code validator
    /// when syntax errors occur.
    /// </summary>
    public interface ILSLSyntaxErrorListener
    {
        /// <summary>
        /// A parsing error at the grammar level has occurred somewhere in the source code.
        /// </summary>
        /// <param name="line">The line on which the error occurred.</param>
        /// <param name="column">The character column at which the error occurred.</param>
        /// <param name="message">The parsing error messaged passed along from the parsing back end.</param>
        void GrammarLevelSyntaxError(int line, int column, string message);


        /// <summary>
        /// A reference to an undefined variable was encountered.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="name">Name of the variable attempting to be referenced.</param>
        void UndefinedVariableReference(LSLSourceCodeRange location, string name);

        /// <summary>
        /// A parameter name for a function or event handler was used more than once.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="type">The type of the new parameter who's name was duplicate.</param>
        /// <param name="name">The name of the new parameter, which was duplicate.</param>
        void ParameterNameRedefined(LSLSourceCodeRange location, LSLType type, string name);

        /// <summary>
        /// A binary operation was encountered that had incorrect expression types on either or both sides.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="left">The left expression.</param>
        /// <param name="operation">The binary operation that was attempted on the two expressions.</param>
        /// <param name="right">The right expression.</param>
        void InvalidBinaryOperation(LSLSourceCodeRange location, ILSLExprNode left, string operation, ILSLExprNode right);

        /// <summary>
        /// A prefix operation was attempted on an invalid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="operation">The prefix operation that was attempted on the expression.</param>
        /// <param name="right">The expression the prefix operation was used on.</param>
        void InvalidPrefixOperation(LSLSourceCodeRange location, string operation, ILSLExprNode right);

        /// <summary>
        /// A postfix operation was attempted on an invalid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="operation">The postfix operation that was attempted on the expression.</param>
        /// <param name="left">The expression the postfix operation was used on.</param>
        void InvalidPostfixOperation(LSLSourceCodeRange location, ILSLExprNode left, string operation);

        /// <summary>
        /// An invalid cast was preformed on some expression
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="castTo">The type that the cast operation attempted to cast the expression to.</param>
        /// <param name="fromExpression">The expression that the cast was attempted on.</param>
        void InvalidCastOperation(LSLSourceCodeRange location, LSLType castTo, ILSLExprNode fromExpression);


        /// <summary>
        /// A variable declaration was initialized with an invalid type on the right.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="variableType">The actual type of the variable attempting to be initialized.</param>
        /// <param name="assignedExpression">The invalid expression that was assigned in the variable declaration.</param>
        void TypeMismatchInVariableDeclaration(LSLSourceCodeRange location, LSLType variableType,
            ILSLExprNode assignedExpression);


        /// <summary>
        /// A variable was redefined.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="variableType">The type of the variable that was considered a re-definition.</param>
        /// <param name="variableName">The name of the variable that was considered a re-definition.</param>
        void VariableRedefined(LSLSourceCodeRange location, LSLType variableType, string variableName);


        /// <summary>
        /// A vector literal contained an invalid expression in its initializer list.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="component">The vector component of the initializer that contained the invalid expression.</param>
        /// <param name="invalidExpressionContent">The expression that was considered to be invalid vector initializer content.</param>
        void InvalidVectorContent(LSLSourceCodeRange location, LSLVectorComponent component,
            ILSLExprNode invalidExpressionContent);

        /// <summary>
        /// A list literal contained an invalid expression in its initializer list.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="index">The index in the initializer list that contained the invalid expression.</param>
        /// <param name="invalidExpressionContent">The expression that was considered to be invalid list initializer content.</param>
        void InvalidListContent(LSLSourceCodeRange location, int index, ILSLExprNode invalidExpressionContent);


        /// <summary>
        /// A rotation literal contained an invalid expression in its initializer list.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="component">The rotation component of the initializer that contained the invalid expression.</param>
        /// <param name="invalidExpressionContent">The expression that was considered to be invalid rotation initializer content.</param>
        void InvalidRotationContent(LSLSourceCodeRange location, LSLRotationComponent component,
            ILSLExprNode invalidExpressionContent);


        /// <summary>
        /// Attempted to return a value from a function with no return type. (A void function)
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The signature of the function the return was attempted from.</param>
        /// <param name="attemptedReturnExpression">The expression that was attempted to be returned.</param>
        void ReturnedValueFromVoidFunction(LSLSourceCodeRange location, LSLFunctionSignature functionSignature,
            ILSLExprNode attemptedReturnExpression);


        /// <summary>
        /// Attempted to return an invalid expression type from a function with a defined return type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The signature of the function the return was attempted from.</param>
        /// <param name="attemptedReturnExpression">The expression that was attempted to be returned.</param>
        void TypeMismatchInReturnValue(LSLSourceCodeRange location, LSLFunctionSignature functionSignature,
            ILSLExprNode attemptedReturnExpression);

        /// <summary>
        /// An empty return statement was encountered in a non void function. (A function with a defined return type)
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The signature of the function the return was attempted from.</param>
        void ReturnedVoidFromANonVoidFunction(LSLSourceCodeRange location, LSLFunctionSignature functionSignature);

        /// <summary>
        /// A jump statement to an undefined label name was encountered.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="labelName">The name of the label that was given to the jump statement.</param>
        void JumpToUndefinedLabel(LSLSourceCodeRange location, string labelName);


        /// <summary>
        /// A call to an undefined function was encountered.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the function used in the call.</param>
        void CallToUndefinedFunction(LSLSourceCodeRange location, string functionName);


        /// <summary>
        /// A user defined or library function was called with the wrong number of parameters.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionSignature">The function signature of the defined function attempting to be called.</param>
        /// <param name="parameterExpressionsGiven">The expressions given to the function call.</param>
        void ImproperParameterCountInFunctionCall(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature, ILSLExprNode[] parameterExpressionsGiven);

        /// <summary>
        /// Attempted to return a value from an event handler.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedReturnExpression">The expression attempted to be returned.</param>
        void ReturnedValueFromEventHandler(LSLSourceCodeRange location, ILSLExprNode attemptedReturnExpression);


        /// <summary>
        /// A user defined function was re-defined.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="previouslyDefinedSignature">The signature of the previously defined function that is considered a duplicated to the new definition.</param>
        void RedefinedFunction(LSLSourceCodeRange location, LSLFunctionSignature previouslyDefinedSignature);


        /// <summary>
        /// A code label was considered a redefinition of an already defined code label, given the scope of the new definition.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="labelName">The name of the label being redefined.</param>
        void RedefinedLabel(LSLSourceCodeRange location, string labelName);



        /// <summary>
        /// A '.' member access was attempted on an invalid variable type, or the variable type did not contain the given component.
        /// Valid component names for vectors are:  x,y and z
        /// Valid component names for rotations are:  x,y,z and s
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="exprLvalue">The variable expression on the left side of the dot operator.</param>
        /// <param name="memberAccessed">The member/component name on the right side of the dot operator.</param>
        void InvalidTupleComponentAccessorOperation(LSLSourceCodeRange location, ILSLExprNode exprLvalue,
            string memberAccessed);


        /// <summary>
        /// The return type of the expression present in an if statements condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the if statement.</param>
        void IfConditionNotValidType(LSLSourceCodeRange location, ILSLExprNode attemptedConditionExpression);


        /// <summary>
        /// The return type of the expression present in an else-if statements condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the else-if statement.</param>
        void ElseIfConditionNotValidType(LSLSourceCodeRange location, ILSLExprNode attemptedConditionExpression);

        /// <summary>
        /// The return type of the expression present in a do-loops condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the do-loop.</param>
        void DoLoopConditionNotValidType(LSLSourceCodeRange location, ILSLExprNode attemptedConditionExpression);

        /// <summary>
        /// The return type of the expression present in a while-loops condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the while-loop.</param>
        void WhileLoopConditionNotValidType(LSLSourceCodeRange location, ILSLExprNode attemptedConditionExpression);


        /// <summary>
        /// The return type of the expression present in a for-loops condition is not a valid type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="attemptedConditionExpression">The invalid expression in the condition area of the for-loop.</param>
        void ForLoopConditionNotValidType(LSLSourceCodeRange location, ILSLExprNode attemptedConditionExpression);


        /// <summary>
        /// A parameter type mismatch was encountered when trying to call a user defined or library function.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="parameterNumberWithError">The index of the parameter with the type mismatch. (Zero based)</param>
        /// <param name="calledFunction">The defined/library function that was attempting to be called.</param>
        /// <param name="parameterExpressionsGiven">The parameter expressions given for the function call.</param>
        void ParameterTypeMismatchInFunctionCall(LSLSourceCodeRange location, int parameterNumberWithError,
            LSLFunctionSignature calledFunction, ILSLExprNode[] parameterExpressionsGiven);


        /// <summary>
        /// A state name was re-defined.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="stateName">The name of the state being redefined.</param>
        void RedefinedStateName(LSLSourceCodeRange location, string stateName);


        /// <summary>
        /// An event handler which was not defined in the library data provider was used in the program.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="givenEventHandlerSignature">The signature of the event handler attempting to be used.</param>
        void UnknownEventHandlerDeclared(LSLSourceCodeRange location,
            LSLEventSignature givenEventHandlerSignature);


        /// <summary>
        /// An event handler was used in the program which was defined in the library data provider, but the given call signature in the program
        /// was incorrect.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="givenEventHandlerSignature">The invalid signature used for the event handler in the source code.</param>
        /// <param name="correctEventHandlerSignature">The actual valid signature for the event handler from the library data provider.</param>
        void IncorrectEventHandlerSignature(LSLSourceCodeRange location,
            LSLEventSignature givenEventHandlerSignature,
            LSLLibraryEventSignature correctEventHandlerSignature);


        /// <summary>
        /// A standard library constant defined in the library data provider was redefined in source code as a global or local variable.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="redefinitionType">The type used in the re-definition.</param>
        /// <param name="originalSignature">The original signature of the constant taken from the library data provider.</param>
        void RedefinedStandardLibraryConstant(LSLSourceCodeRange location, LSLType redefinitionType,
            LSLLibraryConstantSignature originalSignature);


        /// <summary>
        /// A library function that exist in the library data provider was redefined by the user as a user defined function.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the library function the user attempted to redefine.</param>
        /// <param name="libraryFunctionSignatureOverloads">All of the overloads for the library function, there may only be one if no overloads actually exist.</param>
        void RedefinedStandardLibraryFunction(LSLSourceCodeRange location, string functionName,
            IReadOnlyGenericArray<LSLLibraryFunctionSignature> libraryFunctionSignatureOverloads);

        /// <summary>
        /// A state change statement was encountered that attempted to change states to an undefined state name.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="stateName">The undefined state name referenced.</param>
        void ChangeToUndefinedState(LSLSourceCodeRange location, string stateName);


        /// <summary>
        /// An attempt to modify a library constant defined in the library data provider was made.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="constantName">The name of the constant the user attempted to modified.</param>
        void ModifiedLibraryConstant(LSLSourceCodeRange location, string constantName);

        /// <summary>
        /// The user attempted to use 'default' for a user defined state name.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        void RedefinedDefaultState(LSLSourceCodeRange location);

        /// TODO check necessity
        /// <summary>
        /// The given expression was not valid as a statement in a code scope.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        void InvalidStatementExpression(LSLSourceCodeRange location);


        /// <summary>
        /// Dead code after a return path was detected in a function with a non-void return type.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="inFunction">The signature of the function the dead code was detected in.</param>
        /// <param name="deadSegment">An object describing the location an span of the dead code segment.</param>
        void DeadCodeAfterReturnPathDetected(LSLSourceCodeRange location, LSLFunctionSignature inFunction,
            LSLDeadCodeSegment deadSegment);

        /// <summary>
        /// A function with a non-void return type lacks a necessary return statement.  
        /// Not all code paths return a value.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="inFunction">The signature of the function in question.</param>
        void NotAllCodePathsReturnAValue(LSLSourceCodeRange location, LSLFunctionSignature inFunction);

        /// <summary>
        /// A code state does not declare the use of any event handlers at all. (This is not allowed)
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="stateName">The name of the state in which this error occurred.</param>
        void StateHasNoEventHandlers(LSLSourceCodeRange location, string stateName);

        /// <summary>
        /// A conditional expression is missing from an IF, ELSE IF, or WHILE/DO-WHILE statement;
        /// FOR loops can have a missing condition expression, but other control statements cannot.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="statementType">The type of branch/loop statement the condition was missing from.</param>
        void MissingConditionalExpression(LSLSourceCodeRange location, LSLConditionalStatementType statementType);

        /// <summary>
        /// Attempted to define a variable inside of a single statement block.  Such as inside of an IF statement which does not
        /// use braces.  This applies to other statements that can use brace-less single statement blocks as well.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        void DefinedVariableInNonScopeBlock(LSLSourceCodeRange location);

        /// <summary>
        /// An illegal character was found inside of a string literal according to the current <see cref="ILSLStringPreProcessor"/> instance.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="err">The generated character error object from the <see cref="ILSLStringPreProcessor"/> instance.</param>
        void IllegalStringCharacter(LSLSourceCodeRange location, LSLStringCharacterError err);

        /// <summary>
        /// An invalid escape sequence was found inside of a string literal according to the current <see cref="ILSLStringPreProcessor"/> instance.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="err">The generated character error object from the <see cref="ILSLStringPreProcessor"/> instance.</param>
        void InvalidStringEscapeCode(LSLSourceCodeRange location, LSLStringCharacterError err);

        /// <summary>
        /// A call to function was attempted in a static context.  For example, inside of a global variables declaration expression.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        void CallToFunctionInStaticContext(LSLSourceCodeRange location);


        /// <summary>
        /// A library defined event handler was used more than once in the same state.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="eventHandlerName">The name of the event handler which was used more than once.</param>
        /// <param name="stateName">The name of the code state in which the error occured.</param>
        void RedefinedEventHandler(LSLSourceCodeRange location, string eventHandlerName, string stateName);

        /// <summary>
        /// The default code state was missing from the program.
        /// </summary>
        void MissingDefaultState();


        /// <summary>
        /// An overload could not be resolved for an attempted call to an overloaded library function.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the overloaded library function that the user attempted to call.</param>
        /// <param name="givenParameterExpressions">The parameter expressions the user attempted to pass to the overloaded library function.</param>
        void NoSuitableLibraryFunctionOverloadFound(LSLSourceCodeRange location, string functionName,
            IReadOnlyGenericArray<ILSLExprNode> givenParameterExpressions);


        /// <summary>
        /// A call to an overloaded library function matches up with one or more overloads.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="functionName">The name of the overloaded library function that the user attempted to call.</param>
        /// <param name="ambigiousMatches">All of the function overloads the call to the library function matched up with.</param>
        /// <param name="givenParameterExpressions">The parameter expressions the user attempted to pass to the overloaded library function.</param>
        void CallToOverloadedLibraryFunctionIsAmbigious(LSLSourceCodeRange location, string functionName,
            IReadOnlyGenericArray<LSLLibraryFunctionSignature> ambigiousMatches,
            IReadOnlyGenericArray<ILSLExprNode> givenParameterExpressions);


        /// <summary>
        /// The dot operator used to access the tuple component members of vectors and rotations was used on a library constant.
        /// </summary>
        /// <param name="location">Location in source code.</param>
        /// <param name="libraryConstantReferenceNode">The variable reference node on the left side of the dot operator.</param>
        /// <param name="libraryConstantSignature">The library constant signature that was referenced, retrieved from the library data provider.</param>
        /// <param name="accessedMember">The member the user attempted to access.</param>
        void TupleAccessorOnLibraryConstant(LSLSourceCodeRange location,
            ILSLVariableNode libraryConstantReferenceNode,
            LSLLibraryConstantSignature libraryConstantSignature,
            string accessedMember);


        /// <summary>
        /// A binary operator was used in a static context (a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        void BinaryOperatorUsedInStaticContext(LSLSourceCodeRange location);

        /// <summary>
        /// A parenthesized expression was used in a static context (a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        void ParenthesizedExpressionUsedInStaticContext(LSLSourceCodeRange location);


        /// <summary>
        /// A postfix expression was used in a static context (a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        void PostfixOperationUsedInStaticContext(LSLSourceCodeRange location);


        /// <summary>
        /// An invalid prefix expression was used in a static context (a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        /// <param name="type">The operation type.</param>
        void InvalidPrefixOperationUsedInStaticContext(LSLSourceCodeRange location, LSLPrefixOperationType type);

        /// <summary>
        /// A prefix expression with a global variable on the right was used in a static context. (a global variable declaration expression)
        /// </summary>
        /// <param name="location">The location of the error.</param>
        /// <param name="type">The operation type.</param>
        void InvalidPrefixOperationUsedGlobalVariableInStaticContext(LSLSourceCodeRange location,
            LSLPrefixOperationType type);


        /// <summary>
        /// A postfix operation was applied to an expression that was not a variable.
        /// </summary>
        /// <param name="location">The location of the error.</param>
        /// <param name="type">The operation type.</param>
        void PostfixOperationOnNonVariable(LSLSourceCodeRange location, LSLPostfixOperationType type);


        /// <summary>
        /// A modifying prefix operation was applied to an expression that was not a variable.
        /// </summary>
        /// <param name="location">The location of the error.</param>
        /// <param name="type">The operation type.</param>
        void ModifyingPrefixOperationOnNonVariable(LSLSourceCodeRange location, LSLPrefixOperationType type);


        /// <summary>
        /// The negate prefix operator was used on a vector literal in a static context.
        /// IE, a global variable declaration.
        /// </summary>
        /// <param name="location">The location of the error.</param>
        void NegateOperationOnVectorLiteralInStaticContext(LSLSourceCodeRange location);


        /// <summary>
        /// The negate prefix operator was used on a rotation literal in a static context.
        /// IE, a global variable declaration.
        /// </summary>
        /// <param name="location">The location of the error.</param>
        void NegateOperationOnRotationLiteralInStaticContext(LSLSourceCodeRange location);


        /// <summary>
        /// A cast expression was used inside of a static context, IE during the declaration of a global variable.
        /// </summary>
        /// <param name="location">The location of the error.</param>
        void CastExpressionUsedInStaticContext(LSLSourceCodeRange location);
    }
}