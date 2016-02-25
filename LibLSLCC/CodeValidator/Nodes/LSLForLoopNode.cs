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
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.Visitor;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    /// <summary>
    /// Default <see cref="ILSLForLoopNode"/> implementation used by <see cref="LSLCodeValidator"/>
    /// </summary>
    public sealed class LSLForLoopNode : ILSLForLoopNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLForLoopNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <exception cref="ArgumentNullException"><paramref name="context"/> or <paramref name="afterthoughExpressions"/> or <paramref name="initExpressions"/> is <c>null</c>.</exception>
        internal LSLForLoopNode(LSLParser.ForLoopContext context, ILSLExpressionListNode initExpressions,
            ILSLExprNode conditionExpression,
            LSLExpressionListNode afterthoughExpressions, LSLCodeScopeNode code, bool inInSingleStatementScope)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (afterthoughExpressions == null)
            {
                throw new ArgumentNullException("afterthoughExpressions");
            }

            if (initExpressions == null)
            {
                throw new ArgumentNullException("initExpressions");
            }

            InsideSingleStatementScope = inInSingleStatementScope;

            InitExpressionsList = initExpressions;
            InitExpressionsList.Parent = this;

            AfterthoughExpressions = afterthoughExpressions;
            AfterthoughExpressions.Parent = this;

            SourceRange = new LSLSourceCodeRange(context);
            SourceRangeFirstSemicolon = new LSLSourceCodeRange(context.first_semi_colon);
            SourceRangeSecondSemicolon = new LSLSourceCodeRange(context.second_semi_colon);
            SourceRangeOpenParenth = new LSLSourceCodeRange(context.open_parenth);
            SourceRangeCloseParenth = new LSLSourceCodeRange(context.close_parenth);
            SourceRangeForKeyword = new LSLSourceCodeRange(context.loop_keyword);


            SourceRangesAvailable = true;

            Code = code;

            if (Code != null)
            {
                Code.Parent = this;
            }

            ConditionExpression = conditionExpression;

            if (ConditionExpression != null)
            {
                ConditionExpression.Parent = this;
            }
        }


        /// <summary>
        /// The expression list node that contains the expressions used in the initialization clause of the for-loop.
        /// This property should never be null unless the for loop node is an erroneous node.
        /// Ideally you should not be handling a syntax tree containing syntax errors.
        /// </summary>
        public ILSLExpressionListNode InitExpressionsList { get; private set; }

        /// <summary>
        /// The condition expression that controls the loop.
        /// </summary>
        public ILSLExprNode ConditionExpression { get; private set; }

        /// <summary>
        /// The expression list node that contains the expressions used in the afterthought area of the for-loop's clauses.
        /// This property should never be null unless the for loop node is an erroneous node.
        /// Ideally you should not be handling a syntax tree containing syntax errors.
        /// </summary>
        public LSLExpressionListNode AfterthoughExpressions { get; private set; }

        /// <summary>
        /// The code scope node that represents the code scope of the loop body.
        /// </summary>
        public LSLCodeScopeNode Code { get; private set; }


        /// <summary>
        ///     If the scope has a return path, this is set to the node that causes the function to return.
        ///     it may be a return statement, or a control chain node.
        /// </summary>
        public ILSLReadOnlyCodeStatement ReturnPath { get; set; }



        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLReadOnlyExprNode ILSLLoopNode.ConditionExpression
        {
            get { return ConditionExpression; }
        }

        ILSLExpressionListNode ILSLForLoopNode.AfterthoughExpressionList
        {
            get { return AfterthoughExpressions; }
        }

        ILSLCodeScopeNode ILSLLoopNode.Code
        {
            get { return Code; }
        }

        ILSLExpressionListNode ILSLForLoopNode.InitExpressionList
        {
            get { return InitExpressionsList; }
        }

        /// <summary>
        /// Returns true if the for-loop statement contains any initialization expressions, otherwise False.
        /// </summary>
        public bool HasInitExpressions
        {
            get { return InitExpressionsList != null && InitExpressionsList.HasExpressions; }
        }

        /// <summary>
        /// Returns true if the for-loop statement contains a condition expression, otherwise False.
        /// </summary>
        public bool HasConditionExpression
        {
            get { return ConditionExpression != null; }
        }

        /// <summary>
        /// Returns true if the for-loop statement contains any afterthought expressions, otherwise False.
        /// </summary>
        public bool HasAfterthoughtExpressions
        {
            get { return AfterthoughExpressions != null && AfterthoughExpressions.HasExpressions; }
        }

        /// <summary>
        ///     The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        public LSLDeadCodeType DeadCodeType { get; set; }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        /// <summary>
        ///     Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        ///     this is not the scopes level.
        /// </summary>
        public int ScopeId { get; set; }

        /// <summary>
        /// Returns a version of this node type that represents its error state;  in case of a syntax error
        /// in the node that prevents the node from being even partially built.
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
        /// <seealso cref="ILSLCodeScopeNode.IsSingleStatementScope"/>
        public bool InsideSingleStatementScope { get; private set; }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }


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
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; internal set; }


        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        /// <remarks>If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable"/> is <c>false</c> this property will be <c>null</c>.</remarks>
        public LSLSourceCodeRange SourceRange { get; private set; }


        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceRangesAvailable { get; private set; }


        /// <summary>
        /// The source code range of the 'for' keyword in the statement.
        /// </summary>
        /// <remarks>If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable"/> is <c>false</c> this property will be <c>null</c>.</remarks>
        public LSLSourceCodeRange SourceRangeForKeyword { get; private set; }


        /// <summary>
        /// The source code range of the semi-colon that separates the initialization clause from the condition clause of the for-loop;
        /// </summary>
        /// <remarks>If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable"/> is <c>false</c> this property will be <c>null</c>.</remarks>
        public LSLSourceCodeRange SourceRangeFirstSemicolon { get; private set; }


        /// <summary>
        /// The source code range of the semi-colon that separates the condition clause from the afterthought expressions of the for-loop;
        /// </summary>
        /// <remarks>If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable"/> is <c>false</c> this property will be <c>null</c>.</remarks>
        public LSLSourceCodeRange SourceRangeSecondSemicolon { get; private set; }


        /// <summary>
        /// The source code range of the opening parenthesis that starts the for-loop clauses area.
        /// </summary>
        /// <remarks>If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable"/> is <c>false</c> this property will be <c>null</c>.</remarks>
        public LSLSourceCodeRange SourceRangeOpenParenth { get; private set; }


        /// <summary>
        /// The source code range of the closing parenthesis that ends the for-loop clause section.
        /// </summary>
        /// <remarks>If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable"/> is <c>false</c> this property will be <c>null</c>.</remarks>
        public LSLSourceCodeRange SourceRangeCloseParenth { get; private set; }


        /// <summary>
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitForLoop(this);
        }


        /// <summary>
        /// True if the node represents a return path out of its ILSLCodeScopeNode parent, False otherwise.
        /// </summary>
        public bool HasReturnPath
        {
            get { return false; }
        }

        #endregion
    }
}