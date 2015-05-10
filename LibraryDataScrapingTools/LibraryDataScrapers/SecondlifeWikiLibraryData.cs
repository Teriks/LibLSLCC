#region


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibraryDataScrapingTools.ScraperInterfaces;
using LibraryDataScrapingTools.ScraperProxys;


#endregion

namespace LibraryDataScrapingTools.LibraryDataScrapers
{
    public class SecondlifeWikiLibraryData : ILibraryData
    {
        private const string SecondlifeWikiDomain = "http://wiki.secondlife.com";
        private const string SecondlifeWikiBaseUrl = SecondlifeWikiDomain + "/wiki/";

        private readonly Regex _changedEventConstants =
            new Regex("<a href=\"/wiki/[A-Z_]*?\" title=\".*?\">([A-Z_]*?)</a>(?:\\n|\\r|.)*?title=\"Hexadecimal notation for:.*?\" style=\"border-bottom:1px dotted; cursor:help;\">(.*?)</span>");

        private readonly Regex _changedEventConstantsTable =
            new Regex("<tr bgcolor=\"#A7C1F2\">(\\n|\\r|.)*?</td></tr></table>");

        private readonly Regex _clickActionConstants =
            new Regex("<span title=\"integer [A-Z_]*? = (.*?);.*?\">([A-Z_]*?)</span>");

        private readonly WebClient _client = new WebClient();

        private readonly Regex _constantInKeywordsAll =
            new Regex(
                "(integer|float|string|vector|rotation|list|key)\\s+<a\\s+href\\s*=\\s*\".*?\"\\s+title=\".*?\">(.*?)</a> = (.*)");

        private readonly Regex _constantPageLinks =
            new Regex("<li>\\s*<a\\s+href\\s*=\\s*\"(/wiki/.*?)\"\\s+title\\s*=\\s*\".*?\"\\s*>.*?</a>\\s*</li>");

        private readonly Regex _constantPageNavigation =
            new Regex(
                "(?:\\(previous 200\\) |previous 200</a>\\) )\\(<a\\s+href=\"(.*?)\"\\s+title\\s*=\\s*\"Category:LSL Constants\">next 200</a>\\)<div id=");

        private readonly Regex _constantSignature =
            new Regex(
                "Constant: <a href=\"/wiki/.*?\" title=\"(.*?)\" class=\"mw-redirect\">(?:integer|float|vector|string|list|key|quaternion)</a> <strong class=\"selflink\"><span title=\".*?\">(.*?)</span></strong>\\s*=\\s*(.*?); </span>");

        private readonly Dictionary<string, LSLLibraryConstantSignature> _constants =
            new Dictionary<string, LSLLibraryConstantSignature>();

        private readonly Regex _constantsGroupKeywordsAll =
            new Regex("<a\\s+name\\s*=\\s*\"Constants\"\\s+id\\s*=\\s*\"Constants\"></a><h2>((?:\\r|\\n|.))*?</pre>");

        private readonly Regex _eventPageCategory =
            new Regex("<h2>Pages in category \"LSL Events\"</h2>((?:.|(?:\n|\r|\r\n))*?)</tr></table>");

        private readonly Regex _eventPageNavigation =
            new Regex(
                "(?:\\(previous 200\\) |previous 200</a>\\) )\\(<a\\s+href=\"(.*?)\"\\s+title\\s*=\\s*\"Category:LSL Events\">next 200</a>\\)<div id=");


        private readonly LSLEventSignatureRegex _eventSignature =
            new LSLEventSignatureRegex("<pre id=\"lsl-signature\">\\s*event\\s+void\\s+(", ";)");


        private readonly Dictionary<string, LSLLibraryEventSignature> _events =
            new Dictionary<string, LSLLibraryEventSignature>();

        private readonly Regex _functionInKeywordsAll =
            new Regex(
                "(integer|float|string|vector|rotation|list|key|void)\\s+<a\\s+href\\s*=\\s*\".*?\"\\s+title=\".*?\">(.*?)</a>((?:.*?)\\))");

