#region FileInfo

// 
// File: LSLPrefixOperationType.cs
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
    public enum LSLPrefixOperationType
    {
        Increment = 6,
        Decrement = 5,
        Negative = 4,
        Positive = 3,
        BooleanNot = 2,
        BitwiseNot = 1,
        Error = 0
    }


    public static class LSLPrefixOperationTypeTools
    {
        public static string ToOperatorString(this LSLPrefixOperationType type)
        {
            switch (type)
            {
                case LSLPrefixOperationType.Decrement:
                    return "--";
                case LSLPrefixOperationType.Increment:
                    return "++";
                case LSLPrefixOperationType.Negative:
                    return "-";
                case LSLPrefixOperationType.Positive:
                    return "+";
                case LSLPrefixOperationType.BooleanNot:
                    return "!";
                case LSLPrefixOperationType.BitwiseNot:
                    return "~";
            }

            throw new ArgumentException(
                string.Format("Could not convert LSLPrefixOperationType.{0} enum value to operator string", type),
                "type");
        }

        public static LSLPrefixOperationType ParseFromOperator(string operationString)
        {
            if (string.IsNullOrEmpty(operationString))
            {
                throw new ArgumentNullException("operationString");
            }

            switch (operationString)
            {
                case "--":
                    return LSLPrefixOperationType.Decrement;
                case "++":
                    return LSLPrefixOperationType.Increment;
                case "-":
                    return LSLPrefixOperationType.Negative;
                case "+":
                    return LSLPrefixOperationType.Positive;
                case "!":
                    return LSLPrefixOperationType.BooleanNot;
                case "~":
                    return LSLPrefixOperationType.BitwiseNot;
            }

            throw new ArgumentException(
                string.Format("Could not parse \"{0}\" into an LSLPrefixOperationType, invalid operator string",
                    operationString), "operationString");
        }
    }
}