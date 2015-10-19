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
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLVariableNode : ILSLVariableNode, ILSLExprNode
    {
        private string _libraryConstantName = "";
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLVariableNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        private LSLVariableNode()
        {
            IsConstant = false;
        }

        internal LSLParser.LocalVariableDeclarationContext LocalDeclarationContext { get; private set; }
        internal LSLParser.GlobalVariableDeclarationContext GlobalDeclarationContext { get; private set; }
        internal LSLParser.ParameterDefinitionContext ParameterDeclarationContext { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        /// True if this variable node references a library constant, False if it references a user defined variable or parameter.
        /// </summary>
        public bool IsLibraryConstant
        {
            get { return ExpressionType == LSLExpressionType.LibraryConstant; }
        }

        /// <summary>
        /// True if this variable node references a user defined global variable.
        /// </summary>
        public bool IsGlobal
        {
            get { return ExpressionType == LSLExpressionType.GlobalVariable; }
        }

        /// <summary>
        /// True if this variable node references a user defined local variable.
        /// </summary>
        public bool IsLocal
        {
            get { return ExpressionType == LSLExpressionType.LocalVariable; }
        }

        /// <summary>
        /// A reference to the ILSLVariableDeclarationNode in the syntax tree where this variable was initially declared.
        /// </summary>
        public ILSLVariableDeclarationNode Declaration { get; private set; }

        /// <summary>
        /// True if this variable node references a function or event handler parameter.
        /// </summary>
        public bool IsParameter
        {
            get { return ExpressionType == LSLExpressionType.ParameterVariable; }
        }

        /// <summary>
        /// The raw type string describing the type of the variable referenced.
        /// </summary>
        public string TypeString
        {
            get
            {
                if (IsGlobal)
                {
                    return GlobalDeclarationContext.variable_type.Text;
                }
                if (IsLocal)
                {
                    return LocalDeclarationContext.variable_type.Text;
                }
                if (IsParameter)
                {
                    return ParameterDeclarationContext.TYPE().GetText();
                }
                if (IsLibraryConstant)
                {
                    return Type.ToLSLTypeString();
                }


                throw new InvalidOperationException("Object in invalid state");
            }
        }

        public static
            LSLVariableNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLVariableNode(sourceRange, Err.Err);
        }

        internal static LSLVariableNode CreateVar(LSLParser.GlobalVariableDeclarationContext context,
            ILSLVariableDeclarationNode declaration)
        {
            var n = new LSLVariableNode
            {
                Type = LSLTypeTools.FromLSLTypeString(context.variable_type.Text),
                ExpressionType = LSLExpressionType.GlobalVariable,
                IsConstant = false,
                GlobalDeclarationContext = context,
                SourceCodeRange = new LSLSourceCodeRange(context),
                Declaration = declaration
            };

            return n;
        }

        internal static LSLVariableNode CreateVar(LSLParser.LocalVariableDeclarationContext context,
            ILSLVariableDeclarationNode declaration)
        {
            var n = new LSLVariableNode
            {
                Type = LSLTypeTools.FromLSLTypeString(context.variable_type.Text),
                ExpressionType = LSLExpressionType.LocalVariable,
                IsConstant = false,
                LocalDeclarationContext = context,
                SourceCodeRange = new LSLSourceCodeRange(context),
                Declaration = declaration
            };

            return n;
        }

        internal static LSLVariableNode CreateLibraryConstant(LSLType type, string name)
        {
            var n = new LSLVariableNode
            {
                Type = type,
                ExpressionType = LSLExpressionType.LibraryConstant,
                IsConstant = true,
                _libraryConstantName = name,
                SourceCodeRange = new LSLSourceCodeRange()
            };

            return n;
        }

        internal static LSLVariableNode CreateParameter(LSLParser.ParameterDefinitionContext context)
        {
            var n = new LSLVariableNode
            {
                Type = LSLTypeTools.FromLSLTypeString(context.TYPE().GetText()),
                ExpressionType = LSLExpressionType.ParameterVariable,
                IsConstant = false,
                ParameterDeclarationContext = context,
                SourceCodeRange = new LSLSourceCodeRange(context)
            };

            return n;
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLExprNode Members

        /// <summary>
        /// Deep clones the expression node.  It should clone the node and also clone all of its children.
        /// </summary>
        /// <returns>A deep clone of this expression node.</returns>
        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var n = new LSLVariableNode
            {
                GlobalDeclarationContext = GlobalDeclarationContext,
                LocalDeclarationContext = LocalDeclarationContext,
                ParameterDeclarationContext = ParameterDeclarationContext,
                IsConstant = IsConstant,
                Type = Type,
                ExpressionType = ExpressionType,
                HasErrors = HasErrors,
                Parent = Parent,
                _libraryConstantName = _libraryConstantName,
                Declaration = Declaration
            };
            return n;
        }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }


        /// <summary>
        /// The name of the referenced variable.
        /// </summary>
        public string Name
        {
            get
            {
                if (IsGlobal)
                {
                    return GlobalDeclarationContext.variable_name.Text;
                }

                if (IsLocal)
                {
                    return LocalDeclarationContext.variable_name.Text;
                }

                if (IsParameter)
                {
                    return ParameterDeclarationContext.ID().GetText();
                }

                if (IsLibraryConstant)
                {
                    return _libraryConstantName;
                }

                throw new InvalidOperationException("Object in invalid state");
            }
        }


        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; set; }


        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceCodeRange { get; set; }


        /// <summary>
        /// Accept a visit from an implementor of ILSLValidatorNodeVisitor
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

            throw new InvalidOperationException("LSLVariableNode could not be visited, its state is invalid");
        }


        /// <summary>
        /// The expression type/classification of the expression.
        /// <see cref="LSLExpressionType"/>
        /// </summary>
        public LSLExpressionType ExpressionType { get; private set; }


        /// <summary>
        /// True if the expression is constant and can be calculated at compile time.
        /// </summary>
        public bool IsConstant { get; set; }


        /// <summary>
        /// Should produce a user friendly description of the expressions return type.
        /// This is used in some syntax error messages, Ideally you should enclose your description in
        /// parenthesis or something that will make it stand out in a string.
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
        /// The return type of the expression.
        /// </summary>
        public LSLType Type { get; private set; }

        #endregion
    }
}