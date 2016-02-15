using System;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace LSLCCEditor.CompletionUI
{
    public class CompletionDataAddedEventArgs : EventArgs
    {
        public CompletionDataAddedEventArgs(ICompletionData completionData)
        {
            CompletionData = completionData;
        }


        public ICompletionData CompletionData { get; private set; }
    }
}