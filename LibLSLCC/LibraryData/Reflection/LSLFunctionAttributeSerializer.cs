using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.LibraryData.Reflection
{
    internal class LSLFunctionAttributeSerializer
    {
        public ILSLTypeConverter FallBackReturnTypeConverter { get; set; }

        public ILSLTypeConverter FallBackParameterTypeConverter { get; set; }


        public class Info
        {
            public bool Variadic { get; set; }

            public bool ExplicitReturnTypePresent { get; set; }

            public LSLType ReturnType { get; set; }

            public List<LSLParameter> Parameters { get; set; }


            public ILSLTypeConverter ReturnTypeConverter { get; set; }

            public ILSLTypeConverter ParamTypeConverter { get; set; }

            public Info()
            {
                Parameters = new List<LSLParameter>();
            }
        }


        public Info GetInfo(MethodInfo method)
        {
            if (!Attribute.IsDefined(method, typeof (LSLFunctionAttribute)))
            {
                return null;
            }


            var result = new Info();

            var attr =
                method
                    .GetCustomAttributesData().First(x => x.Constructor.DeclaringType == typeof (LSLFunctionAttribute));


            if (attr.ConstructorArguments.Count > 0)
            {
                result.ExplicitReturnTypePresent = true;
                result.ReturnType = (LSLType)attr.ConstructorArguments[0].Value;
            }
            else if (attr.NamedArguments.Any(x => x.MemberInfo.Name == "ReturnType"))
            {
                result.ExplicitReturnTypePresent = true;
                result.ReturnType =
                    (LSLType) attr.NamedArguments.First(x => x.MemberInfo.Name == "ReturnType").TypedValue.Value;
            }


            if (attr.NamedArguments.Any(x => x.MemberInfo.Name == "ReturnTypeConverter"))
            {
                var rtConverter =
                    (Type) attr.NamedArguments.First(x => x.MemberInfo.Name == "ReturnTypeConverter").TypedValue.Value;

                if (result.ExplicitReturnTypePresent)
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            "[LSLFunctionAttribute] of on method '{0}' declared in Type '{1}' has both an explicit " +
                            "return type and a ReturnTypeConverter of Type {2}, only one or the other is allowed.",
                            method.Name, method.DeclaringType.FullName, rtConverter.FullName));
                }

                if (!rtConverter.GetInterfaces().Contains(typeof (ILSLTypeConverter)))
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            "[LSLFunctionAttribute.ReturnTypeConverter] of Type '{0}' on method '{1}' declared in " +
                            "Type '{2}' does not implement ILSLTypeConverter.",
                            rtConverter.FullName,
                            method.Name, 
                            method.DeclaringType.FullName));
                }

                result.ReturnTypeConverter = (ILSLTypeConverter) Activator.CreateInstance(rtConverter);
            }


            if (!result.ExplicitReturnTypePresent && result.ReturnTypeConverter == null &&
                FallBackReturnTypeConverter == null)
            {
                throw new LSLLibraryDataAttributeException(
                    string.Format(
                        "[LSLFunctionAttribute] of on method '{0}' declared in Type '{1}' has no explicit ReturnType or "+
                        "ReturnTypeConverter, and no fall-back converter was specified in the serializer.",
                        method.Name,
                        method.DeclaringType.FullName));
            }


            if (!result.ExplicitReturnTypePresent)
            {
                var preferedReturnTypeConverter = result.ReturnTypeConverter ?? FallBackReturnTypeConverter;

                LSLType returnType;

                if (!preferedReturnTypeConverter.Convert(method.ReturnType, out returnType))
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            "Return type of method '{1}' with [LSLFunctionAttribute] attribute declared in Type '{2}' could not be converted from '{3}' to a  " +
                            "corresponding LSLType by the preferred ReturnTypeConverter of Type '{0}'.",
                            method.Name,
                            method.DeclaringType.FullName,
                            method.ReturnType.FullName,
                            preferedReturnTypeConverter.GetType().FullName));
                }

                result.ReturnType = returnType;
            }


            if (attr.NamedArguments.Any(x => x.MemberInfo.Name == "ParamTypeConverter"))
            {
                var rtConverter =
                    (Type) attr.NamedArguments.First(x => x.MemberInfo.Name == "ParamTypeConverter").TypedValue.Value;

                if (!rtConverter.GetInterfaces().Contains(typeof (ILSLTypeConverter)))
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            "[LSLFunctionAttribute.ParamTypeConverter] of Type '{0}' on method '{1}' declared in Type '{2}' does not implement ILSLTypeConverter.",
                            rtConverter.FullName,
                            method.Name,
                            method.DeclaringType.FullName));
                }

                result.ParamTypeConverter = (ILSLTypeConverter) Activator.CreateInstance(rtConverter);
            }


            var preferedParameterConverter = result.ParamTypeConverter ?? FallBackParameterTypeConverter;

            var parameters = method.GetParameters().ToList();


            foreach (var param in parameters)
            {
                var paramAttributes =
                    param.GetCustomAttributesData()
                        .Where(x => x.Constructor.DeclaringType == typeof (LSLParamAttribute))
                        .ToList();

                LSLType pType;
                bool isVariadic;

                
                if (paramAttributes.Count == 0)
                {
                    //without attribute

                    if (preferedParameterConverter == null)
                    {
                        throw new LSLLibraryDataAttributeException(
                            string.Format(
                                "Required [LSLParamAttribute] missing on parameter index '{0}' in method '{1}' declared in Type '{2}'.  It was required because " +
                                "an explicit [LSLFunctionAttribute.ParamTypeConverter] was not defined on the method, and the serializer provided no fall-back converter.",
                                param.Position,
                                method.Name,
                                method.DeclaringType.FullName));
                    }


                    isVariadic =
                        param.GetCustomAttributesData()
                            .Any(x => x.Constructor.DeclaringType == typeof (ParamArrayAttribute));
                    var cSharpParameterType = isVariadic ? param.ParameterType.GetElementType() : param.ParameterType;


                    if (!preferedParameterConverter.Convert(cSharpParameterType, out pType))
                    {
                        throw new LSLLibraryDataAttributeException(
                            string.Format(
                                "Preferred parameter type converter of Type '{0}' used by attribute [LSLFunctionAttribute] on method '{1}' declared in Type '{2}', failed to converter a parameter of Type '{3}'.",
                                preferedParameterConverter.GetType().FullName,
                                method.Name,
                                method.DeclaringType.FullName,
                                cSharpParameterType.FullName));
                    }

                    if (pType == LSLType.Void && !isVariadic)
                    {
                        throw new LSLLibraryDataAttributeException(
                            string.Format(
                                "Preferred parameter type converter of Type '{0}' used by attribute [LSLFunctionAttribute] on method '{1}'" +
                                " declared in Type '{2}', converted a parameter of Type '{3}' to 'LSLType.Void' and the parameter was not variadic.",
                                preferedParameterConverter.GetType().FullName,
                                method.Name,
                                method.DeclaringType.FullName,
                                cSharpParameterType.FullName));
                    }

                    if (isVariadic)
                    {
                        result.Variadic = true;
                    }
                    result.Parameters.Add(new LSLParameter(pType, param.Name, isVariadic));
                }
                else
                {

                    //with explicit attribute

                    var pAttr = paramAttributes.First();

                    pType = (LSLType) pAttr.ConstructorArguments[0].Value;

                    isVariadic = param.GetCustomAttributesData().
                        Any(x => x.Constructor.DeclaringType == typeof (ParamArrayAttribute));

                    if (isVariadic)
                    {
                        result.Variadic = true;
                    }

                    result.Parameters.Add(new LSLParameter(pType, param.Name, isVariadic));
                }
            }


            return result;
        }
    }
}