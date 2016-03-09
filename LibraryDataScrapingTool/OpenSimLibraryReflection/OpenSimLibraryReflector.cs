#region FileInfo
// 
// File: OpenSimLibraryReflector.cs
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
using System.IO;
using System.Linq;
using System.Reflection;
using LibLSLCC.CodeValidator;
using LibLSLCC.Collections;

#endregion

namespace LibraryDataScrapingTool.OpenSimLibraryReflection
{
    public interface IReflectedLibraryData
    {
        /// <summary>
        ///     The base class of the LSL script in generated code
        /// </summary>
        Type ScriptBaseClass { get; }

        /// <summary>
        ///     Interfaces that contain function signatures representing LSL library calls
        ///     all public interface methods will be reflected
        /// </summary>
        IEnumerable<Type> FunctionContainingInterfaces { get; }

        /// <summary>
        ///     Classes with attributed functions and constants.
        ///     if attribute is equal to ScriptModuleFunctionAttribute, then
        ///     the marked method will be visible to the reflector as a public LSL library function.
        ///     if the attribute is equal to ScriptModuleConstantAttribute, then the field is a public LSL
        ///     constant
        /// </summary>
        IEnumerable<Type> ScriptModuleClasses { get; }

        /// <summary>
        ///     Classes in which all public static fields are to be considered public LSL constants
        ///     regardless of attribute notation
        /// </summary>
        IEnumerable<Type> ScriptConstantContainerClasses { get; }

        /// <summary>
        ///     The type of attribute used to mark methods as public lsl functions inside
        ///     AttributedModuleClasses
        /// </summary>
        Type ScriptModuleFunctionAttribute { get; }

        /// <summary>
        ///     The type of attribute used to mark methods as public lsl constants inside
        ///     AttributedModuleClasses
        /// </summary>
        Type ScriptModuleConstantAttribute { get; }

        /// <summary>
        ///     Runtime type for floats inside generated code
        /// </summary>
        Type RuntimeFloat { get; }

        /// <summary>
        ///     Runtime type for integers inside generated code
        /// </summary>
        Type RuntimeInteger { get; }

        /// <summary>
        ///     Runtime type for strings inside generated code
        /// </summary>
        Type RuntimeString { get; }

        /// <summary>
        ///     Runtime type for lists inside generated code
        /// </summary>
        Type RuntimeList { get; }

        /// <summary>
        ///     Runtime type for vectors inside generated code
        /// </summary>
        Type RuntimeVector { get; }

        /// <summary>
        ///     Runtime type for rotations inside generated code
        /// </summary>
        Type RuntimeQuaternion { get; }

        /// <summary>
        ///     Runtime type for keys inside generated code
        /// </summary>
        Type RuntimeKey { get; }

        /// <summary>
        ///     Get the actual type of a function containing interface by its short class name
        /// </summary>
        /// <param name="name">class name</param>
        /// <returns>type</returns>
        Type GetFunctionContainingInterface(string name);

        /// <summary>
        ///     Get the actual type of a function/constant containing attributed module class by its short class name
        /// </summary>
        /// <param name="name">class name</param>
        /// <returns></returns>
        Type GetAttributedModuleClass(string name);

        /// <summary>
        ///     Get the actual type of a constant containing class by its short class name
        /// </summary>
        /// <param name="name">class name</param>
        /// <returns>type</returns>
        Type GetScriptConstantContainer(string name);



        IReadOnlyHashedSet<string> EventNames { get; }


        /// <summary>
        ///     Maps native return types/parameters for functions, and field types for constants
        ///     to an LSLType
        /// </summary>
        /// <returns>The LSLType that can represent the native type</returns>
        IReadOnlyHashMap<Type, LSLType> OpenSimToLSLTypeMapping();
    }

    public class OpenSimLibraryReflectedTypeData : IReflectedLibraryData
    {
        private readonly HashMap<AssemblyName, Assembly> _loaded = new HashMap<AssemblyName, Assembly>();

