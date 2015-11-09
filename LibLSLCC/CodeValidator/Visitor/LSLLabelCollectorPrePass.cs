#region FileInfo
// 
// File: LSLLabelCollectorPrePass.cs
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
using System.Collections.Generic;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Nodes;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Parser;

namespace LibLSLCC.CodeValidator.Visitor
{
    internal class LSLLabelCollectorPrePass : LSLBaseVisitor<bool>, ILSLTreePreePass
    {
        private readonly LSLVisitorScopeTracker _scopingManager;

        private readonly Stack<StatementIndexContainer> _statementIndexStack =
            new Stack<StatementIndexContainer>();

        private readonly ILSLValidatorServiceProvider _validatorServiceProvider;

        private ulong _currentScopeId;


        public LSLLabelCollectorPrePass(LSLVisitorScopeTracker scopingManager,
            ILSLValidatorServiceProvider validatorServiceProvider)
        {
            _scopingManager = scopingManager;
            _validatorServiceProvider = validatorServiceProvider;

            _statementIndexStack.Push(new StatementIndexContainer {Index = 0, ScopeId = 0});
        }

        public bool HasSyntaxErrors { get; private set; }

        public bool HasSyntaxWarnings { get; private set; }




        /// <summary>
        /// Gets the syntax warning listener.  this property should NOT be used to generate warning events,
        /// use <see cref="GenSyntaxWarning"/> for that instead.
        /// </summary>
        /// <value>
        /// The syntax warning listener.
        /// </value>
        private ILSLSyntaxWarningListener SyntaxWarningListener
        {
            get { return _validatorServiceProvider.SyntaxWarningListener; }
        }

        /// <summary>
        /// Gets the syntax error listener.  this property should NOT be used to generate error events,
        /// use <see cref="GenSyntaxError"/> for that instead.
        /// </summary>
        /// <value>
        /// The syntax error listener.
        /// </value>
        private ILSLSyntaxErrorListener SyntaxErrorListener
        {
            get { return _validatorServiceProvider.SyntaxErrorListener; }
        }

        /// <summary>
        /// Returns a reference to <see cref="SyntaxWarningListener"/> and sets <see cref="HasSyntaxWarnings"/> to <c>true</c>.
        /// </summary>
        /// <returns><see cref="SyntaxWarningListener"/></returns>
        private ILSLSyntaxWarningListener GenSyntaxWarning()
        {
            HasSyntaxWarnings = true;
            return SyntaxWarningListener;
        }


        /// <summary>
        /// Returns a reference to <see cref="SyntaxErrorListener"/> and sets <see cref="HasSyntaxErrors"/> to <c>true</c>.
        /// </summary>
        /// <returns><see cref="SyntaxErrorListener"/></returns>
        private ILSLSyntaxErrorListener GenSyntaxError()
        {
            HasSyntaxErrors = true;
            return SyntaxErrorListener;
        }



        public override bool VisitCodeStatement(LSLParser.CodeStatementContext context)
        {
            base.VisitCodeStatement(context);
            _statementIndexStack.Peek().Index++;
            return false;
        }



        public override bool VisitCodeScopeOrSingleBlockStatement(
            LSLParser.CodeScopeOrSingleBlockStatementContext context)
        {
            if (context == null || !Utility.OnlyOneNotNull(context.code, context.statement))
            {
                throw
                    LSLCodeValidatorInternalException.
                        VisitContextInvalidState("VisitCodeScopeOrSingleBlockStatement",
                            true);
            }


            if (context.statement != null)
            {
                _scopingManager.EnterSingleStatementBlock(context.statement);

                _currentScopeId++;
                _statementIndexStack.Push(new StatementIndexContainer {Index = 0, ScopeId = _currentScopeId});
                base.VisitCodeScopeOrSingleBlockStatement(context);
                _statementIndexStack.Pop();
                _scopingManager.ExitSingleStatementBlock();
            }
            if (context.code != null)
            {
                base.VisitCodeScopeOrSingleBlockStatement(context);
            }

            return true;
        }



        public override bool VisitCodeScope(LSLParser.CodeScopeContext context)
        {
            if (context == null || context.children == null)
            {
                throw LSLCodeValidatorInternalException.VisitContextInvalidState("VisitCodeScope", true);
            }

            _scopingManager.EnterCodeScopeDuringPrePass(context);
            _currentScopeId++;
            _statementIndexStack.Push(new StatementIndexContainer {Index = 0, ScopeId = _currentScopeId});

            var result = base.VisitCodeScope(context);
            _statementIndexStack.Pop();
            _scopingManager.ExitCodeScopeDuringPrePass();

            return result;
        }



        public override bool VisitLabelStatement(LSLParser.LabelStatementContext context)
        {
            if (context == null || context.label_name == null)
            {
                throw LSLCodeValidatorInternalException.VisitContextInvalidState("VisitLabelStatement", true);
            }


            if (_scopingManager.LabelPreDefinedAnywhere(context.label_name.Text))
            {
                GenSyntaxError().RedefinedLabel(new LSLSourceCodeRange(context), context.label_name.Text);
                return false;
            }


            var ctx = new LSLLabelStatementNode(context, _scopingManager.InSingleStatementBlock);

            var statementIndexInfo = _statementIndexStack.Peek();

            ctx.ScopeId = statementIndexInfo.ScopeId;

            ctx.StatementIndex = statementIndexInfo.Index;

            _scopingManager.PreDefineLabel(context.label_name.Text, ctx);


            return base.VisitLabelStatement(context);
        }

        private class StatementIndexContainer
        {
            public int Index;
            public ulong ScopeId;
        }
    }
}