﻿#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Primitives;

#endregion

namespace LibLSLCC.FastEditorParse
{
    public class LSLFastEditorParse
    {
        private readonly Dictionary<string, GlobalFunction> _globalFunctions = new Dictionary<string, GlobalFunction>();
        private readonly Dictionary<string, GlobalVariable> _globalVariables = new Dictionary<string, GlobalVariable>();

        private readonly Stack<Dictionary<string, LocalVariable>> _localVariables =
            new Stack<Dictionary<string, LocalVariable>>();

        private readonly Dictionary<string, LocalParameter> _parameters = new Dictionary<string, LocalParameter>();
        private readonly List<StateBlock> _stateBlocks = new List<StateBlock>();
        private int _toOffset;

        public LSLFastEditorParse()
        {
            InGlobalScope = true;
        }

        public string CurrentState { get; private set; }
        public string CurrentFunction { get; private set; }
        public string CurrentEvent { get; private set; }

        public bool InStateOutsideEvent
        {
            get { return (InState && !InEventCode && !InEventContextRange); }
        }

        public bool InState { get; private set; }
        public bool InEventContextRange { get; private set; }
        public bool InEventCode { get; private set; }
        public bool InFunctionCode { get; private set; }
        public bool InGlobalScope { get; private set; }

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
        public bool InFunctionCallParameterList { get; private set; }

        public bool InCodeArea
        {
            get { return InFunctionCode || InEventCode; }
        }

        public bool InExpressionStatementArea
        {
            get { return InCodeArea && !InExpressionArea; }
        }

        public bool InLocalExpressionArea
        {
            get
            {
                return InLocalVariableDeclarationExpression || InIfConditionExpression ||
                       InElseIfConditionExpression || InFunctionCallParameterList;
            }
        }

        public bool InGlobalExpressionArea
        {
            get { return InGlobalVariableDeclarationExpression; }
        }

        public bool InExpressionArea
        {
            get { return InGlobalExpressionArea || InLocalExpressionArea; }
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
        }

        public class GlobalVariable
        {
            public GlobalVariable(string name, string type, LSLSourceCodeRange range)
            {
                Name = name;
                Type = type;
                SourceCodeRange = range;
            }

            public string Name { get; private set; }
            public string Type { get; private set; }
            public LSLSourceCodeRange SourceCodeRange { get; private set; }
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
            public GlobalFunction(string name, string type, LSLSourceCodeRange range,
                IReadOnlyList<LocalParameter> parameters)
            {
                Parameters = parameters;
                Name = name;
                ReturnType = type;
                SourceCodeRange = range;
            }

            public string Name { get; private set; }
            public string ReturnType { get; private set; }

            public string Signature
            {
                get
                {
                    string sig = "";
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

            public IReadOnlyList<LocalParameter> Parameters { get; private set; }
            public LSLSourceCodeRange SourceCodeRange { get; private set; }
        }

        public class ScopeAddress
        {
            public ScopeAddress(int codeAreaId, int scopeId, int scopeLevel)
            {
                CodeAreaId = codeAreaId;
                ScopeId = scopeId;
                ScopeLevel = scopeLevel;
                InComment = false;
                InString = false;
                InState = false;
            }

            public int CodeAreaId { get; private set; }
            public int ScopeLevel { get; private set; }
            public int ScopeId { get; private set; }
            public bool InComment { get; internal set; }
            public bool InString { get; internal set; }
            public bool InState { get; internal set; }
            public bool InEvent { get; internal set; }
            public bool InFunction { get; internal set; }
        }

        public class LocalVariable
        {
            public LocalVariable(string name, string type, LSLSourceCodeRange range, ScopeAddress address)
            {
                Name = name;
                Type = type;
                SourceCodeRange = range;
                ScopeAddress = address;
            }

            public ScopeAddress ScopeAddress { get; private set; }
            public string Name { get; private set; }
            public string Type { get; private set; }
            public LSLSourceCodeRange SourceCodeRange { get; private set; }
        }

        public class LocalParameter
        {
            public LocalParameter(string name, string type, LSLSourceCodeRange range, ScopeAddress address)
            {
                Name = name;
                Type = type;
                SourceCodeRange = range;
                ScopeAddress = address;
            }

            public ScopeAddress ScopeAddress { get; private set; }
            public string Name { get; private set; }
            public string Type { get; private set; }
            public LSLSourceCodeRange SourceCodeRange { get; private set; }
        }

        private class Visitor : LSLBaseVisitor<bool>
        {
            private readonly LSLFastEditorParse _parent;
            private int _codeAreaId;
            private int _scopeId;
            private int _scopeLevel;

            public Visitor(LSLFastEditorParse parent)
            {
                _parent = parent;
            }

            public override bool VisitGlobalVariableDeclaration(LSLParser.GlobalVariableDeclarationContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                _parent.InGlobalScope = false;


                var variable =
                    new GlobalVariable(
                        context.variable_name.Text,
                        context.variable_type.Text,
                        new LSLSourceCodeRange(context));


                if (!_parent._globalVariables.ContainsKey(context.variable_name.Text))
                {
                    _parent._globalVariables.Add(context.variable_name.Text, variable);
                }

                if (context.operation != null && context.operation.StartIndex < _parent._toOffset)
                {
                    _parent.InGlobalVariableDeclarationExpression = true;
                }


                if (context.semi_colon != null && context.semi_colon.Text == ";" &&
                    context.semi_colon.StartIndex < _parent._toOffset)
                {
                    _parent.InGlobalVariableDeclarationExpression = false;
                    _parent.InGlobalScope = true;
                }


                return true;
            }

            public override bool VisitIfStatement(LSLParser.IfStatementContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                if (context.open_parenth != null && context.open_parenth.Text == "(" &&
                    context.open_parenth.StartIndex < _parent._toOffset)
                {
                    _parent.InIfConditionExpression = true;
                }

                if (context.close_parenth != null && context.close_parenth.Text == ")" &&
                    context.close_parenth.StartIndex < _parent._toOffset)
                {
                    _parent.InIfConditionExpression = false;
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
                    _parent.InElseIfConditionExpression = false;
                }

                return base.VisitElseIfStatement(context);
            }

            public override bool VisitExpr_FunctionCall(LSLParser.Expr_FunctionCallContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                if (context.open_parenth != null && context.open_parenth.Text == "(" &&
                    context.open_parenth.StartIndex < _parent._toOffset)
                {
                    _parent.InFunctionCallParameterList = true;
                }

                if (context.close_parenth != null && context.close_parenth.Text == ")" &&
                    context.close_parenth.StartIndex < _parent._toOffset)
                {
                    _parent.InFunctionCallParameterList = false;
                }

                return base.VisitExpr_FunctionCall(context);
            }

            public override bool VisitLocalVariableDeclaration(LSLParser.LocalVariableDeclarationContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;


                var variable = new LocalVariable(
                    context.variable_name.Text,
                    context.variable_type.Text,
                    new LSLSourceCodeRange(context),
                    new ScopeAddress(_codeAreaId, _scopeId, _scopeLevel)
                    {
                        InState = _parent.InState,
                        InFunction = _parent.InFunctionCode,
                        InEvent = _parent.InEventCode
                    });


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
                    _parent.InLocalVariableDeclarationExpression = false;
                }


                return true;
            }

            public override bool VisitFunctionDeclaration(LSLParser.FunctionDeclarationContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                string returnTypeText = context.return_type == null ? "" : context.return_type.Text;

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


                if (context.code.open_brace.Text == "<missing '}'>" ||
                    context.code.open_brace.StartIndex >= _parent._toOffset) return true;


                _codeAreaId++;

                _parent._parameters.Clear();


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
                            if (_parent._parameters.ContainsKey(i.parameter_name.Text)) continue;

                            var parm = new LocalParameter(
                                i.parameter_name.Text,
                                i.parameter_type.Text,
                                new LSLSourceCodeRange(i),
                                new ScopeAddress(_codeAreaId, _scopeId + 1, _scopeLevel + 1));

                            parms.Add(parm);
                            _parent._parameters.Add(parm.Name, parm);
                        }
                    }
                }


