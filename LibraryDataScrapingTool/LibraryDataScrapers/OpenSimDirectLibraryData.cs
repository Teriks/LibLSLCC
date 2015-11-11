#region FileInfo

// 
// File: OpenSimDirectLibraryData.cs
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
using System.Linq;
using System.Reflection;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;
using LibLSLCC.LibraryData;
using LibraryDataScrapingTools.OpenSimLibraryReflection;
using LibraryDataScrapingTools.ScraperInterfaces;
using LibraryDataScrapingTools.ScraperProxys;

#endregion

namespace LibraryDataScrapingTools.LibraryDataScrapers
{
    public class OpenSimDirectLibraryData : ILibraryData
    {
        private readonly HashSet<Type> _attributedScriptModuleClasses = new HashSet<Type>();
        private readonly IDocumentationProvider _documentationProvider;
        private readonly HashSet<Type> _functionContainingInterfaces = new HashSet<Type>();
        private readonly HashSet<Type> _scriptConstantContainerClasses = new HashSet<Type>();
        private readonly HashMap<Type, HashSet<string>> _subsetsMap = new HashMap<Type, HashSet<string>>();

        public OpenSimDirectLibraryData(IReflectedLibraryData reflectedData,
            IDocumentationProvider documentationProvider)
        {
            ScriptModuleConstantAttribute = reflectedData.ScriptModuleConstantAttribute;
            ScriptModuleFunctionAttribute = reflectedData.ScriptModuleFunctionAttribute;
            DataTypeMapping = reflectedData.OpenSimToLSLTypeMapping();
            ReflectedData = reflectedData;
            _documentationProvider = documentationProvider;
            IncludeConstants = true;
            IncludeFunctions = true;
        }

        public OpenSimDirectLibraryData(IReflectedLibraryData reflectedData)
        {
            ScriptModuleConstantAttribute = reflectedData.ScriptModuleConstantAttribute;
            ScriptModuleFunctionAttribute = reflectedData.ScriptModuleFunctionAttribute;
            DataTypeMapping = reflectedData.OpenSimToLSLTypeMapping();
            ReflectedData = reflectedData;
            _documentationProvider = new BlankDocumentor();
            IncludeConstants = true;
            IncludeFunctions = true;
        }

        public bool IncludeFunctions { get; set; }
        public bool IncludeConstants { get; set; }
        public IReadOnlyHashMap<Type, LSLType> DataTypeMapping { get; set; }
        public Type ScriptModuleConstantAttribute { get; set; }
        public Type ScriptModuleFunctionAttribute { get; set; }
        public IReflectedLibraryData ReflectedData { get; private set; }

        public bool LSLFunctionExist(string name)
        {
            if (!IncludeFunctions) return false;

            var interfaces = _functionContainingInterfaces.Any(
                libraryType =>
                {
                    try
                    {
                        return libraryType.GetMethod(name) != null;
                    }
                    catch (AmbiguousMatchException)
                    {
                        //function exist with overloads
                        return true;
                    }
                });

            var classes = _attributedScriptModuleClasses.Any(x =>
            {
                try
                {
                    var m = x.GetMethod(name);
                    if (m != null && m.GetCustomAttributes(ReflectedData.ScriptModuleFunctionAttribute, true).Any())
                    {
                        return true;
                    }
                    return false;
                }
                catch (AmbiguousMatchException)
                {
                    return
                        x.GetMethods()
                            .Any(
                                t =>
                                    t.Name == name &&
                                    t.GetCustomAttributes(ReflectedData.ScriptModuleFunctionAttribute, true).Any());
                }
            });

            return interfaces || classes;
        }

        public bool LSLConstantExist(string name)
        {
            if (!IncludeConstants) return false;

            const BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Static | BindingFlags.Public;

            var containers = _scriptConstantContainerClasses.Any(constantContrainerType =>
                constantContrainerType.GetField(name, bindingFlags) != null && name != "GCDummy");

            var classes =
                _attributedScriptModuleClasses.Any(
                    x =>
                    {
                        var fieldInfo = x.GetField(name, bindingFlags);
                        return fieldInfo != null &&
                               fieldInfo.GetCustomAttributes(ReflectedData.ScriptModuleConstantAttribute, true).Any();
                    });

            return containers || classes;
        }

        public bool LSLEventExist(string name)
        {
            return false;
        }

        public IReadOnlyGenericArray<LSLLibraryFunctionSignature> LSLFunctionOverloads(string name)
        {
            if (!IncludeFunctions) return new GenericArray<LSLLibraryFunctionSignature>();

            return LSLFunctions().Where(x => x.Name == name).ToGenericArray();
        }

