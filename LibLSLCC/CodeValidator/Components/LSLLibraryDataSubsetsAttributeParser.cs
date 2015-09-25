#region FileInfo

// 
// File: LSLLibraryDataSubsetsAttributeParser.cs
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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

#endregion

namespace LibLSLCC.CodeValidator.Components
{
    [Serializable]
    public class LSLLibraryDataSubsetsAttributeRegex : Regex
    {
        public LSLLibraryDataSubsetsAttributeRegex()
            : base(@"(?:([a-zA-Z]+[a-zA-Z_0-9\-]*)(?:\s*,\s*([a-zA-Z]+[a-zA-Z_0-9\-]*))*)?")
        {
        }

        protected LSLLibraryDataSubsetsAttributeRegex(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public IEnumerable<string> ParseSubsets(string parse)
        {
            var matches = Matches(parse);


            if (matches.Count > 1 && matches[0].Groups.Count > 1)
            {
                var i = 1;
                for (; i < matches[0].Groups.Count; i++)
                {
                    var str = matches[0].Groups[i].ToString();
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        yield return str;
                    }
                }
            }
        }
    }
}