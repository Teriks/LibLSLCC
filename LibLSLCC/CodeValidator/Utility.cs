using System.Linq;

namespace LibLSLCC.CodeValidator
{
    internal static class Utility
    {
        public static bool AnyNull(params object[] vars)
        {
            return vars.Any(v => v == null);
        }



        public static bool OnlyOneNotNull(params object[] vars)
        {
            var oneNotNull = false;
            foreach (var x in vars)
            {
                if (x != null)
                {
                    if (oneNotNull)
                    {
                        return false;
                    }

                    oneNotNull = true;
                }
            }

            return oneNotNull;
        }



        public static bool EqualsOneOf(this object i, params object[] vars)
        {
            if (vars.Contains(i))
            {
                return true;
            }
            return false;
        }
    }
}