#region FileInfo

// 
// File: CompoundDocumentationScraper.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:27 PM
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
using System.Linq;
using LibLSLCC.CodeValidator.Components;
using LibraryDataScrapingTools.ScraperInterfaces;

#endregion

namespace LibraryDataScrapingTools.ScraperProxys
{
    public class CompoundDocumentationScraper : IDocumentationProvider
    {
        private readonly List<IDocumentationProvider> _providers = new List<IDocumentationProvider>();

        public CompoundDocumentationScraper(params IDocumentationProvider[] providers)
        {
            _providers.AddRange(providers);
        }

        public string DocumentFunction(LSLLibraryFunctionSignature function)
        {
            return
                _providers.Select(documentationProvider => documentationProvider.DocumentFunction(function))
                    .FirstOrDefault(d => !string.IsNullOrWhiteSpace(d));
        }

        public string DocumentEvent(LSLLibraryEventSignature eventHandler)
        {
            return
                _providers.Select(documentationProvider => documentationProvider.DocumentEvent(eventHandler))
                    .FirstOrDefault(d => !string.IsNullOrWhiteSpace(d));
        }

        public string DocumentConstant(LSLLibraryConstantSignature constant)
        {
            return
                _providers.Select(documentationProvider => documentationProvider.DocumentConstant(constant))
                    .FirstOrDefault(d => !string.IsNullOrWhiteSpace(d));
        }

        public void AddProvider(IDocumentationProvider p)
        {
            _providers.Add(p);
        }
    }
}