        private readonly Regex _functionPageCategory =
            new Regex("<h2>Pages in category \"LSL Functions\"</h2>((?:.|(?:\n|\r|\r\n))*?)</tr></table>");

        private readonly Regex _functionPageNavigation =
            new Regex(
                "(?:\\(previous 200\\) |previous 200</a>\\) )\\(<a\\s+href=\"(.*?)\"\\s+title\\s*=\\s*\"Category:LSL Functions\">next 200</a>\\)<div id=");

        private readonly LSLFunctionSignatureRegex _functionSignature = new LSLFunctionSignatureRegex("function\\s+(",
            ";)");

        private readonly Dictionary<string, List<LSLLibraryFunctionSignature>> _functions =
            new Dictionary<string, List<LSLLibraryFunctionSignature>>();

        private readonly Regex _functionsGroupInKeywordsAll =
            new Regex("<a\\s+name\\s*=\\s*\"Functions\"\\s+id\\s*=\\s*\"Functions\"></a><h2>((?:\\r|\\n|.))*?</pre>");

        private readonly Regex _hrefLink = new Regex("href=\"(/wiki/.*?)\"");

        private readonly Regex _physicsConstantTableRow =
            new Regex(
                "(?:(?:<tr\\s+id\\s*=\\s*\"([A-Z_]*?)\">)|(?:<tr\\s+style\\s*=\\s*\"background-color:.*?;\"\\s+id\\s*=\\s*\"([A-Z_]*?)\">))(?:\\n|\\r|.)*?</tr>");

        private readonly IEnumerable<string> _subsets;

        public SecondlifeWikiLibraryData(IDocumentationProvider documentationProvider, IEnumerable<string> subsets)
        {
            IDocumentationProvider documentationProvider1 = documentationProvider;
            _subsets = subsets.ToList();


            Log.WriteLine("============================");
            Log.WriteLine("Starting scrape of " + SecondlifeWikiDomain + " ... ");
            Log.WriteLine("============================");


            foreach (var lslLibraryConstantSignature in GetLSLConstants())
            {
                lslLibraryConstantSignature.DocumentationString =
                    documentationProvider1.DocumentConstant(lslLibraryConstantSignature);

                _constants.Add(lslLibraryConstantSignature.Name, lslLibraryConstantSignature);
            }

            foreach (var lslLibraryFunctionSignature in GetLSLFunctions())
            {
                lslLibraryFunctionSignature.DocumentationString =
                    documentationProvider1.DocumentFunction(lslLibraryFunctionSignature);

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


            foreach (var lslLibraryEventSignature in GetLSLEvents())
            {
                lslLibraryEventSignature.DocumentationString =
                    documentationProvider1.DocumentEvent(lslLibraryEventSignature);

                _events.Add(lslLibraryEventSignature.Name, lslLibraryEventSignature);
            }


            Log.WriteLine("============================");
            Log.WriteLine("Finished scrape of " + SecondlifeWikiDomain);
            Log.WriteLine("============================");
        }


        public SecondlifeWikiLibraryData(IEnumerable<string> subsets)
            : this(new BlankDocumentor(), subsets)
        {
        }

        public string EventSubsets { get; set; }
        public string ConstantSubsets { get; set; }
        public string FunctionSubsets { get; set; }


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
            return _events.ContainsKey(name);
        }

        public IReadOnlyList<LSLLibraryFunctionSignature> LSLFunctionOverloads(string name)
        {
            if (_functions.ContainsKey(name))
            {
                return _functions[name];
            }
            return new List<LSLLibraryFunctionSignature>();
        }


        public LSLLibraryConstantSignature LSLConstant(string name)
        {
            if (_constants.ContainsKey(name))
            {
                return _constants[name];
            }
            return null;
        }

        public LSLLibraryEventSignature LSLEvent(string name)
        {
            if (_events.ContainsKey(name))
            {
                return _events[name];
            }
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
            return _events.Values;
        }

