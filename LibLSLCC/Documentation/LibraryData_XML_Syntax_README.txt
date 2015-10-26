================================
===  Library Data XML Syntax ===
================================

The basic syntax of library data files is simple, first off: the root node is always named "LSLLibraryData".


=================================
===  SubsetDescription Node's ===
=================================


The content of the LSLLibraryData Node should start with your SubsetDescription's, which name the subsets
that library definitions can be a member of:


<SubsetDescription Subset="os-lsl" FriendlyName="OpenSim LSL">
	<Description>The subset of standard library functions from LSL supported by OpenSim SecondLife servers.</Description>
</SubsetDescription>


@@@ Subset Attribute @@@

The subset attribute defines the name of the subset, think of it as the name of a module that can be imported.

There are naming rules for subset names, they must follow the regex pattern: ([a-zA-Z]+[a-zA-Z_0-9\\-]*)

If they do not match this pattern, a syntax error will be thrown from the loader.


@@@ FriendlyName Attribute @@@

The friendly name of a subset is basicly just a description, it has no format rules.   It's used by LSLCCEditor
to present the user with a GUI friendly name for a library subset.  It's not really used for anything in OpenSim.


/// Description Node (Child) ////


The Description node that exists as a child of SubsetDescription just contains a description of what
definitions the subset contains,  It's used to show a description of a subset to the user in LSLCCEditor
but is not used in OpenSim.



### NOTES about SubsetDescription ###

You can have multiple subset descriptions in your library data file, but you must not use a subset name that already exist in your file
or another file thats being loaded with yours, subset names must be globally unique or an error will be thrown when loading the XML.



====================================================================
====================================================================
====================================================================



==================================
====  LibraryFunction Node's =====
==================================


The LibraryFunction Node is used to define a library function that is available when compiling.
Heres a basic example of a library function node:


<LibraryFunction Name="llAbs" ReturnType="Integer" Subsets="lsl,os-lsl">
	<Parameter Name="Value" Type="Integer" />
	<DocumentationString>Returns the absolute (positive) version of Value.</DocumentationString>
	
	<!-- Optional Properties, This is just implemented as a property 'dictionary' -->
	<!-- Any property name and value can be attached to functions, constants and library events;  for extensibility. -->
	<!-- The LibLSLCC OpenSim code generator uses 'ModInvoke' to determine if it should generate a 'modInvoke*' runtime function call for calls to this function. -->
	<!-- LSLCCEditor uses the 'Deprecated' property to change the highlighting and documentation formating for deprecated library functions. -->
	<Property Name="ModInvoke" Value="false">
	<Property Name="Deprecated" Value="false">
</LibraryFunction>


@@@ Name Attribute @@@


The name attribute of the function signature simply defines the name of the function, it must follow LSL's function naming rules
or a syntax error will be thrown in the XML loader.


@@@ ReturnType Attribute @@@


The return type of a function can be set to one of the following case sensitive values:

	Void
	Integer
	Float
	Vector
	Rotation
	List
	String
	
Anything else is a syntax error.



@@@ Subset Attribute @@@


The subset attribute defines what subset(s) the function belongs to. 
  
There cannot be a duplicate or ambigious definition of a function that exist withen the same subset or an error will be thrown from the loader.

Something interesting about the node above is that the 'Subsets' attribute is a list.
It does not have to be though;  It can either be a list of subsets or a single subset, 
but it must not be empty or you will get an error.

Additionally the subset name(s) used in the Subset attribute must have an associtated SubsetDescription node
with the same name (SubsetDescription.Subset Attribute).  If you reference a subset name with no SubsetDescription node
for it having been previously defined, the XML loader will throw an error.

You can specify any library constant to exist in more than one library subset by seperating the subset names it belongs to with a comma.
This effectively means that a definition is shared by two or more subsets, and will be present if any subset it belongs to is loaded.
Even if all the subsets it belongs to are loaded at once it will not cause a duplicate definition error because the subsets 'share'
the definition.

This was implemented so that functions definitions, constants, and event handlers shared by both the Linden LSL Implementation, and OpenSim LSL Implementation
would not have to be defined twice in two places.  Having signature definitions that exist in both LSL implementations able to be shared accross subsets
saves alot of space and loading time.



