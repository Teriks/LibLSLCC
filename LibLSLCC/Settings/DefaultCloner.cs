using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibLSLCC.Settings
{
    public class DefaultCloner : ICloner
    {

        public object Clone(object instance)
        {
            var i = instance as ICloneable;
            if (i != null) return i.Clone();

            var methodInfo = instance.GetType().GetMethod("Clone",new Type[] {});
            if (methodInfo != null)
            {
                return methodInfo.Invoke(instance,new object[] {});
            }

            return instance;
        }
    }
}
