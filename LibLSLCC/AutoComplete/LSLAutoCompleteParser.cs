#region FileInfo

// 
// File: LSLAutoCompleteParser.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2016, Teriks
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

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;
using LibLSLCC.Parser;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.AutoComplete
{
    /// <summary>
    ///     An LSL parser that can help with implementing context aware auto-complete inside of code editors.
    /// </summary>
    public sealed class LSLAutoCompleteParser : ILSLAutoCompleteParser
    {
        private static readonly Regex JumpRegex = new Regex("jump\\s*(" + LSLTokenTools.IDRegexString + ")");
        private static readonly Regex LabelRegex = new Regex("@\\s*(" + LSLTokenTools.IDRegexString + ")");
        private LSLAutoCompleteVisitor _autocompleteVisitor;

        /// <summary>
        ///     The offset the <see cref="LSLAutoCompleteParser" /> last parsed to.
        /// </summary>
        public int ParseToOffset
        {
            get { return _autocompleteVisitor.ParseToOffset; }
        }

        /// <summary>
        ///     The name of the state block <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> resides in.
        ///     <c>null</c> if the parse to offset is outside of a state body.
        /// </summary>
        public string CurrentState
        {
            get { return _autocompleteVisitor.CurrentState; }
        }

        /// <summary>
        ///     The name of the function declaration <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> resides in.
        ///     <c>null</c> if the parse to offset is outside of a function body.
        /// </summary>
        public string CurrentFunction
        {
            get { return _autocompleteVisitor.CurrentFunction; }
        }

        /// <summary>
        ///     The name of the event handler declaration <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> offset resides in.
        ///     <c>null</c> if the parse to offset is outside of an event body.
        /// </summary>
        public string CurrentEvent
        {
            get { return _autocompleteVisitor.CurrentEvent; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a state block, but outside of an
        ///     event handler declaration.
        /// </summary>
        public bool InStateScope
        {
            get { return _autocompleteVisitor.InStateScope; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the source code range of an event handler.
        ///     This includes being within the name or parameter definitions.
        /// </summary>
        public bool InEventSourceRange
        {
            get { return _autocompleteVisitor.InEventSourceRange; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside the code body of an event handler.
        /// </summary>
        public bool InEventCodeBody
        {
            get { return _autocompleteVisitor.InEventCodeBody; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside the code body of a function
        ///     declaration.
        /// </summary>
        public bool InFunctionCodeBody
        {
            get { return _autocompleteVisitor.InFunctionCodeBody; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the global scope.
        /// </summary>
        public bool InGlobalScope
        {
            get { return _autocompleteVisitor.InGlobalScope; }
        }

        /// <summary>
        ///     Gets a list of <see cref="LSLAutoCompleteStateBlock" /> objects representing user defined script state blocks.
        /// </summary>
        public IReadOnlyGenericArray<LSLAutoCompleteStateBlock> StateBlocks
        {
            get { return _autocompleteVisitor.StateBlocks; }
        }

        /// <summary>
        ///     Gets a <see cref="LSLAutoCompleteStateBlock" /> object representing the scripts default state.
        /// </summary>
        public LSLAutoCompleteStateBlock DefaultState
        {
            get { return _autocompleteVisitor.DefaultState; }
        }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteGlobalVariable" /> objects representing global variables
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        /// </summary>
        public IEnumerable<LSLAutoCompleteGlobalVariable> GlobalVariables
        {
            get { return _autocompleteVisitor.GlobalVariables; }
        }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteLocalVariable" /> objects representing local variables
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        /// </summary>
        public IEnumerable<LSLAutoCompleteLocalVariable> LocalVariables
        {
            get { return _autocompleteVisitor.LocalVariables; }
        }

        /// <summary>
        ///     Gets a read only hash map of <see cref="LSLAutoCompleteGlobalFunction" /> objects representing global function
        ///     declarations
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.  The functions are keyed in the hash
        ///     map by name.
        /// </summary>
        public IReadOnlyHashMap<string, LSLAutoCompleteGlobalFunction> GlobalFunctionsDictionary
        {
            get { return _autocompleteVisitor.GlobalFunctionsDictionary; }
        }

        /// <summary>
        ///     Gets a read only hash map of <see cref="LSLAutoCompleteGlobalVariable" /> objects representing global variable
        ///     declarations
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.  The declarations are keyed in the hash
        ///     map by name.
        /// </summary>
        public IReadOnlyHashMap<string, LSLAutoCompleteGlobalVariable> GlobalVariablesDictionary
        {
            get { return _autocompleteVisitor.GlobalVariablesDictionary; }
        }

        /// <summary>
        ///     Gets a read only hash map of <see cref="LSLAutoCompleteLocalParameter" /> objects representing local parameter
        ///     declarations
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.  The declarations are keyed in the hash
        ///     map by name.
        /// </summary>
        public IReadOnlyHashMap<string, LSLAutoCompleteLocalParameter> LocalParametersDictionary
        {
            get { return _autocompleteVisitor.LocalParametersDictionary; }
        }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteGlobalFunction" /> objects representing global functions
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        /// </summary>
        public IEnumerable<LSLAutoCompleteGlobalFunction> GlobalFunctions
        {
            get { return _autocompleteVisitor.GlobalFunctions; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in an area where a code statement can exist.
        ///     (<see cref="ILSLAutoCompleteParserState.InMultiCodeStatementArea" /> ||
        ///     <see cref="ILSLAutoCompleteParserState.InBracelessCodeStatementArea" />)
        /// </summary>
        public bool InCodeStatementArea
        {
            get { return _autocompleteVisitor.InCodeStatementArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in a multi statement area where a code
        ///     statement can exist.
        /// </summary>
        public bool InMultiCodeStatementArea
        {
            get { return _autocompleteVisitor.InMultiCodeStatementArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in an area where a brace-less code scope
        ///     (single statement) can exist.
        /// </summary>
        public bool InBracelessCodeStatementArea
        {
            get { return _autocompleteVisitor.InBracelessCodeStatementArea; }
        }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteGlobalFunction" /> objects representing local parameters
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        /// </summary>
        public IEnumerable<LSLAutoCompleteLocalParameter> LocalParameters
        {
            get { return _autocompleteVisitor.LocalParameters; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a binary expression/prefix
        ///     expression/postfix expression or parenthesized expression.
        /// </summary>
        public bool InBasicExpressionTree
        {
            get { return _autocompleteVisitor.InBasicExpressionTree; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is to the right of the dot in a dot member
        ///     accessor expression.
        /// </summary>
        public bool RightOfDotAccessor
        {
            get { return _autocompleteVisitor.RightOfDotAccessor; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of the expression used to declare a
        ///     local variable.
        /// </summary>
        public bool InLocalVariableDeclarationExpression
        {
            get { return _autocompleteVisitor.InLocalVariableDeclarationExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of the expression used to declare a
        ///     global variable.
        /// </summary>
        public bool InGlobalVariableDeclarationExpression
        {
            get { return _autocompleteVisitor.InGlobalVariableDeclarationExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a function declarations parameter
        ///     declaration list.
        /// </summary>
        public bool InFunctionDeclarationParameterList
        {
            get { return _autocompleteVisitor.InFunctionDeclarationParameterList; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of an event declarations parameter
        ///     declaration list.
        /// </summary>
        public bool InEventDeclarationParameterList
        {
            get { return _autocompleteVisitor.InEventDeclarationParameterList; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of an 'if' statements condition
        ///     expression area.
        /// </summary>
        public bool InIfConditionExpression
        {
            get { return _autocompleteVisitor.InIfConditionExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of an 'else if' statements condition
        ///     expression area.
        /// </summary>
        public bool InElseIfConditionExpression
        {
            get { return _autocompleteVisitor.InElseIfConditionExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a function declaration or event
        ///     declaration code body.
        /// </summary>
        public bool InCodeBody
        {
            get { return _autocompleteVisitor.InCodeBody; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in a local expression area, such as a
        ///     condition area or function call arguments. etc..
        /// </summary>
        public bool InLocalExpressionArea
        {
            get { return _autocompleteVisitor.InLocalExpressionArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in a global area. currently only when
        ///     <see cref="ILSLAutoCompleteParserState.InGlobalVariableDeclarationExpression" /> is <c>true</c>.
        /// </summary>
        public bool InGlobalExpressionArea
        {
            get { return _autocompleteVisitor.InGlobalExpressionArea; }
        }

        /// <summary>
        ///     <see cref="ILSLAutoCompleteParserState.InGlobalExpressionArea" /> ||
        ///     <see cref="ILSLAutoCompleteParserState.InLocalExpressionArea" />.
        ///     If the offset is in a global variable declaration expression, or the start of one.  Or
        ///     a local expression area such as an expression statement, loop condition, function call parameters, for loop clauses
        ///     etc..
        /// </summary>
        public bool InExpressionArea
        {
            get { return _autocompleteVisitor.InExpressionArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the expression area of a return statement
        ///     inside of a function.
        /// </summary>
        public bool InFunctionReturnExpression
        {
            get { return _autocompleteVisitor.InFunctionReturnExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the expression area right of a compound
        ///     operation/assignment to a variable, such as after var += (here).
        /// </summary>
        public bool InModifyingVariableAssignmentExpression
        {
            get { return _autocompleteVisitor.InModifyingVariableAssignmentExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the expression area right of a compound
        ///     operation/assignment to a member of a variable, such as after var.x += (here).
        /// </summary>
        public bool InModifyingComponentAssignmentExpression
        {
            get { return _autocompleteVisitor.InModifyingComponentAssignmentExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the expression area right of an assignment
        ///     to a variable, such as after var = (here).
        /// </summary>
        public bool InPlainVariableAssignmentExpression
        {
            get { return _autocompleteVisitor.InPlainVariableAssignmentExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the expression area right of an assignment
        ///     to a member of a variable, such as after var.x = (here).
        /// </summary>
        public bool InPlainComponentAssignmentExpression
        {
            get { return _autocompleteVisitor.InPlainComponentAssignmentExpression; }
        }

        /// <summary>
        ///     <see cref="ILSLAutoCompleteParserState.InPlainVariableAssignmentExpression" /> ||
        ///     <see cref="ILSLAutoCompleteParserState.InModifyingVariableAssignmentExpression" />
        /// </summary>
        public bool InVariableAssignmentExpression
        {
            get { return _autocompleteVisitor.InVariableAssignmentExpression; }
        }

        /// <summary>
        ///     <see cref="ILSLAutoCompleteParserState.InPlainComponentAssignmentExpression" /> ||
        ///     <see cref="ILSLAutoCompleteParserState.InModifyingComponentAssignmentExpression" />
        /// </summary>
        public bool InComponentAssignmentExpression
        {
            get { return _autocompleteVisitor.InComponentAssignmentExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in an area where you could start typing the
        ///     name of the state in a state change statement.
        /// </summary>
        public bool InStateChangeStatementStateNameArea
        {
            get { return _autocompleteVisitor.InStateChangeStatementStateNameArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in an area where you could start typing the
        ///     name of the label in a jump statement.
        /// </summary>
        public bool InJumpStatementLabelNameArea
        {
            get { return _autocompleteVisitor.InJumpStatementLabelNameArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in an area where you could start typing the
        ///     name of a label.
        /// </summary>
        public bool InLabelDefinitionNameArea
        {
            get { return _autocompleteVisitor.InLabelDefinitionNameArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is anywhere in a for loops clauses area.
        /// </summary>
        public bool InForLoopClausesArea
        {
            get { return _autocompleteVisitor.InForLoopClausesArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in a do while statements condition area.
        /// </summary>
        public bool InDoWhileConditionExpression
        {
            get { return _autocompleteVisitor.InDoWhileConditionExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in a while statements condition area.
        /// </summary>
        public bool InWhileConditionExpression
        {
            get { return _autocompleteVisitor.InWhileConditionExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the source code range of a control
        ///     statement.
        /// </summary>
        public bool InControlStatementSourceRange
        {
            get { return _autocompleteVisitor.InControlStatementSourceRange; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a function calls parameter
        ///     expression list.
        /// </summary>
        public bool InFunctionCallParameterList
        {
            get { return _autocompleteVisitor.InFunctionCallParameterList; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a list literals initializer
        ///     expression list.
        /// </summary>
        public bool InListLiteralInitializer
        {
            get { return _autocompleteVisitor.InListLiteralInitializer; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a vector literals initializer
        ///     expression list.
        /// </summary>
        public bool InVectorLiteralInitializer
        {
            get { return _autocompleteVisitor.InVectorLiteralInitializer; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a rotation literals initializer
        ///     expression list.
        /// </summary>
        public bool InRotationLiteralInitializer
        {
            get { return _autocompleteVisitor.InRotationLiteralInitializer; }
        }

        /// <summary>
        ///     <c>true</c> if a library constant can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InExpressionArea" />
        /// </summary>
        public bool CanSuggestLibraryConstant
        {
            get { return _autocompleteVisitor.CanSuggestLibraryConstant; }
        }

        /// <summary>
        ///     <c>true</c> if a function call can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     (<see cref="ILSLAutoCompleteParserState.InLocalExpressionArea" /> ||
        ///     <see cref="ILSLAutoCompleteParserState.InCodeStatementArea" />)
        /// </summary>
        public bool CanSuggestFunction
        {
            get { return _autocompleteVisitor.CanSuggestFunction; }
        }

        /// <summary>
        ///     <c>true</c> if a local variable or parameter name can be suggested at
        ///     <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     (<see cref="ILSLAutoCompleteParserState.InLocalExpressionArea" /> ||
        ///     <see cref="ILSLAutoCompleteParserState.InCodeStatementArea" />)
        /// </summary>
        public bool CanSuggestLocalVariableOrParameter
        {
            get { return _autocompleteVisitor.CanSuggestLocalVariableOrParameter; }
        }

        /// <summary>
        ///     <c>true</c> if a global variable can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     (<see cref="ILSLAutoCompleteParserState.InLocalExpressionArea" /> ||
        ///     <see cref="ILSLAutoCompleteParserState.InCodeStatementArea" />)
        /// </summary>
        public bool CanSuggestGlobalVariable
        {
            get { return _autocompleteVisitor.CanSuggestGlobalVariable; }
        }

        /// <summary>
        ///     <c>true</c> if an event handler can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InStateScope" />
        /// </summary>
        public bool CanSuggestEventHandler
        {
            get { return _autocompleteVisitor.CanSuggestEventHandler; }
        }

        /// <summary>
        ///     <c>true</c> if a state name can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InStateChangeStatementStateNameArea" />
        /// </summary>
        public bool CanSuggestStateName
        {
            get { return _autocompleteVisitor.CanSuggestStateName; }
        }

        /// <summary>
        ///     <c>true</c> if a label name for a jump target can be suggested at
        ///     <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InJumpStatementLabelNameArea" />
        /// </summary>
        public bool CanSuggestLabelNameJumpTarget
        {
            get { return _autocompleteVisitor.CanSuggestLabelNameJumpTarget; }
        }

        /// <summary>
        ///     <c>true</c> if a label definitions name can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InLabelDefinitionNameArea" />
        /// </summary>
        public bool CanSuggestLabelNameDefinition
        {
            get { return _autocompleteVisitor.CanSuggestLabelNameDefinition; }
        }

        /// <summary>
        ///     <c>true</c> if an LSL type name can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        /// </summary>
        public bool CanSuggestTypeName
        {
            get { return _autocompleteVisitor.CanSuggestTypeName; }
        }

        /// <summary>
        ///     Gets the computed scope address at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        /// </summary>
        public LSLAutoCompleteScopeAddress ScopeAddressAtOffset
        {
            get { return _autocompleteVisitor.ScopeAddressAtOffset; }
        }

        /// <summary>
        ///     Gets the source code range of the code body <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> exists inside of.
        /// </summary>
        public LSLSourceCodeRange CurrentCodeAreaRange
        {
            get { return _autocompleteVisitor.CurrentCodeAreaRange; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is after an 'if' or 'else if' statements code
        ///     body.
        /// </summary>
        public bool AfterIfOrElseIfStatement
        {
            get { return _autocompleteVisitor.AfterIfOrElseIfStatement; }
        }

        /// <summary>
        ///     <c>true</c> if a control statement chain can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InCodeStatementArea" />
        /// </summary>
        public bool CanSuggestControlStatement
        {
            get { return _autocompleteVisitor.CanSuggestControlStatement; }
        }

        /// <summary>
        ///     <c>true</c> if a state change statement can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InCodeStatementArea" />
        /// </summary>
        public bool CanSuggestStateChangeStatement
        {
            get { return _autocompleteVisitor.CanSuggestStateChangeStatement; }
        }

        /// <summary>
        ///     <c>true</c> if a return statement can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InCodeStatementArea" />
        /// </summary>
        public bool CanSuggestReturnStatement
        {
            get { return _autocompleteVisitor.CanSuggestReturnStatement; }
        }

        /// <summary>
        ///     <c>true</c> if a jump statement can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InCodeStatementArea" />
        /// </summary>
        public bool CanSuggestJumpStatement
        {
            get { return _autocompleteVisitor.CanSuggestJumpStatement; }
        }

        /// <summary>
        ///     Gets the return type of the function declaration that <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is
        ///     currently in the code body of.
        /// </summary>
        public LSLType CurrentFunctionReturnType
        {
            get { return _autocompleteVisitor.CurrentFunctionReturnType; }
        }


        /// <summary>
        ///     Get an enumerable of <see cref="LSLAutoCompleteLocalLabel" /> objects representing local labels
        ///     that are currently accessible at <see cref="ParseToOffset" />.
        ///     <param name="sourceCode">The source code of the entire script.</param>
        /// </summary>
        public IEnumerable<LSLAutoCompleteLocalLabel> GetLocalLabels(string sourceCode)
        {
            var len = CurrentCodeAreaRange.StopIndex - CurrentCodeAreaRange.StartIndex;


            var match = LabelRegex.Match(sourceCode, CurrentCodeAreaRange.StartIndex, len);

            var names = new HashSet<string>();

            while (match.Success)
            {
                var name = match.Groups[1].ToString();

                match = match.NextMatch();

                if (names.Contains(name))
                {
                    continue;
                }
                names.Add(name);
                yield return new LSLAutoCompleteLocalLabel(name);
            }
        }


        /// <summary>
        ///     Get an enumerable of <see cref="LSLAutoCompleteLocalJump" /> objects representing local jump statements
        ///     that are currently accessible at <see cref="ParseToOffset" />.
        ///     <param name="sourceCode">The source code of the entire script.</param>
        /// </summary>
        public IEnumerable<LSLAutoCompleteLocalJump> GetLocalJumps(string sourceCode)
        {
            var len = CurrentCodeAreaRange.StopIndex - CurrentCodeAreaRange.StartIndex;


            var match = JumpRegex.Match(sourceCode, CurrentCodeAreaRange.StartIndex, len);

            var names = new HashSet<string>();

            while (match.Success)
            {
                var name = match.Groups[1].ToString();

                match = match.NextMatch();

                if (names.Contains(name))
                {
                    continue;
                }
                names.Add(name);
                yield return new LSLAutoCompleteLocalJump(name);
            }
        }


        /// <summary>
        ///     Preforms an auto-complete parse on the specified stream of LSL source code, up to an arbitrary offset.
        /// </summary>
        /// <param name="stream">The input source code stream.</param>
        /// <param name="toOffset">To offset to parse up to (the cursor offset).</param>
        public void Parse(TextReader stream, int toOffset)
        {
            var inputStream = new AntlrInputStream(stream);

            var lexer = new LSLLexer(inputStream);


            var tokenStream = new CommonTokenStream(lexer);

            var parser = new LSLParser(tokenStream);


            parser.RemoveErrorListeners();
            lexer.RemoveErrorListeners();


            _autocompleteVisitor = new LSLAutoCompleteVisitor(toOffset);

            _autocompleteVisitor.Parse(parser.compilationUnit());
        }
    }
}