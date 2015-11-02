
LibLSLCC is capable of loading and combining library data from multiple XML files at once.  
It uses these library data files to determine what functions, constants and events handlers 
are defined while compiling so it can preform syntax checking.

Each library data file can contain one or more described subsets of library information,
this allows you to define one or even multiple 'modules' (called subsets) in a single file.

LibLSLCC can load this data in one of two ways:

1.) It can load only the subsets it needs into memory by filtering the library data when the file loads.
2.) Or it can load everything and Live Filter the data by subset name at runtime.


LSLCCEditor uses live filtering so it can quickly activate/de-activate a library subsets
based on the Library settings applied to an editor tab.


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

The friendly name of a subset is basically just a description, it has no format rules.   It's used by LSLCCEditor
to present the user with a GUI friendly name for a library subset.


/// Description Node (Child) ////


The Description node that exists as a child of SubsetDescription just contains a description for the subset,
It's used to show a description tool-tip in LSLCCEditor.



### NOTES about SubsetDescription ###

You can have multiple subset descriptions in your library data file, but you must not use a subset name that already exist in your file
or another file that’s being loaded along with yours.

Subset names must be globally unique or an error will be thrown when loading the XML.



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

The subset name(s) used in the Subset attribute must have an associated SubsetDescription node with the same name (The SubsetDescription.Subset Attribute).  
If you reference a subset name with no SubsetDescription node having been previously defined for it, the XML loader will throw an error.
  

There cannot be a duplicate or ambiguous definition of a function that exist within the same subset or an error will be thrown from the loader.
You can however share a definition with more than one subset, by listing the subset names in the Subset attribute separated by commas.


In the example node above for the function 'llAbs', the 'Subsets' attribute is a list.
It can either be a list of subsets or a single subset,  but it must not be empty or its a syntax error.

You can specify any library definition to exist in more than one library subset by separating the subset names it belongs to with a comma.
This effectively means that a definition is shared by two or more subsets, and will be present if any subset it belongs to is loaded.
Even if all the subsets it belongs to are loaded at once it will not cause a duplicate definition error because the subsets 'share'
the definition.

This was implemented so that functions definitions, constants, and event handlers shared by both the Linden LSL Implementation, and OpenSim LSL Implementation
would not have to be defined twice in two places.  Having signature definitions that exist in both LSL implementations be shared across subsets
saves a lot of space and loading time.


/// Parameter Node's (Children) ///


Parameter nodes are a child of LibraryFunction.  Obviously, a function definition can have Zero or more defined Parameter nodes.
The order of the parameters in the defined signature is determined by their order of definition in the XML markup.

Examples: 

	<Parameter Name="Value" Type="Integer" />  ; Simple parameter, just a plain old concrete parameter with the name "Value" and the Type "Integer"
	
	<Parameter Name="Value" Type="String" Variadic="true" />  ; A variadic string parameter, variadic parameters must be the last defined parameter in a signature or an error will be thrown by the loader.
															  ; This particular variadic parameter definition only accepts strings.  (It’s like 'params string[] Value' in CSharp)
															  ; Variadic parameters can be used with any LSL type, including Void.
															  
															  
	<Parameter Name="Value" Type="Void" Variadic="true" />  ; A Void variadic parameter, if a variadic parameter type is 'Void' than it accepts anything.  (it’s like 'params object[] Value' in CSharp)



Parameter node rules are very straight forward:

1.) The 'Name' attribute must use valid characters for an LSL parameter definition or a syntax error will occur.
2.) Parameter names must be unique.
3.) Parameters that are not variadic cannot have the Type 'Void', that’s an error.
4.) Variadic Parameters can only be the last parameter of a function definition, if you define more parameters after a variadic parameter an error will be thrown by the loader.


/// DocumentationString Node (Child) ///


Pretty self-explanatory, this can contain documentation for the function.  It's not a required node, but LSLCCEditor uses it.


/// Property Node's (Children) ///


Every library signature type has a property dictionary attached to it, so that you can add arbitrary data to a definition as a Name/Value pair.

For example, LibLSLCC's OpenSim code generator uses the LibraryFunction property:

	<Property Name="ModInvoke" Value="true">


If the code generator sees that this property exists and its value is 'true' (Case Insensitive), than the appropriate 'modInvoke' function 
from the OpenSim modInvoke module will be used to invoke the function by name in the generated code.



### NOTES about LibraryFunction ###

========
What’s considered a duplicate function definition?
====

