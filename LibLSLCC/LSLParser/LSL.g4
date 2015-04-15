
grammar LSL;



TYPE:  'list' | 'vector' | 'float' | 'integer' | 'string' | 'rotation' | 'quaternion' | 'key';

DO: 'do';

IF: 'if';

ELSE: 'else';

WHILE: 'while';

FOR: 'for';

DEFAULT: 'default';

STATE: 'state';

RETURN: 'return';

JUMP: 'jump';

ID : [a-zA-Z_] [a-zA-Z_0-9]*;

HEX_LITERAL
: '0x' [a-fA-F0-9]+
;

INT
: [0-9]+
;


FLOAT
: ((INT '.' INT)|('.' INT)|(INT '.')) (('e'|'E') (PLUS|MINUS) (INT+))? ('f'|'F')?
;

QUOTED_STRING :   '"' ( '\\"' | '\\\\' | ~('"') )*? '"';

SEMI_COLON: ';';

EQUAL: '=';

LOGICAL_EQUAL: '==';

LOGICAL_NOT_EQUAL: '!=';

LESS_THAN: '<';

GREATER_THAN: '>';

LESS_THAN_EQUAL: '<=';

GREATER_THAN_EQUAL: '>=';

RIGHT_SHIFT: '>>';

LEFT_SHIFT: '<<';

RIGHT_SHIFT_EQUAL: '>>=';

LEFT_SHIFT_EQUAL: '<<=';

MINUS: '-';

PLUS: '+';

MINUS_EQUAL: '-=';

PLUS_EQUAL: '+=';

INCREMENT: '++';

DECREMENT: '--';

MUL: '*';

DIV: '/';

MOD: '%';

MUL_EQUAL: '*=';

DIV_EQUAL: '/=';

MOD_EQUAL: '%=';

COMMA: ',';

O_PAREN: '(';

C_PAREN: ')';

O_BRACE: '{';

C_BRACE: '}';

O_BRACKET: '[';

C_BRACKET: ']';

LABEL_PREFIX: '@';

BITWISE_OR: '|';

BITWISE_AND: '&';

BITWISE_NOT: '~';

BITWISE_XOR: '^';

LOGICAL_NOT: '!';

LOGICAL_AND: '&&';

LOGICAL_OR: '||';

DOT: '.';




vectorLiteral:
	LESS_THAN vector_x=expression COMMA vector_y=expression COMMA vector_z=expression GREATER_THAN
	;

rotationLiteral:
	LESS_THAN rotation_x=expression COMMA rotation_y=expression COMMA rotation_z=expression COMMA rotation_s=expression GREATER_THAN
	;



functionDeclaration:
	return_type=TYPE? function_name=ID O_PAREN parameters=optionalParameterList C_PAREN code=codeScope
	;


//declarations are not allowed in single statement blocks
//but this case is handled in code
codeScopeOrSingleBlockStatement: 
	(code=codeScope|statement=codeStatement);




//condition is optional so an error can be provided when its missing
elseIfStatement:
	ELSE IF O_PAREN condition=expression? C_PAREN code=codeScopeOrSingleBlockStatement
	;


elseStatement:
	ELSE code=codeScopeOrSingleBlockStatement
	;

//condition is optional so an error can be provided when its missing
ifStatement:
	IF O_PAREN condition=expression? C_PAREN code=codeScopeOrSingleBlockStatement
	;


controlStructure:
	ifStatement (((elseIfStatement)+ (elseStatement)?) | (elseStatement))?
	;


codeScope:
	O_BRACE codeStatement* C_BRACE
	;

//loop_condition is optional so an error can be provided when its missing
doLoop:
	loop_keyword=DO code=codeScopeOrSingleBlockStatement WHILE O_PAREN loop_condition=expression? C_PAREN SEMI_COLON
	;


//loop_condition is optional so an error can be provided when its missing
whileLoop:
	loop_keyword=WHILE O_PAREN loop_condition=expression? C_PAREN code=codeScopeOrSingleBlockStatement
	;


//for loops without a loop condition are allowed
forLoop:
	loop_keyword=FOR O_PAREN loop_init=expression? SEMI_COLON loop_condition=expression? SEMI_COLON expression_list=optionalExpressionList C_PAREN code=codeScopeOrSingleBlockStatement
	;


loopStructure: 
					do_loop=doLoop
             |      for_loop=forLoop
             |      while_loop=whileLoop
			 ;


codeStatement: 
               variable_declaration=localVariableDeclaration 
             | expression_statement=expressionStatement 
             | return_statement=returnStatement 
             | jump_statement=jumpStatement 
             | label_statement=labelStatement 
             | state_change_statement=stateChangeStatement
             | control_structure=controlStructure
             | loop_structure=loopStructure
			 | code_scope=codeScope
			 | semi_colon=SEMI_COLON
			 ;


expressionStatement:
	expression_rule=expression SEMI_COLON
	;

returnStatement: 
                   RETURN return_expression=expression SEMI_COLON
               |   RETURN SEMI_COLON ;

