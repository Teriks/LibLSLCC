#region FileInfo

// 
// File: LSLBinaryOperationSignature.cs
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

using LibLSLCC.CodeValidator.Enums;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{
    public class LSLBinaryOperationSignature
    {
        public LSLBinaryOperationSignature(string operation, LSLType returns, LSLType left, LSLType right)
        {
            Returns = returns;
            Left = left;
            Right = right;
            Operation = LSLBinaryOperationTypeTools.ParseFromOperator(operation);
        }

        public LSLBinaryOperationSignature(LSLBinaryOperationType operation, LSLType returns, LSLType left,
            LSLType right)
        {
            Returns = returns;
            Left = left;
            Right = right;
            Operation = operation;
        }

        public LSLType Returns { get; }
        public LSLType Left { get; }
        public LSLType Right { get; }
        public LSLBinaryOperationType Operation { get; }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash*31 + Left.GetHashCode();
            hash = hash*31 + Right.GetHashCode();
            hash = hash*31 + Operation.GetHashCode();
            hash = hash*31 + Returns.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            var o = obj as LSLBinaryOperationSignature;
            if (o == null)
            {
                return false;
            }

            return o.Left == Left && o.Right == Right && o.Operation == Operation && o.Returns == Returns;
        }
    }
}