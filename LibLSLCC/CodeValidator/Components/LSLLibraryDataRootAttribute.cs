using System;
using System.Xml.Serialization;

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    ///     If you derive from LSLXmlLibraryDataProvider, the derived class must have this attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class LSLXmlLibraryDataRootAttribute : XmlRootAttribute
    {
        public readonly static string RootElementName = "LSLLibraryData";



        public LSLXmlLibraryDataRootAttribute()
        {
            IsNullable = false;
            ElementName = RootElementName;
        }
    }
}