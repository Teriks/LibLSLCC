#region FileInfo

// 
// File: LSLVectorComponent.cs
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

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Enum representing LSL's vector type components
    /// </summary>
    public enum LSLVectorComponent
    {
        /// <summary>
        ///     X Axis Component
        /// </summary>
        X,

        /// <summary>
        ///     Y Axis Component
        /// </summary>
        Y,

        /// <summary>
        ///     Z Axis Component
        /// </summary>
        Z
    }

    /// <summary>
    ///     <see cref="LSLVectorComponent" /> extension methods for converting the <see cref="LSLVectorComponent" /> into a
    ///     properly formed string and back.
    /// </summary>
    public static class LSLVectorComponentTools
    {
        /// <summary>
        ///     Converts the <see cref="LSLVectorComponent" /> into a name reference that could be used on the right side of the
        ///     dot operator in LSL.
        /// </summary>
        /// <param name="component">The component to convert to a string.</param>
        /// <returns>The lowercase name of the component from the enum.</returns>
        public static string ToComponentName(this LSLVectorComponent component)
        {
            return component.ToString().ToLower();
        }


        /// <summary>
        ///     Converts a string into an <see cref="LSLVectorComponent" />.
        /// </summary>
        /// <param name="name">The name of the component ("x", "y" or "z").</param>
        /// <exception cref="ArgumentNullException">Thrown if 'name' is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if 'name' is not one of: "x", "y" or "z".  (Case Sensitive)</exception>
        /// <returns></returns>
        public static LSLVectorComponent ParseComponentName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }


            switch (name)
            {
                case "x":
                    return LSLVectorComponent.X;
                case "y":
                    return LSLVectorComponent.Y;
                case "z":
                    return LSLVectorComponent.Z;
            }

            throw new ArgumentException(
                string.Format("Could not parse \"{0}\" into an LSLVectorComponent, invalid name", name), "name");
        }
    }
}