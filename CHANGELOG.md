# (1/26/2016 ...)<br/> Event handler cleanup / URL colors

Cleaned up event handler parameter names so that they formatted more traditionally when generating handlers using auto complete.

They were previously capitalized due to the source the library data was scraped from.

Changing the highlighting color of URL's in source code from the theme editor pane is now possible.

Its at: `Settings -> Editor Theme -> Syntax Highlighting / Editor Color -> Urls`


=======


# (1/31/2016 ...)<br/> Fix to code formatter

Fixed an issue with formatting braceless conditional/loop expressions containing comments before the
single statement that makes up the code body.

Previously code such as this:


```LSL

if(TRUE)
    //test
    llOwnerSay("test");

```

Was being formatted like this, when the 'Indent Braceless Control Statements'
option was disabled in the formatter settings:


```LSL

if(TRUE) //test llOwnerSay("test");

```

This is of course a syntax error,  braceless expressions with any sort of comments
preceding the body expression are now forcefully indented regardless of the indent
settings for braceless control/loop statements.


=======


# (1/31/2016 ...)<br/> Code generation change

Remove unnecessary code generation for implicit boolean conversions of vector/rotation/list and string.


=======


# (2/4/2016 7:44AM)<br/> Fix improper code generation issue


Expression statements such as:

~(x=0);

-(x=0);

(unary op) (expression);


and:


(x=3);

(basically any expression in parenthesis as statement);


Were not being accounted for when compiling to CSharp.

CSharp does not allow anything but variable declarations,
variable assignments/modifications, and method calls to be used
as a standalone statement.  You cannot have a unary expression or 
parenthesized expression be a standalone statement.

Therefore these valid LSL expressions must be sanitized when compiling.

This was fixed simply by forcing them to become an argument to a generic
function that does nothing.

public static void ForceStatement<T>(T value){}


=======

# (2/4/2016 11:44PM)<br/> Give logical AND and OR equal precedence

In Linden LSL `(0 && 0 || 1) == TRUE`, and `(1 || 0 && 0) == FALSE`

This seems to imply that whichever operator appears first has precedence.

&& and || have been given equal precedence to achieve this behavior.

===

Adjust code generation for boolean conversion of strings in condition statements

Added back `UTILITES.ToBool` to generated code for string expressions in conditional statements.
Because some string constants may be defined as a CSharp built in string instead of an OpenSim runtime string, which cannot implicitly convert to bool.

=======

# (2/5/2016 11:58AM)<br/> Make modifiable LValues part of grammar

Having:

`expression '=' expression`

And:

`expression '*=' expression`

Etc..

In the expression grammar is causing strange parse trees.



Factoring it down to:

```

lvalue:  memberAccess | ID;

expression: ... expression stuff ...

    ...
    
    | lvalue ALL_MOD_ASSIGN_OPS expression
    | lvalue '=' expression
    
	
```

Helps monumentally...


This also happens to fix the precedence problem with expressions such as `-x = y`.


The downside to this is that assignments to non lvalues will
now result in a non-pretty grammar level syntax error.


The upside is that it fixes several operator precedence issues that
cause incompatibilities with SecondLife,  I currently do not know of any more (yet).

===

Reduced the size of generated OpenSim code.

The variable name mangling scheme has been changed so that symbol names are shorter.

Binary operator stub's also have a new naming scheme.

===

Fix issue with incorrectly detecting invalid escape codes in strings.

Behavior has been changed to simply replace the invalid escape code with the code 
character that comes after the slash, as in Linden LSL.


=======


# (2/7/2016 4:26AM)<br/> Factor grammar to remove control statement ambiguity


A problem with the parser grammar for control statements was causing large performance issues when parsing long
if-elseif-else chains, sometimes to the point of putting the parser in an infinite loop.

This has been fixed by simplifying the grammar and refactoring the code validator class and autocomplete parser.

The abstracted tree LibLSLCC produces still retains the same structure.


========


# (2/8/2016 2:58AM)<br/> Added built in input file globbing to lslcc_cmd

Added built in file globbing and the ability to specify
multiple input files or glob expressions at once when invoking lslcc.exe.

Output file name templating, along with the capability to log
to a single file or multiple files (one per input file) has been added.

See lslcc.exe -h for details on how to use these.

This was added mostly for running the compiler over a ton of
files at once for testing purposes.

===

Also fixed an issue with LSLCCEditor's application settings.

Carriage return sequences in XML text content were being
converted into plain newlines upon saving the settings file.

This no longer happens, as carriage returns are now turned into
XML entities by the settings serializer when settings are persisted.

The application settings fix really only affected how script headers
under the compiler settings menu were formatted in generated code.


# (2/9/2016 4:38AM)<br/> Re-Implement internal LSLLabelCollectorPrePass

This class was not fixed properly after large changes to the ANTLR grammar.

I left it in a state that was breaking return path analysis pretty badly.  oops.

I commented out the visit hooks that were removed by the grammar changes and then forgot to fix it.

Return path analysis for jumps and labels was broken because this pre-pass visitor 
was not defining proper statement indices and code scope ID's for labels used inside of 
events and functions.

===

Fixed code generation for integer/hex literals that overflow/underflow LSL's integer type.

Fixed an issue with the code generation behavior for integer/hex literals that are too
large/small to be contained in a signed 32 bit LSL integer.

Prior to this fix, literals that overflowed would be compiled as is, and then interpreted
as .NET's long data type.  This was causing C# compiler errors when a literal such as this
was passed into a function or operator that only expected an Int32 type.

