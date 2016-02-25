#region FileInfo

// 
// File: LSLAutoCompleteVisitor.cs
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
using System.Runtime.Serialization;
using LibLSLCC.CodeValidator;
using LibLSLCC.Collections;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.AutoComplete
{
    internal class LSLAutoCompleteVisitor : LSLBaseVisitor<bool>, ILSLAutoCompleteParserState
    {
        private readonly HashMap<string, LSLAutoCompleteGlobalFunction> _globalFunctions =
            new HashMap<string, LSLAutoCompleteGlobalFunction>();

        private readonly HashMap<string, LSLAutoCompleteGlobalVariable> _globalVariables =
            new HashMap<string, LSLAutoCompleteGlobalVariable>();

        private readonly Stack<List<LSLAutoCompleteGlobalVariable>> _globalVariablesHidden =
            new Stack<List<LSLAutoCompleteGlobalVariable>>();

        private readonly Stack<LastControlChainStatusContainer> _lastControlChainElementStack =
            new Stack<LastControlChainStatusContainer>();

        private readonly Stack<Dictionary<string, LSLAutoCompleteLocalVariable>> _localVariables =
            new Stack<Dictionary<string, LSLAutoCompleteLocalVariable>>();

        private readonly Stack<List<LSLAutoCompleteLocalVariable>> _localVariablesHidden =
            new Stack<List<LSLAutoCompleteLocalVariable>>();

        private readonly Stack<NestableExpressionElementType> _nestableExpressionElementStack =
            new Stack<NestableExpressionElementType>();

        private readonly HashMap<string, LSLAutoCompleteLocalParameter> _parameters =
            new HashMap<string, LSLAutoCompleteLocalParameter>();

        private readonly int _parseToOffset;

        private readonly GenericArray<LSLAutoCompleteStateBlock> _stateBlocks =
            new GenericArray<LSLAutoCompleteStateBlock>();

        private bool _inEventCodeBody;
        private bool _inFunctionCodeBody;
        private bool _inGlobalScope = true;
        private bool _inStateVisit;


        /// <exception cref="ArgumentOutOfRangeException">parseToOffset</exception>
        public LSLAutoCompleteVisitor(int parseToOffset)
        {
            if (parseToOffset < 0)
            {
                throw new ArgumentOutOfRangeException("parseToOffset", "toOffset cannot be less than zero.");
            }

            _parseToOffset = parseToOffset;
        }


        private int ControlStructureNestingDepth { get; set; }

        private bool InStateVisit
        {
            get { return _inStateVisit; }
            set
            {
                _inStateVisit = value;
                if (value) return;

                if (InEventCodeBody) InEventCodeBody = false;
            }
        }

        private bool BetweenControlStatementKeywords
        {
            get
            {
                return InControlStatementSourceRange &&
                       !InTopLevelCodeScope &&
                       !InIfConditionExpression &&
                       !InElseIfConditionExpression &&
                       !InWhileConditionExpression &&
                       !InForLoopClausesArea &&
                       !InDoWhileConditionExpression &&
                       !(InLocalVariableDeclarationExpression || InVariableAssignmentExpression ||
                         InComponentAssignmentExpression ||
                         InIfConditionExpression || InElseIfConditionExpression || InFunctionCallParameterList ||
                         InFunctionReturnExpression ||
                         InForLoopClausesArea || InDoWhileConditionExpression || InWhileConditionExpression ||
                         InListLiteralInitializer || InVectorLiteralInitializer || InRotationLiteralInitializer);
            }
        }

        private bool InMultiStatementCodeScopeTopLevel { get; set; }
        private bool InSingleStatementCodeScopeTopLevel { get; set; }

        private bool InTopLevelCodeScope
        {
            get { return (InMultiStatementCodeScopeTopLevel | InSingleStatementCodeScopeTopLevel); }
        }

        private int CodeAreaId { get; set; }
        private int ScopeId { get; set; }
        private int ScopeLevel { get; set; }
        private int CodeScopeLevel { get; set; }

        /// <summary>
        ///     The offset the <see cref="LSLAutoCompleteParser" /> last parsed to.
        /// </summary>
        public int ParseToOffset
        {
            get { return _parseToOffset; }
        }

        /// <summary>
        ///     The name of the state block <see cref="ParseToOffset" /> resides in.
        ///     <c>null</c> if the parse to offset is outside of a state body.
        /// </summary>
        public string CurrentState { get; private set; }

        /// <summary>
        ///     The name of the function declaration <see cref="ParseToOffset" /> resides in.
        ///     <c>null</c> if the parse to offset is outside of a function body.
        /// </summary>
        public string CurrentFunction { get; private set; }

        /// <summary>
        ///     The name of the event handler declaration <see cref="ParseToOffset" /> offset resides in.
        ///     <c>null</c> if the parse to offset is outside of an event body.
        /// </summary>
        public string CurrentEvent { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a state block, but outside of an event handler
        ///     declaration.
        /// </summary>
        public bool InStateScope
        {
            get { return (InStateVisit && !InEventCodeBody && !InEventSourceRange); }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the source code range of an event handler.
        ///     This includes being within the name or parameter definitions.
        /// </summary>
        public bool InEventSourceRange { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is anywhere inside the code body of an event handler.
        /// </summary>
        public bool InEventCodeBody
        {
            get { return _inEventCodeBody; }
            // ReSharper disable once FunctionComplexityOverflow
            private set
            {
                _inEventCodeBody = value;

                if (value) return;

                if (InEventSourceRange) InEventSourceRange = false;
                if (InControlStatementSourceRange) InControlStatementSourceRange = false;
                if (InForLoopClausesArea) InForLoopClausesArea = false;
                if (InDoWhileConditionExpression) InDoWhileConditionExpression = false;
                if (InWhileConditionExpression) InWhileConditionExpression = false;
                if (InStateChangeStatementStateNameArea) InStateChangeStatementStateNameArea = false;
                if (InJumpStatementLabelNameArea) InJumpStatementLabelNameArea = false;
                if (InLabelDefinitionNameArea) InLabelDefinitionNameArea = false;
                if (InIfConditionExpression) InIfConditionExpression = false;
                if (InElseIfConditionExpression) InElseIfConditionExpression = false;
                if (InLocalVariableDeclarationExpression) InLocalVariableDeclarationExpression = false;
                if (InMultiStatementCodeScopeTopLevel) InMultiStatementCodeScopeTopLevel = false;
                if (InSingleStatementCodeScopeTopLevel) InSingleStatementCodeScopeTopLevel = false;
            }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside anywhere the code body of a function declaration.
        /// </summary>
        public bool InFunctionCodeBody
        {
            get { return _inFunctionCodeBody; }
            // ReSharper disable once FunctionComplexityOverflow
            private set
            {
                _inFunctionCodeBody = value;

                if (value) return;

                if (InControlStatementSourceRange) InControlStatementSourceRange = false;
                if (InForLoopClausesArea) InForLoopClausesArea = false;
                if (InDoWhileConditionExpression) InDoWhileConditionExpression = false;
                if (InWhileConditionExpression) InWhileConditionExpression = false;
                if (InFunctionReturnExpression) InFunctionReturnExpression = false;
                if (InStateChangeStatementStateNameArea) InStateChangeStatementStateNameArea = false;
                if (InJumpStatementLabelNameArea) InJumpStatementLabelNameArea = false;
                if (InLabelDefinitionNameArea) InLabelDefinitionNameArea = false;
                if (InIfConditionExpression) InIfConditionExpression = false;
                if (InElseIfConditionExpression) InElseIfConditionExpression = false;
                if (InLocalVariableDeclarationExpression) InLocalVariableDeclarationExpression = false;
                if (InMultiStatementCodeScopeTopLevel) InMultiStatementCodeScopeTopLevel = false;
                if (InSingleStatementCodeScopeTopLevel) InSingleStatementCodeScopeTopLevel = false;
            }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the global scope.
        /// </summary>
        public bool InGlobalScope
        {
            get { return _inGlobalScope; }
            private set
            {
                _inGlobalScope = value;
                if (value) return;
                if (InFunctionCodeBody) InFunctionCodeBody = false;
                if (InGlobalVariableDeclarationExpression) InGlobalVariableDeclarationExpression = false;
            }
        }

        /// <summary>
        ///     Gets a list of <see cref="LSLAutoCompleteStateBlock" /> objects representing user defined script state blocks.
        /// </summary>
        public IReadOnlyGenericArray<LSLAutoCompleteStateBlock> StateBlocks
        {
            get { return _stateBlocks; }
        }

        /// <summary>
        ///     Gets a <see cref="LSLAutoCompleteStateBlock" /> object representing the scripts default state.
        /// </summary>
        public LSLAutoCompleteStateBlock DefaultState { get; private set; }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteGlobalVariable" /> objects representing global variables
        ///     that are accessible at <see cref="ParseToOffset" />.
        /// </summary>
        public IEnumerable<LSLAutoCompleteGlobalVariable> GlobalVariables
        {
            get { return _globalVariables.Values; }
        }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteLocalVariable" /> objects representing local variables
        ///     that are accessible at <see cref="ParseToOffset" />.
        /// </summary>
        public IEnumerable<LSLAutoCompleteLocalVariable> LocalVariables
        {
            get { return _localVariables.SelectMany(x => x.Values); }
        }

        /// <summary>
        ///     Gets a read only hash map of <see cref="LSLAutoCompleteGlobalFunction" /> objects representing global function
        ///     declarations
        ///     that are accessible at <see cref="ParseToOffset" />.  The functions are keyed in the hash map by name.
        /// </summary>
        public IReadOnlyHashMap<string, LSLAutoCompleteGlobalFunction> GlobalFunctionsDictionary
        {
            get { return _globalFunctions; }
        }

        /// <summary>
        ///     Gets a read only hash map of <see cref="LSLAutoCompleteGlobalVariable" /> objects representing global variable
        ///     declarations
        ///     that are accessible at <see cref="ParseToOffset" />.  The declarations are keyed in the hash map by name.
        /// </summary>
        public IReadOnlyHashMap<string, LSLAutoCompleteGlobalVariable> GlobalVariablesDictionary
        {
            get { return _globalVariables; }
        }

        /// <summary>
        ///     Gets a read only hash map of <see cref="LSLAutoCompleteLocalParameter" /> objects representing local parameter
        ///     declarations
        ///     that are accessible at <see cref="ParseToOffset" />.  The declarations are keyed in the hash map by name.
        /// </summary>
        public IReadOnlyHashMap<string, LSLAutoCompleteLocalParameter> LocalParametersDictionary
        {
            get { return _parameters; }
        }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteGlobalFunction" /> objects representing global functions
        ///     that are accessible at <see cref="ParseToOffset" />.
        /// </summary>
        public IEnumerable<LSLAutoCompleteGlobalFunction> GlobalFunctions
        {
            get { return _globalFunctions.Values; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in an area where a code statement can exist.
        ///     (<see cref="InMultiCodeStatementArea" /> || <see cref="InBracelessCodeStatementArea" />)
        /// </summary>
        public bool InCodeStatementArea
        {
            get
            {
                return InCodeBody &&
                       InTopLevelCodeScope &&
                       !InExpressionArea &&
                       !InJumpStatementLabelNameArea &&
                       !InStateChangeStatementStateNameArea &&
                       !InLabelDefinitionNameArea &&
                       !BetweenControlStatementKeywords;
            }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in a multi statement area where a code statement can exist.
        /// </summary>
        public bool InMultiCodeStatementArea
        {
            get
            {
                return InCodeBody &&
                       InMultiStatementCodeScopeTopLevel &&
                       !InExpressionArea &&
                       !InJumpStatementLabelNameArea &&
                       !InStateChangeStatementStateNameArea &&
                       !InLabelDefinitionNameArea &&
                       !BetweenControlStatementKeywords;
            }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in an area where a brace-less code scope (single statement) can
        ///     exist.
        /// </summary>
        public bool InBracelessCodeStatementArea
        {
            get
            {
                return InCodeBody &&
                       InSingleStatementCodeScopeTopLevel &&
                       !InExpressionArea &&
                       !InJumpStatementLabelNameArea &&
                       !InStateChangeStatementStateNameArea &&
                       !InLabelDefinitionNameArea &&
                       !BetweenControlStatementKeywords;
            }
        }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteGlobalFunction" /> objects representing local parameters
        ///     that are accessible at <see cref="ParseToOffset" />.
        /// </summary>
        public IEnumerable<LSLAutoCompleteLocalParameter> LocalParameters
        {
            get { return _parameters.Values; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a binary expression/prefix expression/postfix expression
        ///     or parenthesized expression.
        /// </summary>
        public bool InBasicExpressionTree { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is to the right of the dot in a dot member accessor expression.
        /// </summary>
        public bool RightOfDotAccessor { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of the expression used to declare a local variable.
        /// </summary>
        public bool InLocalVariableDeclarationExpression { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of the expression used to declare a global variable.
        /// </summary>
        public bool InGlobalVariableDeclarationExpression { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a function declarations parameter declaration list.
        /// </summary>
        public bool InFunctionDeclarationParameterList { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of an event declarations parameter declaration list.
        /// </summary>
        public bool InEventDeclarationParameterList { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of an 'if' statements condition expression area.
        /// </summary>
        public bool InIfConditionExpression { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of an 'else if' statements condition expression area.
        /// </summary>
        public bool InElseIfConditionExpression { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is anywhere inside of a function declaration or event declaration code
        ///     body.
        /// </summary>
        public bool InCodeBody
        {
            get { return InFunctionCodeBody || InEventCodeBody; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in a local expression area, such as a condition area or function
        ///     call arguments. etc..
        /// </summary>
        public bool InLocalExpressionArea
        {
            get
            {
                return (InLocalVariableDeclarationExpression || InIfConditionExpression ||
                        InElseIfConditionExpression || InFunctionCallParameterList || InFunctionReturnExpression ||
                        InForLoopClausesArea || InDoWhileConditionExpression || InWhileConditionExpression ||
                        InListLiteralInitializer || InVectorLiteralInitializer || InRotationLiteralInitializer ||
                        InVariableAssignmentExpression ||
                        InComponentAssignmentExpression || InBasicExpressionTree) &&
                       !BetweenControlStatementKeywords;
            }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in a global area. currently only when
        ///     <see cref="InGlobalVariableDeclarationExpression" /> is <c>true</c>.
        /// </summary>
        public bool InGlobalExpressionArea
        {
            get { return InGlobalVariableDeclarationExpression; }
        }

        /// <summary>
        ///     <see cref="InGlobalExpressionArea" /> || <see cref="InLocalExpressionArea" />.
        ///     If the offset is in a global variable declaration expression, or the start of one.  Or
        ///     a local expression area such as an expression statement, loop condition, function call parameters, for loop clauses
        ///     etc..
        /// </summary>
        public bool InExpressionArea
        {
            get { return InGlobalExpressionArea || InLocalExpressionArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the expression area of a return statement inside of a function.
        /// </summary>
        public bool InFunctionReturnExpression { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the expression area right of a compound operation/assignment to a
        ///     variable, such as after var += (here).
        /// </summary>
        public bool InModifyingVariableAssignmentExpression { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the expression area right of a compound operation/assignment to a
        ///     member of a variable, such as after var.x += (here).
        /// </summary>
        public bool InModifyingComponentAssignmentExpression { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the expression area right of an assignment to a variable, such as
        ///     after var = (here).
        /// </summary>
        public bool InPlainVariableAssignmentExpression { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the expression area right of an assignment to a member of a
        ///     variable, such as after var.x = (here).
        /// </summary>
        public bool InPlainComponentAssignmentExpression { get; private set; }

        /// <summary>
        ///     <see cref="InPlainVariableAssignmentExpression" /> || <see cref="InModifyingVariableAssignmentExpression" />
        /// </summary>
        public bool InVariableAssignmentExpression
        {
            get { return InPlainVariableAssignmentExpression || InModifyingVariableAssignmentExpression; }
        }

        /// <summary>
        ///     <see cref="InPlainComponentAssignmentExpression" /> || <see cref="InModifyingComponentAssignmentExpression" />
        /// </summary>
        public bool InComponentAssignmentExpression
        {
            get { return InPlainComponentAssignmentExpression || InModifyingComponentAssignmentExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in an area where you could start typing the name of the state in a
        ///     state change statement.
        /// </summary>
        public bool InStateChangeStatementStateNameArea { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in an area where you could start typing the name of the label in a
        ///     jump statement.
        /// </summary>
        public bool InJumpStatementLabelNameArea { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in an area where you could start typing the name of a label.
        /// </summary>
        public bool InLabelDefinitionNameArea { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is anywhere in a for loops clauses area.
        /// </summary>
        public bool InForLoopClausesArea { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in a do while statements condition area.
        /// </summary>
        public bool InDoWhileConditionExpression { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in a while statements condition area.
        /// </summary>
        public bool InWhileConditionExpression { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the source code range of a control statement.
        /// </summary>
        public bool InControlStatementSourceRange { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a function calls parameter expression list.
        /// </summary>
        public bool InFunctionCallParameterList
        {
            get
            {
                return _nestableExpressionElementStack.Count > 0 &&
                       _nestableExpressionElementStack.Peek() ==
                       NestableExpressionElementType.FunctionCallParameterList;
            }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a list literals initializer expression list.
        /// </summary>
        public bool InListLiteralInitializer
        {
            get
            {
                return _nestableExpressionElementStack.Count > 0 &&
                       _nestableExpressionElementStack.Peek() == NestableExpressionElementType.List;
            }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a vector literals initializer expression list.
        /// </summary>
        public bool InVectorLiteralInitializer
        {
            get
            {
                return _nestableExpressionElementStack.Count > 0 &&
                       _nestableExpressionElementStack.Peek() == NestableExpressionElementType.Vector;
            }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a rotation literals initializer expression list.
        /// </summary>
        public bool InRotationLiteralInitializer
        {
            get
            {
                return _nestableExpressionElementStack.Count > 0 &&
                       _nestableExpressionElementStack.Peek() == NestableExpressionElementType.Rotation;
            }
        }

        /// <summary>
        ///     <c>true</c> if a library constant can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InExpressionArea" />
        /// </summary>
        public bool CanSuggestLibraryConstant
        {
            get { return InExpressionArea; }
        }

        /// <summary>
        ///     <c>true</c> if a function call can be suggested at <see cref="ParseToOffset" />.
        ///     (<see cref="InLocalExpressionArea" /> || <see cref="InCodeStatementArea" />)
        /// </summary>
        public bool CanSuggestFunction
        {
            get { return InLocalExpressionArea || InCodeStatementArea; }
        }

        /// <summary>
        ///     <c>true</c> if a local variable or parameter name can be suggested at <see cref="ParseToOffset" />.
        ///     (<see cref="InLocalExpressionArea" /> || <see cref="InCodeStatementArea" />)
        /// </summary>
        public bool CanSuggestLocalVariableOrParameter
        {
            get { return InLocalExpressionArea || InCodeStatementArea; }
        }

        /// <summary>
        ///     <c>true</c> if a global variable can be suggested at <see cref="ParseToOffset" />.
        ///     (<see cref="InLocalExpressionArea" /> || <see cref="InCodeStatementArea" />)
        /// </summary>
        public bool CanSuggestGlobalVariable
        {
            get { return InExpressionArea || InCodeStatementArea; }
        }

        /// <summary>
        ///     <c>true</c> if an event handler can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InStateScope" />
        /// </summary>
        public bool CanSuggestEventHandler
        {
            get { return InStateScope; }
        }

        /// <summary>
        ///     <c>true</c> if a state name can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InStateChangeStatementStateNameArea" />
        /// </summary>
        public bool CanSuggestStateName
        {
            get { return InStateChangeStatementStateNameArea; }
        }

        /// <summary>
        ///     <c>true</c> if a label name for a jump target can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InJumpStatementLabelNameArea" />
        /// </summary>
        public bool CanSuggestLabelNameJumpTarget
        {
            get { return InJumpStatementLabelNameArea; }
        }

        /// <summary>
        ///     <c>true</c> if a label definitions name can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InLabelDefinitionNameArea" />
        /// </summary>
        public bool CanSuggestLabelNameDefinition
        {
            get { return InLabelDefinitionNameArea; }
        }

        /// <summary>
        ///     <c>true</c> if an LSL type name can be suggested at <see cref="ParseToOffset" />.
        /// </summary>
        public bool CanSuggestTypeName
        {
            get
            {
                return (InGlobalScope
                        || InMultiCodeStatementArea
                        || InEventDeclarationParameterList
                        || InFunctionDeclarationParameterList);
            }
        }

        /// <summary>
        ///     Gets the computed scope address at <see cref="ParseToOffset" />.
        /// </summary>
        public LSLAutoCompleteScopeAddress ScopeAddressAtOffset { get; private set; }

        /// <summary>
        ///     Gets the source code range of the code body <see cref="ParseToOffset" /> exists inside of.
        /// </summary>
        public LSLSourceCodeRange CurrentCodeAreaRange { get; private set; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is after an 'if' or 'else if' statements code body.
        /// </summary>
        public bool AfterIfOrElseIfStatement
        {
            get
            {
                return !InSingleStatementCodeScopeTopLevel && _lastControlChainElementStack.Count > 0 &&
                       _lastControlChainElementStack.Peek().IsIfOrElseIf;
            }
        }

        /// <summary>
        ///     <c>true</c> if a control statement chain can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InCodeStatementArea" />
        /// </summary>
        public bool CanSuggestControlStatement
        {
            get { return InCodeStatementArea; }
        }

        /// <summary>
        ///     <c>true</c> if a state change statement can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InCodeStatementArea" />
        /// </summary>
        public bool CanSuggestStateChangeStatement
        {
            get { return InCodeStatementArea; }
        }

        /// <summary>
        ///     <c>true</c> if a return statement can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InCodeStatementArea" />
        /// </summary>
        public bool CanSuggestReturnStatement
        {
            get { return InCodeStatementArea; }
        }

        /// <summary>
        ///     <c>true</c> if a jump statement can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InCodeStatementArea" />
        /// </summary>
        public bool CanSuggestJumpStatement
        {
            get { return InCodeStatementArea; }
        }

        /// <summary>
        ///     Gets the return type of the function declaration that <see cref="ParseToOffset" /> is currently in the code body
        ///     of.
        /// </summary>
        public LSLType CurrentFunctionReturnType { get; private set; }


        private static T SafeStackPeek<T>(Stack<T> stack)
        {
            if (stack.Count == 0)
            {
                throw new StackImbalanceException();
            }
            return stack.Peek();
        }


        private static T SafeStackPop<T>(Stack<T> stack)
        {
            if (stack.Count == 0)
            {
                throw new StackImbalanceException();
            }
            return stack.Pop();
        }


        /// <exception cref="ArgumentNullException"><paramref name="context" /> is <c>null</c>.</exception>
        public void Parse(LSLParser.CompilationUnitContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            try
            {
                VisitCompilationUnit(context);
            }
            catch (StackImbalanceException)
            {
            }

            ScopeAddressAtOffset = new LSLAutoCompleteScopeAddress(CodeAreaId, ScopeId, ScopeLevel);
        }


        public override bool VisitLabelStatement(LSLParser.LabelStatementContext context)
        {
            if (context.Start.StartIndex > _parseToOffset) return true;
            if (context.label_prefix == null) return true;

            if ((_parseToOffset > context.label_prefix.StartIndex &&
                 _parseToOffset <= context.Stop.StopIndex) ||
                (context.label_prefix.StopIndex == context.Stop.StopIndex))
            {
                InLabelDefinitionNameArea = true;
            }

            return base.VisitLabelStatement(context);
        }


        public override bool VisitJumpStatement(LSLParser.JumpStatementContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;
            if (context.jump_keyword == null) return true;

            if ((_parseToOffset > context.jump_keyword.StopIndex &&
                 _parseToOffset <= context.Stop.StopIndex) ||
                (context.jump_keyword.StopIndex == context.Stop.StopIndex))
            {
                InJumpStatementLabelNameArea = true;
            }

            return base.VisitJumpStatement(context);
        }


        public override bool VisitStateChangeStatement(LSLParser.StateChangeStatementContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;
            if (context.state_keyword == null) return true;


            if ((_parseToOffset > context.state_keyword.StopIndex &&
                 _parseToOffset <= context.Stop.StopIndex) ||
                (context.state_keyword.StopIndex == context.Stop.StopIndex))
            {
                InStateChangeStatementStateNameArea = true;
            }

            return base.VisitStateChangeStatement(context);
        }


        public override bool VisitListLiteral(LSLParser.ListLiteralContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;

            _nestableExpressionElementStack.Push(NestableExpressionElementType.List);

            base.VisitListLiteral(context);

            if (context.Stop.Text != "]") return true;

            if (context.Stop.StartIndex >= _parseToOffset) return true;

            SafeStackPop(_nestableExpressionElementStack);


            return true;
        }


        public override bool VisitVectorLiteral(LSLParser.VectorLiteralContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;


            _nestableExpressionElementStack.Push(NestableExpressionElementType.Vector);


            base.VisitVectorLiteral(context);

            if (context.Stop.Text != ">") return true;

            if (context.Stop.StartIndex >= _parseToOffset) return true;

            SafeStackPop(_nestableExpressionElementStack);


            return true;
        }


        public override bool VisitRotationLiteral(LSLParser.RotationLiteralContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;

            _nestableExpressionElementStack.Push(NestableExpressionElementType.Rotation);


            base.VisitRotationLiteral(context);

            if (context.Stop.Text != ">") return true;
            if (context.Stop.StartIndex >= _parseToOffset) return true;


            SafeStackPop(_nestableExpressionElementStack);

            return true;
        }


        public override bool VisitGlobalVariableDeclaration(LSLParser.GlobalVariableDeclarationContext context)
        {
            if (context.variable_name == null || context.variable_type == null) return true;


            if (context.Start.StartIndex <= _parseToOffset)
            {
                InGlobalScope = false;
            }


            var variable =
                new LSLAutoCompleteGlobalVariable(
                    context.variable_name.Text,
                    context.variable_type.Text,
                    new LSLSourceCodeRange(context), new LSLSourceCodeRange(context.variable_type),
                    new LSLSourceCodeRange(context.variable_name));


            if (!_globalVariables.ContainsKey(context.variable_name.Text))
            {
                _globalVariables.Add(context.variable_name.Text, variable);
            }


            if (context.Start.StartIndex <= _parseToOffset)
            {
                if (context.operation != null && context.operation.StartIndex < _parseToOffset)
                {
                    InGlobalVariableDeclarationExpression = true;
                }


                if (context.semi_colon != null && context.semi_colon.Text == ";" &&
                    context.semi_colon.StartIndex < _parseToOffset)
                {
                    _nestableExpressionElementStack.Clear();
                    InGlobalVariableDeclarationExpression = false;
                    InGlobalScope = true;
                }
            }


            return base.VisitGlobalVariableDeclaration(context);
        }


        public override bool VisitExpr_Assignment(LSLParser.Expr_AssignmentContext context)
        {
            if (context.operation != null && _parseToOffset >= context.operation.StopIndex &&
                ((_parseToOffset <= context.Stop.StopIndex) ||
                 (context.operation.StopIndex == context.Stop.StopIndex)))
            {
                if (context.expr_lvalue == null) return true;

                var variable = context.expr_lvalue.variable;
                var accessor = context.expr_lvalue.dotAccessorExpr();

                if (variable != null)
                {
                    if (_parameters.ContainsKey(variable.Text) ||
                        _localVariables.Any(x => x.ContainsKey(variable.Text)) ||
                        _globalVariables.ContainsKey(variable.Text))
                    {
                        InPlainVariableAssignmentExpression = true;
                        InBasicExpressionTree = true;
                    }
                }
                else if (accessor != null)
                {
                    var maybeVar = accessor.expr_lvalue.Text;

                    if (_parameters.ContainsKey(maybeVar) ||
                        _localVariables.Any(x => x.ContainsKey(maybeVar)) ||
                        _globalVariables.ContainsKey(maybeVar))
                    {
                        InPlainComponentAssignmentExpression = true;
                        InBasicExpressionTree = true;
                    }
                }
                else
                {
                    return base.VisitExpr_Assignment(context);
                }
            }
            return base.VisitExpr_Assignment(context);
        }


        public override bool VisitExpr_ModifyingAssignment(LSLParser.Expr_ModifyingAssignmentContext context)
        {
            if (context.operation != null && _parseToOffset >= context.operation.StopIndex &&
                ((_parseToOffset <= context.Stop.StopIndex) ||
                 (context.operation.StopIndex == context.Stop.StopIndex)))
            {
                if (context.expr_lvalue == null) return true;

                var atom = context.expr_lvalue.variable;
                var accessor = context.expr_lvalue.dotAccessorExpr();

                if (atom != null)
                {
                    if (_parameters.ContainsKey(atom.Text) ||
                        _localVariables.Any(x => x.ContainsKey(atom.Text)) ||
                        _globalVariables.ContainsKey(atom.Text))
                    {
                        InModifyingVariableAssignmentExpression = true;
                        InBasicExpressionTree = true;
                    }
                }
                else if (accessor != null)
                {
                    var maybeVar = accessor.expr_lvalue.Text;

                    if (_parameters.ContainsKey(maybeVar) ||
                        _localVariables.Any(x => x.ContainsKey(maybeVar)) ||
                        _globalVariables.ContainsKey(maybeVar))
                    {
                        InModifyingComponentAssignmentExpression = true;
                        InBasicExpressionTree = true;
                    }
                }
                else
                {
                    return base.VisitExpr_ModifyingAssignment(context);
                }
            }
            return base.VisitExpr_ModifyingAssignment(context);
        }


        public override bool VisitReturnStatement(LSLParser.ReturnStatementContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;

            if (InFunctionCodeBody && context.return_expression != null && context.return_keyword != null)
            {
                if (_parseToOffset > context.return_keyword.StopIndex &&
                    ((_parseToOffset <= context.Stop.StopIndex) || context.Stop.Text == "return"))
                {
                    InFunctionReturnExpression = true;
                }
            }
            else if (
                (context.return_keyword == null || context.return_keyword.StopIndex < _parseToOffset)
                && context.return_expression == null &&
                (_parseToOffset < context.Stop.StartIndex || context.Stop.Text != ";"))
            {
                InFunctionReturnExpression = InFunctionCodeBody &&
                                             CurrentFunctionReturnType != LSLType.Void;
            }

            return base.VisitReturnStatement(context);
        }


        private bool VisitElseIfStatement(LSLParser.ControlStructureContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;


            if (context.open_parenth != null && context.open_parenth.Text == "(" &&
                context.open_parenth.StartIndex <= _parseToOffset)
            {
                InElseIfConditionExpression = true;
            }

            if (context.open_parenth != null &&
                context.close_parenth != null &&
                context.close_parenth.Text == ")" &&
                _parseToOffset >= context.close_parenth.StartIndex)
            {
                if (context.open_parenth.StartIndex != context.close_parenth.StartIndex)
                {
                    _nestableExpressionElementStack.Clear();
                    InElseIfConditionExpression = false;

                    if (context.code != null && context.code.code_scope != null &&
                        context.code.code_scope.open_brace.Text == "{")
                    {
                        InSingleStatementCodeScopeTopLevel = false;
                    }
                    else
                    {
                        EnterSingleStatementCodeScope();
                    }
                }
            }


            SafeStackPeek(_lastControlChainElementStack).IsIfOrElseIf = true;

            if (context.condition != null)
            {
                Visit(context.condition);
            }


            if (context.close_parenth == null || context.close_parenth.TokenIndex == -1) return true;

            Visit(context.code);

            if (context.code == null ||
                context.code.exception != null) return true;


            if (context.code.code_scope != null && context.close_parenth.StartIndex <= _parseToOffset &&
                context.code.Start.StartIndex > ParseToOffset)
            {
                InSingleStatementCodeScopeTopLevel = false;
                InMultiStatementCodeScopeTopLevel = false;
                return true;
            }


            if (context.else_statement == null) return true;

            if (context.code.Stop.StopIndex <= _parseToOffset &&
                context.else_statement.Start.StartIndex >= _parseToOffset)
            {
                InMultiStatementCodeScopeTopLevel = false;
                InSingleStatementCodeScopeTopLevel = false;
                return true;
            }

            Visit(context.else_statement);

            return true;
        }


        public override bool VisitElseStatement(LSLParser.ElseStatementContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;

            if (context.code != null && context.code.control_structure != null)
            {
                if (context.code.control_structure.Start.StartIndex < _parseToOffset)
                {
                    return VisitElseIfStatement(context.code.control_structure);
                }

                InMultiStatementCodeScopeTopLevel = false;
                InSingleStatementCodeScopeTopLevel = false;
                return false;
            }

            if (context.code != null &&
                context.code.code_scope != null &&
                context.code.code_scope.open_brace.Text == "{")
            {
                InSingleStatementCodeScopeTopLevel = false;
            }
            else
            {
                EnterSingleStatementCodeScope();
            }


            SafeStackPeek(_lastControlChainElementStack).IsIfOrElseIf = false;

            if (context.code == null || context.code.exception != null) return true;

            if (context.code.code_scope != null &&
                context.else_keyword.StopIndex <= _parseToOffset &&
                context.code.Start.StartIndex >= ParseToOffset)
            {
                InSingleStatementCodeScopeTopLevel = false;
                InMultiStatementCodeScopeTopLevel = false;
                return true;
            }

            return base.VisitElseStatement(context);
        }


        public override bool VisitControlStructure(LSLParser.ControlStructureContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;

            ControlStructureNestingDepth++;
            InControlStatementSourceRange = true;

            if (context.open_parenth != null)
            {
                if (context.open_parenth.Text == "(" && context.open_parenth.StartIndex <= _parseToOffset)
                {
                    InIfConditionExpression = true;
                }
            }


            if (context.open_parenth != null && context.close_parenth != null && context.close_parenth.Text == ")" &&
                _parseToOffset >= context.close_parenth.StartIndex)
            {
                if (context.close_parenth.StartIndex != context.open_parenth.StartIndex)
                {
                    _nestableExpressionElementStack.Clear();
                    InIfConditionExpression = false;


                    if (context.code != null && context.code.code_scope != null &&
                        context.code.code_scope.open_brace.Text == "{")
                    {
                        InSingleStatementCodeScopeTopLevel = false;
                    }
                    else
                    {
                        EnterSingleStatementCodeScope();
                    }
                }
            }


            SafeStackPeek(_lastControlChainElementStack).IsIfOrElseIf = true;


            if (context.condition != null)
            {
                Visit(context.condition);
            }

            if (context.close_parenth == null || context.close_parenth.TokenIndex == -1) return true;

            Visit(context.code);

            if (context.code == null ||
                context.code.exception != null) return true;


            if (context.code.code_scope != null && context.close_parenth.StartIndex <= _parseToOffset &&
                context.code.Start.StartIndex > ParseToOffset)
            {
                InSingleStatementCodeScopeTopLevel = false;
                InMultiStatementCodeScopeTopLevel = false;
                return true;
            }

            if (context.else_statement != null)
            {
                if (context.code.Stop.StopIndex <= _parseToOffset
                    && context.else_statement.Start.StartIndex >= _parseToOffset)
                {
                    InMultiStatementCodeScopeTopLevel = false;
                    InSingleStatementCodeScopeTopLevel = false;
                    return true;
                }

                Visit(context.else_statement);
            }


            if (!(context.Stop.Text == "}" || context.Stop.Text == ";")) return true;
            if (context.Stop.StopIndex > _parseToOffset) return true;

            ControlStructureNestingDepth--;

            if (ControlStructureNestingDepth == 0)
            {
                InControlStatementSourceRange = false;
            }

            return true;
        }


        public override bool VisitWhileLoop(LSLParser.WhileLoopContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;


            ControlStructureNestingDepth++;
            InControlStatementSourceRange = true;


            if (context.open_parenth != null)
            {
                if (context.open_parenth.Text == "(" && context.open_parenth.StartIndex <= _parseToOffset)
                {
                    InWhileConditionExpression = true;
                }
            }

            if (context.open_parenth != null && context.close_parenth != null && context.close_parenth.Text == ")" &&
                _parseToOffset >= context.close_parenth.StartIndex)
            {
                if (context.close_parenth.StartIndex != context.open_parenth.StartIndex)
                {
                    _nestableExpressionElementStack.Clear();
                    InWhileConditionExpression = false;


                    if (context.code != null && context.code.code_scope != null
                        && context.code.code_scope.open_brace.Text == "{")
                    {
                        InSingleStatementCodeScopeTopLevel = false;
                    }
                    else
                    {
                        EnterSingleStatementCodeScope();
                    }
                }
            }


            base.VisitWhileLoop(context);

            if (!(context.Stop.Text == "}" || context.Stop.Text == ";")) return true;
            if (context.Stop.StopIndex > _parseToOffset) return true;

            ControlStructureNestingDepth--;
            if (ControlStructureNestingDepth == 0)
            {
                InControlStatementSourceRange = false;
            }
            return true;
        }


        public override bool VisitForLoop(LSLParser.ForLoopContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;


            ControlStructureNestingDepth++;
            InControlStatementSourceRange = true;


            if (context.open_parenth != null)
            {
                if (context.open_parenth.Text == "(" && context.open_parenth.StartIndex <= _parseToOffset)
                {
                    InForLoopClausesArea = true;
                }
            }

            //TODO separate out the clauses into flags, so that the nested element stack cannot get messed up if syntax errors occur
            //in the expressions preceding the one the cursor is in 
            if (context.open_parenth != null && context.close_parenth != null && context.close_parenth.Text == ")" &&
                _parseToOffset >= context.close_parenth.StartIndex)
            {
                if (context.close_parenth.StartIndex != context.open_parenth.StartIndex)
                {
                    _nestableExpressionElementStack.Clear();
                    InForLoopClausesArea = false;

                    if (context.code != null && context.code.code_scope != null &&
                        context.code.code_scope.open_brace.Text == "{")
                    {
                        InSingleStatementCodeScopeTopLevel = false;
                    }
                    else
                    {
                        EnterSingleStatementCodeScope();
                    }
                }
            }


            base.VisitForLoop(context);

            if (!(context.Stop.Text == "}" || context.Stop.Text == ";")) return true;
            if (context.Stop.StopIndex > _parseToOffset) return true;

            ControlStructureNestingDepth--;
            if (ControlStructureNestingDepth == 0)
            {
                InControlStatementSourceRange = false;
            }


            return true;
        }


        public override bool VisitDoLoop(LSLParser.DoLoopContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;


            ControlStructureNestingDepth++;
            InControlStatementSourceRange = true;

            if (context.code != null && context.code.code_scope != null &&
                context.code.code_scope.open_brace.Text == "{")
            {
                InSingleStatementCodeScopeTopLevel = false;
            }
            else
            {
                EnterSingleStatementCodeScope();
            }


            if (context.open_parenth != null)
            {
                if (context.open_parenth.Text == "(" && context.open_parenth.StartIndex <= _parseToOffset)
                {
                    InDoWhileConditionExpression = true;
                }
            }

            if (context.open_parenth != null && context.close_parenth != null && context.close_parenth.Text == ")" &&
                _parseToOffset >= context.close_parenth.StartIndex)
            {
                if (context.open_parenth.StartIndex != context.close_parenth.StartIndex)
                {
                    _nestableExpressionElementStack.Clear();
                    InDoWhileConditionExpression = false;
                }
            }


            base.VisitDoLoop(context);

            if (context.code != null && context.code.code_scope != null &&
                context.code.code_scope.close_brace != null &&
                context.code.code_scope.close_brace.StartIndex <= _parseToOffset)
            {
                InMultiStatementCodeScopeTopLevel = false;
            }

            if (context.Stop.Text != ";" || context.Stop.StopIndex > _parseToOffset) return true;

            ControlStructureNestingDepth--;
            if (ControlStructureNestingDepth == 0)
            {
                InControlStatementSourceRange = false;
            }
            return true;
        }


        public override bool VisitExpr_FunctionCall(LSLParser.Expr_FunctionCallContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;

            if (context.open_parenth != null && context.open_parenth.Text == "(" &&
                context.open_parenth.StartIndex <= _parseToOffset)
            {
                _nestableExpressionElementStack.Push(NestableExpressionElementType.FunctionCallParameterList);
            }

            base.VisitExpr_FunctionCall(context);

            if (
                context.open_parenth == null ||
                context.close_parenth == null ||
                context.close_parenth.Text != ")" ||
                _parseToOffset < context.close_parenth.StartIndex) return true;

            if (context.expression_list != null && context.expression_list.Stop.Text == ",")
            {
                return true;
            }

            if (context.open_parenth.StartIndex != context.close_parenth.StartIndex)
            {
                SafeStackPop(_nestableExpressionElementStack);
            }


            return true;
        }


        public override bool VisitLocalVariableDeclaration(LSLParser.LocalVariableDeclarationContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;

            if (context.variable_name == null || context.variable_type == null) return true;


            var prevMultiStatementCodeScopeTopLevel = InMultiStatementCodeScopeTopLevel;
            var prevSingleStatementCodeScopeTopLevel = InSingleStatementCodeScopeTopLevel;

            InMultiStatementCodeScopeTopLevel = false;
            InSingleStatementCodeScopeTopLevel = false;

            var variable = new LSLAutoCompleteLocalVariable(
                context.variable_name.Text,
                context.variable_type.Text,
                new LSLSourceCodeRange(context),
                new LSLSourceCodeRange(context.variable_type),
                new LSLSourceCodeRange(context.variable_name),
                new LSLAutoCompleteScopeAddress(CodeAreaId, ScopeId, ScopeLevel));


            var scopeVars = SafeStackPeek(_localVariables);

            LSLAutoCompleteGlobalVariable hiddenGlobalVariable;
            if (_globalVariables.TryGetValue(context.variable_name.Text, out hiddenGlobalVariable))
            {
                SafeStackPeek(_globalVariablesHidden).Add(hiddenGlobalVariable);
                _globalVariables.Remove(context.variable_name.Text);
            }


            LSLAutoCompleteLocalVariable hiddenLocalVariable = null;

            var dict =
                _localVariables.FirstOrDefault(
                    x => x.TryGetValue(context.variable_name.Text, out hiddenLocalVariable));

            if (dict != null)
            {
                SafeStackPeek(_localVariablesHidden).Add(hiddenLocalVariable);
                dict.Remove(context.variable_name.Text);
            }


            if (_parameters.ContainsKey(context.variable_name.Text))
            {
                _parameters.Remove(context.variable_name.Text);
            }

            if (!scopeVars.ContainsKey(context.variable_name.Text))
            {
                scopeVars.Add(
                    context.variable_name.Text,
                    variable);
            }


            if (context.operation != null && context.operation.StartIndex < _parseToOffset)
            {
                InLocalVariableDeclarationExpression = true;
            }


            if (context.semi_colon != null && context.semi_colon.Text == ";" &&
                context.semi_colon.StartIndex < _parseToOffset)
            {
                _nestableExpressionElementStack.Clear();

                InLocalVariableDeclarationExpression = false;
                InMultiStatementCodeScopeTopLevel = prevMultiStatementCodeScopeTopLevel;
                InSingleStatementCodeScopeTopLevel = prevSingleStatementCodeScopeTopLevel;
            }


            return base.VisitLocalVariableDeclaration(context);
        }


        public override bool VisitFunctionDeclaration(LSLParser.FunctionDeclarationContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;

            var returnTypeText = context.return_type == null ? "" : context.return_type.Text;


            var parms = new List<LSLAutoCompleteLocalParameter>();

            if (context.parameters != null && context.parameters.children != null)
            {
                var list = context.parameters.parameterList();
                if (list != null)
                {
                    foreach (var child in list.children)
                    {
                        var i = child as LSLParser.ParameterDefinitionContext;
                        if (i == null) continue;
                        if (i.parameter_name == null || i.parameter_name.Type == -1) continue;
                        if (i.parameter_type == null || i.parameter_type.Type == -1) continue;

                        if (_parameters.ContainsKey(i.parameter_name.Text)) continue;

                        var parm = new LSLAutoCompleteLocalParameter(
                            i.parameter_name.Text,
                            i.parameter_type.Text,
                            new LSLSourceCodeRange(i),
                            new LSLSourceCodeRange(i.parameter_type),
                            new LSLSourceCodeRange(i.parameter_name),
                            new LSLAutoCompleteScopeAddress(CodeAreaId, ScopeId + 1, ScopeLevel + 1));

                        parms.Add(parm);
                        _parameters.Add(parm.Name, parm);
                    }
                }
            }

            if (context.function_name == null || context.function_name.Type == -1) return true;

            if (!_globalFunctions.ContainsKey(context.function_name.Text))
            {
                if (context.return_type != null)
                {
                    _globalFunctions.Add(
                        context.function_name.Text,
                        new LSLAutoCompleteGlobalFunction(context.function_name.Text, returnTypeText,
                            new LSLSourceCodeRange(context), new LSLSourceCodeRange(context.return_type),
                            new LSLSourceCodeRange(context.function_name), parms));
                }
                else
                {
                    _globalFunctions.Add(
                        context.function_name.Text,
                        new LSLAutoCompleteGlobalFunction(context.function_name.Text,
                            new LSLSourceCodeRange(context),
                            new LSLSourceCodeRange(context.function_name), parms));
                }
            }


            if (context.Start.StartIndex >= _parseToOffset) return true;

            InGlobalScope = false;

            if (context.open_parenth != null && context.open_parenth.Text == "(" &&
                context.open_parenth.StartIndex <= _parseToOffset)
            {
                InFunctionDeclarationParameterList = true;
            }

            if (context.open_parenth != null && context.close_parenth != null && context.close_parenth.Text == ")" &&
                _parseToOffset >= context.close_parenth.StartIndex)
            {
                if (context.close_parenth.StartIndex != context.open_parenth.StartIndex)
                {
                    InFunctionDeclarationParameterList = false;
                }
            }


            if (context.code == null || context.code.open_brace == null ||
                context.code.open_brace.Text == "<missing '}'>" ||
                context.code.open_brace.StartIndex > _parseToOffset) return true;


            CurrentCodeAreaRange = new LSLSourceCodeRange(context.code);

            CodeAreaId++;


            InFunctionCodeBody = true;

            CurrentFunction = context.function_name != null ? context.function_name.Text : null;

            CurrentFunctionReturnType = string.IsNullOrEmpty(returnTypeText)
                ? LSLType.Void
                : LSLTypeTools.FromLSLTypeName(returnTypeText);


            base.VisitFunctionDeclaration(context);

            if (context.Stop.StartIndex > _parseToOffset) return true;


            CurrentFunction = null;
            CurrentFunctionReturnType = LSLType.Void;

            _parameters.Clear();

            _nestableExpressionElementStack.Clear();
            InFunctionCodeBody = false;
            InGlobalScope = true;

            return true;
        }


        public override bool VisitEventHandler(LSLParser.EventHandlerContext context)
        {
            if (context.Start.StartIndex >= _parseToOffset) return true;


            InEventSourceRange = true;


            if (context.open_parenth != null && context.open_parenth.Text == "(" &&
                context.open_parenth.StartIndex <= _parseToOffset)
            {
                InEventDeclarationParameterList = true;
            }

            if (context.open_parenth != null && context.close_parenth != null && context.close_parenth.Text == ")" &&
                _parseToOffset >= context.close_parenth.StartIndex)
            {
                if (context.close_parenth.StartIndex != context.open_parenth.StartIndex)
                {
                    InEventDeclarationParameterList = false;
                }
            }


            if (context.code == null || context.code.open_brace == null ||
                context.code.open_brace.Text == "<missing '}'>" ||
                context.code.open_brace.StartIndex > _parseToOffset) return true;

            CodeAreaId++;

            _parameters.Clear();

            if (context.parameters != null && context.parameters.children != null)
            {
                var list = context.parameters.parameterList();
                if (list != null)
                {
                    foreach (var child in list.children)
                    {
                        var i = child as LSLParser.ParameterDefinitionContext;

                        if (i == null) continue;

                        if (_parameters.ContainsKey(i.parameter_name.Text)) continue;

                        var parm = new LSLAutoCompleteLocalParameter(
                            i.parameter_name.Text,
                            i.parameter_type.Text,
                            new LSLSourceCodeRange(i),
                            new LSLSourceCodeRange(i.parameter_type),
                            new LSLSourceCodeRange(i.parameter_name),
                            new LSLAutoCompleteScopeAddress(CodeAreaId, ScopeId + 1, ScopeLevel + 1));


                        _parameters.Add(parm.Name, parm);
                    }
                }
            }


            CurrentCodeAreaRange = new LSLSourceCodeRange(context.code);

            InEventCodeBody = true;

            CurrentEvent = context.handler_name != null ? context.handler_name.Text : null;

            base.VisitEventHandler(context);

            if (context.Stop.StartIndex > _parseToOffset) return true;


            CurrentEvent = null;

            _nestableExpressionElementStack.Clear();
            InEventCodeBody = false;

            InEventSourceRange = false;

            return true;
        }


        public override bool VisitDefaultState(LSLParser.DefaultStateContext context)
        {
            DefaultState = new LSLAutoCompleteStateBlock("default", new LSLSourceCodeRange(context));

            if (_parseToOffset <= context.Start.StartIndex) return true;

            InGlobalScope = false;

            if (context.open_brace == null) return true;

            if (_parseToOffset >= context.Start.StartIndex && context.open_brace.Text != "{")
            {
                return true;
            }

            if (context.open_brace != null && (_parseToOffset < context.open_brace.StartIndex &&
                                               _parseToOffset >= context.Start.StartIndex))
            {
                return true;
            }


            CodeAreaId++;
            ScopeLevel++;
            ScopeId++;


            InStateVisit = true;

            CurrentState = context.state_name != null ? context.state_name.Text : null;

            base.VisitDefaultState(context);


            if (context.Stop.StartIndex > _parseToOffset || context.Stop.Text != "}") return true;

            _nestableExpressionElementStack.Clear();
            InStateVisit = false;


            ScopeLevel--;


            return true;
        }


        public override bool VisitDefinedState(LSLParser.DefinedStateContext context)
        {
            if (context.state_name == null || context.state_name.Type == -1) return true;

            _stateBlocks.Add(new LSLAutoCompleteStateBlock(context.state_name.Text,
                new LSLSourceCodeRange(context)));


            if (_parseToOffset <= context.Start.StartIndex) return true;

            if (context.open_brace == null) return true;

            if (_parseToOffset >= context.Start.StartIndex && context.open_brace.Text != "{")
            {
                return true;
            }

            if (context.open_brace != null && (_parseToOffset < context.open_brace.StartIndex &&
                                               _parseToOffset >= context.Start.StartIndex))
            {
                return true;
            }


            CodeAreaId++;
            ScopeLevel++;
            ScopeId++;


            InStateVisit = true;

            CurrentState = context.state_name != null ? context.state_name.Text : null;

            base.VisitDefinedState(context);

            if (context.Stop.StartIndex > _parseToOffset || context.Stop.Text != "}") return true;

            _nestableExpressionElementStack.Clear();
            InStateVisit = false;

            ScopeLevel--;

            return true;
        }


        private void EnterSingleStatementCodeScope()
        {
            CodeScopeLevel++;
            ScopeLevel++;
            InSingleStatementCodeScopeTopLevel = true;
        }


        public override bool VisitCodeStatement(LSLParser.CodeStatementContext context)
        {
            var singleStatement = context.code_scope == null &&
                                  (context.Parent is LSLParser.ControlStructureContext ||
                                   context.Parent is LSLParser.ElseStatementContext ||
                                   context.Parent is LSLParser.DoLoopContext ||
                                   context.Parent is LSLParser.WhileLoopContext ||
                                   context.Parent is LSLParser.ForLoopContext
                                      );

            if (singleStatement)
            {
                if (_parseToOffset >= context.Start.StartIndex)
                {
                    if (_parseToOffset >= context.Start.StartIndex && context.Stop.Text == ";")
                    {
                        InControlStatementSourceRange = false;

                        CodeScopeLevel--;
                        ScopeLevel--;
                        InSingleStatementCodeScopeTopLevel = false;
                    }
                    else if (_parseToOffset <= context.Start.StartIndex)
                    {
                        InSingleStatementCodeScopeTopLevel = false;
                    }
                }

                base.VisitCodeStatement(context);

                //InSingleStatementCodeScopeTopLevel = prevSign;
            }
            else
            {
                if (context.Start.StartIndex > _parseToOffset) return true;


                var prevMulti = InMultiStatementCodeScopeTopLevel;
                var prevSign = InSingleStatementCodeScopeTopLevel;

                InMultiStatementCodeScopeTopLevel = false;
                InSingleStatementCodeScopeTopLevel = false;


                base.VisitCodeStatement(context);

                if (context.Stop.StopIndex > _parseToOffset || context.Stop.Text != ";") return true;


                _nestableExpressionElementStack.Clear();

                InMultiStatementCodeScopeTopLevel = prevMulti;
                InSingleStatementCodeScopeTopLevel = prevSign;
            }

            return true;
        }


        public override bool VisitCodeScope(LSLParser.CodeScopeContext context)
        {
            if (context.Start.StartIndex > _parseToOffset) return true;


            if (!InSingleStatementCodeScopeTopLevel)
            {
                CodeScopeLevel++;
                ScopeLevel++;
                ScopeId++;
            }


            InSingleStatementCodeScopeTopLevel = false;
            InMultiStatementCodeScopeTopLevel = true;

            _globalVariablesHidden.Push(new List<LSLAutoCompleteGlobalVariable>());
            _localVariablesHidden.Push(new List<LSLAutoCompleteLocalVariable>());


            if (context.Parent is LSLParser.FunctionDeclarationContext ||
                context.Parent is LSLParser.EventHandlerContext)
            {
                foreach (var parameters in LocalParameters)
                {
                    LSLAutoCompleteGlobalVariable val;
                    if (_globalVariables.TryGetValue(parameters.Name, out val))
                    {
                        _globalVariables.Remove(val.Name);
                        SafeStackPeek(_globalVariablesHidden).Add(val);
                    }
                }
            }

            _lastControlChainElementStack.Push(new LastControlChainStatusContainer());
            _localVariables.Push(new Dictionary<string, LSLAutoCompleteLocalVariable>());

            foreach (var i in context.codeStatement())
            {
                if (i.Start.StartIndex > _parseToOffset)
                {
                    return true;
                }

                SafeStackPeek(_lastControlChainElementStack).IsIfOrElseIf = false;
                VisitCodeStatement(i);
            }

            if (context.Stop.StartIndex > _parseToOffset) return true;


            foreach (var variable in SafeStackPeek(_globalVariablesHidden))
            {
                _globalVariables.Add(variable.Name, variable);
            }


            SafeStackPop(_globalVariablesHidden);

            SafeStackPop(_lastControlChainElementStack);

            SafeStackPop(_localVariables);


            if (_localVariables.Count != 0)
            {
                foreach (var variable in SafeStackPeek(_localVariablesHidden))
                {
                    SafeStackPeek(_localVariables).Add(variable.Name, variable);
                }
            }

            SafeStackPop(_localVariablesHidden);


            CodeScopeLevel--;
            if (CodeScopeLevel == 0)
            {
                _nestableExpressionElementStack.Clear();
                InMultiStatementCodeScopeTopLevel = false;
            }

            ScopeLevel--;


            return true;
        }


        public override bool VisitExpr_AddSub(LSLParser.Expr_AddSubContext context)
        {
            if (context.operation.StopIndex <= _parseToOffset &&
                (context.expr_rvalue == null || _parseToOffset <= context.expr_rvalue.Start.StartIndex))
            {
                InBasicExpressionTree = true;
            }

            return base.VisitExpr_AddSub(context);
        }


        public override bool VisitExpr_BitwiseAnd(LSLParser.Expr_BitwiseAndContext context)
        {
            if (context.operation.StopIndex <= _parseToOffset &&
                (context.expr_rvalue == null || _parseToOffset <= context.expr_rvalue.Start.StartIndex))
            {
                InBasicExpressionTree = true;
            }

            return base.VisitExpr_BitwiseAnd(context);
        }


        public override bool VisitExpr_BitwiseOr(LSLParser.Expr_BitwiseOrContext context)
        {
            if (context.operation.StopIndex <= _parseToOffset &&
                (context.expr_rvalue == null || _parseToOffset <= context.expr_rvalue.Start.StartIndex))
            {
                InBasicExpressionTree = true;
            }

            return base.VisitExpr_BitwiseOr(context);
        }


        public override bool VisitExpr_BitwiseShift(LSLParser.Expr_BitwiseShiftContext context)
        {
            if (context.operation.StopIndex <= _parseToOffset &&
                (context.expr_rvalue == null || _parseToOffset <= context.expr_rvalue.Start.StartIndex))
            {
                InBasicExpressionTree = true;
            }

            return base.VisitExpr_BitwiseShift(context);
        }


        public override bool VisitExpr_BitwiseXor(LSLParser.Expr_BitwiseXorContext context)
        {
            if (context.operation.StopIndex <= _parseToOffset &&
                (context.expr_rvalue == null || _parseToOffset <= context.expr_rvalue.Start.StartIndex))
            {
                InBasicExpressionTree = true;
            }

            return base.VisitExpr_BitwiseXor(context);
        }


        public override bool VisitExpr_Logical_And_Or(LSLParser.Expr_Logical_And_OrContext context)
        {
            if (context.operation.StopIndex <= _parseToOffset &&
                (context.expr_rvalue == null || _parseToOffset <= context.expr_rvalue.Start.StartIndex))
            {
                InBasicExpressionTree = true;
            }

            return base.VisitExpr_Logical_And_Or(context);
        }


        public override bool VisitExpr_LogicalCompare(LSLParser.Expr_LogicalCompareContext context)
        {
            if (context.operation.StopIndex <= _parseToOffset &&
                (context.expr_rvalue == null || _parseToOffset <= context.expr_rvalue.Start.StartIndex))
            {
                InBasicExpressionTree = true;
            }
            return base.VisitExpr_LogicalCompare(context);
        }


        public override bool VisitExpr_LogicalEquality(LSLParser.Expr_LogicalEqualityContext context)
        {
            if (context.operation.StopIndex <= _parseToOffset &&
                (context.expr_rvalue == null || _parseToOffset <= context.expr_rvalue.Start.StartIndex))
            {
                InBasicExpressionTree = true;
            }
            return base.VisitExpr_LogicalEquality(context);
        }


        public override bool VisitExpr_MultDivMod(LSLParser.Expr_MultDivModContext context)
        {
            if (context.operation.StopIndex <= _parseToOffset &&
                (context.expr_rvalue == null || _parseToOffset <= context.expr_rvalue.Start.StartIndex))
            {
                InBasicExpressionTree = true;
            }
            return base.VisitExpr_MultDivMod(context);
        }


        public override bool VisitExpr_PrefixOperation(LSLParser.Expr_PrefixOperationContext context)
        {
            if (context.operation.StopIndex <= _parseToOffset &&
                (context.expr_rvalue == null || _parseToOffset <= context.expr_rvalue.Start.StartIndex))
            {
                InBasicExpressionTree = true;
            }
            return base.VisitExpr_PrefixOperation(context);
        }


        public override bool VisitExpr_TypeCast(LSLParser.Expr_TypeCastContext context)
        {
            if (context.close_parenth != null && context.close_parenth.StartIndex <= _parseToOffset &&
                (context.expr_rvalue == null || _parseToOffset <= context.expr_rvalue.Start.StartIndex))
            {
                InBasicExpressionTree = true;
            }

            return base.VisitExpr_TypeCast(context);
        }


        public override bool VisitParenthesizedExpression(LSLParser.ParenthesizedExpressionContext context)
        {
            if (context.open_parenth != null && context.open_parenth.StartIndex <= _parseToOffset &&
                context.close_parenth != null && context.close_parenth.Text == ")" &&
                context.close_parenth.StartIndex >= _parseToOffset)
            {
                InBasicExpressionTree = true;
            }


            return base.VisitParenthesizedExpression(context);
        }


        public override bool VisitDotAccessorExpr(LSLParser.DotAccessorExprContext context)
        {
            if (context.operation.StopIndex <= _parseToOffset &&
                (context.member == null || _parseToOffset <= context.member.StartIndex))
            {
                RightOfDotAccessor = true;
            }

            return base.VisitDotAccessorExpr(context);
        }


        [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
        [Serializable]
        private class StackImbalanceException : Exception
        {
            //
            // For guidelines regarding the creation of new exception types, see
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
            // and
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
            //

            public StackImbalanceException()
            {
            }


            public StackImbalanceException(string message) : base(message)
            {
            }


            public StackImbalanceException(string message, Exception inner) : base(message, inner)
            {
            }


            protected StackImbalanceException(
                SerializationInfo info,
                StreamingContext context) : base(info, context)
            {
            }
        }

        private class LastControlChainStatusContainer
        {
            public bool IsIfOrElseIf;
        }

        private enum NestableExpressionElementType
        {
            Vector,
            Rotation,
            List,
            FunctionCallParameterList
        }
    };
}