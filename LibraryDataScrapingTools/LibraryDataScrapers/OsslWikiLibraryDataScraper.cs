#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibraryDataScrapingTools.ScraperInterfaces;
using LibraryDataScrapingTools.ScraperProxys;

#endregion

namespace LibraryDataScrapingTools.LibraryDataScrapers
{
    public class OsslWikiLibraryDataScraper : ILibraryData
    {
        private readonly WebClient _client = new WebClient();


        private readonly Regex _constantPageNames =
            new Regex(
                "<td>\\s*<a\\s*href\\s*=\\s*\"/index.php\\?title=OSSL_Constants/(.*?)&amp;action=edit&amp;redlink=1\"\\s+class\\s*=\\s*\"new\"\\s+title\\s*=\\s*\"OSSL Constants/.*?\">(.*?)</a>\\s+</td>(?:\\n|\\r|.)*?<td>(?:\\n|\\r)*?(.*?)(?:\\n|\\r)*?</td>");

        private readonly Regex _constantPageNames2 =
            new Regex(
                "<a\\s+href\\s*=\\s*\"/wiki/OSSL_Constants/(.*?)\"\\s+title\\s*=\\s*\"OSSL Constants/.*?\">(.*?)</a>\\s+</td>(?:\\n|\\r|.)*?<td>(?:\\n|\\r)*?(.*?)(?:\\n|\\r)*?</td>");

        private readonly Dictionary<string, LSLLibraryConstantSignature> _constants =
            new Dictionary<string, LSLLibraryConstantSignature>();


        private readonly Regex _functionPageAllFunctionsCatagory =
            new Regex(
                "<h2>Pages in category \"OSSL Functions\"</h2>((?:.|\\n||\\r)*?)</div></div></div><div class=\"printfooter\">");

        private readonly Regex _functionPageFunctionSignatureSection =
            new Regex(
                "<td colspan=\".*?\"><div style=\"font-size:18px;margin-bottom:5px;\">((?:.|\\n||\\r)*?)(?:(?:C#:\\s+)|(?:</td></tr>))");

        private readonly Regex _functionPageLinks = new Regex("href\\s*=\\s*\"(/wiki/Os.*?)\"");


        private readonly LSLFunctionSignatureRegex _functionPageSignatureRegex;


        private readonly Dictionary<string, List<LSLLibraryFunctionSignature>> _functions =
            new Dictionary<string, List<LSLLibraryFunctionSignature>>();

        private readonly IEnumerable<string> _subsets;

        private readonly Dictionary<string, string> _wikiTypeToLslType = new Dictionary<string, string>
        {
            {"void", "void"},
            {"Void", "void"},
            {"integer", "integer"},
            {"int32", "integer"},
            {"Integer", "integer"},
            {"Int32", "integer"},
            {"Int", "integer"},
            {"int", "integer"},
            {"LSL_Types.LSLInteger", "integer"},
            {"LSLInteger", "integer"},
            {"LSL_Integer", "integer"},
            {"LSL_integer", "integer"},
            {"float", "float"},
            {"Float", "float"},
            {"double", "float"},
            {"Double", "float"},
            {"LSLFloat", "float"},
            {"LSL_Float", "float"},
            {"LSL_float", "float"},
            {"LSL_Types.LSLFloat", "float"},
            {"string", "string"},
            {"String", "string"},
            {"LSLString", "string"},
            {"LSL_String", "string"},
            {"LSL_Sstring", "string"},
            {"LSL_Types.LSLString", "string"},
            {"list", "list"},
            {"List", "list"},
            {"LSL_Types.list", "list"},
            {"LSL_List", "list"},
            {"LSL_list", "list"},
            {"vector", "vector"},
            {"Vector", "vector"},
            {"Vector3", "vector"},
            {"LSL_Types.Vector3", "vector"},
            {"LSL_Vector", "vector"},
            {"LSL_vector", "vector"},
            {"rotation", "rotation"},
            {"Rotation", "rotation"},
            {"Quaternion", "rotation"},
            {"quaternion", "rotation"},
            {"LSL_Types.Quaternion", "rotation"},
            {"LSL_Quaternion", "rotation"},
            {"LSL_quaternion", "rotation"},
            {"key", "key"},
            {"Key", "key"},
            {"LSL_Types.key", "key"},
            {"LSL_Key", "key"},
            {"LSL_key", "key"},
        };

        public OsslWikiLibraryDataScraper(IDocumentationProvider provider, IEnumerable<string> subsets)
        {
            _subsets = subsets.ToList();
            _functionPageSignatureRegex = new LSLFunctionSignatureRegex(_wikiTypeToLslType.Keys, "", ";*");

            Log.WriteLine("============================");
            Log.WriteLine("Starting scrape of http://opensimulator.org/wiki/ ... ");
            Log.WriteLine("============================");


            foreach (var lslLibraryConstantSignature in GetLSLConstants())
            {
                lslLibraryConstantSignature.DocumentationString =
                    provider.DocumentConstant(lslLibraryConstantSignature);

                _constants.Add(lslLibraryConstantSignature.Name, lslLibraryConstantSignature);
            }


            foreach (var lslLibraryFunctionSignature in GetLSLFunctions())
            {
                lslLibraryFunctionSignature.DocumentationString =
                    provider.DocumentFunction(lslLibraryFunctionSignature);

                if (_functions.ContainsKey(lslLibraryFunctionSignature.Name))
                {
                    _functions[lslLibraryFunctionSignature.Name].Add(lslLibraryFunctionSignature);
                }
                else
                {
                    _functions.Add(lslLibraryFunctionSignature.Name,
                        new List<LSLLibraryFunctionSignature> {lslLibraryFunctionSignature});
                }
            }


            Log.WriteLine("============================");
            Log.WriteLine("Finished scrape of http://opensimulator.org/wiki/");
            Log.WriteLine("============================");
        }

