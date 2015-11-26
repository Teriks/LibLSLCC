using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.Compilers.OpenSim;
using LibLSLCC.LibraryData;
using LibLSLCC.LibraryData.Reflection;

namespace DemoArea
{


    class MyValueStringConverter : ILSLValueStringConverter
    {

        public bool ConvertProperty(PropertyInfo propertyInfo, LSLType constantType, object fieldValue, out string valueString)
        {
            valueString = fieldValue.ToString();
            return true;
        }

        public bool ConvertField(FieldInfo fieldInfo, LSLType constantType, object fieldValue, out string valueString)
        {
            valueString = fieldValue.ToString();
            return true;
        }
    }


    class MySimpleConverter : ILSLParamTypeConverter, ILSLReturnTypeConverter, ILSLConstantTypeConverter
    {
        private bool Convert(Type inType, out LSLType outType)
        {
            if (typeof (string) == inType)
            {
                outType = LSLType.String;
                return true;
            }
            if (typeof(int) == inType)
            {
                outType = LSLType.Integer;
                return true;
            }
            if (typeof(float) == inType)
            {
                outType = LSLType.Float;
                return true;
            }
            if (typeof(object[]) == inType)
            {
                outType = LSLType.List;
                return true;
            }
            if (typeof(void) == inType)
            {
                outType = LSLType.Void;
                return true;
            }

            outType = LSLType.Void;
            return false;
        }


        /// <summary>
        /// Converts the specified <see cref="ParameterInfo"/> into its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="parameterInfo">Runtime <see cref="ParameterInfo"/> for converting to an <see cref="LSLType"/>.</param>
        /// <param name="basicType">The basic type of the parameter, this will be the parameter arrays base type if the parameter is variadic.</param>
        /// <param name="outType">Resulting <see cref="LSLType"/> from the conversion.</param>
        /// <returns><c>true</c> if the conversion succeeded, <c>false</c> if it failed.</returns>
        public bool ConvertParameter(ParameterInfo parameterInfo, Type basicType, out LSLType outType)
        {
            return Convert(basicType, out outType);
        }

        /// <summary>
        /// Converts the specified <see cref="MethodInfo"/> return type into its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="methodInfo">Runtime <see cref="Type"/> to convert.</param>
        /// <param name="outType">Resulting <see cref="LSLType"/> from the conversion.</param>
        /// <returns><c>true</c> if the conversion succeeded, <c>false</c> if it failed.</returns>
        public bool ConvertReturn(MethodInfo methodInfo, out LSLType outType)
        {
            return Convert(methodInfo.ReturnType, out outType);
        }

        /// <summary>
        /// Converts the specified <see cref="FieldInfo"/> into its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="fieldInfo">Runtime <see cref="FieldInfo"/> to convert.</param>
        /// <param name="outType">Resulting <see cref="LSLType"/> from the conversion.</param>
        /// <returns><c>true</c> if the conversion succeeded, <c>false</c> if it failed.</returns>
        public bool ConvertField(FieldInfo fieldInfo, out LSLType outType)
        {
            return Convert(fieldInfo.FieldType, out outType);
        }

        /// <summary>
        /// Converts the specified <see cref="PropertyInfo"/> into its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="fieldInfo">Runtime <see cref="PropertyInfo"/> to convert.</param>
        /// <param name="outType">Resulting <see cref="LSLType"/> from the conversion.</param>
        /// <returns><c>true</c> if the conversion succeeded, <c>false</c> if it failed.</returns>
        public bool ConvertProperty(PropertyInfo fieldInfo, out LSLType outType)
        {
            return Convert(fieldInfo.PropertyType, out outType);
        }
    }





    //this overrides the converters in the serializer at the class level
    [LSLLibraryDataSerializable(
        ReturnTypeConverter = typeof(MySimpleConverter),
        ParamTypeConverter = typeof(MySimpleConverter),
        ConstantTypeConverter = typeof(MySimpleConverter),
        ValueStringConverter = typeof(MyValueStringConverter)
        )]
    public class AttributeReflectionTestClass
    {

        class PreferedValueStringConverter : ILSLValueStringConverter
        {
            public bool ConvertProperty(PropertyInfo propertyInfo, LSLType constantType, object fieldValue, out string valueString)
            {
                valueString = fieldValue.ToString().ToUpper();
                return true;
            }

            public bool ConvertField(FieldInfo fieldInfo, LSLType constantType, object fieldValue, out string valueString)
            {
                valueString = fieldValue.ToString().ToUpper();
                return true;
            }
        }



        public AttributeReflectionTestClass()
        {

        }





