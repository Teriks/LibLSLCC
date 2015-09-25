#region FileInfo

// 
// File: LSLParameter.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:24 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Imports

using System;
using LibLSLCC.CodeValidator.Enums;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{
    public class LSLParameter
    {
        /// <summary>
        ///     Construct a parameter object
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
        ///     Name of the parameter
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Does this parameter represent a variadic place holder
        /// </summary>
        public bool Variadic { get; }

        /// <summary>
        ///     The parameter type
        /// </summary>
        public LSLType Type { get; }

        /// <summary>
        ///     The parameter index, which gets set when the parameter is added to an LSLFunctionSignature or LSLEventSignature
        /// </summary>
        public int ParameterIndex { get; }

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