        public OsslWikiLibraryDataScraper(IEnumerable<string> subsets) : this(new BlankDocumentor(), subsets)
        {
        }

        public bool LSLFunctionExist(string name)
        {
            return _functions.ContainsKey(name);
        }

        public bool LSLConstantExist(string name)
        {
            return _constants.ContainsKey(name);
        }

        public bool LSLEventExist(string name)
        {
            return false;
        }

        public IReadOnlyList<LSLLibraryFunctionSignature> LSLFunctionOverloads(string name)
        {
            if (LSLFunctionExist(name))
            {
                return _functions[name];
            }
            return new List<LSLLibraryFunctionSignature>();
        }

        public LSLLibraryConstantSignature LSLConstant(string name)
        {
            if (LSLConstantExist(name))
            {
                return _constants[name];
            }
            return null;
        }

        public LSLLibraryEventSignature LSLEvent(string name)
        {
            return null;
        }

        public IEnumerable<IReadOnlyList<LSLLibraryFunctionSignature>> LSLFunctionOverloadGroups()
        {
            return _functions.Values;
        }

        public IEnumerable<LSLLibraryFunctionSignature> LSLFunctions()
        {
            return _functions.Values.SelectMany(x => x);
        }

        public IEnumerable<LSLLibraryConstantSignature> LSLConstants()
        {
            return _constants.Values;
        }

        public IEnumerable<LSLLibraryEventSignature> LSLEvents()
        {
            yield break;
        }

        private IEnumerable<LSLLibraryFunctionSignature> GetLSLFunctions()
        {
            foreach (var functionPage in FunctionPages())
            {
                var p = _client.DownloadString(functionPage);

                p = _functionPageFunctionSignatureSection.Match(p).ToString();


                foreach (var sig in _functionPageSignatureRegex.GetSimpleSignatures(p))
                {
                    if (_wikiTypeToLslType.ContainsKey(sig.ReturnType))
                    {
                        var returnType = LSLType.Void;

                        string actualReturnType = _wikiTypeToLslType[sig.ReturnType];


                        if (actualReturnType != "void")
                        {
                            returnType = LSLTypeTools.FromLSLTypeString(actualReturnType);
                        }


                        var parameters = new List<LSLParameter>();
                        bool badParam = false;
                        string badParamType = "";

                        foreach (var pa in sig.Parameters)
                        {
                            if (_wikiTypeToLslType.ContainsKey(pa.Key))
                            {
                                var parameterType = LSLType.Void;
                                var actualPararmeterType = _wikiTypeToLslType[pa.Key];
                                if (actualPararmeterType != "void")
                                {
                                    parameterType = LSLTypeTools.FromLSLTypeString(actualPararmeterType);
                                }

                                parameters.Add(new LSLParameter(
                                    parameterType, pa.Value,false));
                            }
                            else
                            {
                                badParamType = pa.Key;
                                badParam = true;
                                break;
                            }
                        }

                        if (badParam)
                        {
                            Log.WriteLine("OssFunctionScraper: function parameter type on wiki not recognized {0)",
                                badParamType);
                        }
                        else
                        {
                            var func = new LSLLibraryFunctionSignature(returnType, sig.Name.Trim(), parameters);

                            func.SetSubsets(_subsets);
                            yield return func;
                        }
                    }
                    else
                    {
                        Log.WriteLine("OssFunctionScraper: function return type on wiki not recognized {0)",
                            sig.ReturnType);
                    }
                }
            }
        }

        private IEnumerable<string> FunctionPages()
        {
            const string functionsPage = "http://opensimulator.org/wiki/Category:OSSL_Functions";

            var downloadString = _client.DownloadString(functionsPage);

            downloadString = _functionPageAllFunctionsCatagory.Match(downloadString).ToString();

            Log.WriteLine("OsslWikiLibraryDataScraper: traversing function page links on {0}", functionsPage);

            foreach (Match link in _functionPageLinks.Matches(downloadString))
            {
                var page = "http://opensimulator.org" + link.Groups[1];
                Log.WriteLine("OsslWikiLibraryDataScraper: discovered function page {0}", page);
                yield return page;
            }
        }

        private IEnumerable<LSLLibraryConstantSignature> GetLSLConstants()
        {
            const string constantsPage = "http://opensimulator.org/wiki/OSSL_Constants";

            var constantsList = _client.DownloadString(constantsPage);

            Log.WriteLine("OsslWikiLibraryDataScraper: scraping constants from {0}", constantsPage);

            foreach (Match constant in _constantPageNames.Matches(constantsList))
            {
                string value=constant.Groups[3].ToString().Trim();
                string strValue;
                if(value.Contains("x"))
                {
                    strValue = Convert.ToInt32(value, 16).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    strValue = value;
                }

                var constantSignature =
                    new LSLLibraryConstantSignature(LSLType.Integer,
                        constant.Groups[1].ToString().Trim(), strValue
                        );

                constantSignature.SetSubsets(_subsets);

                yield return constantSignature;
            }

            foreach (Match constant in _constantPageNames2.Matches(constantsList))
            {
                string value = constant.Groups[3].ToString().Trim();
                string strValue;
                if (value.Contains("x"))
                {
                    strValue = Convert.ToInt32(value, 16).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    strValue = value;
                }

                var constantSignature =
                    new LSLLibraryConstantSignature(LSLType.Integer, constant.Groups[1].ToString().Trim(), strValue);

                constantSignature.SetSubsets(_subsets);

                yield return constantSignature;
            }
        }
    }
}