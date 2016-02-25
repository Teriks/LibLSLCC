#region FileInfo

// 
// File: SettingsPropertyChangedEventArgs.cs
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

namespace LibLSLCC.Settings
{
    /// <summary>
    ///     Event arguments for when a settings property is changing.  <see cref="SettingsBaseClass{T}" />
    /// </summary>
    /// <typeparam name="TSetting"></typeparam>
    public class SettingsPropertyChangedEventArgs<TSetting>
    {
        /// <summary>
        ///     Construct the settings property changed event args.
        /// </summary>
        /// <param name="propertyOwner">The object that contains the property that has changed.</param>
        /// <param name="subscriber">The subscriber object that is subscribed to receive the changed event.</param>
        /// <param name="propertyName">The name of the property that has changed.</param>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        public SettingsPropertyChangedEventArgs(TSetting propertyOwner, object subscriber, string propertyName,
            object oldValue, object newValue)
        {
            PropertyOwner = propertyOwner;
            Subscriber = subscriber;
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }


        /// <summary>
        ///     The object that contains the property that has changed.
        /// </summary>
        public TSetting PropertyOwner { get; private set; }

        /// <summary>
        ///     The subscriber object that is subscribed to receive the changed event.
        /// </summary>
        public object Subscriber { get; private set; }

        /// <summary>
        ///     The name of the property that has changed.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        ///     The old value of the property.
        /// </summary>
        public object OldValue { get; private set; }

        /// <summary>
        ///     The new value of the property.
        /// </summary>
        public object NewValue { get; private set; }
    }
}