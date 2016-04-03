#region FileInfo

// 
// File: LSLDefaultSyntaxWarningListener.cs
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

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     The default library implementation of <see cref="ILSLSyntaxWarningListener" /> that can write warning
    ///     information to an arbitrary output stream.  The default stream is standard out.
    /// </summary>
    public class LSLDefaultSyntaxWarningListener : ILSLSyntaxWarningListener
    {
        /// <summary>
        ///     Warns about the occurrence of multiple list or list variable assignments occurring inside of a single expression.
        /// </summary>
        /// <remarks>
        ///     This sort of thing was a common optimization for LSL when LSO was used instead of Mono.
        ///     It can result in unexpected behavior in some LSL compilers such as OpenSims default compiler.
        ///     The code generator provided with this library can handle old LSO optimizations such as this one, but it is good
        ///     to warn about it since the optimization is completely unnecessary now days and may even make your code slower on
        ///     Mono.
        /// </remarks>
        /// <param name="location">The location in the source code.</param>
        public virtual void MultipleListAssignmentsInExpression(LSLSourceCodeRange location)
        {
            OnWarning(location,
                "Multiple list assignments in expression, may not evaluate how you expect.");
        }


        /// <summary>
        ///     Warns about the occurrence of multiple string or string variable assignments occurring inside of a single expression.
        /// </summary>
        /// <remarks>
        ///     This sort of thing was a common optimization for LSL when LSO was used instead of Mono.
        ///     It can result in unexpected behavior in some LSL compilers such as OpenSims default compiler.
        ///     The code generator provided with this library can handle old LSO optimizations such as this one, but it is good
        ///     to warn about it since the optimization is completely unnecessary now days and may even make your code slower on
        ///     Mono.
        /// </remarks>
        /// <param name="location">The location in the source code.</param>
        public virtual void MultipleStringAssignmentsInExpression(LSLSourceCodeRange location)
        {
            OnWarning(location,
                "Multiple string assignments in expression, may not evaluate how you expect.");
        }


        /// <summary>
        ///     Dead code was detected, but it was not an error because it was inside a function with a void return type.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        /// <param name="currentFunction">The signature of the function that dead code was detected in.</param>
        /// <param name="deadSegment">An object describing the range of code that is considered to be dead.</param>
        public virtual void DeadCodeDetected(LSLSourceCodeRange location, ILSLFunctionSignature currentFunction,
            ILSLDeadCodeSegment deadSegment)
        {
            if (deadSegment.SourceRange.IsSingleLine)
            {
                OnWarning(location, "Unreachable code detected in function \"" + currentFunction.Name + "\".");
            }
            else
            {
                OnWarning(location,
                    string.Format(
                        "Unreachable code detected in function \"" + currentFunction.Name +
                        "\" between lines {0} and {1}.",
                        MapLineNumber(deadSegment.SourceRange.LineStart),
                        MapLineNumber(deadSegment.SourceRange.LineEnd)));
            }
        }


        /// <summary>
        ///     Dead code was detected, but it was not an error because it was inside an event handler.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        /// <param name="currentEvent">The signature of the event handler that dead code was detected in.</param>
        /// <param name="deadSegment">An object describing the range of code that is considered to be dead.</param>
        public virtual void DeadCodeDetected(LSLSourceCodeRange location, ILSLEventSignature currentEvent,
            ILSLDeadCodeSegment deadSegment)
        {
            if (deadSegment.SourceRange.IsSingleLine)
            {
                OnWarning(location, "Unreachable code detected in event handler \"" + currentEvent.Name + "\".");
            }
            else
            {
                OnWarning(location,
                    string.Format(
                        "Unreachable code detected in event handler \"" + currentEvent.Name +
                        "\" between lines {0} and {1}.",
                        MapLineNumber(deadSegment.SourceRange.LineStart),
                        MapLineNumber(deadSegment.SourceRange.LineEnd)));
            }
        }


        /// <summary>
        ///     An un-needed but valid semi-colon was detected in code, such as when a semi-colon is on a line by itself or
        ///     multiple semi-colons follow an expression statement when only one is needed.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        public virtual void UselessSemicolon(LSLSourceCodeRange location)
        {
            OnWarning(location, "Pointless semicolon detected.");
        }


        /// <summary>
        ///     An expression statement has no effect; this can happen if you simply reference a variable as an expression statement and do nothing to it. <para/>
        ///     As an example, referencing a counter but forgetting to add an increment or decrement operator to it would cause this. <para/>
        ///     It will compile but it might be an error.
        /// </summary>
        /// <param name="location">The source code location of the expression statement.</param>
        /// <param name="statementExpression">The offending expression used as a statement.</param>
        public virtual void ExpressionStatementHasNoEffect(LSLSourceCodeRange location, ILSLReadOnlyExprNode statementExpression)
        {
            OnWarning(location, "Expression statement has no effect.");
        }


        /// <summary>
        ///     The expression in the 'after thought' of a for loop has no affect. <para/>
        ///     As an example, this can happen if you forget to add an increment or decrement operator to a loop counter variable.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        /// <param name="expression">The offending expression.</param>
        /// <param name="expressionIndex">
        ///     For loop afterthoughts can be a list of expressions to be executed, separated by commas.
        ///     This is the index of the expression in the possible list of expressions.
        ///     If the for loop afterthought does not use a list of expressions, this is always 0.
        /// </param>
        /// <param name="expressionCountTotal">
        ///     The number of expressions used in the for loop afterthought expression list.
        /// </param>
        public virtual void ForLoopAfterthoughtHasNoEffect(LSLSourceCodeRange location, ILSLReadOnlyExprNode expression,
            int expressionIndex,
            int expressionCountTotal)
        {
            if (expressionCountTotal == 1)
            {
                OnWarning(location, "For loop afterthought has no effect.");
            }
            else
            {
                OnWarning(location,
                    string.Format("For loop afterthought number {0} has no effect.", expressionIndex + 1));
            }
        }


        /// <summary>
        ///     The expression in the 'init section' of a for loop has no affect, for example: If its just a variable reference.  <para/>
        ///     As an example, this can happen if you forget to assign a starting value to a loop counter.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        /// <param name="expression">The offending expression.</param>
        /// <param name="expressionIndex">
        ///     For loop init expressions can be a list of expressions to be executed, separated by commas.
        ///     This is the index of the expression in the possible list of expressions.
        ///     If the for loop init expression does not use a list of expressions, this is always 0.
        /// </param>
        /// <param name="expressionCountTotal">
        ///     The number of expressions used in the for loop init expression list.
        /// </param>
        public virtual void ForLoopInitExpressionHasNoEffect(LSLSourceCodeRange location, ILSLReadOnlyExprNode expression,
            int expressionIndex,
            int expressionCountTotal)
        {
            if (expressionCountTotal == 1)
            {
                OnWarning(location, "For loop initializer expression has no effect.");
            }
            else
            {
                OnWarning(location,
                    string.Format("For loop initializer number {0} has no effect.", expressionIndex + 1));
            }
        }


        /// <summary>
        ///     A cast is considered redundant because the expression the user casted already has the same return type as the cast-to type.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        /// <param name="castType">The type the user attempted to cast the expression to.</param>
        public virtual void RedundantCast(LSLSourceCodeRange location, LSLType castType)
        {
            OnWarning(location, string.Format("Redundant cast to {0}.", castType));
        }


        /// <summary>
        ///     A user defined function was never referenced.
        /// </summary>
        /// <param name="location">The location in the source code of the function that was never referenced.</param>
        /// <param name="functionDeclarationNode">The function declaration node of the un-referenced function.</param>
        public virtual void FunctionNeverUsed(LSLSourceCodeRange location,
            ILSLFunctionDeclarationNode functionDeclarationNode)
        {
            OnWarning(location, string.Format("Function \"{0}\" was never used.", functionDeclarationNode.Name));
        }


        /// <summary>
        ///     A user defined global variable was never referenced.
        /// </summary>
        /// <param name="location">The location in the source code of the global variable that was never referenced.</param>
        /// <param name="variable">The variable declaration node of the un-referenced global variable.</param>
        public virtual void GlobalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable)
        {
            const string msg = "Global variable \"{0}\" was never used.";


            OnWarning(location, string.Format(msg, variable.Name));
        }


        /// <summary>
        ///     A user defined local variable was never referenced.
        /// </summary>
        /// <param name="location">The location in the source code of the local variable that was never referenced.</param>
        /// <param name="variable">The variable declaration node of the un-referenced local variable.</param>
        /// <param name="inFunction">The signature of the function in which the local variable exists.</param>
        public virtual void LocalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable,
            ILSLFunctionSignature inFunction)
        {
            const string msg = "Local variable \"{0}\" was never used in function \"{1}\".";


            OnWarning(location, string.Format(msg, variable.Name, inFunction.Name));
        }


        /// <summary>
        ///     A user defined local variable was never referenced.
        /// </summary>
        /// <param name="location">The location in the source code of the local variable that was never referenced.</param>
        /// <param name="variable">The variable declaration node of the un-referenced local variable.</param>
        /// <param name="inEvent">The signature of the event handler in which the local variable exists.</param>
        public virtual void LocalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable,
            ILSLEventSignature inEvent)
        {
            const string msg = "Local variable \"{0}\" was never used in event \"{1}\".";

            OnWarning(location, string.Format(msg, variable.Name, inEvent.Name));
        }


        /// <summary>
        ///     A user defined function parameter was never referenced.
        /// </summary>
        /// <param name="location">The location in the source code of the parameter that was never referenced.</param>
        /// <param name="parameter">The variable declaration node of the un-referenced function parameter.</param>
        /// <param name="inFunction">The signature of the function in which the parameter exists.</param>
        public virtual void FunctionParameterNeverUsed(LSLSourceCodeRange location,
            ILSLVariableDeclarationNode parameter,
            ILSLFunctionSignature inFunction)
        {
            OnWarning(location,
                string.Format("Parameter \"{0}\" was never used in function \"{1}\".", parameter.Name, inFunction.Name));
        }


        /// <summary>
        ///     A parameter name of a user defined function hides the definition of a global variable.
        /// </summary>
        /// <param name="location">The location in source code of the parameter that hides the global variable.</param>
        /// <param name="functionSignature">The function signature to which the parameter definition belongs to.</param>
        /// <param name="parameter">The signature of the parameter that hides the global variable.</param>
        /// <param name="globalVariable">The variable declaration node of the global variable that was hidden.</param>
        public virtual void ParameterHidesGlobalVariable(LSLSourceCodeRange location,
            ILSLFunctionSignature functionSignature,
            ILSLParameterNode parameter, ILSLVariableDeclarationNode globalVariable)
        {
            OnWarning(location,
                string.Format(
                    "Parameter \"{0}\" of function \"{1}\" hides global variable \"{2}\" defined on line {3}.",
                    parameter.Name, functionSignature.Name, globalVariable.Name,
                    MapLineNumber(globalVariable.SourceRange.LineStart)));
        }


        /// <summary>
        ///     A parameter name of a library defined event handler hides the definition of a global variable.
        /// </summary>
        /// <param name="location">The location in source code of the parameter that hides the global variable.</param>
        /// <param name="eventHandlerSignature">The event handler signature to which the parameter definition belongs to.</param>
        /// <param name="parameter">The signature of the parameter that hides the global variable.</param>
        /// <param name="globalVariable">The variable declaration node of the global variable that was hidden.</param>
        public virtual void ParameterHidesGlobalVariable(LSLSourceCodeRange location,
            ILSLEventSignature eventHandlerSignature,
            ILSLParameterNode parameter, ILSLVariableDeclarationNode globalVariable)
        {
            OnWarning(location,
                string.Format(
                    "Parameter \"{0}\" of event handler \"{1}\" hides global variable \"{2}\" defined on line {3}.",
                    parameter.Name, eventHandlerSignature.Name, globalVariable.Name,
                    MapLineNumber(globalVariable.SourceRange.LineStart)));
        }


        /// <summary>
        ///     A local variable name inside of a user defined function hides the definition of one of the functions parameters.
        /// </summary>
        /// <param name="location">The location in source code of the local variable that hides the function parameter.</param>
        /// <param name="functionSignature">The signature of the function in which the local variable is defined.</param>
        /// <param name="localVariable">The variable declaration node of the local variable that hides the parameter.</param>
        /// <param name="parameter">The parameter node of the parameter that was hidden.</param>
        public virtual void LocalVariableHidesParameter(LSLSourceCodeRange location,
            ILSLFunctionSignature functionSignature,
            ILSLVariableDeclarationNode localVariable, ILSLParameterNode parameter)
        {
            OnWarning(location, string.Format("Local variable \"{0}\" in function \"{1}\" hides parameter \"{2}\".",
                localVariable.Name, functionSignature.Name, parameter.Name));
        }


        /// <summary>
        ///     A local variable name inside of a library defined event handler hides the definition of one of the event handlers
        ///     parameters.
        /// </summary>
        /// <param name="location">The location in source code of the local variable that hides the event handler parameter.</param>
        /// <param name="eventHandlerSignature">The signature of the event handler in which the local variable is defined.</param>
        /// <param name="localVariable">The variable declaration node of the local variable that hides the parameter.</param>
        /// <param name="parameter">The parameter node of the parameter that was hidden.</param>
        public virtual void LocalVariableHidesParameter(LSLSourceCodeRange location,
            ILSLEventSignature eventHandlerSignature,
            ILSLVariableDeclarationNode localVariable, ILSLParameterNode parameter)
        {
            OnWarning(location,
                string.Format("Local variable \"{0}\" in event handler \"{1}\" hides parameter \"{2}\".",
                    localVariable.Name, eventHandlerSignature.Name, parameter.Name));
        }


        /// <summary>
        ///     A local variable inside of a user defined function hides the definition of a user defined global variable.
        /// </summary>
        /// <param name="location">The location in source code of the local variable that hides the global variable.</param>
        /// <param name="functionSignature">The signature of the function in which the local variable is defined.</param>
        /// <param name="localVariable">The variable declaration node of the local variable that hides the global variable.</param>
        /// <param name="globalVariable">The variable declaration node of the user defined global variable that was hidden.</param>
        public virtual void LocalVariableHidesGlobalVariable(LSLSourceCodeRange location,
            ILSLFunctionSignature functionSignature,
            ILSLVariableDeclarationNode localVariable, ILSLVariableDeclarationNode globalVariable)
        {
            OnWarning(location,
                string.Format(
                    "Local variable \"{0}\" in function \"{1}\" hides global variable \"{2}\" defined on line {3}.",
                    localVariable.Name, functionSignature.Name, globalVariable.Name,
                    MapLineNumber(globalVariable.SourceRange.LineStart)));
        }


        /// <summary>
        ///     A local variable inside of a library defined event handler hides the definition of a user defined global variable.
        /// </summary>
        /// <param name="location">The location in source code of the local variable that hides the global variable.</param>
        /// <param name="eventHandlerSignature">The parsed signature of the event handler in which the local variable is defined.</param>
        /// <param name="localVariable">The variable declaration node of the local variable that hides the global variable.</param>
        /// <param name="globalVariable">The variable declaration node of the user defined global variable that was hidden.</param>
        public virtual void LocalVariableHidesGlobalVariable(LSLSourceCodeRange location,
            ILSLEventSignature eventHandlerSignature,
            ILSLVariableDeclarationNode localVariable, ILSLVariableDeclarationNode globalVariable)
        {
            OnWarning(location,
                string.Format(
                    "Local variable \"{0}\" in event handler \"{1}\" hides global variable \"{2}\" defined on line {3}.",
                    localVariable.Name, eventHandlerSignature.Name, globalVariable.Name,
                    MapLineNumber(globalVariable.SourceRange.LineStart)));
        }


        /// <summary>
        ///     A library function that was marked as being deprecated was used.
        /// </summary>
        /// <param name="location">The location in source code where the deprecated function was called.</param>
        /// <param name="functionSignature">The function signature of the deprecated library function that was called.</param>
        public virtual void UseOfDeprecatedLibraryFunction(LSLSourceCodeRange location,
            ILSLFunctionSignature functionSignature)
        {
            OnWarning(location,
                string.Format(
                    "The library function \"{0}\" is deprecated, it is recommended you use an alternative or remove it.",
                    functionSignature.Name));
        }


        /// <summary>
        ///     A library constant that was marked as being deprecated was used.
        /// </summary>
        /// <param name="location">The location in source code where the deprecated constant was referenced.</param>
        /// <param name="constantSignature">The library constant signature of the deprecated constant that was referenced.</param>
        public virtual void UseOfDeprecatedLibraryConstant(LSLSourceCodeRange location,
            ILSLConstantSignature constantSignature)
        {
            OnWarning(location,
                string.Format(
                    "The library constant \"{0}\" is deprecated, it is recommended you use an alternative or remove it.",
                    constantSignature.Name));
        }


        /// <summary>
        ///     A library event handler that was marked as being deprecated was used.
        /// </summary>
        /// <param name="location">The location in source code where the deprecated event handler was referenced.</param>
        /// <param name="eventSignature">The event signature of the deprecated event handler that was referenced.</param>
        public virtual void UseOfDeprecatedLibraryEventHandler(LSLSourceCodeRange location,
            ILSLEventSignature eventSignature)
        {
            OnWarning(location,
                string.Format(
                    "The library event handler \"{0}\" is deprecated, it is recommended you use an alternative or remove it.",
                    eventSignature.Name));
        }


        /// <summary>
        ///     A local variable was re-declared inside of a nested scope, such as an if statement or for loop, ect... <para/>
        ///     This is not an error, but bad practice. This function handles the warning case inside function declarations.
        /// </summary>
        /// <param name="location">The source code range of the new variable declaration.</param>
        /// <param name="currentFunctionBodySignature">The signature of the function the new variable was declared in.</param>
        /// <param name="newDeclarationNode">The tree node of the new declaration that has not been added to the tree yet.</param>
        /// <param name="previousDeclarationNode">
        ///     The previous variable declaration node which already exist in the syntax tree, in
        ///     an outer scope.
        /// </param>
        public virtual void VariableRedeclaredInInnerScope(LSLSourceCodeRange location,
            ILSLFunctionSignature currentFunctionBodySignature,
            ILSLVariableDeclarationNode newDeclarationNode, ILSLVariableDeclarationNode previousDeclarationNode)
        {
            OnWarning(location,
                string.Format(
                    "Local variable \"{0}\" in function \"{1}\" hides a previous declaration in an outer scope (on line {2}).",
                    newDeclarationNode.Name, currentFunctionBodySignature.Name,
                    MapLineNumber(previousDeclarationNode.SourceRange.LineStart)));
        }


        /// <summary>
        ///     A local variable was re-declared inside of a nested scope, such as an if statement or for loop, ect... <para/>
        ///     This is not an error, but bad practice.  This function handles the warning case inside event handlers.
        /// </summary>
        /// <param name="location">The source code range of the new variable declaration.</param>
        /// <param name="currentEventBodySignature">The signature of the event handler the new variable was declared in.</param>
        /// <param name="newDeclarationNode">The tree node of the new declaration that has not been added to the tree yet.</param>
        /// <param name="previousDeclarationNode">
        ///     The previous variable declaration node which already exist in the syntax tree, in
        ///     an outer scope.
        /// </param>
        public virtual void VariableRedeclaredInInnerScope(LSLSourceCodeRange location,
            ILSLEventSignature currentEventBodySignature,
            ILSLVariableDeclarationNode newDeclarationNode, ILSLVariableDeclarationNode previousDeclarationNode)
        {
            OnWarning(location,
                string.Format(
                    "Local variable \"{0}\" in event handler \"{1}\" hides a previous declaration in an outer scope (on line {2}).",
                    newDeclarationNode.Name, currentEventBodySignature.Name,
                    MapLineNumber(previousDeclarationNode.SourceRange.LineStart)));
        }


        /// <summary>
        ///     Occurs when an integer literal present in the source code is greater than the max value of a 32 bit LSL integer.
        /// </summary>
        /// <param name="location">The source code range of the integer literal.</param>
        /// <param name="literalText">The text representing the integer literal.</param>
        /// <param name="negated">Whether or not a negate operator was applied to the integer literal.</param>
        public virtual void IntegerLiteralOverflow(LSLSourceCodeRange location, string literalText, bool negated)
        {
            OnWarning(location,
                string.Format("Integer literal \"{0}\" overflows LSL's integer type, it will compile to -1.",
                    literalText));
        }


        /// <summary>
        ///     Occurs when an integer literal present in the source code is less than the minimum value of a 32 bit LSL integer.
        /// </summary>
        /// <param name="location">The source code range of the integer literal.</param>
        /// <param name="literalText">The text representing the integer literal.</param>
        /// <param name="negated">Whether or not a negate operator was applied to the integer literal.</param>
        public void IntegerLiteralUnderflow(LSLSourceCodeRange location, string literalText, bool negated)
        {
            OnWarning(location,
                string.Format("Integer literal \"-{0}\" underflows LSL's integer type, it will compile to 1.",
                    literalText));
        }


        /// <summary>
        ///     Occurs when a hex literal present in the source code is greater than the max value of a 32 bit LSL integer.
        /// </summary>
        /// <param name="location">The source code range of the hex literal.</param>
        /// <param name="literalText">The text representing the hex literal.</param>
        /// <param name="negated">Whether or not a negate operator was applied to the hex literal.</param>
        public virtual void HexLiteralOverflow(LSLSourceCodeRange location, string literalText, bool negated)
        {
            if (negated)
            {
                OnWarning(location,
                    string.Format("Negated hex literal \"{0}\" overflows LSL's integer type;  It will compile to \"-(-1)\" or effectively \"1\".",
                    literalText));
            }
            else
            {
                OnWarning(location,
                    string.Format("Hex literal \"{0}\" overflows LSL's integer type, it will compile to -1.",
                        literalText));
            }
        }


        /// <summary>
        ///     Occurs when a hex literal present in the source code is less than the minimum value of a 32 bit LSL integer.
        /// </summary>
        /// <param name="location">The source code range of the hex literal.</param>
        /// <param name="literalText">The text representing the hex literal.</param>
        /// <param name="negated">Whether or not a negate operator was applied to the hex literal.</param>
        public void HexLiteralUnderflow(LSLSourceCodeRange location, string literalText, bool negated)
        {
            if (negated)
            {
                OnWarning(location,
                    string.Format("Negated hex literal \"{0}\" underflows LSL's integer type;  It will compile to \"-(-1)\" or effectively \"1\".",
                    literalText));
            }
            else
            {
                OnWarning(location,
                    string.Format("Hex literal \"{0}\" underflows LSL's integer type, it will compile to -1.",
                        literalText));
            }
        }


        /// <summary>
        ///     Occurs when a return value inside of an event handler returns an expression instead of nothing.  <para/>
        ///     The return value of the expression is simply discarded in this case.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="eventSignature">The signature of the event handler this warning occurred in.</param>
        /// <param name="returnExpression">The return expression.</param>
        public virtual void ReturnedValueFromEventHandler(LSLSourceCodeRange location, ILSLEventSignature eventSignature,
            ILSLReadOnlyExprNode returnExpression)
        {
            OnWarning(location,
                string.Format(
                    "Return statement in event handler \"{0}\" returns a value that will be discarded.",
                    eventSignature.Name));
        }



        /// <summary>
        ///     Occurs when an float literal present in the source code is greater than the max value of a 32 bit LSL float.
        /// </summary>
        /// <param name="location">The source code range of the float literal.</param>
        /// <param name="literalText">The text representing the float literal.</param>
        /// <param name="negated">Whether or not a negate operator was applied to the float literal.</param>
        public void FloatLiteralUnderflow(LSLSourceCodeRange location, string literalText, bool negated)
        {
            if (negated)
            {
                OnWarning(location,
                    string.Format("Negated float literal \"{0}\" underflows LSL's float type, it will compile to -0.0.",
                    literalText));
            }
            else
            {
                OnWarning(location,
                    string.Format("Float literal \"{0}\" underflows LSL's float type, it will compile to 0.0.",
                        literalText));
            }
        }



        /// <summary>
        ///     Occurs when an float literal present in the source code is less than the minimum value of a 32 bit LSL float.
        /// </summary>
        /// <param name="location">The source code range of the float literal.</param>
        /// <param name="literalText">The text representing the float literal.</param>
        /// <param name="negated">Whether or not a negate operator was applied to the float literal.</param>
        public void FloatLiteralOverflow(LSLSourceCodeRange location, string literalText, bool negated)
        {
            if (negated)
            {
                OnWarning(location,
                    string.Format("Negated float literal \"{0}\" overflows LSL's float type;  It will compile to -Infinity.",
                    literalText));
            }
            else
            {
                OnWarning(location,
                    string.Format("Float literal \"{0}\" overflows LSL's float type, it will compile to Infinity.",
                        literalText));
            }
        }


        /// <summary>
        ///     A constant value was used for the conditional expression in a control or loop statement.
        /// </summary>
        /// <param name="location">The location in the source code.</param>
        /// <param name="expression">The offending expression.</param>
        /// <param name="conditionalStatementType">The type of conditional statement the expression was used in.</param>
        public virtual void ConditionalExpressionIsConstant(LSLSourceCodeRange location, ILSLReadOnlyExprNode expression,
            LSLConditionalStatementType conditionalStatementType)
        {
            if (conditionalStatementType == LSLConditionalStatementType.If)
            {
                OnWarning(location, "Conditional expression in 'if' branch is constant.");
            }
            else if (conditionalStatementType == LSLConditionalStatementType.ElseIf)
            {
                OnWarning(location, "Conditional expression in 'else if' branch is constant.");
            }
            else if (conditionalStatementType == LSLConditionalStatementType.For)
            {
                OnWarning(location, "Conditional expression in 'for' loop is constant.");
            }
            else if (conditionalStatementType == LSLConditionalStatementType.While)
            {
                OnWarning(location, "Conditional expression in 'while' loop is constant.");
            }
            else if (conditionalStatementType == LSLConditionalStatementType.DoWhile)
            {
                OnWarning(location, "Conditional expression in 'do while' loop is constant.");
            }
        }


        /// <summary>
        ///     A hook to allow the modification of line numbers used in either the header of a warning or the body.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         You should pass line numbers you wish to put into customized error messages
        ///         through this function so that the derived class can easily offset them.
        ///         Line numbers reported in syntax warnings default to using a 'one' based index where
        ///         line #1 is the first line of source code.  To modify all line numbers reported in syntax
        ///         warnings you could overload this function and return the passed in value with some offset
        ///         added/subtracted.
        ///         For example, if you want all line number references in warnings sent to OnWarning to have a 0 based index.
        ///         then you should return (oneBasedLine-1) from this function.
        ///     </para>
        /// </remarks>
        /// <param name="oneBasedLine">
        ///     The 'one' based line number that we might need to modify, a common modification would be to
        ///     subtract 1 from it.
        /// </param>
        /// <returns>The possibly modified line number to use in the warning message.</returns>
        protected virtual int MapLineNumber(int oneBasedLine)
        {
            return oneBasedLine;
        }


        /// <summary>
        ///     A hook for intercepting warning messages produced by the implementations of all other functions in the
        ///     LSLDefaultSyntaxWarningListener object.
        ///     The default behavior is to write error messages to the Console.
        /// </summary>
        /// <param name="location">Location in source code for the warning.</param>
        /// <param name="message">The warning message.</param>
        protected virtual void OnWarning(LSLSourceCodeRange location, string message)
        {
            Console.WriteLine("({0},{1}) WARNING: {2}", MapLineNumber(location.LineStart), location.ColumnStart,
                message + Environment.NewLine);
        }
    }
}