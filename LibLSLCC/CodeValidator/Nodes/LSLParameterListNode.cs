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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Antlr4.Runtime;
using LibLSLCC.AntlrParser;
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLParameterListNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLParameterListNode : ILSLParameterListNode, IEnumerable<ILSLParameterNode>
    {
        private readonly HashMap<string, LSLParameterNode> _parameters = new HashMap<string, LSLParameterNode>();
        private readonly GenericArray<LSLSourceCodeRange> _sourceRangeCommaList = new GenericArray<LSLSourceCodeRange>();
        private ILSLSyntaxTreeNode _parent;
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLParameterListNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        private LSLParameterListNode(LSLParser.OptionalParameterListContext context,
            LSLParameterListType parameterListType)
        {
            SourceRange = new LSLSourceCodeRange(context);

            SourceRangesAvailable = true;

            ParameterListType = parameterListType;
        }


        /// <summary>
        ///     Construct an empty <see cref="LSLParameterListNode" />.
        /// </summary>
        public LSLParameterListNode()
        {
        }


        /// <summary>
        ///     Construct an <see cref="LSLParameterListNode" /> with the parameter nodes in <paramref name="parameters" />.
        /// </summary>
        /// <param name="parameters">The parameters to fill this parameter list node with.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameters" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        ///     If two parameters with the same name exist in <paramref name="parameters" />.
        /// </exception>
        public LSLParameterListNode(IEnumerable<LSLParameterNode> parameters)
        {
            if (parameters == null) throw new ArgumentNullException("parameters");

            foreach (var v in parameters)
            {
                Add(v);
            }
        }


        /// <summary>
        ///     Construct an <see cref="LSLParameterListNode" /> with the parameter nodes in <paramref name="parameters" />.
        /// </summary>
        /// <param name="parameters">The parameters to fill this parameter list node with.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameters" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        ///     If two parameters with the same name exist in <paramref name="parameters" />.
        /// </exception>
        public LSLParameterListNode(params LSLParameterNode[] parameters)
        {
            if (parameters == null) throw new ArgumentNullException("parameters");

            foreach (var v in parameters)
            {
                Add(v);
            }
        }


        /// <summary>
        ///     Create an <see cref="LSLParameterListNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        private LSLParameterListNode(LSLParameterListNode other)
        {
            if (other == null) throw new ArgumentNullException("other");


            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
                _sourceRangeCommaList = other.SourceRangeCommaList.ToGenericArray();
            }

            foreach (var param in other._parameters.Values)
            {
                Add(param.Clone());
            }

            HasErrors = other.HasErrors;
        }


        /// <summary>
        ///     A list of parameter definition nodes that this parameter list node contains, or an empty list. <para/>
        ///     This will never be <c>null</c>.
        /// </summary>
        public IReadOnlyGenericArray<ILSLParameterNode> Parameters
        {
            get { return _parameters.Values.ToGenericArray(); }
        }

        /// <summary>
        ///     The source code range for each comma separator that appears in the parameter list in order, or an empty list. <para/>
        ///     This will never be <c>null</c>.
        /// </summary>
        public IReadOnlyGenericArray<LSLSourceCodeRange> SourceRangeCommaList
        {
            get { return _sourceRangeCommaList; }
        }

        /// <summary>
        ///     The parameter list type;  FunctionParameters or EventParameters.
        /// </summary>
        public LSLParameterListType ParameterListType { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        /// <summary>
        ///     The parent node of this syntax tree node.
        /// </summary>
        /// <exception cref="InvalidOperationException" accessor="set">If Parent has already been set.</exception>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value" /> is <c>null</c>.</exception>
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
        ///     True if this syntax tree node contains syntax errors. <para/>
        ///     <see cref="SourceRange"/> should point to a more specific error location when this is <c>true</c>. <para/>
        ///     Other source ranges will not be available.
        /// </summary>
        public bool HasErrors { get; private set; }

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
        ///     Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}" />
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="visitor"/> is <c>null</c>.</exception>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");

            return visitor.VisitParameterDefinitionList(this);
        }


        /// <summary>
        ///     Deep clones the syntax tree node.  It should clone the node and all of its children and cloneable properties,
        ///     except the parent.
        ///     When cloned, the parent node reference should be left <c>null</c>.
        /// </summary>
        /// <returns>A deep clone of this syntax tree node.</returns>
        public LSLParameterListNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLParameterListNode(this);
        }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
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
        ///     duplicate parameter errors via a validator strategies object. <see cref="ILSLCodeValidatorStrategies" />.
        /// </summary>
        /// <param name="context">The context to build from</param>
        /// <param name="validatorStrategies">The validator strategies object to use for reporting errors or warnings</param>
        /// <param name="parameterListType">The parameter list type.</param>
        /// <returns>the created parameter list node</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="context" /> or <paramref name="validatorStrategies" /> is
        ///     <c>null</c>.
        /// </exception>
        internal static LSLParameterListNode BuildFromParserContext(LSLParser.OptionalParameterListContext context,
            LSLParameterListType parameterListType, ILSLCodeValidatorStrategies validatorStrategies)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (validatorStrategies == null)
            {
                throw new ArgumentNullException("validatorStrategies");
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

                        validatorStrategies.SyntaxErrorListener.ParameterNameRedefined(
                            paramLocation,
                            parameterListType,
                            LSLTypeTools.FromLSLTypeName(parameter.TYPE().GetText()),
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

                    result.Add(addition);

                    parameterIndex++;
                }
            }

            parameterNames.Clear();

            return result;
        }


        /// <summary>
        ///     Add a parameter definition node to this parameter list node.
        /// </summary>
        /// <param name="node">The parameter definition node to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the 'node' parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        ///     If a parameter with the same name already exists.
        /// </exception>
        public void Add(LSLParameterNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (_parameters.ContainsKey(node.Name))
            {
                throw new ArgumentException(
                    "Parameter with the name \"{0}\" has already been added to the parameter list.");
            }

            node.Parent = this;
            node.ParameterIndex = _parameters.Count;
            _parameters.Add(node.Name, node);
        }


        private enum Err
        {
            Err
        }


        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ILSLParameterNode"/>'s in this parameter list.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ILSLParameterNode> GetEnumerator()
        {
            return Parameters.GetEnumerator();
        }


        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}