#region FileInfo

// 
// File: LSLAutoCompleteParser.cs
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Primitives;

#endregion

namespace LibLSLCC.AutoCompleteParser
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class LSLAutoCompleteParser
    {
        private readonly Dictionary<string, GlobalFunction> _globalFunctions = new Dictionary<string, GlobalFunction>();
        private readonly Dictionary<string, GlobalVariable> _globalVariables = new Dictionary<string, GlobalVariable>();
        private readonly Regex _jumpRegex = new Regex("jump\\s*(" + LSLLexer.IDRegex + ")");
        private readonly Regex _labelRegex = new Regex("@\\s*(" + LSLLexer.IDRegex + ")");

        private readonly Stack<Dictionary<string, LocalVariable>> _localVariables =
            new Stack<Dictionary<string, LocalVariable>>();

        private readonly Dictionary<string, LocalParameter> _parameters = new Dictionary<string, LocalParameter>();
        private readonly List<StateBlock> _stateBlocks = new List<StateBlock>();

        protected readonly Stack<NestableExpressionElementType> NestableExpressionElementStack =
            new Stack<NestableExpressionElementType>();

        private bool _inEventCodeBody;
        private bool _inFunctionCodeBody;
        private bool _inGlobalScope;
        private bool _inState;
        private int _toOffset;

        public LSLAutoCompleteParser()
        {
            InGlobalScope = true;
        }

        public string CurrentState { get; private set; }
        public string CurrentFunction { get; private set; }
        public string CurrentEvent { get; private set; }

        public bool InStateOutsideEvent
        {
            get { return (InState && !InEventCodeBody && !InEventSourceRange); }
        }

        public bool InState
        {
            get { return _inState; }
            private set
            {
                _inState = value;
                if (value) return;

                if (InEventCodeBody) InEventCodeBody = false;
            }
        }

        public bool InEventSourceRange { get; private set; }

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

        public IEnumerable<StateBlock> StateBlocks
        {
            get { return _stateBlocks; }
        }

        public StateBlock DefaultState { get; private set; }

        public IEnumerable<GlobalVariable> GlobalVariables
        {
            get { return _globalVariables.Values; }
        }

        public IEnumerable<LocalVariable> LocalVariables
        {
            get { return _localVariables.SelectMany(x => x.Values); }
        }

        public IDictionary<string, GlobalFunction> GlobalFunctionsDictionary
        {
            get { return _globalFunctions; }
        }

        public IDictionary<string, GlobalVariable> GlobalVariablesDictionary
        {
            get { return _globalVariables; }
        }

        public IDictionary<string, LocalParameter> LocalParametersDictionary
        {
            get { return _parameters; }
        }

        public IEnumerable<GlobalFunction> GlobalFunctions
        {
            get { return _globalFunctions.Values; }
        }

        public IEnumerable<LocalParameter> LocalParameters
        {
            get { return _parameters.Values; }
        }

        public bool InLocalVariableDeclarationExpression { get; private set; }
        public bool InGlobalVariableDeclarationExpression { get; private set; }
        public bool InFunctionDeclarationParameterList { get; private set; }
        public bool InEventParameterList { get; private set; }
        public bool InIfConditionExpression { get; private set; }
        public bool InElseIfConditionExpression { get; private set; }

        public bool InCodeBody
        {
            get { return InFunctionCodeBody || InEventCodeBody; }
        }

        public bool InExpressionStatementArea
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

        public bool InTopLevelCodeScope
        {
            get { return InMultiStatementCodeScopeTopLevel | InSingleStatementCodeScopeTopLevel; }
        }

        public bool BetweenControlStatementKeywords
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
                       !(InLocalVariableDeclarationExpression || InIfConditionExpression ||
                       InElseIfConditionExpression || InFunctionCallParameterList || InFunctionReturnExpression ||
                       InForLoopClausesArea || InDoWhileConditionExpression || InWhileConditionExpression ||
                       InListLiteralContent || InVectorLiteralContent || InRotationLiteralContent);
            }
        }

        public bool InLocalExpressionArea
        {
            get
            {
                return (InLocalVariableDeclarationExpression || InIfConditionExpression ||
                       InElseIfConditionExpression || InFunctionCallParameterList || InFunctionReturnExpression ||
                       InForLoopClausesArea || InDoWhileConditionExpression || InWhileConditionExpression ||
                       InListLiteralContent || InVectorLiteralContent || InRotationLiteralContent) &&
                       !BetweenControlStatementKeywords;
            }
        }

        /// <summary>
        ///     Only true if InGlobalVariableDeclarationExpression is true
        /// </summary>
        public bool InGlobalExpressionArea
        {
            get { return InGlobalVariableDeclarationExpression; }
        }

        /// <summary>
        ///     InGlobalExpressionArea || InLocalExpressionArea
        ///     If the offset is in a global variable declaration expression, or the start of one.  Or
        ///     a local expression area such as an expression statement, loop condition, function call parameters, for loop clauses
        ///     ect.
        /// </summary>
        public bool InExpressionArea
        {
            get { return InGlobalExpressionArea || InLocalExpressionArea; }
        }

        public bool InFunctionReturnExpression { get; private set; }
        public bool InPlainAssignmentRightExpression { get; private set; }

        public bool InAssignmentRightExpression
        {
            get { return InPlainAssignmentRightExpression || InModifyingAssignmentRightExpression; }
        }

        public bool InModifyingAssignmentRightExpression { get; private set; }
        public bool InStateChangeStatementStateNameArea { get; private set; }
        public bool InJumpStatementLabelNameArea { get; private set; }
        public bool InLabelDefinitionNameArea { get; private set; }
        public bool InForLoopClausesArea { get; private set; }
        public bool InDoWhileConditionExpression { get; private set; }
        public bool InWhileConditionExpression { get; private set; }
        public bool InControlStatementSourceRange { get; private set; }
        public bool InMultiStatementCodeScopeTopLevel { get; private set; }
        public bool InSingleStatementCodeScopeTopLevel { get; private set; }

        public bool InFunctionCallParameterList
        {
            get
            {
                return NestableExpressionElementStack.Count > 0 &&
                       NestableExpressionElementStack.Peek() == NestableExpressionElementType.FunctionCallParameterList;
            }
        }

        public bool InListLiteralContent
        {
            get
            {
                return NestableExpressionElementStack.Count > 0 &&
                       NestableExpressionElementStack.Peek() == NestableExpressionElementType.List;
            }
        }

        public bool InVectorLiteralContent
        {
            get
            {
                return NestableExpressionElementStack.Count > 0 &&
                       NestableExpressionElementStack.Peek() == NestableExpressionElementType.Vector;
            }
        }

        public bool InRotationLiteralContent
        {
            get
            {
                return NestableExpressionElementStack.Count > 0 &&
                       NestableExpressionElementStack.Peek() == NestableExpressionElementType.Rotation;
            }
        }

        public bool CanSuggestLibraryConstant
        {
            get { return InExpressionArea || InAssignmentRightExpression; }
        }

        public bool CanSuggestFunction
        {
            get { return InLocalExpressionArea || InExpressionStatementArea; }
        }

        public bool CanSuggestLocalVariableOrParameter
        {
            get { return InLocalExpressionArea || InExpressionStatementArea; }
        }

        public bool CanSuggestGlobalVariable
        {
            get { return InExpressionArea || InExpressionStatementArea; }
        }

        public bool CanSuggestEventHandler
        {
            get { return InStateOutsideEvent; }
        }

        public bool CanSuggestStateName
        {
            get { return InStateChangeStatementStateNameArea; }
        }

        public bool CanSuggestLabelNameJumpTarget
        {
            get { return InJumpStatementLabelNameArea; }
        }

        public bool CanSuggestLabelNameDefinition
        {
            get { return InLabelDefinitionNameArea; }
        }

        public bool CanSuggestTypeName
        {
            get
            {
                return (InGlobalScope
                        || InExpressionStatementArea
                        || InEventParameterList
                        || InFunctionDeclarationParameterList /*|| CanSuggestTypeCast*/)
                       && !InSingleStatementCodeScopeTopLevel;
            }
        }

        public ScopeAddress ScopeAddressAtOffset { get; private set; }
        public LSLSourceCodeRange CurrentCodeAreaRange { get; private set; }

        public bool AfterIfOrElseIfStatement
        {
            get;
            set;
        }


        public bool CanSuggestControlStatement
        {
            get { return InTopLevelCodeScope; }
        }

        public IEnumerable<LocalLabel> GetLocalLabels(string sourceCode)
        {
            var len = CurrentCodeAreaRange.StopIndex - CurrentCodeAreaRange.StartIndex;


            var match = _labelRegex.Match(sourceCode, CurrentCodeAreaRange.StartIndex, len);

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
                yield return new LocalLabel(name);
            }
        }

        public IEnumerable<LocalJump> GetLocalJumps(string sourceCode)
        {
            var len = CurrentCodeAreaRange.StopIndex - CurrentCodeAreaRange.StartIndex;


            var match = _jumpRegex.Match(sourceCode, CurrentCodeAreaRange.StartIndex, len);

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
                yield return new LocalJump(name);
            }
        }

        public void Parse(TextReader stream, int toOffset)
        {
            _globalFunctions.Clear();
            _globalVariables.Clear();
            _localVariables.Clear();
            _parameters.Clear();

            _toOffset = toOffset;

            var inputStream = new AntlrInputStream(stream);

            var lexer = new LSLLexer(inputStream);


            var tokenStream = new CommonTokenStream(lexer);

            var parser = new LSLParser(tokenStream);


            parser.RemoveErrorListeners();
            lexer.RemoveErrorListeners();


            var x = new Visitor(this);

            x.Visit(parser.compilationUnit());

            ScopeAddressAtOffset = new ScopeAddress(x.CodeAreaId, x.ScopeId, x.ScopeLevel);
        }

        protected enum NestableExpressionElementType
        {
            Vector,
            Rotation,
            List,
            FunctionCallParameterList
        }

        public class GlobalVariable
        {
            public GlobalVariable(string name, string type, LSLSourceCodeRange range, LSLSourceCodeRange typeRange,
                LSLSourceCodeRange nameRange)
            {
                Name = name;
                Type = type;
                SourceCodeRange = range;

                NameSourceCodeRange = nameRange;

                TypeSourceCodeRange = typeRange;
            }

            public string Name { get; private set; }
            public string Type { get; private set; }
            public LSLSourceCodeRange SourceCodeRange { get; private set; }
            public LSLSourceCodeRange NameSourceCodeRange { get; private set; }
            public LSLSourceCodeRange TypeSourceCodeRange { get; private set; }
        }

        public class StateBlock
        {
            public StateBlock(string name, LSLSourceCodeRange range)
            {
                Name = name;
                SourceCodeRange = range;
            }

            public string Name { get; private set; }
            public LSLSourceCodeRange SourceCodeRange { get; private set; }
        }

        public class GlobalFunction
        {
            public GlobalFunction(string name, string type, LSLSourceCodeRange range, LSLSourceCodeRange typeRange,
                LSLSourceCodeRange nameRange,
                IReadOnlyList<LocalParameter> parameters)
            {
                Parameters = parameters;
                Name = name;
                ReturnType = type;
                SourceCodeRange = range;

                NameSourceCodeRange = nameRange;

                TypeSourceCodeRange = typeRange;

                HasReturnType = true;
            }

            public GlobalFunction(string name, LSLSourceCodeRange range, LSLSourceCodeRange nameRange,
                IReadOnlyList<LocalParameter> parameters)
            {
                Parameters = parameters;
                Name = name;
                ReturnType = "";
                SourceCodeRange = range;

                NameSourceCodeRange = nameRange;

                TypeSourceCodeRange = null;

                HasReturnType = false;
            }

            public bool HasReturnType { get; private set; }
            public LSLSourceCodeRange NameSourceCodeRange { get; private set; }
            public LSLSourceCodeRange TypeSourceCodeRange { get; private set; }
            public string Name { get; }
            public string ReturnType { get; }

            public string Signature
            {
                get
                {
                    var sig = "";
                    if (ReturnType != "")
                    {
                        sig += ReturnType + " ";
                    }

                    sig += Name + "(";

                    if (Parameters.Count > 0)
                    {
                        sig += string.Join(", ", Parameters.Select(x => x.Type + " " + x.Name));
                    }
                    sig += ");";

                    return sig;
                }
            }

            public IReadOnlyList<LocalParameter> Parameters { get; }
            public LSLSourceCodeRange SourceCodeRange { get; private set; }
        }

        public class ScopeAddress
        {
            public ScopeAddress(int codeAreaId, int scopeId, int scopeLevel)
            {
                CodeAreaId = codeAreaId;
                ScopeId = scopeId;
                ScopeLevel = scopeLevel;
            }

            public int CodeAreaId { get; private set; }
            public int ScopeLevel { get; private set; }
            public int ScopeId { get; private set; }
        }

        public class LocalVariable
        {
            public LocalVariable(string name, string type, LSLSourceCodeRange range, LSLSourceCodeRange typeRange,
                LSLSourceCodeRange nameRange, ScopeAddress address)
            {
                Name = name;
                Type = type;
                SourceCodeRange = range;
                ScopeAddress = address;
                NameSourceCodeRange = nameRange;

                TypeSourceCodeRange = TypeSourceCodeRange;
            }

            public LSLSourceCodeRange NameSourceCodeRange { get; private set; }
            public LSLSourceCodeRange TypeSourceCodeRange { get; }
            public ScopeAddress ScopeAddress { get; private set; }
            public string Name { get; private set; }
            public string Type { get; private set; }
            public LSLSourceCodeRange SourceCodeRange { get; private set; }
        }

        public class LocalLabel
        {
            public LocalLabel(string name)
            {
                Name = name;
            }

            public string Name { get; private set; }
        }

        public class LocalJump
        {
            public LocalJump(string target)
            {
                Target = target;
            }

            public string Target { get; private set; }
        }

        public class LocalParameter
        {
            public LocalParameter(string name, string type, LSLSourceCodeRange range, LSLSourceCodeRange typeRange,
                LSLSourceCodeRange nameRange, ScopeAddress address)
            {
                Name = name;
                Type = type;
                SourceCodeRange = range;
                ScopeAddress = address;

                TypeSourceCodeRange = typeRange;
                NameSourceCodeRange = nameRange;
            }

            public LSLSourceCodeRange NameSourceCodeRange { get; private set; }
            public LSLSourceCodeRange TypeSourceCodeRange { get; private set; }
            public ScopeAddress ScopeAddress { get; private set; }
            public string Name { get; }
            public string Type { get; }
            public LSLSourceCodeRange SourceCodeRange { get; private set; }
        }

        private class Visitor : LSLBaseVisitor<bool>
        {
            private readonly LSLAutoCompleteParser _parent;
            private bool _startIfControlChain;
            private int _startIfControlChainCodeScope = -1;

            public Visitor(LSLAutoCompleteParser parent)
            {
                _parent = parent;
            }

            public int CodeAreaId { get; private set; }
            public int ScopeId { get; private set; }
            public int ScopeLevel { get; private set; }
            private int CodeScopeLevel { get; set; }
            private int ControlStructureNestingDepth { get; set; }

            public override bool VisitExpr_TypeCast(LSLParser.Expr_TypeCastContext context)
            {
                return base.VisitExpr_TypeCast(context);
            }

            public override bool VisitParenthesizedExpression(LSLParser.ParenthesizedExpressionContext context)
            {
                return base.VisitParenthesizedExpression(context);
            }

            public override bool VisitLabelStatement(LSLParser.LabelStatementContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;
                if (context.label_prefix == null) return true;

                if (context.semi_colon == null || (_parent._toOffset > context.label_prefix.StopIndex &&
                                                   (context.semi_colon.Text != ";" ||
                                                    _parent._toOffset < context.semi_colon.StartIndex)))
                {
                    _parent.InLabelDefinitionNameArea = true;
                }

                return base.VisitLabelStatement(context);
            }

            public override bool VisitJumpStatement(LSLParser.JumpStatementContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;
                if (context.jump_keyword == null) return true;

                if (context.semi_colon == null || (_parent._toOffset > context.jump_keyword.StopIndex &&
                                                   (context.semi_colon.Text != ";" ||
                                                    _parent._toOffset < context.semi_colon.StartIndex)))
                {
                    _parent.InJumpStatementLabelNameArea = true;
                }

                return base.VisitJumpStatement(context);
            }

            public override bool VisitStateChangeStatement(LSLParser.StateChangeStatementContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;
                if (context.state_keyword == null) return true;

                if (context.semi_colon == null || (_parent._toOffset > context.state_keyword.StopIndex &&
                                                   (context.semi_colon.Text != ";" ||
                                                    _parent._toOffset < context.semi_colon.StartIndex)))
                {
                    _parent.InStateChangeStatementStateNameArea = true;
                }

                return base.VisitStateChangeStatement(context);
            }

            public override bool VisitListLiteral(LSLParser.ListLiteralContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                _parent.NestableExpressionElementStack.Push(NestableExpressionElementType.List);

                var val = base.VisitListLiteral(context);

                if (context.Stop.Text != "]") return true;

                if (context.Stop.StartIndex >= _parent._toOffset) return true;

                _parent.NestableExpressionElementStack.Pop();


                return val;
            }

            public override bool VisitVectorLiteral(LSLParser.VectorLiteralContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;


                _parent.NestableExpressionElementStack.Push(NestableExpressionElementType.Vector);


                var val = base.VisitVectorLiteral(context);

                if (context.Stop.Text != ">") return true;

                if (context.Stop.StartIndex >= _parent._toOffset) return true;

                _parent.NestableExpressionElementStack.Pop();


                return val;
            }

            public override bool VisitRotationLiteral(LSLParser.RotationLiteralContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                _parent.NestableExpressionElementStack.Push(NestableExpressionElementType.Rotation);


                var val = base.VisitRotationLiteral(context);

                if (context.Stop.Text != ">") return true;
                if (context.Stop.StartIndex >= _parent._toOffset) return true;


                _parent.NestableExpressionElementStack.Pop();


                return val;
            }

            public override bool VisitGlobalVariableDeclaration(LSLParser.GlobalVariableDeclarationContext context)
            {
                if (context.variable_name == null || context.variable_type == null) return true;


                if (context.Start.StartIndex <= _parent._toOffset)
                {
                    _parent.InGlobalScope = false;
                }


                var variable =
                    new GlobalVariable(
                        context.variable_name.Text,
                        context.variable_type.Text,
                        new LSLSourceCodeRange(context), new LSLSourceCodeRange(context.variable_type),
                        new LSLSourceCodeRange(context.variable_name));


                if (!_parent._globalVariables.ContainsKey(context.variable_name.Text))
                {
                    _parent._globalVariables.Add(context.variable_name.Text, variable);
                }


                if (context.Start.StartIndex <= _parent._toOffset)
                {
                    if (context.operation != null && context.operation.StartIndex < _parent._toOffset)
                    {
                        _parent.InGlobalVariableDeclarationExpression = true;
                    }


                    if (context.semi_colon != null && context.semi_colon.Text == ";" &&
                        context.semi_colon.StartIndex < _parent._toOffset)
                    {
                        _parent.NestableExpressionElementStack.Clear();
                        _parent.InGlobalVariableDeclarationExpression = false;
                        _parent.InGlobalScope = true;
                    }
                }


                return base.VisitGlobalVariableDeclaration(context);
            }

            public override bool VisitExpr_Assignment(LSLParser.Expr_AssignmentContext context)
            {
                if (context.operation != null && _parent._toOffset > context.operation.StopIndex &&
                    _parent._toOffset <= context.Stop.StopIndex)
                {
                    if (context.expr_lvalue == null) return true;

                    var atom = context.expr_lvalue as LSLParser.Expr_AtomContext;

                    if (atom == null) return base.VisitExpr_Assignment(context);
                    if (atom.variable == null) return base.VisitExpr_Assignment(context);


                    if (_parent._parameters.ContainsKey(atom.variable.Text) ||
                        _parent._localVariables.Any(x => x.ContainsKey(atom.variable.Text)) ||
                        _parent._globalVariables.ContainsKey(atom.variable.Text))
                    {
                        _parent.InPlainAssignmentRightExpression = true;
                    }
                }
                return base.VisitExpr_Assignment(context);
            }

            public override bool VisitExpr_ModifyingAssignment(LSLParser.Expr_ModifyingAssignmentContext context)
            {
                if (context.operation != null && _parent._toOffset > context.operation.StopIndex &&
                    _parent._toOffset <= context.Stop.StopIndex)
                {
                    if (context.expr_lvalue == null) return true;

                    var atom = context.expr_lvalue as LSLParser.Expr_AtomContext;

                    if (atom == null) return base.VisitExpr_ModifyingAssignment(context);

                    if (atom.variable != null)
                    {
                        if (_parent._parameters.ContainsKey(atom.variable.Text) ||
                            _parent._localVariables.Any(x => x.ContainsKey(atom.variable.Text)) ||
                            _parent._globalVariables.ContainsKey(atom.variable.Text))
                        {
                            _parent.InModifyingAssignmentRightExpression = true;
                        }
                    }
                    else if (atom.list_literal != null || atom.string_literal != null)
                    {
                        _parent.InModifyingAssignmentRightExpression = true;
                    }
                }
                return base.VisitExpr_ModifyingAssignment(context);
            }

            public override bool VisitReturnStatement(LSLParser.ReturnStatementContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                if (_parent.InFunctionCodeBody && context.return_expression != null && context.return_keyword != null)
                {
                    if (_parent._toOffset > context.return_keyword.StopIndex &&
                        _parent._toOffset <= context.return_expression.Stop.StartIndex)
                    {
                        _parent.InFunctionReturnExpression = true;
                    }
                }
                else if (
                    (context.return_keyword == null || context.return_keyword.StopIndex < _parent._toOffset)
                    && context.return_expression == null)
                {
                    _parent.InFunctionReturnExpression = true;
                }

                return base.VisitReturnStatement(context);
            }

            public override bool VisitIfStatement(LSLParser.IfStatementContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                

                if (context.open_parenth != null)
                {
                    if (context.open_parenth.Text == "(" && context.open_parenth.StartIndex < _parent._toOffset)
                    {
                        _parent.InIfConditionExpression = true;
                    }
                }

                if (context.close_parenth != null)
                {
                    if (context.close_parenth.Text == ")" && context.close_parenth.StartIndex < _parent._toOffset)
                    {
                        _parent.NestableExpressionElementStack.Clear();
                        _parent.InIfConditionExpression = false;
                        _parent.InSingleStatementCodeScopeTopLevel = true;
                        _startIfControlChainCodeScope = this.CodeScopeLevel;
                        _startIfControlChain = true;
                    }
                }


                return base.VisitIfStatement(context);
            }

            public override bool VisitElseIfStatement(LSLParser.ElseIfStatementContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;


                if (context.open_parenth != null && context.open_parenth.Text == "(" &&
                    context.open_parenth.StartIndex < _parent._toOffset)
                {
                    _parent.InElseIfConditionExpression = true;
                }

                if (context.close_parenth != null && context.close_parenth.Text == ")" &&
                    context.close_parenth.StartIndex < _parent._toOffset)
                {
                    _parent.NestableExpressionElementStack.Clear();
                    _parent.InElseIfConditionExpression = false;
                    _parent.InSingleStatementCodeScopeTopLevel = true;
                }
                return base.VisitElseIfStatement(context);
            }

            public override bool VisitElseStatement(LSLParser.ElseStatementContext context)
            {
                _startIfControlChainCodeScope = -1;
                _startIfControlChain = false;
                _parent.AfterIfOrElseIfStatement = false;
                _parent.InSingleStatementCodeScopeTopLevel = true;
                return base.VisitElseStatement(context);
            }

            public override bool VisitWhileLoop(LSLParser.WhileLoopContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;


                if (context.Start.StartIndex <= _parent._toOffset)
                {
                    ControlStructureNestingDepth++;
                    _parent.InControlStatementSourceRange = true;
                }

                if (context.open_parenth != null)
                {
                    if (context.open_parenth.Text == "(" && context.open_parenth.StartIndex < _parent._toOffset)
                    {
                        _parent.InWhileConditionExpression = true;
                    }
                }

                if (context.close_parenth != null)
                {
                    if (context.close_parenth.Text == ")" && context.close_parenth.StartIndex < _parent._toOffset)
                    {
                        _parent.NestableExpressionElementStack.Clear();
                        _parent.InWhileConditionExpression = false;
                        _parent.InSingleStatementCodeScopeTopLevel = true;
                    }
                }


                var val = base.VisitWhileLoop(context);

                if (context.Stop.Text != "}") return val;
                if (context.Stop.StopIndex >= _parent._toOffset) return val;

                ControlStructureNestingDepth--;
                if (ControlStructureNestingDepth == 0)
                {
                    _parent.InControlStatementSourceRange = false;
                }
                return val;
            }

            public override bool VisitForLoop(LSLParser.ForLoopContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;


                if (context.Start.StartIndex <= _parent._toOffset)
                {
                    ControlStructureNestingDepth++;
                    _parent.InControlStatementSourceRange = true;
                }

                if (context.open_parenth != null)
                {
                    if (context.open_parenth.Text == "(" && context.open_parenth.StartIndex < _parent._toOffset)
                    {
                        _parent.InForLoopClausesArea = true;
                    }
                }
                //TODO separate out the clauses into flags, so that the nested element stack cannot get messed up if syntax errors occur
                //in the expressions preceding the one the cursor is in 
                if (context.close_parenth != null)
                {
                    if (context.close_parenth.Text == ")" && context.close_parenth.StartIndex < _parent._toOffset)
                    {
                        _parent.NestableExpressionElementStack.Clear();
                        _parent.InForLoopClausesArea = false;
                        _parent.InSingleStatementCodeScopeTopLevel = true;
                    }
                }


                var val = base.VisitForLoop(context);

                if (context.Stop.Text != "}") return val;
                if (context.Stop.StopIndex >= _parent._toOffset) return val;

                ControlStructureNestingDepth--;
                if (ControlStructureNestingDepth == 0)
                {
                    _parent.InControlStatementSourceRange = false;
                }


                return val;
            }

            public override bool VisitDoLoop(LSLParser.DoLoopContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                _parent.InSingleStatementCodeScopeTopLevel = true;

                if (context.Start.StartIndex <= _parent._toOffset)
                {
                    ControlStructureNestingDepth++;
                    _parent.InControlStatementSourceRange = true;
                }

                if (context.open_parenth != null)
                {
                    if (context.open_parenth.Text == "(" && context.open_parenth.StartIndex < _parent._toOffset)
                    {
                        _parent.InDoWhileConditionExpression = true;
                    }
                }

                if (context.close_parenth != null)
                {
                    if (context.close_parenth.Text == ")" && context.close_parenth.StartIndex < _parent._toOffset)
                    {
                        _parent.NestableExpressionElementStack.Clear();
                        _parent.InDoWhileConditionExpression = false;
                    }
                }


                var val = base.VisitDoLoop(context);

                if (context.Stop.Text != "}") return val;
                if (context.Stop.StopIndex >= _parent._toOffset) return val;

                ControlStructureNestingDepth--;
                if (ControlStructureNestingDepth == 0)
                {
                    _parent.InControlStatementSourceRange = false;
                }
                return val;
            }

            public override bool VisitControlStructure(LSLParser.ControlStructureContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;


                if (context.Start.StartIndex <= _parent._toOffset)
                {
                    _parent.InControlStatementSourceRange = true;
                    ControlStructureNestingDepth++;
                }

                var val = base.VisitControlStructure(context);


                if (context.Stop.Text != "}") return val;
                if (context.Stop.StopIndex >= _parent._toOffset) return val;

                ControlStructureNestingDepth--;
                if (ControlStructureNestingDepth == 0)
                {
                    _parent.InControlStatementSourceRange = false;
                }

                _parent.AfterIfOrElseIfStatement = _startIfControlChain;

                return val;
            }

            public override bool VisitExpr_FunctionCall(LSLParser.Expr_FunctionCallContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                if (context.open_parenth != null && context.open_parenth.Text == "(" &&
                    context.open_parenth.StartIndex < _parent._toOffset)
                {
                    _parent.NestableExpressionElementStack.Push(NestableExpressionElementType.FunctionCallParameterList);
                }

                var v = base.VisitExpr_FunctionCall(context);

                if (context.close_parenth != null && context.close_parenth.Text == ")" &&
                    context.close_parenth.StartIndex < _parent._toOffset)
                {
                    _parent.NestableExpressionElementStack.Pop();
                }


                return v;
            }

            public override bool VisitLocalVariableDeclaration(LSLParser.LocalVariableDeclarationContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                if (context.variable_name == null || context.variable_type == null) return true;


                var prevMultiStatementCodeScopeTopLevel = _parent.InMultiStatementCodeScopeTopLevel;
                var prevSingleStatementCodeScopeTopLevel = _parent.InSingleStatementCodeScopeTopLevel;

                _parent.InMultiStatementCodeScopeTopLevel = false;
                _parent.InSingleStatementCodeScopeTopLevel = false;

                var variable = new LocalVariable(
                    context.variable_name.Text,
                    context.variable_type.Text,
                    new LSLSourceCodeRange(context),
                    new LSLSourceCodeRange(context.variable_type),
                    new LSLSourceCodeRange(context.variable_name),
                    new ScopeAddress(CodeAreaId, ScopeId, ScopeLevel));


                var scopeVars = _parent._localVariables.Peek();

                if (_parent._globalVariables.ContainsKey(context.variable_name.Text))
                {
                    _parent._globalVariables.Remove(context.variable_name.Text);
                }

                if (_parent._parameters.ContainsKey(context.variable_name.Text))
                {
                    _parent._parameters.Remove(context.variable_name.Text);
                }

                if (!scopeVars.ContainsKey(context.variable_name.Text))
                {
                    scopeVars.Add(
                        context.variable_name.Text,
                        variable);
                }


                if (context.operation != null && context.operation.StartIndex < _parent._toOffset)
                {
                    _parent.InLocalVariableDeclarationExpression = true;
                }


                if (context.semi_colon != null && context.semi_colon.Text == ";" &&
                    context.semi_colon.StartIndex < _parent._toOffset)
                {
                    _parent.NestableExpressionElementStack.Clear();

                    _parent.InLocalVariableDeclarationExpression = false;
                    _parent.InMultiStatementCodeScopeTopLevel = prevMultiStatementCodeScopeTopLevel;
                    _parent.InSingleStatementCodeScopeTopLevel = prevSingleStatementCodeScopeTopLevel;
                }


                return base.VisitLocalVariableDeclaration(context);
            }



            public override bool VisitFunctionDeclaration(LSLParser.FunctionDeclarationContext context)
            {
                var returnTypeText = context.return_type == null ? "" : context.return_type.Text;


                var parms = new List<LocalParameter>();

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

                            if (_parent._parameters.ContainsKey(i.parameter_name.Text)) continue;

                            var parm = new LocalParameter(
                                i.parameter_name.Text,
                                i.parameter_type.Text,
                                new LSLSourceCodeRange(i),
                                new LSLSourceCodeRange(i.parameter_type),
                                new LSLSourceCodeRange(i.parameter_name),
                                new ScopeAddress(CodeAreaId, ScopeId + 1, ScopeLevel + 1));

                            parms.Add(parm);
                            _parent._parameters.Add(parm.Name, parm);
                        }
                    }
                }

                if (context.function_name == null || context.function_name.Type == -1) return true;

                if (!_parent._globalFunctions.ContainsKey(context.function_name.Text))
                {
                    if (context.return_type != null)
                    {
                        _parent._globalFunctions.Add(
                            context.function_name.Text,
                            new GlobalFunction(context.function_name.Text, returnTypeText,
                                new LSLSourceCodeRange(context), new LSLSourceCodeRange(context.return_type),
                                new LSLSourceCodeRange(context.function_name), parms));
                    }
                    else
                    {
                        _parent._globalFunctions.Add(
                            context.function_name.Text,
                            new GlobalFunction(context.function_name.Text,
                                new LSLSourceCodeRange(context),
                                new LSLSourceCodeRange(context.function_name), parms));
                    }
                }


                if (context.Start.StartIndex >= _parent._toOffset) return true;

                _parent.InGlobalScope = false;

                if (context.open_parenth != null && context.open_parenth.Text == "(" &&
                    context.open_parenth.StartIndex < _parent._toOffset)
                {
                    _parent.InFunctionDeclarationParameterList = true;
                }

                if (context.close_parenth != null && context.close_parenth.Text == ")" &&
                    context.close_parenth.StartIndex < _parent._toOffset)
                {
                    _parent.InFunctionDeclarationParameterList = false;
                }


                if (context.code == null || context.code.open_brace == null ||
                    context.code.open_brace.Text == "<missing '}'>" ||
                    context.code.open_brace.StartIndex >= _parent._toOffset) return true;


                _parent.CurrentCodeAreaRange = new LSLSourceCodeRange(context.code);

                CodeAreaId++;


                _parent.InFunctionCodeBody = true;

                _parent.CurrentFunction = context.function_name != null ? context.function_name.Text : null;

                base.VisitFunctionDeclaration(context);

                if ((context.Stop.StartIndex) >= _parent._toOffset) return true;

                _parent._parameters.Clear();

                _parent.NestableExpressionElementStack.Clear();
                _parent.InFunctionCodeBody = false;
                _parent.InGlobalScope = true;

                return true;
            }

            public override bool VisitEventHandler(LSLParser.EventHandlerContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;


                _parent.InEventSourceRange = true;


                if (context.open_parenth != null && context.open_parenth.Text == "(" &&
                    context.open_parenth.StartIndex < _parent._toOffset)
                {
                    _parent.InEventParameterList = true;
                }

                if (context.close_parenth != null && context.close_parenth.Text == ")" &&
                    context.close_parenth.StartIndex < _parent._toOffset)
                {
                    _parent.InEventParameterList = false;
                }


                if (context.code == null || context.code.open_brace == null ||
                    context.code.open_brace.Text == "<missing '}'>" ||
                    context.code.open_brace.StartIndex >= _parent._toOffset) return true;

                CodeAreaId++;

                _parent._parameters.Clear();

                if (context.parameters != null && context.parameters.children != null)
                {
                    var list = context.parameters.parameterList();
                    if (list != null)
                    {
                        foreach (var child in list.children)
                        {
                            var i = child as LSLParser.ParameterDefinitionContext;

                            if (i == null) continue;

                            if (_parent._parameters.ContainsKey(i.parameter_name.Text)) continue;

                            var parm = new LocalParameter(
                                i.parameter_name.Text,
                                i.parameter_type.Text,
                                new LSLSourceCodeRange(i),
                                new LSLSourceCodeRange(i.parameter_type),
                                new LSLSourceCodeRange(i.parameter_name),
                                new ScopeAddress(CodeAreaId, ScopeId + 1, ScopeLevel + 1));

                            _parent._parameters.Add(parm.Name, parm);
                        }
                    }
                }


                _parent.CurrentCodeAreaRange = new LSLSourceCodeRange(context.code);

                _parent.InEventCodeBody = true;

                _parent.CurrentEvent = context.handler_name != null ? context.handler_name.Text : null;

                base.VisitEventHandler(context);

                if ((context.Stop.StartIndex) >= _parent._toOffset) return true;

                _parent.NestableExpressionElementStack.Clear();
                _parent.InEventCodeBody = false;

                _parent.InEventSourceRange = false;

                return true;
            }

            public override bool VisitDefaultState(LSLParser.DefaultStateContext context)
            {
                _parent.DefaultState = new StateBlock("default", new LSLSourceCodeRange(context));

                if (_parent._toOffset <= context.Start.StartIndex) return true;

                _parent.InGlobalScope = false;

                if (context.open_brace == null) return true;

                if (((_parent._toOffset >= context.Start.StartIndex && context.open_brace.Text != "{") ||
                     (_parent._toOffset <= context.open_brace.StartIndex &&
                      _parent._toOffset >= context.Start.StartIndex)))
                {
                    return true;
                }

                if (context.open_brace != null && _parent._toOffset <= context.open_brace.StartIndex)
                {
                    return true;
                }


                CodeAreaId++;
                ScopeLevel++;
                ScopeId++;


                _parent.InState = true;

                _parent.CurrentState = context.state_name != null ? context.state_name.Text : null;

                base.VisitDefaultState(context);


                if (((context.Stop.StartIndex) >= _parent._toOffset) || context.Stop.Text != "}") return true;

                _parent.NestableExpressionElementStack.Clear();
                _parent.InState = false;


                ScopeLevel--;


                return true;
            }

            public override bool VisitDefinedState(LSLParser.DefinedStateContext context)
            {
                if (context.state_name == null || context.state_name.Type == -1) return true;

                _parent._stateBlocks.Add(new StateBlock(context.state_name.Text, new LSLSourceCodeRange(context)));


                if (_parent._toOffset <= context.Start.StartIndex) return true;

                if (context.open_brace == null) return true;

                if (((_parent._toOffset >= context.Start.StartIndex && context.open_brace.Text != "{") ||
                     (_parent._toOffset <= context.open_brace.StartIndex &&
                      _parent._toOffset >= context.Start.StartIndex)))
                {
                    return true;
                }

                if (context.open_brace != null && _parent._toOffset <= context.open_brace.StartIndex)
                {
                    return true;
                }


                CodeAreaId++;
                ScopeLevel++;
                ScopeId++;


                _parent.InState = true;

                _parent.CurrentState = context.state_name != null ? context.state_name.Text : null;

                base.VisitDefinedState(context);

                if (((context.Stop.StartIndex) >= _parent._toOffset) || context.Stop.Text != "}") return true;

                _parent.NestableExpressionElementStack.Clear();
                _parent.InState = false;

                ScopeLevel--;

                return true;
            }


            public override bool VisitCodeStatement(LSLParser.CodeStatementContext context)
            {
                if (context.Parent is LSLParser.CodeScopeOrSingleBlockStatementContext)
                {

                    //var prevSign = _parent.InSingleStatementCodeScopeTopLevel;
                    if (_parent._toOffset > context.Stop.StartIndex && context.Stop.Text == ";")
                    {
                        _parent.InControlStatementSourceRange = false;
                    }

                    _parent.InSingleStatementCodeScopeTopLevel = false;
                    
                    _parent.AfterIfOrElseIfStatement = _startIfControlChain;

                    base.VisitCodeStatement(context);

                    //_parent.InSingleStatementCodeScopeTopLevel = prevSign;
                }
                else
                {
                    if (context.Start.StartIndex > _parent._toOffset) return true;


                    var prevMulti = _parent.InMultiStatementCodeScopeTopLevel;
                    var prevSign = _parent.InSingleStatementCodeScopeTopLevel;

                    _parent.InMultiStatementCodeScopeTopLevel = false;
                    _parent.InSingleStatementCodeScopeTopLevel = false;


                    

                    base.VisitCodeStatement(context);

                    if (context.Stop.StopIndex >= _parent._toOffset || context.Stop.Text != ";") return true;

                    var isControlStatementOneLiner = (context.control_structure != null || context.loop_structure != null) && context.Stop.Text == ";";

                    if (_startIfControlChainCodeScope == this.CodeScopeLevel 
                        && !_parent.InSingleStatementCodeScopeTopLevel 
                        && !isControlStatementOneLiner)
                    {
                        _startIfControlChainCodeScope = -1;
                        _startIfControlChain = false;
                        _parent.AfterIfOrElseIfStatement = false;
                    }


                    _parent.NestableExpressionElementStack.Clear();

                    _parent.InMultiStatementCodeScopeTopLevel = prevMulti;
                    _parent.InSingleStatementCodeScopeTopLevel = prevSign;
                }

                return true;
            }

            public override bool VisitCodeScope(LSLParser.CodeScopeContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                _parent.InSingleStatementCodeScopeTopLevel = false;
                _parent.InMultiStatementCodeScopeTopLevel = true;
                CodeScopeLevel++;


                ScopeLevel++;
                ScopeId++;

                _parent._localVariables.Push(new Dictionary<string, LocalVariable>());

                foreach (var i in context.codeStatement())
                {
                    if (i.Start.StartIndex > _parent._toOffset)
                    {
                        return true;
                    }
                    VisitCodeStatement(i);
                }

                if ((context.Stop.StartIndex) >= _parent._toOffset) return true;

                _parent._localVariables.Pop();


                CodeScopeLevel--;
                if (CodeScopeLevel == 0)
                {
                    _parent.NestableExpressionElementStack.Clear();
                    _parent.InMultiStatementCodeScopeTopLevel = false;
                }

                ScopeLevel--;

                _parent.AfterIfOrElseIfStatement = _startIfControlChain;

                return true;
            }
        };
    }
}