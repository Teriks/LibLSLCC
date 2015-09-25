#region FileInfo

// 
// File: LSLType.cs
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

#endregion

namespace LibLSLCC.CodeValidator.Enums
{
    public enum LSLType
    {
        /// <summary>
        ///     LSL type key
        /// </summary>
        Key = 7,

        /// <summary>
        ///     LSL type integer
        /// </summary>
        Integer = 6,

        /// <summary>
        ///     LSL type string
        /// </summary>
        String = 5,

        /// <summary>
        ///     LSL type float
        /// </summary>
        Float = 4,

        /// <summary>
        ///     LSL type list
        /// </summary>
        List = 3,

        /// <summary>
        ///     LSL type vector
        /// </summary>
        Vector = 2,

        /// <summary>
        ///     LSL type rotation
        /// </summary>
        Rotation = 1,

        /// <summary>
        ///     LSL type void (symbolic)
        /// </summary>
        Void = 0
    }


    public static class LSLTypeTools
    {
        /// <summary>
        ///     Convert an LSL type name into an LSLType representation (case insensitive).
        /// </summary>
        /// <param name="typeName">LSL Type name as string.</param>
        /// <returns>An LSLType representation of said type name.</returns>
        /// <exception cref="ArgumentException">If typeName was not recognized.</exception>
        /// <exception cref="ArgumentNullException">If typeName was null.</exception>
        public static LSLType FromLSLTypeString(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException("typeName");
            }


            switch (typeName.ToLower())
            {
                case "integer":
                    return LSLType.Integer;
                case "float":
                    return LSLType.Float;
                case "string":
                    return LSLType.String;
                case "key":
                    return LSLType.Key;
                case "vector":
                    return LSLType.Vector;
                case "quaternion":
                    return LSLType.Rotation;
                case "rotation":
                    return LSLType.Rotation;
                case "list":
                    return LSLType.List;
            }

            throw new ArgumentException("\"" + typeName + "\" is not a valid LSL typename", "typeName");
        }

        /// <summary>
        ///     Convert an LSLType to an LSL type string.
        /// </summary>
        /// <param name="type">LSLType to convert.</param>
        /// <returns>LSL type string representation.</returns>
        /// <exception cref="ArgumentException">If LSLType is LSLType.Void.</exception>
        public static string ToLSLTypeString(LSLType type)
        {
            if (type == LSLType.Void)
            {
                throw new ArgumentException("Cannot convert LSLType.Void to a valid LSL type string", "type");
            }

            return type.ToString().ToLower();
        }
    }
}