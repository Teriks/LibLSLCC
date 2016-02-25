#region FileInfo

// 
// File: LSLVariableNode.cs
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
using LibLSLCC.CodeValidator.Visitor;
using LibLSLCC.Parser;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    /// <summary>
    ///     Default <see cref="ILSLVariableNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLVariableNode : ILSLVariableNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLVariableNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        private LSLVariableNode()
        {
            IsConstant = false;
        }


        /// <summary>
        ///     Create an <see cref="LSLVariableNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        public LSLVariableNode(LSLVariableNode other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            Name = other.Name;
            Type = other.Type;

            TypeName = other.TypeName;
            IsConstant = other.IsConstant;

            ExpressionType = other.ExpressionType;
            Declaration = other.Declaration;

            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
            }

            HasErrors = other.HasErrors;
            Parent = other.Parent;
        }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        ///     True if this variable node references a library constant, False if it references a user defined variable or
        ///     parameter.
        /// </summary>
        public bool IsLibraryConstant
        {
            get { return ExpressionType == LSLExpressionType.LibraryConstant; }
        }

        /// <summary>
        ///     True if this variable node references a user defined global variable.
        /// </summary>
        public bool IsGlobal
        {
            get { return ExpressionType == LSLExpressionType.GlobalVariable; }
        }

        /// <summary>
        ///     True if this variable node references a user defined local variable.
        /// </summary>
        public bool IsLocal
        {
            get { return ExpressionType == LSLExpressionType.LocalVariable; }
        }

        /// <summary>
        ///     A reference to the <see cref="ILSLVariableDeclarationNode" /> in the syntax tree where this variable was initially
        ///     declared.
        /// </summary>
        public ILSLVariableDeclarationNode Declaration { get; private set; }

        /// <summary>
        ///     True if this variable node references a function or event handler parameter.
        /// </summary>
        public bool IsParameter
        {
            get { return ExpressionType == LSLExpressionType.ParameterVariable; }
        }

        /// <summary>
        ///     The raw type string describing the type of the variable referenced.
        /// </summary>
        public string TypeName { get; private set; }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLVariableNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLVariableNode(sourceRange, Err.Err);
        }


        /// <exception cref="ArgumentNullException"><paramref name="context" /> or <paramref name="declaration" /> is <c>null</c>.</exception>
        internal static LSLVariableNode CreateVar(LSLParser.GlobalVariableDeclarationContext context,
            ILSLVariableDeclarationNode declaration)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }

            return new LSLVariableNode
            {
                Name = context.variable_name.Text,
                TypeName = context.variable_type.Text,
                Type = LSLTypeTools.FromLSLTypeName(context.variable_type.Text),
                ExpressionType = LSLExpressionType.GlobalVariable,
                IsConstant = false,
                SourceRange = new LSLSourceCodeRange(context),
                Declaration = declaration,
                SourceRangesAvailable = true
            };
        }


        /// <exception cref="ArgumentNullException"><paramref name="context" /> or <paramref name="declaration" /> is <c>null</c>.</exception>
        internal static LSLVariableNode CreateVar(LSLParser.LocalVariableDeclarationContext context,
            ILSLVariableDeclarationNode declaration)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }

            return new LSLVariableNode
            {
                Name = context.variable_name.Text,
                TypeName = context.variable_type.Text,
                Type = LSLTypeTools.FromLSLTypeName(context.variable_type.Text),
                ExpressionType = LSLExpressionType.LocalVariable,
                IsConstant = false,
                SourceRange = new LSLSourceCodeRange(context),
                Declaration = declaration,
                SourceRangesAvailable = true
            };
        }


        /// <exception cref="ArgumentException">
        ///     if <paramref name="name" /> contains invalid ID characters, or
        ///     <paramref name="type" /> is <see cref="LSLType.Void" />.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
        internal static LSLVariableNode CreateLibraryConstant(LSLType type, string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (type == LSLType.Void)
            {
                throw new ArgumentException(
                    typeof (LSLVariableNode).Name + ".CreateLibraryConstant:  type cannot be LSLType.Void.", "type");
            }

            if (!LSLTokenTools.IDRegex.IsMatch(name))
            {
                throw new ArgumentException(
                    typeof (LSLVariableNode).Name + ".CreateLibraryConstant:  name contains invalid ID characters.",
                    "name");
            }

            return new LSLVariableNode
            {
                Name = name,
                TypeName = type.ToLSLTypeName(),
                Type = type,
                ExpressionType = LSLExpressionType.LibraryConstant,
                IsConstant = true,
                SourceRangesAvailable = false
            };
        }


        /// <exception cref="ArgumentNullException"><paramref name="node" /> is <c>null</c>.</exception>
        internal static LSLVariableNode CreateParameter(LSLParameterNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            return new LSLVariableNode
            {
                Name = node.Name,
                TypeName = node.Type.ToLSLTypeName(),
                Type = node.Type,
                ExpressionType = LSLExpressionType.ParameterVariable,
                IsConstant = false,
                SourceRange = node.SourceRange,
                SourceRangesAvailable = true
            };
        }

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLExprNode Members

        /// <summary>
        ///     Deep clones the expression node.  It should clone the node and all of its children and cloneable properties, except
        ///     the parent.
        ///     When cloned, the parent node reference should still point to the same node.
        /// </summary>
        /// <returns>A deep clone of this expression node.</returns>
        public ILSLExprNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLVariableNode(this);
        }


        /// <summary>
        ///     The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }


        /// <summary>
        ///     The name of the referenced variable.
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        ///     True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; private set; }


        /// <summary>
        ///     The source code range that this syntax tree node occupies.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRange { get; internal set; }


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
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (IsGlobal)
            {
                return visitor.VisitGlobalVariableReference(this);
            }
            if (IsLocal)
            {
                return visitor.VisitLocalVariableReference(this);
            }
            if (IsParameter)
            {
                return visitor.VisitParameterVariableReference(this);
            }
            if (IsLibraryConstant)
            {
                return visitor.VisitLibraryConstantVariableReference(this);
            }

            throw new InvalidOperationException(typeof (LSLVariableNode).Name +
                                                " could not be visited, its state is invalid");
        }


        /// <summary>
        ///     The expression type/classification of the expression. see: <see cref="LSLExpressionType" />
        /// </summary>
        /// <value>
        ///     The type of the expression.
        /// </value>
        public LSLExpressionType ExpressionType { get; private set; }


        /// <summary>
        ///     True if the expression is constant and can be calculated at compile time.
        /// </summary>
        public bool IsConstant { get; set; }

        /// <summary>
        ///     True if the expression statement has some modifying effect on a local parameter or global/local variable;  or is a
        ///     function call.  False otherwise.
        /// </summary>
        public bool HasPossibleSideEffects
        {
            get { return false; }
        } //variable nodes (references) are never an expression that can cause a program's state to be altered


        /// <summary>
        ///     Should produce a user friendly description of the expressions return type.
        ///     This is used in some syntax error messages, Ideally you should enclose your description in
        ///     parenthesis or something that will make it stand out in a string.
        /// </summary>
        /// <returns></returns>
        public string DescribeType()
        {
            return "(" + Type + (this.IsLiteral() ? " Literal)" : ")");
        }


        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }


        /// <summary>
        ///     The return type of the expression. see: <see cref="LSLType" />
        /// </summary>
        public LSLType Type { get; private set; }

        #endregion
    }
}