        public IReadOnlyHashMap<string, Assembly> AllOpenSimAssemblies
        {
            get { return _allOpenSimAssemblies; }
        }

        private readonly string _openSimBinDirectory;
        private readonly HashMap<string, Assembly> _allOpenSimAssemblies = new HashMap<string, Assembly>();
        private readonly GenericArray<Type> _scriptModuleClasses = new GenericArray<Type>();
        private readonly HashedSet<string> _eventNames;

        public OpenSimLibraryReflectedTypeData(string openSimBinDirectory)
        {
            _openSimBinDirectory = openSimBinDirectory;



            foreach (var assemblyPath in Directory.EnumerateFiles(openSimBinDirectory, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    string assemblyName = Path.GetFileName(assemblyPath);
                    if (assemblyName != null && !_allOpenSimAssemblies.ContainsKey(assemblyName))
                        _allOpenSimAssemblies.Add(assemblyName, Assembly.LoadFrom(assemblyPath));
                }
                catch
                {
                    //this is sparta
                }
            }


            ScriptApiAssembly = _allOpenSimAssemblies["OpenSim.Region.ScriptEngine.Shared.dll"];


            ScriptRuntimeAssembly = _allOpenSimAssemblies["OpenSim.Region.ScriptEngine.Shared.Api.Runtime.dll"];

            RegionFrameworkAssembly = _allOpenSimAssemblies["OpenSim.Region.Framework.dll"];

            OpenMetaverseTypesAssembly = _allOpenSimAssemblies["OpenMetaverseTypes.dll"];


            ScriptModuleFunctionAttribute =
                RegionFrameworkAssembly.GetType("OpenSim.Region.Framework.Interfaces.ScriptInvocationAttribute");

            ScriptModuleConstantAttribute =
                RegionFrameworkAssembly.GetType("OpenSim.Region.Framework.Interfaces.ScriptConstantAttribute");

            AppDomain.CurrentDomain.AssemblyResolve += _currentDomainOnAssemblyResolve;


            _eventNames = new HashedSet<string>(
                ScriptRuntimeAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.ScriptBase.Executor")
                    .GetNestedType("scriptEvents")
                    .GetMembers(BindingFlags.Public | BindingFlags.Static)
                    .Where(x => x.Name != "None")
                    .Select(x => x.Name)
                    );


            foreach (var assembly in AllOpenSimAssemblies.Values)
            {
                const BindingFlags bindingFlags =
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                try
                {
                    var types = assembly.GetTypes();

                    var interfaces = types.Where(x => x.GetInterfaces().Any(y => y.Name == "INonSharedRegionModule"));

                    _scriptModuleClasses.AddRange(
                        interfaces.Where(t => 
                        t.GetFields(bindingFlags).Cast<MemberInfo>().Concat(t.GetMethods(bindingFlags))
                        .Any(h => h.GetCustomAttributes(true).Any(
                            x =>
                            {
                                var n = x.GetType().Name;
                                return n == "ScriptConstantAttribute" || n == "ScriptInvocationAttribute";
                            }))).ToList()
                        );
                }
                catch (ReflectionTypeLoadException e)
                {

                    Log.WriteLineWithHeader("[OpenSimLibraryReflector ASSEMBLY LOAD EXCEPTION]", string.Join(Environment.NewLine, e.LoaderExceptions.Select(x => x.Message)));
                }
            }




            FunctionContainingInterfaces =
                ScriptApiAssembly.GetTypes()
                    .Where(x => x.IsInterface && x.Namespace == "OpenSim.Region.ScriptEngine.Shared.Api.Interfaces")
                    .ToList();


            ScriptBaseClass =
                ScriptRuntimeAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.ScriptBase.ScriptBaseClass");

            ScriptConstantContainerClasses = new GenericArray<Type> { ScriptBaseClass };

            AppDomain.CurrentDomain.AssemblyResolve -= _currentDomainOnAssemblyResolve;
        }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.Api.Runtime.dll
        /// </summary>
        public Assembly ScriptRuntimeAssembly { get; private set; }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.dll
        /// </summary>
        public Assembly ScriptApiAssembly { get; private set; }