Integer and Hex literals that overflow/underflow LSL's integer type are now compiled as -1
just like in the Linden compiler.

A new compiler warning is issued when this happens.


# (2/9/2016 9:32 PM)<br/> Allow returning values from event handlers, with warning 
 
The Linden compiler allows you to return a value from an event handler, LibLSLCC previously  
deemed this a syntax error. 
 
The behavior is now to issue a warning about it, C# code generation has been adjusted so that 
the expression of such a return value is evaluated, but discarded. 
 
This is the same behavior that the Linden compiler uses. 
 
===
 
Cleaned up the code formatting of C# code generated by the OpenSim compiler.
 
Previously, if a statement inside of a code scope was optimized away, a blank line 
would be generated in its place. 
 
The new behavior is to omit the blank line where the statement would have been.

# (2/11/2016 9:32 PM)<br/> Clean up all syntax tree node implementations

LibLSLCC's syntax tree nodes no longer hold any references to the ANTLR tree nodes
they abstract.  This allows for complete garbage collection of the ANTLR tree once 
LibLSLCC is done building its own abstracted tree.


# (2/11/2016 11:46 PM)<br/> Fix many code analysis warnings, suppress some

Just a large code cleanup in general.  A few significant public API changes.
This is why I am still not versioning yet, getting close though.


# (1.0.0.199)<br/> Move to v1 and versioning.

This release of LibLSLCC contains large changes in namespace organization,
as well as many tweaks to the general public interface and syntax tree class hierarchy.

This is the result of a lot of refactoring and bug fixes over a period of three months,
to both the compiler/parsing library and the editor itself.

The editor UI now looks uniform across different versions of Windows.

The auto-complete parser has been nearly re-written, as well as the code formatter;
which has several new formatting options.

The library data distributed with the editor has been updated so that Linden LSL and OpenSim LSL
have differing return-types, and constant values where necessary. This mostly affects functions that
return key's in Secondlife but return string's in OpenSim instead.

This also affects the value of some string constants such as the JSON_ constants.

===

The entire library has been documented and de-warning-ified.

The XML documentation file for IntelliSense is now properly named and placed next to the LibLSLCC
binaries in the released zip file, so that documentation tooltips in VisualStudio work for the library.

There are also Doxygen and Sandcastle documentation generator settings/projects in the LibLSLCC
source tree.

===

All pre-versioning tags except the last one before version one have been deleted from the repo.

That repo history is kinda meaningless to me at this point, and it sorts incorrectly in some cases
due to the non semantic versioning I was using while working up to something I was comfortable having people depend on.

Versions for assemblies will now be handled as follows:

Major Increment: public API changes
Minor Increment: backwards compatible features
Patch Increment: significant bug fixes
Revision Increment: on commit affecting the assembly

Each assembly and executable has its own version.

The editor installer version is taken from the editor executable, and the zip file containing the LibLSLCC 
library and lslcc command line compiler has it's name derived from LibLSLCC's assembly version.


# (1.0.1.1)<br/> Build system fixes, build Warnings on linux resolved.


Build system fixes, MonoDevelop was having issues opening the no-editor solution.

The ToolsVersion in the msbuild file of a few projects mismatched the solution file causing it to complain.

Fixed A few crefs in XML comments that were causing warnings due to having space after the type referenced.

===

version.py build script exception fixed on Linux.

'subprocess.check_output' was being passed a full command instead of a list with the command and it's arguments; that does not work on Linux but it does on Windows.

Reference to non-existent git tag in version.json fixed, revision number is now commits since tag "1.0.0.199" for every assembly.


# (1.0.2.1)<br/> Bug fix to hex literal handling.

Fix incorrect hex literal overflow detection.

Issue was more apparent on Linux than Windows, causing it to slip through.


# (1.0.3.1)<br/> New editor 'edit' menu, fixes to library data and versioning scheme.


Added standard 'Edit' menu to editor.

It contains the usual Undo, Redo, Cut, Copy, Paste, Delete and 'Select All' in button form.

New feature is the reason for the minor version increment on the editor.

There was also a fix to the text editing control for an out of bounds error that could occur while starting to type with the editor completely empty.

===

Fixed an issue with the distributed library data which affected the 'OS Attach Temp' module that is selectable from the editor.

Having it selected while in 'OpenSim LSL' mode had no effect, due to it not belonging to the required library subset 'os-attach-temp'

This meant that llAttachToAvatarTemp was not actually made available by having this module selected in OpenSim mode.

This also affected the command line compiler, which got a patch increment along with LibLSLCC.

===

Revision numbers for each assembly are now calculated by the number of commits to the entire project tree since there was a change to the Major, Minor or Patch number of the specific assembly.

This is so the revision number in the tag always increments even when LibLSLCC received no changes in a new release.

It has to be this way because tag names are derived from the current version of the LibLSLCC assembly. If there are changes elsewhere that do not effect it, or any assembly at for that matter, the tag still needs to increment.


# (1.0.4.29)<br/> Fix handling of float literal overflows/underflows, library data corrections

Float literal overflows/underflows now generate a warning, underflows compile to exactly '0.0' and overflows compile to float.PositiveInfinity.

Various problems with library data fixed.

A few missing constants were added and constants with incorrectly documented values were corrected.

LibraryDataScrapingTool now pulls constants from the specified viewer keywords file instead of the LSL wiki.




