using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace lslcc
{
    internal class ArgSwitch
    {
        public ArgSwitch(string name, IList<string> arguments)
        {
            Arguments = new Collection<string>(arguments);
            Name = name;
        }
        public ArgSwitch(string name)
        {
            Arguments = new Collection<string>();
            Name = name;
        }

        public ArgSwitch()
        {
            Arguments = new Collection<string>();
            Name = "";
            IsProgramArgument = true;
        }

        public bool IsProgramArgument { get; private set; }
        public string Name { get; private set; }
        public Collection<string> Arguments { get; private set; }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }


        public override bool Equals(object obj)
        {
            var o = obj as ArgSwitch;
            if (o == null) return false;

            return o.Name == Name;
        }
    }
}