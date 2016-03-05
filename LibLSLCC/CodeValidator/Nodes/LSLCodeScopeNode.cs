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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LibLSLCC.AntlrParser;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLCodeScopeNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLCodeScopeNode : ILSLCodeScopeNode, ILSLCodeStatement, IEnumerable<ILSLReadOnlyCodeStatement>
    {
        HashSet<LSLLabelStatementNode> _preDefinedLabels = new HashSet<LSLLabelStatementNode>(
            new LambdaEqualityComparer<LSLLabelStatementNode>(ReferenceEquals));


// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLCodeScopeNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <summary>
        /// Sets a labels parent to this <see cref="LSLCodeScopeNode"/>, also sets <see cref="LSLLabelStatementNode.ParentScopeId"/> to this nodes <see cref="ScopeId"/>.
        /// </summary>
        /// <param name="label">The label node to take pre define ownership of.</param>
        /// <returns><paramref name="label"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="label"/> is <c>null</c>.</exception>
        public LSLLabelStatementNode PreDefineLabel(LSLLabelStatementNode label)
        {
            if (label == null) throw new ArgumentNullException("label");

            if (_preDefinedLabels == null)
            {
                _preDefinedLabels = new HashSet<LSLLabelStatementNode>(
                    new LambdaEqualityComparer<LSLLabelStatementNode>(ReferenceEquals)
                    );
            }

            label.Parent = this;
            label.ParentScopeId = ScopeId;
            _preDefinedLabels.Add(label);
            return label;
        }


        /// <summary>
        ///     Create a <see cref="LSLCodeScopeNode" /> with the given scope ID.
        /// </summary>
        /// <param name="scopeId">The <see cref="ParentScopeId"/>.</param>
        public LSLCodeScopeNode(int scopeId)
        {
            ScopeId = scopeId;
        }







        /// <summary>
        ///     Construct an  <see cref="LSLCodeScopeNode" /> with the given scope ID and statements. <para/>
        ///     <see cref="EndScope" /> is called after adding the statements in the enumerable, you will not be able to add more statements with <see cref="AddStatement(ILSLCodeStatement)" />. <para/>
        ///     If only a single statement is added <see cref="IsSingleStatementScope"/> will be <c>true</c>.
        /// </summary>
        /// <param name="scopeId">The <see cref="ParentScopeId"/>.</param>
        /// <param name="statements">The statements to add to the code scope.</param>
        /// <exception cref="ArgumentNullException"><paramref name="statements" /> is <c>null</c>.</exception>
        public LSLCodeScopeNode(int scopeId, IEnumerable<ILSLCodeStatement> statements)
        {
            if (statements == null) throw new ArgumentNullException("statements");

            ScopeId = scopeId;

            foreach (var stat in statements)
            {
                AddStatement(stat);
            }

            EndScope();
        }




        /// <summary>
        ///     Construct an <see cref="LSLCodeScopeNode" /> with the given scope ID and expressions as statements.  <para/>
        ///     An <see cref="LSLExpressionStatementNode"/> is implicitly created for each <see cref="ILSLExprNode"/>. <para/>
        ///     <see cref="EndScope" /> is called after adding the statements, you will not be able to add more statements with <see cref="AddStatement(ILSLCodeStatement)" />. <para/>
        ///     If only a single expression is added <see cref="IsSingleStatementScope"/> will be <c>true</c>.
        /// </summary>
        /// <param name="scopeId">The <see cref="ParentScopeId"/>.</param>
        /// <param name="expressions">The expressions to use as statements in the single statement code scope.</param>
        /// <exception cref="ArgumentNullException"><paramref name="expressions" /> is <c>null</c>.</exception>
        public LSLCodeScopeNode(int scopeId, params ILSLExprNode[] expressions)
            : this(scopeId, expressions.Select(x => new LSLExpressionStatementNode(x)))
        {

        }


        /// <summary>
        ///     Construct an <see cref="LSLCodeScopeNode" /> with the given scope ID and statements. <para/>
        ///     <see cref="EndScope" /> is called after adding the statements, you will not be able to add more statements with <see cref="AddStatement(ILSLCodeStatement)" />. <para/>
        ///     If only a single statement is added <see cref="IsSingleStatementScope"/> will be <c>true</c>.
        /// </summary>
        /// <param name="scopeId">The <see cref="ParentScopeId"/>.</param>
        /// <param name="statements">The statements to add to the code scope.</param>
        /// <exception cref="ArgumentNullException"><paramref name="statements" /> is <c>null</c>.</exception>
        public LSLCodeScopeNode(int scopeId, params ILSLCodeStatement[] statements) : this(scopeId, statements.AsEnumerable())
        {
            
        }


        /*
        /// <summary>
        ///     Clone this <see cref="LSLCodeScopeNode" /> from another.
        /// </summary>
        /// <param name="other">The other node.</param>
        private LSLCodeScopeNode(LSLCodeScopeNode other)
        {
            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
            }

            foreach (var child in other._codeStatements)
            {
                AddCodeStatement(child.Clone());
            }

            HasErrors = other.HasErrors;
        }*/


        /// <exception cref="ArgumentNullException"><paramref name="context" /> is <c>null</c>.</exception>
        internal LSLCodeScopeNode(LSLParser.CodeScopeContext context, int scopeId)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ScopeId = scopeId;

            SourceRange = new LSLSourceCodeRange(context);

            SourceRangesAvailable = true;
        }


        /// <exception cref="ArgumentNullException"><paramref name="context" /> or <paramref name="statement" /> is <c>null</c>.</exception>
        internal LSLCodeScopeNode(LSLParser.CodeStatementContext context, int scopeId, ILSLCodeStatement statement)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }

            AddStatement(statement);
            EndScope();

            ScopeId = scopeId;
            _isSingleStatementScope = true;

            SourceRange = new LSLSourceCodeRange(context);

            SourceRangesAvailable = true;
        }


        /// <summary>
        ///     Constant jump descriptors for constant jumps that occur in this scope
        ///     used only with JumpStatementAnalysis is turned on and dead code caused by
        ///     jump statements is being detected.
        /// </summary>
        public IEnumerable<LSLConstantJumpDescription> ConstantJumps
        {
            get { return _constantJumps; }
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
        public ILSLReadOnlyCodeStatement FirstDeadStatementNode { get; private set; }

        /// <summary>
        ///     Returns descriptions of all dead code segments in the top level of this scope,
        ///     if there are any
        /// </summary>
        public IEnumerable<ILSLReadOnlyDeadCodeSegment> DeadCodeSegments
        {
            get { return _deadCodeSegments; }
        }

        /// <summary>
        ///     The top level return statement for a code scope, if one exists
        /// </summary>
        public ILSLReturnStatementNode ReturnStatementNode { get; private set; }

        /// <summary>
        ///     The statement that acts as the return path for this scope, if there is one
        /// </summary>
        /// <remarks>
        ///     This can be a <see cref="LSLReturnStatementNode" /> or an <see cref="LSLControlStatementNode" />
        ///     as of the current implementation,  loop statements cannot currently
        ///     act as a sole return path; there is no constant evaluation in condition
        ///     statements so they are equivalent to a singular if statement
        /// </remarks>
        public ILSLReadOnlyCodeStatement ReturnPath { get; set; }

        /// <summary>
        ///     Always <c>false</c> for <see cref="LSLVariableDeclarationNode" />.
        /// </summary>
        /// <seealso cref="ILSLCodeScopeNode.IsSingleStatementScope" />
        /// <exception cref="NotSupportedException" accessor="set">if <c>value</c> is <c>true</c>.</exception>
        public bool InsideSingleStatementScope
        {
            get { return false; }
            set
            {
                if (value)
                {
                    throw new NotSupportedException(GetType().Name + " cannot exist in a single statement scope.");
                }
            }
        }

        /// <summary>
        ///     The scope ID of this code scope.
        ///     All child statements will inherit this ID.
        /// </summary>
        /// <seealso cref="ILSLReadOnlyCodeStatement.ParentScopeId"/>
        public int ScopeId { get; private set; }

        /// <summary>
        ///     True if this code scope is an implicit braceless scope.
        ///     Bracless code scopes can only occur as the code body in loop type constructs and control statements. <para/>
        ///     Setting this to <c>true</c> when the scope does not currently contain exactly one statement will cause an <see cref="ArgumentException"/> to be thrown.
        /// </summary>
        /// <exception cref="ArgumentException" accessor="set">If the code scope does not contain exactly one statement and <paramref name="value"/> is set to <c>true</c>.</exception>
        public bool IsSingleStatementScope
        {
            get { return _isSingleStatementScope; }
            set
            {
                if (value && _codeStatements.Count != 1)
                {
                    throw new ArgumentException(
                        string.Format("Cannot set IsSingleStatementScope to true unless the scope contains exactly one one statement, it currently has {0}.", _codeStatements.Count), 
                        "value");
                }
                _isSingleStatementScope = value;
            }
        }

        /// <summary>
        ///     The type of code scope this node represents.
        /// </summary>
        public LSLCodeScopeType CodeScopeType { get;

            //this is set by other nodes when added as a child.
            internal set; }

        #region ILSLReturnPathNode Members

        /// <summary>
        ///     True if this code scope has a valid return path
        /// </summary>
        public bool HasReturnPath { get; private set; }

        #endregion

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        ///     Code statements that are children of this code scope
        /// </summary>
        public IEnumerable<ILSLReadOnlyCodeStatement> CodeStatements
        {
            get { return _codeStatements; }
        }

        /// <summary>
        ///     True if this code scope contains any child statements
        /// </summary>
        public bool HasCodeStatements
        {
            get { return _codeStatements.Any(); }
        }

        /// <summary>
        ///     The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        public LSLDeadCodeType DeadCodeType { get; set; }


        /// <summary>
        ///     Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        ///     this is not the scopes level.
        /// </summary>
        public int ParentScopeId { get; set; }


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
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLCodeScopeNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLCodeScopeNode(sourceRange, Err.Err);
        }


        private static bool IsJumpDead(LSLJumpStatementNode jump)
        {
            if (jump.IsDeadCode) return true;


            var jumpParent = jump.Parent;


            var parentAsStatement = jumpParent as ILSLCodeStatement;

            while (parentAsStatement == null || !parentAsStatement.IsDeadCode)
            {
                jumpParent = jumpParent.Parent;

                if (jumpParent == null || jumpParent is ILSLEventHandlerNode ||
                    jumpParent is ILSLFunctionDeclarationNode)
                {
                    break;
                }

                parentAsStatement = jumpParent as ILSLCodeStatement;

                if (parentAsStatement != null && parentAsStatement.IsDeadCode)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Convenience method to add a new <seealso cref="LSLExpressionStatementNode"/> via <seealso cref="AddStatement(ILSLCodeStatement)"/>.
        /// </summary>
        /// <seealso cref="AddStatement(ILSLCodeStatement)"/>
        /// <seealso cref="LSLExpressionStatementNode"/>
        /// <param name="expression"></param>
        public void AddStatement(ILSLExprNode expression)
        {
            AddStatement(new LSLExpressionStatementNode(expression));
        }


        /// <summary>
        ///     Add a code statement to the code scope, the dead code detection algorithm is encapsulated inside
        ///     of this object, the algorithm is on-line and most of its logic occurs in this function
        /// </summary>
        /// <remarks>
        ///     if JumpStatementAnalysis is enabled, this function expects the JumpTarget of every
        ///     <see cref="LSLJumpStatementNode" /> object (the <see cref="LSLLabelStatementNode" /> reference) to have a correct
        ///     StatementIndex and ScopeId already. this includes JumpTarget's that are jumps forward in code
        ///     and have not been added to the code scope yet.  this prerequisite should be accomplished
        ///     with a pre-pass that creates the jump graph ahead of time. so that all Jump statement objects
        ///     have a reference to where they jump to exactly, and all label statements have references to
        ///     the jump statements that jump to to them.
        /// </remarks>
        /// <param name="statement">The statement to add to the code scope</param>
        /// <exception cref="ArgumentNullException"><paramref name="statement" /> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="EndScope"/> has already been called.</exception>
        /// <seealso cref="EndScope" />
        public void AddStatement(ILSLCodeStatement statement)
        {
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }

            if (_endScope)
            {
                throw new InvalidOperationException(
                    "EndScope has already been called, cannot add any more statements.");
            }


            var asCodeScope = statement as LSLCodeScopeNode;

            if (asCodeScope != null)
            {
                asCodeScope.CodeScopeType = LSLCodeScopeType.AnonymousBlock;
            }

            var asLabel = statement as LSLLabelStatementNode;

            if (_preDefinedLabels.Contains(asLabel))
            {
                _preDefinedLabels.Remove(asLabel);
            }
            else
            {
                statement.Parent = this;
            }


            statement.InsideSingleStatementScope = IsSingleStatementScope;

            statement.StatementIndex = _codeStatements.Count;
            statement.IsLastStatementInScope = true;
            statement.ParentScopeId = ScopeId;

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
                    if (constantJump.JumpTarget.ParentScopeId != ScopeId)
                    {
                        //we jumped out of the scope, into the outer scope. this is the only possible scenario
                        //because we can't jump into a nested scope (like into an if statement, or scope block).
                        //everything after the jump out is dead, like a return statement

                        _insideDeadCode = true;


                        if (FirstDeadStatementNode == null)
                        {
                            FirstDeadStatementNode = statement;
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


            if (!_inDeadJumpedOverCode && !IsSingleStatementScope)
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
                                x => (x.SourceRange.StartIndex < label.SourceRange.StartIndex) && !IsJumpDead(x)))
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


            if (!_insideDeadCode) return;

            if (_insideDeadCodeAfterReturnPath)
            {
                var label = statement as LSLLabelStatementNode;

                var dead = true;

                if (label != null)
                {
                    if (
                        label.JumpsToHere.Any(
                            x => (x.SourceRange.StartIndex < label.SourceRange.StartIndex) && !IsJumpDead(x)))
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



        /// <summary>
        ///     Must be called after all statements have been added to the code scope; in order to
        ///     tell the dead code detection algorithm that the end of the scope has been reached.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="EndScope"/> has already been called.</exception>
        public void EndScope()
        {
            if (_endScope)
            {
                throw new InvalidOperationException("EndScope had already been called.");
            }


            _endScope = true;
            _insideDeadCode = false;
            _inDeadJumpedOverCode = false;
            _deadCodeJumpOverEnding = -1;


            while (_deadCodeSegmentsStack.Any())
            {
                _deadCodeSegments.Add(_deadCodeSegmentsStack.Pop());
            }
        }


        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        /// <summary>
        ///     True if this syntax tree node contains syntax errors. <para/>
        ///     <see cref="SourceRange"/> should point to a more specific error location when this is <c>true</c>. <para/>
        ///     Other source ranges will not be available.
        /// </summary>
        public bool HasErrors { get; internal set; }


        /// <summary>
        ///     The source code range this statement occupies in the source code
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRange { get; private set; }


        /// <summary>
        ///     Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceRangesAvailable { get; private set; }


        /// <summary>
        ///     Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}" />
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="visitor"/> is <c>null</c>.</exception>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");

            return IsSingleStatementScope
                ? visitor.VisitSingleStatementCodeScope(this)
                : visitor.VisitMultiStatementCodeScope(this);
        }


        /// <summary>
        ///     The parent node of this syntax tree node.
        /// </summary>
        /// <exception cref="InvalidOperationException" accessor="set">If Parent has already been set.</exception>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value" /> is <c>null</c>.</exception>
        public ILSLSyntaxTreeNode Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                {
                    throw new InvalidOperationException(GetType().Name +
                                                        ": Parent node already set, it can only be set once.");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value", GetType().Name + ": Parent cannot be set to null.");
                }

                _parent = value;
            }
        }

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
        private ILSLSyntaxTreeNode _parent;
        private bool _endScope;
        private bool _isSingleStatementScope;

        #endregion


        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ILSLReadOnlyCodeStatement"/>'s in the code scope.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ILSLReadOnlyCodeStatement> GetEnumerator()
        {
            return CodeStatements.GetEnumerator();
        }


        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}