using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace LibLSLCC.CSharp
{
    public class CSharpInheritanceListValidationResult
    {
        public bool Success { get; internal set; }

        public string ErrorDescription { get; internal set; }

        public int ErrorIndex { get; internal set; }

        public CSharpClassNameValidationResult[] InheritedTypes { get; internal set; }
        public string[] ConstrainedTypeParameters { get; internal set; }
        public CSharpTypeConstraintValidationResult[][] ParameterConstraints { get; internal set; }

        public string Fullsignature { get; internal set; }
    }

    public enum CSharpTypeConstraintType
    {
        New,
        Struct,
        Class,
        Type,
    }


    public class CSharpTypeConstraintValidationResult
    {
        public CSharpTypeConstraintType ConstraintType { get; internal set; }
        public CSharpClassNameValidationResult TypeSignature { get; internal set; }


        private bool CompareTypeSignatures(CSharpClassNameValidationResult x, CSharpClassNameValidationResult y)
        {
            if (x == null) return y == null;
            if (y == null) return false;
            return string.Equals(x.FullSignature, y.FullSignature);
        }

        public override bool Equals(object obj)
        {
            var o = obj as CSharpTypeConstraintValidationResult;
            if (o == null) return false;

            return o.ConstraintType == ConstraintType && CompareTypeSignatures(o.TypeSignature, TypeSignature);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) ConstraintType*397) ^
                       (TypeSignature != null && TypeSignature.FullSignature != null
                           ? TypeSignature.FullSignature.GetHashCode()
                           : 0);
            }
        }

        public string ConstraintString
        {
            get
            {
                switch (ConstraintType)
                {
                    case CSharpTypeConstraintType.New:
                        return "new()";
                    case CSharpTypeConstraintType.Struct:
                        return "struct";
                    case CSharpTypeConstraintType.Class:
                        return "class";
                    case CSharpTypeConstraintType.Type:
                        return TypeSignature.FullSignature ?? "";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        public override string ToString()
        {
            return ConstraintString;
        }
    }


    public static class CSharpInheritanceListValidator
    {
        //type -> regular|generic
        //constraint -> class|struct|new()|type
        //(type (,type)*)? (where ID : constraint (,constraint)*)*

        private enum States
        {
            WaitingForFirstWord,
            AccumulatingFirstWord,
            AfterWhereKeyword,
            AfterFirstInheritedType,
            AccumulatingGenericPart,
            AccumulatingInheritedType,
            AccumulatingConstraintParam,
            AfterConstraintColon,
            AccumulatingTypeConstraint,
            EndOfListWithWhereClauses,
            EndOfListWithoutWhereClauses,
        }


        private static bool IsValidTypeConstraint(string str, out string err,
            out CSharpTypeConstraintValidationResult init)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                init = null;
                err = "Missing type constraint specifier.";
                return true;
            }

            init = new CSharpTypeConstraintValidationResult();

            if (str == "class")
            {
                init.ConstraintType = CSharpTypeConstraintType.Class;
                err = null;
                return true;
            }
            if (str == "struct")
            {
                init.ConstraintType = CSharpTypeConstraintType.Struct;
                err = null;
                return true;
            }
            if (Regex.IsMatch(str, @"^new\(\s*\)$"))
            {
                init.ConstraintType = CSharpTypeConstraintType.New;
                err = null;
                return true;
            }


            init.TypeSignature = CSharpClassNameValidator.ValidateInitialization(str, true);
            init.ConstraintType = CSharpTypeConstraintType.Type;

            if (!init.TypeSignature.Success)
            {
                err = init.TypeSignature.ErrorDescription;
                init = null;
                return false;
            }

            if (CSharpClassNameValidator.IsTypeAliasKeyword(init.TypeSignature.BaseName))
            {
                err = string.Format("Cannot use the built in type alias '{0}' as a type constraint.",
                    init.TypeSignature.BaseName);
                init = null;
                return false;
            }

            err = null;
            return true;
        }


        private static bool IsValidInheritedType(string str, out string err, out CSharpClassNameValidationResult init)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                init = null;
                err = "Missing inherited class specifier.";
                return true;
            }


            init = CSharpClassNameValidator.ValidateInitialization(str, true);
            if (!init.Success)
            {
                err = init.ErrorDescription;
                init = null;
                return false;
            }


            if (CSharpClassNameValidator.IsTypeAliasKeyword(init.BaseName))
            {
                err = string.Format("Cannot inherit from the built in type alias '{0}'.", init.BaseName);
                init = null;
                return false;
            }


            err = null;
            return true;
        }


        private class EquateValidatedTypes : IEqualityComparer<CSharpClassNameValidationResult>
        {
            public bool Equals(CSharpClassNameValidationResult x, CSharpClassNameValidationResult y)
            {
                return string.Equals(x.FullSignature, y.FullSignature);
            }

            public int GetHashCode(CSharpClassNameValidationResult obj)
            {
                return obj.FullSignature.GetHashCode();
            }
        }


        public static CSharpInheritanceListValidationResult Validate(string validate)
        {
            var result = new CSharpInheritanceListValidationResult();

            var compareValidatedType = new EquateValidatedTypes();

            var inheritedTypes = new HashSet<CSharpClassNameValidationResult>(compareValidatedType);
            var constrainedTypeParameters = new HashSet<string>();
            var typeConstraints =
                new List<HashSet<CSharpTypeConstraintValidationResult>>();

            result.Success = true;


            States state = States.WaitingForFirstWord;

            States stateBeforeGenericPart = 0;

            string accum = "";
            int unmatchedGenericBraces = 0;

            for (int index = 0; index < validate.Length; index++)
            {
                var c = validate[index];

                bool end = index == validate.Length - 1;
                accum += c;

                if (end)
                {
                    if (state == States.AccumulatingInheritedType ||
                        state == States.AfterFirstInheritedType ||
                        state == States.AccumulatingFirstWord ||
                        state == States.WaitingForFirstWord ||
                        (state == States.AccumulatingGenericPart &&
                         (stateBeforeGenericPart == States.AccumulatingInheritedType ||
                          stateBeforeGenericPart == States.AccumulatingFirstWord)))
                    {
                        var word = accum.Trim();

                        CSharpClassNameValidationResult init;
                        string err;
                        if (!IsValidInheritedType(word, out err, out init))
                        {
                            result.ErrorDescription = err;
                            result.ErrorIndex = index;
                            result.Success = false;
                            return result;
                        }

                        if (!inheritedTypes.Add(init))
                        {
                            result.ErrorDescription = string.Format("Type '{0}' cannot be inherited more than once.",
                                init.FullSignature);
                            result.ErrorIndex = index;
                            result.Success = false;
                            return result;
                        }

                        state = States.EndOfListWithoutWhereClauses;
                        continue;
                    }
                    if (state == States.AccumulatingTypeConstraint || state == States.AfterConstraintColon ||
                        (state == States.AccumulatingGenericPart &&
                         (stateBeforeGenericPart == States.AccumulatingTypeConstraint)))
                    {
                        var word = accum.Trim();

                        string err;
                        CSharpTypeConstraintValidationResult init;
                        if (!IsValidTypeConstraint(word, out err, out init))
                        {
                            result.ErrorDescription = err;
                            result.ErrorIndex = index;
                            result.Success = false;
                            return result;
                        }

                        if (typeConstraints.Count == 0)
                        {
                            typeConstraints.Add(new HashSet<CSharpTypeConstraintValidationResult> {init});
                        }
                        else
                        {
                            if (!typeConstraints.Last().Add(init))
                            {
                                result.ErrorDescription =
                                    string.Format(
                                        "Type constraint '{0}' cannot be used more than once for generic parameter '{1}'.",
                                        init.ConstraintString, constrainedTypeParameters.Last());
                                result.ErrorIndex = index;
                                result.Success = false;
                                return result;
                            }
                        }

                        state = States.EndOfListWithWhereClauses;

                        continue;
                    }
                }

                if (c == '<')
                {
                    if (state == States.AccumulatingFirstWord || state == States.AccumulatingTypeConstraint ||
                        state == States.AccumulatingInheritedType)
                    {
                        stateBeforeGenericPart = state;
                        state = States.AccumulatingGenericPart;
                        unmatchedGenericBraces++;
                        continue;
                    }

                    result.ErrorDescription = string.Format("Unexpected character {0}.", c);
                    result.ErrorIndex = index;
                    result.Success = false;
                    return result;
                }

                if (c == '>')
                {
                    if (state == States.AccumulatingGenericPart)
                    {
                        unmatchedGenericBraces--;

                        if (unmatchedGenericBraces == 0)
                        {
                            state = stateBeforeGenericPart;
                        }
                        continue;
                    }

                    result.ErrorDescription = string.Format("Unexpected character {0}.", c);
                    result.ErrorIndex = index;
                    result.Success = false;
                    return result;
                }


                if (c == ',')
                {
                    if (state == States.AfterWhereKeyword)
                    {
                        result.ErrorDescription = string.Format("Unexpected character {0}.", c);
                        result.ErrorIndex = index;
                        result.Success = false;
                        return result;
                    }
                }

                if (!char.IsWhiteSpace(c))
                {
                    if (state == States.AfterWhereKeyword)
                    {
                        accum = "" + c;
                        state = States.AccumulatingConstraintParam;
                        continue;
                    }
                    if (state == States.WaitingForFirstWord)
                    {
                        accum = "" + c;
                        state = States.AccumulatingFirstWord;

                        if (c == 'w')
                        {
                            if (index + 4 > validate.Length) continue;
                            var lookAheadAsertion = validate.Substring(index, 5);
                            if (lookAheadAsertion == "where")
                            {
                                accum = "";
                                index += 4;
                                state = States.AfterWhereKeyword;
                            }
                        }

                        continue;
                    }
                    if (state == States.AccumulatingFirstWord)
                    {
                        if (c == ',')
                        {
                            var type = accum.TrimEnd(',').Trim();
                            CSharpClassNameValidationResult init;
                            string err;
                            if (!IsValidInheritedType(type, out err, out init))
                            {
                                result.ErrorDescription = err;
                                result.ErrorIndex = index;
                                result.Success = false;
                                return result;
                            }

                            if (!inheritedTypes.Add(init))
                            {
                                result.ErrorDescription = string.Format(
                                    "Type '{0}' cannot be inherited more than once.", init.FullSignature);
                                result.ErrorIndex = index;
                                result.Success = false;
                                return result;
                            }

                            accum = "";
                            state = States.AfterFirstInheritedType;
                        }
                        if (c == 'w')
                        {
                            var lookAheadAsertion = validate.Substring(index, 5);
                            if (lookAheadAsertion == "where")
                            {
                                var word = accum.TrimEnd('w').Trim();

                                CSharpClassNameValidationResult init;
                                string err;
                                if (!IsValidInheritedType(word, out err, out init))
                                {
                                    result.ErrorDescription = err;
                                    result.ErrorIndex = index;
                                    result.Success = false;
                                    return result;
                                }

                                if (!inheritedTypes.Add(init))
                                {
                                    result.ErrorDescription =
                                        string.Format("Type '{0}' cannot be inherited more than once.",
                                            init.FullSignature);
                                    result.ErrorIndex = index;
                                    result.Success = false;
                                    return result;
                                }

                                state = States.AfterWhereKeyword;
                                accum = "";
                                index += 4;
                            }
                        }
                        continue;
                    }
                    if (state == States.AfterFirstInheritedType)
                    {
                        accum = "" + c;
                        state = States.AccumulatingInheritedType;
                        continue;
                    }
                    if (state == States.AccumulatingInheritedType)
                    {
                        if (c == ',')
                        {
                            var word = accum.TrimEnd(',').Trim();

                            CSharpClassNameValidationResult init;
                            string err;
                            if (!IsValidInheritedType(word, out err, out init))
                            {
                                result.ErrorDescription = err;
                                result.ErrorIndex = index;
                                result.Success = false;
                                return result;
                            }

                            if (!inheritedTypes.Add(init))
                            {
                                result.ErrorDescription = string.Format(
                                    "Type '{0}' cannot be inherited more than once.", init.FullSignature);
                                result.ErrorIndex = index;
                                result.Success = false;
                                return result;
                            }
                        }
                        if (c == 'w')
                        {
                            if (index + 4 > validate.Length) continue;
                            var lookAheadAsertion = validate.Substring(index, 5);
                            if (lookAheadAsertion == "where")
                            {
                                var word = accum.TrimEnd('w').Trim();

                                CSharpClassNameValidationResult init;
                                string err;
                                if (!IsValidInheritedType(word, out err, out init))
                                {
                                    result.ErrorDescription = err;
                                    result.ErrorIndex = index;
                                    result.Success = false;
                                    return result;
                                }

                                if (!inheritedTypes.Add(init))
                                {
                                    result.ErrorDescription =
                                        string.Format("Type '{0}' cannot be inherited more than once.",
                                            init.FullSignature);
                                    result.ErrorIndex = index;
                                    result.Success = false;
                                    return result;
                                }

                                state = States.AfterWhereKeyword;
                                accum = "";
                                index += 4;
                            }
                        }
                        continue;
                    }
                    if (state == States.AccumulatingConstraintParam)
                    {
                        if (c == ':')
                        {
                            var constrainedType = accum.TrimEnd(':').Trim();

                            if (!CSharpCompilerSingleton.Compiler.IsValidIdentifier(constrainedType))
                            {
                                result.ErrorDescription = string.Format("Invalid generic type constraint name {0}.",
                                    constrainedType);
                                result.ErrorIndex = index;
                                result.Success = false;
                                return result;
                            }

                            if (!constrainedTypeParameters.Add(constrainedType))
                            {
                                result.ErrorDescription =
                                    string.Format(
                                        "Generic parameter '{0}' cannot have more than one type constraint list.",
                                        constrainedType);
                                result.ErrorIndex = index;
                                result.Success = false;
                                return result;
                            }

                            accum = "";
                            state = States.AfterConstraintColon;
                            continue;
                        }
                    }
                    if (state == States.AfterConstraintColon)
                    {
                        accum = "" + c;
                        state = States.AccumulatingTypeConstraint;
                        continue;
                    }
                    if (state == States.AccumulatingTypeConstraint)
                    {
                        if (c == ',')
                        {
                            var word = accum.TrimEnd(',').Trim();


                            string err;
                            CSharpTypeConstraintValidationResult init;
                            if (!IsValidTypeConstraint(word, out err, out init))
                            {
                                result.ErrorDescription = err;
                                result.ErrorIndex = index;
                                result.Success = false;
                                return result;
                            }

                            if (typeConstraints.Count == 0)
                            {
                                typeConstraints.Add(new HashSet<CSharpTypeConstraintValidationResult>() {init});
                            }
                            else
                            {
                                if (!typeConstraints.Last().Add(init))
                                {
                                    result.ErrorDescription =
                                        string.Format(
                                            "Type constraint '{0}' cannot be used more than once for generic parameter '{1}'.",
                                            init.ConstraintString, constrainedTypeParameters.Last());
                                    result.ErrorIndex = index;
                                    result.Success = false;
                                    return result;
                                }
                            }
                            accum = "";
                        }
                        if (c == 'w')
                        {
                            if (index + 4 > validate.Length) continue;
                            var lookAheadAsertion = validate.Substring(index, 5);
                            if (lookAheadAsertion == "where")
                            {
                                state = States.AfterWhereKeyword;
                                index += 4;
                                accum = "";
                            }
                        }
                        continue;
                    }
                }

                if (!char.IsWhiteSpace(c)) continue;

                switch (state)
                {
                    case States.AfterConstraintColon:
                        accum = "";
                        continue;
                    case States.WaitingForFirstWord:
                        accum = "";
                        continue;
                    case States.AfterFirstInheritedType:
                        accum = "";
                        continue;
                    case States.AfterWhereKeyword:
                        accum = "";
                        break;
                }
            }

            if (state != States.EndOfListWithoutWhereClauses &&
                state != States.EndOfListWithWhereClauses &&
                state != States.WaitingForFirstWord)
            {
                result.Success = false;
                result.ErrorDescription = "Class inheritance list is incomplete.";
                result.ErrorIndex = validate.Length - 1;
                return result;
            }

            result.ConstrainedTypeParameters = constrainedTypeParameters.ToArray();
            result.InheritedTypes = inheritedTypes.ToArray();
            result.ParameterConstraints = typeConstraints.Select(x => x.ToArray()).ToArray();

            result.Fullsignature = "";
            if (result.InheritedTypes.Length > 0)
            {
                result.Fullsignature += string.Join(", ", result.InheritedTypes.Select(x => x.FullSignature));
                if (result.ConstrainedTypeParameters.Length > 0)
                {
                    result.Fullsignature += " ";
                }
            }


            result.Fullsignature += string.Join(" ", result.ConstrainedTypeParameters.Select(
                (x, i) =>
                    "where " + result.ConstrainedTypeParameters[i] + " : " +
                    string.Join(", ", result.ParameterConstraints[i].Select(c => c.ConstraintString))));


            return result;
        }
    }
}