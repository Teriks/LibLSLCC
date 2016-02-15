#region FileInfo
// 
// File: TextboxHint.cs
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

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LSLCCEditor.Utility.Wpf
{
    public static class TextboxHint
    {
        public static readonly DependencyProperty Hint = DependencyProperty.RegisterAttached(
            "Hint",
            typeof (object),
            typeof (TextboxHint),
            new FrameworkPropertyMetadata(null, OnHintChanged));


        public static object GetHint(DependencyObject d)
        {
            return d.GetValue(Hint);
        }


        public static void SetHint(DependencyObject d, object value)
        {
            d.SetValue(Hint, value);
        }


        private static void OnHintChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;
            if (textBox == null) return;


            textBox.GotKeyboardFocus += Textbox_GotKeyboardFocus;
            textBox.Loaded += Textbox_Loaded;
            textBox.LostKeyboardFocus += Textbox_Loaded;
            textBox.TextChanged += TextBox_OnTextChanged;
        }

        private static void TextBox_OnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var control = (Control) sender;
            if (!ShouldShowHint(control))
            {
                RemoveHint(control);
            }
        }

        private static void Textbox_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            var control = (Control) sender;
            if (ShouldShowHint(control))
            {
                RemoveHint(control);
            }
        }


        private static void Textbox_Loaded(object sender, RoutedEventArgs e)
        {
            var control = (Control) sender;
            if (ShouldShowHint(control))
            {
                ShowHint(control);
            }
        }


        private static void RemoveHint(UIElement control)
        {
            var layer = AdornerLayer.GetAdornerLayer(control);


            if (layer == null) return;

            var adorners = layer.GetAdorners(control);
            if (adorners == null)
            {
                return;
            }

            foreach (var adorner in adorners.OfType<TextboxHintAdorner>())
            {
                adorner.Visibility = Visibility.Hidden;
                layer.Remove(adorner);
            }
        }


        private static void ShowHint(Control control)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);

            if (layer != null)
            {
                layer.Add(new TextboxHintAdorner(control, GetHint(control)));
            }
        }


        private static bool ShouldShowHint(Control c)
        {
            if (c is TextBox)
            {
                return ((TextBox) c).Text == string.Empty;
            }
            return false;
        }
    }
}