                if (!_parent._globalFunctions.ContainsKey(context.function_name.Text))
                {
                    _parent._globalFunctions.Add(
                        context.function_name.Text,
                        new GlobalFunction(context.function_name.Text, returnTypeText,
                            new LSLSourceCodeRange(context), parms));
                }


                _parent.InFunctionCode = true;

                _parent.CurrentFunction = context.function_name != null ? context.function_name.Text : null;

                base.VisitFunctionDeclaration(context);

                if ((context.Stop.StartIndex - 1) > _parent._toOffset) return true;

                _parent.InFunctionCode = false;
                _parent.InGlobalScope = true;

                return true;
            }

            public override bool VisitEventHandler(LSLParser.EventHandlerContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;


                _parent.InEventContextRange = true;


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


                if (context.code.open_brace.Text == "<missing '}'>" ||
                    context.code.open_brace.StartIndex >= _parent._toOffset) return true;

                _codeAreaId++;

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
                                new ScopeAddress(_codeAreaId, _scopeId + 1, _scopeLevel + 1));

                            _parent._parameters.Add(parm.Name, parm);
                        }
                    }
                }

                _parent.InEventCode = true;

                _parent.CurrentEvent = context.handler_name != null ? context.handler_name.Text : null;

                base.VisitEventHandler(context);

                if ((context.Stop.StartIndex) >= _parent._toOffset) return true;

                _parent.InEventCode = false;

                _parent.InEventContextRange = false;

                return true;
            }

            public override bool VisitDefaultState(LSLParser.DefaultStateContext context)
            {
                if (_parent._toOffset <= context.Start.StartIndex) return true;

                _parent.InGlobalScope = false;


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

                _parent.DefaultState = new StateBlock("default", new LSLSourceCodeRange(context));

                _codeAreaId++;
                _scopeLevel++;
                _scopeId++;


                _parent.InState = true;

                _parent.CurrentState = context.state_name != null ? context.state_name.Text : null;

                base.VisitDefaultState(context);


                if ((context.Stop.StartIndex) >= _parent._toOffset) return true;

                _parent.InState = false;


                _scopeLevel--;


                return true;
            }

            public override bool VisitDefinedState(LSLParser.DefinedStateContext context)
            {
                if (_parent._toOffset <= context.Start.StartIndex) return true;

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

                _parent._stateBlocks.Add(new StateBlock(context.state_name.Text,
                    new LSLSourceCodeRange(context)));

                _codeAreaId++;
                _scopeLevel++;
                _scopeId++;


                _parent.InState = true;

                _parent.CurrentState = context.state_name != null ? context.state_name.Text : null;

                base.VisitDefinedState(context);

                if ((context.Stop.StartIndex) >= _parent._toOffset) return true;


                _parent.InState = false;

                _scopeLevel--;

                return true;
            }

            public override bool VisitCodeScope(LSLParser.CodeScopeContext context)
            {
                if (context.Start.StartIndex >= _parent._toOffset) return true;

                _scopeLevel++;
                _scopeId++;

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

                _scopeLevel--;
                return true;
            }
        };
    }
}