Two library function definitions in the same subset are considered to be duplicate if any of the following apply:

1.) The two functions have no parameters and the same name.
2.) The two functions have the same name, equal parameter count, and all the parameter types are the same (including the variadic ones).

Return type is not considered, since it cannot be responsible for making a function unique during overload resolution,
only the function parameter types can.


========
Can I define Function Overloads?
===

	Yes, you can create overloads of functions that exist within the same library subset.

	This was implemented so that compiled code has the ability to call overloaded functions in whatever runtime its targeting.
	
	The algorithm used for picking the correct function overload behaves identically to how CSharp overload resolution behaves,
	With some exceptions...  LSL 'list' do not 'fill out' variadic parameters, and the only forms of signature match degradation
	are implicit conversions between types such as 'key' <-> 'string' or 'integer' -> 'float'.

	Conversion of types into the 'Void' type used for variadic Void parameters is also considered an implicit conversion.
	
	The algorithm is as follows:
	
	1.) The algorithm finds a set of candidates, or possible overload matches, there may be only one or none at all in some cases.
		A candidate is considered to be any function signature which can be successfully called with the given parameter expressions.
		From this set, the 'best' or most 'specific' function signature needs to be further narrowed down if there is more than one match.

	2.) If the previous step returns only one match, the algorithm has found an exact overload match and it's returned.  Otherwise....

	3.) If there are multiple matches an no parameters have been passed, there must be an overload with a single variadic parameter.
		The definition that contains no parameters at all is preferred and returned as a match.  Otherwise...

	== 
	
	4.) The algorithm picks the most 'specific' signature out of the multiple match candidates.
		This is done by ranking all possible matches by the amount of implicit conversions required for the match to succeed.
		A grouping algorithm is used to group together match candidates who share the same rank (IE, the number of implicit conversions required).
	    
	5.) The amount of signature groups with the same ranking (number of implicit conversions) is checked.
	    if there is only one group, that means that every match candidate in the group has the same rank, and no overload resolution is possible
	    because no overload is a 'better' choice out of the other ones.  The algorithm returns multiple matches in that case which causes an ambiguity syntax error.  
		Otherwise, if there is more than one group....

	==
	
	6.) The signature group with the lowest rank is found. (The whose signatures require the least implicit conversions to match)
		The smallest group is actually found in the previous step when the signatures are grouped by rank, but it only comes into play here.
	
	7.) If the signature group with the lowest rank only contains a single signature, that signature is returned as an un-ambiguous match.
		Otherwise...
	
	8.) If the signature group with the lowest rank contains multiple signatures, the signature with the closest amount of concrete (non-variadic) parameters
		to the number of expressions being passed is selected as an un-ambiguous match.
	
	===


	Say we have these defined:
	
	1.)	void Func(string str);
	2.)	void Func(key k);
	3.)	void Func(key k, string str, key k2);
	4.)	void Func(key k, string str, string str);
		
		
	Here's some sample overload resolutions:
	
		Func(""); -> matches: ------------------------ void function(string str);  // because "" is a string literal and no implicit conversion was required, it’s an exact match.
		
		Func((key)""); -> matches: ------------------- void function(key k);       // because the passed type was is a key, and it’s the best match for a key parameter since no implicit conversion is required.
		
		Func((key)"",(key)"", ""); -> matches: ------- void Func(key k, string str, string str);  // because signature #4 requires only 1 implicit conversion and signature #3 requires 2.
		
		Func("", "", ""); -> matches: ---------------- void Func(key k, string str, string str);  // again because signature #4 requires only 1 implicit conversion and signature #3 requires 2.
		
		Func((key)"", (key)"", (key)""); -> matches: - void Func(key k, string str, key k2);      // because signature #3 requires only 1 implicit conversion and signature #4 requires 2.
	
	===
	
	Here are some good examples involving variadic parameters, given the following definitions:
	
	1.) void Func();
	2.) void Func(params void[] i);
    3.) void Func(params integer[] i);
	4.)	void Func(key i, params string[] s);
	5.) void Func(string i, params string[] s);
    6.) void Func(int i, params int[] i2);
    7.) void Func(int i,  int i2, params string[] i3);
		
	And for the given calls:
	
		Func(); -> matches: -------------- void Func();   //the path of least resistance, functions without parameters are preferred over functions with a single variadic parameter.
		
		Func(0); -> matches: ------------- void Func(int i, params int[] i2);  //It matches because the second parameter is allowed to be empty, and the one concrete parameter is more specific than 'params void[] i' or 'params integer[] i'
																		       //in definition #2 and #3.  This surprises a lot of people who think it should match #3, try it in CSharp and see!
																
		Func(""); -> matches: ------------ void Func(string i, params string[] s);  //Same principle as the last match, it matches the overload with the 'string' parameter first because "" implicitly converts to a key, but 'string' is more "specific".
																				    //definition #2 is ignored because it is the most generic alternative.
																	 
		Func("", (key)""); -> matches: --- void Func(string i, params string[] s);  //because signature #4 requires 2 implicit conversions to match and #5 requires only 1;
		
		Func(0,0); -> matches: ----------- void Func(int i,  int i2, params string[] i3);  //because the third parameter does not have to exist, and more concrete parameters (non-variadic) match than in definition #6, #3 and #2
		
	===
	
	

