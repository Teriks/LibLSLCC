using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LibLSLCC.Collections;
using LibLSLCC.Settings;

namespace LibLSLCC.CSharp
{
    public class CSharpInheritanceList : SettingsBaseClass<CSharpInheritanceList>, IObservableHashSetItem
    {
        private readonly IReadOnlyHashedSet<string> _hashEqualityPropertyNames = new HashedSet<string>()
        {
            "FullSignature"
        };

        private CSharpInheritanceListValidationResult _validatedList;
        private string _fullSignature;


        private CSharpInheritanceList()
        {
        }

        public string FullSignature
        {
            get { return _fullSignature; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Inheritance list signature string cannot be null.");
                }

                var vSig = CSharpInheritanceListValidator.Validate(value);

                if (!vSig.Success)
                {
                    throw new ArgumentException(_validatedList.ErrorDescription, "value");
                }


                SetField(ref _validatedList, vSig, "ValidatedList");
                SetField(ref _fullSignature, value, "FullSignature");
            }
        }

        [XmlIgnore]
        public CSharpInheritanceListValidationResult ValidatedList
        {
            get { return _validatedList; }
            set { _validatedList = value; }
        }

        [XmlIgnore]
        public string ListWithColonIfNeccessary
        {
            get
            {
                if (FullSignature == null) return null;

                if (_validatedList.InheritedTypes.Length > 0)
                {
                    return ": " + FullSignature;
                }
                return FullSignature;
            }
        }


        public static implicit operator CSharpInheritanceList(string fullSignature)
        {
            return new CSharpInheritanceList(fullSignature);
        }


        public CSharpInheritanceList(string fullSignature)
        {
            if (fullSignature == null)
            {
                throw new ArgumentNullException("fullSignature", "Inheritance list signature string cannot be null.");
            }

            _validatedList = CSharpInheritanceListValidator.Validate(fullSignature);

            if (!_validatedList.Success)
            {
                throw new ArgumentException(_validatedList.ErrorDescription, "fullSignature");
            }

            _fullSignature = fullSignature;
        }

        public override string ToString()
        {
            return FullSignature;
        }


        public override int GetHashCode()
        {
            if (FullSignature == null) return -1;
            return FullSignature.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var ns = obj as CSharpInheritanceList;
            if (ns == null) return false;

            if (ns.FullSignature != null && FullSignature != null)
                return FullSignature.Equals(ns.FullSignature, StringComparison.Ordinal);

            return ns.FullSignature == FullSignature;
        }

        public IReadOnlyHashedSet<string> HashEqualityPropertyNames
        {
            get { return _hashEqualityPropertyNames; }
        }

        public bool IsEmpty
        {
            get { return string.IsNullOrWhiteSpace(FullSignature); }
        }
    }
}