using System;
using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.Primitives
{



    public class LSLFunctionSignature
    {
        private readonly List<LSLParameter> _parameters;




        protected LSLFunctionSignature()
        {
            _parameters = new List<LSLParameter>();
            Name = "";
            ReturnType = LSLType.Void;
        }

        public override string ToString()
        {
            return SignatureString;
        }

        


        public LSLFunctionSignature(LSLFunctionSignature other)
        {
            Name = other.Name;
            _parameters = other._parameters.ToList();
            ReturnType = other.ReturnType;
            HasVariadicParameter = other.HasVariadicParameter;
            VariadicParameterIndex = other.VariadicParameterIndex;
        }

        public static LSLFunctionSignature Parse(string cSignature)
        {
            var regex = new LSLFunctionSignatureRegex("",";*");
            var m = regex.GetSignature(cSignature);
            if (m == null)
            {
                throw new ArgumentException("Syntax error parsing function signature", "cSignature");
            }
            return m;
        }


        public LSLFunctionSignature(LSLType returnType, string name, IEnumerable<LSLParameter> parameters = null)
        {
            ReturnType = returnType;
            Name = name;

            ParameterCount = 0;
            ConcreteParameterCount = 0;

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


        /// <summary>
        ///     Returns the number of parameters the function signature has including variadic parameters
        /// </summary>
        public int ParameterCount { get; set; }

        /// <summary>
        ///     Returns the number of non variadic parameters the function signature has 
        /// </summary>
        public int ConcreteParameterCount { get; set; }

        /// <summary>
        ///     The functions LSL return type
        /// </summary>
        public LSLType ReturnType { get; protected set; }

        /// <summary>
        ///     The functions name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        ///     Indexable list of objects describing the functions parameters
        /// </summary>
        public IReadOnlyList<LSLParameter> Parameters
        {
            get { return _parameters; }
        }

        public string SignatureString
        {
            get
            {
                var returnString = "";
                if (ReturnType != LSLType.Void)
                {
                    returnString = LSLTypeTools.ToLSLTypeString(ReturnType) + " ";
                }

                var paramNames=Parameters.Select(x =>
                {
                    if (!x.Variadic)
                    {
                        return LSLTypeTools.ToLSLTypeString(x.Type) + " " + x.Name;
                    }
                    return "params " + x.Name + "...";
                });

                return returnString + Name + "(" +string.Join(", ", paramNames)+")";
            }
        }

        public bool HasVariadicParameter { get; private set; }

        public int VariadicParameterIndex { get; private set; }



        public void AddParameter(LSLParameter parameter)
        {
            if (parameter.Variadic)
            {
                if (!HasVariadicParameter)
                {
                   
                    HasVariadicParameter = true;
                    VariadicParameterIndex = _parameters.Count;
                }
                else
                {
                    throw new ArgumentException(
                        "Signature already has a variadic parameter, cannot add another",
                        "parameter");
                }
            }
            else
            {
                ConcreteParameterCount++;
            }

            ParameterCount++;

            parameter.ParameterIndex = _parameters.Count;

            _parameters.Add(parameter);
        }



        /// <summary>
        ///     Determines if two function signatures match exactly, parameter names do not matter but parameter
        ///     types do.
        /// </summary>
        /// <param name="otherSignature">The other function signature to compare to</param>
        /// <returns>True if the two signatures are identical</returns>
        public bool SignatureMatches(LSLFunctionSignature otherSignature)
        {
            if (ReturnType != otherSignature.ReturnType)
            {
                return false;
            }
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


        /// <summary>
        /// This implementation of get hash code uses LSLParameter.Type.GetHashCode and LSLParameter.Type.Variadic
        /// in order to get a hash for the signature parameters. this makes parameters unique by type and variadic'ness
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash*31 + ReturnType.GetHashCode();
            hash = hash*31 + Name.GetHashCode();

            return Parameters.Aggregate(hash, (current, lslParameter) =>
            {
                var c=current*31 + lslParameter.Type.GetHashCode();
                c = c*31 + lslParameter.Variadic.GetHashCode();
                return c;
            });
        }


        /// <summary>
        /// Delegates SignatureMatches after checking type equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var o = obj as LSLFunctionSignature;
            if (o == null)
            {
                return false;
            }

            return SignatureMatches(o);
        }
    }
}