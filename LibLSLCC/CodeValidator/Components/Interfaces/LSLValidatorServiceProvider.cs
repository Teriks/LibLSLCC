#region FileInfo

// 
// File: LSLValidatorServiceProvider.cs
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

namespace LibLSLCC.CodeValidator.Components.Interfaces
{
    /// <summary>
    ///     Represents various sub strategies and listeners that are used in LSLCodeValidator's implementation
    /// </summary>
    public interface ILSLValidatorServiceProvider
    {
        ILSLExpressionValidator ExpressionValidator { get; }
        ILSLMainLibraryDataProvider MainLibraryDataProvider { get; }
        ILSLStringPreProcessor StringLiteralPreProcessor { get; }
        ILSLSyntaxErrorListener SyntaxErrorListener { get; }
        ILSLSyntaxWarningListener SyntaxWarningListener { get; }
    }

    public static class ILSLValidatorServiceProvideExtensions
    {
        /// <summary>
        ///     Returns true if all service provider properties are non null
        /// </summary>
        /// <param name="provider">The ILSLValidatorServiceProvider to check</param>
        /// <returns>True if all properties are initialized</returns>
        public static bool IsComplete(this ILSLValidatorServiceProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }


            return provider.ExpressionValidator != null
                   && provider.MainLibraryDataProvider != null
                   && provider.StringLiteralPreProcessor != null
                   && provider.SyntaxErrorListener != null
                   && provider.SyntaxWarningListener != null;
        }
    }
}