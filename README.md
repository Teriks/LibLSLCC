# Binary Releases

If you don't want to build the Library or Editor to use it,
I release the latest binaries for the project in each GitHub release.

https://github.com/Teriks/LibLSLCC/releases


Each release contains an AnyCPU Release/Debug version of LibLSLCC,
an AnyCPU binary copy of lslcc (The command line compiler), and an XML documentation 
file generated for the library by Visual Studio/MSBuild.

Releases include platform specific installers for LSLCCEditor. However, the only 
difference between each installer currently is the install directory that they default to.

The installers for both x64 and x86 platforms use platform independent AnyCPU binaries.

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

## OpenSim Code Generation


LibLSLCC includes a CSharp code generator that targets the OpenSim runtime.

The code validator (parser/tree builder) and OpenSim code generator in LibLSLCC have both been designed
with the intent of implementing Linden LSL with near perfect cross compatibility.

The project is basically a complete reverse engineering of the Linden compiler's
grammar, non grammar level syntax rules, and generated code behavior.

I have integrated LibLSLCC into OpenSim, see here:

https://github.com/Teriks/OpenSim_With_LibLSLCC


### Notable Codegen Features:

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
   
 * Correct Code generation for jumps over variable declarations.  Jumping over a variable declaration
   will leave said variable with a default value instead of null.  Leaving these variables null is 
   something both the Linden compiler and current OpenSim compiler do, which is a step out of the LSL
   SandBox, as null variables are not supported elsewhere in LSL and generally not expected as a function parameter.

 * Labels no longer require a "NoOp()" after every label, the OpenSim code generator 
   in LibLSLCC simply detects when they are the last statement in a scope, then adds a semi-colon
   immediately after them in the generated code.

 * Full and optimized support for the CO-OP script stop strategy in OpenSim.

 * Correct order of evaluation via the use of operator function stubs
   that are generated on demand.  Old mono list optimizations will now
   port over without breaking, as well as other odd scripts that rely
   on Right to Left evaluation being the norm.

 * Support for certain unicode characters in script symbols. (Most of the same characters the linden compiler allows)



# About LSLCCEditor

The project also includes an LSL Editor that was originally built to test the compiler library.
It has since developed into a full featured multi-tabbed IDE that is built on top of LibLSLCC's parsing framework.

Please take note that LSLCCEditor and its related projects are **Windows Only**, and you can 
only build the __LibLSLCC-WithEditor*.sln__ solution files on Windows using a version of Visual Studio that
support's targeting the .NET 4.5 framework.



VS2015+ is your best bet for building however, if your version of Visual Studio's
support's .NET 4.5 you will probably be able to build the Editor project files with it.


### Notable Editor Features:

 * Tabbed multi script editing.

 * Syntax highlighting.

 * Documentation tooltips.

 * Context aware auto complete.

 * Go to definition (navigation by symbol).

 * Code formatting.

 * Library data for both Linden and Opensim SecondLife servers. 

 * Compilation to CSharp code for OpenSim as either
    client uploadable code or plain server side code. 


Here A few images of the editor small enough to squeeze into this readme...  

This dark color scheme can be found here: https://github.com/Teriks/LSLCCEditorThemes

