#region FileInfo
// 
// File: LSLTupleComponent.cs
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

using System;

#endregion

namespace LibLSLCC.CodeValidator.Enums
{

    /// <summary>
    /// Represents the component accessed by an LSLTupleAccessorNode syntax tree node.
    /// </summary>
    public enum LSLTupleComponent
    {
        /// <summary>
        /// Component X
        /// </summary>
        X,

        /// <summary>
        /// Component Y
        /// </summary>
        Y,

        /// <summary>
        /// Component Z
        /// </summary>
        Z,

        /// <summary>
        /// Component S
        /// </summary>
        S
    }


    /// <summary>
    /// LSLTupleComponent extensions for converting LSLTupleComponent to and from its source code string equivalent.
    /// </summary>
    public static class LSLTupleComponentTools
    {

        /// <summary>
        /// Returns the source code string equivalent of the given LSLTupleComponent.
        /// Effectively the enum name as lowercase.
        /// </summary>
        /// <param name="component">The LSLTupleComponent to be converted to a string.</param>
        /// <returns>The source code equivalent string for the given LSLTupleComponent.</returns>
        public static string ToComponentName(this LSLTupleComponent component)
        {
            return component.ToString().ToLower();
        }


        /// <summary>
        /// Parses and LSLTupleComponent from its source code string equivalent.
        /// </summary>
        /// <param name="name">The string to parse the LSLTupleComponent from.</param>
        /// <exception cref="ArgumentNullException">Thrown if 'name' is null.</exception>
        /// <exception cref="ArgumentException">Thrown if 'name' is not a valid source code string equivalent to any LSLTupleComponent.</exception>
        /// <returns>The parsed LSLTupleComponent.</returns>
        public static LSLTupleComponent ParseComponentName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            switch (name)
            {
                case "x":
                    return LSLTupleComponent.X;
                case "y":
                    return LSLTupleComponent.Y;
                case "z":
                    return LSLTupleComponent.Z;
                case "s":
                    return LSLTupleComponent.S;
            }

            throw new ArgumentException(
                string.Format("Could not parse \"{0}\" into an LSLVectorComponent, invalid name", name), "name");
        }
    }
}