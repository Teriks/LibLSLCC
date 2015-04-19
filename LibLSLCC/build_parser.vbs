


SET objFSO = CREATEOBJECT("Scripting.FileSystemObject")

dim CurrentDirectory
CurrentDirectory = objFSO.GetAbsolutePathName(".")

SET objFile = objFSO.GetFile(CurrentDirectory+"\\LSLParser\\~touch")
SET objFile2 = objFSO.GetFile(CurrentDirectory+"\\LSLParser\\LSL.g4")








a = (DateDiff("yyyy", objFile.DateLastModified, objFile2.DateLastModified) <> "0")
b = (DateDiff("d", objFile.DateLastModified, objFile2.DateLastModified) <> "0")
c = (DateDiff("m", objFile.DateLastModified, objFile2.DateLastModified) <> "0")
d = (DateDiff("h", objFile.DateLastModified, objFile2.DateLastModified) <> "0")
e = (DateDiff("n", objFile.DateLastModified, objFile2.DateLastModified) <> "0")
f = (DateDiff("s", objFile.DateLastModified, objFile2.DateLastModified) <> "0")


	
exist = objFSO.FileExists(CurrentDirectory+"\\LSLParser\\LSLParser.cs")
exist = exist And objFSO.FileExists(CurrentDirectory+"\\LSLParser\\LSLVisitor.cs")
exist = exist And objFSO.FileExists(CurrentDirectory+"\\LSLParser\\LSLListener.cs")
exist = exist And objFSO.FileExists(CurrentDirectory+"\\LSLParser\\LSLLexer.cs")
exist = exist And objFSO.FileExists(CurrentDirectory+"\\LSLParser\\LSLLexer.tokens")
exist = exist And objFSO.FileExists(CurrentDirectory+"\\LSLParser\\LSL.tokens")
exist = exist And objFSO.FileExists(CurrentDirectory+"\\LSLParser\\LSLBaseVisitor.cs")
exist = exist And objFSO.FileExists(CurrentDirectory+"\\LSLParser\\LSLBaseListener.cs")

If (a) Or (b) Or (c) Or (d) Or (e) Or (f) Or Not exist Then

	Set objShell = CreateObject("Shell.Application")

	Set objFolder = objShell.NameSpace(CurrentDirectory+"\LSLParser")
	  
	Set objFolderItem = objFolder.ParseName("LSL.g4")
	Set objFolderItem2 = objFolder.ParseName("~touch")
	
	n = CDate(Now())
	  
	objFolderItem.ModifyDate = n
	objFolderItem2.ModifyDate = n

	wscript.echo "Parser out of date, building parser."
	
	Set oShell = WScript.CreateObject ("WScript.Shell")

	oShell.Run "cmd /c java -classpath ./antlr-4.5-complete.jar org.antlr.v4.Tool -Dlanguage=CSharp -visitor -listener -package LibLSLCC -o ./LSLParser ./LSLParser/LSL.g4", 1, 0



End If
