#region FileInfo
// 
// File: ArgSwitchDesc.cs
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace lslcc
{
    internal sealed class ArgSwitchDesc
    {
        private readonly string _name;
        private Collection<string> _descriptionLines = new Collection<string>();
        private HashSet<string> _cantBeUsedWith = new HashSet<string>();

        public string Name
        {
            get { return _name; }
        }

        public HashSet<string> CantBeUsedWith
        {
            get { return _cantBeUsedWith; }
            set { _cantBeUsedWith = value; }
        }


        public bool MustBeUsedAlone { get; set; }

        public bool MustAppearOnlyOnce { get; set; }

        public int MinArgs { get; set; }
        public int MaxArgs { get; set; }

        public string HelpLine { get; set; }

        public Collection<string> DescriptionLines
        {
            get { return _descriptionLines; }
            set { _descriptionLines = value; }
        }

        public event EventHandler<ArgSwitchHelpWriteEvent> WriteBeforeShortHelp;


        public ArgSwitchDesc AddWriteBeforeShortHelp(EventHandler<ArgSwitchHelpWriteEvent> handler)
        {
            WriteBeforeShortHelp += handler;
            return this;
        }


        public event EventHandler<ArgSwitchHelpWriteEvent> WriteAfterShortHelp;

        public ArgSwitchDesc AddWriteAfterShortHelp(EventHandler<ArgSwitchHelpWriteEvent> handler)
        {
            WriteAfterShortHelp += handler;
            return this;
        }

        public event EventHandler<ArgSwitchHelpWriteEvent> WriteBeforeDescription;

        public ArgSwitchDesc AddWriteBeforeDescription(EventHandler<ArgSwitchHelpWriteEvent> handler)
        {
            WriteBeforeDescription += handler;
            return this;
        }

        public event EventHandler<ArgSwitchHelpWriteEvent> WriteAfterDescription;

        public ArgSwitchDesc AddWriteAfterDescription(EventHandler<ArgSwitchHelpWriteEvent> handler)
        {
            WriteAfterDescription += handler;
            return this;
        }

        public ArgSwitchDesc(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name cannot be null or whitespace.", "name");
            }


            _name = name;
            MustAppearOnlyOnce = true;
            MinArgs = -1;
            MaxArgs = -1;
        }

        
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }


        public override bool Equals(object obj)
        {
            var o = obj as ArgSwitchDesc;
            if (o == null) return false;

            return o.Name == Name;
        }


        internal void OnWriteBeforeShortHelp(ArgSwitchHelpWriteEvent e)
        {
            var handler = WriteBeforeShortHelp;
            if (handler != null) handler(this, e);
        }


        internal void OnWriteAfterShortHelp(ArgSwitchHelpWriteEvent e)
        {
            var handler = WriteAfterShortHelp;
            if (handler != null) handler(this, e);
        }


        internal void OnWriteBeforeDescription(ArgSwitchHelpWriteEvent e)
        {
            var handler = WriteBeforeDescription;
            if (handler != null) handler(this, e);
        }


        internal void OnWriteAfterDescription(ArgSwitchHelpWriteEvent e)
        {
            var handler = WriteAfterDescription;
            if (handler != null) handler(this, e);
        }
    }
}