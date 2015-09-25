#region FileInfo

// 
// File: LSLAntlrTreeIntrospection.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:24 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Imports

using System;
using LibLSLCC.CodeValidator.Enums;

#endregion

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