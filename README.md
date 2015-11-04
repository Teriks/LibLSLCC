#Binary Releases

If you don't want to build the Library or Editor to use it,
I release the latest binaries for the project in each GitHub release.

https://github.com/Teriks/LibLSLCC/releases


Each release contains an AnyCPU, x86 and x64 version of LibLSLCC
with a documentation .XML file generated for the library.

They also include platform specific installers for LSLCCEditor.


Granted, I only really need to release an AnyCPU binary, but since
LSLCCEditor uses a platform specific binary when building with an installer
I supply all possible binary types for LibLSLCC in the release.

The x64 and x86 platform specific binaries are just a side effect
of the WiX installer having to be configured differently when building 
for different platforms (x64 or x86).


My OpenSim Fork uses the AnyCPU binary as its compatibile across platforms.


#About LibLSLCC 

 
The LibLSLCC library is a compiler framework for writing LSL compilers.

The LibLSLCC code validator/syntax tree builder provides **full front end syntax checking of LSL**.
It even includes extended warnings that you would find in most compilers for other languages,
but that are not implemented in either the Linden compiler or current OpenSim compiler.


Warnings for things such as: 

 * Constant expressions in conditional/loop statements.
	
 * Un-used variables.
	
 * Dead code.
	
 * Deprecated function usage.
	
	
That's not everything just some common ones.. this list is pretty long.
I suggest you mess around with the editor releases to discover what all 
LibLSLCC can tell you about your code.
	
	
LibLSLCC can also be used for general purpose LSL parsing tasks, it provides
its own rich syntax tree (Created as LSL code is validated) that has been completely
abstracted from ANTLR.  The syntax tree is tailored specifically for dealing with LSL,
and there is an interface for every node so that you can implement your own code DOM 
if want to.


====


LibLSLCC includes a CSharp code generator that targets the OpenSim runtime.

The Code Validator and OpenSim code generator in LibLSLCC have both been designed
with the intent of implementing Linden LSL with 100 percent cross compatibility.

The project is basically a complete reverse engineering of the Linden compiler's
Grammar/Rules and generated code behavior.



I have integrated LibLSLCC into OpenSim, See Here:

https://github.com/Teriks/OpenSim_With_LibLSLCC 
 
Or:
         
https://gitlab.com/erihoss/OpenSim_With_LibLSLCC 


====   
	   
The Code Validator/OpenSim Code Generator Features:

 * Full front end Syntax Checking, including dead code detection.  
   no more esoteric CSharp compiler errors or line mapping funkyness.
	
 * Dead code elimination from generated code where applicable.
   This includes un-used functions and global variables, as well
   as any dead code in a function/event body that does not cause a compile 
   error and is safe to remove given its context.
	
 * Correct code generation for global variables that reference each other.
	
 * Symbol name mangling specific to globals/parameters/local variables/labels and 
   user defined functions.  This completely abstracts variable scoping rules from the CSharp 
   compiler underneath.  All variable scoping rules are handled by the front end LibLSLCC Code Validator.
   The scoping rules implemented are %100 true to LSL.  Symbol mangling also removes the possibility 
   of causing a CSharp syntax error by using a keyword/Class name as a variable or function name.
	  
	  
 * Correct Code generation for jumps over declared variables.
	
 * Labels no longer require a "NoOp()" after every label, the OpenSim code generator 
   in LibLSLCC simply detects when they are the last statement in a scope, then adds a semi-colon
   immediately after them in the generated code.
	
 * Full and optimized support for the CO-OP script stop strategy in OpenSim.
   An option to enable this is not in the editor yet, but the OpenSim fork enables it
   when it's seen in OpenSim.ini.
	
 * Correct order of operations via the use of operator function stubs
   that are generated on demand.  Old mono list optimizations will now
   port over without breaking, as well as other funky scripts that rely
   on Right to Left evaluation being the norm.
	  
 * Correct treatment of vectors/rotations/lists and strings as booleans.
	
 * Vectors and rotations can now be negated.
	
 * At this point I am scratching my head because I cannot remember what else...
   I put a lot of time into this.
	 
	
#About LibLSLCCEditor

  
The project also includes full featured LSL Editor that was originally built to test the compiler library.
It has since developed into a full blown multi-tabbed IDE that is built on top of LibLSLCC's parsing framework.

Please take note that LibLSLCCEditor and its related projects are **Windows Only**, and you can 
only build the **LibLSLCC-WithEditor*.sln** solution's on Windows using a version of Visual Studio that
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
	
 * Compilation to CSharp code for OpenSim.
	 (CO-OP script stop mode cannot be enabled in the editor yet.)
	

 

#Project Dependencies

All binary dependencies are distributed with the build.
You should not need to add any binaries yourself.

===

The LibLSLCC library itself depends on the **CSharp ANTLR 4.5.1 Runtime**.

You can find a license for ANTLR 4 in the **ThirdPartyLicenses** folder
of the build directory.

For more information about **ANTLR** go here:

http://www.antlr.org/index.html


===

The ***LibraryDataScrapingTools** project depends on **System.Data.SQLite**
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

 
 
#Building LibLSLCC and LSLCCEditor On Windows

 
LibLSLCC requires Java to be installed so that the ANTLR 4 parser generator tool 
can be run as part of the pre-build step for the LibLSLCC Library.  

The java executable will need to be in your PATH so that it can run from the command prompt. 
Fortunately installing java itself sets this up for you by default.


In order to build the **LibLSLCC-WithEditor-WithInstaller.sln** solution on Windows you must 
install the WiX Installer Toolset from http://wixtoolset.org/.  WiX will integrate with Visual Studio 
and allow you to load the WiX Project type, so you can build the **LSLCCEditorInstaller** project that
is part of this particular solution file.


All C# projects in the solution can be built using Microsoft Visual Studio Express/Comunity Edition 2015.


**LSLCCEditor** and **LSLCCEditor.CompletionUI** are only buildable on Windows as they depend on WPF and AvalonEdit, 
but the **LibLSLCC** project and related projects are cross platform.  

Additionally **LSLCCEditor** and **LSLCCEditor.CompletionUI** can only be built in versions of Visual Studio 
that support targeting the .NET 4.5 framework.

I have not tested other versions of Visual Studio besides VS2010 with the **LSLCCEditor** portion of the build.
VS2010 for sure **does not work**, as the editor uses a .NET framework level (4.5) that is too high for it to compile, 
and WiX v3.10.1 does not support integration with VS2010.


If you happen to be using a version of Visual Studio that is incompatible with **WiX** or the **LSLCCEditor** portion of the
build, and you just want to build the library portion of the project;  You can use **LibLSLCC-NoEditor.sln**.


It contains only the library project and projects from the source tree which have a .NET 4.0 compatibility profile.
The projects in the **LibLSLCC-NoEditor.sln** solution have been tested with VS2010 so far, they will most likely
build with every version of Visual Studio starting with VS2010.
 


#Building LibLSLCC on *Nix Platforms with monodevelop 


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


#Building LibLSLCC on *Nix Platforms with xbuild from the shell


To build LibLSLCC from the command line using mono and xbuild on nix platforms, first install Mono 
for your distribution then CD into the LibLSLCC source tree where **LibLSLCC-NoEditor.sln** resides.

enter the command:

	xbuild /p:Configuration=Release LibLSLCC-NoEditor.sln
	

LibLSLCC should start building.