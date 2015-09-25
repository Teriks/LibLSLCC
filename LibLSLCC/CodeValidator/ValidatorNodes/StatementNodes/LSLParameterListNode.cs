#region FileInfo

// 
// File: LSLParameterListNode.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:24 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Imports

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLParameterListNode : ILSLParameterListNode
    {
        private readonly List<LSLParameterNode> _parameters = new List<LSLParameterNode>();
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
        }

        public LSLParameterListNode(LSLParser.OptionalParameterListContext context)
        {
            ParserContext = context;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }

        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLParameterListNode(Err err)
            // ReSharper restore UnusedParameter.Local
        {
        }

        internal LSLParser.OptionalParameterListContext ParserContext { get; private set; }

        public IReadOnlyList<LSLParameterNode> Parameters
        {
            get { return _parameters; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public bool HasParameterNodes
        {
            get { return _parameters.Count > 0; }
        }

        IReadOnlyList<ILSLParameterNode> ILSLParameterListNode.Parameters
        {
            get { return _parameters; }
        }

        public ILSLSyntaxTreeNode Parent { get; set; }
        public bool HasErrors { get; set; }
        public LSLSourceCodeRange SourceCodeRange { get; }

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
        ///     duplicate parameter errors via the validatorServices ILSLValidatorServiceProvider.
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

        public void AddParameterNode(LSLParameterNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Parent = this;
            _parameters.Add(node);
        }

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