        public LSLLibraryConstantSignature LSLConstant(string name)
        {
            if (!IncludeConstants) return null;

            const BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;

            if (name == "GCDummy") return null;

            foreach (var constantContrainerType in _scriptConstantContainerClasses)
            {
                var subsets = _subsetsMap[constantContrainerType];

                var constant = constantContrainerType.GetField(name, bindingFlags);


                if (constant != null)
                {
                    var type = LslTypeFromCsharpType(constant.FieldType);
                    if (type != null)
                    {
                        var con = new LSLLibraryConstantSignature(type.Value, constant.Name,
                            constant.GetValue(null).ToString());
                        con.DocumentationString = _documentationProvider.DocumentConstant(con);
                        con.Subsets.SetSubsets(subsets);
                        return con;
                    }

                    Log.WriteLineWithHeader("[OpenSimLibraryDataReflector]: ",
                        "constant {0} of {1}, type is an un-recognized data type ({2})",
                        name, constantContrainerType.FullName, constant.Name);

                    return null;
                }

                Log.WriteLineWithHeader("[OpenSimLibraryDataReflector]: ",
                    "constant {0} does not exist in {1}",
                    name, constantContrainerType.FullName);
            }

            foreach (var constantContrainerType in _attributedScriptModuleClasses)
            {
                var subsets = _subsetsMap[constantContrainerType];

                var constant = constantContrainerType.GetField(name, bindingFlags);

                if (constant == null)
                {
                    Log.WriteLineWithHeader("[OpenSimLibraryDataReflector]: ",
                    "constant {0} does not exist in {1}",
                    name, constantContrainerType.FullName);

                    return null;
                }

                if(constant.GetCustomAttributes(ScriptModuleConstantAttribute,true).Any()) continue;

                
                var type = LslTypeFromCsharpType(constant.FieldType);

                if (type != null)
                {
                    var con = new LSLLibraryConstantSignature(type.Value, constant.Name,
                        constant.GetValue(null).ToString());
                    con.Expand = true;
                    con.DocumentationString = _documentationProvider.DocumentConstant(con);
                    con.Subsets.SetSubsets(subsets);
                    return con;
                }

                Log.WriteLineWithHeader("[OpenSimLibraryDataReflector]: ",
                    "constant {0} of {1}, type is an un-recognized data type ({2})",
                    name, constantContrainerType.FullName, constant.Name);

                return null;

            }

            return null;
        }

        public LSLLibraryEventSignature LSLEvent(string name)
        {
            return null;
        }

        public IEnumerable<IReadOnlyGenericArray<LSLLibraryFunctionSignature>> LSLFunctionOverloadGroups()
        {
            if (!IncludeFunctions) return new GenericArray<GenericArray<LSLLibraryFunctionSignature>>();

            return LSLFunctions().GroupBy(x => x.Name).Select(x => x.ToGenericArray());
        }

        public IEnumerable<LSLLibraryFunctionSignature> LSLFunctions()
        {
            if (!IncludeFunctions) yield break;

            foreach (var method in _functionContainingInterfaces.SelectMany(InterfaceMethods))
            {
                yield return method;
            }


            foreach (var method in _attributedScriptModuleClasses.SelectMany(ModuleMethods))
            {
                yield return method;
            }
        }

        public IEnumerable<LSLLibraryConstantSignature> LSLConstants()
        {
            if (!IncludeConstants) yield break;

            const BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;

            foreach (var constantContrainerType in _scriptConstantContainerClasses)
            {
                var constantContainer = Activator.CreateInstance(constantContrainerType);


                var subsets = _subsetsMap[constantContrainerType];

                IEnumerable<FieldInfo> fields =
                    constantContrainerType.GetFields(bindingFlags);

                foreach (var c in fields.Where(x => x.Name != "GCDummy"))
                {
                    var valueString = c.GetValue(constantContainer).ToString();

                    var type = LslTypeFromCsharpType(c.FieldType);
                    if (type == null)
                    {
                        Log.WriteLineWithHeader("[OpenSimLibraryDataReflector]: ",
                            "constant {0} of {1}, type is an un-recognized data type ({2})",
                            c.Name, constantContrainerType.FullName, c.FieldType.Name);
                    }
                    else
                    {
                        var f = new LSLLibraryConstantSignature(type.Value, c.Name, valueString);
                        f.Subsets.SetSubsets(subsets);
                        f.DocumentationString = _documentationProvider.DocumentConstant(f);
                        yield return f;
                    }
                }
            }


            foreach (var module in _attributedScriptModuleClasses)
            {
                var fields =
                    module.GetFields(bindingFlags)
                        .Where(x =>
                            x.GetCustomAttributes(ScriptModuleConstantAttribute, true).Any());

                var subsets = _subsetsMap[module];

                foreach (var c in fields.Where(x => x.Name != "GCDummy"))
                {
                    var valueString = c.GetValue(null).ToString();

                    var type = LslTypeFromCsharpType(c.FieldType);
                    if (type == null)
                    {
                        Log.WriteLineWithHeader("[OpenSimLibraryDataReflector]: ",
                            "constant {0} of optional script module {1}, type is an un-recognized data type ({2})",
                            c.Name, module.FullName, c.FieldType.Name);
                    }
                    else
                    {
                        var f = new LSLLibraryConstantSignature(type.Value, c.Name, valueString);
                        f.Expand = true;
                        f.Subsets.SetSubsets(subsets);
                        f.DocumentationString = _documentationProvider.DocumentConstant(f);
                        yield return f;
                    }
                }
            }
        }

