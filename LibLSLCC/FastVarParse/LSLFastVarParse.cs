using System;
using System.IO;
using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.Visitor;
using System.Collections.Generic;
using LibLSLCC.CodeValidator.Primitives;
using System.Linq;

namespace LibLSLCC.FastVarParser
{
    public class LSLFastVarParse
    {

        public class GlobalVariable
        {
            public string Name { get; private set; }
            public string Type { get; private set; }

            public LSLSourceCodeRange SourceCodeRange { get; private set; }

            public GlobalVariable(string name, string type, LSLSourceCodeRange range)
            {
                this.Name = name;
                this.Type = type;
                this.SourceCodeRange = range;
            }
        }


        public class StateBlock
        {
            public string Name { get; private set; }

            public LSLSourceCodeRange SourceCodeRange { get; private set; }

            public StateBlock(string name, LSLSourceCodeRange range)
            {
                this.Name = name;
                this.SourceCodeRange = range;
            }
        }


        public class GlobalFunction
        {
            public string Name { get; private set; }
            public string ReturnType { get; private set; }

            public string Signature
            {
                get
                {
                    string sig ="";
                    if(ReturnType!=""){
                        sig+=ReturnType+" ";
                    }

                    sig += Name + "(";

                    if (Parameters.Count > 0)
                    {
                        sig += string.Join(", ", Parameters.Select(x=>x.Type+" "+x.Name));
                    }
                    sig+=");";

                    return sig;
                }
            }


            public IReadOnlyList<LocalParameter> Parameters { get; private set; }

            public LSLSourceCodeRange SourceCodeRange { get; private set; }

            public GlobalFunction(string name, string type, LSLSourceCodeRange range,  IReadOnlyList<LocalParameter> parameters)
            {
                this.Parameters = parameters;
                this.Name = name;
                this.ReturnType = type;
                this.SourceCodeRange = range;
            }
        }

        public class ScopeAddress
        {
            public int CodeAreaID { get; private set; }

            public int ScopeLevel { get; private set; }

            public int ScopeID { get; private set; }

            public bool InComment { get; set; }

            public bool InString { get; set; }

            public bool InState { get; set; }


            public ScopeAddress(int codeAreaID, int scopeID, int scopeLevel)
            {
                CodeAreaID = codeAreaID;
                ScopeID = scopeID;
                ScopeLevel = scopeLevel;
                InComment = false;
                InString = false;
                InState = false;
            }

        }

        public class LocalVariable
        {
            public ScopeAddress ScopeAddress { get; private set; }

            public string Name { get; private set; }
            public string Type { get; private set; }

            public LSLSourceCodeRange SourceCodeRange { get; private set; }

            public LocalVariable(string name, string type, LSLSourceCodeRange range, ScopeAddress address)
            {
                this.Name = name;
                this.Type = type;
                this.SourceCodeRange = range;
                this.ScopeAddress = address;
            }
        }


        public class LocalParameter
        {
            public ScopeAddress ScopeAddress { get; private set; }

            public string Name { get; private set; }
            public string Type { get; private set; }

            public LSLSourceCodeRange SourceCodeRange { get; private set; }

            public LocalParameter(string name, string type, LSLSourceCodeRange range, ScopeAddress address)
            {
                this.Name = name;
                this.Type = type;
                this.SourceCodeRange = range;
                this.ScopeAddress = address;
            }
        }


        List<StateBlock> _stateBlocks = new List<StateBlock>();

        public IReadOnlyList<StateBlock> StateBlocks
        {
            get { return _stateBlocks; }
        }

        public StateBlock DefaultState
        {
            get;
            private set;
        }

        List<GlobalVariable> _globalVariables = new List<GlobalVariable>();
        public IReadOnlyList<GlobalVariable> GlobalVariables
        {
            get { return _globalVariables; }
        }
        List<LocalVariable> _localVariables = new List<LocalVariable>();

        List<LocalParameter> _parameters = new List<LocalParameter>();
        public IReadOnlyList<LocalVariable> LocalVariables
        {
            get { return _localVariables; }
        }

        List<GlobalFunction> _globalFunctions = new List<GlobalFunction>();
        public IReadOnlyList<GlobalFunction> GlobalFunctions
        {
            get { return _globalFunctions; }
        }

