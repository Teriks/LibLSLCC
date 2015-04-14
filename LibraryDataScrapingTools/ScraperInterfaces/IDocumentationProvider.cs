#region

using LibLSLCC.CodeValidator.Components;

#endregion

namespace LibraryDataScrapingTools.ScraperInterfaces
{
    public interface IDocumentationProvider
    {
        string DocumentFunction(LSLLibraryFunctionSignature function);
        string DocumentEvent(LSLLibraryEventSignature eventHandler);
        string DocumentConstant(LSLLibraryConstantSignature constant);
    }
}