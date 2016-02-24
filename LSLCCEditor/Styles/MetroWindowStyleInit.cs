#region FileInfo

// 
// File: MetroWindowStyleInit.cs
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

#region Imports

using System.Windows;
using System.Windows.Input;

#endregion

namespace LSLCCEditor.Styles
{
    public static class MetroWindowStyleInit
    {
        public static void Init(Window window)
        {
            window.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow,
                OnCanResizeWindow));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow,
                OnCanMinimizeWindow));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow,
                OnCanResizeWindow));
        }


        private static void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            var t = sender as Window;
            e.CanExecute = t.ResizeMode == ResizeMode.CanResize || t.ResizeMode == ResizeMode.CanResizeWithGrip;
        }


        private static void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            var t = sender as Window;
            e.CanExecute = t.ResizeMode != ResizeMode.NoResize;
        }


        private static void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            var t = target as Window;
            SystemCommands.CloseWindow(t);
        }


        private static void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            var t = target as Window;
            SystemCommands.MaximizeWindow(t);
        }


        private static void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            var t = target as Window;
            SystemCommands.MinimizeWindow(t);
        }


        private static void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            var t = target as Window;
            SystemCommands.RestoreWindow(t);
        }
    }
}