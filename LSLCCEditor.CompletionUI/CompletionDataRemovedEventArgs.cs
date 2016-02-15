using System;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace LSLCCEditor.CompletionUI
{
    public class CompletionDataRemovedEventArgs : EventArgs
    {
        public CompletionDataRemovedEventArgs(ICompletionData completionData)
        {
            CompletionData = completionData;
        }


        public ICompletionData CompletionData { get; private set; }
    }
}