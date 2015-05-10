using System;

namespace LibLSLCC.CodeValidator.Enums
{
    /// <summary>
    ///     Enum representing LSL's rotation type components
    /// </summary>
    public enum LSLRotationComponent 
    {
        X,
        Y,
        Z,
        S
    }


    public static class LSLRotationComponentTools
    {
        public static string ToComponentName(this LSLRotationComponent component)
        {
            return component.ToString().ToLower();
        }



        public static LSLRotationComponent ParseComponentName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }


            switch (name)
            {
                case "x":
                    return LSLRotationComponent.X;
                case "y":
                    return LSLRotationComponent.Y;
                case "z":
                    return LSLRotationComponent.Z;
                case "s":
                    return LSLRotationComponent.S;
            }

            throw new ArgumentException(
                string.Format("Could not parse \"{0}\" into an LSLRotationComponent, invalid name", name), "name");
        }
    }
}