# Binary Releases

If you don't want to build the Library or Editor to use it,
I release the latest binaries for the project in each GitHub release.

https://github.com/Teriks/LibLSLCC/releases


Each release contains an AnyCPU Release/Debug version of LibLSLCC,
an AnyCPU binary copy of lslcc (The command line compiler), and an XML documentation 
file generated for the library by Visual Studio/MSBuild.

Releases also include platform specific installers for LSLCCEditor.

The only difference between each installer at this point is the program
install directory that they default too.  The installers for both x64 and x86
platforms have been changed to use platform independent AnyCPU binaries.


# About LibLSLCC 


LibLSLCC is a framework for writing LSL compilers.

The LibLSLCC code validator/syntax tree builder provides full front end syntax checking of LSL.
It even includes extended warnings that you would find in most compilers for other languages,
but that are not implemented in either the Linden compiler or current OpenSim compiler.


Warnings for things such as: 

 * Constant expressions in conditional/loop statements.

 * Un-used variables.

 * Dead code.

 * Deprecated function usage.


That's not everything, just some common ones; the list is pretty long.
I suggest you mess around with the editor releases to discover what all 
LibLSLCC can tell you about your code.


LibLSLCC can also be used for general purpose LSL parsing tasks, it provides
its own rich syntax tree that has been completely abstracted from ANTLR.

The syntax tree is tailored specifically for dealing with LSL, and there is an interface for every node 
so that you can implement your own code DOM to feed to compilers/formatters if you want to.


====


LibLSLCC includes a CSharp code generator that targets the OpenSim runtime.

The code validator (parser/tree builder) and OpenSim code generator in LibLSLCC have both been designed
with the intent of implementing Linden LSL with near 100 percent cross compatibility.

The project is basically a complete reverse engineering of the Linden compiler's
grammar, non grammar level syntax rules, and generated code behavior.



I have integrated LibLSLCC into OpenSim, see here:

https://github.com/Teriks/OpenSim_With_LibLSLCC 

Or:

https://gitlab.com/erihoss/OpenSim_With_LibLSLCC 


====

A few notable code validator/OpenSim code generator features:

 * Full front end Syntax Checking, including dead code detection.  
   Esoteric CSharp compiler errors and line mapping have been eliminated.

 * Dead code elimination from generated code where applicable.
   This includes un-used functions and global variables, as well
   as any dead code in a function/event body that does not cause a compile 
   error and is safe to remove given its context.

 * Correct code generation for global variables that reference each other
   while still maintaining full support for script resets in server generated code.

 * Symbol name mangling specific to globals/parameters/local variables/labels and 
   user defined functions.  This completely abstracts variable scoping rules from the CSharp 
   compiler underneath.  All variable scoping rules are handled by the front end LibLSLCC code validator
   and are implemented in a fashion that is true to Linden LSL.  Symbol mangling also removes the possibility 
   of causing a CSharp syntax error by using a keyword/Class name as a variable or function name.


 * Correct Code generation for jumps over variable declarations;  jumping over a variable declaration
   will leave said variable with a default value, instead of null.  Leaving these variables null is 
   something both the Linden compiler and current OpenSim compiler do, which is a step out of the LSL
   SandBox, as null variables are not supported elsewhere in LSL and generally not expected as a function parameter.

 * Labels no longer require a "NoOp()" after every label, the OpenSim code generator 
   in LibLSLCC simply detects when they are the last statement in a scope, then adds a semi-colon
   immediately after them in the generated code.

 * Full and optimized support for the CO-OP script stop strategy in OpenSim.

 * Correct order of operations via the use of operator function stubs
   that are generated on demand.  Old mono list optimizations will now
   port over without breaking, as well as other funky scripts that rely
   on Right to Left evaluation being the norm.


 * Negatable vectors and rotations.

 * Support for certain unicode characters in script symbols, (Same characters the linden Compiler allows)


# About LibLSLCCEditor


The project also includes an LSL Editor that was originally built to test the compiler library.
It has since developed into a full featured multi-tabbed IDE that is built on top of LibLSLCC's parsing framework.

Please take note that LSLCCEditor and its related projects are **Windows Only**, and you can 
only build the __LibLSLCC-WithEditor*.sln__ solution's on Windows using a version of Visual Studio that
support's targeting the .NET 4.5 framework.



VS2015 is your best bet, its what I use personally.  However if your version of Visual Studio's
support's .NET 4.5, you will probably be able to build the Editor project files with it.


LSLCCEditor Features:

 * Syntax Highlighting.

 * Documentation tooltips.

 * Context aware auto complete.

 * Go to definition (navigation by symbol).

 * Code formatting.

 * Library data for both Linden and Opensim SecondLife servers. 

 * Compilation to CSharp code for OpenSim as either
    client uploadable code or plain server side code. 




# Project Dependencies

All binary dependencies are distributed with the build.
You should not need to add any binaries yourself.

===

The LibLSLCC library itself depends on the **CSharp ANTLR 4.5.1 Runtime**.

You can find a license for ANTLR 4 in the **ThirdPartyLicenses** folder
of the build directory.

For more information about **ANTLR** go here:

http://www.antlr.org/index.html


===

The **LibraryDataScrapingTool** project depends on **System.Data.SQLite**
when compiled on Windows.  When its compiled on mono it uses **Mono.Data.Sqlite**.

see here: 

https://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki


===

