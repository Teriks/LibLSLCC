#region


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
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
        private readonly IReflectedLibraryData _reflectedData;
        private readonly HashSet<Type> _scriptConstantContainerClasses = new HashSet<Type>();
        private readonly Dictionary<Type, HashSet<string>> _subsetsMap = new Dictionary<Type, HashSet<string>>();

        public bool IncludeFunctions { get; set; }
        public bool IncludeConstants { get; set; }
        

        public OpenSimDirectLibraryData(IReflectedLibraryData reflectedData,
            IDocumentationProvider documentationProvider)
        {
            OptionalScriptModuleConstantAttribute = reflectedData.ScriptModuleConstantAttribute;
            OptionalScriptModuleFunctionAttribute = reflectedData.ScriptModuleFunctionAttribute;
            DataTypeMapping = reflectedData.OpenSimToLSLTypeMapping();
            _reflectedData = reflectedData;
            _documentationProvider = documentationProvider;
            IncludeConstants = true;
            IncludeFunctions = true;
        }

        public OpenSimDirectLibraryData(IReflectedLibraryData reflectedData)
        {
            OptionalScriptModuleConstantAttribute = reflectedData.ScriptModuleConstantAttribute;
            OptionalScriptModuleFunctionAttribute = reflectedData.ScriptModuleFunctionAttribute;
            DataTypeMapping = reflectedData.OpenSimToLSLTypeMapping();
            _reflectedData = reflectedData;
            _documentationProvider = new BlankDocumentor();
            IncludeConstants = true;
            IncludeFunctions = true;
        }

        public IReadOnlyDictionary<Type, LSLType> DataTypeMapping { get; set; }
        public Type OptionalScriptModuleConstantAttribute { get; set; }

        public Type OptionalScriptModuleFunctionAttribute { get; set; }

        public IReflectedLibraryData ReflectedData
        {
            get { return _reflectedData; }
        }


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
                constantContrainerType.GetField(name, bindingFlags) != null);

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

        public IReadOnlyList<LSLLibraryFunctionSignature> LSLFunctionOverloads(string name)
        {
            if(!IncludeFunctions) return new List<LSLLibraryFunctionSignature>();

            return LSLFunctions().Where(x => x.Name == name).ToList();
        }

        public LSLLibraryConstantSignature LSLConstant(string name)
        {
            if (!IncludeConstants) return null;

            foreach (var constantContrainerType in _scriptConstantContainerClasses)
            {
                var subsets = _subsetsMap[constantContrainerType];

                var constant = constantContrainerType.GetField(name);

                if (constant != null)
                {
                    var type = LslTypeFromCsharpType(constant.FieldType);
                    if (type != null)
                    {
                        var con = new LSLLibraryConstantSignature(type.Value, constant.Name);
                        con.DocumentationString = _documentationProvider.DocumentConstant(con);
                        con.SetSubsets(subsets);
                        return con;
                    }
                    Log.WriteLine(
                        "OpenSimLibraryDataReflector: constant {0} of ScriptBaseClass, type is an un-recognized data type ({1})",
                        name, constant.Name);

                    return null;
                }
                Log.WriteLine(
                    "OpenSimLibraryDataReflector: constant {0} does not exist in ScriptBaseClass",
                    name);

                return null;
            }

            return null;
        }

        public LSLLibraryEventSignature LSLEvent(string name)
        {
            return null;
        }

        public IEnumerable<IReadOnlyList<LSLLibraryFunctionSignature>> LSLFunctionOverloadGroups()
        {
            if (!IncludeFunctions) return new List<List<LSLLibraryFunctionSignature>>();

            return LSLFunctions().GroupBy(x => x.Name).Select(x => x.ToList());
        }

        public IEnumerable<LSLLibraryFunctionSignature> LSLFunctions()
        {
            if (!IncludeFunctions) return new List<LSLLibraryFunctionSignature>();

            return
                _functionContainingInterfaces.SelectMany(InterfaceMethods)
                    .Concat(_attributedScriptModuleClasses.SelectMany(ModuleMethods));
        }

        public IEnumerable<LSLLibraryConstantSignature> LSLConstants()
        {
            if (!IncludeConstants) yield break;

            const BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Public | BindingFlags.Static;

            foreach (var constantContrainerType in _scriptConstantContainerClasses)
            {
                var constantContainer = Activator.CreateInstance(constantContrainerType);

               

                var subsets = _subsetsMap[constantContrainerType];

                IEnumerable<FieldInfo> fields =
                    constantContrainerType.GetFields(bindingFlags);

                foreach (var c in fields)
                {
                    string valueString = c.GetValue(constantContainer).ToString();

                    var type = LslTypeFromCsharpType(c.FieldType);
                    if (type == null)
                    {
                        Log.WriteLine(
                            "OpenSimLibraryDataReflector: constant {0} of {1}, type is an un-recognized data type ({2})",
                            c.Name, constantContrainerType.Name, c.FieldType.Name);
                    }
                    else
                    {
                        var f = new LSLLibraryConstantSignature(type.Value, c.Name, valueString);
                        f.SetSubsets(subsets);
                        f.DocumentationString = _documentationProvider.DocumentConstant(f);
                        yield return f;
                    }
                }
            }


            foreach (var module in _attributedScriptModuleClasses)
            {
                var constantContainer = Activator.CreateInstance(module);
            
                IEnumerable<FieldInfo> fields;

                if (OptionalScriptModuleConstantAttribute == null)
                {
                    fields = module.GetFields(bindingFlags);
                }
                else
                {
                    fields = module.GetFields(bindingFlags)
                        .Where(x =>
                            x.GetCustomAttributes(OptionalScriptModuleConstantAttribute, true).Any());
                }

                var subsets = _subsetsMap[module];

                foreach (var c in fields)
                {
                    string valueString = c.GetValue(constantContainer).ToString();

                    var type = LslTypeFromCsharpType(c.FieldType);
                    if (type == null)
                    {
                        Log.WriteLine(
                            "OpenSimLibraryDataReflector: constant {0} of optional script module {1}, type is an un-recognized data type ({2})",
                            c.Name, module.Name, c.FieldType.Name);
                    }
                    else
                    {
                        var f = new LSLLibraryConstantSignature(type.Value, c.Name, valueString);
                        f.SetSubsets(subsets);
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
        /// /// <param name="subsets">The subsets functions from this interface will have in their Subsets property</param>
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
        /// /// <param name="subsets">The subsets functions from this interface will have in their Subsets property</param>
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

        private LSLType? LslTypeFromCsharpParameterType(Type t, string name)
        {
            if (DataTypeMapping.ContainsKey(t))
            {
                return DataTypeMapping[t];
            }
            return null;
        }

        private static bool IsParams(ParameterInfo param)
        {
            return param.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0;
        }

        private IEnumerable<LSLLibraryFunctionSignature> InterfaceMethods(Type containerType)
        {
            foreach (var v in containerType.GetMethods())
            {
                var subsets = _subsetsMap[containerType];

                var returnType = LslTypeFromCsharpType(v.ReturnType);


                var pTypes = new List<LSLParameter>();

                foreach (var p in v.GetParameters())
                {
                    var isVariadic = IsParams(p);
                    if (p.ParameterType == (typeof (object[])) && isVariadic)
                    {
                        pTypes.Add(new LSLParameter(LSLType.Void, p.Name, true));
                        continue;
                    }
                    if (isVariadic)
                    {
                        Log.WriteLine(
                            "OpenSimLibraryDataReflector: Interface function {0} of type {1}, variadic parameter {2} is an un-recognized data type ({3}), function ommited",
                            v.Name, containerType.Name, p.Name, p.ParameterType.Name);
                        yield break;
                    }

                    var type = LslTypeFromCsharpParameterType(p.ParameterType, p.Name);
                    if (type != null)
                    {
                        pTypes.Add(new LSLParameter(type.Value, p.Name,false));
                    }
                    else
                    {
                        Log.WriteLine(
                            "OpenSimLibraryDataReflector: Interface function {0} of type {1}, parameter {2} is an un-recognized data type ({3}), function ommited",
                            v.Name, containerType.Name, p.Name, p.ParameterType.Name);
                        yield break;
                    }
                }

                if (returnType != null)
                {
                    var f = new LSLLibraryFunctionSignature(returnType.Value, v.Name, pTypes);
                    f.DocumentationString = _documentationProvider.DocumentFunction(f);
                    f.SetSubsets(subsets);
                    yield return f;
                }
                else
                {
                    Log.WriteLine(
                        "OpenSimLibraryDataReflector: function {0} of type {1} return type is an un-recognized data type ({2})",
                        v.Name, containerType.Name, v.ReturnType.Name);
                }
            }
        }

        private IEnumerable<LSLLibraryFunctionSignature> ModuleMethods(Type containerType)
        {
            const BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance;


            if (OptionalScriptModuleConstantAttribute == null)
            {
                yield break;
            }


            IEnumerable<MethodInfo> methods;

            if (OptionalScriptModuleFunctionAttribute == null)
            {
                methods = containerType.GetMethods(bindingFlags);
            }
            else
            {
                methods = containerType.GetMethods(bindingFlags)
                    .Where(x =>
                        x.GetCustomAttributes(OptionalScriptModuleFunctionAttribute).Any());
            }

            var subsets = _subsetsMap[containerType];

            foreach (var v in methods)
            {
                var returnType = LslTypeFromCsharpType(v.ReturnType);


                var pTypes = new List<LSLParameter>();

                foreach (var p in v.GetParameters().Skip(2))
                {
                    var isVariadic = IsParams(p);
                    if (p.ParameterType == (typeof(object[])) && isVariadic)
                    {
                        pTypes.Add(new LSLParameter(LSLType.Void, p.Name, true));
                        continue;
                    }
                    if (isVariadic)
                    {
                        Log.WriteLine(
                            "OpenSimLibraryDataReflector: Optional script module function {0} of type {1}, variadic parameter {2} is an un-recognized data type ({3}), function ommited",
                            v.Name, containerType.Name, p.Name, p.ParameterType.Name);
                        yield break;
                    }

                    var type = LslTypeFromCsharpParameterType(p.ParameterType, p.Name);
                    if (type != null)
                    {
                        pTypes.Add(new LSLParameter(type.Value, p.Name, false));
                    }
                    else
                    {
                        Log.WriteLine(
                            "OpenSimLibraryDataReflector: Optional script module function {0} of type {1}, parameter {2} is an un-recognized data type ({3}), function ommited",
                            v.Name, containerType.Name, p.Name, p.ParameterType.Name);
                        yield break;
                    }
                }


                if (returnType != null)
                {
                    var f = new LSLLibraryFunctionSignature(returnType.Value, v.Name, pTypes);
                    f.DocumentationString = _documentationProvider.DocumentFunction(f);
                    f.SetSubsets(subsets);
                    yield return f;
                }
                else
                {
                    Log.WriteLine(
                        "OpenSimLibraryDataReflector: Optional script module  {0} of type {1} return type is an un-recognized data type ({2})",
                        v.Name, containerType.Name, v.ReturnType.Name);
                }
            }
        }
    }
}