![LSLCCEditor](http://imgur.com/qOoVhjK.png)

![LSLCCEditor](http://imgur.com/hwIOHAf.png)


# Project Dependencies

All binary dependencies are distributed with the build.
You should not need to add any binaries yourself.

## ANTLR

The LibLSLCC library itself depends on the **CSharp ANTLR 4 Runtime**.

You can find a license for ANTLR 4 in the **ThirdPartyLicenses** folder
of the build directory.

For more information about **ANTLR** go here:

http://www.antlr.org/index.html


## SQLite

The **LibraryDataScrapingTool** project depends on **System.Data.SQLite**
when compiled on Windows.  When compiled on mono it uses **Mono.Data.Sqlite**.

see here: 

https://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki

## AvalonEdit

The LSLCCEditor application depends on **AvalonEdit**, and a few pieces
of code forked from **AvalonEdit**.  You can also find a license for **AvalonEdit**
in the **ThirdPartyLicenses** folder of the build directory.

 
For more information about **AvalonEdit** go here:

http://avalonedit.net/


 
# Building LibLSLCC and LSLCCEditor On Windows

Important thing to note:

 * Building **LibLSLCC-WithEditor-WithInstaller.sln** should work with **VS2015 and up**.

 * Building **LibLSLCC-WithEditor-NoInstaller.sln** should work with **VS2012 and up**.

 * Building **LibLSLCC-NoEditor.sln** should work with **VS2010 and up**.


LibLSLCC requires Java to be installed so that the ANTLR 4 parser generator tool 
can be run as part of the pre-build step for the LibLSLCC Library.  

The java executable will need to be in your PATH so that it can run from the command prompt. 
Fortunately installing java itself sets this up for you by default.


In order to build the **LibLSLCC-WithEditor-WithInstaller.sln** solution on Windows you must 
install the latest WiX Installer Toolset from http://wixtoolset.org/releases/.

You must also install the WiX Toolset Extension for your version of Visual Studio, which can
also be found at http://wixtoolset.org/releases/.

Installing the extension will allow you to load the WiX Project type, so you can build the 
**LSLCCEditorInstaller** project that is part of the **LibLSLCC-WithEditor-WithInstaller.sln** solution file.


**LibLSLCC-WithEditor-WithInstaller.sln** will only build the installer project when building for
**x64** or **x86** in **Release** mode.  The WiX installer build will not be triggered if you have the
"Platform" setting set to "AnyCPU" in Visual Studio, you must change it to either **x64** or **x86**.


The **LSLCCEditor** related projects are only buildable on Windows as they depend on WPF and AvalonEdit, 
but the **LibLSLCC** library, tools, and command line compiler are cross platform.  


Additionally the **LSLCCEditor** related projects can only be built in versions of Visual Studio 
that support targeting the .NET 4.5 framework. (VS2012+)


I have not tested other versions of Visual Studio besides VS2010 with the **LSLCCEditor** portion of the build.
VS2010 for sure **does not work**, as the editor uses a .NET framework level (4.5) that is too high for it to target.

If you happen to be using a version of Visual Studio that is incompatible with the **LSLCCEditor** portion of the
build, and you just want to build the library portion of the project;  Then you should use **LibLSLCC-NoEditor.sln**.


It contains only the library project and projects from the source tree which have a .NET 4.0 compatibility profile.
The projects in the **LibLSLCC-NoEditor.sln** solution have been tested with VS2010 so far, they will most likely
build with every version of Visual Studio starting with VS2010.


# Building LibLSLCC on *Nix Platforms with mono/xbuild


To build LibLSLCC from the command line using mono/xbuild on nix platforms, first install Mono (complete) 
for your distribution and then CD into the LibLSLCC source tree where **LibLSLCC-NoEditor.sln** resides.

enter the command:

	xbuild /p:Configuration=Release LibLSLCC-NoEditor.sln


LibLSLCC should start building.


**Note about xbuild when using Mono 4.* on *Nix:**


The latest versions of mono starting with major version 4 cannot build assemblies targeting .NET Framework v4.0

If your building using 4.* version of mono you need to force all projects to target the v4.5 framework, with:

	xbuild /p:Configuration=Release /p:TargetFrameworkVersion="v4.5" LibLSLCC-NoEditor.sln


The python build script will do that for you behind the scenes.


# Cross Platform Python Build Scripts

The Python build scripts require python 3.* and the pip package manager for python3 to be installed.  You must also have an appropriate version of 
Visual Studios+WiX installed on Windows, or the Mono (complete) development package if your on *nix.

pip is installed with Python on Windows by default when using the offical installer package, on Linux or other platforms you may need to install it manually.  If you do not have pip (As indicated by the error produced when running build.py for the first time without pip installed), you should download the pip bootstrap script from <https://bootstrap.pypa.io/get-pip.py> and run it with python3 under admin or super user privileges.  If your distro has a python3-pip package already, you can also install using your package manager.

If your using Windows, you must install python and add it to your path if you want to call this script from the command line (There is an option to do this automatically from the installer dialog).
When installing python on Windows there is an option to associate .py files with the python executable, so that clicking on .py
files inside the file explorer will run them as well.

**build.py** can be used to build the library and optionally create a timestamped binary release zip file.

When build.py first runs it will download any dependencies it needs to the 'BuildScriptLibs' folder in the project tree, after that is done it will continue doing anythin you requested it to do.

Simply running `python build.py` will build Release and Debug versions of all projects that are buildable on your platform.

Use this to display additional build options:
 
	python build.py --help

The quickest way to build all the Release mode binaries for each project that will build on your platform is:

	python build.py --only-release

And to build just LibLSLCC:

	python build.py --only-liblslcc

To build and package up a version stamped release in the BinaryRelease folder:

	python build.py --binary-release

To clean the entire build:

	python build.py --clean

To update assembly versions (according to version.json):

	python build.py --update-versions


build.py --update-versions is meant to be used as a pre-commit hook.
