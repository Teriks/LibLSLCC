#region FileInfo
// 
// File: LSLExpressionListNode.cs
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
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public enum LSLExpressionListType
    {
        ListInitializer,
        UserFunctionCallParameters,
        LibraryFunctionCallParameters,
        ForLoopAfterthoughts,
        ForLoopInitExpressions
    }


    public class LSLExpressionListNode : ILSLExpressionListNode
    {
        private readonly List<LSLSourceCodeRange> _commaSourceCodeRanges = new List<LSLSourceCodeRange>();
        private readonly List<ILSLExprNode> _expressionNodes = new List<ILSLExprNode>();
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLExpressionListNode(LSLSourceCodeRange sourceRange, Err err)

            // ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLExpressionListNode(LSLParser.OptionalExpressionListContext parserContext,
            LSLExpressionListType listType)
        {
            ParserContext = parserContext;
            ListType = listType;
            SourceCodeRange = new LSLSourceCodeRange(parserContext);
        }

        internal LSLExpressionListNode(LSLParser.OptionalExpressionListContext parserContext,
            IEnumerable<ILSLExprNode> expressions,
            LSLExpressionListType listType)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException("expressions");
            }

            ListType = listType;

            foreach (var lslExprNode in expressions)
            {
                AddExpression(lslExprNode);
            }
            SourceCodeRange = new LSLSourceCodeRange(parserContext);
        }

        internal LSLParser.OptionalExpressionListContext ParserContext { get; set; }

        public IReadOnlyList<ILSLExprNode> ExpressionNodes
        {
            get { return _expressionNodes; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public LSLExpressionListType ListType { get; private set; }

        IReadOnlyList<ILSLReadOnlyExprNode> ILSLExpressionListNode.ExpressionNodes
        {
            get { return _expressionNodes; }
        }

        public bool AllExpressionsConstant
        {
            get { return ExpressionNodes.Count == 0 || ExpressionNodes.All(lslExprNode => lslExprNode.IsConstant); }
        }

        public bool HasExpressionNodes
        {
            get { return ExpressionNodes.Count > 0; }
        }

        public IReadOnlyList<LSLSourceCodeRange> CommaSourceCodeRanges
        {
            get { return _commaSourceCodeRanges; }
        }

        public static LSLExpressionListNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLExpressionListNode(sourceRange, Err.Err);
        }

        public void AddExpression(ILSLExprNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Parent = this;

            _expressionNodes.Add(node);
        }

        public LSLExpressionListNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var r = new LSLExpressionListNode(ParserContext, ListType)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };


            foreach (var expressionNode in _expressionNodes)
            {
                r.AddExpression(expressionNode);
            }

            return r;
        }

        public void AddCommaRange(LSLSourceCodeRange range)
        {
            _commaSourceCodeRanges.Add(range);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        public LSLSourceCodeRange SourceCodeRange { get; private set; }


        public bool HasErrors { get; set; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (ListType == LSLExpressionListType.ForLoopAfterthoughts)
            {
                return visitor.VisitForLoopAfterthoughts(this);
            }

            if (ListType == LSLExpressionListType.LibraryFunctionCallParameters)
            {
                return visitor.VisitLibraryFunctionCallParameters(this);
            }

            if (ListType == LSLExpressionListType.UserFunctionCallParameters)
            {
                return visitor.VisitUserFunctionCallParameters(this);
            }

            if (ListType == LSLExpressionListType.ListInitializer)
            {
                return visitor.VisitListLiteralInitializerList(this);
            }

            if (ListType == LSLExpressionListType.ForLoopInitExpressions)
            {
                return visitor.VisitForLoopInitExpressions(this);
            }

            throw new InvalidOperationException("Visit LSLExpressionList, unknown ListTypes");
        }


        public ILSLSyntaxTreeNode Parent { get; set; }

        #endregion
    }
}