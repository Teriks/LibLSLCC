using System;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.Primitives
{
    public class LSLParameter
    {
        /// <summary>
        /// Construct a parameter object
        /// </summary>
        /// <param name="type">The parameter type</param>
        /// <param name="name">The parameter name</param>
        /// <param name="variadic">Is the parameter variadic</param>
        /// <exception cref="ArgumentException">If variadic is true and type does not equal LSLType.Void</exception>
        public LSLParameter(LSLType type, string name, bool variadic)
        {
            Type = type;
            Name = name;
            Variadic = variadic;

            if (variadic && type != LSLType.Void)
            {
                throw new ArgumentException("LSLType must be Void for variadic parameters", "type");
            }
        }


        /// <summary>
        /// Name of the parameter
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Does this parameter represent a variadic place holder
        /// </summary>
        public bool Variadic { get; private set; }

        /// <summary>
        /// The parameter type
        /// </summary>
        public LSLType Type { get; private set; }

        /// <summary>
        /// The parameter index, which gets set when the parameter is added to an LSLFunctionSignature or LSLEventSignature
        /// </summary>
        public int ParameterIndex { get; set; }



        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash*31 + Type.GetHashCode();
            hash = hash*31 + Name.GetHashCode();
            hash = hash*31 + ParameterIndex.GetHashCode();
            hash = hash*31 + Variadic.GetHashCode();
            return hash;
        }


        public override bool Equals(object obj)
        {
            var o = obj as LSLParameter;
            if (o == null)
            {
                return false;
            }

            return o.Name == Name && o.ParameterIndex == ParameterIndex && o.Type == Type && Variadic == o.Variadic;
        }
    }
}