        private LSLLibraryConstantSignature GetSigFromConstantPage(string url)
        {
            var page = _client.DownloadString(url);

            var hexNotation=new Regex("<span title=\"Hexadecimal notation for: (.*?)\"");

            var match = _constantSignature.Match(page);
            if (match.Success)
            {
                string val = match.Groups[3].ToString();
                string strValue;


                var type = LSLTypeTools.FromLSLTypeString(match.Groups[1].ToString());


                if (type ==LSLType.Integer || type == LSLType.Float)
                {
                    var hxMatch = hexNotation.Match(val);
                    if (hxMatch.Success)
                    {
                        strValue = hxMatch.Groups[1].ToString();
                    }
                    else if (val.Contains("x"))
                    {
                        strValue = Convert.ToInt32(val, 16).ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        strValue = val;
                    }
                }
                else if(type == LSLType.Rotation || type == LSLType.Vector)
                {
                    strValue = val.Replace("&lt;", "").Replace("&gt;", "");

                }
                else if (type == LSLType.String || type == LSLType.Key)
                {
                    strValue = val.Replace("&quot;", "");
                }
                else
                {
                    strValue = val;
                }



                var constantSignature =
                    new LSLLibraryConstantSignature(type,
                        match.Groups[2].ToString(), strValue);


                constantSignature.SetSubsets(_subsets);

                Log.WriteLine("SecondlifeWikiLibraryData: retrieved constant {0}; from {1}",
                    constantSignature.SignatureString, url);

                return constantSignature;
            }
            return null;
        }


        private LSLLibraryFunctionSignature GetSigFromFunctionPage(string url)
        {
            var page = _client.DownloadString(url);

            var matches = _functionSignature.Regex.Matches(page);
            if (matches.Count != 0)
            {
                foreach (Match match in matches)
                {
                    var functionSignature = LSLLibraryFunctionSignature.Parse(match.Groups[1].ToString());
                    functionSignature.SetSubsets(_subsets);
                    if (url.ToLower() == SecondlifeWikiBaseUrl + functionSignature.Name.ToLower())
                    {
                        Log.WriteLine("SecondlifeWikiLibraryData: retrieved function {0}; from {1}",
                            functionSignature.SignatureString,
                            url);

                        return functionSignature;
                    }
                }
                return null;
            }
            return null;
        }

        private LSLLibraryEventSignature GetSigFromEventPage(string url)
        {
            var page = _client.DownloadString(url);

            var match = _eventSignature.Regex.Match(page);
            if (match.Success)
            {
                var eventSignature = LSLLibraryEventSignature.Parse(match.Groups[1].ToString());
                eventSignature.SetSubsets(_subsets);

                Log.WriteLine("SecondlifeWikiLibraryData: retrieved event {0}; from {1}",
                    eventSignature.SignatureString, url);

                return eventSignature;
            }
            return null;
        }


        private IEnumerable<string> GetLSLEventPages()
        {
            var currentPage = SecondlifeWikiBaseUrl + "Category:LSL_Events";

            Log.WriteLine("SecondlifeWikiLibraryData: navigating to events page {0}", currentPage);

            var page = _client.DownloadString(currentPage);


            Match match;

            var givenLinks = new HashSet<string>();

            do
            {
                var searchContent = _eventPageCategory.Match(page).Groups[1].ToString();

                foreach (Match linkMatch in _hrefLink.Matches(searchContent))
                {
                    var linkRel = linkMatch.Groups[1].ToString();
                    if (linkRel != "/wiki/Event_Order")
                    {
                        if (!givenLinks.Contains(linkRel))
                        {
                            var pageUrl = SecondlifeWikiDomain + linkRel;

                            Log.WriteLine("SecondlifeWikiLibraryData: discovered events page {0}", pageUrl);

                            yield return pageUrl;
                            givenLinks.Add(linkRel);
                        }
                    }
                }


                match = _eventPageNavigation.Match(page);
                currentPage = SecondlifeWikiDomain + match.Groups[1].ToString().Replace("&amp;", "&");

                Log.WriteLine("SecondlifeWikiLibraryData: navigating to events page {0}", currentPage);


                page = Encoding.UTF8.GetString(_client.DownloadData(currentPage));
            } while (match.Success);
        }

