#region FileInfo
// 
// File: ILSLSyntaxWarningListener.cs
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
using LibLSLCC.CodeValidator.Nodes;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.LibraryData;

#endregion

namespace LibLSLCC.CodeValidator.Components
{

    /// <summary>
    /// Interface for a syntax error listener, this called into by the code validator
    /// when syntax errors occur.
    /// </summary>
    public interface ILSLSyntaxWarningListener
    {
        /// <summary>
        /// Warns about the occurrence of multiple list or list variable assignments occurring inside of a single expression.
        /// This sort of thing was a common optimization for LSL when LSO was used instead of Mono.
        /// It can result in unexpected behavior in some LSL compilers such as OpenSims default compiler.
        /// 
        /// The code generator provided with this library can handle old LSO optimizations such as this one, but it is good
        /// to warn about it since the optimization is completely unnecessary now days and may even make your code slower on Mono.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        void MultipleListAssignmentsInExpression(LSLSourceCodeRange location);




        /// <summary>
        /// Warns about the occurrence of multiple string or string variable assignments occurring inside of a single expression.
        /// This sort of thing was a common optimization for LSL when LSO was used instead of Mono.
        /// It can result in unexpected behavior in some LSL compilers such as OpenSims default compiler.
        /// 
        /// The code generator provided with this library can handle old LSO optimizations such as this one, but it is good
        /// to warn about it since the optimization is completely unnecessary now days and may even make your code slower on Mono.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        void MultipleStringAssignmentsInExpression(LSLSourceCodeRange location);




        /// <summary>
        /// Dead code was detected, but it was not an error because it was in a function with a void return type.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        /// <param name="currentFunction">The signature of the function that dead code was detected in.</param>
        /// <param name="deadSegment">An object describing the range of code that is considered to be dead.</param>
        void DeadCodeDetected(LSLSourceCodeRange location, LSLFunctionSignature currentFunction,
            LSLDeadCodeSegment deadSegment);

        /// <summary>
        /// Dead code was detected, but it was not an error because it was in an event handler.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        /// <param name="currentEvent">The signature of the event handler that dead code was detected in.</param>
        /// <param name="deadSegment">An object describing the range of code that is considered to be dead.</param>
        void DeadCodeDetected(LSLSourceCodeRange location, LSLEventSignature currentEvent,
            LSLDeadCodeSegment deadSegment);


        /// <summary>
        /// A constant value was used for the conditional expression in a control or loop statement.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        /// <param name="expression">The offending expression.</param>
        /// <param name="conditionalStatementType">The type of conditional statement the expression was used in.</param>
        void ConditionalExpressionIsConstant(LSLSourceCodeRange location, ILSLExprNode expression,
            LSLConditionalStatementType conditionalStatementType);

        /// <summary>
        /// An un-needed but valid semi-colon was detected in code, such as when a semi-colon is on a line by itself or multiple semi-colons follow an expression statement when only one is needed.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        void UselessSemicolon(LSLSourceCodeRange location);

        /// <summary>
        /// An expression statement has no effect.  This can happen if you simply reference a variable as an expression statement and do nothing to it.
        /// For example, reference a counter but forget to add an increment or decrement operator to it.
        /// It would compile but it might be an error.
        /// </summary>
        /// <param name="location"></param>
        void ExpressionStatementHasNoEffect(LSLSourceCodeRange location);


        /// <summary>
        /// The expression in the 'after thought' of a for loop has no affect, for example: If its just a variable reference.
        /// This can happen if you forget to add an increment or decrement operator to a loop counter.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        /// <param name="expression">The offending expression.</param>
        /// <param name="expressionIndex">
        /// For loop afterthoughts can be a list of expressions to be executed, separated by commas.  
        /// This is the index of the expression in the possible list of expressions.
        /// If the for loop afterthought does not use a list of expressions, this is always 0.
        /// </param>
        /// <param name="expressionCountTotal">
        /// The number of expressions used in the for loop afterthought expression list.
        /// </param>
        void ForLoopAfterthoughtHasNoEffect(LSLSourceCodeRange location, ILSLExprNode expression, int expressionIndex, int expressionCountTotal);


        /// <summary>
        /// The expression in the 'init section' of a for loop has no affect, for example: If its just a variable reference.
        /// This can happen if you forget to assign a starting value to a loop counter.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        /// <param name="expression">The offending expression.</param>
        /// <param name="expressionIndex">
        /// For loop init expressions can be a list of expressions to be executed, separated by commas.  
        /// This is the index of the expression in the possible list of expressions.
        /// If the for loop init expression does not use a list of expressions, this is always 0.
        /// </param>
        /// <param name="expressionCountTotal">
        /// The number of expressions used in the for loop init expression list.
        /// </param>
        void ForLoopInitExpressionHasNoEffect(LSLSourceCodeRange location, ILSLExprNode expression, int expressionIndex, int expressionCountTotal);

        /// <summary>
        /// A cast is considered redundant because the expression the user is attempting to cast is already
        /// of the same type being cast to.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        /// <param name="castType">The type the user attempted to cast the expression to.</param>
        void RedundantCast(LSLSourceCodeRange location, LSLType castType);


