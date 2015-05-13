grammar LSL;

@lexer::header {
	using System.Collections.Generic;
	using LibLSLCC.CodeValidator.Primitives;
}

@lexer::members {

	public List<LSLComment> Comments = new List<LSLComment>();

    private struct LineCount
    {
        public int Lines;
        public int EndColumn;
    }
    private static LineCount CountStringLines(int startColumn, string str)
    {
		int cnt=0;
        int lastLineStart = 0;
        int i = 0;
        int endColumn;
		foreach(var c in str){
			if(c == '\n'){
				cnt++;
                lastLineStart = i+1;
			}
            i++;
		}

        if (lastLineStart == 0)
        {
            endColumn = (startColumn + str.Length) - 1;
        }
        else
        {
            endColumn = (i - lastLineStart) - 1;
        }
        return new LineCount { Lines = cnt, EndColumn = endColumn };
	}

}

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


fragment
NameChar
   : NameStartChar
   | '0'..'9'
   | '_'
   | '\u00B7'
   | '\u0300'..'\u036F'
   | '\u203F'..'\u2040'
   ;
fragment
NameStartChar
   : 'A'..'Z' | 'a'..'z'
   | '\u00C0'..'\u00D6'
   | '\u00D8'..'\u00F6'
   | '\u00F8'..'\u02FF'
   | '\u0370'..'\u037D'
   | '\u037F'..'\u1FFF'
   | '\u200C'..'\u200D'
   | '\u2070'..'\u218F'
   | '\u2C00'..'\u2FEF'
   | '\u3001'..'\uD7FF'
   | '\uF900'..'\uFDCF'
   | '\uFDF0'..'\uFFFD'
   ;

ID : NameStartChar NameChar*;

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
	LESS_THAN vector_x=expression comma_one=COMMA vector_y=expression comma_two=COMMA vector_z=expression GREATER_THAN
	;

rotationLiteral:
	LESS_THAN rotation_x=expression comma_one=COMMA rotation_y=expression comma_two=COMMA rotation_z=expression comma_three=COMMA rotation_s=expression GREATER_THAN
	;



functionDeclaration:
	return_type=TYPE? function_name=ID open_parenth=O_PAREN parameters=optionalParameterList close_parenth=C_PAREN code=codeScope
	;


//declarations are not allowed in single statement blocks
//but this case is handled in code
codeScopeOrSingleBlockStatement: 
	(code=codeScope|statement=codeStatement);




//condition is optional so an error can be provided when its missing
elseIfStatement:
	else_keyword=ELSE if_keyword=IF open_parenth=O_PAREN condition=expression? close_parenth=C_PAREN code=codeScopeOrSingleBlockStatement
	;


elseStatement:
	else_keyword=ELSE code=codeScopeOrSingleBlockStatement
	;

//condition is optional so an error can be provided when its missing
ifStatement:
	if_keyword=IF open_parenth=O_PAREN condition=expression? close_parenth=C_PAREN code=codeScopeOrSingleBlockStatement
	;


controlStructure:
	ifStatement (((elseIfStatement)+ (elseStatement)?) | (elseStatement))?
	;


codeScope:
	open_brace=O_BRACE codeStatement* close_brace=C_BRACE
	;

//loop_condition is optional so an error can be provided when its missing
doLoop:
	loop_keyword=DO code=codeScopeOrSingleBlockStatement while_keyword=WHILE open_parenth=O_PAREN loop_condition=expression? close_parenth=C_PAREN semi_colon=SEMI_COLON
	;


//loop_condition is optional so an error can be provided when its missing
whileLoop:
	loop_keyword=WHILE open_parenth=O_PAREN loop_condition=expression? close_parenth=C_PAREN code=codeScopeOrSingleBlockStatement
	;


//for loops without a loop condition are allowed
forLoop:
	loop_keyword=FOR open_parenth=O_PAREN loop_init=optionalExpressionList first_semi_colon=SEMI_COLON loop_condition=expression? second_semi_colon=SEMI_COLON expression_list=optionalExpressionList close_parenth=C_PAREN code=codeScopeOrSingleBlockStatement
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
	expression_rule=expression semi_colon=SEMI_COLON
	;

returnStatement: 
                   return_keyword=RETURN return_expression=expression semi_colon=SEMI_COLON
               |   return_keyword=RETURN semi_colon=SEMI_COLON ;

labelStatement:
	label_prefix=LABEL_PREFIX label_name=ID semi_colon=SEMI_COLON
	;


jumpStatement:
	jump_keyword=JUMP jump_target=ID semi_colon=SEMI_COLON
	;

stateChangeStatement:
	state_keyword=STATE state_target=(ID|DEFAULT) semi_colon=SEMI_COLON
	;


localVariableDeclaration: 
                     variable_type=TYPE variable_name=ID operation = EQUAL variable_value=expression semi_colon = SEMI_COLON 
                   | variable_type=TYPE variable_name=ID semi_colon=SEMI_COLON
				   ;

globalVariableDeclaration:
					 variable_type=TYPE variable_name=ID operation = EQUAL variable_value=expression semi_colon = SEMI_COLON 
                   | variable_type=TYPE variable_name=ID semi_colon = SEMI_COLON
				   ;



//this is so I can easily get full source code interval info, including line number and column information for commas. since in this configuration
//the comma and next expression are a single node, and .comma is a member of the node (an IToken object)
expressionListTail:
	comma=COMMA expression;


expressionList:
	expression (expressionListTail)*
	;


//probably not correct, seems to get the tree I need though
//LSL has weird operator precedence that I have not fully figured out
expression:


    O_PAREN expr_value=expression C_PAREN													 #ParenthesizedExpression

|   function_name=ID (open_parenth=O_PAREN  expression_list=optionalExpressionList close_parenth=C_PAREN) #Expr_FunctionCall

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
	parameter_type=TYPE parameter_name=ID
	;

parameterList: 
	parameterDefinition (COMMA parameterDefinition )*
	;

eventHandler:
	handler_name=ID open_parenth=O_PAREN parameters=optionalParameterList close_parenth=C_PAREN code=codeScope
	;


/* 
"default" is allowed here just so a more specific error can be given about it,
instead of a parser token type mismatch
*/
definedState:
	STATE state_name=(ID|DEFAULT) open_brace=O_BRACE eventHandler* close_brace=C_BRACE
	;



defaultState:
	state_name=DEFAULT open_brace=O_BRACE eventHandler* close_brace=C_BRACE
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
: '/*' .*? '*/' {
                    var lineData = CountStringLines(this.TokenStartColumn, this.Text);
					Comments.Add(new LSLComment()
					{
						Text = this.Text, 
                        SourceCodeRange = new LSLSourceCodeRange(
                            this.TokenStartLine,
                            this.TokenStartColumn,
                            this.TokenStartLine + lineData.Lines, 
                            lineData.EndColumn, 
                            this.TokenStartCharIndex, 
                            this.Text.Length+this.TokenStartCharIndex),
							Type = LSLCommentType.Block
					});
				} -> channel(HIDDEN)
;

LineComment
: '//' ~[\r\n]* {
                    var lineData = CountStringLines(this.TokenStartColumn, this.Text);
					Comments.Add(new LSLComment()
					{
						Text = this.Text, 
                        SourceCodeRange = new LSLSourceCodeRange(
                            this.TokenStartLine,
                            this.TokenStartColumn,
                            this.TokenStartLine + lineData.Lines, 
                            lineData.EndColumn, 
                            this.TokenStartCharIndex, 
                            this.Text.Length+this.TokenStartCharIndex),
							Type = LSLCommentType.SingleLine
					});
				} -> channel(HIDDEN)
;