/// Parameter Node's (Children) ///


Parameter nodes are a child of LibraryFunction. Obviously, a function definition can have Zero or more defined Parameter nodes.
The order of the parameters in the defined signature is determined by their order in the XML markup.

Examples: 

	<Parameter Name="Value" Type="Integer" />  ; Simple parameter, just a plain old concrete parameter with the name "Value" and the Type "Integer"
	
	<Parameter Name="Value" Type="String" Variadic="true" />  ; A variadic string parameter, variadic parameters must be the last defined parameter in a signature or an error will be thrown by the loader.
															  ; This particular variadic parameter definition only accepts multiple strings.  (its like 'params string[] Value' in CSharp)
															  ; Variadic parameters can be used with any LSL type, not just strings.
															  
															  
	<Parameter Name="Value" Type="Void" Variadic="true" />  ; A Void variadic parameter, if a variadic parameter type is 'Void' than it accepts anything.  (its like 'params object[] Value' in CSharp)



Parameter node rules are very straight forward:

1.) The 'Name' attribute must use valid characters for an LSL parameter defintion or a syntax error will occur.
2.) No duplicate parameter names.
3.) Parameters that are not variadic cannot have the Type 'Void', thats an error.
4.) Variadic Parameters can only be the last parameter of a function definition, if you define more parameters after a variadic parameter an error will be thrown by the loader.



/// DocumentationString Node (Child) ///


Pretty self explanitory, this can contain documentation for the function.  It's not a required node, but LSLCCEditor uses it.


/// Property Node's (Children) ///


Every library signature has a property dictionary attached to it, so that you can add arbitrary data to a definition as a Name/Value pair.

For example, LibLSLCC's OpenSim code generator uses the property:

	<Property Name="ModInvoke" Value="true">

If the code generator sees that this property exists and its value is 'true' (Case Insensitive), than the apropriate 'modInvoke' function 
from the OpenSim modInvoke module will be used to invoke the function by name in the generated code.



### NOTES about LibraryFunction ###


Whats considered a duplicate function definition?:

Two library function definitions in the same subset are considered to be duplicate if any of the following apply:

1.) The two functions have no parameters and the same name.
2.) The two functions have the same name, each have no variadic parameters, and all the parameter types are the same.

Return type is not considered, since it cannot be responsible for making a function unique, only the function name and parameter types can.


==


