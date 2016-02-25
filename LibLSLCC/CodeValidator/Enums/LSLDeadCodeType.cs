#region FileInfo

// 
// File: LSLDeadCodeType.cs
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

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Describes the cause of dead code
    /// </summary>
    public enum LSLDeadCodeType
    {
        /// <summary>
        ///     No dead code.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Dead code was found after a jump out of the current code scope.
        ///     Such as an anonymous code scope, or a code scope that belongs to a control statement.
        /// </summary>
        AfterJumpOutOfScope = 1,

        /// <summary>
        ///     Dead code was found after a constant jump that always results in an infinite loop.
        ///     A constant jump is a jump that will always occur no matter what.
        /// </summary>
        AfterJumpLoopForever = 2,

        /// <summary>
        ///     A constant jump jumps over the dead code.
        ///     A constant jump is a jump that will always occur no matter what.
        /// </summary>
        JumpOverCode = 3,

        /// <summary>
        ///     Dead code was found after a constant return path.
        /// </summary>
        AfterReturnPath = 4
    }
}