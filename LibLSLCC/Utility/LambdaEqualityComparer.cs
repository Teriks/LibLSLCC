using System;
using System.Collections.Generic;

namespace LibLSLCC.Utility
{
    public class LambdaEqualityComparer<T> : IEqualityComparer<T>
    {
        public  Func<T, int> Hash { get; set; }

        public LambdaEqualityComparer(Func<T, T, bool> cmp, Func<T, int> hash = null)
        {
            Hash = hash;
            Cmp = cmp;
        }

        public bool Equals(T x, T y)
        {
            return Cmp(x, y);
        }

        public int GetHashCode(T obj)
        {
            return Hash != null ? Hash(obj) : obj.GetHashCode();
        }

        public Func<T, T, bool> Cmp { get; set; }
    }
}