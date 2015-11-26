using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LibLSLCC.CSharp
{

    public class CSharpInheritanceListValidationResult
    {
        /// <summary>
        /// Gets a value indicating whether the inheritance list signature parse was successful.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; internal set; }


        /// <summary>
        /// Will contain a syntax error description if <see cref="Success"/> is <c>false</c>
        /// </summary>
        /// <value>
        /// The error description.
        /// </value>
        public string ErrorDescription { get; internal set; }


        /// <summary>
        /// Gets the index at which a syntax error was detected when <see cref="Success"/> is <c>false</c>
        /// </summary>
        /// <value>
        /// The index of the syntax error.
        /// </value>
        public int ErrorIndex { get; internal set; }

        /// <summary>
        /// Gets the parsing results inherited types in the inheritance list.
        /// </summary>
        /// <value>
        /// The parsing results for the inherited types in the inheritance list.
        /// </value>
        public CSharpClassNameValidationResult[] InheritedTypes { get; internal set; }

        /// <summary>
        /// Gets a list of constrained parameter names that were given constraints with the 'where' keyword.
        /// </summary>
        /// <value>
        /// The constrained type parameter names.
        /// </value>
        public string[] ConstrainedTypeParameters { get; internal set; }

        /// <summary>
        /// Gets a multi-dimensional array of parameter constraint descriptions that directly 
        /// correspond to the names in <see cref="ConstrainedTypeParameters"/>.
        /// All type constraints applied to <see cref="ConstrainedTypeParameters"/>[i] can be found using <see cref="ParameterConstraints"/>[i].
        /// </summary>
        /// <value>
        /// The parameter constraints associated with the generic parameter names in <see cref="ConstrainedTypeParameters"/>.
        /// </value>
        public CSharpTypeConstraintValidationResult[][] ParameterConstraints { get; internal set; }

        /// <summary>
        /// Gets the full formated signature of the parsed inheritance list.
        /// </summary>
        /// <value>
        /// The full formated signature of the parsed inheritance list.
        /// </value>
        public string Fullsignature { get; internal set; }
    }


    /// <summary>
    /// Represents an inheritance list generic type constraint.
    /// </summary>
    public enum CSharpTypeConstraintType
    {
        /// <summary>
        /// Generic type is constrained with 'new()'
        /// </summary>
        New,

        /// <summary>
        /// Generic type is constrained with 'struct'
        /// </summary>
        Struct,

        /// <summary>
        /// Generic type is constrained with 'class'
        /// </summary>
        Class,

        /// <summary>
        /// Generic type is constrained with an interface or class type.
        /// </summary>
        Type,
    }

    /// <summary>
    /// Represents a parsed type constraint in an inheritance list signature.
    /// </summary>
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


        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// The given object must firstly be a <see cref="CSharpTypeConstraintValidationResult"/> in order to be considered equal at all.
        /// <see cref="ConstraintType"/> and <see cref="TypeSignature"/> must be equal in order for this to return true.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> (<see cref="CSharpTypeConstraintValidationResult"/>) to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> (<see cref="CSharpTypeConstraintValidationResult"/>) is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var o = obj as CSharpTypeConstraintValidationResult;
            if (o == null) return false;

            return o.ConstraintType == ConstraintType && CompareTypeSignatures(o.TypeSignature, TypeSignature);
        }

        /// <summary>
        /// Returns a hash code for this instance, based off <see cref="ConstraintType"/> and <see cref="TypeSignature"/> if it is not null.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
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

        /// <summary>
        /// Gets the string representation of the type constraint.
        /// </summary>
        /// <remarks>
        /// When the type constraint is an interface or class name, the full formated signature of the parsed interface/class name is returned.
        /// </remarks>
        /// <value>
        /// The type constraint string.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
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

        /// <summary>
        /// Returns <see cref="ConstraintString"/>.
        /// </summary>
        /// <returns>
        /// <see cref="ConstraintString"/>.
        /// </returns>
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
                return false;
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

            if (CSharpKeywords.IsTypeAliasKeyword(init.TypeSignature.BaseName))
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
                return false;
            }


            init = CSharpClassNameValidator.ValidateInitialization(str, true);
            if (!init.Success)
            {
                err = init.ErrorDescription;
                return false;
            }


            if (CSharpKeywords.IsTypeAliasKeyword(init.BaseName))
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

        /// <summary>
        /// Validates/parses the specified CSharp inheritance list given in <paramref name="input"/>.
        /// The parser supports 'where' type constraints on generic parameters.
        /// The signature given should be the source content immediately after a classes declared name, without the separating colon if there is one.
        /// </summary>
        /// <param name="input">The inheritance list signature to parse.</param>
        /// <returns></returns>
        public static CSharpInheritanceListValidationResult Validate(string input)
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

            for (int index = 0; index < input.Length; index++)
            {
                var c = input[index];

                bool end = index == input.Length - 1;
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
                            result.ErrorIndex = (index - (accum.Length - 1)) + (init == null ? 0 : init.ErrorIndex);
                            result.Success = false;
                            return result;
                        }

                        if (!inheritedTypes.Add(init))
                        {
                            result.ErrorDescription = string.Format("Type '{0}' cannot be inherited more than once.",
                                init.FullSignature);
                            result.ErrorIndex = (index - (accum.Length - 1)) + (init.ErrorIndex);
                            result.Success = false;
                            return result;
                        }

                        state = States.EndOfListWithoutWhereClauses;
                        continue;
                    }
                    if (state == States.AccumulatingTypeConstraint ||
                        state == States.AfterConstraintColon ||
                        (state == States.AccumulatingGenericPart &&
                         (stateBeforeGenericPart == States.AccumulatingTypeConstraint)))
                    {
                        var word = accum.Trim();

                        string err;
                        CSharpTypeConstraintValidationResult init;
                        if (!IsValidTypeConstraint(word, out err, out init))
                        {
                            result.ErrorDescription = err;
                            result.ErrorIndex = (index - (accum.Length - 1));
                            result.Success = false;
                            return result;
                        }


                        if (!typeConstraints.Last().Add(init))
                        {
                            result.ErrorDescription =
                                string.Format(
                                    "Type constraint '{0}' cannot be used more than once for generic parameter '{1}'.",
                                    init.ConstraintString, constrainedTypeParameters.Last());
                            result.ErrorIndex = (index - (accum.Length - 1));
                            result.Success = false;
                            return result;
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

                    if (state == States.AccumulatingGenericPart)
                    {
                        unmatchedGenericBraces++;
                        continue;
                    }

                    result.ErrorDescription = string.Format("Unexpected character '{0}'.", c);
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

                    result.ErrorDescription = string.Format("Unexpected character '{0}'.", c);
                    result.ErrorIndex = index;
                    result.Success = false;
                    return result;
                }


                if (c == ',')
                {
                    if ((state == States.AfterWhereKeyword && inheritedTypes.Count > 0) ||
                        state == States.AccumulatingConstraintParam)
                    {
                        result.ErrorDescription = string.Format("Unexpected character '{0}'.", c);
                        result.ErrorIndex = index;
                        result.Success = false;
                        return result;
                    }
                    if (state == States.AccumulatingTypeConstraint)
                    {
                        var word = accum.TrimEnd(',').Trim();

                        string err;
                        CSharpTypeConstraintValidationResult init;
                        if (!IsValidTypeConstraint(word, out err, out init))
                        {
                            result.ErrorDescription = err;
                            result.ErrorIndex = (index - (accum.Length - 1));
                            result.Success = false;
                            return result;
                        }

                        if (!typeConstraints.Last().Add(init))
                        {
                            result.ErrorDescription =
                                string.Format(
                                    "Type constraint '{0}' cannot be used more than once for generic parameter '{1}'.",
                                    init.ConstraintString, constrainedTypeParameters.Last());
                            result.ErrorIndex = (index - (accum.Length - 1));
                            result.Success = false;
                            return result;
                        }

                        accum = "";
                        continue;
                    }
                    if (state == States.AccumulatingInheritedType ||
                        state == States.AccumulatingFirstWord ||
                        (state == States.AfterWhereKeyword && inheritedTypes.Count == 0))
                    {
                        var type = accum.TrimEnd(',').Trim();
                        CSharpClassNameValidationResult init;
                        string err;
                        if (!IsValidInheritedType(type, out err, out init))
                        {
                            result.ErrorDescription = err;
                            result.ErrorIndex = (index - (accum.Length - 1));
                            result.Success = false;
                            return result;
                        }

                        if (!inheritedTypes.Add(init))
                        {
                            result.ErrorDescription = string.Format(
                                "Type '{0}' cannot be inherited more than once.", init.FullSignature);
                            result.ErrorIndex = (index - (accum.Length - 1));
                            result.Success = false;
                            return result;
                        }

                        accum = "";
                        state = States.AfterFirstInheritedType;
                        continue;
                    }
                }

                if (c == 'w')
                {
                    var ahead = index + 5;

                    if (ahead > input.Length) goto pastWhereCheck;

                    var lookAheadAsertion = input.Substring(index, 5) == "where";
                    var lookBehindsertion = index == 0 || char.IsWhiteSpace(input[index - 1]);

                    if (lookAheadAsertion && lookBehindsertion)
                    {
                        if (ahead < input.Length && !char.IsWhiteSpace(input[ahead]) && input[ahead] != ',')
                            goto pastWhereCheck;

                        if (state == States.WaitingForFirstWord)
                        {
                            accum = "";
                            index += 4;
                            state = States.AfterWhereKeyword;

                            //there is an ambiguous case here because you can inherit a class named where, before a where clause occurs

                            if (index + 1 == input.Length)
                            {
                                inheritedTypes.Add(CSharpClassNameValidator.ValidateInitialization("where", false));
                                state = States.EndOfListWithoutWhereClauses;
                                continue;
                            }
                            bool haveWhitespace = false;

                            for (int i = index + 1; i < input.Length; i++)
                            {
                                var cr = input[i];
                                if (char.IsWhiteSpace(cr))
                                {
                                    haveWhitespace = true;
                                    if (i == input.Length - 1)
                                    {
                                        inheritedTypes.Add(CSharpClassNameValidator.ValidateInitialization("where",
                                            false));
                                        state = States.EndOfListWithoutWhereClauses;
                                        break;
                                    }
                                    continue;
                                }

                                if (cr == 'w' && haveWhitespace)
                                {
                                    ahead = i + 5;
                                    if (ahead > input.Length) continue;
                                    lookAheadAsertion = input.Substring(i, 5) == "where";
                                    if (lookAheadAsertion)
                                    {
                                        if (ahead < input.Length && !char.IsWhiteSpace(input[ahead]) &&
                                            input[ahead] != ',') continue;

                                        inheritedTypes.Add(CSharpClassNameValidator.ValidateInitialization("where",
                                            false));
                                        index = i + 4;
                                        state = States.AfterWhereKeyword;
                                        break;
                                    }
                                }

                                if (cr == ',')
                                {
                                    inheritedTypes.Add(CSharpClassNameValidator.ValidateInitialization("where", false));
                                    index = i;
                                    state = States.AccumulatingInheritedType;
                                }
                                break;
                            }
                            continue;
                        }
                        if (state == States.AccumulatingTypeConstraint)
                        {
                            var word = accum.TrimEnd('w').Trim();

                            string err;
                            CSharpTypeConstraintValidationResult init;
                            if (!IsValidTypeConstraint(word, out err, out init))
                            {
                                result.ErrorDescription = err;
                                result.ErrorIndex = (index - (accum.Length - 1));
                                result.Success = false;
                                return result;
                            }

                            if (!typeConstraints.Last().Add(init))
                            {
                                result.ErrorDescription =
                                    string.Format(
                                        "Type constraint '{0}' cannot be used more than once for generic parameter '{1}'.",
                                        init.ConstraintString, constrainedTypeParameters.Last());
                                result.ErrorIndex = (index - (accum.Length - 1));
                                result.Success = false;
                                return result;
                            }

                            accum = "";
                            index += 4;
                            state = States.AfterWhereKeyword;
                            continue;
                        }
                        if (state == States.AccumulatingInheritedType || state == States.AccumulatingFirstWord)
                        {
                            var word = accum.TrimEnd('w').Trim();

                            CSharpClassNameValidationResult init;
                            string err;
                            if (!IsValidInheritedType(word, out err, out init))
                            {
                                result.ErrorDescription = err;
                                result.ErrorIndex = (index - (accum.Length - 1)) + (init == null ? 0 : init.ErrorIndex);
                                result.Success = false;
                                return result;
                            }

                            if (!inheritedTypes.Add(init))
                            {
                                result.ErrorDescription =
                                    string.Format("Type '{0}' cannot be inherited more than once.",
                                        init.FullSignature);
                                result.ErrorIndex = (index - (accum.Length - 1)) + (init.ErrorIndex);
                                result.Success = false;
                                return result;
                            }

                            state = States.AfterWhereKeyword;
                            accum = "";
                            index += 4;
                            continue;
                        }
                    }
                }

                pastWhereCheck:


                if (c == ':')
                {
                    if (state == States.AfterWhereKeyword)
                    {
                        result.ErrorDescription = string.Format("Unexpected character '{0}'.", c);
                        result.ErrorIndex = index;
                        result.Success = false;
                        return result;
                    }
                    if (state == States.AccumulatingConstraintParam)
                    {
                        var constrainedType = accum.TrimEnd(':').Trim();

                        if (!CSharpIDValidator.IsValidIdentifier(constrainedType))
                        {
                            result.ErrorDescription = string.Format("Invalid generic type constraint name '{0}'.",
                                constrainedType);
                            result.ErrorIndex = (index - (accum.Length - 1));
                            result.Success = false;
                            return result;
                        }

                        if (!constrainedTypeParameters.Add(constrainedType))
                        {
                            result.ErrorDescription =
                                string.Format(
                                    "Generic parameter '{0}' cannot have more than one type constraint list.",
                                    constrainedType);
                            result.ErrorIndex = (index - (accum.Length - 1));
                            result.Success = false;
                            return result;
                        }

                        typeConstraints.Add(new HashSet<CSharpTypeConstraintValidationResult>());

                        accum = "";
                        state = States.AfterConstraintColon;
                        continue;
                    }
                }

                if (!char.IsWhiteSpace(c))
                {
                    switch (state)
                    {
                        case States.AfterWhereKeyword:
                            accum = "" + c;
                            state = States.AccumulatingConstraintParam;
                            continue;
                        case States.WaitingForFirstWord:
                            accum = "" + c;
                            state = States.AccumulatingFirstWord;
                            continue;
                        case States.AfterFirstInheritedType:
                            accum = "" + c;
                            state = States.AccumulatingInheritedType;
                            continue;
                        case States.AfterConstraintColon:
                            accum = "" + c;
                            state = States.AccumulatingTypeConstraint;
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
                result.ErrorIndex = input.Length;
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