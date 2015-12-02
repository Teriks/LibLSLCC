using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibLSLCC.Settings
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class MemberClonerAttribute : Attribute
    {
        public Type ClonerType { get; private set; }
        public MemberClonerAttribute(Type cloner)
        {
            ClonerType = cloner;
        }

    }
}
