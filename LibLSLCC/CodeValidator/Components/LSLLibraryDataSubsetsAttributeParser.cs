using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace LibLSLCC.CodeValidator.Components
{

        [Serializable]
        public class LSLLibraryDataSubsetsAttributeRegex : Regex
        {
            public LSLLibraryDataSubsetsAttributeRegex()
                : base(@"(?:([a-zA-Z]+[a-zA-Z_0-9\-]*)(?:\s*,\s*([a-zA-Z]+[a-zA-Z_0-9\-]*))*)?")
            {
                    
            }

           protected LSLLibraryDataSubsetsAttributeRegex(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                
            }

            public IEnumerable<string> ParseSubsets(string parse)
            {
                 MatchCollection matches = Matches(parse);
               

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