The LSLCCEditor application depends on **AvalonEdit**, and a few pieces
of code forked from **AvalonEdit**.  You can also find a license for **AvalonEdit**
in the **ThirdPartyLicenses** folder of the build directory.

 
For more information about **AvalonEdit** go here:

http://avalonedit.net/


=== 


 
# Building LibLSLCC and LSLCCEditor On Windows

First off:

 * Building **LibLSLCC-WithEditor-WithInstaller.sln** is only tested in **VS2015**.

 * Building **LibLSLCC-WithEditor-NoInstaller.sln** should work with **VS2012 and up**.

 * Building **LibLSLCC-NoEditor.sln** should work with **VS2010 and up**.


===


LibLSLCC requires Java to be installed so that the ANTLR 4 parser generator tool 
can be run as part of the pre-build step for the LibLSLCC Library.  

The java executable will need to be in your PATH so that it can run from the command prompt. 
Fortunately installing java itself sets this up for you by default.


In order to build the **LibLSLCC-WithEditor-WithInstaller.sln** solution on Windows you must 
install the WiX Installer Toolset from http://wixtoolset.org/.  WiX will integrate with Visual Studio 
and allow you to load the WiX Project type, so you can build the **LSLCCEditorInstaller** project that
is part of this particular solution file.

Please make note that WiX will only integrate with Visual Studio if it is installed **After**
you install the version of Visual Studio your going to use.  See http://wixtoolset.org/ for 
all versions of Visual Studio that WiX it is compatible with.

**LibLSLCC-WithEditor-WithInstaller.sln** will only build the installer project when building for
**x64** or **x86** in **Release** mode.  The WiX installer build will not be triggered if you have the
"Platform" setting set to "AnyCPU" in Visual Studio, you must change it to either **x64** or **x86**.


The **LSLCCEditor** related projects are only buildable on Windows as they depend on WPF and AvalonEdit, 
but the **LibLSLCC** library, tools, and command line compiler are cross platform.  


Additionally the **LSLCCEditor** related projects can only be built in versions of Visual Studio 
that support targeting the .NET 4.5 framework. (VS2012+)


I have not tested other versions of Visual Studio besides VS2010 with the **LSLCCEditor** portion of the build.
VS2010 for sure **does not work**, as the editor uses a .NET framework level (4.5) that is too high for it to target, 
and WiX v3.10.1 does not support integration with VS2010.

If you happen to be using a version of Visual Studio that is incompatible with **WiX** or the **LSLCCEditor** portion of the
build, and you just want to build the library portion of the project;  Then you should use **LibLSLCC-NoEditor.sln**.


It contains only the library project and projects from the source tree which have a .NET 4.0 compatibility profile.
The projects in the **LibLSLCC-NoEditor.sln** solution have been tested with VS2010 so far, they will most likely
build with every version of Visual Studio starting with VS2010.


# Building LibLSLCC on *Nix Platforms with monodevelop 


Use **LibLSLCC-NoEditor.sln** when building for Mono.

Only the LibLSLCC, LibraryDataScrapingTools, lslcc_cmd and DemoArea projects are buildable on Mono 
and the **LibLSLCC-NoEditor.sln** includes only these projects for convenience.

Java is also required when building on *Nix platforms so that the ANTRL 4 tool can run.  
Make sure you have the latest version of Java available for your distribution installed and
that it is runnable from the command line.

You can open the provided **LibLSLCC-NoEditor.sln** solution on Linux using the latest version of MonoDevelop,
Or you can build it from the command line using the xbuild command (See the next section).

Other than some of the projects in the solution being un-buildable under Mono, the build on *Nix platforms behaves
the exact same way under xbuild/monodevelop as it does under MSBuild on Windows.


# Building LibLSLCC on *Nix Platforms with xbuild from the shell


To build LibLSLCC from the command line using mono and xbuild on nix platforms, first install Mono 
for your distribution then CD into the LibLSLCC source tree where **LibLSLCC-NoEditor.sln** resides.

enter the command:

	xbuild /p:Configuration=Release LibLSLCC-NoEditor.sln


LibLSLCC should start building.


**Note about xbuild when using Mono 4.* on *Nix:**


It seems that the latest versions of mono starting with major version 4 have trouble building 
project files that target the v4.0 framework.

If your building using 4.* version of mono you can work around this by forcing all projects to 
target the v4.5 framework, with:

	xbuild /p:Configuration=Release /p:TargetFrameworkVersion="v4.5" LibLSLCC-NoEditor.sln



# Python Build Scripts

The Python build scripts require python 2.7.* or 3.* to be installed.

If your using Windows, you must install python and add it to your path if you want to call this script from the command line.
When installing python on Windows there is an option to associate .py files with the python executable, so that clicking on .py
files inside the file explorer will run them as well.

**build.py** can be used to build the library and optionally create a timestamped binary release zip file.

Simply running `python build.py` will build Release and Debug versions of all projects that are buildable on your platform.

Use this to display additional build options:
 
	python build.py --help

The quickest way to build all the Release mode binaries for each project that will build on your platform is:

	python build.py --only-release

And to build just LibLSLCC:

	python build.py --only-liblslcc


**clean.py** simply cleans all configurations/platforms of the build when you run it.


# About Build Warnings

On mono you may get alot of warnings about missing comments.

They are suppressed in the Windows build but not yet in the mono build as the warning numbers are different.
You will also get warnings about unknown warning numbers being suppressed (XML comment warnings), but this is harmless.