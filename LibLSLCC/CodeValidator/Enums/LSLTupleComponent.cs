using System;

namespace LibLSLCC.CodeValidator.Enums
{
    public enum LSLTupleComponent 
    {
        X,
        Y,
        Z,
        S
    }


    public static class LSLTupleComponentTools
    {
        public static string ToComponentName(this LSLTupleComponent component)
        {
            return component.ToString().ToLower();
        }



        public static LSLTupleComponent ParseComponentName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            switch (name)
            {
                case "x":
                    return LSLTupleComponent.X;
                case "y":
                    return LSLTupleComponent.Y;
                case "z":
                    return LSLTupleComponent.Z;
                case "s":
                    return LSLTupleComponent.S;
            }

            throw new ArgumentException(
                string.Format("Could not parse \"{0}\" into an LSLVectorComponent, invalid name", name), "name");
        }
    }
}