        /// <summary>
        /// A user defined function was never referenced.
        /// </summary>
        /// <param name="location">The location in the source code of the function that was never referenced.</param>
        /// <param name="functionDeclarationNode">The function declaration node of the un-referenced function.</param>
        void FunctionNeverUsed(LSLSourceCodeRange location, ILSLFunctionDeclarationNode functionDeclarationNode);

        /// <summary>
        /// A user defined global variable was never referenced.
        /// </summary>
        /// <param name="location">The location in the source code of the global variable that was never referenced.</param>
        /// <param name="variable">The variable declaration node of the un-referenced global variable.</param>
        void GlobalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable);

        /// <summary>
        /// A user defined local variable was never referenced.
        /// </summary>
        /// <param name="location">The location in the source code of the local variable that was never referenced.</param>
        /// <param name="variable">The variable declaration node of the un-referenced local variable.</param>
        /// <param name="inFunction">The signature of the function in which the local variable exists.</param>
        void LocalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable,
            LSLFunctionSignature inFunction);

        /// <summary>
        /// A user defined local variable was never referenced.
        /// </summary>
        /// <param name="location">The location in the source code of the local variable that was never referenced.</param>
        /// <param name="variable">The variable declaration node of the un-referenced local variable.</param>
        /// <param name="inEvent">The signature of the event handler in which the local variable exists.</param>
        void LocalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable,
            LSLEventSignature inEvent);


        /// <summary>
        /// A user defined function parameter was never referenced.
        /// </summary>
        /// <param name="location">The location in the source code of the parameter that was never referenced.</param>
        /// <param name="parameter">The variable declaration node of the un-referenced function parameter.</param>
        /// <param name="inFunction">The signature of the function in which the parameter exists.</param>
        void FunctionParameterNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode parameter,
            LSLFunctionSignature inFunction);


        /// <summary>
        /// A parameter name of a user defined function hides the definition of a global variable.
        /// </summary>
        /// <param name="location">The location in source code of the parameter that hides the global variable.</param>
        /// <param name="functionSignature">The function signature to which the parameter definition belongs to.</param>
        /// <param name="parameter">The signature of the parameter that hides the global variable.</param>
        /// <param name="globalVariable">The variable declaration node of the global variable that was hidden.</param>
        void ParameterHidesGlobalVariable(LSLSourceCodeRange location, LSLFunctionSignature functionSignature,
            LSLParameterNode parameter, LSLVariableDeclarationNode globalVariable);


        /// <summary>
        /// A parameter name of a library defined event handler hides the definition of a global variable.
        /// </summary>
        /// <param name="location">The location in source code of the parameter that hides the global variable.</param>
        /// <param name="eventHandlerSignature">The event handler signature to which the parameter definition belongs to.</param>
        /// <param name="parameter">The signature of the parameter that hides the global variable.</param>
        /// <param name="globalVariable">The variable declaration node of the global variable that was hidden.</param>
        void ParameterHidesGlobalVariable(LSLSourceCodeRange location, LSLEventSignature eventHandlerSignature,
            LSLParameterNode parameter, LSLVariableDeclarationNode globalVariable);

        /// <summary>
        /// A local variable name inside of a user defined function hides the definition of one of the functions parameters.
        /// </summary>
        /// <param name="location">The location in source code of the local variable that hides the function parameter.</param>
        /// <param name="functionSignature">The pre-defined signature of the function in which the local variable is defined.</param>
        /// <param name="localVariable">The variable declaration node of the local variable that hides the parameter.</param>
        /// <param name="parameter">The parameter node of the parameter that was hidden.</param>
        void LocalVariableHidesParameter(LSLSourceCodeRange location, LSLPreDefinedFunctionSignature functionSignature,
            LSLVariableDeclarationNode localVariable, LSLParameterNode parameter);

        /// <summary>
        /// A local variable name inside of a library defined event handler hides the definition of one of the event handlers parameters.
        /// </summary>
        /// <param name="location">The location in source code of the local variable that hides the event handler parameter.</param>
        /// <param name="eventHandlerSignature">The parsed signature of the event handler in which the local variable is defined.</param>
        /// <param name="localVariable">The variable declaration node of the local variable that hides the parameter.</param>
        /// <param name="parameter">The parameter node of the parameter that was hidden.</param>
        void LocalVariableHidesParameter(LSLSourceCodeRange location,
            LSLParsedEventHandlerSignature eventHandlerSignature, LSLVariableDeclarationNode localVariable,
            LSLParameterNode parameter);


        /// <summary>
        /// A local variable inside of a user defined function hides the definition of a user defined global variable.
        /// </summary>
        /// <param name="location">The location in source code of the local variable that hides the global variable.</param>
        /// <param name="functionSignature">The pre-defined signature of the function in which the local variable is defined.</param>
        /// <param name="localVariable">The variable declaration node of the local variable that hides the global variable.</param>
        /// <param name="globalVariable">The variable declaration node of the user defined global variable that was hidden.</param>
        void LocalVariableHidesGlobalVariable(LSLSourceCodeRange location,
            LSLPreDefinedFunctionSignature functionSignature, LSLVariableDeclarationNode localVariable,
            LSLVariableDeclarationNode globalVariable);


        /// <summary>
        /// A local variable inside of a library defined event handler hides the definition of a user defined global variable.
        /// </summary>
        /// <param name="location">The location in source code of the local variable that hides the global variable.</param>
        /// <param name="eventHandlerSignature">The parsed signature of the event handler in which the local variable is defined.</param>
        /// <param name="localVariable">The variable declaration node of the local variable that hides the global variable.</param>
        /// <param name="globalVariable">The variable declaration node of the user defined global variable that was hidden.</param>
        void LocalVariableHidesGlobalVariable(LSLSourceCodeRange location,
            LSLParsedEventHandlerSignature eventHandlerSignature, LSLVariableDeclarationNode localVariable,
            LSLVariableDeclarationNode globalVariable);


        /// <summary>
        /// A library function that was marked as being deprecated was used.
        /// </summary>
        /// <param name="location">The location in source code where the deprecated function was called.</param>
        /// <param name="functionSignature">The library function signature of the deprecated function that was called.</param>
        void UseOfDeprecatedLibraryFunction(LSLSourceCodeRange location, LSLLibraryFunctionSignature functionSignature);


        /// <summary>
        /// A library constant that was marked as being deprecated was used.
        /// </summary>
        /// <param name="location">The location in source code where the deprecated constant was referenced.</param>
        /// <param name="constantSignature">The library constant signature of the deprecated constant that was referenced.</param>
        void UseOfDeprecatedLibraryConstant(LSLSourceCodeRange location, LSLLibraryConstantSignature constantSignature);



        /// <summary>
        /// A library event handler that was marked as being deprecated was used.
        /// </summary>
        /// <param name="location">The location in source code where the deprecated event handler was referenced.</param>
        /// <param name="eventSignature">The library event signature of the deprecated event handler that was referenced.</param>
        void UseOfDeprecatedLibraryEventHandler(LSLSourceCodeRange location, LSLLibraryEventSignature eventSignature);



        /// <summary>
        /// A local variable was re-declared inside of a nested scope, such as an if statement or for loop, ect...
        /// This is not an error, but bad practice. This function handles the warning case inside function declarations.
        /// </summary>
        /// <param name="location">The source code range of the new variable declaration.</param>
        /// <param name="currentFunctionBodySignature">The signature of the function the new variable was declared in.</param>
        /// <param name="newDeclarationNode">The tree node of the new declaration that has not been added to the tree yet.</param>
        /// <param name="previousDeclarationNode">The previous variable declaration node which already exist in the syntax tree, in an outer scope.</param>
        void VariableRedeclaredInInnerScope(LSLSourceCodeRange location,
            LSLFunctionSignature currentFunctionBodySignature,
            LSLVariableDeclarationNode newDeclarationNode, 
            LSLVariableDeclarationNode previousDeclarationNode);


        /// <summary>
        /// A local variable was re-declared inside of a nested scope, such as an if statement or for loop, ect...
        /// This is not an error, but bad practice.  This function handles the warning case inside event handlers.
        /// </summary>
        /// <param name="location">The source code range of the new variable declaration.</param>
        /// <param name="currentEventBodySignature">The signature of the event handler the new variable was declared in.</param>
        /// <param name="newDeclarationNode">The tree node of the new declaration that has not been added to the tree yet.</param>
        /// <param name="previousDeclarationNode">The previous variable declaration node which already exist in the syntax tree, in an outer scope.</param>
        void VariableRedeclaredInInnerScope(LSLSourceCodeRange location,
            LSLEventSignature currentEventBodySignature,
            LSLVariableDeclarationNode newDeclarationNode,
            LSLVariableDeclarationNode previousDeclarationNode);

        /// <summary>
        /// Occurs when an integer literal present in the source code is greater than the max value of a 32 bit LSL integer.
        /// </summary>
        /// <param name="location">The source code range of the integer literal.</param>
        /// <param name="literalText">The text representing the integer literal.</param>
        void IntegerLiteralOverflow(LSLSourceCodeRange location, string literalText);


        /// <summary>
        /// Occurs when a hex literal present in the source code is greater than the max value of a 32 bit LSL integer.
        /// </summary>
        /// <param name="location">The source code range of the hex literal.</param>
        /// <param name="literalText">The text representing the hex literal.</param>
        void HexLiteralOverflow(LSLSourceCodeRange location, string literalText);


        /// <summary>
        /// Occurs when a return value inside of an event handler returns an expression instead of nothing.
        /// The return value of the expression is simply discarded in this case.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="eventSignature">The signature of the event handler this warning occurred in.</param>
        /// <param name="returnExpression">The return expression.</param>
        void ReturnedValueFromEventHandler(LSLSourceCodeRange location, LSLEventSignature eventSignature,
            ILSLExprNode returnExpression);
    }
}