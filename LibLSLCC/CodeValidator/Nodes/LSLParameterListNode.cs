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
    public class LSLParameterListNode : ILSLParameterListNode
    {
        private readonly GenericArray<LSLParameterNode> _parameters = new GenericArray<LSLParameterNode>();
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLParameterListNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            //this.SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLParameterListNode(LSLParser.OptionalParameterListContext context,
            IEnumerable<LSLParameterNode> parameterNodes)
        {
            if (parameterNodes == null)
            {
                throw new ArgumentNullException("parameterNodes");
            }


            foreach (var lslParameterNode in _parameters)
            {
                AddParameterNode(lslParameterNode);
            }

            ParserContext = context;
            SourceCodeRange = new LSLSourceCodeRange(context);

            SourceCodeRangesAvailable = true;
        }

        public LSLParameterListNode(LSLParser.OptionalParameterListContext context)
        {
            ParserContext = context;
            SourceCodeRange = new LSLSourceCodeRange(context);

            SourceCodeRangesAvailable = true;
        }


        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLParameterListNode(Err err)
            // ReSharper restore UnusedParameter.Local
        {
        }

        internal LSLParser.OptionalParameterListContext ParserContext { get; private set; }


        /// <summary>
        /// The <see cref="LSLParameterNode"/> objects that are children of this node, or an empty list.
        /// </summary>
        public IReadOnlyGenericArray<LSLParameterNode> Parameters
        {
            get { return _parameters; }
        }

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
        public bool HasErrors { get; set; }


        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceCodeRangesAvailable { get; private set; }


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
        /// <param name="onAdd">an optional action to preform on each parameter when it is added</param>
        /// <returns>the created parameter list node</returns>
        public static LSLParameterListNode BuildDirectlyFromContext(
            LSLParser.OptionalParameterListContext context,
            ILSLValidatorServiceProvider validatorServices, Action<LSLParameterNode> onAdd = null)

        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (validatorServices == null)
            {
                throw new ArgumentNullException("validatorServices");
            }


            var result = new LSLParameterListNode(context);

            var parameterList = context.parameterList();

            if (parameterList == null)
            {
                return result;
            }


            var parameterNames = new HashSet<string>();


            var parameterIndex = 0;
            foreach (var parameter in parameterList.parameterDefinition())
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

                if (onAdd != null)
                {
                    onAdd(addition);
                }

                parameterIndex++;
            }


            parameterNames.Clear();

            return result;
        }

        /// <summary>
        ///     Builds a parameter list node directly from a parser context, without checking for duplicate parameters
        /// </summary>
        /// <param name="context">The context to build from</param>
        /// <param name="onAdd">an optional action to preform on each parameter when it is added</param>
        /// <returns>the created parameter list node</returns>
        public static LSLParameterListNode BuildDirectlyFromContext(
            LSLParser.OptionalParameterListContext context, Action<LSLParameterNode> onAdd = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var result = new LSLParameterListNode(context);

            var parameterList = context.parameterList();

            if (parameterList == null)
            {
                return result;
            }

            var parameterIndex = 0;
            foreach (var parameter in parameterList.parameterDefinition())
            {
                var addition = new LSLParameterNode(parameter)
                {
                    ParameterIndex = parameterIndex
                };

                result.AddParameterNode(addition);

                if (onAdd != null)
                {
                    onAdd(addition);
                }

                parameterIndex++;
            }

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

        /// <summary>
        /// Clear all parameter definition nodes from this parameter list node.
        /// </summary>
        public void ClearParameters()
        {
            _parameters.Clear();
        }

        protected enum Err
        {
            Err
        }
    }
}