using System;

namespace LibLSLCC.CodeValidator.Nodes.Interfaces
{
    public static class LSLLiteralNodeInterfaceExtensions
    {

        public static bool IsIntegerLiteralOverflowed(this ILSLIntegerLiteralNode node)
        {
            try
            {
                Convert.ToInt32(node.RawText);
                return false;
            }
            catch (OverflowException)
            {
                return true;
            }
        }

        public static bool IsHexLiteralOverflowed(this ILSLHexLiteralNode node)
        {
            try
            {
                Convert.ToInt32(node.RawText, 16);
                return false;
            }
            catch (OverflowException)
            {
                return true;
            }
        }
    }
}
