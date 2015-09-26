#region FileInfo
// 
// File: LSLLibraryDataSubsetsAttributeParser.cs
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