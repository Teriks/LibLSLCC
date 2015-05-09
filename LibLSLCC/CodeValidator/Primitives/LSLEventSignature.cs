using System;
using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.Primitives
{
    public class LSLEventSignature
    {
        private readonly List<LSLParameter> _parameters;

        public LSLEventSignature(LSLEventSignature other)
        {
            Name = other.Name;
            _parameters = other._parameters.ToList();
        }

        protected LSLEventSignature()
        {
            _parameters = new List<LSLParameter>();
            Name = "";
        }

        public override string ToString()
        {
            return SignatureString;
        }

  



        public LSLEventSignature(string name, IEnumerable<LSLParameter> parameters)
        {
            Name = name;

            if (parameters == null)
            {
                _parameters = new List<LSLParameter>();
            }
            else
            {
                _parameters = new List<LSLParameter>();
                foreach (var lslParameter in parameters)
                {
                    AddParameter(lslParameter);
                }
            }
        }

        protected LSLEventSignature(string name)
        {
            Name = name;
            _parameters=new List<LSLParameter>();
        }


        /// <summary>
        ///     The number of parameters the event handler signature has
        /// </summary>
        public int ParameterCount
        {
            get { return _parameters.Count(); }
        }

        /// <summary>
        ///     The event handlers name
        /// </summary>
        public string Name { get; protected set; }


        /// <summary>
        ///     Indexable list of objects describing the event handlers parameters
        /// </summary>
        public IReadOnlyList<LSLParameter> Parameters
        {
            get { return _parameters; }
        }

        public string SignatureString
        {
            get
            {
                return Name + "(" +
                       string.Join(", ", Parameters.Select(x => LSLTypeTools.ToLSLTypeString(x.Type) + " " + x.Name)) +
                       ")";
            }
        }




        /// <summary>
        ///     Determines if two event handler signatures match exactly, parameter names do not matter but parameter
        ///     types do.
        /// </summary>
        /// <param name="otherSignature">The other event handler signature to compare to.</param>
        /// <returns>True if the two signatures are identical.</returns>
        public bool SignatureMatches(LSLEventSignature otherSignature)
        {
            if (Name != otherSignature.Name)
            {
                return false;
            }
            if (ParameterCount != otherSignature.ParameterCount)
            {
                return false;
            }
            for (var i = 0; i < ParameterCount; i++)
            {
                if (Parameters[i].Type != otherSignature.Parameters[i].Type)
                {
                    return false;
                }
            }
            return true;
        }



        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash*31 + Name.GetHashCode();

            return Parameters.Aggregate(hash, (current, lslParameter) => current*31 + lslParameter.GetHashCode());
        }



        public override bool Equals(object obj)
        {
            var o = obj as LSLEventSignature;
            if (o == null)
            {
                return false;
            }

            return SignatureMatches(o);
        }


        public void AddParameter(LSLParameter parameter)
        {
            if (parameter.Variadic)
            {
                throw new ArgumentException(
                    "Cannot add variadic parameters to an event signature", "parameter");
            }

            parameter.ParameterIndex = _parameters.Count;
            _parameters.Add(parameter);
        }

        public static LSLEventSignature Parse(string cSignature)
        {
            var regex = new LSLEventSignatureRegex("", ";*");
            var m = regex.GetSignature(cSignature);
            if (m == null)
            {
                throw new ArgumentException("Syntax error parsing event signature", "cSignature");
            }
            return m;
        }
    }
}