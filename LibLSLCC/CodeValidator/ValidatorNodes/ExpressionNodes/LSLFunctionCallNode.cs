#region FileInfo
// 
// 
// File: LSLFunctionCallNode.cs
// 
// Last Compile: 25/09/2015 @ 5:46 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// ============================================================
// ============================================================
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// Copyright (c) 2015, Teriks
// All rights reserved.
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
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLFunctionCallNode : ILSLFunctionCallNode, ILSLExprNode
    {
        private readonly bool _libraryFunction;
        private readonly LSLFunctionSignature _librarySignature;
        private readonly LSLPreDefinedFunctionSignature _preDefinition;
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLFunctionCallNode(LSLSourceCodeRange sourceCodeRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceCodeRange;
            HasErrors = true;
        }

        internal LSLFunctionCallNode(LSLParser.Expr_FunctionCallContext context,
            LSLPreDefinedFunctionSignature preDefinition,
            LSLExpressionListNode parameterList)
        {
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }


            ParserContext = context;

            _preDefinition = preDefinition;
            _libraryFunction = false;
            ParameterListNode = parameterList;

            parameterList.Parent = this;

            SourceCodeRange = new LSLSourceCodeRange(context);
            OpenParenthSourceCodeRange = new LSLSourceCodeRange(context.open_parenth);
            CloseParenthSourceCodeRange = new LSLSourceCodeRange(context.close_parenth);
            FunctionNameSourceCodeRange = new LSLSourceCodeRange(context.function_name);
        }

        internal LSLFunctionCallNode(LSLParser.Expr_FunctionCallContext context,
            LSLFunctionSignature signature,
            LSLExpressionListNode parameterList)
        {
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }


            ParserContext = context;
            _librarySignature = signature;
            _preDefinition = null;
            _libraryFunction = true;
            ParameterListNode = parameterList;
            parameterList.Parent = this;

            SourceCodeRange = new LSLSourceCodeRange(context);
            OpenParenthSourceCodeRange = new LSLSourceCodeRange(context.open_parenth);
            CloseParenthSourceCodeRange = new LSLSourceCodeRange(context.close_parenth);
            FunctionNameSourceCodeRange = new LSLSourceCodeRange(context.function_name);
        }

        internal LSLParser.Expr_FunctionCallContext ParserContext { get; }

        public IReadOnlyList<ILSLExprNode> ParameterExpressions
        {
            get { return ParameterListNode.ExpressionNodes; }
        }

        public LSLExpressionListNode ParameterListNode { get; }

        public LSLFunctionDeclarationNode DefinitionNode
        {
            get
            {
                if (!_libraryFunction)
                {
                    return _preDefinition.DefinitionNode;
                }

                throw new InvalidOperationException("Cannot get a definition node for a library function call.");
            }
        }

        public LSLSourceCodeRange OpenParenthSourceCodeRange { get; }
        public LSLSourceCodeRange CloseParenthSourceCodeRange { get; }
        public LSLSourceCodeRange FunctionNameSourceCodeRange { get; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public string Name
        {
            get { return ParserContext.function_name.Text; }
        }

        IReadOnlyList<ILSLReadOnlyExprNode> ILSLFunctionCallNode.ParameterExpressions
        {
            get { return ParameterExpressions; }
        }

        public LSLFunctionSignature Signature
        {
            get
            {
                if (_libraryFunction)
                {
                    return _librarySignature;
                }
                return _preDefinition;
            }
        }

        ILSLExpressionListNode ILSLFunctionCallNode.ParameterListNode
        {
            get { return ParameterListNode; }
        }

        ILSLFunctionDeclarationNode ILSLFunctionCallNode.DefinitionNode
        {
            get { return DefinitionNode; }
        }

        public static LSLFunctionCallNode GetError(LSLSourceCodeRange sourceCodeRange)
        {
            return new LSLFunctionCallNode(sourceCodeRange, Err.Err);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLExprNode Members

        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var parameterList = ParameterListNode == null ? null : ParameterListNode.Clone();

            if (_libraryFunction)
            {
                return new LSLFunctionCallNode(ParserContext, _librarySignature, parameterList)
                {
                    HasErrors = HasErrors,
                    Parent = Parent
                };
            }

            return new LSLFunctionCallNode(ParserContext, _preDefinition, parameterList)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };
        }


        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (ExpressionType == LSLExpressionType.LibraryFunction)
            {
                return visitor.VisitLibraryFunctionCall(this);
            }

            if (ExpressionType == LSLExpressionType.UserFunction)
            {
                return visitor.VisitUserFunctionCall(this);
            }

            throw new InvalidOperationException(
                "LSLFunctionCallNode could not be visited, object is in an invalid state");
        }


        public LSLType Type
        {
            get { return Signature.ReturnType; }
        }

        public LSLExpressionType ExpressionType
        {
            get { return _libraryFunction ? LSLExpressionType.LibraryFunction : LSLExpressionType.UserFunction; }
        }

        public bool IsConstant
        {
            get { return false; }
        }


        public string DescribeType()
        {
            return "(" + Type + (this.IsLiteral() ? " Literal)" : ")");
        }


        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }

        #endregion
    }
}