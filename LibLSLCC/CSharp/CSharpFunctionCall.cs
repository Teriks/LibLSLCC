using System;
using System.Xml.Serialization;
using LibLSLCC.Collections;
using LibLSLCC.Settings;

namespace LibLSLCC.CSharp
{
    /// <summary>
    /// Abstraction that provides parsing and validation for CSharp inheritance list strings.
    /// </summary>
    public class CSharpFunctionCall : SettingsBaseClass<CSharpFunctionCall>, IObservableHashSetItem
    {
        private readonly IReadOnlyHashedSet<string> _hashEqualityPropertyNames = new HashedSet<string>()
        {
            "FullSignature"
        };

        private CSharpFunctionCallValidationResult _validatedSignature;
        private string _fullSignature;

        /// <summary>
        /// Parameterless constructor used by <see cref="SettingsBaseClass{CSharpNamespace}"/>
        /// </summary>
        private CSharpFunctionCall()
        {
        }


        /// <summary>
        /// Gets or sets the full signature of the function call.  Parsing and validation occurs when this property is set.
        /// </summary>
        /// <value>
        /// The full function call signature.
        /// </value>
        /// <exception cref="System.ArgumentNullException">Thrown if the value is set to <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <see cref="CSharpFunctionCallValidator.Validate"/> fails to successfully parse a value set to this property.</exception>
        public string FullSignature
        {
            get { return _fullSignature; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Inheritance list signature string cannot be null.");
                }

                var vSig = CSharpFunctionCallValidator.Validate(value);

                if (!vSig.Success)
                {
                    throw new ArgumentException(_validatedSignature.ErrorDescription, "value");
                }


                SetField(ref _validatedSignature, vSig, "ValidatedSignature");
                SetField(ref _fullSignature, value, "FullSignature");
            }
        }


        /// <summary>
        /// Gets the parsing/validation results of the function call string.
        /// </summary>
        /// <value>
        /// The parsing/validation results of the function call string.
        /// </value>
        [XmlIgnore]
        public CSharpFunctionCallValidationResult ValidatedSignature
        {
            get { return _validatedSignature; }
            set { _validatedSignature = value; }
        }





        public static implicit operator CSharpFunctionCall(string fullSignature)
        {
            return new CSharpFunctionCall(fullSignature);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFunctionCall"/> class by parsing a function call from <paramref name="callString"/>
        /// </summary>
        /// <param name="callString">The full call signature to parse.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="callString"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <see cref="CSharpFunctionCallValidator.Validate"/> fails to successfully parse <paramref name="callString"/>.</exception>
        public CSharpFunctionCall(string callString)
        {
            if (callString == null)
            {
                throw new ArgumentNullException("callString", "Inheritance list signature string cannot be null.");
            }

            _validatedSignature = CSharpFunctionCallValidator.Validate(callString);

            if (!_validatedSignature.Success)
            {
                throw new ArgumentException(_validatedSignature.ErrorDescription, "callString");
            }

            _fullSignature = callString;
        }


        /// <summary>
        /// Returns <see cref="FullSignature"/>.
        /// </summary>
        /// <returns>
        /// <see cref="FullSignature"/>.
        /// </returns>
        public override string ToString()
        {
            return FullSignature;
        }

        /// <summary>
        /// Returns the hash code of <see cref="FullSignature"/>.
        /// </summary>
        /// <returns>
        /// Returns the hash code of <see cref="FullSignature"/>. 
        /// </returns>
        public override int GetHashCode()
        {
            if (FullSignature == null) return -1;
            return FullSignature.GetHashCode();
        }


        /// <summary>
        /// Compares using <see cref="FullSignature"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is a <see cref="CSharpFunctionCall"/> with an equal <see cref="FullSignature"/> value; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var ns = obj as CSharpFunctionCall;
            if (ns == null) return false;

            if (ns.FullSignature != null && FullSignature != null)
                return FullSignature.Equals(ns.FullSignature, StringComparison.Ordinal);

            return ns.FullSignature == FullSignature;
        }



        IReadOnlyHashedSet<string> IObservableHashSetItem.HashEqualityPropertyNames
        {
            get { return _hashEqualityPropertyNames; }
        }

    }
}