Can I define Function Overloads?:

	Yes, you can create overload's of functions that exist within the same library subset.
	
	The algorithm used for picking the correct function overload behave's identically to how CSharp overload resolution behaves,
	With some exceptions...  LSL 'list' do not 'fill out' variadic parameters, and the only forms of signature match degradation
	are implicit conversions between 'key', 'string' types, as well as implicit conversions of every type into 'params any[] p' variadic
	parameters.
	
	The algorithm is as follows:
	
	1.) The algorithm finds a set of candidates, or possible overload matches, there may be only one or none at all in some cases.
		A candidate is considered to be any function signature which can be succesfully called with the given parameter expressions.
		From this set, the 'best' or most 'specific' function signature needs to be further narrowed down.
	
	2.) The algorithm picks the most 'specific' signature if multiple overloads candidates are found.
		This is done by ranking all possible matches by the amount of implicit conversions required for the match to succeed,
	    If all matching signatures require the exact same amount of implicit conversions to succeed, the match is declared ambigious and overload resolution fails.
	
	3.) If the match is not found to be ambigious in the previous step, the algorithm groups the matching signatures into
		groups using the number of implicit conversion required as a grouping key.  The signature group that uses the smallest amount of implicit conversions then is found.
	
	4.) If the signature group whos signatures require the least amount of implicit conversions only consist of a single signature, its returned as an un-ambigious match.
	
	5.) If the found group contains multiple signatures, the signature with the closest amount of concrete (non-variadic) parameters
		is selected as an un-ambigious match.
	
	==


	say we have:
	
	1.)	void Func(string str);
	2.)	void Func(key k);
	3.)	void Func(key k, string str, key k2);
	4.)	void Func(key k, string str, string str);
		
		
	defined... Here's some sample overload resolutions:
	
		Func(""); -> matches: ------------------------ void function(string str);  // because "" is a string literal and no implicit conversion was required, its an exact match.
		
		Func((key)""); -> matches: ------------------- void function(key k);  // because the passed type was is a key, and its the best match for a key parameter since no implicit conversion is required.
		
		Func((key)"",(key)"", ""); -> matches: ------- void Func(key k, string str, string str);  // because signature #4 requires only 1 implict conversion and signature #3 requires 2.
		
		Func("", "", ""); -> matches: ---------------- void Func(key k, string str, string str);  // again because signature #4 requires only 1 implict conversion and signature #3 requires 2.
		
		Func((key)"", (key)"", (key)""); -> matches: - void Func(key k, string str, key k2);  // because signature #3 requires only 1 implict conversion and signature #4 requires 2.
	
	==
	
	here are some good examples given the following definitions:
	
	1.) void Func();
	2.) void Func(params void[] i);
    3.) void Func(params integer[] i);
	4.)	void Func(key i, params string[] s);
	5.) void Func(string i, params string[] s);
    6.) void Func(int i, params int[] i2);
    7.) void Func(int i,  int i2, params string[] i3);
		
	and for the given calls:
	
		Func(); -> matches: -------------- void Func();   //the path of least resistance, functions without parameters are prefered over functions with a single variadic parameter.
		
		Func(0); -> matches: ------------- void Func(int i, params int[] i2);  //It matches because the second parameter is allowed to be empty, and the one concrete parameter is more specific than 'params void[] i' or 'params integer[] i'
																		       //in definition #2 and #3.  This surprises a lot of people who think it should match #3, try it in CSharp and see!
																
		Func(""); -> matches: ------------ void Func(string i, params string[] s);  //Same principle as the last match, it matches the overload with the 'string' parameter first because "" implicitly converts to a key, but 'string' is more "specific".
																				    //definition #2 is ignored because it is the most generic alternative.
																	 
		Func("", (key)""); -> matches: --- void Func(string i, params string[] s);  //because signature #4 requires 2 implicit conversions to match and #5 requires only 1;
		
		Func(0,0); -> matches: ----------- void Func(int i,  int i2, params string[] i3); //because the third parameter does not have to exist, and more concrete parameters (non-variadic) match than in definition #6, #3 and #2
		
	==
	
	

Sometimes variadic function parameters can cause non-duplicate function overloads to easily become ambigious at the call site.

For instance, take these declarations:

	void Func(string one, params string[] two);

	void Func(string one, params void[] two);

	

	Func(""); -> ambigious match, won't compile.
	
	Func("",""): -> void Func(string one, params string[] two);
	
	Func("",0): -> void Func(string one, params void[] two);

	

If you have used variadic functions in CSharp then ambiguity problems such as this should be familiar to you.

	
====================================================================
====================================================================
====================================================================


==================================
====  LibraryConstant Node's =====
==================================


Library constant nodes represent the library constants defined in a subset.
They are unique by name alone, and if you define a constant with the same name in the same subset an error will be thrown the the XML loader.

Some of the more interesting definitions:

<LibraryConstant Name="EOF" Type="String" Value="&#xA;&#xA;&#xA;" Subsets="lsl,os-lsl">
	<DocumentationString>Indicates the last line of a notecard was read.</DocumentationString>
	<!-- optional properties -->
	<Property Name="Deprecated" Value="false">
	<Property Name="Expand" Value="false">
</LibraryConstant>

<LibraryConstant Name="JSON_ARRAY" Type="String" Value="﷒" Subsets="lsl">
	<DocumentationString />
	<!-- optional properties -->
	<Property Name="Deprecated" Value="false">
	<Property Name="Expand" Value="false">
</LibraryConstant>


<LibraryConstant Name="PI" Type="Float" Value="3.1415926535897932384626433832795" Subsets="lsl,os-lsl">
	<DocumentationString>3.14159265 - The number of radians in a semi-circle.</DocumentationString>
	<!-- optional properties -->
	<Property Name="Deprecated" Value="false">
	<Property Name="Expand" Value="false">
