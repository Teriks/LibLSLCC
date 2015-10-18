#region FileInfo
// 
// File: LSLRotationComponent.cs
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
    ///     Enum representing LSL's rotation type components
    /// </summary>
    public enum LSLRotationComponent
    {
        /// <summary>
        /// X Quaternion Component.
        /// </summary>
        X,

        /// <summary>
        /// Y Quaternion Component.
        /// </summary>
        Y,

        /// <summary>
        /// Z Quaternion Component.
        /// </summary>
        Z,

        /// <summary>
        /// S Quaternion Component.
        /// </summary>
        S
    }


    /// <summary>
    /// Extensions for LSLRotationComponent. Used to convert the component enum into a name, or parse it from a string.
    /// </summary>
    public static class LSLRotationComponentTools
    {
        /// <summary>
        /// Convert the component into a string that would be valid to use as a component reference in actual LSL code.
        /// </summary>
        /// <param name="component">The component to convert to a string.</param>
        /// <returns>The lowercase name of the component from the enum.</returns>
        public static string ToComponentName(this LSLRotationComponent component)
        {
            return component.ToString().ToLower();
        }

        /// <summary>
        /// Converts a string into an LSLRotationComponent.
        /// </summary>
        /// <param name="name">The component name to parse, without the dot operator in front.</param>
        /// <exception cref="ArgumentException">Thrown if 'name' was not: "x", "y", "z" or "s"</exception>
        /// <exception cref="ArgumentNullException">Thrown if 'name' is null.</exception>
        /// <returns>The corresponding LSLRotationComponent.</returns>
        public static LSLRotationComponent ParseComponentName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }


            switch (name)
            {
                case "x":
                    return LSLRotationComponent.X;
                case "y":
                    return LSLRotationComponent.Y;
                case "z":
                    return LSLRotationComponent.Z;
                case "s":
                    return LSLRotationComponent.S;
            }

            throw new ArgumentException(
                string.Format("Could not parse \"{0}\" into an LSLRotationComponent, invalid name", name), "name");
        }
    }
}