        //The explicitly specified value string will be used to define
        //a value for this constant, even when an object instance
        //is passed to the class serializer.
        [LSLConstant(LSLType.Integer, ValueString = "5")]
        public int CONSTANT_X = 10;


        //This constant gets expanded into the generated source, and also generates a deprecation warning.
        [LSLConstant(LSLType.Integer, Expand = true, Deprecated = true)]
        static public int CONSTANT_Y = 10;


        //The serializer will read private properties and fields
        //The type and value string converters used here are the one's specified in the class attribute.
        [LSLConstant]
        static public string CONSTANT_B { private get { return "hello"; } set { value = ""; } }


        //ValueString is explicitly set so it will use that instead of the field value, no converters required.
        [LSLConstant(LSLType.String, ValueString = "sup")]
        static public string CONSTANT_TT { get; set; }



        //The converter on the property attribute will be used instead of the serializer's or the class level converter.
        //You can override type converters and value string converters at the member level.
        [LSLConstant(LSLType.Key, ValueStringConverter = typeof(PreferedValueStringConverter))]
        static public string CONSTANT_D { get { return "hello world"; } }


        //A function that explicitly returns LSLType.Float.
        //The class level parameter converter is used to convert the parameter types.
        [LSLFunction(LSLType.Float)]
        public string function(
            string arg1,
            string arg2,
            params string[] variadic)
        {
            return "";
        }


        //A function that explicitly returns LSLType.Float.
        //The member level parameter converter specified in the attribute is used to convert the parameter types.
        [LSLFunction(LSLType.Float, ParamTypeConverter = typeof(MySimpleConverter))]
        public string function2(
            string arg1,
            string arg2,
            params string[] variadic)
        {
            return "";
        }


        //The member level parameter converter's specified in the attribute are used to convert the return type and parameter types respectively.
        [LSLFunction(ReturnTypeConverter = typeof(MySimpleConverter), ParamTypeConverter = typeof(MySimpleConverter))]
        public string function3(
            string arg1,
            string arg2,
            params string[] variadic)
        {
            return "";
        }



        //This explicitly defines everything, it overrides any conversion
        //that may take place, its also the way you would attribute your methods 
        //if you do not wish to use converters at all.
        [LSLFunction(LSLType.Float)]
        public string function(
            [LSLParam(LSLType.Integer)] int arg1,
            [LSLParam(LSLType.String)] string arg2,
            [LSLParam(LSLType.Integer)] params int[] variadic)
        {
            return "";
        }

        //A function that explicitly returns LSLType.Float.
        //The class level parameter converter is used to convert the first parameter
        //since it has no attribute, the rest have their LSLType's explicitly defined.
        [LSLFunction(LSLType.Float)]
        public string function(
            int arg1,
            [LSLParam(LSLType.String)] string arg2,
            [LSLParam(LSLType.Void)] params object[] variadic)
        {
            return "";
        }


        //A mod invoke function.  Also, the class converters can convert the return type and parameter
        //types for us since we did not specify them explicitly here.
        //
        //This function is also marked as deprecated, so it generates a warning when you use it from LSL.
        [LSLFunction(ModInvoke = true, Deprecated = true)]
        public object[] myModuleFunction(string param)
        {
            return new object[0];
        }

    }



