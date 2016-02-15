using System;
using System.IO;

namespace lslcc
{
    internal sealed class ArgSwitchHelpWriteEvent : EventArgs
    {
        public ArgSwitchHelpWriteEvent(TextWriter writer)
        {
            Writer = writer;
        }


        public TextWriter Writer { get; private set; }
    }
}