#region FileInfo

// 
// File: LSLRotationComponent.cs
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
    /// <summary>
    ///     Enum representing LSL's rotation type components
    /// </summary>
    public enum LSLRotationComponent
    {
        X,
        Y,
        Z,
        S
    }


    public static class LSLRotationComponentTools
    {
        public static string ToComponentName(this LSLRotationComponent component)
        {
            return component.ToString().ToLower();
        }

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