        public IEnumerable<LSLLibraryEventSignature> LSLEvents()
        {
            yield break;
        }

        public void IncludeFunctionContainingInterface(Type type, IEnumerable<string> subsets)
        {
            if (!type.IsInterface)
            {
                throw new ArgumentException(string.Format("Type {0} is not an interface", type.FullName), "type");
            }

            if (_functionContainingInterfaces.Contains(type))
            {
                throw new ArgumentException("Duplicate library container type " + type.Name, "type");
            }
            _functionContainingInterfaces.Add(type);
            _subsetsMap.Add(type, new HashSet<string>(subsets));
        }

        /// <summary>
        ///     Adds ReflectedData.GetFunctionContainingInterface(typeName) to the list of function containing interfaces
        ///     that provide library functions to this ILibraryData object
        /// </summary>
        /// <param name="typeName">the name of the CSharp a class name that exist in ReflectedData.FunctionContainingInterfaces</param>
        /// <param name="subsets">The subsets functions from this interface will have in their Subsets property</param>
        public void IncludeFunctionContainingInterface(string typeName, IEnumerable<string> subsets)
        {
            var type = ReflectedData.GetFunctionContainingInterface(typeName);
            IncludeFunctionContainingInterface(type, subsets);
        }

        public void IncludeScriptConstantContainerClass(Type type, IEnumerable<string> subsets)
        {
            if (!type.IsClass)
            {
                throw new ArgumentException(string.Format("Type {0} is not a class", type.FullName), "type");
            }
            if (_scriptConstantContainerClasses.Contains(type))
            {
                throw new ArgumentException("Duplicate constant container type " + type.Name, "type");
            }
            _scriptConstantContainerClasses.Add(type);
            _subsetsMap.Add(type, new HashSet<string>(subsets));
        }

        /// <summary>
        ///     Adds ReflectedData.IncludeScriptConstantContainerClass(typeName) to the list of constant containing classes
        ///     that provide library constants to this ILibraryData object
        /// </summary>
        /// <param name="typeName">the name of the CSharp a class name that exist in ReflectedData.ScriptConstantContainerClasses</param>
        /// ///
        /// <param name="subsets">The subsets functions from this interface will have in their Subsets property</param>
        public void IncludeScriptConstantContainerClass(string typeName, IEnumerable<string> subsets)
        {
            var type = ReflectedData.GetScriptConstantContainer(typeName);
            IncludeScriptConstantContainerClass(type, subsets);
        }

        public void IncludeAttributedModuleClass(Type type, IEnumerable<string> subsets)
        {
            if (!type.IsClass)
            {
                throw new ArgumentException(string.Format("Type {0} is not a class", type.FullName), "type");
            }
            if (_attributedScriptModuleClasses.Contains(type))
            {
                throw new ArgumentException("Duplicate optional script module container type " + type.Name, "type");
            }
            _attributedScriptModuleClasses.Add(type);
            _subsetsMap.Add(type, new HashSet<string>(subsets));
        }

        /// <summary>
        ///     Adds ReflectedData.GetAttributedModuleClass(typeName) to the list of constant/function containing attribute marked
        ///     classes
        ///     that provide library constants+functions to this ILibraryData object
        /// </summary>
        /// <param name="typeName">the name of the CSharp a class name that exist in ReflectedData.AttributedModuleClasses</param>
        /// ///
        /// <param name="subsets">The subsets functions from this interface will have in their Subsets property</param>
        public void IncludeAttributedModuleClass(string typeName, IEnumerable<string> subsets)
        {
            var type = ReflectedData.GetAttributedModuleClass(typeName);
            IncludeAttributedModuleClass(type, subsets);
        }

        private LSLType? LslTypeFromCsharpType(Type t)
        {
            if (DataTypeMapping.ContainsKey(t))
            {
                return DataTypeMapping[t];
            }
            return null;
        }

        private LSLType? LslTypeFromCsharpParameterType(Type t)
        {
            if (DataTypeMapping.ContainsKey(t))
            {
                return DataTypeMapping[t];
            }
            return null;
        }

