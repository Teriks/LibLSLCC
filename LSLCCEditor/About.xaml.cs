#region FileInfo
// 
// File: About.xaml.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2016, Teriks
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
using System.IO;
using System.Reflection;
using System.Windows;

namespace LSLCCEditor
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public static readonly DependencyProperty NameAndVersionProperty = DependencyProperty.Register("NameAndVersion", typeof(string), typeof(About), new PropertyMetadata(default(string)));

        public string NameAndVersion
        {
            get { return (string)GetValue(NameAndVersionProperty); }
            set { SetValue(NameAndVersionProperty, value); }
        }

        public static readonly DependencyProperty CopyrightProperty = DependencyProperty.Register(
            "Copyright", typeof (string), typeof (About), new PropertyMetadata(default(string)));

        public string Copyright
        {
            get { return (string) GetValue(CopyrightProperty); }
            set { SetValue(CopyrightProperty, value); }
        }

        public About()
        {
            var callingAssembly = Assembly.GetCallingAssembly();


            InitializeComponent();
            NameAndVersion = "LSLCCEditor v" + callingAssembly.GetName().Version;


            var attributes = callingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

            if (attributes.Length > 0)
            {
                var attribute = attributes[0] as AssemblyCopyrightAttribute;

                Copyright = attribute.Copyright;
            }

            foreach (var assy in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = assy.GetName();

                if(name.Name == "Microsoft.GeneratedCode") continue;

                LoadedAssembliesBox.Items.Add(name.Name + " v"+name.Version);
            }

            try
            {
                using (var reader = File.OpenRead("license.rtf"))
                {
                    LicenseText.SelectAll();
                    LicenseText.Selection.Load(reader, DataFormats.Rtf);
                }
            }
            catch
            {
                //
            }
        }
    }
}
