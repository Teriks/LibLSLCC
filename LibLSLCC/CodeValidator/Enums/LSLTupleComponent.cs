﻿#region FileInfo

// 
// File: LSLTupleComponent.cs
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
    public enum LSLTupleComponent
    {
        X,
        Y,
        Z,
        S
    }


    public static class LSLTupleComponentTools
    {
        public static string ToComponentName(this LSLTupleComponent component)
        {
            return component.ToString().ToLower();
        }

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