Sometimes variadic function parameters can cause non-duplicate function overloads to easily become ambiguous at the call site.

For instance, take these declarations:

	void Func(string one, params string[] two);

	void Func(string one, params void[] two);

	

	Func(""); -> ambiguous match, won't compile.
	
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


Identical purpose to the Subset's attribute on LibraryFunctionSignature Nodes discussed above.


@@@ Value Attribute @@@

The value associated with the constant.

	Formatting is as follows:
	
	Vector:   Value="0.0,0.0,0.0"    // you should not include the greater and less than signs that enclose the vector, they will just be removed anyway.
									 // you can use integer component's to, as well as positive and negative prefixes on the component values.
	
	Rotation: Value="0.0,0.0,0.0,0.0"  // Same Idea with Rotation's, don't enclose the rotation with greater and less than signs, if you do they just get removed anyway.
	
	Float:    Value="1.333333333333"   // Same as LSL Syntax,  Floats can also be specified in hexadecimal format. (hexadecimal format will be preserved in the Value string, its not 'converted' upon parsing.)

	Integer:  Value="1"	  //Same as LSL Syntax,  Integers can also be specified in hexadecimal format (hexadecimal format will be preserved in the Value string, its not 'converted' upon parsing.)
	
	String:   Value="hello world"  // Do not include quotes unless you want them in the string, you can use normal XML escaping to specify any special characters, or paste them in as raw characters.
								   // You should save your File as UTF16 (Unicode) if you want to be able to use all the raw characters that LSL and the LibLSLCC compiler actually supports.
								   
	Key:      Value="UUID"  //Identical formatting to String, do not include quotes unless you want them in the key string.


	List:     Value="0 , &quot;hello world&quot; , &lt;0,0,0&gt; , 0.0 , (key)&quot;I'm a key type&quot"
			
			  //That is a list value that contains the list: [0, "hello world", <0,0,0>, 0.0, (key)"I'm a key type"].  
			  //Some of the characters used obviously need to be XML escaped as you can see above.

			  //List should not be wrapped in [] symbol's;  They get removed anyway when the value string is parsed, validated and formated.
			  //Only literal expression values can be parsed out of the string,  if you add variable references, function calls, nested lists or other miscellaneous expressions the loader will throw a syntax error.
			  //You can use the cast expression (key)"" to specify a key typed value;  You can also the negate/positive prefix operators on Float and Integer values that are not hexadecimal.


===	
	

/// DocumentationString Node (Child) ///


Pretty self-explanatory, this can contain documentation for the function.  It's not a required node, but LSLCCEditor uses it.


/// Property Node (Children) ///


The same Name/Value dictionary that exists in all definition nodes. LibLSLCC editor uses the 'Deprecated' property to change the
highlighting of deprecated constants in the editor, as well as the formatting in the documentation tool-tip.

The 'Expand' property is used by the OpenSim code generator, if 'Expand' is set to 'true' then the constants value will be expanded into the
generated source code much in the same way a C preprocessor would expand preprocessor definitions.  The value will be placed into the generated 
source code as a code literal that is appropriate for the constants associated type.  If 'Expand' is false, the constant will be created as a
reference to a static variable that exists in the runtime base class of the generated script object.


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
	If you try to use it, an unknown attribute syntax error will be thrown by the loader.


#Notes:


The 'Deprecated' property is only used by LSLCCEditor to mark events as deprecated in their documentation string.
Currently there are not really any deprecated event handlers that exist in standard LSL. 
But who knows, maybe someday in LSL, or for someone else's runtime implementation there will be.


====================================================================
====================================================================
====================================================================
