        public IReadOnlyList<LocalParameter> LocalParameters
        {
            get { return _parameters; }
        }
        private class Visitor : LSLBaseVisitor<bool> {
            LSLFastVarParse Parent;
            int scopeID = 0;
            int scopeLevel = 0;
            int codeAreaID = 0;
            bool inState = false;


            public Visitor(LSLFastVarParse parent)
            {
                Parent = parent;
            }

            public override bool VisitGlobalVariableDeclaration(LSLParser.GlobalVariableDeclarationContext context)
            {
                if (context.variable_type == null || context.variable_name == null) return true;

                Parent._globalVariables.Add(new GlobalVariable(context.variable_name.Text, context.variable_type.Text, new LSLSourceCodeRange(context)));
                return true;
            }

            public override bool VisitLocalVariableDeclaration(LSLParser.LocalVariableDeclarationContext context)
            {
                if (context.variable_type == null || context.variable_name == null) return true;

                Parent._localVariables.Add(new LocalVariable(context.variable_name.Text, context.variable_type.Text, new LSLSourceCodeRange(context), new ScopeAddress(codeAreaID, scopeID, scopeLevel) { InState = inState }));
                return true;
            }

            public override bool VisitFunctionDeclaration(LSLParser.FunctionDeclarationContext context)
            {
                string returnTypeText = context.return_type == null ? "" : context.return_type.Text;

                if (context.function_name == null || context.code == null) return true;

                
                codeAreaID++;

                List<LocalParameter> parms = new List<LocalParameter>();

                if (context.parameters != null && context.parameters.children != null)
                {
                    var list = context.parameters.parameterList();
                    if (list != null)
                    {
                        foreach (var child in list.children)
                        {
                            var i = child as LSLParser.ParameterDefinitionContext;
                            if (i != null)
                            {
                                var parm = new LocalParameter(i.parameter_name.Text, i.parameter_type.Text, new LSLSourceCodeRange(i), new ScopeAddress(codeAreaID, scopeID + 1, scopeLevel + 1));
                                parms.Add(parm);
                                Parent._parameters.Add(parm);
                            }
                        }
                    }
                }

                Parent._globalFunctions.Add(new GlobalFunction(context.function_name.Text, returnTypeText, new LSLSourceCodeRange(context), parms));


                return base.VisitFunctionDeclaration(context);
            }


            public override bool VisitEventHandler(LSLParser.EventHandlerContext context)
            {
                codeAreaID++;

                if (context.parameters != null && context.parameters.children != null)
                {
                    var list = context.parameters.parameterList();
                    if (list != null)
                    {
                        foreach (var child in list.children)
                        {
                            var i = child as LSLParser.ParameterDefinitionContext;
                            if (i != null)
                            {
                                Parent._parameters.Add(new LocalParameter(i.parameter_name.Text, i.parameter_type.Text, new LSLSourceCodeRange(i), new ScopeAddress(codeAreaID, scopeID + 1, scopeLevel + 1)));
                            }
                        }
                    }
                }

                return base.VisitEventHandler(context);
            }
            

            public override bool VisitDefaultState(LSLParser.DefaultStateContext context)
            {
                this.Parent.DefaultState = new StateBlock("default", new LSLSourceCodeRange(context));

                codeAreaID++;
                scopeLevel++;
                scopeID++;
                inState = true;
                base.VisitDefaultState(context);
                inState = false;
                scopeLevel--;
                return true;
            }

            public override bool VisitDefinedState(LSLParser.DefinedStateContext context)
            {
                if (context.state_name == null) return true;


                this.Parent._stateBlocks.Add(new StateBlock(context.state_name.Text, 
                    new LSLSourceCodeRange(context)));

                codeAreaID++;
                scopeLevel++;
                scopeID++;
                inState = true;
                base.VisitDefinedState(context);
                inState = false;
                scopeLevel--;
                return true;
            }


            public override bool VisitCodeScope(LSLParser.CodeScopeContext context)
            {
                scopeLevel++;
                scopeID++;
                base.VisitCodeScope(context);
                scopeLevel--;
                return true;
            }
        };

        private static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public IEnumerable<LocalParameter> GetLocalParameters(ScopeAddress address, int offset)
        {
            return LocalParameters.Where(
                        x =>
                            x.ScopeAddress.CodeAreaID == address.CodeAreaID &&
                            x.ScopeAddress.ScopeID <= address.ScopeID &&
                            x.ScopeAddress.ScopeLevel == address.ScopeLevel &&
                            x.SourceCodeRange.StartIndex < offset);
        }

