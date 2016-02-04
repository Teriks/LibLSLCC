# Building LibLSLCC and LSLCCEditor On Windows

Important thing to note:

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

Please make note that WiX will only integrate with Visual Studio if it is installed **after**
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

Only the **LibLSLCC**, **LibraryDataScrapingTools**, **lslcc_cmd** and **DemoArea** projects are buildable on Mono 
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

The Python build scripts require python 2.7.* or 3.* to be installed.  You must also have an appropriate version of 
Visual Studios installed on Windows, or the Mono development package if your on *nix.

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

To build and package up a timestamped release in the BinaryRelease folder:

	python build.py --binary-release-zip


**clean.py** simply cleans all configurations/platforms of the build when you run it.


# About Build Warnings

On mono you may get alot of warnings about missing comments.

They are suppressed in the Windows build but not yet in the mono build as the warning numbers are different.
You will also get warnings about unknown warning numbers being suppressed (XML comment warnings), but this is harmless.