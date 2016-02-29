#region FileInfo

// 
// File: LSLBinaryOperationSignature.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 

#endregion

#region Imports

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Represents the signature of a binary operation. Includes the types on either side of the expression, and the
    ///     operation type.
    /// </summary>
    public sealed class LSLBinaryOperationSignature
    {
        /// <summary>
        ///     Construct an <see cref="LSLBinaryOperationSignature" /> from the source code string representation of the binary
        ///     operation,
        ///     the operations return type, the return type of the expression on the left and the return type of the expression on
        ///     the right.
        /// </summary>
        /// <param name="operation">Source code string representation of the operator.</param>
        /// <param name="returns">The return type of the binary operation.</param>
        /// <param name="left">The return type of the expression on the left side of the binary operation.</param>
        /// <param name="right">The return type of the expression on the right side of the binary operation.</param>
        public LSLBinaryOperationSignature(string operation, LSLType returns, LSLType left, LSLType right)
        {
            Returns = returns;
            Left = left;
            Right = right;
            Operation = LSLBinaryOperationTypeTools.ParseFromOperator(operation);
        }


        /// <summary>
        ///     Construct an <see cref="LSLBinaryOperationSignature" /> from an <see cref="LSLBinaryOperationType" />, the
        ///     operations return type,
        ///     the return type of the expression on the left and the return type of the expression on the right.
        /// </summary>
        /// <param name="operation">The <see cref="LSLBinaryOperationType" /> of the binary operation signature.</param>
        /// <param name="returns">The return type of the binary operation.</param>
        /// <param name="left">The return type of the expression on the left side of the binary operation.</param>
        /// <param name="right">The return type of the expression on the right side of the binary operation.</param>
        public LSLBinaryOperationSignature(LSLBinaryOperationType operation, LSLType returns, LSLType left,
            LSLType right)
        {
            Returns = returns;
            Left = left;
            Right = right;
            Operation = operation;
        }


        /// <summary>
        ///     The return type of the binary operation.
        /// </summary>
        public LSLType Returns { get; private set; }

        /// <summary>
        ///     The return type of the expression on the left side of the binary operation.
        /// </summary>
        public LSLType Left { get; private set; }

        /// <summary>
        ///     The return type of the expression on the right side of the binary operation.
        /// </summary>
        public LSLType Right { get; private set; }

        /// <summary>
        ///     The <see cref="LSLBinaryOperationType" /> describing the operator used in the binary expression.
        /// </summary>
        public LSLBinaryOperationType Operation { get; private set; }


        /// <summary>
        ///     Gets a hash code for the <see cref="LSLBinaryOperationSignature" /> object derived from the properties: Returns,
        ///     Left, Right and Operation.
        /// </summary>
        /// <returns>The hash code for this object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash*31 + Left.GetHashCode();
                hash = hash*31 + Right.GetHashCode();
                hash = hash*31 + Operation.GetHashCode();
                hash = hash*31 + Returns.GetHashCode();
                return hash;
            }
        }


        /// <summary>
        ///     Compares this <see cref="LSLBinaryOperationSignature" /> to another <see cref="LSLBinaryOperationSignature" /> or
        ///     object.
        ///     The properties: Returns, Left, Right and Operation are compared for equality and if they are all equal True is
        ///     returned, otherwise False.
        ///     If 'obj' is not an <see cref="LSLBinaryOperationSignature" />, then false is always returned.
        /// </summary>
        /// <param name="obj">The other <see cref="LSLBinaryOperationSignature" /> to compare this one to.</param>
        /// <returns>
        ///     True if 'obj' is an <see cref="LSLBinaryOperationSignature" /> object, and the properties: Returns, Left,
        ///     Right and Operation are equal in both of them.
        /// </returns>
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