        /// <summary>
        ///     OpenMetaverseTypes.dll
        /// </summary>
        public Assembly OpenMetaverseTypesAssembly { get; private set; }


        /// <summary>
        ///     OpenSim.Region.Framework.dll
        /// </summary>
        public Assembly RegionFrameworkAssembly { get; private set; }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.ScriptBase.ScriptBaseClass
        /// </summary>
        public Type ScriptBaseClass { get; private set; }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.Api.Interfaces.*
        /// </summary>
        public IEnumerable<Type> FunctionContainingInterfaces { get; private set; }

        public Type GetFunctionContainingInterface(string name)
        {
            return FunctionContainingInterfaces.SingleOrDefault(x => x.Name == name);
        }

        public Type GetAttributedModuleClass(string name)
        {
            return ScriptModuleClasses.SingleOrDefault(x => x.Name == name);
        }

        public Type GetScriptConstantContainer(string name)
        {
            return ScriptConstantContainerClasses.SingleOrDefault(x => x.Name == name);
        }

        public IReadOnlyHashedSet<string> EventNames
        {
            get { return _eventNames; }
        }

        /// <summary>
        ///     INonSharedRegionModule's that use ScriptConstantAttribute or ScriptInvocationAttribute anywhere in the class
        /// </summary>
        public IEnumerable<Type> ScriptModuleClasses
        {
            get { return _scriptModuleClasses; }
        }

        /// <summary>
        ///     ScriptBaseClass
        /// </summary>
        public IEnumerable<Type> ScriptConstantContainerClasses { get; private set; }

        /// <summary>
        ///     OpenSim.Region.Framework.Interfaces.ScriptInvocationAttribute
        /// </summary>
        public Type ScriptModuleFunctionAttribute { get; private set; }

        /// <summary>
        ///     OpenSim.Region.Framework.Interfaces.ScriptConstantAttribute
        /// </summary>
        public Type ScriptModuleConstantAttribute { get; private set; }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.LSL_Types.LSLFloat
        /// </summary>
        public Type RuntimeFloat
        {
            get
            {
                var t =
                    ScriptApiAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.LSL_Types")
                        .GetNestedType("LSLFloat");
                return t;
            }
        }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.LSL_Types.LSLInteger
        /// </summary>
        public Type RuntimeInteger
        {
            get
            {
                var t =
                    ScriptApiAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.LSL_Types")
                        .GetNestedType("LSLInteger");
                return t;
            }
        }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.LSL_Types.LSLString
        /// </summary>
        public Type RuntimeString
        {
            get
            {
                var t =
                    ScriptApiAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.LSL_Types")
                        .GetNestedType("LSLString");
                return t;
            }
        }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.LSL_Types.LSLList
        /// </summary>
        public Type RuntimeList
        {
            get
            {
                var t =
                    ScriptApiAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.LSL_Types")
                        .GetNestedType("list");
                return t;
            }
        }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.LSL_Types.LSLVector
        /// </summary>
        public Type RuntimeVector
        {
            get
            {
                var t =
                    ScriptApiAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.LSL_Types")
                        .GetNestedType("Vector3");
                return t;
            }
        }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.LSL_Types.LSLQuaternion
        /// </summary>
        public Type RuntimeQuaternion
        {
            get
            {
                var t =
                    ScriptApiAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.LSL_Types")
                        .GetNestedType("Quaternion");
                return t;
            }
        }

        /// <summary>
        ///     OpenSim.Region.ScriptEngine.Shared.LSL_Types.key
        /// </summary>
        public Type RuntimeKey
        {
            get
            {
                var t =
                    ScriptApiAssembly.GetType("OpenSim.Region.ScriptEngine.Shared.LSL_Types")
                        .GetNestedType("key");
                return t;
            }
        }


