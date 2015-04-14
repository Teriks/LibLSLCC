using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LibLSLCC.CodeValidator.Components
{


        public class LSLLibraryDataSubsetsAttributeRegex : Regex
        {
            public LSLLibraryDataSubsetsAttributeRegex()
                : base(@"(?:([a-zA-Z]+[a-zA-Z_0-9\-]*)(?:\s*,\s*([a-zA-Z]+[a-zA-Z_0-9\-]*))*)?")
            {
                    
            }

            public IEnumerable<string> ParseSubsets(string parse)
            {
                 MatchCollection matches = this.Matches(parse);
               

                if (matches.Count > 1 && matches[0].Groups.Count > 1)
                {
                    int i = 1;
                    for (; i < matches[0].Groups.Count; i++)
                    {
                        var str = matches[0].Groups[i].ToString();
                        if (!String.IsNullOrWhiteSpace(str))
                        {
                            yield return str;
                        }
                    }
                }
            }
        }
    
}