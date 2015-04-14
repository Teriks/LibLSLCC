#region

using System.Collections.Generic;
using LibLSLCC.CodeValidator.Components;

#endregion

namespace LibraryDataScrapingTools.ScraperInterfaces
{
    public interface ILibraryData
    {
        bool LSLFunctionExist(string name);
        bool LSLConstantExist(string name);
        bool LSLEventExist(string name);

        IReadOnlyList<LSLLibraryFunctionSignature> LSLFunctionOverloads(string name);
        LSLLibraryConstantSignature LSLConstant(string name);
        LSLLibraryEventSignature LSLEvent(string name);

        IEnumerable<IReadOnlyList<LSLLibraryFunctionSignature>> LSLFunctionOverloadGroups();
        IEnumerable<LSLLibraryFunctionSignature> LSLFunctions();
        IEnumerable<LSLLibraryConstantSignature> LSLConstants();
        IEnumerable<LSLLibraryEventSignature> LSLEvents();
    }
}