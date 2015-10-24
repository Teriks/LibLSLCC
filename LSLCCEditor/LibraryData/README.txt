
Any library data XML files you add to this directory will be loaded by the editor.

If there are syntax errors in the XML, or conflicts in the library data that are immediately obvious, the editor will tell you about them when it starts and then close.

A runtime error can occur if a duplicate definition is found when requesting to use one or more module subsets from the editor at the same time.
The editor will tell you the problem and exit when you try to use two library subsets from the 'Tab Library Data' menu that contain conflicting definitions.

The 'lsl' and 'os-lsl' subsets are considered mutually exclusive by the editor, these module subsets cannot be used at the same time.

All other subset names besides 'lsl' and 'os-lsl' are free to be loaded at the same time in the editor, you have to make sure there are no conflicting
definitions in any subsets that can possibly be loaded in the editor at the same time.