    /// <summary>
    /// Currently a scratch pad area
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {


            var x = new LSLLibraryDataReflectionSerializer
            {
                PropertyBindingFlags =
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly,

                FieldBindingFlags =
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly,

                MethodBindingFlags = 
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly,

                //a fall back value string converter used by the serializer
                //or null.. not really used since the class we are reflecting
                //defines all of its converters explicitly at the class level via the LSLLibraryDataSerializable
                //
                //This is just demonstrating that you can have a fall back.
                //
                //It goes to say, if any converter in the serializer is null, and its actually required
                //to convert a type because a class or class member does not explicitly attribute itself
                //then the serializer will throw an informative exception about what member needed a converter
                //but could not find one to use.
                ValueStringConverter = new MyValueStringConverter(),

                //a fall back constant type converter used by the serializer when nothing else is available
                //or null..
                ConstantTypeConverter = new MySimpleConverter(),

                //fall back return type converter
                ReturnTypeConverter = new MySimpleConverter(),

                //fall back parameter type converter
                ParamTypeConverter = new MySimpleConverter(),


                //we have plenty of converters set up, might as well
                //serialize all the un-attributed methods as well?
                //sure, if you want.
                AttributedMethodsOnly = false,


                //Constants too.
                AttributedConstantsOnly = false,

                //we can converter non attributed parameter types, for sure.
                //don't exclude them from the de-serialized signatures.
                AttributedParametersOnly = false,

                
            };



            //a provider with no live filtering enabled, only load functions with the subset "my-lsl"
            LSLLibraryDataProvider myProvider = new LSLLibraryDataProvider(new [] {"my-lsl"}, false);



            //declare the subset my-lsl for use.
            myProvider.AddSubsetDescription(new LSLLibrarySubsetDescription("my-lsl", "My LSL Demo"));




            //define an event with no parameters, make sure its subsets are set so that it gets put in the "my-lsl" subset.
            myProvider.DefineEventHandler(new LSLLibraryEventSignature("my_event") {Subsets = { "my-lsl" } });

            myProvider.DefineEventHandler(new LSLLibraryEventSignature("my_deprecated_event") { Subsets = { "my-lsl" }, Deprecated = true });


            Console.WriteLine("Methods....\n\n");


            var methods = x.DeSerializeMethods(typeof(AttributeReflectionTestClass)).ToList();


            foreach (var c in methods)
            {
                //add this function to the my-lsl subset
                c.Subsets.Add("my-lsl");

                myProvider.DefineFunction(c);

                Console.WriteLine(c.ToString());
            }



            Console.WriteLine("\n\nWithout instance provided....\n\n");

            var constants = x.DeSerializeConstants(typeof (AttributeReflectionTestClass)).ToList();

            foreach (var c in constants)
            {
                //add this constant to the my-lsl subset

                c.Subsets.Add("my-lsl");

                myProvider.DefineConstant(c);

                Console.WriteLine(c.ToString());
            }


            Console.WriteLine("\n\nWith instance provided....\n\n");

            constants = x.DeSerializeConstants(typeof(AttributeReflectionTestClass), new AttributeReflectionTestClass()).ToList();

            foreach (var c in constants)
            {
                //Don't add anything to the provider here.  Just demonstrating the difference between
                //serializing classes that contain static or non-static fields when providing an object instance
                //to the serializer.
                Console.WriteLine(c.ToString());
            }




            //Set up the implementations LSLCodeValidator relies on's
            var validatorServices = new LSLValidatorServiceProvider();

            validatorServices.ExpressionValidator = new LSLDefaultExpressionValidator();
            validatorServices.StringLiteralPreProcessor = new LSLDefaultStringPreProcessor();

            //These both print to stdout by default.
            validatorServices.SyntaxErrorListener = new LSLDefaultSyntaxErrorListener();
            validatorServices.SyntaxWarningListener = new LSLDefaultSyntaxWarningListener();

            //Use ours, we only defined a few things
            validatorServices.LibraryDataProvider = myProvider;


            Console.WriteLine("\n\nSyntax Checking Demo Code...\n\n");

            LSLCodeValidator validator = new LSLCodeValidator(validatorServices);

                   

            StringReader strReader = new StringReader(
 @"

default{

    my_event(){
            
            //Valid to call AttributeReflectionTestClass.function(int arg1, string arg2, params int[] variadic);
            //No syntax errors, the function is overloaded and also variadic.
            
            string testStr = ""hello world"";

            integer i = 0;
            for(; i < CONSTANT_X ; i++)
            {
                //reference a deprecated constant, causes a warning
                integer expand = CONSTANT_Y;

                function(expand, testStr , 1,2,3,4,5);

                list testList = myModuleFunction(""hi there"");
            }

    }


    my_deprecated_event(){
        //reference a deprecated event, causes a deprecation warning.

    }

}
");


            ILSLCompilationUnitNode tree = validator.Validate(strReader);

            //Validate returns null if a syntax error was encountered.
            //I do not want users to visit a tree with syntax errors.
            //
            //The 'IsError' property that tree node interfaces have is only
            //to propagate source code errors up to the root node of the syntax tree,
            //so that the validator can tell if it needs to return null or not.
            //
            if (tree != null)
            {
                Console.WriteLine("\n\nNo syntax errors!\n\n");

                var compilerSettings = new LSLOpenSimCompilerSettings();

                compilerSettings.InsertCoOpTerminationCalls = true;
                compilerSettings.GenerateClass = true;
                compilerSettings.GeneratedClassName = "MyClass";
                compilerSettings.GeneratedConstructorSignature = "()";
                compilerSettings.GeneratedClassNamespace = "MyNameSpace";

                LSLOpenSimCompiler compiler = new LSLOpenSimCompiler(myProvider, compilerSettings);


                MemoryStream memStream = new MemoryStream();
                var memWriter = new StreamWriter(memStream);

                compiler.Compile(tree, memWriter);


                Console.WriteLine(Encoding.Default.GetString(memStream.ToArray()));
            }

        }
    }
}
