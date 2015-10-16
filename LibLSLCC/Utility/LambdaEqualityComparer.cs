using System;
using System.Collections.Generic;

namespace LibLSLCC.Utility
{
    public class LamdaEqualityComparer<T> : IEqualityComparer<T>
    {
        public  Func<T, int> Hash { get; set; }

        public LamdaEqualityComparer(Func<T, T, bool> cmp, Func<T, int> hash = null)
        {
            Hash = hash;
            this.Cmp = cmp;
        }

        public bool Equals(T x, T y)
        {
            return Cmp(x, y);
        }

        public int GetHashCode(T obj)
        {
            if (Hash != null) return Hash(obj);
            return obj.GetHashCode();
        }

        public Func<T, T, bool> Cmp { get; set; }
    }
}