See LICENSE for license. 
 
 
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
	
 
 
Happy Coding/Scripting. 

 
Note:

	This solution contains an installer project for the code editor which requires  
	the WiX Toolset to build.  see: http://wixtoolset.org/ for the WiX Toolset download 
	if your having trouble building the installer project. 
 