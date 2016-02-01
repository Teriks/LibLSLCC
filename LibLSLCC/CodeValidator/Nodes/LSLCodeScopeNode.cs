#region FileInfo
// 
// File: LSLCodeScopeNode.cs
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    public class LSLCodeScopeNode : ILSLCodeScopeNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLCodeScopeNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLCodeScopeNode(LSLParser.CodeScopeContext context, ulong scopeId, LSLCodeScopeType codeScopeType)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            CodeScopeContext = context;
            ScopeId = scopeId;
            IsCodeScope = true;

            SourceCodeRange = new LSLSourceCodeRange(context);

            CodeScopeType = codeScopeType;

            SourceCodeRangesAvailable = true;
        }

        internal LSLCodeScopeNode(LSLParser.CodeStatementContext context, ulong scopeId, LSLCodeScopeType codeScopeType)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            SingleStatementContext = context;
            ScopeId = scopeId;
            IsSingleBlockStatement = true;

            SourceCodeRange = new LSLSourceCodeRange(context);

            CodeScopeType = codeScopeType;

            SourceCodeRangesAvailable = true;
        }

        internal LSLParser.CodeScopeContext CodeScopeContext { get; private set; }
        internal LSLParser.CodeStatementContext SingleStatementContext { get; private set; }

        /// <summary>
        ///     Code statements that are children of this code scope
        /// </summary>
        public IEnumerable<ILSLCodeStatement> CodeStatements
        {
            get { return _codeStatements; }
        }

        /// <summary>
        ///     The first statement node to be considered dead, when dead code is detected
        /// </summary>
        public ILSLCodeStatement FirstDeadStatementNode { get; private set; }

        /// <summary>
        ///     The parser context for the first statement node that is considered to be dead,
        ///     when dead code is detected
        /// </summary>
        internal LSLParser.CodeStatementContext FirstDeadStatementNodeContext { get; private set; }



        /// <summary>
        /// The type of code scope this node represents.
        /// </summary>
        public LSLCodeScopeType CodeScopeType { get; private set; }

        #region ILSLReturnPathNode Members

        /// <summary>
        ///     True if this code scope has a valid return path
        /// </summary>
        public bool HasReturnPath { get; set; }

        #endregion

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        ///     Constant jump descriptors for constant jumps that occur in this scope
        ///     used only with JumpStatementAnalysis is turned on and dead code caused by
        ///     jump statements is being detected.
        /// </summary>
        public IEnumerable<LSLConstantJumpDescription> ConstantJumps
        {
            get { return _constantJumps ?? new List<LSLConstantJumpDescription>(); }
        }

        /// <summary>
        ///     Is this scope a single statement scope, like a brace-less 'if' branch.
        ///     true if IsCodeScope is false
        /// </summary>
        public bool IsSingleBlockStatement { get; private set; }

        /// <summary>
        ///     Is this a normal braced code scope.
        ///     true if IsSingleBlockStatement is false
        /// </summary>
        public bool IsCodeScope { get; private set; }

        /// <summary>
        ///     Code statements that are children of this code scope
        /// </summary>
        IEnumerable<ILSLReadOnlyCodeStatement> ILSLCodeScopeNode.CodeStatements
        {
            get { return _codeStatements; }
        }

        /// <summary>
        ///     ReturnStatementNode != null
        /// </summary>
        public bool HasReturnStatementNode
        {
            get { return ReturnStatementNode != null; }
        }

        /// <summary>
        ///     FirstDeadStatementNode != null
        /// </summary>
        public bool HasDeadStatementNodes
        {
            get { return FirstDeadStatementNode != null; }
        }

        /// <summary>
        ///     The first statement node to be considered dead, when dead code is detected
        /// </summary>
        ILSLReadOnlyCodeStatement ILSLCodeScopeNode.FirstDeadStatementNode
        {
            get { return FirstDeadStatementNode; }
        }

        /// <summary>
        ///     Returns descriptions of all dead code segments in the top level of this scope,
        ///     if there are any
        /// </summary>
        public IEnumerable<LSLDeadCodeSegment> DeadCodeSegments
        {
            get { return _deadCodeSegments; }
        }

        /// <summary>
        ///     The top level return statement for a code scope, if one exists
        /// </summary>
        public LSLReturnStatementNode ReturnStatementNode { get; private set; }

        /// <summary>
        ///     The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        public LSLDeadCodeType DeadCodeType { get; set; }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        /// <summary>
        ///     A unique identifier for this scope, all direct descendants
        ///     share this ScopeId, ScopeId is 1 for the top scope of functions or
        ///     event handlers, the tree builder increments the id as new scopes are encountered
        ///     inside the top level scope
        /// </summary>
        public ulong ScopeId { get; set; }

        /// <summary>
        ///     The index of this statement in its parent scope
        /// </summary>
        public int StatementIndex { get; set; }

        /// <summary>
        ///     Is this the last statement in its parent scope
        /// </summary>
        public bool IsLastStatementInScope { get; set; }

        /// <summary>
        ///     Is this statement considered dead code
        /// </summary>
        public bool IsDeadCode { get; set; }

        /// <summary>
        ///     The statement that acts as the return path for this scope, if there is one
        /// </summary>
        /// <remarks>
        ///     This can be a <see cref="LSLReturnStatementNode"/> or an <see cref="LSLControlStatementNode"/>
        ///     as of the current implementation,  loop statements cannot currently
        ///     act as a sole return path; there is no constant evaluation in condition
        ///     statements so they are equivalent to a singular if statement
        /// </remarks>
        public ILSLCodeStatement ReturnPath { get; set; }

        public static
            LSLCodeScopeNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLCodeScopeNode(sourceRange, Err.Err);
        }

        private bool IsJumpDead(LSLJumpStatementNode jump)
        {
            if (jump.IsDeadCode) return true;


            var tn = jump.Parent;


            var n = tn as ILSLCodeStatement;
            while (n == null || !n.IsDeadCode)
            {
                tn = tn.Parent;

                if (tn == null || tn is ILSLEventHandlerNode || tn is ILSLFunctionDeclarationNode)
                {
                    break;
                }

                n = tn as ILSLCodeStatement;

                if (n != null && n.IsDeadCode)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Add a code statement to the code scope, the dead code detection algorithm is encapsulated inside
        ///     of this object, the algorithm is on-line and most of its logic occurs in this function
        /// </summary>
        /// <remarks>
        ///     if JumpStatementAnalysis is enabled, this function expects the JumpTarget of every
        ///     <see cref="LSLJumpStatementNode"/> object (the <see cref="LSLLabelStatementNode"/> reference) to have a correct
        ///     StatementIndex and ScopeId already. this includes JumpTarget's that are jumps forward in code
        ///     and have not been added to the code scope yet.  this prerequisite should be accomplished
        ///     with a pre-pass that creates the jump graph ahead of time. so that all Jump statement objects
        ///     have a reference to where they jump to exactly, and all label statements have references to
        ///     the jump statements that jump to to them.
        /// </remarks>
        /// <param name="statement">The statement to add to the code scope</param>
        /// <param name="context">The parser context which it was added in (while visiting that context)</param>
        public void AddCodeStatement(ILSLCodeStatement statement, LSLParser.CodeStatementContext context)
        {
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }


            statement.Parent = this;

            statement.StatementIndex = _codeStatements.Count;
            statement.IsLastStatementInScope = true;
            statement.ScopeId = ScopeId;

            if (_lastStatementAdded != null)
            {
                _lastStatementAdded.IsLastStatementInScope = false;
            }

            _lastStatementAdded = statement;
            _codeStatements.Add(statement);


            if (!_insideDeadCode)
            {
                //inspect all constant jumps that have been added prior to the current statement
                foreach (var constantJump in ConstantJumps)
                {
                    if (constantJump.JumpTarget.ScopeId != ScopeId)
                    {
                        //we jumped out of the scope, into the outer scope. this is the only possible scenario
                        //because we can't jump into a nested scope (like into an if statement, or scope block).
                        //everything after the jump out is dead, like a return statement

                        _insideDeadCode = true;


                        if (FirstDeadStatementNode == null)
                        {
                            FirstDeadStatementNode = statement;
                            FirstDeadStatementNodeContext = context;
                        }

                        _deadCodeSegmentsStack.Push(
                            new LSLDeadCodeSegment(LSLDeadCodeType.AfterJumpOutOfScope));
                    }
                    else if ((constantJump.EffectiveJumpStatement.StatementIndex < statement.StatementIndex) &&
                             (constantJump.JumpTarget.StatementIndex > statement.StatementIndex))
                    {
                        //this is a jump over code scenario
                        //when _deadCodeJumpOverEnding statement index is reached, we are no longer inside
                        //dead code
                        _inDeadJumpedOverCode = true;
                        _deadCodeJumpOverEnding = constantJump.JumpTarget.StatementIndex;
                        _insideDeadCode = true;

                        if (FirstDeadStatementNode == null)
                        {
                            FirstDeadStatementNode = statement;
                            FirstDeadStatementNodeContext = context;
                        }

                        _deadCodeSegmentsStack.Push(
                            new LSLDeadCodeSegment(LSLDeadCodeType.JumpOverCode));
                    }
                    else if ((constantJump.EffectiveJumpStatement.StatementIndex < statement.StatementIndex) &&
                             (constantJump.JumpTarget.StatementIndex <
                              constantJump.EffectiveJumpStatement.StatementIndex))
                    {
                        //we are below a forever loop that jumps upward into the same scope
                        //so everything else after is dead, like a return statement

                        _insideDeadCode = true;

                        if (FirstDeadStatementNode == null)
                        {
                            FirstDeadStatementNode = statement;
                            FirstDeadStatementNodeContext = context;
                        }

                        _deadCodeSegmentsStack.Push(
                            new LSLDeadCodeSegment(LSLDeadCodeType.AfterJumpLoopForever));
                    }
                }


                var control = statement as LSLControlStatementNode;
                if (control != null)
                {
                    var cJump = control.GetConstantJump();
                    if (cJump != null && !_insideDeadCode)
                    {
                        _constantJumps.Add(cJump);
                    }
                }

                var jump = statement as LSLJumpStatementNode;

                //don't add the jump if its an error, because the only
                //reason for it to be an error would be for it to have
                //referenced a non existent label. the recursive nature
                //of how a constant jump is derived from a control chain
                //keeps GetConstantJump() from ever returning an erroneous node
                //since its child scopes won't add them either
                if (jump != null && !_insideDeadCode && !jump.HasErrors)
                {
                    jump.ConstantJump = true;
                    _constantJumps.Add(new LSLConstantJumpDescription(jump));
                }
            }


            if (!_inDeadJumpedOverCode && !IsSingleBlockStatement)
            {
                if (!HasReturnPath && statement.HasReturnPath)
                {
                    HasReturnPath = true;
                    ReturnPath = statement;
                    //if the return path was a top level return statement
                    //save it, otherwise null
                    ReturnStatementNode = statement as LSLReturnStatementNode;

                    _afterLabelJumpDownOverReturn = false;
                }
                else if (HasReturnPath)
                {
                    //this code is after a return path, everything after is dead

                    var label = statement as LSLLabelStatementNode;

                    var dead = !_afterLabelJumpDownOverReturn;

                    if (label != null)
                    {
                        if (
                            label.JumpsToHere.Any(
                                x => (x.SourceCodeRange.StartIndex < label.SourceCodeRange.StartIndex) && !IsJumpDead(x)))
                        {
                            dead = false;
                            _afterLabelJumpDownOverReturn = true;
                            HasReturnPath = false;
                        }
                    }


                    if (dead)
                    {
                        if (FirstDeadStatementNode == null)
                        {
                            FirstDeadStatementNode = statement;
                            FirstDeadStatementNodeContext = context;
                        }

                        if (!_insideDeadCode)
                        {
                            _deadCodeSegmentsStack.Push(
                                new LSLDeadCodeSegment(LSLDeadCodeType.AfterReturnPath));
                        }


                        _insideDeadCode = true;
                        _insideDeadCodeAfterReturnPath = true;
                    }
                }
            }
            else
            {
                HasReturnPath = statement.HasReturnPath;
                ReturnStatementNode = statement as LSLReturnStatementNode;
            }


            if (_insideDeadCode)
            {
                if (_insideDeadCodeAfterReturnPath)
                {
                    var label = statement as LSLLabelStatementNode;

                    var dead = true;

                    if (label != null)
                    {
                        if (
                            label.JumpsToHere.Any(
                                x => (x.SourceCodeRange.StartIndex < label.SourceCodeRange.StartIndex) && !IsJumpDead(x)))
                        {
                            dead = false;
                            _afterLabelJumpDownOverReturn = true;
                            HasReturnPath = false;
                        }
                    }

                    if (dead)
                    {
                        statement.IsDeadCode = true;
                        statement.DeadCodeType = _deadCodeSegmentsStack.Peek().DeadCodeType;
                        _deadCodeSegmentsStack.Peek().AddStatement(statement);
                    }
                    else
                    {
                        _insideDeadCode = false;
                        _insideDeadCodeAfterReturnPath = false;
                    }
                }
                else if (_inDeadJumpedOverCode && statement.StatementIndex < _deadCodeJumpOverEnding)
                {
                    statement.IsDeadCode = true;
                    statement.DeadCodeType = _deadCodeSegmentsStack.Peek().DeadCodeType;
                    _deadCodeSegmentsStack.Peek().AddStatement(statement);
                }
                else if (_inDeadJumpedOverCode && statement.StatementIndex == _deadCodeJumpOverEnding)
                {
                    _deadCodeSegments.Add(_deadCodeSegmentsStack.Pop());
                    _insideDeadCode = false;
                    _inDeadJumpedOverCode = false;
                }
                else if (!_inDeadJumpedOverCode)
                {
                    statement.IsDeadCode = true;
                    statement.DeadCodeType = _deadCodeSegmentsStack.Peek().DeadCodeType;
                    _deadCodeSegmentsStack.Peek().AddStatement(statement);
                }
            }
        }

        /// <summary>
        ///     Must be called after all statements have been added to the code scope, in order to
        ///     tell the on-line dead code detection algorithm that the end of the scope has been reached
        /// </summary>
        public void EndScope()
        {
            _insideDeadCode = false;
            _inDeadJumpedOverCode = false;
            _deadCodeJumpOverEnding = -1;


            while (_deadCodeSegmentsStack.Any())
            {
                _deadCodeSegments.Add(_deadCodeSegmentsStack.Pop());
            }
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        /// <summary>
        ///     Does this node or its children have errors
        /// </summary>
        public bool HasErrors { get; set; }


        /// <summary>
        ///     The source code range this statement occupies in the source code
        /// </summary>
        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceCodeRangesAvailable { get; private set; }


        /// <summary>
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (IsSingleBlockStatement)
            {
                return visitor.VisitSingleStatementCodeScope(this);
            }
            return visitor.VisitMultiStatementCodeScope(this);
        }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }


        #endregion // ReSharper disable UnusedParameter.Local

        #region AddCodeStatementState

        private readonly List<ILSLCodeStatement> _codeStatements = new List<ILSLCodeStatement>();

        private readonly List<LSLConstantJumpDescription> _constantJumps = new List<LSLConstantJumpDescription>();

        /// <summary>
        ///     When the dead code segment ends, the segment is put in this list and made public
        ///     and readonly through the DeadCodeSegments Property
        /// </summary>
        private readonly List<LSLDeadCodeSegment> _deadCodeSegments = new List<LSLDeadCodeSegment>();

        /// <summary>
        ///     The stack is used to keep track of the current dead code segment that statements
        ///     are being put into
        /// </summary>
        private readonly Stack<LSLDeadCodeSegment> _deadCodeSegmentsStack = new Stack<LSLDeadCodeSegment>();

        /// <summary>
        ///     Used to terminate dead code that was jumped over,
        ///     when that case is detected, this is set to the statement index
        ///     of the label we jumped to, so we can stop classifying nodes as dead
        ///     once we reach that point
        /// </summary>
        private long _deadCodeJumpOverEnding = -1;


        /// <summary>
        ///     State to track if we are in the middle of a jump forward over statements
        /// </summary>
        private bool _inDeadJumpedOverCode;


        /// <summary>
        ///     State, set to true with the next add will be dead code
        /// </summary>
        private bool _insideDeadCode;

        private ILSLCodeStatement _lastStatementAdded;
        private bool _insideDeadCodeAfterReturnPath;
        private bool _afterLabelJumpDownOverReturn;

        #endregion
    }
}