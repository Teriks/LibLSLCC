#region FileInfo
// 
// 
// File: LSLAntlrTreeIntrospection.cs
// 
// Last Compile: 25/09/2015 @ 5:46 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// ============================================================
// ============================================================
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// Copyright (c) 2015, Teriks
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
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