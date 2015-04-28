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
        private readonly List<GlobalFunction> _globalFunctions = new List<GlobalFunction>();
        private readonly List<GlobalVariable> _globalVariables = new List<GlobalVariable>();
        private readonly List<LocalVariable> _localVariables = new List<LocalVariable>();
        private readonly List<LocalParameter> _parameters = new List<LocalParameter>();
        private readonly List<StateBlock> _stateBlocks = new List<StateBlock>();

        public IReadOnlyList<StateBlock> StateBlocks
        {
            get { return _stateBlocks; }
        }

        public StateBlock DefaultState { get; private set; }

        public IReadOnlyList<GlobalVariable> GlobalVariables
        {
            get { return _globalVariables; }
        }

        public IReadOnlyList<LocalVariable> LocalVariables
        {
            get { return _localVariables; }
        }

        public IReadOnlyList<GlobalFunction> GlobalFunctions
        {
            get { return _globalFunctions; }
        }

        public IReadOnlyList<LocalParameter> LocalParameters
        {
            get { return _parameters; }
        }

        private static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            if (val.CompareTo(max) > 0) return max;
            return val;
        }

        public IEnumerable<LocalParameter> GetLocalParameters(ScopeAddress address, int offset)
        {
            return LocalParameters.Where(
                x =>
                    x.ScopeAddress.CodeAreaId == address.CodeAreaId &&
                    x.ScopeAddress.ScopeId <= address.ScopeId &&
                    x.ScopeAddress.ScopeLevel == address.ScopeLevel &&
                    x.SourceCodeRange.StartIndex < offset);
        }

        public IEnumerable<LocalVariable> GetLocalVariables(ScopeAddress address, int offset)
        {
            return LocalVariables.Where(
                x =>
                    x.ScopeAddress.CodeAreaId == address.CodeAreaId &&
                    x.ScopeAddress.ScopeId <= address.ScopeId &&
                    x.ScopeAddress.ScopeLevel == address.ScopeLevel &&
                    x.SourceCodeRange.StartIndex < offset);
        }

        public static ScopeAddress FastParseToOffset(string text, int offset)
        {
            LSLCommentStringSkipper skipper = new LSLCommentStringSkipper();
            bool inState = false;
            int level = 0;
            int id = 0;
            int codeAreaId = 0;


            offset = Clamp(offset, 0, text.Length);

            for (int i = 0; i < offset; i++)
            {
                skipper.FeedChar(text, i, offset);


                if (!(skipper.InComment || skipper.InString))
                {
                    int defaultStopOffset = i + 7;
                    int stateStopOffset = i + 5;

                    if ((defaultStopOffset < text.Length) && text.Substring(i, 7) == "default")
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
                    if (!inState && (stateStopOffset < text.Length) && text.Substring(i, 5) == "state")
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

                        if (!inState) continue;

                        level++;
                        id++;
                        codeAreaId++;

                    }
                    else if (text[i] == '{')
                    {
                        level++;
                        if ((inState && level == 2) || (!inState && level == 1))
                        {
                            codeAreaId++;
                        }
                        id++;
                    }
                    else if (text[i] == '}')
                    {

                        level--;
                        if (inState && level == 0)
                        {
                            inState = false;
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

        public void Parse(TextReader stream)
        {
            _globalFunctions.Clear();
            _globalVariables.Clear();
            _localVariables.Clear();
            _parameters.Clear();

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
            public bool InComment { get; set; }
            public bool InString { get; set; }
            public bool InState { get; set; }
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
            private bool _inState;
            private int _scopeId;
            private int _scopeLevel;

            public Visitor(LSLFastEditorParse parent)
            {
                _parent = parent;
            }

            public override bool VisitGlobalVariableDeclaration(LSLParser.GlobalVariableDeclarationContext context)
            {
                if (context.variable_type == null || context.variable_name == null) return true;

                _parent._globalVariables.Add(
                    new GlobalVariable(
                        context.variable_name.Text,
                        context.variable_type.Text,
                        new LSLSourceCodeRange(context)));

                return true;
            }

            public override bool VisitLocalVariableDeclaration(LSLParser.LocalVariableDeclarationContext context)
            {
                if (context.variable_type == null || context.variable_name == null) return true;

                _parent._localVariables.Add(
                    new LocalVariable(
                        context.variable_name.Text,
                        context.variable_type.Text,
                        new LSLSourceCodeRange(context),
                        new ScopeAddress(_codeAreaId, _scopeId, _scopeLevel) {InState = _inState}));

                return true;
            }

            public override bool VisitFunctionDeclaration(LSLParser.FunctionDeclarationContext context)
            {
                string returnTypeText = context.return_type == null ? "" : context.return_type.Text;

                if (context.function_name == null || context.code == null) return true;


                _codeAreaId++;

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

                            var parm = new LocalParameter(
                                i.parameter_name.Text,
                                i.parameter_type.Text,
                                new LSLSourceCodeRange(i),
                                new ScopeAddress(_codeAreaId, _scopeId + 1, _scopeLevel + 1));

                            parms.Add(parm);
                            _parent._parameters.Add(parm);
                        }
                    }
                }

                _parent._globalFunctions.Add(new GlobalFunction(context.function_name.Text, returnTypeText,
                    new LSLSourceCodeRange(context), parms));


                return base.VisitFunctionDeclaration(context);
            }

            public override bool VisitEventHandler(LSLParser.EventHandlerContext context)
            {
                _codeAreaId++;

                if (context.parameters != null && context.parameters.children != null)
                {
                    var list = context.parameters.parameterList();
                    if (list != null)
                    {
                        foreach (var child in list.children)
                        {
                            var i = child as LSLParser.ParameterDefinitionContext;

                            if (i == null) continue;

                            _parent._parameters.Add(
                                new LocalParameter(
                                    i.parameter_name.Text,
                                    i.parameter_type.Text,
                                    new LSLSourceCodeRange(i),
                                    new ScopeAddress(_codeAreaId, _scopeId + 1, _scopeLevel + 1)));
                        }
                    }
                }

                return base.VisitEventHandler(context);
            }

            public override bool VisitDefaultState(LSLParser.DefaultStateContext context)
            {
                _parent.DefaultState = new StateBlock("default", new LSLSourceCodeRange(context));

                _codeAreaId++;
                _scopeLevel++;
                _scopeId++;
                _inState = true;

                base.VisitDefaultState(context);

                _inState = false;
                _scopeLevel--;
                return true;
            }

            public override bool VisitDefinedState(LSLParser.DefinedStateContext context)
            {
                if (context.state_name == null) return true;


                _parent._stateBlocks.Add(new StateBlock(context.state_name.Text,
                    new LSLSourceCodeRange(context)));

                _codeAreaId++;
                _scopeLevel++;
                _scopeId++;
                _inState = true;

                base.VisitDefinedState(context);

                _inState = false;
                _scopeLevel--;
                return true;
            }

            public override bool VisitCodeScope(LSLParser.CodeScopeContext context)
            {
                _scopeLevel++;
                _scopeId++;

                base.VisitCodeScope(context);

                _scopeLevel--;
                return true;
            }
        };
    }
}