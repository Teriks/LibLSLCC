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
    ///     LibLSLCC.CodeValidator.Components.LibraryDataProvider.LSLDefaultLibraryDataProvider.xml
    ///     to define its data
    /// </summary>
    public class LSLDefaultLibraryDataProvider : LSLXmlLibraryDataProvider
    {
        private LSLLibraryBaseData _liveFilteringBaseLibraryData;
        private LSLLibraryDataAdditions _liveFilteringLibraryDataAdditions;



        /// <summary>
        /// Constructs an LSLDefaultLibraryDataProvider using the embedded LSLDefaultLibraryDataProvider.xml file.
        /// </summary>
        /// <param name="liveFiltering">
        /// If this is set to true, all subsets will be loaded into memory. And when you change the active subsets query results will change.
        /// Otherwise if this is false, only subsets present upon construction will be loaded.
        /// </param>
        /// <param name="libraryBaseData">The base library data to use.</param>
        /// <param name="dataAdditions">Library data additions.</param>
        /// <exception cref="InvalidOperationException">If the embedded library data could not be loaded from the assembly manifest.</exception>
        public LSLDefaultLibraryDataProvider(bool liveFiltering, LSLLibraryBaseData libraryBaseData,
            LSLLibraryDataAdditions dataAdditions = LSLLibraryDataAdditions.None) : this(GetSubsets(libraryBaseData,dataAdditions),liveFiltering)
        {

        }


        /// <summary>
        /// Constructs an LSLDefaultLibraryDataProvider using the embedded LSLDefaultLibraryDataProvider.xml file.
        /// </summary>
        /// <param name="activeSubsets">The library subsets to utilize.</param>
        /// <param name="liveFiltering">
        /// If this is set to true, all subsets will be loaded into memory. And when you change the active subsets query results will change.
        /// Otherwise if this is false, only subsets present upon construction will be loaded.
        /// </param>
        /// <exception cref="InvalidOperationException">If the embedded library data could not be loaded from the assembly manifest.</exception>
        public LSLDefaultLibraryDataProvider(IEnumerable<string> activeSubsets, bool liveFiltering = true) : base(activeSubsets, liveFiltering)
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
                        "Could not locate manifest resource LibLSLCC.CodeValidator.Components.LibraryData.LSLDefaultLibraryDataProvider.xml");
                }

                var reader = new XmlTextReader(libraryData);

                FillFromXml(reader);
            }
        }

        /// <summary>
        /// Constructs an LSLDefaultLibraryDataProvider using the embedded LSLDefaultLibraryDataProvider.xml file in live filtering mode with no active subsets set.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the embedded library data could not be loaded from the assembly manifest.</exception>
        public LSLDefaultLibraryDataProvider() : base(true)
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
                        "Could not locate manifest resource LibLSLCC.CodeValidator.Components.LibraryData.LSLDefaultLibraryDataProvider.xml");
                }

                var reader = new XmlTextReader(libraryData);

                FillFromXml(reader);
            }
        }



        /// <summary>
        /// Easy way to add either 'lsl' or 'os-lsl' to the active subsets
        /// </summary>
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

        /// <summary>
        /// Easy way to add extra modules to the active subsets
        /// </summary>
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

            foreach (var name in dataAdditions.ToSubsetNames())
            {
                yield return name;
            }
        }
    }


    /// <summary>
    /// Represents the available additionaly library subsets in LSLDefaultLibraryDataProvider.xml
    /// </summary>
    [Flags]
    public enum LSLLibraryDataAdditions
    {
        /// <summary>
        /// Specifies no extra library data
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies the addition of the OpenSim OSSL functions
        /// </summary>
        OpenSimOssl = 1,

        /// <summary>
        /// Specifies the addition of the OpenSim light share functions
        /// </summary>
        OpenSimWindlight = 2,

        /// <summary>
        /// Specifies the addition of the OpenSim bullet phys* functions
        /// </summary>
        OpenSimBulletPhysics = 4,

        /// <summary>
        /// Specifies the addition of the OpenSim mod invoke functions
        /// </summary>
        OpenSimModInvoke = 8,

        /// <summary>
        /// Specifies the addition of the OpenSim json store functions
        /// </summary>
        OpenSimJsonStore = 16
    }


    /// <summary>
    /// Extensions for the LSLLibraryDataAdditions flags Enum
    /// </summary>
    public static class LSLLibraryDataAdditionEnumExtensions
    {

        /// <summary>
        /// Converts LSLLibraryDataAdditions to the corresponding subset string
        /// </summary>
        /// <param name="flags"></param>
        /// <returns>a string representation of the subset</returns>
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


    /// <summary>
    /// Represents the available base library subsets in LSLDefaultLibraryDataProvider.xml
    /// </summary>
    public enum LSLLibraryBaseData
    {
        /// <summary>
        /// Standard LSL
        /// </summary>
        StandardLsl,

        /// <summary>
        /// OpenSim subset of Standard LSL
        /// </summary>
        OpensimLsl
    }


    /// <summary>
    /// Extensions for the LSLLibraryBaseDataEnumExtensions Enum
    /// </summary>
    public static class LSLLibraryBaseDataEnumExtensions
    {
        /// <summary>
        /// Converts LSLLibraryBaseData to the corresponding subset string
        /// </summary>
        /// <param name="flag"></param>
        /// <returns>a string representation of the subset</returns>
        public static string ToSubsetName(this LSLLibraryBaseData flag)
        {
            switch (flag)
            {
                case LSLLibraryBaseData.StandardLsl:
                    return "lsl";
                case LSLLibraryBaseData.OpensimLsl:
                    return "os-lsl";
                default:
                    throw new ArgumentOutOfRangeException("flag", flag, null);
            }
        }
    }
}