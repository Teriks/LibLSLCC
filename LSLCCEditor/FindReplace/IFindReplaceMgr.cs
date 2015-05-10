﻿using System.Collections;

namespace FindReplace
{
    /// <summary>
    /// The design is like this:
    ///    (WPF wrapper) -- this interface -- (FindReplaceEngine)
    /// </summary>
    interface IFindReplaceMgr
    {
        bool CaseSensitive { get; set; }
        object CurrentEditor { get; set; }
        IEnumerable Editors { get; set; }
        string ReplacementText { get; set; }
        FindReplaceMgr.SearchScope SearchIn { get; set; }
        bool SearchUp { get; set; }
        string TextToFind { get; set; }
        bool UseRegEx { get; set; }
        bool UseWildcards { get; set; }
        bool WholeWord { get; set; }

        IEditor ConvertEditor(object o);
    }
}
