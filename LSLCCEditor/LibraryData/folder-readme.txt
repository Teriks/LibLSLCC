This folder is used for nothing, its basically just a visual studio's filter folder.

The 'default.xml' and 'LibraryData_XML_Syntax_README.txt' file are included from the LibLSLCC library
project tree in the MSBUILD file, they do not actually exist here, but you will see them in this folder
when the project is opened in Visual Studios.

The files are copied to the build output directory under the 'library_data' folder via an MSBUILD
Copy task after the build completes, this is so they can be picked up by the installer project.