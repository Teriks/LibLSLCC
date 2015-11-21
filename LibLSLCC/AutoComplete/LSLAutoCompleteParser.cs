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
using System.IO;
using System.Linq;
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
    /// An LSL parser that can help with implementing context aware auto-complete inside of code editors.
    /// It is not advisable that you consume this class at this point in time.
    /// </summary>
    public class LSLAutoCompleteParser
    {
        private readonly HashMap<string, GlobalFunction> _globalFunctions = new HashMap<string, GlobalFunction>();
        private readonly HashMap<string, GlobalVariable> _globalVariables = new HashMap<string, GlobalVariable>();

        private static readonly Regex JumpRegex = new Regex("jump\\s*(" + LSLTokenTools.IDRegexString + ")");
        private static readonly Regex LabelRegex = new Regex("@\\s*(" + LSLTokenTools.IDRegexString + ")");

        private readonly Stack<Dictionary<string, LocalVariable>> _localVariables =
            new Stack<Dictionary<string, LocalVariable>>();

        private readonly HashMap<string, LocalParameter> _parameters = new HashMap<string, LocalParameter>();
        private readonly GenericArray<StateBlock> _stateBlocks = new GenericArray<StateBlock>();

        private readonly Stack<NestableExpressionElementType> _nestableExpressionElementStack =
            new Stack<NestableExpressionElementType>();



        private class LastControlChainStatusContainer
        {
            public bool IsIfOrElseIf;
        }

        private readonly Stack<LastControlChainStatusContainer> _lastControlChainElementStack =
            new Stack<LastControlChainStatusContainer>();


        private bool _inEventCodeBody;
        private bool _inFunctionCodeBody;
        private bool _inGlobalScope;
        private bool _inState;
        private int _toOffset;

        public int ParseToOffset { get { return _toOffset; } }

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

        public IReadOnlyGenericArray<StateBlock> StateBlocks
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

        public IReadOnlyHashMap<string, GlobalFunction> GlobalFunctionsDictionary
        {
            get { return _globalFunctions; }
        }

        public IReadOnlyHashMap<string, GlobalVariable> GlobalVariablesDictionary
        {
            get { return _globalVariables; }
        }

        public IReadOnlyHashMap<string, LocalParameter> LocalParametersDictionary
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


        public bool InBasicExpressionTree { get; private set; }

        public bool RightOfDotAccessor { get; private set; }

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
            get { return (InMultiStatementCodeScopeTopLevel | InSingleStatementCodeScopeTopLevel); }
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
                       InListLiteralContent || InVectorLiteralContent || InRotationLiteralContent || InVariableAssignmentExpression || 
                       InComponentAssignmentExpression || InBasicExpressionTree ) &&
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

        public bool InModifyingVariableAssignmentExpression { get; private set; }

        public bool InModifyingComponentAssignmentExpression { get; private set; }

        public bool InPlainVariableAssignmentExpression { get; private set; }

        public bool InPlainComponentAssignmentExpression { get; private set; }

        public bool InVariableAssignmentExpression
        {
            get { return InPlainVariableAssignmentExpression || InModifyingVariableAssignmentExpression ; }
        }


        public bool InComponentAssignmentExpression
        {
            get { return InPlainComponentAssignmentExpression || InModifyingComponentAssignmentExpression; }
        }

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
                return _nestableExpressionElementStack.Count > 0 &&
                       _nestableExpressionElementStack.Peek() == NestableExpressionElementType.FunctionCallParameterList;
            }
        }

        public bool InListLiteralContent
        {
            get
            {
                return _nestableExpressionElementStack.Count > 0 &&
                       _nestableExpressionElementStack.Peek() == NestableExpressionElementType.List;
            }
        }

        public bool InVectorLiteralContent
        {
            get
            {
                return _nestableExpressionElementStack.Count > 0 &&
                       _nestableExpressionElementStack.Peek() == NestableExpressionElementType.Vector;
            }
        }

        public bool InRotationLiteralContent
        {
            get
            {
                return _nestableExpressionElementStack.Count > 0 &&
                       _nestableExpressionElementStack.Peek() == NestableExpressionElementType.Rotation;
            }
        }

        public bool CanSuggestLibraryConstant
        {
            get { return InExpressionArea;  }
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
            get { return !InSingleStatementCodeScopeTopLevel && _lastControlChainElementStack.Count > 0 && _lastControlChainElementStack.Peek().IsIfOrElseIf; }
        }


        public bool CanSuggestControlStatement
        {
            get { return InTopLevelCodeScope; }
        }

        public bool CanSuggestStateChangeStatement
        {
            get { return InTopLevelCodeScope; }
        }

        public bool CanSuggestReturnStatement
        {
            get { return InTopLevelCodeScope; }
        }

        public bool CanSuggestJumpStatement
        {
            get { return InTopLevelCodeScope; }
        }

        public LSLType CurrentFunctionReturnType { get; set; }

        public IEnumerable<LocalLabel> GetLocalLabels(string sourceCode)
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
                yield return new LocalLabel(name);
            }
        }

        public IEnumerable<LocalJump> GetLocalJumps(string sourceCode)
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
                yield return new LocalJump(name);
            }
        }

        public void Parse(TextReader stream, int toOffset)
        {
            _lastControlChainElementStack.Clear();
            _nestableExpressionElementStack.Clear();
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
            public GlobalFunction(string name, string type, LSLSourceCodeRange range, LSLSourceCodeRange typeRange, LSLSourceCodeRange nameRange, List<LocalParameter> parameters)
            {
                Parameters = parameters.WrapWithGenericArray();
                Name = name;
                ReturnType = type;
                SourceCodeRange = range;

                NameSourceCodeRange = nameRange;

                TypeSourceCodeRange = typeRange;

                HasReturnType = true;
            }

            public GlobalFunction(string name, LSLSourceCodeRange range, LSLSourceCodeRange nameRange,
                List<LocalParameter> parameters)
            {
                Parameters = parameters.WrapWithGenericArray();
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
            public string Name { get; private set; }
            public string ReturnType { get; private set; }

            public string FullSignature
            {
                get
                {
                    var sig = "";
                    if (ReturnType != "")
                    {
                        sig += ReturnType + " ";
                    }

                    sig += ParametersSignature + ";";

                    return sig;
                }
            }

            public string ParametersSignature
            {
                get
                {
                    var sig = "()";
                    

                    if (Parameters.Count > 0)
                    {
                        sig = string.Join(", ", Parameters.Select(x => x.Type + " " + x.Name));
                    }

                    return sig;
                }
            }

            public IReadOnlyGenericArray<LocalParameter> Parameters { get; private set; }
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


            public override string ToString()
            {
                return string.Format("(CodeAreaID: {0}, ScopeId: {1}, ScopeLevel: {2})", CodeAreaId,ScopeId,ScopeLevel);
            }
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

                TypeSourceCodeRange = typeRange;
            }

            public LSLSourceCodeRange NameSourceCodeRange { get; private set; }
            public LSLSourceCodeRange TypeSourceCodeRange { get; private set; }
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
            public string Name { get; private set; }
            public string Type { get; private set; }
            public LSLSourceCodeRange SourceCodeRange { get; private set; }
        }

        private class Visitor : LSLBaseVisitor<bool>
        {
            private readonly LSLAutoCompleteParser _parent;

            public Visitor(LSLAutoCompleteParser parent)
            {
                _parent = parent;
            }

            public int CodeAreaId { get; private set; }
            public int ScopeId { get; private set; }
            public int ScopeLevel { get; private set; }
            private int CodeScopeLevel { get; set; }
            private int ControlStructureNestingDepth { get; set; }




            public override bool VisitLabelStatement(LSLParser.LabelStatementContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;
                if (context.label_prefix == null) return true;

                if ((_parent._toOffset > context.label_prefix.StartIndex &&
                    _parent._toOffset <= context.Stop.StopIndex) || (context.label_prefix.StopIndex == context.Stop.StopIndex))
                {
                    _parent.InLabelDefinitionNameArea = true;
                }

                return base.VisitLabelStatement(context);
            }

            public override bool VisitJumpStatement(LSLParser.JumpStatementContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;
                if (context.jump_keyword == null) return true;

                if ((_parent._toOffset > context.jump_keyword.StopIndex &&
                    _parent._toOffset <= context.Stop.StopIndex) || (context.jump_keyword.StopIndex==context.Stop.StopIndex))
                {
                    _parent.InJumpStatementLabelNameArea = true;
                }

                return base.VisitJumpStatement(context);
            }

            public override bool VisitStateChangeStatement(LSLParser.StateChangeStatementContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;
                if (context.state_keyword == null) return true;


                if ((_parent._toOffset > context.state_keyword.StopIndex &&
                    _parent._toOffset <= context.Stop.StopIndex) || (context.state_keyword.StopIndex == context.Stop.StopIndex))
                {
                    _parent.InStateChangeStatementStateNameArea = true;
                }

                return base.VisitStateChangeStatement(context);
            }

            public override bool VisitListLiteral(LSLParser.ListLiteralContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                _parent._nestableExpressionElementStack.Push(NestableExpressionElementType.List);

                var val = base.VisitListLiteral(context);

                if (context.Stop.Text != "]") return true;

                if (context.Stop.StartIndex >= _parent._toOffset) return true;

                _parent._nestableExpressionElementStack.Pop();


                return val;
            }

            public override bool VisitVectorLiteral(LSLParser.VectorLiteralContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;


                _parent._nestableExpressionElementStack.Push(NestableExpressionElementType.Vector);


                var val = base.VisitVectorLiteral(context);

                if (context.Stop.Text != ">") return true;

                if (context.Stop.StartIndex >= _parent._toOffset) return true;

                _parent._nestableExpressionElementStack.Pop();


                return val;
            }

            public override bool VisitRotationLiteral(LSLParser.RotationLiteralContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                _parent._nestableExpressionElementStack.Push(NestableExpressionElementType.Rotation);


                var val = base.VisitRotationLiteral(context);

                if (context.Stop.Text != ">") return true;
                if (context.Stop.StartIndex >= _parent._toOffset) return true;


                _parent._nestableExpressionElementStack.Pop();


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
                        _parent._nestableExpressionElementStack.Clear();
                        _parent.InGlobalVariableDeclarationExpression = false;
                        _parent.InGlobalScope = true;
                    }
                }


                return base.VisitGlobalVariableDeclaration(context);
            }

            public override bool VisitExpr_Assignment(LSLParser.Expr_AssignmentContext context)
            {
                if (context.operation != null && _parent._toOffset >= context.operation.StopIndex &&
                    ((_parent._toOffset <= context.Stop.StopIndex) || (context.operation.StopIndex == context.Stop.StopIndex)))
                {
                    if (context.expr_lvalue == null) return true;

                    var atom = context.expr_lvalue as LSLParser.Expr_AtomContext;
                    var accessor = context.expr_lvalue as LSLParser.Expr_DotAccessorContext;

                    if (atom != null)
                    {
                        if (atom.variable != null)
                        {
                            if (_parent._parameters.ContainsKey(atom.variable.Text) ||
                                _parent._localVariables.Any(x => x.ContainsKey(atom.variable.Text)) ||
                                _parent._globalVariables.ContainsKey(atom.variable.Text))
                            {
                                _parent.InPlainVariableAssignmentExpression = true;
                                _parent.InBasicExpressionTree = true;
                            }
                        }
                        else
                        {
                            return base.VisitExpr_Assignment(context);
                        }
                    }
                    else if (accessor != null)
                    {
                        var maybeVar = accessor.expr_lvalue.GetText();

                        if (_parent._parameters.ContainsKey(maybeVar) ||
                                 _parent._localVariables.Any(x => x.ContainsKey(maybeVar)) ||
                                 _parent._globalVariables.ContainsKey(maybeVar))
                        {
                            _parent.InPlainComponentAssignmentExpression = true;
                            _parent.InBasicExpressionTree = true;
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
                if (context.operation != null && _parent._toOffset >= context.operation.StopIndex &&
                    ((_parent._toOffset <= context.Stop.StopIndex) || (context.operation.StopIndex == context.Stop.StopIndex)))
                {
                    if (context.expr_lvalue == null) return true;

                    var atom = context.expr_lvalue as LSLParser.Expr_AtomContext;
                    var accessor = context.expr_lvalue as LSLParser.Expr_DotAccessorContext;

                    if (atom != null)
                    {
                        if (atom.variable != null)
                        {
                            if (_parent._parameters.ContainsKey(atom.variable.Text) ||
                                _parent._localVariables.Any(x => x.ContainsKey(atom.variable.Text)) ||
                                _parent._globalVariables.ContainsKey(atom.variable.Text))
                            {
                                _parent.InModifyingVariableAssignmentExpression = true;
                                _parent.InBasicExpressionTree = true;
                            }
                        }
                    }
                    else if (accessor != null)
                    {
                        var maybeVar = accessor.expr_lvalue.GetText();

                        if (_parent._parameters.ContainsKey(maybeVar) ||
                                 _parent._localVariables.Any(x => x.ContainsKey(maybeVar)) ||
                                 _parent._globalVariables.ContainsKey(maybeVar))
                        {
                            _parent.InModifyingComponentAssignmentExpression = true;
                            _parent.InBasicExpressionTree = true;
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
                    if (context.open_parenth.Text == "(" && context.open_parenth.StartIndex <= _parent._toOffset)
                    {
                        _parent.InIfConditionExpression = true;
                    }
                }

                if (context.open_parenth != null && context.close_parenth != null && context.close_parenth.Text == ")" && 
                    _parent._toOffset >= context.close_parenth.StartIndex)
                {
                    if (context.close_parenth.StartIndex != context.open_parenth.StartIndex)
                    {
                        _parent._nestableExpressionElementStack.Clear();
                        _parent.InIfConditionExpression = false;
                        

                        if (context.code != null && context.code.code != null &&
                            context.code.code.open_brace.Text == "{")
                        {
                            _parent.InSingleStatementCodeScopeTopLevel = false;
                        }
                        else
                        {
                            EnterSingleStatementCodeScope();
                        }

                    }

                }


                _parent._lastControlChainElementStack.Peek().IsIfOrElseIf = true;

                return base.VisitIfStatement(context);
            }

            public override bool VisitElseIfStatement(LSLParser.ElseIfStatementContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;


                if (context.open_parenth != null && context.open_parenth.Text == "(" &&
                    context.open_parenth.StartIndex <= _parent._toOffset)
                {
                    _parent.InElseIfConditionExpression = true;
                }

                if (context.open_parenth != null &&  context.close_parenth != null && context.close_parenth.Text == ")" &&
                      _parent._toOffset >= context.close_parenth.StartIndex)
                {
                    if (context.open_parenth.StartIndex != context.close_parenth.StartIndex)
                    {
                        _parent._nestableExpressionElementStack.Clear();
                        _parent.InElseIfConditionExpression = false;
                        

                        if (context.code != null && context.code.code != null &&
                            context.code.code.open_brace.Text == "{")
                        {
                            _parent.InSingleStatementCodeScopeTopLevel = false;
                        }
                        else
                        {
                            EnterSingleStatementCodeScope();
                        }
                    }
                }

                _parent._lastControlChainElementStack.Peek().IsIfOrElseIf = true;

                return base.VisitElseIfStatement(context);
            }


            public override bool VisitElseStatement(LSLParser.ElseStatementContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;




                if (context.code != null && context.code.code != null &&
                    context.code.code.open_brace.Text == "{")
                {
                    _parent.InSingleStatementCodeScopeTopLevel = false;
                }
                else
                {
                    EnterSingleStatementCodeScope();
                }


                _parent._lastControlChainElementStack.Peek().IsIfOrElseIf = false;

                return base.VisitElseStatement(context);
            }

            public override bool VisitWhileLoop(LSLParser.WhileLoopContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;


                ControlStructureNestingDepth++;
                _parent.InControlStatementSourceRange = true;
                

                if (context.open_parenth != null)
                {
                    if (context.open_parenth.Text == "(" && context.open_parenth.StartIndex <= _parent._toOffset)
                    {
                        _parent.InWhileConditionExpression = true;
                    }
                }

                if (context.open_parenth != null && context.close_parenth != null && context.close_parenth.Text == ")" &&
                    _parent._toOffset >= context.close_parenth.StartIndex)
                {
                    if (context.close_parenth.StartIndex != context.open_parenth.StartIndex)
                    {
                        _parent._nestableExpressionElementStack.Clear();
                        _parent.InWhileConditionExpression = false;
                        

                        if (context.code != null && context.code.code != null
                            && context.code.code.open_brace.Text == "{")
                        {
                            _parent.InSingleStatementCodeScopeTopLevel = false;
                        }
                        else
                        {
                            EnterSingleStatementCodeScope();
                        }
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


                ControlStructureNestingDepth++;
                _parent.InControlStatementSourceRange = true;


                if (context.open_parenth != null)
                {
                    if (context.open_parenth.Text == "(" && context.open_parenth.StartIndex <= _parent._toOffset)
                    {
                        _parent.InForLoopClausesArea = true;
                    }
                }
                //TODO separate out the clauses into flags, so that the nested element stack cannot get messed up if syntax errors occur
                //in the expressions preceding the one the cursor is in 
                if (context.open_parenth != null && context.close_parenth != null && context.close_parenth.Text == ")" &&
                    _parent._toOffset >= context.close_parenth.StartIndex)
                {
                    if (context.close_parenth.StartIndex != context.open_parenth.StartIndex)
                    {
                        _parent._nestableExpressionElementStack.Clear();
                        _parent.InForLoopClausesArea = false;
                        
                        if (context.code != null && context.code.code != null &&
                            context.code.code.open_brace.Text == "{")
                        {
                            _parent.InSingleStatementCodeScopeTopLevel = false;
                        }
                        else
                        {
                            EnterSingleStatementCodeScope();
                        }
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


                ControlStructureNestingDepth++;
                _parent.InControlStatementSourceRange = true;

                if (context.code != null && context.code.code != null &&
                    context.code.code.open_brace.Text == "{")
                {
                    _parent.InSingleStatementCodeScopeTopLevel = false;
                }
                else
                {
                    EnterSingleStatementCodeScope();
                }



                if (context.open_parenth != null)
                {
                    if (context.open_parenth.Text == "(" && context.open_parenth.StartIndex <= _parent._toOffset)
                    {
                        _parent.InDoWhileConditionExpression = true;
                    }
                }

                if (context.open_parenth != null && context.close_parenth != null && context.close_parenth.Text == ")" &&
                    _parent._toOffset >= context.close_parenth.StartIndex)
                {
                    if (context.open_parenth.StartIndex != context.close_parenth.StartIndex)
                    {
                        _parent._nestableExpressionElementStack.Clear();
                        _parent.InDoWhileConditionExpression = false;
                    }
                }

                

                var val = base.VisitDoLoop(context);

                if (context.code != null && context.code.code != null && context.code.code.close_brace != null &&
                    context.code.code.close_brace.StartIndex <= _parent._toOffset)
                {
                    _parent.InMultiStatementCodeScopeTopLevel = false;
                }

                if (context.Stop.Text != ";" || context.Stop.StopIndex >= _parent._toOffset) return val;

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


                if (context.Stop.Text != ";" || context.Stop.StopIndex >= _parent._toOffset) return val;

                ControlStructureNestingDepth--;
                if (ControlStructureNestingDepth == 0)
                {
                    _parent.InControlStatementSourceRange = false;
                }


                return val;
            }


            public override bool VisitExpr_FunctionCall(LSLParser.Expr_FunctionCallContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                if (context.open_parenth != null && context.open_parenth.Text == "(" &&
                    context.open_parenth.StartIndex <= _parent._toOffset)
                {
                    _parent._nestableExpressionElementStack.Push(NestableExpressionElementType.FunctionCallParameterList);
                }

                var v = base.VisitExpr_FunctionCall(context);

                if (context.open_parenth != null && context.close_parenth != null
                    && context.close_parenth.Text == ")" && _parent._toOffset >= context.close_parenth.StartIndex)
                {
                    if (context.open_parenth.StartIndex != context.close_parenth.StartIndex)
                    {
                        _parent._nestableExpressionElementStack.Pop();
                    }
                }


                return v;
            }

            readonly Stack<List<GlobalVariable>> _globalVariablesHidden = new Stack<List<GlobalVariable>>(); 

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

                GlobalVariable hiddenGlobalVariable;
                if (_parent._globalVariables.TryGetValue(context.variable_name.Text, out hiddenGlobalVariable))
                {
                    _globalVariablesHidden.Peek().Add(hiddenGlobalVariable);
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
                    _parent._nestableExpressionElementStack.Clear();

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
                    context.open_parenth.StartIndex <= _parent._toOffset)
                {
                    _parent.InFunctionDeclarationParameterList = true;
                }

                if (context.open_parenth != null && context.close_parenth != null && context.close_parenth.Text == ")" &&
                    _parent._toOffset >= context.close_parenth.StartIndex)
                {
                    if (context.close_parenth.StartIndex != context.open_parenth.StartIndex)
                    {
                        _parent.InFunctionDeclarationParameterList = false;
                    }
                }


                if (context.code == null || context.code.open_brace == null ||
                    context.code.open_brace.Text == "<missing '}'>" ||
                    context.code.open_brace.StartIndex > _parent._toOffset) return true;


                _parent.CurrentCodeAreaRange = new LSLSourceCodeRange(context.code);

                CodeAreaId++;


                _parent.InFunctionCodeBody = true;

                _parent.CurrentFunction = context.function_name != null ? context.function_name.Text : null;

                _parent.CurrentFunctionReturnType = returnTypeText == ""
                    ? LSLType.Void
                    : LSLTypeTools.FromLSLTypeString(returnTypeText);


                base.VisitFunctionDeclaration(context);

                if (context.Stop.StartIndex > _parent._toOffset) return true;





                _parent.CurrentFunction = null;
                _parent.CurrentFunctionReturnType = LSLType.Void;

                _parent._parameters.Clear();

                _parent._nestableExpressionElementStack.Clear();
                _parent.InFunctionCodeBody = false;
                _parent.InGlobalScope = true;

                return true;
            }

            public override bool VisitEventHandler(LSLParser.EventHandlerContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;


                _parent.InEventSourceRange = true;


                if (context.open_parenth != null && context.open_parenth.Text == "(" &&
                    context.open_parenth.StartIndex <= _parent._toOffset)
                {
                    _parent.InEventParameterList = true;
                }

                if (context.open_parenth != null && context.close_parenth != null && context.close_parenth.Text == ")" &&
                    _parent._toOffset >= context.close_parenth.StartIndex)
                {
                    if (context.close_parenth.StartIndex != context.open_parenth.StartIndex)
                    {
                        _parent.InEventParameterList = false;
                    }
                }


                if (context.code == null || context.code.open_brace == null ||
                    context.code.open_brace.Text == "<missing '}'>" ||
                    context.code.open_brace.StartIndex > _parent._toOffset) return true;

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

                if (context.Stop.StartIndex > _parent._toOffset) return true;



                _parent.CurrentEvent = null;

                _parent._nestableExpressionElementStack.Clear();
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

                if (_parent._toOffset >= context.Start.StartIndex && context.open_brace.Text != "{")
                {
                    return true;
                }

                if (context.open_brace != null && (_parent._toOffset < context.open_brace.StartIndex &&
                      _parent._toOffset >= context.Start.StartIndex))
                {
                    return true;
                }


                CodeAreaId++;
                ScopeLevel++;
                ScopeId++;


                _parent.InState = true;

                _parent.CurrentState = context.state_name != null ? context.state_name.Text : null;

                base.VisitDefaultState(context);


                if (context.Stop.StartIndex > _parent._toOffset || context.Stop.Text != "}") return true;

                _parent._nestableExpressionElementStack.Clear();
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

                if (_parent._toOffset >= context.Start.StartIndex && context.open_brace.Text != "{")
                {
                    return true;
                }

                if (context.open_brace != null && (_parent._toOffset < context.open_brace.StartIndex &&
                      _parent._toOffset >= context.Start.StartIndex))
                {
                    return true;
                }


                CodeAreaId++;
                ScopeLevel++;
                ScopeId++;


                _parent.InState = true;

                _parent.CurrentState = context.state_name != null ? context.state_name.Text : null;

                base.VisitDefinedState(context);

                if (context.Stop.StartIndex > _parent._toOffset || context.Stop.Text != "}") return true;

                _parent._nestableExpressionElementStack.Clear();
                _parent.InState = false;

                ScopeLevel--;

                return true;
            }



            private void EnterSingleStatementCodeScope()
            {
                CodeScopeLevel++;
                ScopeLevel++;
                _parent.InSingleStatementCodeScopeTopLevel = true;
            }



            public override bool VisitCodeStatement(LSLParser.CodeStatementContext context)
            {
                if (context.Parent is LSLParser.CodeScopeOrSingleBlockStatementContext)
                {

                    if (_parent._toOffset >= context.Start.StartIndex)
                    {
                        if (_parent._toOffset >= context.Start.StartIndex && context.Stop.Text == ";")
                        {
                            _parent.InControlStatementSourceRange = false;

                            CodeScopeLevel--;
                            ScopeLevel--;
                            _parent.InSingleStatementCodeScopeTopLevel = false;
                        }
                        else if (_parent._toOffset <= context.Start.StartIndex)
                        {
                            _parent.InSingleStatementCodeScopeTopLevel = false;
                        }
                    }

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

                    if (context.Stop.StopIndex > _parent._toOffset || context.Stop.Text != ";") return true;


                    _parent._nestableExpressionElementStack.Clear();

                    _parent.InMultiStatementCodeScopeTopLevel = prevMulti;
                    _parent.InSingleStatementCodeScopeTopLevel = prevSign;
                }

                return true;
            }




            public override bool VisitCodeScope(LSLParser.CodeScopeContext context)
            {
                if (context.Start.StartIndex > _parent._toOffset) return true;

                

                if (!_parent.InSingleStatementCodeScopeTopLevel)
                {
                    CodeScopeLevel++;
                    ScopeLevel++;
                    ScopeId++;
                }


                _parent.InSingleStatementCodeScopeTopLevel = false;
                _parent.InMultiStatementCodeScopeTopLevel = true;

                _globalVariablesHidden.Push(new List<GlobalVariable>());


                if (context.Parent is LSLParser.FunctionDeclarationContext ||
                    context.Parent is LSLParser.EventHandlerContext)
                {
                    foreach (var parameters in _parent.LocalParameters)
                    {
                        GlobalVariable val;
                        if (_parent._globalVariables.TryGetValue(parameters.Name, out val))
                        {
                            _parent._globalVariables.Remove(val.Name);
                            _globalVariablesHidden.Peek().Add(val);
                        }
                    }
                }

                _parent._lastControlChainElementStack.Push(new LastControlChainStatusContainer());
                _parent._localVariables.Push(new Dictionary<string, LocalVariable>());

                foreach (var i in context.codeStatement())
                {
                    if (i.Start.StartIndex > _parent._toOffset)
                    {
                        return true;
                    }

                    _parent._lastControlChainElementStack.Peek().IsIfOrElseIf = false;
                    VisitCodeStatement(i);
                }

                if (context.Stop.StartIndex > _parent._toOffset) return true;


                foreach (var var in _globalVariablesHidden.Peek())
                {
                    _parent._globalVariables.Add(var.Name, var);
                }

                _globalVariablesHidden.Pop();

                _parent._lastControlChainElementStack.Pop();
                _parent._localVariables.Pop();


                CodeScopeLevel--;
                if (CodeScopeLevel == 0)
                {
                    _parent._nestableExpressionElementStack.Clear();
                    _parent.InMultiStatementCodeScopeTopLevel = false;
                }

                ScopeLevel--;


                return true;
            }



            public override bool VisitExpr_AddSub(LSLParser.Expr_AddSubContext context)
            {
                if (context.operation.StopIndex <= _parent._toOffset &&
                    (context.expr_rvalue == null || _parent._toOffset <= context.expr_rvalue.Start.StartIndex))
                {
                    _parent.InBasicExpressionTree = true;
                }

                return base.VisitExpr_AddSub(context);
            }

            public override bool VisitExpr_BitwiseAnd(LSLParser.Expr_BitwiseAndContext context)
            {
                if (context.operation.StopIndex <= _parent._toOffset && 
                    (context.expr_rvalue == null || _parent._toOffset <= context.expr_rvalue.Start.StartIndex))
                {
                    _parent.InBasicExpressionTree = true;
                }

                return base.VisitExpr_BitwiseAnd(context);
            }

            public override bool VisitExpr_BitwiseOr(LSLParser.Expr_BitwiseOrContext context)
            {
                if (context.operation.StopIndex <= _parent._toOffset &&
                    (context.expr_rvalue == null || _parent._toOffset <= context.expr_rvalue.Start.StartIndex))
                {
                    _parent.InBasicExpressionTree = true;
                }

                return base.VisitExpr_BitwiseOr(context);
            }

            public override bool VisitExpr_BitwiseShift(LSLParser.Expr_BitwiseShiftContext context)
            {
                if (context.operation.StopIndex <= _parent._toOffset &&
                    (context.expr_rvalue == null || _parent._toOffset <= context.expr_rvalue.Start.StartIndex))
                {
                    _parent.InBasicExpressionTree = true;
                }

                return base.VisitExpr_BitwiseShift(context);
            }

            public override bool VisitExpr_BitwiseXor(LSLParser.Expr_BitwiseXorContext context)
            {
                if (context.operation.StopIndex <= _parent._toOffset &&
                    (context.expr_rvalue == null || _parent._toOffset <= context.expr_rvalue.Start.StartIndex))
                {
                    _parent.InBasicExpressionTree = true;
                }

                return base.VisitExpr_BitwiseXor(context);
            }



            public override bool VisitExpr_LogicalAnd(LSLParser.Expr_LogicalAndContext context)
            {
                if (context.operation.StopIndex <= _parent._toOffset && 
                    (context.expr_rvalue == null || _parent._toOffset <= context.expr_rvalue.Start.StartIndex))
                {
                    _parent.InBasicExpressionTree = true;
                }

                return base.VisitExpr_LogicalAnd(context);
            }

            public override bool VisitExpr_LogicalCompare(LSLParser.Expr_LogicalCompareContext context)
            {
                if (context.operation.StopIndex <= _parent._toOffset && 
                    (context.expr_rvalue == null || _parent._toOffset <= context.expr_rvalue.Start.StartIndex))
                {
                    _parent.InBasicExpressionTree = true;
                }
                return base.VisitExpr_LogicalCompare(context);
            }

            public override bool VisitExpr_LogicalEquality(LSLParser.Expr_LogicalEqualityContext context)
            {
                if (context.operation.StopIndex <= _parent._toOffset &&
                    (context.expr_rvalue == null || _parent._toOffset <= context.expr_rvalue.Start.StartIndex))
                {
                    _parent.InBasicExpressionTree = true;
                }
                return base.VisitExpr_LogicalEquality(context);
            }

            public override bool VisitExpr_LogicalOr(LSLParser.Expr_LogicalOrContext context)
            {
                if (context.operation.StopIndex <= _parent._toOffset && 
                    (context.expr_rvalue == null || _parent._toOffset <= context.expr_rvalue.Start.StartIndex))
                {
                    _parent.InBasicExpressionTree = true;
                }
                return base.VisitExpr_LogicalOr(context);
            }

            public override bool VisitExpr_MultDivMod(LSLParser.Expr_MultDivModContext context)
            {
                if (context.operation.StopIndex <= _parent._toOffset &&
                    (context.expr_rvalue == null || _parent._toOffset <= context.expr_rvalue.Start.StartIndex))
                {
                    _parent.InBasicExpressionTree = true;
                }
                return base.VisitExpr_MultDivMod(context);
            }

            public override bool VisitExpr_PrefixOperation(LSLParser.Expr_PrefixOperationContext context)
            {
                if (context.operation.StopIndex <= _parent._toOffset && 
                    (context.expr_rvalue == null || _parent._toOffset <= context.expr_rvalue.Start.StartIndex))
                {
                    _parent.InBasicExpressionTree = true;
                }
                return base.VisitExpr_PrefixOperation(context);
            }


            public override bool VisitExpr_TypeCast(LSLParser.Expr_TypeCastContext context)
            {
                if (context.close_parenth != null && context.close_parenth.StartIndex <= _parent._toOffset &&
                    (context.expr_rvalue == null || _parent._toOffset <= context.expr_rvalue.Start.StartIndex))
                {
                    _parent.InBasicExpressionTree = true;
                }

                return base.VisitExpr_TypeCast(context);
            }

            public override bool VisitParenthesizedExpression(LSLParser.ParenthesizedExpressionContext context)
            {
                if (context.open_parenth != null && context.open_parenth.StartIndex <= _parent._toOffset &&
                    context.close_parenth != null && context.close_parenth.Text == ")" &&
                    context.close_parenth.StartIndex >= _parent._toOffset)
                {
                    _parent.InBasicExpressionTree = true;
                }


                return base.VisitParenthesizedExpression(context);
            }


            public override bool VisitExpr_DotAccessor(LSLParser.Expr_DotAccessorContext context)
            {
                if (context.operation.StopIndex <= _parent._toOffset &&
                    (context.member == null || _parent._toOffset <= context.member.StartIndex))
                {
                    _parent.RightOfDotAccessor = true;
                }

                return base.VisitExpr_DotAccessor(context);
            }
        };
    }
}