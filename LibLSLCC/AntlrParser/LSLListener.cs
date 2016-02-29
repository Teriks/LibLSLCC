//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.5.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from AntlrParser\LSL.g4 by ANTLR 4.5.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace LibLSLCC.AntlrParser {
using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="LSLParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.5.2")]
[System.CLSCompliant(false)]
public interface ILSLListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.vectorLiteral"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVectorLiteral([NotNull] LSLParser.VectorLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.vectorLiteral"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVectorLiteral([NotNull] LSLParser.VectorLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.rotationLiteral"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRotationLiteral([NotNull] LSLParser.RotationLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.rotationLiteral"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRotationLiteral([NotNull] LSLParser.RotationLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.functionDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunctionDeclaration([NotNull] LSLParser.FunctionDeclarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.functionDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunctionDeclaration([NotNull] LSLParser.FunctionDeclarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.elseStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterElseStatement([NotNull] LSLParser.ElseStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.elseStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitElseStatement([NotNull] LSLParser.ElseStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.controlStructure"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterControlStructure([NotNull] LSLParser.ControlStructureContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.controlStructure"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitControlStructure([NotNull] LSLParser.ControlStructureContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.codeScope"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCodeScope([NotNull] LSLParser.CodeScopeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.codeScope"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCodeScope([NotNull] LSLParser.CodeScopeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.doLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDoLoop([NotNull] LSLParser.DoLoopContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.doLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDoLoop([NotNull] LSLParser.DoLoopContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.whileLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhileLoop([NotNull] LSLParser.WhileLoopContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.whileLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhileLoop([NotNull] LSLParser.WhileLoopContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.forLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForLoop([NotNull] LSLParser.ForLoopContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.forLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForLoop([NotNull] LSLParser.ForLoopContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.loopStructure"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLoopStructure([NotNull] LSLParser.LoopStructureContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.loopStructure"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLoopStructure([NotNull] LSLParser.LoopStructureContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.codeStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCodeStatement([NotNull] LSLParser.CodeStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.codeStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCodeStatement([NotNull] LSLParser.CodeStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.expressionStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpressionStatement([NotNull] LSLParser.ExpressionStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.expressionStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpressionStatement([NotNull] LSLParser.ExpressionStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.returnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturnStatement([NotNull] LSLParser.ReturnStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.returnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturnStatement([NotNull] LSLParser.ReturnStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.labelStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLabelStatement([NotNull] LSLParser.LabelStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.labelStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLabelStatement([NotNull] LSLParser.LabelStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.jumpStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterJumpStatement([NotNull] LSLParser.JumpStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.jumpStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitJumpStatement([NotNull] LSLParser.JumpStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.stateChangeStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStateChangeStatement([NotNull] LSLParser.StateChangeStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.stateChangeStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStateChangeStatement([NotNull] LSLParser.StateChangeStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.localVariableDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLocalVariableDeclaration([NotNull] LSLParser.LocalVariableDeclarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.localVariableDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLocalVariableDeclaration([NotNull] LSLParser.LocalVariableDeclarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.globalVariableDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterGlobalVariableDeclaration([NotNull] LSLParser.GlobalVariableDeclarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.globalVariableDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitGlobalVariableDeclaration([NotNull] LSLParser.GlobalVariableDeclarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.expressionListTail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpressionListTail([NotNull] LSLParser.ExpressionListTailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.expressionListTail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpressionListTail([NotNull] LSLParser.ExpressionListTailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.expressionList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpressionList([NotNull] LSLParser.ExpressionListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.expressionList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpressionList([NotNull] LSLParser.ExpressionListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.dotAccessorExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDotAccessorExpr([NotNull] LSLParser.DotAccessorExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.dotAccessorExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDotAccessorExpr([NotNull] LSLParser.DotAccessorExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.modifiableLeftValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterModifiableLeftValue([NotNull] LSLParser.ModifiableLeftValueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.modifiableLeftValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitModifiableLeftValue([NotNull] LSLParser.ModifiableLeftValueContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_PrefixOperation</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_PrefixOperation([NotNull] LSLParser.Expr_PrefixOperationContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_PrefixOperation</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_PrefixOperation([NotNull] LSLParser.Expr_PrefixOperationContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ParenthesizedExpression</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParenthesizedExpression([NotNull] LSLParser.ParenthesizedExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ParenthesizedExpression</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParenthesizedExpression([NotNull] LSLParser.ParenthesizedExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_Atom</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_Atom([NotNull] LSLParser.Expr_AtomContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_Atom</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_Atom([NotNull] LSLParser.Expr_AtomContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_TypeCast</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_TypeCast([NotNull] LSLParser.Expr_TypeCastContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_TypeCast</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_TypeCast([NotNull] LSLParser.Expr_TypeCastContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_DotAccessorExpr</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_DotAccessorExpr([NotNull] LSLParser.Expr_DotAccessorExprContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_DotAccessorExpr</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_DotAccessorExpr([NotNull] LSLParser.Expr_DotAccessorExprContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_BitwiseShift</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_BitwiseShift([NotNull] LSLParser.Expr_BitwiseShiftContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_BitwiseShift</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_BitwiseShift([NotNull] LSLParser.Expr_BitwiseShiftContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_LogicalCompare</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_LogicalCompare([NotNull] LSLParser.Expr_LogicalCompareContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_LogicalCompare</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_LogicalCompare([NotNull] LSLParser.Expr_LogicalCompareContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_LogicalEquality</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_LogicalEquality([NotNull] LSLParser.Expr_LogicalEqualityContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_LogicalEquality</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_LogicalEquality([NotNull] LSLParser.Expr_LogicalEqualityContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_BitwiseOr</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_BitwiseOr([NotNull] LSLParser.Expr_BitwiseOrContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_BitwiseOr</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_BitwiseOr([NotNull] LSLParser.Expr_BitwiseOrContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_Logical_And_Or</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_Logical_And_Or([NotNull] LSLParser.Expr_Logical_And_OrContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_Logical_And_Or</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_Logical_And_Or([NotNull] LSLParser.Expr_Logical_And_OrContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_BitwiseXor</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_BitwiseXor([NotNull] LSLParser.Expr_BitwiseXorContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_BitwiseXor</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_BitwiseXor([NotNull] LSLParser.Expr_BitwiseXorContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_Assignment</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_Assignment([NotNull] LSLParser.Expr_AssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_Assignment</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_Assignment([NotNull] LSLParser.Expr_AssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_MultDivMod</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_MultDivMod([NotNull] LSLParser.Expr_MultDivModContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_MultDivMod</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_MultDivMod([NotNull] LSLParser.Expr_MultDivModContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_BitwiseAnd</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_BitwiseAnd([NotNull] LSLParser.Expr_BitwiseAndContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_BitwiseAnd</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_BitwiseAnd([NotNull] LSLParser.Expr_BitwiseAndContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_PostfixOperation</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_PostfixOperation([NotNull] LSLParser.Expr_PostfixOperationContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_PostfixOperation</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_PostfixOperation([NotNull] LSLParser.Expr_PostfixOperationContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_ModifyingAssignment</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_ModifyingAssignment([NotNull] LSLParser.Expr_ModifyingAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_ModifyingAssignment</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_ModifyingAssignment([NotNull] LSLParser.Expr_ModifyingAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_FunctionCall</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_FunctionCall([NotNull] LSLParser.Expr_FunctionCallContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_FunctionCall</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_FunctionCall([NotNull] LSLParser.Expr_FunctionCallContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_AddSub</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr_AddSub([NotNull] LSLParser.Expr_AddSubContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_AddSub</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr_AddSub([NotNull] LSLParser.Expr_AddSubContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.optionalExpressionList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOptionalExpressionList([NotNull] LSLParser.OptionalExpressionListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.optionalExpressionList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOptionalExpressionList([NotNull] LSLParser.OptionalExpressionListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.optionalParameterList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOptionalParameterList([NotNull] LSLParser.OptionalParameterListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.optionalParameterList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOptionalParameterList([NotNull] LSLParser.OptionalParameterListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.listLiteral"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterListLiteral([NotNull] LSLParser.ListLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.listLiteral"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitListLiteral([NotNull] LSLParser.ListLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.compilationUnit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCompilationUnit([NotNull] LSLParser.CompilationUnitContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.compilationUnit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCompilationUnit([NotNull] LSLParser.CompilationUnitContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.parameterDefinition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParameterDefinition([NotNull] LSLParser.ParameterDefinitionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.parameterDefinition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParameterDefinition([NotNull] LSLParser.ParameterDefinitionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.parameterList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParameterList([NotNull] LSLParser.ParameterListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.parameterList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParameterList([NotNull] LSLParser.ParameterListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.eventHandler"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEventHandler([NotNull] LSLParser.EventHandlerContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.eventHandler"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEventHandler([NotNull] LSLParser.EventHandlerContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.definedState"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDefinedState([NotNull] LSLParser.DefinedStateContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.definedState"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDefinedState([NotNull] LSLParser.DefinedStateContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.defaultState"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDefaultState([NotNull] LSLParser.DefaultStateContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.defaultState"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDefaultState([NotNull] LSLParser.DefaultStateContext context);
}
} // namespace LibLSLCC.AntlrParser