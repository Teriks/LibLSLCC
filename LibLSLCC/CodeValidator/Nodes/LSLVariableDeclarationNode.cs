#region FileInfo
// 
// File: LSLVariableDeclarationNode.cs
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

using System.Diagnostics.CodeAnalysis;
using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Collections;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    public class LSLVariableDeclarationNode : ILSLVariableDeclarationNode, ILSLCodeStatement
    {
        private readonly GenericArray<LSLVariableNode> _references = new GenericArray<LSLVariableNode>();
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

        public IReadOnlyGenericArray<LSLVariableNode> References
        {
            get { return _references; }
        }

        /// <summary>
        /// If the scope has a return path, this is set to the node that causes the function to return.
        /// it may be a return statement, or a control chain node.
        /// </summary>
        public ILSLCodeStatement ReturnPath { get; set; }


        /// <summary>
        /// The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        public LSLDeadCodeType DeadCodeType { get; set; }


        /// <summary>
        /// The name of the variable.
        /// </summary>
        public string Name
        {
            get { return VariableNode.Name; }
        }

        /// <summary>
        /// The variable type.
        /// </summary>
        public LSLType Type
        {
            get { return VariableNode.Type; }
        }


        /// <summary>
        /// The raw type string representing the variable type, taken from the source code.
        /// </summary>
        public string TypeString
        {
            get { return VariableNode.TypeString; }
        }

        ILSLVariableNode ILSLVariableDeclarationNode.VariableNode
        {
            get { return VariableNode; }
        }

        /// <summary>
        /// True if an expression was used to initialize this variable declaration node when it was defined.
        /// </summary>
        public bool HasDeclarationExpression
        {
            get { return DeclarationExpression != null; }
        }

        /// <summary>
        /// True if this variable declaration is local to a function or event handler.  False if it is a global variable, parameter definition, or library constant.
        /// </summary>
        public bool IsLocal
        {
            get { return VariableNode.IsLocal; }
        }

        /// <summary>
        /// True if this variable declaration is in the global program scope.  False if it is a local variable, parameter definition, or library constant.
        /// </summary>
        public bool IsGlobal
        {
            get { return VariableNode.IsGlobal; }
        }

        /// <summary>
        /// True if this variable declaration represents a local function/event handler parameter.  False if it is a local variable, global variable, or library constant.
        /// </summary>
        public bool IsParameter
        {
            get { return VariableNode.IsParameter; }
        }

        /// <summary>
        /// True if this variable declaration represents a library defined constant.  False if it is a local variable, global variable, or parameter definition.
        /// </summary>
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

        /// <summary>
        /// Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        /// this is not the scopes level.
        /// </summary>
        public ulong ScopeId { get; set; }

        IReadOnlyGenericArray<ILSLVariableNode> ILSLVariableDeclarationNode.References
        {
            get { return References; }
        }

        public static
            LSLVariableDeclarationNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLVariableDeclarationNode(sourceRange, Err.Err);
        }


        /// <summary>
        /// Creates a reference to VariableNode by cloning and setting its SourceCodeRange
        /// to that of <paramref name="referenceToken"/>
        /// </summary>
        /// <param name="referenceToken">The variable reference token from the parser</param>
        /// <returns>VariableNode cloned, with its SourceCodeRange set</returns>
        internal LSLVariableNode CreateReference(IToken referenceToken)
        {
            var v = (LSLVariableNode) VariableNode.Clone();

            v.SourceCodeRange = new LSLSourceCodeRange(referenceToken);
            SourceCodeRangesAvailable = true;
            _references.Add(v);

            return v;
        }


        /// <summary>
        /// Creates a reference to VariableNode by cloning and setting its SourceCodeRange
        /// to that of <paramref name="range"/>
        /// </summary>
        /// <param name="range">The source-code range of the variable token from the parser</param>
        /// <returns>VariableNode cloned, with its SourceCodeRange set</returns>
        internal LSLVariableNode CreateReference(LSLSourceCodeRange range)
        {
            var v = (LSLVariableNode)VariableNode.Clone();

            v.SourceCodeRange = range.Clone();
            SourceCodeRangesAvailable = true;
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

            n.SourceCodeRangesAvailable = true;

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

            n.SourceCodeRangesAvailable = true;

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

            n.SourceCodeRangesAvailable = true;

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

            n.SourceCodeRangesAvailable = true;

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
                SourceCodeRange = new LSLSourceCodeRange(context),
                SourceCodeRangesAvailable = true
            };


            n.VariableNode.Parent = n;


            return n;
        }


        /// <summary>
        /// Deep clones the variable declaration node.  It should clone the node and also clone its VariableNode child.
        /// </summary>
        /// <returns>A deep clone of this variable declaration node.</returns>
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
                OperationSourceCodeRange = OperationSourceCodeRange,
                SourceCodeRangesAvailable = SourceCodeRangesAvailable

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


        /// <summary>
        /// True if this statement belongs to a single statement code scope.
        /// A single statement code scope is a brace-less code scope that can be used in control or loop statements.
        /// </summary>
        public bool IsSingleBlockStatement { get; private set; }



        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }


        /// <summary>
        /// The index of this statement in its scope
        /// </summary>
        public int StatementIndex { get; set; }


        /// <summary>
        /// True if the node represents a return path out of its ILSLCodeScopeNode parent, False otherwise.
        /// </summary>
        public bool HasReturnPath
        {
            get { return false; }
        }


        /// <summary>
        /// Is this statement the last statement in its scope
        /// </summary>
        public bool IsLastStatementInScope { get; set; }


        /// <summary>
        /// Is this statement dead code
        /// </summary>
        public bool IsDeadCode { get; set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; internal set; }


        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceCodeRangesAvailable { get; private set; }


        public LSLSourceCodeRange OperationSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range of the type specifier for the variable declaration.
        /// </summary>
        public LSLSourceCodeRange TypeSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range that encompasses the variables name in the declaration.
        /// </summary>
        public LSLSourceCodeRange NameSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range of the assignment operator in the declaration expression if one was used.
        /// This value is only meaningful if either 'IsLocal' or 'IsGlobal' are true, and 'HasDeclarationExpression' is also true.
        /// </summary>
        public LSLSourceCodeRange OperatorSourceCodeRange
        {
            get { return OperationSourceCodeRange; }
        }

        /// <summary>
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
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