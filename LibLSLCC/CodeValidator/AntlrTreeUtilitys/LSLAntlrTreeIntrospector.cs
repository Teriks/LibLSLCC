#region FileInfo
// 
// File: LSLAntlrTreeIntrospection.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
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
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.AntlrTreeUtilitys
{
    internal static class LSLAntlrTreeIntrospector
    {
        public static bool IsBracelessCodeScopeStatement(LSLParser.CodeStatementContext context)
        {
            //if the statement is a code scope who's parent is not another code scope then it cannot be
            //a brace-less code scope statement used with a do/while/for loop or if/else statement.

            //Control structures (if statement nodes) are excluded if their parent is an else statement.
            //because that constitutes an 'if else' statement combo that is possibly followed by a single statement, 
            //or a code scope.

            return context.code_scope == null &&  
                    !(context.Parent is LSLParser.CodeScopeContext) && 
                    !(context.Parent is LSLParser.ElseStatementContext && context.control_structure != null);
        }


        public static LSLCodeScopeType ResolveCodeScopeNodeType(LSLParser.CodeStatementContext context)
        {
            if (context.Parent is LSLParser.ControlStructureContext)
            {
                if (context.Parent.Parent.Parent is LSLParser.ElseStatementContext)
                {
                    //This is statement is after 'else if'
                    return LSLCodeScopeType.ElseIf;
                }
                
                //This statement is after just 'if'
                return LSLCodeScopeType.If;
            }
            if (context.Parent is LSLParser.ElseStatementContext)
            {
                return LSLCodeScopeType.Else;
            }
            if (context.Parent is LSLParser.DoLoopContext)
            {
                return LSLCodeScopeType.DoLoop;
            }
            if (context.Parent is LSLParser.WhileLoopContext)
            {
                return LSLCodeScopeType.WhileLoop;
            }
            if (context.Parent is LSLParser.ForLoopContext)
            {
                return LSLCodeScopeType.ForLoop;
            }

            var parent = context.Parent as LSLParser.CodeScopeContext;
            if (parent != null)
            {
                return ResolveCodeScopeNodeType(parent);
            }

            //bugcheck assert
            throw new InvalidOperationException(
                "BUGCHECK: Unexpected ANTLR syntax tree structure in " + typeof(LSLAntlrTreeIntrospector).FullName + ".ResolveCodeScopeNodeType");
        }



        public static LSLCodeScopeType ResolveCodeScopeNodeType(LSLParser.CodeScopeContext context)
        {
            if (context.Parent is LSLParser.FunctionDeclarationContext)
            {
                return LSLCodeScopeType.Function;
            }
            if (context.Parent is LSLParser.EventHandlerContext)
            {
                return LSLCodeScopeType.EventHandler;

            }


            if (context.Parent.Parent is LSLParser.ControlStructureContext)
            {
                if (context.Parent.Parent.Parent.Parent is LSLParser.ElseStatementContext)
                {
                    return LSLCodeScopeType.ElseIf;
                }
                return LSLCodeScopeType.If;
            }
            if (context.Parent.Parent is LSLParser.ElseStatementContext)
            {
                return LSLCodeScopeType.Else;
            }
            if (context.Parent.Parent is LSLParser.DoLoopContext)
            {
                return LSLCodeScopeType.DoLoop;
            }
            if (context.Parent.Parent is LSLParser.WhileLoopContext)
            {
                return LSLCodeScopeType.WhileLoop;
            }
            if (context.Parent.Parent is LSLParser.ForLoopContext)
            {
                return LSLCodeScopeType.ForLoop;
            }
            if (context.Parent.Parent is LSLParser.CodeScopeContext)
            {
                return LSLCodeScopeType.AnonymousBlock;
            }

            //bugcheck assert
            throw new InvalidOperationException(
                "BUGCHECK: Unexpected ANTLR syntax tree structure in " + typeof(LSLAntlrTreeIntrospector).FullName + ".ResolveCodeScopeNodeType");
        }
    }
}