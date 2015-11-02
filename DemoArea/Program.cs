using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.Compilers;
using LibLSLCC.LibraryData;
using LibLSLCC.LibraryData.Reflection;

namespace Tests
{

    class MyValueStringConverter : ILSLValueStringConverter
    {
        /// <summary>
        /// Convert the value taken from a property or field with the <see cref="LSLConstantAttribute"/> into
        /// something that is valid to assign to <see cref="LSLLibraryConstantSignature.ValueString"/> given the specified
        /// <see cref="LSLType"/> that is to be assigned to <see cref="LSLLibraryConstantSignature.Type"/>.
        /// </summary>
        /// <param name="constantType">The <see cref="LSLType"/> being assigned to <see cref="LSLLibraryConstantSignature.Type"/>.</param>
        /// <param name="fieldValue">The value taking from the property or field with an <see cref="LSLConstantAttribute"/>.</param>
        /// <param name="valueString">
        /// The string to assign to <see cref="LSLLibraryConstantSignature.ValueString"/>.
        /// this should be a string that <see cref="LSLLibraryConstantSignature"/> is able to parse for the given <see cref="LSLType"/>.
        /// You should not assign <c>null</c> to <paramref name="valueString"/> if you intend to return <c>true</c>, this is invalid and the serializer will throw an exception.
        /// </param>
        /// <returns>
        /// True if the conversion succeeded, false if it did not.
        /// </returns>
        public bool Convert(LSLType constantType, object fieldValue, out string valueString)
        {
            valueString = fieldValue.ToString();
            return true;
        }
    }


    class MySimpleConverter : ILSLTypeConverter
    {
        /// <summary>
        /// Converts the specified <see cref="Type"/> into its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="inType">Runtime <see cref="Type"/> to convert.</param>
        /// <param name="outType">Resulting <see cref="LSLType"/> from the conversion.</param>
        /// <returns><c>true</c> if the conversion succeeded, <c>false</c> if it failed.</returns>
        public bool Convert(Type inType, out LSLType outType)
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
    }


    //this overrides the converter in the serializer at the class level
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
            public bool Convert(LSLType constantType, object fieldValue, out string valueString)
            {
                valueString = fieldValue.ToString().ToUpper();
                return true;
            }
        }



        public AttributeReflectionTestClass()
        {

        }





        //if we don't provide an instance, the ValueString will be used.
        [LSLConstant(LSLType.Integer, ValueString = "5")]
        public int CONSTANT_X = 5;



        //the serializer will read private properties and fields
        [LSLConstant]
        static public string CONSTANT_B { private get { return "hello"; } set { value = ""; } }


        //ValueString is explicitly set so it will use that instead of the field value, no converters required.
        [LSLConstant(LSLType.String, ValueString = "sup")]
        static public string CONSTANT_TT { get; set; }



        //the converter on the property attribute will be used instead of the serializer's or class level converter.
        //you can override type converters and value string converters at the class and member level.
        [LSLConstant(LSLType.Key, ValueStringConverter = typeof(PreferedValueStringConverter))]
        static public string CONSTANT_D { get { return "hello world"; } }


        [LSLFunction(LSLType.Float)]
        public string function(
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


        [LSLFunction(LSLType.Float)]
        public string function(
            int arg1,
            [LSLParam(LSLType.String)] string arg2,
            [LSLParam(LSLType.Void)] params object[] variadic)
        {
            return "";
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
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,

                FieldBindingFlags =
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,

                MethodBindingFlags = 
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,

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

                
            };



            //a provider with no live filtering enabled, only load functions with the subset "my-lsl"
            LSLLibraryDataProvider myProvider = new LSLLibraryDataProvider(new [] {"my-lsl"}, false);





           
            //declare the subset my-lsl for use.
            myProvider.AddSubsetDescription(new LSLLibrarySubsetDescription("my-lsl", "My LSL Demo"));




            //define an event with no parameters, make sure its subsets are set so that it gets put in the "my-lsl" subset.
            myProvider.DefineEventHandler(new LSLLibraryEventSignature("my_event") {Subsets = { "my-lsl" } });



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
                //don't add anything here to the provider, just demonstrating the difference between
                //serializing classes with non-static fields with and without providing the serializer a class instance.
                Console.WriteLine(c.ToString());
            }




            //set up the implementations LSLCodeValidator relies on's
            var validatorServices = new LSLCustomValidatorServiceProvider();

            validatorServices.ExpressionValidator = new LSLDefaultExpressionValidator();
            validatorServices.StringLiteralPreProcessor = new LSLDefaultStringPreProcessor();

            //these both print to stdout by default.
            validatorServices.SyntaxErrorListener = new LSLDefaultSyntaxErrorListener();
            validatorServices.SyntaxWarningListener = new LSLDefaultSyntaxWarningListener();

            //use ours, we only defined a few things
            validatorServices.LibraryDataProvider = myProvider;





            Console.WriteLine("\n\nSyntax Checking Demo Code...\n\n");

            LSLCodeValidator validator = new LSLCodeValidator(validatorServices);

                   

            StringReader strReader = new StringReader(
 @"

default{

    my_event(){
            
            //Valid to call AttributeReflectionTestClass.function(int arg1, string arg2, params int[] variadic);
            //No syntax errors, the function is overloaded and also variadic.
            
            string test = ""hello world"";

            integer i = 0;
            for(;i<100;i++)
            {
                function(0, test , 1,2,3,4,5);
            }

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

                var compilerSettings = new LSLOpenSimCSCompilerSettings(myProvider);

                compilerSettings.InsertCoOpTerminationCalls = true;
                compilerSettings.GenerateClass = true;
                compilerSettings.GeneratedClassName = "MyClass";
                compilerSettings.GeneratedConstructorDefinition = "public MyClass(){}";
                compilerSettings.GeneratedClassNamespace = "MyNameSpace";

                LSLOpenSimCSCompiler compiler = new LSLOpenSimCSCompiler(compilerSettings);


                MemoryStream memStream = new MemoryStream();
                var memWriter = new StreamWriter(memStream);

                compiler.Compile(tree, memWriter);


                Console.WriteLine(Encoding.Default.GetString(memStream.ToArray()));
            }

        }
    }
}
