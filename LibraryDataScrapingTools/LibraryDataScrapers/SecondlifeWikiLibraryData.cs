#region FileInfo
// 
// File: SecondlifeWikiLibraryData.cs
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;
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
            new Regex(
                "<a href=\"/wiki/[A-Z_]*?\" title=\".*?\">([A-Z_]*?)</a>(?:\\n|\\r|.)*?title=\"Hexadecimal notation for:.*?\" style=\"border-bottom:1px dotted; cursor:help;\">(.*?)</span>");

        private readonly Regex _changedEventConstantsTable =
            new Regex("<tr bgcolor=\"#A7C1F2\">(\\n|\\r|.)*?</td></tr></table>");

        private readonly Regex _clickActionConstants =
            new Regex("<span title=\"integer [A-Z_]*? = (.*?);.*?\">([A-Z_]*?)</span>");

        private readonly Regex _constantInKeywordsAll =
            new Regex(
                "(integer|float|string|vector|rotation|list|key)\\s+<a\\s+href\\s*=\\s*\".*?\"\\s+title=\".*?\">(.*?)</a> = (.*)");

        private readonly Regex _constantPageLinks =
            new Regex("<li>\\s*<a\\s+href\\s*=\\s*\"(/wiki/.*?)\"\\s+title\\s*=\\s*\".*?\"\\s*>.*?</a>\\s*</li>");

        private readonly Regex _constantPageNavigation =
            new Regex(
                "(?:\\(previous 200\\) |previous 200</a>\\) )\\(<a\\s+href=\"(.*?)\"\\s+title\\s*=\\s*\"Category:LSL Constants\">next 200</a>\\)<div");

        private readonly HashMap<string, LSLLibraryConstantSignature> _constants =
            new HashMap<string, LSLLibraryConstantSignature>();

        private readonly Regex _constantsGroupKeywordsAll =
            new Regex("<a\\s+name\\s*=\\s*\"Constants\"\\s+id\\s*=\\s*\"Constants\"></a><h2>((?:\\r|\\n|.))*?</pre>");

        private readonly Regex _constantSignature =
            new Regex(
                "Constant: <a href=\"/wiki/.*?\" title=\"(.*?)\" class=\"mw-redirect\">(?:integer|float|vector|string|list|key|quaternion)</a> <strong class=\"selflink\"><span title=\".*?\">(.*?)</span></strong>\\s*=\\s*(.*?); </span>");

        private readonly Regex _deprecatedMarker  = new Regex("<td style=\"color:white;background:#990000; border-width:1px;\" title=\".*?\" width=\"100%\"> <b>Deprecated");


        private readonly Regex _eventPageNavigation =
            new Regex(
                "(?:\\(previous 200\\) |previous 200</a>\\) )\\(<a\\s+href=\"(.*?)\"\\s+title\\s*=\\s*\"Category:LSL Events\">next 200</a>\\)<div");

        private readonly HashMap<string, LSLLibraryEventSignature> _events =
            new HashMap<string, LSLLibraryEventSignature>();

        private readonly LSLEventSignatureRegex _eventSignature =
            new LSLEventSignatureRegex("<pre id=\"lsl-signature\">\\s*event\\s+void\\s+(", ";)");

        private readonly Regex _functionInKeywordsAll =
            new Regex(
                "(integer|float|string|vector|rotation|list|key|void)\\s+<a\\s+href\\s*=\\s*\".*?\"\\s+title=\".*?\">(.*?)</a>((?:.*?)\\))");


        private readonly Regex _functionPageNavigation =
            new Regex(
                "(?:\\(previous 200\\) |previous 200</a>\\) )\\(<a\\s+href=\"(.*?)\"\\s+title\\s*=\\s*\"Category:LSL Functions\">next 200</a>\\)<div");

        private readonly HashMap<string, GenericArray<LSLLibraryFunctionSignature>> _functions =
            new HashMap<string, GenericArray<LSLLibraryFunctionSignature>>();

        private readonly Regex _functionsGroupInKeywordsAll =
            new Regex("<a\\s+name\\s*=\\s*\"Functions\"\\s+id\\s*=\\s*\"Functions\"></a><h2>((?:\\r|\\n|.))*?</pre>");

        private readonly LSLFunctionSignatureRegex _functionSignature = new LSLFunctionSignatureRegex("function\\s+(",
            ";)");

        private readonly Regex _hrefLink = new Regex("href=\"(/wiki/.*?)\"");
        private readonly Regex _matchConstantllUnescapeUrl = new Regex(@"llUnescapeUrl\((.*?)\)");
        private readonly Regex _mwPagesContent = new Regex("<div id=\"mw-pages\">(.*?)</table>", RegexOptions.Singleline);

        private readonly Regex _physicsConstantTableRow =
            new Regex(
                "(?:(?:<tr\\s+id\\s*=\\s*\"([A-Z_]*?)\">)|(?:<tr\\s+style\\s*=\\s*\"background-color:.*?;\"\\s+id\\s*=\\s*\"([A-Z_]*?)\">))(?:\\n|\\r|.)*?</tr>");

        private readonly IEnumerable<string> _subsets;

        private readonly CachedWebDownloader _client;

        public static readonly string WebCacheFileDirectory;

        static SecondlifeWikiLibraryData()
        {
            var appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appDirectory != null)
            {
                WebCacheFileDirectory = Path.Combine(appDirectory, "SecondlifeWikiLibraryDataCache");
            }
            else
            {
                throw new Exception("Could not find the directory of the executing assembly in order to store Cache Data for type (SecondlifeWikiLibraryData.");
            }
        }

        public SecondlifeWikiLibraryData(IDocumentationProvider documentationProvider, IEnumerable<string> subsets)
        {
            
            _client = new CachedWebDownloader(WebCacheFileDirectory);

            var documentationProvider1 = documentationProvider;
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
                        new GenericArray<LSLLibraryFunctionSignature> {lslLibraryFunctionSignature});
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

        public IReadOnlyGenericArray<LSLLibraryFunctionSignature> LSLFunctionOverloads(string name)
        {
            if (_functions.ContainsKey(name))
            {
                return _functions[name];
            }
            return new GenericArray<LSLLibraryFunctionSignature>();
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

        public IEnumerable<IReadOnlyGenericArray<LSLLibraryFunctionSignature>> LSLFunctionOverloadGroups()
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

            var hexNotation = new Regex("<span title=\"Hexadecimal notation for: (.*?)\"");

            var match = _constantSignature.Match(page);

            if (!match.Success) return null;


            var val = match.Groups[3].ToString();
            string strValue;


            var type = LSLTypeTools.FromLSLTypeString(match.Groups[1].ToString());


            if (type == LSLType.Integer || type == LSLType.Float)
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
            else if (type == LSLType.Rotation || type == LSLType.Vector)
            {
                strValue = val.Replace("&lt;", "").
                               Replace("&gt;", "").
                               Replace("<","").
                               Replace(">","");
            }
            else if (type == LSLType.String || type == LSLType.Key)
            {
                strValue = val.Replace("&quot;", "").
                               Replace("\"","");

                var specialUnicode = _matchConstantllUnescapeUrl.Match(strValue);
                if (specialUnicode.Success)
                {
                    strValue = HttpUtility.UrlDecode(specialUnicode.Groups[1].ToString());
                }
            }
            else
            {
                strValue = val;
            }


            var constantSignature =
                new LSLLibraryConstantSignature(type,
                    match.Groups[2].ToString(), strValue) {Deprecated = _deprecatedMarker.IsMatch(page)};




            constantSignature.SetSubsets(_subsets);

            Log.WriteLineWithHeader(
                "[SecondlifeWikiLibraryData]: ", "Retrieved" + (constantSignature.Deprecated ? " (DEPRECATED) " : "") +
                "constant {0}; from {1}",
                constantSignature.SignatureString, url);

            return constantSignature;
        }

        private LSLLibraryFunctionSignature GetSigFromFunctionPage(string url)
        {
            var page = _client.DownloadString(url);

            var matches = _functionSignature.Regex.Matches(page);
            if (matches.Count == 0) return null;


            foreach (Match match in matches)
            {
                var functionSignature = LSLLibraryFunctionSignature.Parse(match.Groups[1].ToString());


                functionSignature.Deprecated = _deprecatedMarker.IsMatch(page);

                functionSignature.SetSubsets(_subsets);


                if (url.ToLower() != SecondlifeWikiBaseUrl + functionSignature.Name.ToLower()) continue;


                Log.WriteLineWithHeader(
                "[SecondlifeWikiLibraryData]: ", "Retrieved" + (functionSignature.Deprecated ? " (DEPRECATED) " : "") +
                    "function {0}; from {1}",
                    functionSignature.SignatureString,
                    url);

                return functionSignature;
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

                Log.WriteLineWithHeader(
                "[SecondlifeWikiLibraryData]: ", "Retrieved event {0}; from {1}",
                    eventSignature.SignatureString, url);

                return eventSignature;
            }
            return null;
        }

        private IEnumerable<string> GetLSLEventPages()
        {
            var currentPage = SecondlifeWikiBaseUrl + "Category:LSL_Events";

            Log.WriteLineWithHeader(
                "[SecondlifeWikiLibraryData]: ", "Navigating to events page {0}", currentPage);

            var page = _client.DownloadString(currentPage);


            Match match;

            var givenLinks = new HashSet<string>();

            do
            {
                var searchContent = _mwPagesContent.Match(page).Value;

                foreach (Match linkMatch in _hrefLink.Matches(searchContent))
                {
                    var linkRel = linkMatch.Groups[1].ToString();
                    if (linkRel != "/wiki/Event_Order")
                    {
                        if (!givenLinks.Contains(linkRel))
                        {
                            var pageUrl = SecondlifeWikiDomain + linkRel;

                            Log.WriteLineWithHeader("[SecondlifeWikiLibraryData]: ", "Discovered events page {0}", pageUrl);

                            yield return pageUrl;
                            givenLinks.Add(linkRel);
                        }
                    }
                }


                match = _eventPageNavigation.Match(page);
                currentPage = SecondlifeWikiDomain + match.Groups[1].ToString().Replace("&amp;", "&");

                if (currentPage.Trim() == SecondlifeWikiDomain) break;

                Log.WriteLineWithHeader("[SecondlifeWikiLibraryData]: ", "Navigating to events page {0}", currentPage);

                page = Encoding.UTF8.GetString(_client.DownloadData(currentPage));

            } while (match.Success);
        }

        private IEnumerable<string> GetLSLFunctionPages()
        {
            var currentPage = SecondlifeWikiBaseUrl + "Category:LSL_Functions";
            var givenLinks = new HashSet<string>();

            Log.WriteLineWithHeader("[SecondlifeWikiLibraryData]: ", "Navigating to functions page {0}", currentPage);
            var page = _client.DownloadString(currentPage);


            var match = _functionPageNavigation.Match(page);

            while (match.Success)
            {
                var searchContent = _mwPagesContent.Match(page);

                foreach (Match linkMatch in  _hrefLink.Matches(searchContent.Value))
                {
                    var linkRel = linkMatch.Groups[1].ToString();

                    if (!givenLinks.Contains(linkRel))
                    {
                        var pageUrl = SecondlifeWikiDomain + linkRel;
                        Log.WriteLineWithHeader("[SecondlifeWikiLibraryData]: ", "Discovered function page {0}", pageUrl);
                        yield return pageUrl;
                        givenLinks.Add(linkRel);
                    }
                }


                match = _functionPageNavigation.Match(page);
                currentPage = SecondlifeWikiDomain + match.Groups[1].ToString().Replace("&amp;", "&");

                if (currentPage.Trim() == SecondlifeWikiDomain) break;

                Log.WriteLineWithHeader("[SecondlifeWikiLibraryData]: ", "Navigating to functions page {0}", currentPage);

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
            var particleConstants = _client.DownloadString(SecondlifeWikiBaseUrl + "LlParticleSystem");

            var matches = _physicsConstantTableRow.Matches(particleConstants);

            foreach (Match match in matches)
            {
                var matchString = match.ToString();
                var valueCase1 = Regex.Match(matchString, "title=\"Hexadecimal notation for: (.*?)\"");
                var valueCase2 = Regex.Match(matchString, "<td align=\"center\">(.*)");


                var name = match.Groups[1].ToString();
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = match.Groups[2].ToString();
                }

                if (string.IsNullOrWhiteSpace(valueCase1.Groups[1].ToString()))
                {
                    var constant = new LSLLibraryConstantSignature(LSLType.Integer, name,
                        valueCase2.Groups[1].ToString());
                    constant.SetSubsets(_subsets);
                    yield return constant;
                }
                else
                {
                    var constant = new LSLLibraryConstantSignature(LSLType.Integer, name,
                        valueCase1.Groups[1].ToString());
                    constant.SetSubsets(_subsets);
                    yield return constant;
                }
            }
        }

        private IEnumerable<LSLLibraryConstantSignature> LSLConstantsFromClickActionPage()
        {
            var particleConstants = _client.DownloadString(SecondlifeWikiBaseUrl + "LlSetClickAction");

            var matches = _clickActionConstants.Matches(particleConstants);


            foreach (Match match in matches)
            {
                var c = new LSLLibraryConstantSignature(LSLType.Integer, match.Groups[2].ToString(),
                    match.Groups[1].ToString());
                c.SetSubsets(_subsets);
                yield return c;
            }
        }

        private IEnumerable<LSLLibraryConstantSignature> LSLConstantsFromChangedEventPage()
        {
            var particleConstants = _client.DownloadString(SecondlifeWikiBaseUrl + "Changed");

            var tableMatch = _changedEventConstantsTable.Match(particleConstants);
            var table = tableMatch.ToString();
            var matches = _changedEventConstants.Matches(table);


            foreach (Match match in matches)
            {
                var s = Convert.ToInt32(match.Groups[2].ToString(), 16);
                var c = new LSLLibraryConstantSignature(LSLType.Integer, match.Groups[1].ToString(),
                    s.ToString(CultureInfo.InvariantCulture));
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
                        strVal = webVal.Contains("x")
                            ? Convert.ToInt32(webVal, 16).ToString()
                            : webVal;
                    }

                    if (type == LSLType.Vector || type == LSLType.Rotation)
                    {
                        strVal = webVal.Replace("&lt;", "").Replace("&gt;", "");
                    }

                    if (type == LSLType.String || type == LSLType.Key)
                    {
                        strVal = webVal.Replace("&quot;", "").Replace("\"","");
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
                                constantMatch.Groups[2].ToString(), strVal);
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

            Log.WriteLineWithHeader("[SecondlifeWikiLibraryData]: ", "Navigating to constants page {0}", currentPage);

            var page = _client.DownloadString(currentPage);

            var match = _constantPageNavigation.Match(page);

            while (match.Success)
            {
                var content = _mwPagesContent.Match(page);


                foreach (Match link in _constantPageLinks.Matches(content.Value))
                {
                    var linkRel = link.Groups[1].ToString();
                    if (!givenLinks.Contains(linkRel))
                    {
                        var pageUrl = SecondlifeWikiDomain + linkRel;

                        Log.WriteLineWithHeader("[SecondlifeWikiLibraryData]: ", "Discovered constant page {0}", pageUrl);
                        yield return pageUrl;

                        givenLinks.Add(linkRel);
                    }
                }


                match = _constantPageNavigation.Match(page);
                currentPage = SecondlifeWikiDomain + match.Groups[1].ToString().Replace("&amp;", "&");

                if (currentPage.Trim() == SecondlifeWikiDomain) break;

                Log.WriteLineWithHeader("[SecondlifeWikiLibraryData]: ", "Navigating to constants page {0}", currentPage);

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
                    var sig = functionMatch.Groups[1] + " " + functionMatch.Groups[2] + functionMatch.Groups[3];


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
                    .Union(LSLConstantsFromChangedEventPage()).Select(x =>
                    {

                        x.Name = x.Name.Replace(' ', '_');
                        return x;
                    });
        }
    }
}