        private IEnumerable<string> GetLSLFunctionPages()
        {
            var currentPage = SecondlifeWikiBaseUrl + "Category:LSL_Functions";
            var givenLinks = new HashSet<string>();

            Log.WriteLine("SecondlifeWikiLibraryData: navigating to functions page {0}", currentPage);
            var page = _client.DownloadString(currentPage);


            Match match = _functionPageNavigation.Match(page);

            while (match.Success)
            {
                var searchContent = _functionPageCategory.Match(page).Groups[1].ToString();

                foreach (Match linkMatch in  _hrefLink.Matches(searchContent))
                {
                    var linkRel = linkMatch.Groups[1].ToString();

                    if (!givenLinks.Contains(linkRel))
                    {
                        var pageUrl = SecondlifeWikiDomain + linkRel;
                        Log.WriteLine("SecondlifeWikiLibraryData: discovered function page {0}", pageUrl);
                        yield return pageUrl;
                        givenLinks.Add(linkRel);
                    }
                }


                match = _functionPageNavigation.Match(page);
                currentPage = SecondlifeWikiDomain + match.Groups[1].ToString().Replace("&amp;", "&");

                Log.WriteLine("SecondlifeWikiLibraryData: navigating to functions page {0}", currentPage);

                page = Encoding.UTF8.GetString(_client.DownloadData(currentPage));
            }
        }


        private IEnumerable<LSLLibraryFunctionSignature> GetLSLFunctions()
        {
            return
                GetLSLFunctionPages()
                    .Select(GetSigFromFunctionPage)
                    .Where(x => x != null)
                    .Union(GetLSLFunctionsFromKeywordsAll());
        }


        private IEnumerable<LSLLibraryConstantSignature> LSLConstantsFromParticlePage()
        {
            string particleConstants = _client.DownloadString(SecondlifeWikiBaseUrl + "LlParticleSystem");

            var matches = _physicsConstantTableRow.Matches(particleConstants);

            foreach (Match match in matches)
            {
                var matchString = match.ToString();
                var valueCase1 = Regex.Match(matchString, "title=\"Hexadecimal notation for: (.*?)\"");
                var valueCase2 = Regex.Match(matchString, "<td align=\"center\">(.*)");
                

                string name = match.Groups[1].ToString();
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = match.Groups[2].ToString();
                }

                if (string.IsNullOrWhiteSpace(valueCase1.Groups[1].ToString()))
                {
                    var constant = new LSLLibraryConstantSignature(LSLType.Integer, name, valueCase2.Groups[1].ToString());
                    constant.SetSubsets(_subsets);
                    yield return constant;
                }
                else
                {
                    var constant = new LSLLibraryConstantSignature(LSLType.Integer, name, valueCase1.Groups[1].ToString());
                    constant.SetSubsets(_subsets);
                    yield return constant;
                }

                
                
            }
        }


        private IEnumerable<LSLLibraryConstantSignature> LSLConstantsFromClickActionPage()
        {
            string particleConstants = _client.DownloadString(SecondlifeWikiBaseUrl + "LlSetClickAction");

            var matches = _clickActionConstants.Matches(particleConstants);


            foreach (Match match in matches)
            {
                var c = new LSLLibraryConstantSignature(LSLType.Integer, match.Groups[2].ToString(), match.Groups[1].ToString());
                c.SetSubsets(_subsets);
                yield return c;
            }
        }

        private IEnumerable<LSLLibraryConstantSignature> LSLConstantsFromChangedEventPage()
        {
            string particleConstants = _client.DownloadString(SecondlifeWikiBaseUrl + "Changed");

            var tableMatch = _changedEventConstantsTable.Match(particleConstants);
            var table = tableMatch.ToString();
            var matches = _changedEventConstants.Matches(table);


            foreach (Match match in matches)
            {
                var s = Convert.ToInt32(match.Groups[2].ToString(), 16);
                var c = new LSLLibraryConstantSignature(LSLType.Integer, match.Groups[1].ToString(),s.ToString(CultureInfo.InvariantCulture));
                c.SetSubsets(_subsets);
                yield return c;
            }
        }


