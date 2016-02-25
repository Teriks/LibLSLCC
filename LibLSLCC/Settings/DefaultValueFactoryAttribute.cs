#region FileInfo

// 
// File: DefaultValueFactoryAttribute.cs
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

#endregion

namespace LibLSLCC.Settings
{
    /// <summary>
    ///     An attribute for defining a default value factory for a field or property.
    /// </summary>
    /// <seealso cref="DefaultValueInitializer" />
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DefaultValueFactoryAttribute : Attribute
    {
        /// <summary>
        ///     Construct the <see cref="DefaultValueFactoryAttribute" /> using a given type and <paramref name="initOrder" />.
        /// </summary>
        /// <param name="factoryType">
        ///     The <see cref="Type" /> for producing default field/property values, which should derive from
        ///     <see cref="IDefaultSettingsValueFactory" />.
        /// </param>
        /// <param name="initOrder">
        ///     The priority of default value initialization for this field/property when
        ///     <see cref="DefaultValueInitializer.Init{T}" /> is used.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="factoryType" /> does not implement
        ///     <see cref="IDefaultSettingsValueFactory" />.
        /// </exception>
        public DefaultValueFactoryAttribute(Type factoryType, int initOrder = 0)
        {
            FactoryType = factoryType;
            InitOrder = initOrder;
            Factory = Activator.CreateInstance(factoryType) as IDefaultSettingsValueFactory;

            if (Factory == null)
            {
                throw new ArgumentException(
                    string.Format(
                        "Cannot use '{0}' as a default value factory as it does not implement IDefaultSettingsValueFactory.",
                        factoryType.FullName));
            }
        }


        /// <summary>
        ///     The <see cref="Type" /> of the default value factory used to create a default value for the field/property.
        /// </summary>
        public Type FactoryType { get; private set; }

        /// <summary>
        ///     The priority of default value initialization for this field/property when
        ///     <see cref="DefaultValueInitializer.Init{T}" /> is used.
        /// </summary>
        public int InitOrder { get; private set; }

        /// <summary>
        ///     An instance of the default value factory type, specified by <see cref="FactoryType" />.
        /// </summary>
        public IDefaultSettingsValueFactory Factory { get; private set; }
    }
}