labelStatement:
	LABEL_PREFIX label_name=ID SEMI_COLON
	;


jumpStatement:
	JUMP jump_target=ID SEMI_COLON
	;

stateChangeStatement:
	STATE state_target=(ID|DEFAULT) SEMI_COLON
	;


localVariableDeclaration: 
                     variable_type=TYPE variable_name=ID EQUAL variable_value=expression SEMI_COLON 
                   | variable_type=TYPE variable_name=ID SEMI_COLON
				   ;

globalVariableDeclaration:
					 variable_type=TYPE variable_name=ID EQUAL variable_value=expression SEMI_COLON 
                   | variable_type=TYPE variable_name=ID SEMI_COLON
				   ;



expressionList:
	expression (COMMA expression)*
	;


//probably not correct, seems to get the tree I need though
//LSL has weird operator precedence that I have not fully figured out
expression:


    O_PAREN expr_value=expression C_PAREN													 #ParenthesizedExpression

|   function_name=ID (O_PAREN  expression_list=optionalExpressionList C_PAREN)               #Expr_FunctionCall

|   expr_lvalue=expression operation=DOT member=ID											 #Expr_DotAccessor

|   expr_lvalue=expression operation=(INCREMENT | DECREMENT )                                #Expr_PostfixOperation

|   operation=(INCREMENT | DECREMENT| MINUS | PLUS| LOGICAL_NOT | BITWISE_NOT) expr_rvalue=expression  #Expr_PrefixOperation

|   (O_PAREN cast_type=TYPE C_PAREN ) expr_rvalue=expression  #Expr_TypeCast

|   expr_lvalue=expression operation=(MUL|DIV|MOD) expr_rvalue=expression             #Expr_MultDivMod
|   expr_lvalue=expression operation=(MINUS|PLUS) expr_rvalue=expression              #Expr_AddSub
|   expr_lvalue=expression operation=(LEFT_SHIFT|RIGHT_SHIFT) expr_rvalue=expression  #Expr_BitwiseShift

|   expr_lvalue=expression operation=(LESS_THAN|GREATER_THAN|GREATER_THAN_EQUAL|LESS_THAN_EQUAL) expr_rvalue=expression  #Expr_LogicalCompare

|   expr_lvalue=expression operation=(LOGICAL_EQUAL|LOGICAL_NOT_EQUAL) expr_rvalue=expression  #Expr_LogicalEquality

|   expr_lvalue=expression (operation=BITWISE_AND) expr_rvalue=expression      #Expr_BitwiseAnd
|   expr_lvalue=expression (operation=BITWISE_XOR) expr_rvalue=expression      #Expr_BitwiseXor
|   expr_lvalue=expression (operation=BITWISE_OR) expr_rvalue=expression       #Expr_BitwiseOr
|   expr_lvalue=expression (operation=LOGICAL_AND) expr_rvalue=expression      #Expr_LogicalAnd
|   expr_lvalue=expression (operation=LOGICAL_OR) expr_rvalue=expression       #Expr_LogicalOr

|   expr_lvalue=expression operation=EQUAL expr_rvalue=expression                                    #Expr_Assignment

|   expr_lvalue=expression operation=(PLUS_EQUAL|MINUS_EQUAL|MUL_EQUAL|DIV_EQUAL|MOD_EQUAL) expr_rvalue=expression #Expr_ModifyingAssignment


|
    (
	 variable=ID
	|string_literal=QUOTED_STRING
	|integer_literal=INT
	|float_literal=FLOAT
	|hex_literal=HEX_LITERAL
	|vector_literal=vectorLiteral
	|rotation_literal=rotationLiteral
	|list_literal=listLiteral
	)  #Expr_Atom
; 




//this is so I can easily get source code intervals for expression lists even when they are empty
//for API consistency
optionalExpressionList:
	expressionList?;



//this is so I can easily get source code intervals for parameter lists even when they are empty
//for API consistency
optionalParameterList:
	parameterList?;



listLiteral:
	O_BRACKET expression_list=optionalExpressionList C_BRACKET
	;


compilationUnit:
	(globalVariableDeclaration|functionDeclaration)* defaultState? definedState*
	;


parameterDefinition:
	TYPE ID
	;

parameterList: 
	parameterDefinition (COMMA parameterDefinition )*
	;

eventHandler:
	handler_name=ID O_PAREN parameters=optionalParameterList C_PAREN code=codeScope
	;


/* 
"default" is allowed here just so a more specific error can be given about it,
instead of a parser token type mismatch
*/
definedState:
	STATE state_name=(ID|DEFAULT) O_BRACE eventHandler* C_BRACE
	;



defaultState:
	state_name=DEFAULT O_BRACE eventHandler* C_BRACE
	;




Whitespace
: [ \t]+ ->skip
;

Newline
: ( '\r' '\n'?
| '\n'
)->skip
;

BlockComment
: '/*' .*? '*/'  ->skip
;

LineComment
: '//' ~[\r\n]* ->skip
;
