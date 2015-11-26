using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibLSLCC.CSharp
{
    public class CSharpFunctionCallValidationResult
    {
        public bool Success { get; internal set; }

        public string ErrorDescription { get; internal set; }

        public int ErrorIndex { get; internal set; }

        public CSharpClassNameValidationResult[] ExplicitGenericParameters { get; internal set; }

        public CSharpParameterSignature[] Parameters { get; internal set; }

        public string MethodName { get; internal set; }
    }


    public enum CSharpParameterModifier
    {
        None,
        Out,
        Ref,
        New
    }

    public class CSharpParameterSignature
    {
        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public CSharpParameterModifier Modifier { get; private set; }

        public string Name { get; private set; }

        internal CSharpParameterSignature(string name, CSharpParameterModifier modifier)
        {
            Modifier = modifier;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var o = obj as CSharpParameterSignature;
            if (o == null) return false;

            return string.Equals(o.Name, Name);
        }
    }


    public static class CSharpFunctionCallValidator
    {
        private enum States
        {
            WaitingForFirstCharacter,
            AccumulatingMethodNamePart,
            AccumulatingExplicitGenericMethodParameters,
            AfterExplicitGenericMethodParameters,
            AccumulatingGenericTypePart,
            WaitingForParameterName,
            AccumulatingParameter,
            AfterSignature,
            InParenthesizedExpression,
            InCurlyBraceExpression,
            InBracketExpression
        }


        private static bool IsValidPlainParameter(string paramText, int paramStartIndex, int paramEndIndex,
            out string err, out int errIndex)
        {
            errIndex = paramStartIndex;


            //check a string's syntax, or some other literal's syntax

            //Not using Roslyn.Compilers since its not a namespace included with mono by default yet.
            //Could use NRefactory, but.. it's an added assembly on windows and not on mono, it comes with mono by default and may be some other version. 
            //DLL hell?..

            //this is slow as hell for UI usage scenarios, but does what I want without adding large dependencies.

            string testp = string.Format("class P{{void F<T>(T a){{}}P(){{F({0});}}}}", paramText);
            const int pstartIndex = 31;

            using (var compiler = CodeDomProvider.CreateProvider("CSharp"))
            {
                var cParams = new CompilerParameters(new string[] {})
                {
                    GenerateInMemory = true,
                    GenerateExecutable = false
                };

                var r = compiler.CompileAssemblyFromSource(cParams, testp);

                //all errors except references to undefined variable/method/type references, and also type inference failures
                //CS0103: is undefined variable references
                //CS0246: is undefined type references
                //CS0411: is type cannot be inferred, this happens if the parameter has a syntactically valid lambda expression passed to it

                var errs = r.Errors.Cast<CompilerError>().ToList();
                var relevantErrors =
                    errs.Where(
                        x => x.ErrorNumber != "CS0103" && x.ErrorNumber != "CS0246" && x.ErrorNumber != "CS0411")
                        .ToList();

                if (relevantErrors.Count > 0)
                {
                    err = relevantErrors[0].ErrorText;
                    errIndex = (relevantErrors[0].Column - pstartIndex) + paramStartIndex;
                    return false;
                }
            }

            err = null;
            return true;
        }


        private static bool IsValidNewParameter(string paramText, int paramStartIndex, int paramEndIndex, out string err,
            out int errIndex)
        {
            errIndex = paramStartIndex;
            err = null;

            char startChar = paramText[0];

            if (CSharpIDValidator.IsValidIdentifier(paramText))
            {
                errIndex = paramStartIndex;
                err = "'new' keyword cannot be used with a variable reference.";
                return false;
            }

            if (char.IsDigit(startChar))
            {
                errIndex = paramStartIndex;
                err = "'new' keyword cannot be used on a numeric literal.";
                return false;
            }

            if (startChar == '"' || paramText.StartsWith("@\""))
            {
                errIndex = paramStartIndex;
                err = "'new' keyword cannot be used on a string literal.";
                return false;
            }

            if (startChar == '\'')
            {
                errIndex = paramStartIndex;
                err = "'new' keyword cannot be used on a character literal.";
                return false;
            }


            //validate a new type inferred array, anonymous object or class initialization

            string testp = string.Format("class P{{void F(object a){{}}P(){{F(new {0});}}}}", paramText);
            const int pstartIndex = 36;

            using (var compiler = CodeDomProvider.CreateProvider("CSharp"))
            {
                var cParams = new CompilerParameters(new string[] {})
                {
                    GenerateInMemory = true,
                    GenerateExecutable = false
                };

                var r = compiler.CompileAssemblyFromSource(cParams, testp);

                //all errors except references to undefined variable/method/type references
                //CS0103: is undefined variable references
                //CS0246: is undefined type references

                var errs = r.Errors.Cast<CompilerError>().ToList();
                var relevantErrors =
                    errs.Where(
                        x => x.ErrorNumber != "CS0103" && x.ErrorNumber != "CS0246").ToList();
                if (relevantErrors.Count > 0)
                {
                    err = relevantErrors[0].ErrorText;
                    errIndex = (relevantErrors[0].Column - pstartIndex) + paramStartIndex;
                    return false;
                }
            }

            return true;
        }


        public static CSharpFunctionCallValidationResult Validate(string signature)
        {
            var result = new CSharpFunctionCallValidationResult {Success = true};

            var explicitGenericParameters = new List<CSharpClassNameValidationResult>();
            var parameters = new List<CSharpParameterSignature>();

            var lastModifier = CSharpParameterModifier.None;

            var state = States.WaitingForFirstCharacter;

            var stateBeforeBrackets = new Stack<States>();
            var stateBeforeGenericTypePart = new Stack<States>();
            var stateBeforeParenthesis = new Stack<States>();
            var stateBeforeCurlyBraces = new Stack<States>();

            string accum = "";
            int unmatchedGenericBrackets = 0;
            int unmatchedParenthesis = 0;
            int unmatchedCurlyBraces = 0;
            int unmatchedBrackets = 0;
            string methodName = "";

            for (int index = 0; index < signature.Length; index++)
            {
                var c = signature[index];
                accum += c;


                if (c == 'o' || c == 'r' || c == 'n')
                {
                    int wordBoundry = index + 2;
                    int ahead = wordBoundry + 1;

                    if (wordBoundry >= signature.Length) goto afterRefOutChecks;
                    var lookAhead = signature.Substring(index, 3);
                    var lookAheadAssertion = lookAhead == "out" || lookAhead == "ref" || lookAhead == "new";
                    if (ahead < signature.Length && !char.IsWhiteSpace(signature[ahead])) goto afterRefOutChecks;

                    if (lookAheadAssertion && state == States.WaitingForParameterName &&
                        lastModifier == CSharpParameterModifier.None)
                    {
                        lastModifier = lookAhead == "out"
                            ? CSharpParameterModifier.Out
                            : (lookAhead == "ref" ? CSharpParameterModifier.Ref : CSharpParameterModifier.New);

                        accum = "";
                        index = wordBoundry;
                        state = States.WaitingForParameterName;
                        continue;
                    }
                }


                afterRefOutChecks:

                if (c == '[')
                {
                    if (state == States.AccumulatingParameter || state == States.WaitingForParameterName)
                    {
                        stateBeforeBrackets.Push(States.AccumulatingParameter);
                        state = States.InBracketExpression;
                        unmatchedBrackets++;
                        continue;
                    }


                    if (state == States.InBracketExpression || state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        unmatchedBrackets++;
                        continue;
                    }
                }

                if (c == ']')
                {
                    if (state == States.InBracketExpression || state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        unmatchedBrackets--;
                        if (unmatchedBrackets < 0)
                        {
                            result.Success = false;
                            result.ErrorDescription = "Unexpected closing bracket.";
                            result.ErrorIndex = index;
                            return result;
                        }
                        if (unmatchedBrackets == 0)
                        {
                            if (stateBeforeBrackets.Count > 0)
                            {
                                state = stateBeforeBrackets.Pop();
                            }
                            else if (state == States.InBracketExpression)
                            {
                                goto unexpectedBracket;
                            }
                        }
                        continue;

                        unexpectedBracket:

                        result.Success = false;
                        result.ErrorDescription = "Unexpected closing bracket.";
                        result.ErrorIndex = index;
                        return result;
                    }
                }


                if (c == '{')
                {
                    if (state == States.AccumulatingParameter || state == States.WaitingForParameterName)
                    {
                        stateBeforeCurlyBraces.Push(States.AccumulatingParameter);
                        state = States.InCurlyBraceExpression;
                        unmatchedCurlyBraces++;
                        continue;
                    }

                    if (state == States.InBracketExpression || state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        unmatchedCurlyBraces++;
                        continue;
                    }
                }

                if (c == '}')
                {
                    if (state == States.InBracketExpression || state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        unmatchedCurlyBraces--;
                        if (unmatchedCurlyBraces < 0)
                        {
                            result.Success = false;
                            result.ErrorDescription = "Unexpected closing brace.";
                            result.ErrorIndex = index;
                            return result;
                        }
                        if (unmatchedCurlyBraces == 0)
                        {
                            if (stateBeforeCurlyBraces.Count > 0)
                            {
                                state = stateBeforeCurlyBraces.Pop();
                            }
                            else if (state == States.InCurlyBraceExpression)
                            {
                                goto unexpectedCurlyBrace;
                            }
                        }

                        continue;
                    }

                    unexpectedCurlyBrace:

                    result.Success = false;
                    result.ErrorDescription = "Unexpected closing brace.";
                    result.ErrorIndex = index;
                    return result;
                }

                if (c == ')')
                {
                    if (state == States.WaitingForParameterName && parameters.Count == 0)
                    {
                        if (lastModifier == CSharpParameterModifier.None)
                        {
                            state = States.AfterSignature;
                            accum = "";
                        }
                        //otherwise the signature is incomplete
                        continue;
                    }

                    if (state == States.InBracketExpression || state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        unmatchedParenthesis--;
                        if (unmatchedParenthesis < 0)
                        {
                            result.Success = false;
                            result.ErrorDescription = "Unexpected closing parenthesis.";
                            result.ErrorIndex = index;
                            return result;
                        }
                        if (unmatchedParenthesis == 0)
                        {
                            if (stateBeforeParenthesis.Count > 0)
                            {
                                state = stateBeforeParenthesis.Pop();
                            }
                            else if (state == States.InParenthesizedExpression)
                            {
                                goto unexpectedParenth;
                            }
                        }
                        continue;
                    }
                    if (state == States.AccumulatingParameter)
                    {
                        var param = accum.Substring(0, accum.Length - 1).Trim();


                        if (!CSharpIDValidator.IsValidIdentifier(param) &&
                            (lastModifier == CSharpParameterModifier.Ref || lastModifier == CSharpParameterModifier.Out))
                        {
                            result.Success = false;
                            result.ErrorDescription =
                                "'ref' and 'out' can only be used with a direct variable reference.";
                            result.ErrorIndex = index - (accum.Length - 1);
                            return result;
                        }


                        string err;
                        int errIndex;

                        if (lastModifier == CSharpParameterModifier.New &&
                            !IsValidNewParameter(param, index - (accum.Length - 1), index, out err, out errIndex))
                        {
                            result.Success = false;
                            result.ErrorDescription = err;
                            result.ErrorIndex = errIndex;
                            return result;
                        }


                        if (lastModifier == CSharpParameterModifier.None &&
                            !IsValidPlainParameter(param, index - (accum.Length - 1), index, out err, out errIndex))
                        {
                            result.Success = false;
                            result.ErrorDescription = err;
                            result.ErrorIndex = errIndex;
                            return result;
                        }

                        parameters.Add(new CSharpParameterSignature(param, lastModifier));
                        accum = "";
                        lastModifier = CSharpParameterModifier.None;
                        state = States.AfterSignature;
                        continue;
                    }

                    unexpectedParenth:
                    result.Success = false;
                    result.ErrorDescription = "Unexpected closing parenthesis.";
                    result.ErrorIndex = index;
                    return result;
                }
                if (c == '(')
                {
                    if (state == States.AccumulatingParameter || state == States.WaitingForParameterName)
                    {
                        stateBeforeParenthesis.Push(States.AccumulatingParameter);
                        state = States.InParenthesizedExpression;
                        unmatchedParenthesis++;
                        continue;
                    }
                    if (state == States.AfterExplicitGenericMethodParameters)
                    {
                        accum = "";
                        state = States.WaitingForParameterName;
                        continue;
                    }
                    if (state == States.AccumulatingMethodNamePart)
                    {
                        methodName = accum.TrimEnd('(').Trim();
                        accum = "";
                        state = States.WaitingForParameterName;
                        continue;
                    }

                    if (state == States.InBracketExpression || state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        unmatchedParenthesis++;
                        continue;
                    }

                    result.Success = false;
                    result.ErrorDescription = "Unexpected opening parenthesis.";
                    result.ErrorIndex = index;
                    return result;
                }


                if (c == ',')
                {
                    if (state == States.InBracketExpression || state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        continue;
                    }

                    if (state == States.AccumulatingExplicitGenericMethodParameters)
                    {
                        var param = accum.TrimEnd(',').Trim();

                        var validate = CSharpClassNameValidator.ValidateInitialization(param, true);
                        if (!validate.Success)
                        {
                            result.Success = false;
                            result.ErrorDescription = validate.ErrorDescription;
                            result.ErrorIndex = (index - (accum.Length - 1)) + validate.ErrorIndex;
                            return result;
                        }

                        explicitGenericParameters.Add(validate);

                        accum = "";
                        continue;
                    }
                    if (state == States.AccumulatingParameter)
                    {
                        var param = accum.TrimEnd(',').Trim();

                        if (!CSharpIDValidator.IsValidIdentifier(param) &&
                            (lastModifier == CSharpParameterModifier.Ref || lastModifier == CSharpParameterModifier.Out))
                        {
                            result.Success = false;
                            result.ErrorDescription =
                                "'ref' and 'out' can only be used with a direct variable reference.";
                            result.ErrorIndex = index - (accum.Length - 1);
                            return result;
                        }

                        string err;
                        int errIndex;

                        if (lastModifier == CSharpParameterModifier.New &&
                            !IsValidNewParameter(param, index - (accum.Length - 1), index, out err, out errIndex))
                        {
                            result.Success = false;
                            result.ErrorDescription = err;
                            result.ErrorIndex = errIndex;
                            return result;
                        }


                        if (lastModifier == CSharpParameterModifier.None &&
                            !IsValidPlainParameter(param, index - (accum.Length - 1), index, out err, out errIndex))
                        {
                            result.Success = false;
                            result.ErrorDescription = err;
                            result.ErrorIndex = errIndex;
                            return result;
                        }

                        state = States.WaitingForParameterName;
                        parameters.Add(new CSharpParameterSignature(param, lastModifier));
                        lastModifier = CSharpParameterModifier.None;
                        accum = "";
                        continue;
                    }

                    result.Success = false;
                    result.ErrorDescription = "Unexpected ',' character.";
                    result.ErrorIndex = index;
                    return result;
                }
                if (c == '<')
                {
                    if (state == States.AccumulatingParameter ||
                        state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression ||
                        state == States.InBracketExpression)
                    {
                        continue;
                    }
                    if (state == States.AccumulatingMethodNamePart)
                    {
                        methodName = accum.TrimEnd('<').Trim();
                        accum = "";
                        state = States.AccumulatingExplicitGenericMethodParameters;
                        continue;
                    }
                    if (state == States.AccumulatingExplicitGenericMethodParameters)
                    {
                        unmatchedGenericBrackets++;
                        stateBeforeGenericTypePart.Push(state);
                        state = States.AccumulatingGenericTypePart;
                        continue;
                    }
                    if (state == States.AccumulatingGenericTypePart)
                    {
                        unmatchedGenericBrackets++;
                        continue;
                    }

                    result.Success = false;
                    result.ErrorDescription = "Unexpected '<' character.";
                    result.ErrorIndex = index;
                    return result;
                }
                if (c == '>')
                {
                    if (state == States.AccumulatingParameter ||
                        state == States.InCurlyBraceExpression ||
                        state == States.InParenthesizedExpression ||
                        state == States.InBracketExpression)
                    {
                        continue;
                    }
                    if (state == States.AccumulatingGenericTypePart)
                    {
                        unmatchedGenericBrackets--;
                        if (unmatchedGenericBrackets == 0)
                        {
                            state = stateBeforeGenericTypePart.Pop();
                        }
                        continue;
                    }
                    if (state == States.AccumulatingExplicitGenericMethodParameters)
                    {
                        var param = accum.Substring(0, accum.Length > 0 ? accum.Length - 1 : 0).Trim();

                        var validate = CSharpClassNameValidator.ValidateInitialization(param, true);
                        if (!validate.Success)
                        {
                            result.Success = false;
                            result.ErrorDescription = validate.ErrorDescription;
                            result.ErrorIndex = (index - (accum.Length - 1)) + validate.ErrorIndex;
                            return result;
                        }

                        explicitGenericParameters.Add(validate);
                        accum = "";
                        state = States.AfterExplicitGenericMethodParameters;
                        continue;
                    }

                    result.Success = false;
                    result.ErrorDescription = "Unexpected '>' character.";
                    result.ErrorIndex = index;
                    return result;
                }

                if (!char.IsWhiteSpace(c))
                {
                    if (state == States.AccumulatingParameter ||
                        state == States.AccumulatingExplicitGenericMethodParameters ||
                        state == States.AccumulatingMethodNamePart ||
                        state == States.AccumulatingGenericTypePart ||
                        state == States.InCurlyBraceExpression ||
                        state == States.InBracketExpression ||
                        state == States.InParenthesizedExpression)
                    {
                        continue;
                    }
                    if (state == States.WaitingForParameterName)
                    {
                        accum = "" + c;
                        state = States.AccumulatingParameter;
                        continue;
                    }
                    if (state == States.WaitingForFirstCharacter)
                    {
                        accum = "" + c;
                        state = States.AccumulatingMethodNamePart;
                        continue;
                    }

                    result.Success = false;
                    result.ErrorDescription = string.Format("Unexpected '{0}' character.", c);
                    result.ErrorIndex = index;
                    return result;
                }
            }


            if (state != States.AfterSignature)
            {
                result.Success = false;
                result.ErrorDescription = "Call signature incomplete.";
                result.ErrorIndex = signature.Length - 1;
                return result;
            }


            result.MethodName = methodName;
            result.Parameters = parameters.ToArray();
            result.ExplicitGenericParameters = explicitGenericParameters.ToArray();
            return result;
        }
    }
}