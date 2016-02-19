#region FileInfo
// 
// File: LSLParameterListNode.cs
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
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Collections;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    /// <summary>
    /// Default <see cref="ILSLParameterListNode"/> implementation used by <see cref="LSLCodeValidator"/>
    /// </summary>
    public sealed class LSLParameterListNode : ILSLParameterListNode
    {
        private readonly GenericArray<LSLParameterNode> _parameters = new GenericArray<LSLParameterNode>();

        private readonly GenericArray<LSLSourceCodeRange> _sourceRangeCommaList = new GenericArray<LSLSourceCodeRange>();

// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLParameterListNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        private LSLParameterListNode(LSLParser.OptionalParameterListContext context, LSLParameterListType parameterListType)
        {
            SourceRange = new LSLSourceCodeRange(context);

            SourceRangesAvailable = true;

            ParameterListType = parameterListType;
        }

        /// <summary>
        /// The <see cref="LSLParameterNode"/> objects that are children of this node, or an empty list.
        /// </summary>
        public IReadOnlyGenericArray<LSLParameterNode> Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// The source code range for each comma separator that appears in the parameter list in order, or an empty list object.
        /// </summary>
        public IReadOnlyGenericArray<LSLSourceCodeRange> SourceRangeCommaList
        {
            get { return _sourceRangeCommaList; }
        }

        /// <summary>
        /// The parameter list type;  FunctionParameters or EventParameters.
        /// </summary>
        public LSLParameterListType ParameterListType { get; private set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        /// True if this parameter list node contains parameter definition nodes.
        /// </summary>
        public bool HasParameterNodes
        {
            get { return _parameters.Count > 0; }
        }

        IReadOnlyGenericArray<ILSLParameterNode> ILSLParameterListNode.Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }


        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; private set; }


        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceRange { get; private set; }



        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceRangesAvailable { get; private set; }


        /// <summary>
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitParameterDefinitionList(this);
        }


        /// <summary>
        /// Returns a version of this node type that represents its error state;  in case of a syntax error
        /// in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLParameterListNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLParameterListNode(sourceRange, Err.Err);
        }


        /// <summary>
        ///     Builds a parameter list node directly from a parser context, checking for duplicates and reporting
        ///     duplicate parameter errors via the validatorServices <see cref="ILSLValidatorServiceProvider"/>.
        /// </summary>
        /// <param name="context">The context to build from</param>
        /// <param name="validatorServices">The validator service provider to use for reporting errors or warnings</param>
        /// <param name="parameterListType">The parameter list type.</param>
        /// <returns>the created parameter list node</returns>
        internal static LSLParameterListNode BuildFromParserContext(LSLParser.OptionalParameterListContext context, LSLParameterListType parameterListType, ILSLValidatorServiceProvider validatorServices)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (validatorServices == null)
            {
                throw new ArgumentNullException("validatorServices");
            }

           
            var result = new LSLParameterListNode(context, parameterListType);

            var parameterList = context.parameterList();

            if (parameterList == null)
            {
                return result;
            }

            var parameterNames = new HashSet<string>();

            var parameterIndex = 0;

            foreach (var comma in parameterList.children)
            {
                //'comma' for some reason will be an internal object
                //that cannot be accessed via cast when a COMMA token is encountered.
                //
                //However, Payload contains a CommonToken instance, which implements IToken.
                var token = comma.Payload as IToken;

                //when a parameter def is found, 'comma' will be the grammar defined
                //LSLParser.ParameterDefinitionContext type.
                var parameter = comma as LSLParser.ParameterDefinitionContext;

                if (token != null)
                {
                    result._sourceRangeCommaList.Add(new LSLSourceCodeRange(token));
                }
                else if (parameter != null)
                {

                    if (parameterNames.Contains(parameter.ID().GetText()))
                    {
                        var paramLocation = new LSLSourceCodeRange(parameter);

                        validatorServices.SyntaxErrorListener.ParameterNameRedefined(
                            paramLocation,
                            LSLTypeTools.FromLSLTypeString(parameter.TYPE().GetText()),
                            parameter.ID().GetText());

                        result.HasErrors = true;

                        result._parameters.Clear();

                        return result;
                    }


                    parameterNames.Add(parameter.ID().GetText());

                    var addition = new LSLParameterNode(parameter)
                    {
                        ParameterIndex = parameterIndex
                    };

                    result.AddParameterNode(addition);

                    parameterIndex++;
                }
            }

            parameterNames.Clear();

            return result;
        }


        /// <summary>
        /// Add a parameter definition node to this parameter list node.
        /// </summary>
        /// <param name="node">The parameter definition node to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the 'node' parameter is null.</exception>
        public void AddParameterNode(LSLParameterNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Parent = this;
            _parameters.Add(node);
        }


        private enum Err
        {
            Err
        }
    }
}