        private IEnumerable<LSLLibraryConstantSignature> LSLConstantsFromKeywordsAll()
        {
            var data = _client.DownloadString(SecondlifeWikiBaseUrl + "Category:LSL_Keywords/All");

            var group = _constantsGroupKeywordsAll.Match(data);
            if (group.Success)
            {
                var groupString = group.ToString();
                foreach (Match constantMatch in _constantInKeywordsAll.Matches(groupString))
                {
                    var type = LSLTypeTools.FromLSLTypeString(constantMatch.Groups[1].ToString());
                    var webVal = constantMatch.Groups[3].ToString();
                    var strVal = "";

                    if (type == LSLType.Integer)
                    {

                        strVal = webVal.Contains("x") ? 
                            Convert.ToInt32(webVal, 16).ToString() : webVal;
                    }

                    if (type == LSLType.Vector || type==LSLType.Rotation)
                    {
                        strVal = webVal.Replace("&lt;", "").Replace("&gt;","");
                    }

                    LSLLibraryConstantSignature result;
                    if (string.IsNullOrWhiteSpace(strVal))
                    {
                        result =
                            new LSLLibraryConstantSignature(
                                LSLTypeTools.FromLSLTypeString(constantMatch.Groups[1].ToString()),
                                constantMatch.Groups[2].ToString());
                    }
                    else
                    {
                        result =
                        new LSLLibraryConstantSignature(
                            LSLTypeTools.FromLSLTypeString(constantMatch.Groups[1].ToString()),
                            constantMatch.Groups[2].ToString(),strVal);
                    }
                    

               

                    result.SetSubsets(_subsets);
                    yield return result;
                }
            }
        }

        private IEnumerable<LSLLibraryEventSignature> GetLSLEvents()
        {
            return GetLSLEventPages().Select(GetSigFromEventPage).Where(x => x != null);
        }


        private IEnumerable<string> GetLSLConstantPages()
        {
            var currentPage = SecondlifeWikiBaseUrl + "Category:LSL_Constants";

            var givenLinks = new HashSet<string>();

            Log.WriteLine("SecondlifeWikiLibraryData: navigating to constants page {0}", currentPage);

            var page = _client.DownloadString(currentPage);

            Match match = _constantPageNavigation.Match(page);

            while (match.Success)
            {
                foreach (Match link in _constantPageLinks.Matches(page))
                {
                    var linkRel = link.Groups[1].ToString();
                    if (!givenLinks.Contains(linkRel))
                    {
                        var pageUrl = "http://wiki.secondlife.com" + linkRel;

                        Log.WriteLine("SecondlifeWikiLibraryData: discovered constant page {0}", pageUrl);
                        yield return pageUrl;

                        givenLinks.Add(linkRel);
                    }
                }


                match = _constantPageNavigation.Match(page);
                currentPage = "http://wiki.secondlife.com" + match.Groups[1].ToString().Replace("&amp;", "&");

                Log.WriteLine("SecondlifeWikiLibraryData: navigating to constants page {0}", currentPage);

                page = Encoding.UTF8.GetString(_client.DownloadData(currentPage));
            }
        }


        private IEnumerable<LSLLibraryFunctionSignature> GetLSLFunctionsFromKeywordsAll()
        {
            var data = _client.DownloadString(SecondlifeWikiBaseUrl + "Category:LSL_Keywords/All");

            var group = _functionsGroupInKeywordsAll.Match(data);
            if (group.Success)
            {
                foreach (Match functionMatch in _functionInKeywordsAll.Matches(group.ToString()))
                {
                    string sig = functionMatch.Groups[1] + " " + functionMatch.Groups[2] + functionMatch.Groups[3];


                    var result = LSLLibraryFunctionSignature.Parse(sig);
                    result.SetSubsets(_subsets);
                    yield return result;
                }
            }
        }


        public IEnumerable<LSLLibraryConstantSignature> GetLSLConstants()
        {

            return
                GetLSLConstantPages()
                    .Select(GetSigFromConstantPage)
                    .Where(x => x != null)
                    .Union(LSLConstantsFromKeywordsAll())
                    .Union(LSLConstantsFromParticlePage())
                    .Union(LSLConstantsFromClickActionPage())
                    .Union(LSLConstantsFromChangedEventPage());
        }
    }
}