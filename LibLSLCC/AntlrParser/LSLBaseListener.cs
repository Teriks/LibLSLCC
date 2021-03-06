//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.6
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from AntlrParser\LSL.g4 by ANTLR 4.6

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
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="ILSLListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.6")]
[System.CLSCompliant(false)]
public partial class LSLBaseListener : ILSLListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.vectorLiteral"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVectorLiteral([NotNull] LSLParser.VectorLiteralContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.vectorLiteral"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVectorLiteral([NotNull] LSLParser.VectorLiteralContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.rotationLiteral"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRotationLiteral([NotNull] LSLParser.RotationLiteralContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.rotationLiteral"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRotationLiteral([NotNull] LSLParser.RotationLiteralContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.functionDeclaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFunctionDeclaration([NotNull] LSLParser.FunctionDeclarationContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.functionDeclaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFunctionDeclaration([NotNull] LSLParser.FunctionDeclarationContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.elseStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterElseStatement([NotNull] LSLParser.ElseStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.elseStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitElseStatement([NotNull] LSLParser.ElseStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.controlStructure"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterControlStructure([NotNull] LSLParser.ControlStructureContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.controlStructure"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitControlStructure([NotNull] LSLParser.ControlStructureContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.codeScope"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCodeScope([NotNull] LSLParser.CodeScopeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.codeScope"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCodeScope([NotNull] LSLParser.CodeScopeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.doLoop"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDoLoop([NotNull] LSLParser.DoLoopContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.doLoop"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDoLoop([NotNull] LSLParser.DoLoopContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.whileLoop"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWhileLoop([NotNull] LSLParser.WhileLoopContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.whileLoop"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWhileLoop([NotNull] LSLParser.WhileLoopContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.forLoop"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterForLoop([NotNull] LSLParser.ForLoopContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.forLoop"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitForLoop([NotNull] LSLParser.ForLoopContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.loopStructure"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLoopStructure([NotNull] LSLParser.LoopStructureContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.loopStructure"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLoopStructure([NotNull] LSLParser.LoopStructureContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.codeStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCodeStatement([NotNull] LSLParser.CodeStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.codeStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCodeStatement([NotNull] LSLParser.CodeStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.expressionStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpressionStatement([NotNull] LSLParser.ExpressionStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.expressionStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpressionStatement([NotNull] LSLParser.ExpressionStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.returnStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReturnStatement([NotNull] LSLParser.ReturnStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.returnStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReturnStatement([NotNull] LSLParser.ReturnStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.labelStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLabelStatement([NotNull] LSLParser.LabelStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.labelStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLabelStatement([NotNull] LSLParser.LabelStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.jumpStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterJumpStatement([NotNull] LSLParser.JumpStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.jumpStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitJumpStatement([NotNull] LSLParser.JumpStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.stateChangeStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStateChangeStatement([NotNull] LSLParser.StateChangeStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.stateChangeStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStateChangeStatement([NotNull] LSLParser.StateChangeStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.localVariableDeclaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLocalVariableDeclaration([NotNull] LSLParser.LocalVariableDeclarationContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.localVariableDeclaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLocalVariableDeclaration([NotNull] LSLParser.LocalVariableDeclarationContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.globalVariableDeclaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterGlobalVariableDeclaration([NotNull] LSLParser.GlobalVariableDeclarationContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.globalVariableDeclaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitGlobalVariableDeclaration([NotNull] LSLParser.GlobalVariableDeclarationContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.expressionListTail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpressionListTail([NotNull] LSLParser.ExpressionListTailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.expressionListTail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpressionListTail([NotNull] LSLParser.ExpressionListTailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.expressionList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpressionList([NotNull] LSLParser.ExpressionListContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.expressionList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpressionList([NotNull] LSLParser.ExpressionListContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.dotAccessorExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDotAccessorExpr([NotNull] LSLParser.DotAccessorExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.dotAccessorExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDotAccessorExpr([NotNull] LSLParser.DotAccessorExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.modifiableLeftValue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterModifiableLeftValue([NotNull] LSLParser.ModifiableLeftValueContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.modifiableLeftValue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitModifiableLeftValue([NotNull] LSLParser.ModifiableLeftValueContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>ParenthesizedExpression</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParenthesizedExpression([NotNull] LSLParser.ParenthesizedExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>ParenthesizedExpression</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParenthesizedExpression([NotNull] LSLParser.ParenthesizedExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_PrefixOperation</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_PrefixOperation([NotNull] LSLParser.Expr_PrefixOperationContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_PrefixOperation</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_PrefixOperation([NotNull] LSLParser.Expr_PrefixOperationContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_Atom</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_Atom([NotNull] LSLParser.Expr_AtomContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_Atom</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_Atom([NotNull] LSLParser.Expr_AtomContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_TypeCast</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_TypeCast([NotNull] LSLParser.Expr_TypeCastContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_TypeCast</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_TypeCast([NotNull] LSLParser.Expr_TypeCastContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_DotAccessorExpr</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_DotAccessorExpr([NotNull] LSLParser.Expr_DotAccessorExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_DotAccessorExpr</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_DotAccessorExpr([NotNull] LSLParser.Expr_DotAccessorExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_BitwiseShift</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_BitwiseShift([NotNull] LSLParser.Expr_BitwiseShiftContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_BitwiseShift</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_BitwiseShift([NotNull] LSLParser.Expr_BitwiseShiftContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_LogicalCompare</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_LogicalCompare([NotNull] LSLParser.Expr_LogicalCompareContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_LogicalCompare</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_LogicalCompare([NotNull] LSLParser.Expr_LogicalCompareContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_LogicalEquality</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_LogicalEquality([NotNull] LSLParser.Expr_LogicalEqualityContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_LogicalEquality</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_LogicalEquality([NotNull] LSLParser.Expr_LogicalEqualityContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_BitwiseOr</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_BitwiseOr([NotNull] LSLParser.Expr_BitwiseOrContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_BitwiseOr</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_BitwiseOr([NotNull] LSLParser.Expr_BitwiseOrContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_Logical_And_Or</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_Logical_And_Or([NotNull] LSLParser.Expr_Logical_And_OrContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_Logical_And_Or</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_Logical_And_Or([NotNull] LSLParser.Expr_Logical_And_OrContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_BitwiseXor</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_BitwiseXor([NotNull] LSLParser.Expr_BitwiseXorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_BitwiseXor</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_BitwiseXor([NotNull] LSLParser.Expr_BitwiseXorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_Assignment</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_Assignment([NotNull] LSLParser.Expr_AssignmentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_Assignment</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_Assignment([NotNull] LSLParser.Expr_AssignmentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_MultDivMod</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_MultDivMod([NotNull] LSLParser.Expr_MultDivModContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_MultDivMod</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_MultDivMod([NotNull] LSLParser.Expr_MultDivModContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_BitwiseAnd</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_BitwiseAnd([NotNull] LSLParser.Expr_BitwiseAndContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_BitwiseAnd</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_BitwiseAnd([NotNull] LSLParser.Expr_BitwiseAndContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_PostfixOperation</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_PostfixOperation([NotNull] LSLParser.Expr_PostfixOperationContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_PostfixOperation</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_PostfixOperation([NotNull] LSLParser.Expr_PostfixOperationContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_FunctionCall</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_FunctionCall([NotNull] LSLParser.Expr_FunctionCallContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_FunctionCall</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_FunctionCall([NotNull] LSLParser.Expr_FunctionCallContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_ModifyingAssignment</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_ModifyingAssignment([NotNull] LSLParser.Expr_ModifyingAssignmentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_ModifyingAssignment</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_ModifyingAssignment([NotNull] LSLParser.Expr_ModifyingAssignmentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Expr_AddSub</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr_AddSub([NotNull] LSLParser.Expr_AddSubContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Expr_AddSub</c>
	/// labeled alternative in <see cref="LSLParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr_AddSub([NotNull] LSLParser.Expr_AddSubContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.optionalExpressionList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOptionalExpressionList([NotNull] LSLParser.OptionalExpressionListContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.optionalExpressionList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOptionalExpressionList([NotNull] LSLParser.OptionalExpressionListContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.optionalParameterList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOptionalParameterList([NotNull] LSLParser.OptionalParameterListContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.optionalParameterList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOptionalParameterList([NotNull] LSLParser.OptionalParameterListContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.listLiteral"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterListLiteral([NotNull] LSLParser.ListLiteralContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.listLiteral"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitListLiteral([NotNull] LSLParser.ListLiteralContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.compilationUnit"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCompilationUnit([NotNull] LSLParser.CompilationUnitContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.compilationUnit"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCompilationUnit([NotNull] LSLParser.CompilationUnitContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.parameterDefinition"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParameterDefinition([NotNull] LSLParser.ParameterDefinitionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.parameterDefinition"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParameterDefinition([NotNull] LSLParser.ParameterDefinitionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.parameterList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParameterList([NotNull] LSLParser.ParameterListContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.parameterList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParameterList([NotNull] LSLParser.ParameterListContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.eventHandler"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterEventHandler([NotNull] LSLParser.EventHandlerContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.eventHandler"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitEventHandler([NotNull] LSLParser.EventHandlerContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.definedState"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDefinedState([NotNull] LSLParser.DefinedStateContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.definedState"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDefinedState([NotNull] LSLParser.DefinedStateContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LSLParser.defaultState"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDefaultState([NotNull] LSLParser.DefaultStateContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LSLParser.defaultState"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDefaultState([NotNull] LSLParser.DefaultStateContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}
} // namespace LibLSLCC.AntlrParser
