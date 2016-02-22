using System.Collections.Generic;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;

namespace LibLSLCC.AutoComplete
{
    /// <summary>
    /// Interface for <see cref="LSLAutoCompleteParser"/>'s parse state.
    /// </summary>
    public interface ILSLAutoCompleteParserState
    {
        /// <summary>
        ///     The offset the <see cref="LSLAutoCompleteParser" /> last parsed to.
        /// </summary>
        int ParseToOffset { get; }

        /// <summary>
        ///     The name of the state block <see cref="ParseToOffset" /> resides in.
        ///     <c>null</c> if the parse to offset is outside of a state body.
        /// </summary>
        string CurrentState { get; }

        /// <summary>
        ///     The name of the function declaration <see cref="ParseToOffset" /> resides in.
        ///     <c>null</c> if the parse to offset is outside of a function body.
        /// </summary>
        string CurrentFunction { get; }

        /// <summary>
        ///     The name of the event handler declaration <see cref="ParseToOffset" /> offset resides in.
        ///     <c>null</c> if the parse to offset is outside of an event body.
        /// </summary>
        string CurrentEvent { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a state block, but outside of an event handler
        ///     declaration.
        /// </summary>
        bool InStateScope { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the source code range of an event handler.
        ///     This includes being within the name or parameter definitions.
        /// </summary>
        bool InEventSourceRange { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside the code body of an event handler.
        /// </summary>
        bool InEventCodeBody { get;
            // ReSharper disable once FunctionComplexityOverflow
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside the code body of a function declaration.
        /// </summary>
        bool InFunctionCodeBody { get;
            // ReSharper disable once FunctionComplexityOverflow
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the global scope.
        /// </summary>
        bool InGlobalScope { get; }

        /// <summary>
        ///     Gets a list of <see cref="LSLAutoCompleteStateBlock" /> objects representing user defined script state blocks.
        /// </summary>
        IReadOnlyGenericArray<LSLAutoCompleteStateBlock> StateBlocks { get; }

        /// <summary>
        ///     Gets a <see cref="LSLAutoCompleteStateBlock" /> object representing the scripts default state.
        /// </summary>
        LSLAutoCompleteStateBlock DefaultState { get; }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteGlobalVariable" /> objects representing global variables
        ///     that are accessible at <see cref="ParseToOffset" />.
        /// </summary>
        IEnumerable<LSLAutoCompleteGlobalVariable> GlobalVariables { get; }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteLocalVariable" /> objects representing local variables
        ///     that are accessible at <see cref="ParseToOffset" />.
        /// </summary>
        IEnumerable<LSLAutoCompleteLocalVariable> LocalVariables { get; }

        /// <summary>
        ///     Gets a read only hash map of <see cref="LSLAutoCompleteGlobalFunction" /> objects representing global function
        ///     declarations
        ///     that are accessible at <see cref="ParseToOffset" />.  The functions are keyed in the hash map by name.
        /// </summary>
        IReadOnlyHashMap<string, LSLAutoCompleteGlobalFunction> GlobalFunctionsDictionary { get; }

        /// <summary>
        ///     Gets a read only hash map of <see cref="LSLAutoCompleteGlobalVariable" /> objects representing global variable
        ///     declarations
        ///     that are accessible at <see cref="ParseToOffset" />.  The declarations are keyed in the hash map by name.
        /// </summary>
        IReadOnlyHashMap<string, LSLAutoCompleteGlobalVariable> GlobalVariablesDictionary { get; }

        /// <summary>
        ///     Gets a read only hash map of <see cref="LSLAutoCompleteLocalParameter" /> objects representing local parameter
        ///     declarations
        ///     that are accessible at <see cref="ParseToOffset" />.  The declarations are keyed in the hash map by name.
        /// </summary>
        IReadOnlyHashMap<string, LSLAutoCompleteLocalParameter> LocalParametersDictionary { get; }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteGlobalFunction" /> objects representing global functions
        ///     that are accessible at <see cref="ParseToOffset" />.
        /// </summary>
        IEnumerable<LSLAutoCompleteGlobalFunction> GlobalFunctions { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in an area where a code statement can exist.
        ///     (<see cref="InMultiCodeStatementArea" /> || <see cref="InBracelessCodeStatementArea" />)
        /// </summary>
        bool InCodeStatementArea { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in a multi statement area where a code statement can exist.
        /// </summary>
        bool InMultiCodeStatementArea { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in an area where a brace-less code scope (single statement) can
        ///     exist.
        /// </summary>
        bool InBracelessCodeStatementArea { get; }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteGlobalFunction" /> objects representing local parameters
        ///     that are accessible at <see cref="ParseToOffset" />.
        /// </summary>
        IEnumerable<LSLAutoCompleteLocalParameter> LocalParameters { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a binary expression/prefix expression/postfix expression
        ///     or parenthesized expression.
        /// </summary>
        bool InBasicExpressionTree { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is to the right of the dot in a dot member accessor expression.
        /// </summary>
        bool RightOfDotAccessor { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of the expression used to declare a local variable.
        /// </summary>
        bool InLocalVariableDeclarationExpression { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of the expression used to declare a global variable.
        /// </summary>
        bool InGlobalVariableDeclarationExpression { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a function declarations parameter declaration list.
        /// </summary>
        bool InFunctionDeclarationParameterList { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of an event declarations parameter declaration list.
        /// </summary>
        bool InEventDeclarationParameterList { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of an 'if' statements condition expression area.
        /// </summary>
        bool InIfConditionExpression { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of an 'else if' statements condition expression area.
        /// </summary>
        bool InElseIfConditionExpression { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a function declaration or event declaration code body.
        /// </summary>
        bool InCodeBody { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in a local expression area, such as a condition area or function
        ///     call arguments. etc..
        /// </summary>
        bool InLocalExpressionArea { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in a global area. currently only when
        ///     <see cref="InGlobalVariableDeclarationExpression" /> is <c>true</c>.
        /// </summary>
        bool InGlobalExpressionArea { get; }

        /// <summary>
        ///     <see cref="InGlobalExpressionArea" /> || <see cref="InLocalExpressionArea" />.
        ///     If the offset is in a global variable declaration expression, or the start of one.  Or
        ///     a local expression area such as an expression statement, loop condition, function call parameters, for loop clauses
        ///     etc..
        /// </summary>
        bool InExpressionArea { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the expression area of a return statement inside of a function.
        /// </summary>
        bool InFunctionReturnExpression { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the expression area right of a compound operation/assignment to a
        ///     variable, such as after var += (here).
        /// </summary>
        bool InModifyingVariableAssignmentExpression { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the expression area right of a compound operation/assignment to a
        ///     member of a variable, such as after var.x += (here).
        /// </summary>
        bool InModifyingComponentAssignmentExpression { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the expression area right of an assignment to a variable, such as
        ///     after var = (here).
        /// </summary>
        bool InPlainVariableAssignmentExpression { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the expression area right of an assignment to a member of a
        ///     variable, such as after var.x = (here).
        /// </summary>
        bool InPlainComponentAssignmentExpression { get; }

        /// <summary>
        ///     <see cref="InPlainVariableAssignmentExpression" /> || <see cref="InModifyingVariableAssignmentExpression" />
        /// </summary>
        bool InVariableAssignmentExpression { get; }

        /// <summary>
        ///     <see cref="InPlainComponentAssignmentExpression" /> || <see cref="InModifyingComponentAssignmentExpression" />
        /// </summary>
        bool InComponentAssignmentExpression { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in an area where you could start typing the name of the state in a
        ///     state change statement.
        /// </summary>
        bool InStateChangeStatementStateNameArea { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in an area where you could start typing the name of the label in a
        ///     jump statement.
        /// </summary>
        bool InJumpStatementLabelNameArea { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in an area where you could start typing the name of a label.
        /// </summary>
        bool InLabelDefinitionNameArea { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is anywhere in a for loops clauses area.
        /// </summary>
        bool InForLoopClausesArea { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in a do while statements condition area.
        /// </summary>
        bool InDoWhileConditionExpression { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in a while statements condition area.
        /// </summary>
        bool InWhileConditionExpression { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is in the source code range of a control statement.
        /// </summary>
        bool InControlStatementSourceRange { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a function calls parameter expression list.
        /// </summary>
        bool InFunctionCallParameterList { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a list literals initializer expression list.
        /// </summary>
        bool InListLiteralInitializer { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a vector literals initializer expression list.
        /// </summary>
        bool InVectorLiteralInitializer { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is inside of a rotation literals initializer expression list.
        /// </summary>
        bool InRotationLiteralInitializer { get; }

        /// <summary>
        ///     <c>true</c> if a library constant can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InExpressionArea" />
        /// </summary>
        bool CanSuggestLibraryConstant { get; }

        /// <summary>
        ///     <c>true</c> if a function call can be suggested at <see cref="ParseToOffset" />.
        ///     (<see cref="InLocalExpressionArea" /> || <see cref="InCodeStatementArea" />)
        /// </summary>
        bool CanSuggestFunction { get; }

        /// <summary>
        ///     <c>true</c> if a local variable or parameter name can be suggested at <see cref="ParseToOffset" />.
        ///     (<see cref="InLocalExpressionArea" /> || <see cref="InCodeStatementArea" />)
        /// </summary>
        bool CanSuggestLocalVariableOrParameter { get; }

        /// <summary>
        ///     <c>true</c> if a global variable can be suggested at <see cref="ParseToOffset" />.
        ///     (<see cref="InLocalExpressionArea" /> || <see cref="InCodeStatementArea" />)
        /// </summary>
        bool CanSuggestGlobalVariable { get; }

        /// <summary>
        ///     <c>true</c> if an event handler can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InStateScope" />
        /// </summary>
        bool CanSuggestEventHandler { get; }

        /// <summary>
        ///     <c>true</c> if a state name can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InStateChangeStatementStateNameArea" />
        /// </summary>
        bool CanSuggestStateName { get; }

        /// <summary>
        ///     <c>true</c> if a label name for a jump target can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InJumpStatementLabelNameArea" />
        /// </summary>
        bool CanSuggestLabelNameJumpTarget { get; }

        /// <summary>
        ///     <c>true</c> if a label definitions name can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InLabelDefinitionNameArea" />
        /// </summary>
        bool CanSuggestLabelNameDefinition { get; }

        /// <summary>
        ///     <c>true</c> if an LSL type name can be suggested at <see cref="ParseToOffset" />.
        /// </summary>
        bool CanSuggestTypeName { get; }

        /// <summary>
        ///     Gets the computed scope address at <see cref="ParseToOffset" />.
        /// </summary>
        LSLAutoCompleteScopeAddress ScopeAddressAtOffset { get; }

        /// <summary>
        ///     Gets the source code range of the code body <see cref="ParseToOffset" /> exists inside of.
        /// </summary>
        LSLSourceCodeRange CurrentCodeAreaRange { get; }

        /// <summary>
        ///     <c>true</c> if <see cref="ParseToOffset" /> is after an 'if' or 'else if' statements code body.
        /// </summary>
        bool AfterIfOrElseIfStatement { get; }

        /// <summary>
        ///     <c>true</c> if a control statement chain can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InCodeStatementArea" />
        /// </summary>
        bool CanSuggestControlStatement { get; }

        /// <summary>
        ///     <c>true</c> if a state change statement can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InCodeStatementArea" />
        /// </summary>
        bool CanSuggestStateChangeStatement { get; }

        /// <summary>
        ///     <c>true</c> if a return statement can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InCodeStatementArea" />
        /// </summary>
        bool CanSuggestReturnStatement { get; }

        /// <summary>
        ///     <c>true</c> if a jump statement can be suggested at <see cref="ParseToOffset" />.
        ///     <see cref="InCodeStatementArea" />
        /// </summary>
        bool CanSuggestJumpStatement { get; }

        /// <summary>
        ///     Gets the return type of the function declaration that <see cref="ParseToOffset" /> is currently in the code body
        ///     of.
        /// </summary>
        LSLType CurrentFunctionReturnType { get; }
    }
}