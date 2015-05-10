#region


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