using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
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


                ValueStringConverter = new MyValueStringConverter(),

                ConstantTypeConverter = new MySimpleConverter(),

                ParamTypeConverter = new MySimpleConverter()
            };


            Console.WriteLine("Methods....\n\n");

            var methods = x.DeSerializeMethods(typeof(AttributeReflectionTest)).ToList();

            foreach (var c in methods)
            {
                Console.WriteLine(c.ToString());
            }



            Console.WriteLine("\n\nWithout instance provided....\n\n");

            var constants = x.DeSerializeConstants(typeof (AttributeReflectionTest)).ToList();

            foreach (var c in constants)
            {
                Console.WriteLine(c.ToString());
            }


            Console.WriteLine("\n\nWith instance provided....\n\n");

            constants = x.DeSerializeConstants(typeof(AttributeReflectionTest), new AttributeReflectionTest()).ToList();

            foreach (var c in constants)
            {
                Console.WriteLine(c.ToString());
            }
        }
    }


    //this overrides the converter in the serializer at the class level
    [LSLLibraryDataSerializable(ReturnTypeConverter = typeof(MySimpleConverter))]
    public class AttributeReflectionTest
    {

        class PreferedValueStringConverter : ILSLValueStringConverter
        {
            public bool Convert(LSLType constantType, object fieldValue, out string valueString)
            {
                valueString = fieldValue.ToString().ToUpper();
                return true;
            }
        }



        public AttributeReflectionTest()
        {
            
        }



        public int CONSTANT_A = 5;


        //if we don't provide an instance, this will be given a default value because its not static or constant
        [LSLConstant(LSLType.Integer, ValueString = "5")]
        public int CONSTANT_X = 5;



        //the serializer will read private properties and fields
        [LSLConstant]
        static public string CONSTANT_B { private get { return "hello"; } set { value = ""; } }


        //ValueString is explicitly set so it will use that instead of the field value, no converters required.
        [LSLConstant(LSLType.String, ValueString = "sup")]
        static public string CONSTANT_TT { get; set;}



        //the converter on the property attribute will be used instead of the serializer's or class level converter.
        //you can override type converters and value string converters at the class and member level.
        [LSLConstant(LSLType.Key, ValueStringConverter = typeof(PreferedValueStringConverter))]
        static public string CONSTANT_D { get { return "hello world"; }  }


        [LSLFunction(LSLType.Float)]
        public string function(
            string arg1,
            string arg2,
            params string[] variadic)
        {
            return "";
        }


        [LSLFunction(LSLType.Float)]
        public  string function(
            [LSLParam(LSLType.Integer)] int arg1,
            [LSLParam(LSLType.String)] string arg2,
            [LSLParam(LSLType.String)] params string[] variadic)
        {
            return "";
        }

        //uses the class ReturnTypeConverter, and the ParamTypeConverter on this method.
        [LSLFunction(ParamTypeConverter = typeof(MySimpleConverter))]
        public string function2(
            int arg1,
            string arg2,
            params string[] variadic)
        {
            return "";
        }

    }


}
