#region FileInfo
// 
// File: LLSDDocumentationScraper.cs
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

using System.IO;
using LibLSLCC.LibraryData;
using LibraryDataScrapingTool.ScraperInterfaces;
using OpenMetaverse.StructuredData;

namespace LibraryDataScrapingTool.LibraryDataScrapers
{
    public class LLSDDocumentationScraper : IDocumentationProvider
    {

        private readonly OSDMap _data;

        public LLSDDocumentationScraper(string filepath)
        {
            using (var file = File.OpenRead(filepath))
            {
                _data = (OSDMap)OSDParser.DeserializeLLSDXml(file);
            }
        }


        public string DocumentFunction(LSLLibraryFunctionSignature function)
        {
            var v = LLSDLibraryData.DocumentFunction(_data, function.Name);
            return v;
        }

        public string DocumentEvent(LSLLibraryEventSignature eventHandler)
        {
            var v = LLSDLibraryData.DocumentEvent(_data, eventHandler.Name);
            return v;
        }

        public string DocumentConstant(LSLLibraryConstantSignature constant)
        {
            var v = LLSDLibraryData.DocumentConstant(_data, constant.Name);
            return v;
        }
    }
}