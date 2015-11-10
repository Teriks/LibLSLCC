using LibLSLCC.Utility;

namespace LibLSLCC.Compilers
{
    public class NameSpaceImport : SettingsBaseClass
    {
        private string _name;

        public NameSpaceImport()
        {

        }

        public NameSpaceImport(string name)
        {
            Name = name;
        }

        public static implicit operator NameSpaceImport(string name)
        {
            return new NameSpaceImport(name);
        }

        public string Name
        {
            get { return _name; }
            set { SetField(ref _name,value, "Name"); }
        }


        public override string ToString()
        {
            return Name;
        }
    }
}