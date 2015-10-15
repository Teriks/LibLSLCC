#region FileInfo
// 
// File: LSLDefaultLibraryDataProvider.cs
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
using System.Xml;
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    ///     The LSLDefaultLibraryDataProvider reads XML from the embedded resource
    ///     LibLSLCC.CodeValidator.Components.LibraryData.StandardLSL.xml
    ///     to define its data
    /// </summary>
    [LSLXmlLibraryDataRoot]
    public class LSLDefaultLibraryDataProvider : LSLXmlLibraryDataProvider
    {
        private LSLLibraryBaseData _liveFilteringBaseLibraryData;
        private LSLLibraryDataAdditions _liveFilteringLibraryDataAdditions;



        public LSLDefaultLibraryDataProvider(bool liveFiltering, LSLLibraryBaseData libraryBaseData,
            LSLLibraryDataAdditions dataAdditions = LSLLibraryDataAdditions.None) : base(GetSubsets(libraryBaseData,dataAdditions),liveFiltering)
        {
            using (
                var libraryData =
                    GetType()
                        .Assembly.GetManifestResourceStream(
                            "LibLSLCC.CodeValidator.Components.LibraryData.LSLDefaultLibraryDataProvider.xml"))
            {
                if (libraryData == null)
                {
                    throw new InvalidOperationException(
                        "Could not locate manifest resource LibLSLCC.CodeValidator.Components.Resources.StandardLSLLibraryData.xml");
                }

                var reader = new XmlTextReader(libraryData);

                FillFromXml(reader);
            }
        }

        public LSLLibraryBaseData LiveFilteringBaseLibraryData
        {
            get { return _liveFilteringBaseLibraryData; }
            set
            {
                if (value == _liveFilteringBaseLibraryData) return;

                ActiveSubsets.SetSubsets(GetSubsets(value, LiveFilteringLibraryDataAdditions));

                _liveFilteringBaseLibraryData = value;
            }
        }

        public LSLLibraryDataAdditions LiveFilteringLibraryDataAdditions
        {
            get { return _liveFilteringLibraryDataAdditions; }
            set
            {
                if (value == _liveFilteringLibraryDataAdditions) return;

                ActiveSubsets.SetSubsets(GetSubsets(LiveFilteringBaseLibraryData, value));

                _liveFilteringLibraryDataAdditions = value;
            }
        }

        private static IEnumerable<string> GetSubsets(LSLLibraryBaseData libraryBaseData, LSLLibraryDataAdditions dataAdditions)
        {
            yield return libraryBaseData.ToSubsetName();

            if(libraryBaseData == LSLLibraryBaseData.All) yield break;

            foreach (var name in dataAdditions.ToSubsetNames())
            {
                yield return name;
            }
        }
    }

    [Flags]
    public enum LSLLibraryDataAdditions
    {
        None = 0,
        OpenSimOssl = 1,
        OpenSimWindlight = 2,
        OpenSimBulletPhysics = 4,
        OpenSimModInvoke = 8,
        OpenSimJsonStore = 16
    }


    public static class LSLLibraryDataAdditionEnumExtensions
    {
        public static IEnumerable<string> ToSubsetNames(this LSLLibraryDataAdditions flags)
        {
            if ((flags & LSLLibraryDataAdditions.OpenSimOssl) == LSLLibraryDataAdditions.OpenSimOssl)
            {
                yield return ("ossl");
            }

            if ((flags & LSLLibraryDataAdditions.OpenSimWindlight) == LSLLibraryDataAdditions.OpenSimWindlight)
            {
                yield return ("os-lightshare");
            }

            if ((flags & LSLLibraryDataAdditions.OpenSimBulletPhysics) == LSLLibraryDataAdditions.OpenSimBulletPhysics)
            {
                yield return ("os-bullet-physics");
            }

            if ((flags & LSLLibraryDataAdditions.OpenSimModInvoke) == LSLLibraryDataAdditions.OpenSimModInvoke)
            {
                yield return ("os-mod-api");
            }


            if ((flags & LSLLibraryDataAdditions.OpenSimJsonStore) == LSLLibraryDataAdditions.OpenSimJsonStore)
            {
                yield return ("os-json-store");
            }
        } 
    }

    public enum LSLLibraryBaseData
    {
        StandardLsl,
        OpensimLsl,
        All
    }

    public static class LSLLibraryBaseDataEnumExtensions
    {
        public static string ToSubsetName(this LSLLibraryBaseData flag)
        {
            switch (flag)
            {
                case LSLLibraryBaseData.StandardLsl:
                    return "lsl";
                case LSLLibraryBaseData.OpensimLsl:
                    return "os-lsl";
                case LSLLibraryBaseData.All:
                    return "all";
                default:
                    throw new ArgumentOutOfRangeException("flag", flag, null);
            }
        }
    }
}