#region FileInfo

// 
// File: LSLForLoopNode.cs
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
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.AntlrParser;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLForLoopNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLForLoopNode : ILSLForLoopNode, ILSLCodeStatement
    {
        private ILSLSyntaxTreeNode _parent;
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLForLoopNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <summary>
        ///     Construct an <see cref="LSLForLoopNode" /> without init expressions or afterthought expressions.
        /// </summary>
        /// <param name="condition">The for loop condition expression, may be <c>null</c>.</param>
        /// <param name="code">The code body of the for loop.</param>
        public LSLForLoopNode(ILSLExprNode condition, LSLCodeScopeNode code)
            : this(new LSLExpressionListNode(), condition, new LSLExpressionListNode(), code)
        {
        }


        /// <summary>
        ///     Construct an <see cref="LSLForLoopNode" /> without init expressions.
        /// </summary>
        /// <param name="condition">The for loop condition expression, may be <c>null</c>.</param>
        /// <param name="afterthoughtExpressions">The afterthought expression list.</param>
        /// <param name="code">The code body of the for loop.</param>
        /// <exception cref="ArgumentNullException"><paramref name="afterthoughtExpressions" /> is <c>null</c>.</exception>
        public LSLForLoopNode(ILSLExprNode condition,
            LSLExpressionListNode afterthoughtExpressions, LSLCodeScopeNode code)
            : this(new LSLExpressionListNode(), condition, afterthoughtExpressions, code)
        {
        }



        /// <summary>
        ///     Construct an <see cref="LSLForLoopNode" /> without afterthought expressions.
        /// </summary>
        /// <param name="initExpressions">The init expression list.</param>
        /// <param name="condition">The for loop condition expression, may be <c>null</c>.</param>
        /// <param name="code">The code body of the for loop.</param>
        /// <exception cref="ArgumentNullException"><paramref name="initExpressions" /> is <c>null</c>.</exception>
        public LSLForLoopNode(LSLExpressionListNode initExpressions, ILSLExprNode condition, LSLCodeScopeNode code)
            : this(initExpressions, condition, new LSLExpressionListNode(), code)
        {
        }




        /// <summary>
        ///     Construct an <see cref="LSLForLoopNode" /> with all possible children.
        /// </summary>
        /// <param name="initExpression">The init expression.</param>
        /// <param name="condition">The for loop condition expression, may be <c>null</c>.</param>
        /// <param name="afterthoughtExpression">The afterthought expression.</param>
        /// <param name="code">The code body of the for loop.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="initExpression" /> or
        ///     <paramref name="afterthoughtExpression" /> or <paramref name="code" /> is <c>null</c>.
        /// </exception>
        public LSLForLoopNode(ILSLExprNode initExpression, ILSLExprNode condition,
            ILSLExprNode afterthoughtExpression, LSLCodeScopeNode code) : 
            this(new LSLExpressionListNode(initExpression), condition, new LSLExpressionListNode(afterthoughtExpression), code)
        {
            
        }



        /// <summary>
        ///     Construct an <see cref="LSLForLoopNode" /> with all possible children.
        /// </summary>
        /// <param name="initExpressions">The init expression list.</param>
        /// <param name="condition">The for loop condition expression, may be <c>null</c>.</param>
        /// <param name="afterthoughtExpressions">The afterthought expression list.</param>
        /// <param name="code">The code body of the for loop.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="initExpressions" /> or
        ///     <paramref name="afterthoughtExpressions" /> or <paramref name="code" /> is <c>null</c>.
        /// </exception>
        public LSLForLoopNode(LSLExpressionListNode initExpressions, ILSLExprNode condition,
            LSLExpressionListNode afterthoughtExpressions, LSLCodeScopeNode code)
        {
            if (initExpressions == null) throw new ArgumentNullException("initExpressions");
            if (afterthoughtExpressions == null) throw new ArgumentNullException("afterthoughtExpressions");
            if (code == null) throw new ArgumentNullException("code");

            InitExpressionList = initExpressions;
            InitExpressionList.Parent = this;

            ConditionExpression = condition;

            if (ConditionExpression != null)
            {
                ConditionExpression.Parent = this;
            }

            AfterthoughtExpressionList = afterthoughtExpressions;
            AfterthoughtExpressionList.Parent = this;

            Code = code;
            Code.Parent = this;
            Code.CodeScopeType = LSLCodeScopeType.ForLoop;
        }



        /// <exception cref="ArgumentNullException">
        ///     <paramref name="context" /> or <paramref name="initExpression" /> or
        ///     <paramref name="afterthoughtExpressionsList" /> or <paramref name="code" /> is <c>null</c>.
        /// </exception>
        internal LSLForLoopNode(LSLParser.ForLoopContext context, LSLExpressionListNode initExpression,
            ILSLExprNode conditionExpression,
            LSLExpressionListNode afterthoughtExpressionsList, LSLCodeScopeNode code) 
            : this(initExpression, conditionExpression, afterthoughtExpressionsList, code)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            SourceRange = new LSLSourceCodeRange(context);
            SourceRangeFirstSemicolon = new LSLSourceCodeRange(context.first_semi_colon);
            SourceRangeSecondSemicolon = new LSLSourceCodeRange(context.second_semi_colon);
            SourceRangeOpenParenth = new LSLSourceCodeRange(context.open_parenth);
            SourceRangeCloseParenth = new LSLSourceCodeRange(context.close_parenth);
            SourceRangeForKeyword = new LSLSourceCodeRange(context.loop_keyword);


            SourceRangesAvailable = true;
        }


        /*
        /// <summary>
        ///     Create an <see cref="LSLForLoopNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        public LSLForLoopNode(LSLForLoopNode other)
        {
            if (other == null) throw new ArgumentNullException("other");

            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRangeForKeyword = other.SourceRangeForKeyword;
                SourceRangeOpenParenth = other.SourceRangeOpenParenth;
                SourceRangeFirstSemicolon = other.SourceRangeFirstSemicolon;
                SourceRangeSecondSemicolon = other.SourceRangeSecondSemicolon;
                SourceRangeCloseParenth = other.SourceRangeCloseParenth;
                SourceRange = other.SourceRange;
            }

            Code = other.Code.Clone();
            Code.Parent = this;

            if (other.HasConditionExpression)
            {
                ConditionExpression = other.ConditionExpression.Clone();
                ConditionExpression.Parent = this;
            }

            InitExpressionList = other.InitExpressionList.Clone();
            InitExpressionList.Parent = this;

            AfterthoughtExpressionList = other.AfterthoughtExpressionList.Clone();
            AfterthoughtExpressionList.Parent = this;

            LSLStatementNodeTools.CopyStatement(this, other);

            HasErrors = other.HasErrors;
        }*/


        /// <summary>
        ///     The expression list node that contains the expressions used in the initialization clause of the for-loop.
        ///     This property should never be null unless the for loop node is an erroneous node.
        ///     Ideally you should not be handling a syntax tree containing syntax errors.
        /// </summary>
        public LSLExpressionListNode InitExpressionList { get; private set; }

        /// <summary>
        ///     The condition expression that controls the loop.
        /// </summary>
        public ILSLExprNode ConditionExpression { get; private set; }

        /// <summary>
        ///     The expression list node that contains the expressions used in the afterthought area of the for-loop's clauses.
        ///     This property should never be null unless the for loop node is an erroneous node.
        ///     Ideally you should not be handling a syntax tree containing syntax errors.
        /// </summary>
        public LSLExpressionListNode AfterthoughtExpressionList { get; private set; }

        /// <summary>
        ///     The code scope node that represents the code scope of the loop body.
        /// </summary>
        public LSLCodeScopeNode Code { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLReadOnlyExprNode ILSLLoopNode.ConditionExpression
        {
            get { return ConditionExpression; }
        }

        ILSLExpressionListNode ILSLForLoopNode.InitExpressionList
        {
            get { return InitExpressionList; }
        }

        ILSLExpressionListNode ILSLForLoopNode.AfterthoughtExpressionList
        {
            get { return AfterthoughtExpressionList; }
        }

        ILSLCodeScopeNode ILSLLoopNode.Code
        {
            get { return Code; }
        }

        /// <summary>
        ///     Returns true if the for-loop statement contains any initialization expressions, otherwise False.
        /// </summary>
        public bool HasInitExpressions
        {
            get { return InitExpressionList != null && InitExpressionList.HasExpressions; }
        }

        /// <summary>
        ///     Returns true if the for-loop statement contains a condition expression, otherwise False.
        /// </summary>
        public bool HasConditionExpression
        {
            get { return ConditionExpression != null; }
        }

        /// <summary>
        ///     Returns true if the for-loop statement contains any afterthought expressions, otherwise False.
        /// </summary>
        public bool HasAfterthoughtExpressions
        {
            get { return AfterthoughtExpressionList != null && AfterthoughtExpressionList.HasExpressions; }
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
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLForLoopNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLForLoopNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLCodeStatement Members

        /// <summary>
        ///     True if this statement belongs to a single statement code scope.
        ///     A single statement code scope is a braceless code scope that can be used in control or loop statements.
        /// </summary>
        /// <seealso cref="ILSLCodeScopeNode.IsSingleStatementScope" />
        public bool InsideSingleStatementScope { get; set; }


        /// <summary>
        ///     The parent node of this syntax tree node.
        /// </summary>
        /// <exception cref="InvalidOperationException" accessor="set">If Parent has already been set.</exception>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value" /> is <see langword="null" />.</exception>
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


        /// <summary>
        ///     Is this statement the last statement in its scope
        /// </summary>
        public bool IsLastStatementInScope { get; set; }


        /// <summary>
        ///     Is this statement dead code
        /// </summary>
        public bool IsDeadCode { get; set; }


        /// <summary>
        ///     The index of this statement in its scope
        /// </summary>
        public int StatementIndex { get; set; }


        /// <summary>
        ///     True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; internal set; }


        /// <summary>
        ///     The source code range that this syntax tree node occupies.
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
        ///     The source code range of the 'for' keyword in the statement.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeForKeyword { get; private set; }


        /// <summary>
        ///     The source code range of the semi-colon that separates the initialization clause from the condition clause of the
        ///     for-loop;
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeFirstSemicolon { get; private set; }


        /// <summary>
        ///     The source code range of the semi-colon that separates the condition clause from the afterthought expressions of
        ///     the for-loop;
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeSecondSemicolon { get; private set; }


        /// <summary>
        ///     The source code range of the opening parenthesis that starts the for-loop clauses area.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeOpenParenth { get; private set; }


        /// <summary>
        ///     The source code range of the closing parenthesis that ends the for-loop clause section.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeCloseParenth { get; private set; }


        /// <summary>
        ///     Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}" />
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="visitor"/> is <see langword="null" />.</exception>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");

            return visitor.VisitForLoop(this);
        }


        /// <summary>
        ///     True if the node represents a return path out of its ILSLCodeScopeNode parent, False otherwise.
        /// </summary>
        public bool HasReturnPath
        {
            get { return false; }
        }

        #endregion
    }
}