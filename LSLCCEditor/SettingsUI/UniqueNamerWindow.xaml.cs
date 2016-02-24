#region FileInfo
// 
// File: UniqueNamerWindow.xaml.cs
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
using System.Windows;
using LSLCCEditor.Styles;

namespace LSLCCEditor.SettingsUI
{
    /// <summary>
    /// Interaction logic for UniqueNamerWindow.xaml
    /// </summary>
    public partial class UniqueNamerWindow : Window
    {
        private readonly string _startingName;
        private readonly bool _generateNumericSuffix;
        private readonly HashSet<string> _takenNames;


        public static readonly DependencyProperty ChosenNameProperty = DependencyProperty.Register(
            "ChosenName", typeof (string), typeof (UniqueNamerWindow), new PropertyMetadata(default(string), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            UniqueNamerWindow window = (UniqueNamerWindow) dependencyObject;
            string val = (string)dependencyPropertyChangedEventArgs.NewValue;
            window.OkButton.IsEnabled = true;
            if (string.IsNullOrWhiteSpace(val))
            {
                window.OkButton.IsEnabled = false;
                throw new Exception("Must provide a name.");
            }

            if (window._takenNames.Contains(val) && !(window._generateNumericSuffix==false && window._startingName == val))
            {
                window.OkButton.IsEnabled = false;
                throw new Exception("That name is taken already.");
            }
        }

        public string ChosenName
        {
            get { return (string) GetValue(ChosenNameProperty); }
            set { SetValue(ChosenNameProperty, value); }
        }


        public UniqueNamerWindow(IEnumerable<string> takenNames, string startingName, bool generateNumericSuffix = true)
        {
            _startingName = startingName;
            _generateNumericSuffix = generateNumericSuffix;
            _takenNames = new HashSet<string>(takenNames);

            InitializeComponent();
            MetroWindowStyleInit.Init(this);

            Canceled = true;

            if (_generateNumericSuffix)
            {

                var name = _startingName;
                int num = 1;

                while (_takenNames.Contains(name))
                {
                    name = _startingName + " " + num;
                    num++;
                }

                ChosenName = name;
            }
            else
            {
                ChosenName = _startingName;
            }
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            Canceled = false;
            Close();
        }

        public bool Canceled { get; private set; }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            Canceled = true;
            Close();
        }
    }
}
