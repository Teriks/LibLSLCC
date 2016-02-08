# (1/26/2016 ...) Event handler cleanup / URL colors

Cleaned up event handler parameter names so that they formatted more traditionally when generating handlers using autocomplete.

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


# (2/8/2016) Add built in input file globbing to lslcc_cmd

Added built in file globbing and the ability to specify
multiple input files or glob expressions at a time.

Also added output file name templating and options
for loging to a single file, or multiple files for each
input file.

see lslcc.exe -h for details

==

This commit also fixes an issue with LSLCCEditors application
settings.  Carriage return sequences in xml node content were being
converted into newlines upon saving the settings file, this no
longer happens.