</LibraryConstant>


@@@ Name Attribute @@@


The name attribute must be unique per subset, it also must abide by LSL's symbol naming conventions or the XML loader will throw a syntax error.


@@@ Type Attribute @@@


Can be any LSL type, except Void.  The name is case sensitive, (must start with an uppercase letter).
If you specify the type as Void the loader with throw a syntax error.


@@@ Subset Attribute @@@


Identical purpose to the Subset's attribute on LibraryFunctionSignature Nodes.  Disscussed above.


@@@ Value Attribute @@@

The value associtated with the constant.

	Formating is as follows:
	
	Vector:   Value="0.0,0.0,0.0"    // you should not include the greater and less than signs that enclose the vector, they will just be removed anyway.
									 // you can use integer component's to, as well as positive and negative prefixes on the component values.
	
	Rotation: Value="0.0,0.0,0.0,0.0"  // Same Idea with Rotation's, don't enclose the rotation with qreater and less than signs, if you do they just get removed anyway.
	
	Float:    Value="1.333333333333"  // As you would expect.
	
	String:   Value="hello world"  // Do not include quotes unless you want them in the string, you can use normal XML escaping to specify any special characters, or paste them in as raw characters.
								   // You should save your File as UTF16 (Unicode) if you want to be able to use all the raw characters that LSL and the LibLSLCC compiler actually supports.
								   
	Key:      Value="UUID"  //Identical formating to String, do not include quotes unless you want them in the key string.
								   
	List:     //List should be wrapped in [] symbol's, only literal expression values will be parsed out of the string.
			  //If you add variable references, function calls or nested lists the loader will throw a syntax error.
			  //You can use the cast expression (key)"" to specify a key typed value.
			  //You can also the negate/positive prefix LSL operators on Float and Integer values that are not hexdecimal.


===	
	

/// DocumentationString Node (Child) ///

A place to add documentation for the library constant, it's not really used in OpenSim, but it is used in LSLCCEditor.
This node is not required to be present.

/// Property Node (Children) ///

The same Name/Value dictionary that exists in all LibrarySignature nodes. LibLSLCC editor uses the 'Deprecated' property to change the
highlighting of deprecated constants in the editor, as well as the formating in the documentation tooltip.

The 'Expand' property is used by the OpenSim code generator, if 'Expand' is set to 'true' then the constants value will be expanded into the
generated source code much in the same way a C prepreprocessor would expand prepreprocessor definitions.  The value will be placed into
the generated source code as a code literal that is apropriate for the constants associtated type when 'Expand' is true, otherwise the constant
will be created as a reference to a static variable that exists in the base class of the generated script class.


====================================================================
====================================================================
====================================================================



==================================
====  EventHandler Node's ========
==================================

EventHandler nodes are for defining the event handlers that are able to be used in a subset or subset(s).

Examples:


<EventHandler Name="state_entry" Subsets="lsl,os-lsl">
	<DocumentationString>The state_entry event occurs whenever a new state is entered, including at program start, and is always the first event handled.</DocumentationString>
	<!-- optional properties -->
	<Property Name="Deprecated" Value="false">
</EventHandler>


<EventHandler Name="collision_start" Subsets="lsl,os-lsl">
	<Parameter Name="NumberOfCollisions" Type="Integer" />
	<DocumentationString>This event is raised when another object, or avatar, starts colliding with the object the script is attached to.  The number of detected objects is passed to the script.</DocumentationString>
	<!-- optional properties -->
	<Property Name="Deprecated" Value="false">
</EventHandler>


EventHandler nodes are pretty much identical to LibraryFunction nodes, except for the following:

1.) No ReturnType can be specified, LSL events are not allowed to return values.
2.) Events cannot have Variadic parameters, The 'Variadic' attribute is not even available for use on the Parameter children nodes.



#Notes:

The 'Deprecated' property is only used by LSLCCEditor to mark events as deprecated in their documentation string.
Currently there are not really any deprecated event handlers that exist in standard LSL, but who knows; maybe someday.


====================================================================
====================================================================
====================================================================




