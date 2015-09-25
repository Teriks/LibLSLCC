#region FileInfo
// 
// 
// File: LSLVariableDeclarationNode.cs
// 
// Last Compile: 25/09/2015 @ 11:47 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLVariableDeclarationNode : ILSLVariableDeclarationNode, ILSLCodeStatement
    {
        private readonly List<LSLVariableNode> _references = new List<LSLVariableNode>();
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLVariableDeclarationNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        private LSLVariableDeclarationNode()
        {
            IsSingleBlockStatement = false;
        }

        /// <summary>
        ///     The variable node that was created upon construction of
        ///     this variable declaration
        /// </summary>
        public LSLVariableNode VariableNode { get; private set; }

        public ILSLExprNode DeclarationExpression { get; private set; }

        public IReadOnlyList<LSLVariableNode> References
        {
            get { return _references; }
        }

        public ILSLCodeStatement ReturnPath { get; set; }
        public LSLDeadCodeType DeadCodeType { get; set; }

        public string Name
        {
            get { return VariableNode.Name; }
        }

        public LSLType Type
        {
            get { return VariableNode.Type; }
        }

        public string TypeString
        {
            get { return VariableNode.TypeString; }
        }

        ILSLVariableNode ILSLVariableDeclarationNode.VariableNode
        {
            get { return VariableNode; }
        }

        public bool HasDeclarationExpression
        {
            get { return DeclarationExpression != null; }
        }

        public bool IsLocal
        {
            get { return VariableNode.IsLocal; }
        }

        public bool IsGlobal
        {
            get { return VariableNode.IsGlobal; }
        }

        public bool IsParameter
        {
            get { return VariableNode.IsParameter; }
        }

        public bool IsLibraryConstant
        {
            get { return VariableNode.IsLibraryConstant; }
        }

        ILSLReadOnlyExprNode ILSLVariableDeclarationNode.DeclarationExpression
        {
            get { return DeclarationExpression; }
        }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        public ulong ScopeId { get; set; }

        IReadOnlyList<ILSLVariableNode> ILSLVariableDeclarationNode.References
        {
            get { return References; }
        }

        public static
            LSLVariableDeclarationNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLVariableDeclarationNode(sourceRange, Err.Err);
        }

        /// <summary>
        ///     Creates a reference to VariableNode by cloning and setting its SourceCodeRange
        ///     to that of referenceToken
        /// </summary>
        /// <param name="referenceToken">The variable reference token from the parser</param>
        /// <returns>VariableNode cloned, with its SourceCodeRange set</returns>
        internal LSLVariableNode CreateReference(IToken referenceToken)
        {
            var v = (LSLVariableNode) VariableNode.Clone();

            v.SourceCodeRange = new LSLSourceCodeRange(referenceToken);

            _references.Add(v);

            return v;
        }

        internal static LSLVariableDeclarationNode CreateVar(LSLParser.GlobalVariableDeclarationContext context)
        {
            var n = new LSLVariableDeclarationNode();
            n.VariableNode = LSLVariableNode.CreateVar(context, n);
            n.SourceCodeRange = new LSLSourceCodeRange(context);
            n.VariableNode.Parent = n;

            n.TypeSourceCodeRange = new LSLSourceCodeRange(context.variable_type);
            n.NameSourceCodeRange = new LSLSourceCodeRange(context.variable_name);

            if (context.operation != null)
            {
                n.OperationSourceCodeRange = new LSLSourceCodeRange(context.operation);
            }

            return n;
        }

        internal static LSLVariableDeclarationNode CreateVar(LSLParser.GlobalVariableDeclarationContext context,
            ILSLExprNode declarationExpression)
        {
            var n = new LSLVariableDeclarationNode();
            n.VariableNode = LSLVariableNode.CreateVar(context, n);
            n.SourceCodeRange = new LSLSourceCodeRange(context);
            n.VariableNode.Parent = n;
            n.DeclarationExpression = declarationExpression;

            n.TypeSourceCodeRange = new LSLSourceCodeRange(context.variable_type);
            n.NameSourceCodeRange = new LSLSourceCodeRange(context.variable_name);

            declarationExpression.Parent = n;

            if (context.operation != null)
            {
                n.OperationSourceCodeRange = new LSLSourceCodeRange(context.operation);
            }

            return n;
        }

        internal static LSLVariableDeclarationNode CreateVar(LSLParser.LocalVariableDeclarationContext context,
            ILSLExprNode declarationExpression)
        {
            var n = new LSLVariableDeclarationNode();
            n.VariableNode = LSLVariableNode.CreateVar(context, n);
            n.SourceCodeRange = new LSLSourceCodeRange(context);
            n.VariableNode.Parent = n;
            n.DeclarationExpression = declarationExpression;

            n.TypeSourceCodeRange = new LSLSourceCodeRange(context.variable_type);
            n.NameSourceCodeRange = new LSLSourceCodeRange(context.variable_name);

            declarationExpression.Parent = n;

            if (context.operation != null)
            {
                n.OperationSourceCodeRange = new LSLSourceCodeRange(context.operation);
            }

            return n;
        }

        internal static LSLVariableDeclarationNode CreateVar(LSLParser.LocalVariableDeclarationContext context)
        {
            var n = new LSLVariableDeclarationNode();
            n.VariableNode = LSLVariableNode.CreateVar(context, n);
            n.SourceCodeRange = new LSLSourceCodeRange(context);
            n.VariableNode.Parent = n;

            n.TypeSourceCodeRange = new LSLSourceCodeRange(context.variable_type);
            n.NameSourceCodeRange = new LSLSourceCodeRange(context.variable_name);

            if (context.operation != null)
            {
                n.OperationSourceCodeRange = new LSLSourceCodeRange(context.operation);
            }

            return n;
        }

        internal static LSLVariableDeclarationNode CreateLibraryConstant(LSLType type, string name)
        {
            var n = new LSLVariableDeclarationNode
            {
                VariableNode = LSLVariableNode.CreateLibraryConstant(type, name),
                SourceCodeRange = new LSLSourceCodeRange()
            };

            n.VariableNode.Parent = n;

            return n;
        }

        internal static LSLVariableDeclarationNode CreateParameter(LSLParser.ParameterDefinitionContext context)
        {
            var n = new LSLVariableDeclarationNode
            {
                VariableNode = LSLVariableNode.CreateParameter(context),
                SourceCodeRange = new LSLSourceCodeRange(context)
            };

            n.VariableNode.Parent = n;

            return n;
        }

        public LSLVariableDeclarationNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var r = new LSLVariableDeclarationNode
            {
                HasErrors = HasErrors,
                Parent = Parent,
                StatementIndex = StatementIndex,
                IsLastStatementInScope = IsLastStatementInScope,
                VariableNode = (LSLVariableNode) VariableNode.Clone(),
                OperationSourceCodeRange = OperationSourceCodeRange
            };

            r._references.AddRange(_references);
            return r;
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLCodeStatement Members

        public bool IsConstant
        {
            get { return VariableNode.IsConstant; }
            set { VariableNode.IsConstant = value; }
        }

        public LSLParser.LocalVariableDeclarationContext LocalDeclarationContext
        {
            get { return VariableNode.LocalDeclarationContext; }
        }

        public LSLParser.GlobalVariableDeclarationContext GlobalDeclarationContext
        {
            get { return VariableNode.GlobalDeclarationContext; }
        }

        public LSLParser.ParameterDefinitionContext ParameterDeclarationContext
        {
            get { return VariableNode.ParameterDeclarationContext; }
        }

        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }


        public int StatementIndex { get; set; }

        public bool HasReturnPath
        {
            get { return false; }
        }

        public bool IsLastStatementInScope { get; set; }

        public bool IsDeadCode { get; set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public bool HasErrors { get; set; }


        public LSLSourceCodeRange SourceCodeRange { get; private set; }


        public LSLSourceCodeRange OperationSourceCodeRange { get; private set; }


        public LSLSourceCodeRange TypeSourceCodeRange { get; private set; }

        public LSLSourceCodeRange NameSourceCodeRange { get; private set; }

        public LSLSourceCodeRange OperatorSourceCodeRange
        {
            get { return OperationSourceCodeRange; }
        }

        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (IsGlobal)
            {
                return visitor.VisitGlobalVariableDeclaration(this);
            }
            return visitor.VisitLocalVariableDeclaration(this);
        }

        #endregion
    }
}