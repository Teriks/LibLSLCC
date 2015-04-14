#region

using LibLSLCC.CodeValidator.Components;
using LibraryDataScrapingTools.ScraperInterfaces;

#endregion

namespace LibraryDataScrapingTools.ScraperProxys
{
    public class BlankDocumentor : IDocumentationProvider
    {
        public string DocumentFunction(LSLLibraryFunctionSignature function)
        {
            return "";
        }

        public string DocumentEvent(LSLLibraryEventSignature eventHandler)
        {
            return "";
        }

        public string DocumentConstant(LSLLibraryConstantSignature constant)
        {
            return "";
        }
    }
}