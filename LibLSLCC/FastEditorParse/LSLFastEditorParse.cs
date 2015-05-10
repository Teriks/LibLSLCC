#region


using System;
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

        public bool AfterDefaultState
        {
            get { return !InGlobalScope && !InState; }
        }

        public bool InStateOutsideEvent
        {
            get { return (InState && !InEventHandler && !InBetweenStateNameEndAndCodeStart); }
        }

        public bool InState { get; private set; }

        public bool InBetweenStateNameEndAndCodeStart { get; private set; }
        public bool InEventHandler { get; private set; }
        public bool InFunctionDeclaration { get; private set; }
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

        private static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            if (val.CompareTo(max) > 0) return max;
            return val;
        }

        public static ScopeAddress FastParseToOffset(string text, int offset)
        {
            LSLCommentStringSkipper skipper = new LSLCommentStringSkipper();
            bool inState = false;
            bool inEvent = false;
            bool inFunction = false;
            int level = 0;
            int id = 0;
            int codeAreaId = 0;


            offset = Clamp(offset, 0, text.Length);

            for (int i = 0; i < offset; i++)
            {
                if (text[i] == '"')
                {
                    Console.Write("TEST");
                }
                skipper.FeedChar(text, i, offset);


                if (!(skipper.InComment || skipper.InString))
                {
                    int defaultStopOffset = i + 7;
                    int stateStopOffset = i + 5;

                    if (!inEvent && !inFunction && !inState && (defaultStopOffset < text.Length) &&
                        text.Substring(i, 7) == "default")
                    {
                        inState = true;

                        if (text[defaultStopOffset] == '{')
                        {
                            i = defaultStopOffset + 1;
                        }
                        else
                        {
                            var dstateSkipper = new LSLCommentStringSkipper();

                            var o = defaultStopOffset;
                            while (o < offset)
                            {
                                var c = text[o];


                                if (c == '{') break;

                                dstateSkipper.FeedChar(text, o, offset);
                                if (!(dstateSkipper.InComment || dstateSkipper.InString) && !char.IsWhiteSpace(c))
                                {
                                    inState = false;
                                    break;
                                }

                                o++;
                            }

                            if (inState) i = o + 1;
                        }

                        if (inState)
                        {
                            level++;
                            id++;
                            codeAreaId++;
                        }
                    }
                    if (!inEvent && !inFunction && !inState && (stateStopOffset < text.Length) &&
                        text.Substring(i, 5) == "state")
                    {
                        inState = true;

                        if (text[stateStopOffset] == '{')
                        {
                            i = stateStopOffset + 1;
                        }
                        else
                        {
                            var stateSkipper = new LSLCommentStringSkipper();
                            bool spacesBetween = false;
                            var o = stateStopOffset;
                            while (o < offset)
                            {
                                var c = text[o];


                                if (c == ';')
                                {
                                    inState = false;
                                    break;
                                }
                                if (c == '{')
                                {
                                    break;
                                }

                                stateSkipper.FeedChar(text, o, offset);

                                if (stateSkipper.InComment)
                                {
                                    spacesBetween = true;
                                }
                                else if (c == ' ')
                                {
                                    spacesBetween = true;
                                }

                                if (!(stateSkipper.InComment || stateSkipper.InString) && !char.IsWhiteSpace(c) &&
                                    !spacesBetween)
                                {
                                    inState = false;
                                    break;
                                }

                                o++;
                            }

                            if (inState) i = o + 1;
                        }

                        if (inState)
                        {
                            level++;
                            id++;
                            codeAreaId++;
                        }
                    }
                    else if (text[i] == '{' && !skipper.InComment && !skipper.InString)
                    {
                        level++;
                        if (!inEvent && (inState && level == 2))
                        {
                            inEvent = true;
                            codeAreaId++;
                        }
                        else if (!inFunction && !inState && level == 1)
                        {
                            char t = text[i];
                            inFunction = true;
                            codeAreaId++;
                        }
                        id++;
                    }
                    else if (text[i] == '}' && !skipper.InComment && !skipper.InString)
                    {
                        level--;
                        if (inState && level == 0)
                        {
                            inState = false;
                        }
                        else if (inState && inEvent && level == 1)
                        {
                            inEvent = false;
                        }
                        else if (inFunction && level == 0)
                        {
                            inFunction = false;
                        }
                    }
                }
            }
            return new ScopeAddress(codeAreaId, id, level)
            {
                InState = inState,
                InString = skipper.InString,
                InComment = skipper.InComment
            };
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
                if (context.Start.StartIndex > _parent._toOffset) return true;

                if (context.variable_type == null || context.variable_name == null) return true;

                var variable =
                    new GlobalVariable(
                        context.variable_name.Text,
                        context.variable_type.Text,
                        new LSLSourceCodeRange(context));


                if (!_parent._globalVariables.ContainsKey(context.variable_name.Text))
                {
                    _parent._globalVariables.Add(context.variable_name.Text, variable);
                }


                return true;
            }

            public override bool VisitLocalVariableDeclaration(LSLParser.LocalVariableDeclarationContext context)
            {
                if (context.Start.StartIndex > _parent._toOffset) return true;

                if (context.variable_type == null || context.variable_name == null) return true;


                var variable = new LocalVariable(
                    context.variable_name.Text,
                    context.variable_type.Text,
                    new LSLSourceCodeRange(context),
                    new ScopeAddress(_codeAreaId, _scopeId, _scopeLevel)
                    {
                        InState = _parent.InState,
                        InFunction = _parent.InFunctionDeclaration,
                        InEvent = _parent.InEventHandler
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

                return true;
            }

            public override bool VisitFunctionDeclaration(LSLParser.FunctionDeclarationContext context)
            {
                if (context.Start.StartIndex > _parent._toOffset) return true;

                string returnTypeText = context.return_type == null ? "" : context.return_type.Text;

                if (context.function_name == null || context.code == null) return true;


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


                _parent.InFunctionDeclaration = true;

                _parent.CurrentFunction = context.function_name != null ? context.function_name.Text : null;

                base.VisitFunctionDeclaration(context);

                if (context.Stop.StartIndex > _parent._toOffset) return true;

                _parent.InFunctionDeclaration = false;

                return true;
            }



            public override bool VisitEventHandler(LSLParser.EventHandlerContext context)
            {

                if ((context.handler_name.StopIndex + 1) == _parent._toOffset)
                {
                    return true;
                }

                if ((context.handler_name.StopIndex < _parent._toOffset) &&
                    (_parent._toOffset < context.code.Start.StartIndex))
                {
                    _parent.InBetweenStateNameEndAndCodeStart = true;
                    return true;
                }

                if (context.Start.StartIndex > _parent._toOffset) return true;

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

                _parent.InEventHandler = true;

                _parent.CurrentEvent = context.handler_name != null ? context.handler_name.Text : null;

                base.VisitEventHandler(context);

                if (context.Stop.StartIndex > _parent._toOffset) return true;

                _parent.InEventHandler = false;

                return true;
            }



            public override bool VisitDefaultState(LSLParser.DefaultStateContext context)
            {
                if (context.Start.StartIndex > _parent._toOffset) return true;

                _parent.DefaultState = new StateBlock("default", new LSLSourceCodeRange(context));

                _codeAreaId++;
                _scopeLevel++;
                _scopeId++;


                _parent.InGlobalScope = false;
                _parent.InState = true;

                _parent.CurrentState = context.state_name != null ? context.state_name.Text : null;

                base.VisitDefaultState(context);


                if (context.Stop.StartIndex > _parent._toOffset) return true;

                _parent.InState = false;


                _scopeLevel--;


                return true;
            }

            public override bool VisitDefinedState(LSLParser.DefinedStateContext context)
            {
                if (context.Start.StartIndex > _parent._toOffset) return true;

                if (context.state_name == null) return true;


                _parent._stateBlocks.Add(new StateBlock(context.state_name.Text,
                    new LSLSourceCodeRange(context)));

                _codeAreaId++;
                _scopeLevel++;
                _scopeId++;


                _parent.InState = true;

                _parent.CurrentState = context.state_name != null ? context.state_name.Text : null;

                base.VisitDefinedState(context);

                if (context.Stop.StartIndex > _parent._toOffset) return true;


                _parent.InState = false;

                _scopeLevel--;

                return true;
            }

            public override bool VisitCodeScope(LSLParser.CodeScopeContext context)
            {
                if (context.Start.StartIndex > _parent._toOffset) return true;

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

                if (context.Stop.StartIndex > _parent._toOffset) return true;

                _parent._localVariables.Pop();

                _scopeLevel--;
                return true;
            }
        };
    }
}