using System;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.AntlrTreeUtilitys
{
    internal static class LSLAntlrTreeIntrospector
    {
        public static LSLCodeScopeType ResolveCodeScopeNodeType(LSLParser.CodeStatementContext context)
        {
            if (context.Parent is LSLParser.CodeScopeOrSingleBlockStatementContext)
            {
                if (context.Parent.Parent is LSLParser.IfStatementContext)
                {
                    return LSLCodeScopeType.IfCodeRoot;
                }
                if (context.Parent.Parent is LSLParser.ElseIfStatementContext)
                {
                    return LSLCodeScopeType.ElseIfCodeRoot;
                }
                if (context.Parent.Parent is LSLParser.ElseStatementContext)
                {
                    return LSLCodeScopeType.ElseCodeRoot;
                }
                if (context.Parent.Parent is LSLParser.DoLoopContext)
                {
                    return LSLCodeScopeType.DoLoopCodeRoot;
                }
                if (context.Parent.Parent is LSLParser.WhileLoopContext)
                {
                    return LSLCodeScopeType.WhileLoopCodeRoot;
                }
                if (context.Parent.Parent is LSLParser.ForLoopContext)
                {
                    return LSLCodeScopeType.ForLoopCodeRoot;
                }
            }
            else
            {
                var parent = context.Parent as LSLParser.CodeScopeContext;
                if (parent != null)
                {
                    return ResolveCodeScopeNodeType(parent);
                }
            }

            throw new InvalidOperationException(
                "Could not resolve code scope type from statement in LSLAntlrTreeUtilitys.ResolveCodeScopeNodeType");
        }



        public static LSLCodeScopeType ResolveCodeScopeNodeType(LSLParser.CodeScopeContext context)
        {
            if (context.Parent is LSLParser.CodeScopeOrSingleBlockStatementContext)
            {
                if (context.Parent.Parent is LSLParser.IfStatementContext)
                {
                    return LSLCodeScopeType.IfCodeRoot;
                }
                if (context.Parent.Parent is LSLParser.ElseIfStatementContext)
                {
                    return LSLCodeScopeType.ElseIfCodeRoot;
                }
                if (context.Parent.Parent is LSLParser.ElseStatementContext)
                {
                    return LSLCodeScopeType.ElseCodeRoot;
                }
                if (context.Parent.Parent is LSLParser.DoLoopContext)
                {
                    return LSLCodeScopeType.DoLoopCodeRoot;
                }
                if (context.Parent.Parent is LSLParser.WhileLoopContext)
                {
                    return LSLCodeScopeType.WhileLoopCodeRoot;
                }
                if (context.Parent.Parent is LSLParser.ForLoopContext)
                {
                    return LSLCodeScopeType.ForLoopCodeRoot;
                }
            }
            else if (context.Parent is LSLParser.FunctionDeclarationContext)
            {
                return LSLCodeScopeType.FunctionCodeRoot;
            }
            else if (context.Parent is LSLParser.EventHandlerContext)
            {
                return LSLCodeScopeType.EventHandlerCodeRoot;
            }
            else if (context.Parent is LSLParser.CodeStatementContext)
            {
                return LSLCodeScopeType.StatementBlockRoot;
            }

            throw new InvalidOperationException(
                "Could not resolve code scope type from code scope in LSLAntlrTreeUtilitys.ResolveCodeScopeNodeType");
        }
    }
}