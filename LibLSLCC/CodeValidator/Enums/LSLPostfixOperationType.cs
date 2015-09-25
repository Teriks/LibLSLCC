#region FileInfo

// 
// File: LSLPostfixOperationType.cs
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
    public enum LSLPostfixOperationType
    {
        Increment = 2,
        Decrement = 1,
        Error = 0
    }

    public static class LSLPostfixOperationTypeTools
    {
        public static string ToOperatorString(this LSLPostfixOperationType type)
        {
            switch (type)
            {
                case LSLPostfixOperationType.Decrement:
                    return "--";
                case LSLPostfixOperationType.Increment:
                    return "++";
            }

            throw new ArgumentException(
                string.Format("Could not convert LSLPostfixOperationType.{0} enum value to operator string", type),
                "type");
        }

        public static LSLPostfixOperationType ParseFromOperator(string operationString)
        {
            if (string.IsNullOrEmpty(operationString))
            {
                throw new ArgumentNullException("operationString");
            }

            switch (operationString)
            {
                case "--":
                    return LSLPostfixOperationType.Decrement;
                case "++":
                    return LSLPostfixOperationType.Increment;
            }

            throw new ArgumentException(
                string.Format("Could not parse \"{0}\" into an LSLPostfixOperationType, invalid operator string",
                    operationString), "operationString");
        }
    }
}