#region FileInfo
// 
// File: LSLEditorCompletionWindowItemBrushes.cs
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
using System.Reflection;
using System.Windows.Media;
using LibLSLCC.Settings;
using LSLCCEditor.Utility.Xml;

namespace LSLCCEditor.EditControl
{
    public class LSLEditorCompletionWindowItemBrushes :  SettingsBaseClass<LSLEditorCompletionWindowItemBrushes>
    {
        private XmlSolidBrush _typeBrush = new SolidColorBrush(Color.FromRgb(50, 52, 138));
        private XmlSolidBrush _eventHandlerBrush = new SolidColorBrush(Color.FromRgb(0, 76, 127));
        private XmlSolidBrush _globalFunctionBrush = new SolidColorBrush(Color.FromRgb(153, 0, 204));
        private XmlSolidBrush _globalVariableBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private XmlSolidBrush _libraryConstantBrush = new SolidColorBrush(Color.FromRgb(50, 52, 138));
        private XmlSolidBrush _libraryFunctionBrush = new SolidColorBrush(Color.FromRgb(127, 0, 38));
        private XmlSolidBrush _libraryFunctionDeprecatedBrush = new SolidColorBrush(Color.FromRgb(232, 19, 174));
        private XmlSolidBrush _localParameterBrush = new SolidColorBrush(Color.FromRgb(0, 102, 0));
        private XmlSolidBrush _localVariableBrush = new SolidColorBrush(Color.FromRgb(0, 102, 255));
        private XmlSolidBrush _labelNameDefinitionBrush = new SolidColorBrush(Color.FromRgb(127, 0, 38));
        private XmlSolidBrush _stateNameBrush = new SolidColorBrush(Colors.Black);
        private XmlSolidBrush _labelNameJumpTargetBrush = new SolidColorBrush(Colors.Black);
        private XmlSolidBrush _controlStatementBrush = new SolidColorBrush(Colors.Black);


        private class DefaultBrushFactory : IDefaultSettingsValueFactory
        {
            private static readonly LSLEditorCompletionWindowItemBrushes Default = new LSLEditorCompletionWindowItemBrushes();
            public bool CheckForNecessaryResets(MemberInfo member, object objectInstance, object settingValue)
            {
                if (settingValue == null)
                {
                    return true;
                }
                return false;
            }

            public object GetDefaultValue(MemberInfo member, object objectInstance)
            {
                var prop = member as PropertyInfo;
                if (prop != null)
                {
                    return ((ICloneable)prop.GetValue(Default)).Clone();
                }
                return null;
            }
        }

        [DefaultValueFactory(typeof(DefaultBrushFactory))]
        public XmlSolidBrush TypeBrush
        {
            get { return _typeBrush; }
            set { SetField(ref _typeBrush, value, "TypeBrush"); }
        }

        [DefaultValueFactory(typeof(DefaultBrushFactory))]
        public XmlSolidBrush EventHandlerBrush
        {
            get { return _eventHandlerBrush; }
            set { SetField(ref _eventHandlerBrush, value, "EventHandlerBrush"); }
        }

        [DefaultValueFactory(typeof(DefaultBrushFactory))]
        public XmlSolidBrush GlobalFunctionBrush
        {
            get { return _globalFunctionBrush; }
            set { SetField(ref _globalFunctionBrush, value, "GlobalFunctionBrush"); }
        }

        [DefaultValueFactory(typeof(DefaultBrushFactory))]
        public XmlSolidBrush GlobalVariableBrush
        {
            get { return _globalVariableBrush; }
            set { SetField(ref _globalVariableBrush, value, "GlobalVariableBrush"); }
        }

        [DefaultValueFactory(typeof(DefaultBrushFactory))]
        public XmlSolidBrush LibraryConstantBrush
        {
            get { return _libraryConstantBrush; }
            set { SetField(ref _libraryConstantBrush, value, "LibraryConstantBrush"); }
        }

        [DefaultValueFactory(typeof(DefaultBrushFactory))]
        public XmlSolidBrush LibraryFunctionBrush
        {
            get { return _libraryFunctionBrush; }
            set { SetField(ref _libraryFunctionBrush, value, "LibraryFunctionBrush"); }
        }

        [DefaultValueFactory(typeof(DefaultBrushFactory))]
        public XmlSolidBrush LibraryFunctionDeprecatedBrush
        {
            get { return _libraryFunctionDeprecatedBrush; }
            set { SetField(ref _libraryFunctionDeprecatedBrush, value, "LibraryFunctionDeprecatedBrush"); }
        }

        [DefaultValueFactory(typeof(DefaultBrushFactory))]
        public XmlSolidBrush LocalParameterBrush
        {
            get { return _localParameterBrush; }
            set { SetField(ref _localParameterBrush, value, "LocalParameterBrush"); }
        }

        [DefaultValueFactory(typeof(DefaultBrushFactory))]
        public XmlSolidBrush LocalVariableBrush
        {
            get { return _localVariableBrush; }
            set { SetField(ref _localVariableBrush, value, "LocalVariableBrush"); }
        }

        [DefaultValueFactory(typeof(DefaultBrushFactory))]
        public XmlSolidBrush LabelNameDefinitionBrush
        {
            get { return _labelNameDefinitionBrush; }
            set { SetField(ref _labelNameDefinitionBrush, value, "LabelNameDefinitionBrush"); }
        }

        [DefaultValueFactory(typeof(DefaultBrushFactory))]
        public XmlSolidBrush StateNameBrush
        {
            get { return _stateNameBrush; }
            set { SetField(ref _stateNameBrush, value, "StateNameBrush"); }
        }

        [DefaultValueFactory(typeof(DefaultBrushFactory))]
        public XmlSolidBrush LabelNameJumpTargetBrush
        {
            get { return _labelNameJumpTargetBrush; }
            set { SetField(ref _labelNameJumpTargetBrush, value, "LabelNameJumpTargetBrush"); }
        }

        [DefaultValueFactory(typeof(DefaultBrushFactory))]
        public XmlSolidBrush ControlStatementBrush
        {
            get { return _controlStatementBrush; }
            set { SetField(ref _controlStatementBrush, value, "ControlStatementBrush"); }
        }
    }
}