        private static bool IsParams(ParameterInfo param)
        {
            return param.GetCustomAttributes(typeof (ParamArrayAttribute), false).Length > 0;
        }

        private IEnumerable<LSLLibraryFunctionSignature> InterfaceMethods(Type containerType)
        {
            foreach (var v in containerType.GetMethods())
            {
                var subsets = _subsetsMap[containerType];

                var returnType = LslTypeFromCsharpType(v.ReturnType);

                var pTypes = new GenericArray<LSLParameter>();

                foreach (var p in v.GetParameters())
                {
                    var isVariadic = IsParams(p);
                    if (p.ParameterType == (typeof (object[])) && isVariadic)
                    {
                        pTypes.Add(new LSLParameter(LSLType.Void, p.Name, true));
                        goto omitRestOfParameters;
                    }

                    if (isVariadic)
                    {
                        Log.WriteLineWithHeader("[OpenSimLibraryDataReflector]: ",
                            "Interface function {0} of type {1}, variadic parameter {2} is an un-recognized data type ({3}), function omitted",
                            v.Name, containerType.Name, p.Name, p.ParameterType.Name);
                        goto omitFunction;
                    }

                    var type = LslTypeFromCsharpParameterType(p.ParameterType);
                    if (type != null)
                    {
                        pTypes.Add(new LSLParameter(type.Value, p.Name, false));
                    }
                    else
                    {
                        Log.WriteLineWithHeader("[OpenSimLibraryDataReflector]: ",
                            "Interface function {0} of type {1}, parameter {2} is an un-recognized data type ({3}), function omitted",
                            v.Name, containerType.Name, p.Name, p.ParameterType.Name);

                        goto omitFunction;
                    }
                }

                omitRestOfParameters:

                if (returnType != null)
                {
                    var f = new LSLLibraryFunctionSignature(returnType.Value, v.Name, pTypes);
                    f.DocumentationString = _documentationProvider.DocumentFunction(f);
                    f.Subsets.SetSubsets(subsets);

                    yield return f;
                }
                else
                {
                    Log.WriteLineWithHeader("[OpenSimLibraryDataReflector]: ",
                        "function {0} of type {1} return type is an un-recognized data type ({2})",
                        v.Name, containerType.Name, v.ReturnType.Name);
                }

                omitFunction:
                ;
            }
        }

        private IEnumerable<LSLLibraryFunctionSignature> ModuleMethods(Type containerType)
        {
            const BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;


            if (ScriptModuleConstantAttribute == null)
            {
                yield break;
            }


            var methods = containerType.GetMethods(bindingFlags)
                .Where(x =>
                    x.GetCustomAttributes(ScriptModuleFunctionAttribute, true).Any());


            var subsets = _subsetsMap[containerType];

            foreach (var v in methods)
            {
                var returnType = LslTypeFromCsharpType(v.ReturnType);


                var pTypes = new GenericArray<LSLParameter>();

                foreach (var p in v.GetParameters().Skip(2))
                {
                    var isVariadic = IsParams(p);
                    if (p.ParameterType == (typeof (object[])) && isVariadic)
                    {
                        pTypes.Add(new LSLParameter(LSLType.Void, p.Name, true));
                        goto omitRestOfParameters;
                    }
                    if (isVariadic)
                    {
                        Log.WriteLineWithHeader("[OpenSimLibraryDataReflector]: ",
                            "Optional script module function {0} of type {1}, variadic parameter {2} is an un-recognized data type ({3}), function omitted",
                            v.Name, containerType.Name, p.Name, p.ParameterType.Name);
                        goto omitFunction;
                    }

                    var type = LslTypeFromCsharpParameterType(p.ParameterType);
                    if (type != null)
                    {
                        pTypes.Add(new LSLParameter(type.Value, p.Name, false));
                    }
                    else
                    {
                        Log.WriteLineWithHeader("[OpenSimLibraryDataReflector]: ",
                            "Optional script module function {0} of type {1}, parameter {2} is an un-recognized data type ({3}), function omitted",
                            v.Name, containerType.Name, p.Name, p.ParameterType.Name);
                        goto omitFunction;
                    }
                }

                omitRestOfParameters:


                if (returnType != null)
                {
                    var f = new LSLLibraryFunctionSignature(returnType.Value, v.Name, pTypes);
                    f.DocumentationString = _documentationProvider.DocumentFunction(f);
                    f.Subsets.SetSubsets(subsets);
                    f.ModInvoke = true;
                    yield return f;
                }
                else
                {
                    Log.WriteLineWithHeader("[OpenSimLibraryDataReflector]: ",
                        "Optional script module  {0} of type {1} return type is an un-recognized data type ({2})",
                        v.Name, containerType.Name, v.ReturnType.Name);
                }

                omitFunction:
                ;
            }
        }
    }
}