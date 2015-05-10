using System;

namespace LibLSLCC.CodeValidator.Enums
{
    /// <summary>
    ///     Enum representing LSL's vector type components
    /// </summary>
    public enum LSLVectorComponent 
    {
        X,
        Y,
        Z
    }


    public static class LSLVectorComponentTools
    {
        public static string ToComponentName(this LSLVectorComponent component)
        {
            return component.ToString().ToLower();
        }



        public static LSLVectorComponent ParseComponentName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }


            switch (name)
            {
                case "x":
                    return LSLVectorComponent.X;
                case "y":
                    return LSLVectorComponent.Y;
                case "z":
                    return LSLVectorComponent.Z;
            }

            throw new ArgumentException(
                string.Format("Could not parse \"{0}\" into an LSLVectorComponent, invalid name", name), "name");
        }
    }
}