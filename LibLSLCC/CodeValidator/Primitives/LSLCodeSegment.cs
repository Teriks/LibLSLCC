#region FileInfo

// 
// File: LSLCodeSegment.cs
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

using System.Collections.Generic;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{
    public class LSLCodeSegment
    {
        private readonly List<ILSLReadOnlyCodeStatement> _statementNodes;

        public LSLCodeSegment()
        {
            _statementNodes = new List<ILSLReadOnlyCodeStatement>();
            StartNode = null;
            EndNode = null;
            SourceCodeRange = new LSLSourceCodeRange();
        }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }
        public ILSLReadOnlyCodeStatement StartNode { get; private set; }
        public ILSLReadOnlyCodeStatement EndNode { get; private set; }

        public IReadOnlyList<ILSLReadOnlyCodeStatement> StatementNodes
        {
            get { return _statementNodes.AsReadOnly(); }
        }

        public virtual void AddStatement(ILSLReadOnlyCodeStatement statement)
        {
            EndNode = statement;
            if (StartNode == null)
            {
                StartNode = statement;
                SourceCodeRange = new LSLSourceCodeRange(statement);
                _statementNodes.Add(statement);
            }
            else
            {
                _statementNodes.Add(statement);
                SourceCodeRange.ExtendTo(EndNode);
            }
        }
    }
}