        /// <summary>
        ///     OpenMetaverse.UUID
        /// </summary>
        public Type OpenMetaverseKey
        {
            get
            {
                var t =
                    OpenMetaverseTypesAssembly.GetType("OpenMetaverse.UUID");
                return t;
            }
        }

        /// <summary>
        ///     OpenMetaverse.Vector3
        /// </summary>
        public Type OpenMetaverseVector3
        {
            get
            {
                var t =
                    OpenMetaverseTypesAssembly.GetType("OpenMetaverse.Vector3");
                return t;
            }
        }

        /// <summary>
        ///     OpenMetaverse.Vector3d
        /// </summary>
        public Type OpenMetaverseVector3D
        {
            get
            {
                var t =
                    OpenMetaverseTypesAssembly.GetType("OpenMetaverse.Vector3d");
                return t;
            }
        }

        /// <summary>
        ///     OpenMetaverse.Vector3d
        /// </summary>
        public Type OpenMetaverseVector4
        {
            get
            {
                var t =
                    OpenMetaverseTypesAssembly.GetType("OpenMetaverse.Vector4");
                return t;
            }
        }

        /// <summary>
        ///     OpenMetaverse.Quaternion
        /// </summary>
        public Type OpenMetaverseQuaternion
        {
            get
            {
                var t =
                    OpenMetaverseTypesAssembly.GetType("OpenMetaverse.Quaternion");
                return t;
            }
        }

        public IReadOnlyHashMap<Type, LSLType> OpenSimToLSLTypeMapping()
        {
            var r = new HashMap<Type, LSLType>
            {
                {
                    typeof(object[]),
                    LSLType.List
                },
                {
                    typeof (void),
                    LSLType.Void
                },

                {
                    typeof (bool),
                    LSLType.Integer
                }
                ,
                {
                    typeof (string),
                    LSLType.String
                }
                ,
                {
                    typeof (int),
                    LSLType.Integer
                }
                ,
                {
                    typeof (long),
                    LSLType.Integer
                }
                ,
                {
                    typeof (float),
                    LSLType.Float
                }
                ,
                {
                    typeof (double),
                    LSLType.Float
                }
                ,
                {
                    RuntimeFloat,
                    LSLType.Float
                }
                ,
                {
                    RuntimeInteger,
                    LSLType.Integer
                }
                ,
                {
                    RuntimeKey,
                    LSLType.Key
                }
                ,
                {
                    RuntimeList,
                    LSLType.List
                }
                ,
                {
                    RuntimeQuaternion,
                    LSLType.Rotation
                }
                ,
                {
                    RuntimeVector,
                    LSLType.Vector
                }
                ,
                {
                    RuntimeString,
                    LSLType.String
                }
                ,
                {
                    OpenMetaverseKey,
                    LSLType.Key
                }
                ,
                {
                    OpenMetaverseQuaternion,
                    LSLType.Rotation
                }
                ,
                {
                    OpenMetaverseVector3,
                    LSLType.Vector
                }
                ,
                {
                    OpenMetaverseVector3D,
                    LSLType.Vector
                }
                ,
                {
                    OpenMetaverseVector4,
                    LSLType.Rotation
                }
            };


            return r;
        }

        private Assembly _currentDomainOnAssemblyResolve(object sender, ResolveEventArgs loadArgs)
        {
            var aname = new AssemblyName(loadArgs.Name);

            if (_loaded.ContainsKey(aname))
            {
                return _loaded[aname];
            }


            if (loadArgs.RequestingAssembly == null)
                return null;


            var path = Path.Combine(_openSimBinDirectory, aname.Name + ".dll");

            if (!File.Exists(path))
                return null;

            var assembly = Assembly.LoadFrom(path);
            _loaded.Add(aname, assembly);
            return assembly;
        }
    }
}