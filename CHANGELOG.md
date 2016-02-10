# (1/26/2016 ...) Event handler cleanup / URL colors

Cleaned up event handler parameter names so that they formatted more traditionally when generating handlers using auto complete.

They were previously capitalized due to the source the library data was scraped from.

Changing the highlighting color of URL's in source code from the theme editor pane is now possible.

Its at: `Settings -> Editor Theme -> Syntax Highlighting / Editor Color -> Urls`


=======


# (1/31/2016 ...) Fix to code formatter

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


# (1/31/2016 ...) Code generation change

Remove unnecessary code generation for implicit boolean conversions of vector/rotation/list and string


=======


# (2/4/2016 7:44AM) Fix improper code generation issue


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

# (2/4/2016 11:44PM) Give logical AND and OR equal precedence

In Linden LSL `(0 && 0 || 1) == TRUE`, and `(1 || 0 && 0) == FALSE`

This seems to imply that whichever operator appears first has precedence.

&& and || have been given equal precedence to achieve this behavior.


# Adjust code generation for boolean conversion of strings in condition statements

Added back `UTILITES.ToBool` to generated code for string expressions in conditional statements.
Because some string constants may be defined as a CSharp built in string instead of an OpenSim runtime string, which cannot implicitly convert to bool.

=======

# (2/5/2016 11:58AM) Make modifiable LValues part of grammar

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


# Reduced the size of generated OpenSim code

The variable name mangling scheme has been changed so that symbol names are shorter.

Binary operator stub's also have a new naming scheme.


# Fix issue with incorrectly detecting invalid escape codes in strings

Behavior has been changed to simply replace the invalid escape code with the code 
character that comes after the slash, as in Linden LSL.


=======


# (2/7/2016 4:26AM) Factor grammar to remove control statement ambiguity


A problem with the parser grammar for control statements was causing large performance issues when parsing long
if-elseif-else chains, sometimes to the point of putting the parser in an infinite loop.

This has been fixed by simplifying the grammar and refactoring the code validator class and autocomplete parser.

The abstracted tree LibLSLCC produces still retains the same structure.


========


# (2/8/2016 2:58AM) Added built in input file globbing to lslcc_cmd

Added built in file globbing and the ability to specify
multiple input files or glob expressions at once when invoking lslcc.exe.

Output file name templating, along with the capability to log
to a single file or multiple files (one per input file) has been added.

See lslcc.exe -h for details on how to use these.

This was added mostly for running the compiler over a ton of
files at once for testing purposes.

==

Also fixed an issue with LSLCCEditor's application settings.

Carriage return sequences in XML text content were being
converted into plain newlines upon saving the settings file.

This no longer happens, as carriage returns are now turned into
XML entities by the settings serializer when settings are persisted.

The application settings fix really only affected how script headers
under the compiler settings menu were formatted in generated code.


# (2/9/2016 4:38AM) Re-Implement internal LSLLabelCollectorPrePass

This class was not fixed properly after large changes to the ANTLR grammar.

I left it in a state that was breaking return path analysis pretty badly.  oops.

I commented out the visit hooks that were removed by the grammar changes and then forgot to fix it.

Return path analysis for jumps and labels was broken because this pre-pass visitor 
was not defining proper statement indices and code scope ID's for labels used inside of 
events and functions.


# Fixed code generation for integer/hex literals that overflow/underflow LSL's integer type

Fixed an issue with the code generation behavior for integer/hex literals that are too
large/small to be contained in a signed 32 bit LSL integer.

Prior to this fix, literals that overflowed would be compiled as is, and then interpreted
as .NET's long data type.  This was causing C# compiler errors when a literal such as this
was passed into a function or operator that only expected an Int32 type.

Integer and Hex literals that overflow/underflow LSL's integer type are now compiled as -1
just like in the Linden compiler.

A new compiler warning is issued when this happens.


# (2/9/2016 9:32 PM) Allow returning values from event handlers, with warning 
 
The Linden compiler allows you to return a value from an event handler, LibLSLCC previously  
deemed this a syntax error. 
 
The behavior is now to issue a warning about it, C# code generation has been adjusted so that 
the expression of such a return value is evaluated, but discarded. 
 
This is the same behavior that the Linden compiler uses. 
 
# Cleaned up the code formatting of C# code generated by the OpenSim compiler 
 
Previously, if a statement inside of a code scope was optimized away, a blank line 
would be generated in its place. 
 
The new behavior is just to omit the blank line where the statement would have been.