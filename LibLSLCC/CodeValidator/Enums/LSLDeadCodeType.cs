#region FileInfo

// 
// File: LSLDeadCodeType.cs
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

namespace LibLSLCC.CodeValidator.Enums
{
    /// <summary>
    ///     Describes the cause of dead code
    /// </summary>
    public enum LSLDeadCodeType
    {
        None = 0,
        AfterJumpOutOfScope = 1,
        AfterJumpLoopForever = 2,
        JumpOverCode = 3,
        AfterReturnPath = 4
    }
}