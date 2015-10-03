#About LibLSLCC and LibLSLCCEditor


The LibLSLCC Library was designed to be a drop in replacement for OpenSim's default compiler.
LibLSLCC implements an true compiler for LSL with full front end syntax checking, true to Linden LSL 
Operator precedence, and compatibility fixes for issues that the default OpenSim compiler and OpenSim LSL runtime suffers from. 
 
The LibLSLCC compiler also provides helpful extended syntax warnings, errors and code validations not present in the Linden compiler. 
 
 
The project includes full featured LSL Editor with context aware auto complete, go to definition (navigation by symbol), code formatting, and library data
for both Linden and Opensim SecondLife servers.  It was built to test the compiler originally but has developed into a full blown multi tabbed IDE.



As of 10/2/2015 I have integrated my compiler into the OpenSim 0.8.2.0 Development version and
made the code public


See:

	https://github.com/Teriks/OpenSim_0.8.2.0_Dev_LibLSLCC

	Or
	
	https://gitlab.com/erihoss/OpenSim_0.8.2.0_Dev_LibLSLCC
	

	

#Building LibLSLCC and LSLCCEditor


LibLSLCC requires Java to be installed so that the ANTLR 4 parser generator tool (Which is written in Java) can be run as 
part of the pre-build step for the LibLSLCC Library.  The java executeable will need to be in your PATH so that it can run from the 
command prompt.  You can see if you have java installed by opening a command prompt/terminal and typing 'java', if you get an error
instead of information about the java executable's command line options then java is not installed and you will need to go
install it before the build will work.

You can build the entire solution on Windows using Microsoft Visual Studio express and up (2015 is what I am using).
LSLCCEditor, LSLCCEditor.CompletionUI and LSLCCEditor installer are only buildable on Windows as they depend on WPF, AvalonEdit
and WiX Installer framework,  but the LibLSLCC library is cross platform.

In order to build the LSLCCEditorInstaller project on Windows you must install the WiX Installer Toolset from http://wixtoolset.org/
after you have installed Microsoft Visual Studio.  Wix will integrate with Visual Studio and allow you to load the WiX Project 
type so you can build the LSLCCEditorInstaller project.

If you do not have WiX installed the project will still load but Visual Studio will show an error about LSLCCEditorInstaller 
being an unsupported project type.  If you don't care about building the installer for the editor just click through the error
and the project will be ignored when you try to build.

If you only wish to build the LibLSLCC library, you can go to Build -> Configuration Manager in Visual Studios and uncheck 'Build'
next to every project in the solution besides LibLSLCC.  You must first set the active solution configuration to the desired build
type (Release or Debug), and the active solution platform to the desired architecture before doing this, so that the settings acutally
apply to the type of build you want.

Binarys for all projects are placed in their project directory under bin/(Architecture x86|x64)/(Build Type Release|Debug)



#Building LibLSLCC on *Nix Platforms with xbuild/monodevelop


Java is also required when building on *Nix platforms so that the ANTRL 4 tool can run.  Make sure you have the latest
version of Java available for your distribution installed and that it is runnable from the command line.

Only the LibLSLCC, lslcc_cmd and LibraryDataScrapingTools projects are buildable on Mono.

You can open the provided Visual Studio solution on Linux using the latest version of MonoDevelop.
You will need to unload or remove the LibLSLCCEditor, LSLCCEditor.CompletionUI and LSLCCEditorInstaller projects before building
the solution with MonoDevelop as they will not build.  This is because the editor is built using the WPF UI Framework which currently
has no implementation in Mono, and the installer is Windows MSI based.

MonoDevelop will complain some about trying to load the LSLCCEditorInstaller project, but you can just click through the error and then
remove or unload the project as well as the other incompatible projects from the solution before building.

Other than some of the projects in the solution being un-buildable under Mono, the build on *Nix platforms behaves the exact same way
under XBuild as it does under MSBuild on Windows.





 