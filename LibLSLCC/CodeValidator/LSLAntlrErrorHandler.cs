#region FileInfo

// 
// File: LSLAntlrErrorHandler.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:25 PM
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

using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Components.Interfaces;

#endregion

namespace LibLSLCC.CodeValidator
{
    internal class LSLAntlrErrorHandler<T> : IAntlrErrorListener<T>
    {
        private readonly ILSLSyntaxErrorListener _errorListener;

        public LSLAntlrErrorHandler(ILSLSyntaxErrorListener errorListener)
        {
            _errorListener = errorListener;
        }

        public bool HasErrors { get; private set; }

        public void SyntaxError(IRecognizer recognizer, T offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            HasErrors = true;
            _errorListener.GrammarLevelSyntaxError(line, charPositionInLine, msg);
        }
    }
}