        public IEnumerable<LocalVariable> GetLocalVariables(ScopeAddress address, int offset)
        {
            return LocalVariables.Where(
                        x =>
                            x.ScopeAddress.CodeAreaID == address.CodeAreaID &&
                            x.ScopeAddress.ScopeID <= address.ScopeID &&
                            x.ScopeAddress.ScopeLevel == address.ScopeLevel &&
                            x.SourceCodeRange.StartIndex < offset);
        }


        private class CommentStringSkipper
        {
            bool inBlockComment = false;

            public bool InBlockComment
            {
                get { return inBlockComment; }
            }
            bool inLineComment = false;

            public bool InLineComment
            {
                get { return inLineComment; }
                set { inLineComment = value; }
            }
            bool inString = false;

            public bool InString
            {
                get { return inString; }
            }

            public bool InComment
            {
                get { return this.inLineComment || this.inBlockComment; }
            }
            public void Reset()
            {
                this.inBlockComment = false;
                this.inLineComment = false;
                this.inString = false;
            }

            public void FeedChar(string text, int i, int offset)
            {


                if (!inLineComment && !inString)
                {
                    if (text[i] == '/' && i < offset - 1 && text[i + 1] == '*')
                    {
                        inBlockComment = true;
                    }
                    else if (inBlockComment && text[i-2] == '*' && text[i-1] == '/')
                    {
                        inBlockComment = false;
                    }
                }
                if (!inBlockComment && !inString)
                {
                    if (text[i] == '/' && i < offset - 1 && text[i + 1] == '/')
                    {
                        inLineComment = true;
                    }
                    else if (inLineComment && text[i-1] == '\n')
                    {
                        inLineComment = false;
                    }
                }
                if (!inLineComment && !inBlockComment)
                {
                    if (!inString && text[i] == '"')
                    {
                        inString = true;
                    }
                    else if (inString && text[i-1] == '"')
                    {
                        int c = 0;
                        int s = i - 2;
                        int o = s;
                        for (o = s; text[o] == '\\'; o--, c++) ;

                        if (c == 0 || ((c % 2) == 0))
                        {
                            inString = false;
                        }
                    }
                }
            }
        }


        public static ScopeAddress FastParseToOffset(string text, int offset)
        {

            CommentStringSkipper skipper = new CommentStringSkipper();
            bool inState = false;
            int level = 0;
            int id = 0;
            int codeAreaID = 0;


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

                        if (text[defaultStopOffset]=='{')
                        {
                            i = defaultStopOffset + 1;
                        }
                        else
                        {
                            var dstateSkipper = new CommentStringSkipper();
                            
                            var o = defaultStopOffset;
                            var c = '0';
                            while (o < offset)
                            {
                                c = text[o];

                                

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
                            codeAreaID++;
                            
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
                            var stateSkipper = new CommentStringSkipper();
                            bool spacesBetween = false;
                            var o = stateStopOffset;
                            var c = '0';
                            while (o < offset)
                            {
                                c = text[o];


                                if (c == ';') { inState = false; break; }
                                if (c == '{') { break; }

                                stateSkipper.FeedChar(text, o, offset);

                                if (stateSkipper.InComment) { 
                                    spacesBetween = true;
                                }
                                else if (c == ' ')
                                {
                                    spacesBetween = true;
                                }

                                if (!(stateSkipper.InComment || stateSkipper.InString) && !char.IsWhiteSpace(c) && !spacesBetween)
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
                            codeAreaID++;
                        }
                    }
                    else if (text[i] == '{')
                    {
                        char t = text[i - 1];
                        level++;
                        if ((inState && level == 2) || (!inState && level == 1))
                        {
                            codeAreaID++;
                        }
                        id++;
                    }
                    else if (text[i] == '}')
                    {
                        char t = text[i - 1];
                        level--;
                        if (inState && level == 0)
                        {
                            inState = false;
                        }
                    }

                }
            }
            return new ScopeAddress(codeAreaID, id, level) { InState = inState, InString = skipper.InString, InComment = skipper.InComment };
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


            Visitor x = new Visitor(this);

            x.Visit(parser.compilationUnit());

        }
    }
}