#region Imports

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using LibLSLCC.LibraryData;
using LibLSLCC.Utility;
using LibLSLCC.Utility.ListParser;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Represents the basic signature of an LSL constant.
    /// </summary>
    public class LSLConstantSignature : ILSLConstantSignature
    {
        private static string _floatRegexString = "([-+]?[0-9]*(?:\\.[0-9]*))";

        private static readonly Regex VectorValidationRegex =
            new Regex("^" + _floatRegexString + "\\s*,\\s*" + _floatRegexString + "\\s*,\\s*" + _floatRegexString + "$");

        private static readonly Regex RotationValidationRegex =
            new Regex("^" + _floatRegexString + "\\s*,\\s*" + _floatRegexString + "\\s*,\\s*" + _floatRegexString +
                      "\\s*,\\s*" + _floatRegexString + "$");

        private string _name;
        private LSLType _type;
        private string _valueString;


        /// <summary>
        ///     construct a completely empty signature, without a name, type or valuestring.
        /// </summary>
        protected LSLConstantSignature()
        {
        }


        /// <summary>
        ///     Construct the <see cref="ILSLConstantSignature" /> by cloning another one.
        /// </summary>
        /// <param name="other">The other <see cref="ILSLConstantSignature" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        public LSLConstantSignature(ILSLConstantSignature other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            _type = other.Type;
            _valueString = other.ValueString;
        }


        /// <summary>
        ///     Construct the LSLLibraryConstantSignature from a given <see cref="LSLType" /> and constant name.
        ///     <see cref="ValueString" /> is given the default
        ///     value for the given <see cref="LSLType" /> passed in <paramref name="type" />
        /// </summary>
        /// <param name="type">The constant type.</param>
        /// <param name="name">The constant name.</param>
        /// <exception cref="LSLInvalidSymbolNameException">If <paramref name="name" /> is an invalid LSL ID token.</exception>
        /// <exception cref="LSLInvalidConstantTypeException">
        ///     if <paramref name="type" /> is
        ///     <see cref="LSLType.Void" />.
        /// </exception>
        public LSLConstantSignature(LSLType type, string name)
        {
            Name = name;
            Type = type;

            //use _valueString to bypass validation, since its faster
            //and we know what is required by the class.
            switch (type)
            {
                case LSLType.Key:
                    _valueString = "00000000-0000-0000-0000-000000000000";
                    break;
                case LSLType.Integer:
                    _valueString = "0";
                    break;
                case LSLType.String:
                    _valueString = "";
                    break;
                case LSLType.Float:
                    _valueString = "0.0";
                    break;
                case LSLType.List:
                    _valueString = ""; //empty list
                    break;
                case LSLType.Vector:
                    _valueString = "0,0,0";
                    break;
                case LSLType.Rotation:
                    _valueString = "0,0,0,0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }


        /// <summary>
        ///     Construct the LSLLibraryConstantSignature from a given <see cref="LSLType" />, constant name, and Value string.
        /// </summary>
        /// <param name="type">The constant type.</param>
        /// <param name="name">The constant name.</param>
        /// <param name="valueString">
        ///     The string value that represents the constant.  Must be appropriate for
        ///     <paramref name="type" />.
        /// </param>
        /// <exception cref="LSLInvalidSymbolNameException">If <paramref name="name" /> is an invalid LSL ID token.</exception>
        /// <exception cref="LSLInvalidConstantTypeException">
        ///     if <paramref name="type" /> is
        ///     <see cref="LSLType" />.
        /// </exception>
        /// <exception cref="LSLInvalidConstantValueStringException">
        ///     If <paramref name="valueString" /> is an invalid value for a float and <paramref name="type" /> is set to
        ///     <see cref="LSLType" />
        ///     or
        ///     If <paramref name="valueString" /> is an invalid value for an integer and <paramref name="type" /> is set to
        ///     <see cref="LSLType" />
        ///     or
        ///     If <paramref name="valueString" /> is an invalid value for a vector and <paramref name="type" /> is set to
        ///     <see cref="LSLType" />
        ///     or
        ///     If <paramref name="valueString" /> is an invalid value for a rotation and <paramref name="type" /> is set to
        ///     <see cref="LSLType" />
        /// </exception>
        public LSLConstantSignature(LSLType type, string name, string valueString)
        {
            Name = name;
            Type = type;
            ValueString = valueString;
        }


        /// <summary>
        ///     Returns a formated signature string for the constant, in the form:  NAME = ValueStringAsCodeLiteral
        ///     Without a trailing semi-colon character.
        /// </summary>
        public string SignatureString
        {
            get { return Type.ToLSLTypeName() + " " + Name + " = " + ValueStringAsCodeLiteral; }
        }

        /// <summary>
        ///     The <see cref="LSLType" /> that the library constant is defined with.
        /// </summary>
        /// <exception cref="LSLInvalidConstantTypeException" accessor="set">
        ///     if <paramref name="value" /> is
        ///     <see cref="LSLType" />.
        /// </exception>
        public LSLType Type
        {
            get { return _type; }
            protected set
            {
                if (value == LSLType.Void)
                {
                    throw new LSLInvalidConstantTypeException(
                        "Library Constant's Type may not be set to Void.");
                }
                _type = value;
            }
        }

        /// <summary>
        ///     The name of the library constant, must abide by LSL symbol naming rules or an exception will be thrown.
        /// </summary>
        /// <exception cref="LSLInvalidSymbolNameException" accessor="set">If <paramref name="value" /> is an invalid LSL ID token.</exception>
        public string Name
        {
            get { return _name; }
            protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new LSLInvalidSymbolNameException(
                        GetType().Name + ": Constant name was null or whitespace.");
                }

                if (!LSLTokenTools.IDRegexAnchored.IsMatch(value))
                {
                    throw new LSLInvalidSymbolNameException(
                        string.Format(
                            GetType().Name + ": Constant name '{0}' contained invalid characters or formatting.",
                            value));
                }

                _name = value;
            }
        }

        /// <summary>
        ///     The value string of the library constant, you must set <see cref="Type" /> first to a value that is not
        ///     <see cref="LSLType.Void" /> or an exception will be thrown.
        /// </summary>
        /// <value>
        ///     The value string.
        /// </value>
        /// <exception cref="LSLInvalidConstantValueStringException" accessor="set">
        ///     If the Value is an invalid value for a float and <see cref="Type" /> is set to <see cref="LSLType.Float" />
        ///     or
        ///     If the Value is an invalid value for an integer and <see cref="Type" /> is set to <see cref="LSLType.Integer" />
        ///     or
        ///     If the Value is an invalid value for a vector and <see cref="Type" /> is set to <see cref="LSLType.Vector" />
        ///     or
        ///     If the Value is an invalid value for a rotation and <see cref="Type" /> is set to <see cref="LSLType.Rotation" />
        /// </exception>
        /// <exception cref="Type" accessor="set">
        ///     If you try to set this value and
        ///     <see cref="LSLType.Void" /> is equal to <see cref="LSLType" />.
        /// </exception>
        /// <remarks>
        ///     Only integral or hexadecimal values are allowed when <see cref="LSLType.Integer" /> is set to
        ///     <see cref="LSLType" />
        ///     Only floating point or hexadecimal values are allowed when <see cref="LSLType.Float" /> is set to
        ///     <see cref="LSLType" />
        ///     The enclosing less than and greater than symbols will be removed when <see cref="LSLType.Vector" /> is set to
        ///     <see cref="LSLType" /> or <see cref="LSLType" />.
        /// </remarks>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value" /> is <c>null</c>.</exception>
        public string ValueString
        {
            get { return _valueString; }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", GetType().Name + ".ValueString cannot be set to null.");
                }

                switch (Type)
                {
                    case LSLType.Float:
                        SetFloatValueString(value);
                        return;
                    case LSLType.Integer:
                        SetIntegerValueString(value);
                        return;
                    case LSLType.List:
                        SetListValueString(value);
                        return;
                    case LSLType.Vector:
                        SetVectorValueString(value);
                        return;
                    case LSLType.Rotation:
                        SetRotationValueString(value);
                        return;
                    case LSLType.Void:
                        throw new LSLInvalidConstantTypeException(
                            "Could not set ValueString because the 'Type' Properties value is set to Void.");
                }

                _valueString = value;
            }
        }

        /// <summary>
        ///     Returns a string which represents what this Constant would look like
        ///     if it were expanded into an LSL code literal.  This takes the Type and contents
        ///     of ValueString into account.
        /// </summary>
        public string ValueStringAsCodeLiteral
        {
            get
            {
                if (Type == LSLType.Key || Type == LSLType.String)
                {
                    return "\"" + LSLFormatTools.ShowControlCodeEscapes(ValueString) + "\"";
                }
                if (Type == LSLType.Vector || Type == LSLType.Rotation)
                {
                    return "<" + ValueString + ">";
                }
                if (Type == LSLType.List)
                {
                    return "[" + ValueString + "]";
                }

                return ValueString;
            }
        }


        /// <summary>
        ///     Delegates to the SignatureString Property.
        /// </summary>
        /// <returns>
        ///     The SignatureString Property.
        /// </returns>
        public override string ToString()
        {
            return SignatureString;
        }


        /// <summary>
        ///     Returns the hash code of the <see cref="LSLLibraryConstantSignature" /> object.
        ///     <para />
        ///     The <see cref="Type" /> and <see cref="Name" /> properties are used to generate the hash code.
        /// </summary>
        /// <returns>The generated hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;


                hash = hash*31 + Type.GetHashCode();
                hash = hash*31 + Name.GetHashCode();


                return hash;
            }
        }


        /// <summary>
        ///     Determines whether the Type and Name properties of another <see cref="ILSLConstantSignature" /> equal the Type and
        ///     Name
        ///     properties of this object.
        ///     <para />
        ///     If the passed object is not an <see cref="ILSLConstantSignature" /> object then the result will always be false.
        /// </summary>
        /// <param name="obj">The object to compare this object with.</param>
        /// <returns>
        ///     True if the object is an <see cref="ILSLConstantSignature" /> object and the Name and Type properties of both
        ///     objects
        ///     are equal to each other.
        /// </returns>
        public override bool Equals(object obj)
        {
            var o = obj as ILSLConstantSignature;
            if (o == null)
            {
                return false;
            }

            return o.Name == Name && o.Type == Type;
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to the <see cref="LSLType" /> passed in <paramref name="type" />.
        /// </summary>
        /// <param name="type">The <see cref="LSLType" /> <paramref name="valueString" /> must be valid for.</param>
        /// <param name="valueString">The value string to validate.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="valueString" /> can successfully be parsed for the given <see cref="LSLType" />
        ///     .
        /// </returns>
        /// <exception cref="ArgumentException">if <paramref name="type" /> is <see cref="LSLType.Void" />.</exception>
        public static bool ValidateValueString(LSLType type, string valueString)
        {
            string discard;
            return TryParseValueString(type, valueString, out discard);
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to the <see cref="LSLType" /> passed in <paramref name="type" />.
        /// </summary>
        /// <param name="type">The <see cref="LSLType" /> <paramref name="valueString" /> must be valid for.</param>
        /// <param name="valueString">The value string to validate.</param>
        /// <param name="formated">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="valueString" /> can successfully be parsed for the given <see cref="LSLType" />
        ///     .
        /// </returns>
        /// <exception cref="ArgumentException">if <paramref name="type" /> is <see cref="LSLType.Void" />.</exception>
        public static bool TryParseValueString(LSLType type, string valueString, out string formated)
        {
            if (type == LSLType.Void)
            {
                throw new ArgumentException("type must not be LSLType.Void", "type");
            }

            formated = null;


            switch (type)
            {
                case LSLType.Float:
                    return TryParseFloatValueString(valueString, out formated);
                case LSLType.Integer:
                    return TryParseIntegerValueString(valueString, out formated);
                case LSLType.List:
                    return TryParseListValueString(valueString, out formated);
                case LSLType.Vector:
                    return TryParseVectorValueString(valueString, out formated);
                case LSLType.Rotation:
                    return TryParseRotationValueString(valueString, out formated);
                case LSLType.String:
                case LSLType.Key:
                    formated = valueString;
                    return true;
            }

            return false;
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.Float" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <param name="errMessage">An error message describing why the parse failed if this function returns <c>false</c>.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.Float" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseFloatValueString(string value, out string valueString, out string errMessage)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "value cannot be null.");
            }

            valueString = null;

            string stripSpecifiers = value.TrimEnd('f', 'F', 'd', 'D');

            double f;
            if (!double.TryParse(stripSpecifiers, out f))
            {
                int i;
                if (!int.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out i))
                {
                    errMessage =
                        string.Format("Float Constant ValueString:  Given string '{0}' is not a valid float value.",
                            value);

                    return false;
                }
                valueString = value;
            }
            else
            {
                valueString = stripSpecifiers;
            }

            errMessage = null;
            return true;
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.Float" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.Float" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseFloatValueString(string value, out string valueString)
        {
            string discard;
            return TryParseFloatValueString(value, out valueString, out discard);
        }


        private void SetFloatValueString(string value)
        {
            string msg;
            if (!TryParseFloatValueString(value, out _valueString, out msg))
            {
                throw new LSLInvalidConstantValueStringException(msg);
            }
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.String" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <param name="errMessage">An error message describing why the parse failed if this function returns <c>false</c>.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.String" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseIntegerValueString(string value, out string valueString, out string errMessage)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "value cannot be null.");
            }

            valueString = null;

            bool b;
            if (!bool.TryParse(value, out b))
            {
                int i;
                if (!int.TryParse(value, out i) &&
                    !int.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out i))
                {
                    errMessage =
                        string.Format(
                            "Integer Constant ValueString:  Given value '{0}' is not a valid integer, hexadecimal or boolean value.",
                            value);

                    return false;
                }

                valueString = value;
            }
            else
            {
                valueString = b ? "1" : "0";
            }

            errMessage = null;
            return true;
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.String" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.String" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseIntegerValueString(string value, out string valueString)
        {
            string discard;
            return TryParseIntegerValueString(value, out valueString, out discard);
        }


        private void SetIntegerValueString(string value)
        {
            string msg;
            if (!TryParseIntegerValueString(value, out _valueString, out msg))
            {
                throw new LSLInvalidConstantValueStringException(msg);
            }
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.List" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <param name="errMessage">An error message describing why the parse failed if this function returns <c>false</c>.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.List" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseListValueString(string value, out string valueString, out string errMessage)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "value cannot be null.");
            }

            valueString = null;

            try
            {
                string s = value.Trim(' ');

                if (string.IsNullOrWhiteSpace(s))
                {
                    errMessage =
                        "List Constant ValueString Invalid: May not be whitespace.";
                    return false;
                }


                char firstChar = s[0];
                char lastChar = s[s.Length - 1];

                if ((firstChar == '[' || lastChar == ']') &&
                    (firstChar != '[' || lastChar != ']'))
                {
                    errMessage =
                        "List Constant ValueString '{0}' Invalid: If brackets are used for the List value string, both brackets must be present.";
                    return false;
                }
                if (firstChar == '[')
                {
                    s = s.Substring(1, s.Length - 2);
                }

                valueString = string.IsNullOrWhiteSpace(s) ? "" : LSLListParser.Format("[" + s + "]", false);
            }
            catch (LSLListParserSyntaxException e)
            {
                errMessage = "List Constant ValueString Invalid: " + e.Message;
                return false;
            }

            errMessage = null;
            return true;
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.List" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.List" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseListValueString(string value, out string valueString)
        {
            string discard;
            return TryParseListValueString(value, out valueString, out discard);
        }


        private void SetListValueString(string value)
        {
            string msg;
            if (!TryParseListValueString(value, out _valueString, out msg))
            {
                throw new LSLInvalidConstantValueStringException(msg);
            }
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.Rotation" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <param name="errMessage">An error message describing why the parse failed if this function returns <c>false</c>.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.Rotation" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseRotationValueString(string value, out string valueString, out string errMessage)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "value cannot be null.");
            }

            valueString = null;

            string s = value.Trim(' ');


            if (string.IsNullOrWhiteSpace(s))
            {
                errMessage =
                    "Rotation Constant ValueString Invalid: May not be whitespace.";
                return false;
            }


            char firstChar = s[0];
            char lastChar = s[s.Length - 1];

            if ((firstChar == '<' || lastChar == '>') &&
                (firstChar != '<' || lastChar != '>'))
            {
                errMessage =
                    "Rotation Constant ValueString '{0}' Invalid: If rotation quotes are used for a Rotation value string, both '<' and '>' must be present.";
                return false;
            }

            if (firstChar == '<')
            {
                s = s.Substring(1, s.Length - 2);
            }


            var match = RotationValidationRegex.Match(s);
            if (!match.Success)
            {
                errMessage =
                    string.Format("Rotation Constant ValueString: '{0}' could not be parsed and formated.", value);
                return false;
            }

            valueString = match.Groups[1] + ", " + match.Groups[2] + ", " + match.Groups[3] + ", " + match.Groups[4];

            errMessage = null;
            return true;
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.Rotation" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.Rotation" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseRotationValueString(string value, out string valueString)
        {
            string discard;
            return TryParseRotationValueString(value, out valueString, out discard);
        }


        private void SetRotationValueString(string value)
        {
            string msg;
            if (!TryParseRotationValueString(value, out _valueString, out msg))
            {
                throw new LSLInvalidConstantValueStringException(msg);
            }
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.Vector" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <param name="errMessage">An error message describing why the parse failed if this function returns <c>false</c>.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.Vector" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseVectorValueString(string value, out string valueString, out string errMessage)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "value cannot be null.");
            }

            valueString = null;

            string s = value.Trim(' ');


            if (string.IsNullOrWhiteSpace(s))
            {
                errMessage =
                    "Vector Constant ValueString Invalid: May not be null or whitespace.";

                return false;
            }

            char firstChar = s[0];
            char lastChar = s[s.Length - 1];

            if ((firstChar == '<' || lastChar == '>') &&
                (firstChar != '<' || lastChar != '>'))
            {
                errMessage =
                    "Vector Constant ValueString '{0}' Invalid: If vector quotes are used for a Vector value string, both '<' and '>' must be present.";
                return false;
            }

            if (firstChar == '<')
            {
                s = s.Substring(1, s.Length - 2);
            }


            var match = VectorValidationRegex.Match(s);
            if (!match.Success)
            {
                errMessage =
                    string.Format("Vector Constant ValueString: '{0}' could not be parsed and formated.", value);

                return false;
            }

            valueString = match.Groups[1] + ", " + match.Groups[2] + ", " + match.Groups[3] + ", " + match.Groups[4];

            errMessage = null;
            return true;
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.Vector" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.Vector" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseVectorValueString(string value, out string valueString)
        {
            string discard;
            return TryParseVectorValueString(value, out valueString, out discard);
        }


        private void SetVectorValueString(string value)
        {
            string msg;
            if (!TryParseVectorValueString(value, out _valueString, out msg))
            {
                throw new LSLInvalidConstantValueStringException(msg);
            }
        }
    }
}