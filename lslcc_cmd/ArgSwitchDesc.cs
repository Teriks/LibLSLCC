using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace lslcc
{
    internal sealed class ArgSwitchDesc
    {
        private readonly string _name;
        private Collection<string> _descriptionLines = new Collection<string>();
        private HashSet<string> _cantBeUsedWith = new HashSet<string>();

        public string Name
        {
            get { return _name; }
        }

        public HashSet<string> CantBeUsedWith
        {
            get { return _cantBeUsedWith; }
            set { _cantBeUsedWith = value; }
        }


        public bool MustBeUsedAlone { get; set; }

        public bool MustAppearOnlyOnce { get; set; }

        public int MinArgs { get; set; }
        public int MaxArgs { get; set; }

        public string HelpLine { get; set; }

        public Collection<string> DescriptionLines
        {
            get { return _descriptionLines; }
            set { _descriptionLines = value; }
        }

        public event EventHandler<ArgSwitchHelpWriteEvent> WriteBeforeShortHelp;


        public ArgSwitchDesc AddWriteBeforeShortHelp(EventHandler<ArgSwitchHelpWriteEvent> handler)
        {
            WriteBeforeShortHelp += handler;
            return this;
        }


        public event EventHandler<ArgSwitchHelpWriteEvent> WriteAfterShortHelp;

        public ArgSwitchDesc AddWriteAfterShortHelp(EventHandler<ArgSwitchHelpWriteEvent> handler)
        {
            WriteAfterShortHelp += handler;
            return this;
        }

        public event EventHandler<ArgSwitchHelpWriteEvent> WriteBeforeDescription;

        public ArgSwitchDesc AddWriteBeforeDescription(EventHandler<ArgSwitchHelpWriteEvent> handler)
        {
            WriteBeforeDescription += handler;
            return this;
        }

        public event EventHandler<ArgSwitchHelpWriteEvent> WriteAfterDescription;

        public ArgSwitchDesc AddWriteAfterDescription(EventHandler<ArgSwitchHelpWriteEvent> handler)
        {
            WriteAfterDescription += handler;
            return this;
        }

        public ArgSwitchDesc(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name cannot be null or whitespace.", "name");
            }


            _name = name;
            MustAppearOnlyOnce = true;
            MinArgs = -1;
            MaxArgs = -1;
        }

        
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }


        public override bool Equals(object obj)
        {
            var o = obj as ArgSwitchDesc;
            if (o == null) return false;

            return o.Name == Name;
        }


        internal void OnWriteBeforeShortHelp(ArgSwitchHelpWriteEvent e)
        {
            var handler = WriteBeforeShortHelp;
            if (handler != null) handler(this, e);
        }


        internal void OnWriteAfterShortHelp(ArgSwitchHelpWriteEvent e)
        {
            var handler = WriteAfterShortHelp;
            if (handler != null) handler(this, e);
        }


        internal void OnWriteBeforeDescription(ArgSwitchHelpWriteEvent e)
        {
            var handler = WriteBeforeDescription;
            if (handler != null) handler(this, e);
        }


        internal void OnWriteAfterDescription(ArgSwitchHelpWriteEvent e)
        {
            var handler = WriteAfterDescription;
            if (handler != null) handler(this, e);
        }
    }
}