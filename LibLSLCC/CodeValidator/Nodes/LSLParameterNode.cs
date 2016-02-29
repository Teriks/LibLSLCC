#region FileInfo

// 
// File: LSLParameterNode.cs
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
using LibLSLCC.AntlrParser;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLParameterNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLParameterNode : ILSLParameterNode, ILSLSyntaxTreeNode
    {
        private ILSLSyntaxTreeNode _parent;
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLParameterNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <summary>
        ///     Construct a <see cref="LSLParameterNode" /> with the given <see cref="Type" /> and <see cref="Name" />.
        /// </summary>
        /// <param name="type">The <see cref="Type" />.</param>
        /// <param name="parameterName">The <see cref="Name" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameterName" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        ///     if <paramref name="type" /> is <see cref="LSLType.Void" /> or
        ///     <paramref name="parameterName" /> contains characters that are invalid in an LSL ID token.
        /// </exception>
        public LSLParameterNode(LSLType type, string parameterName)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName");
            }

            if (type == LSLType.Void)
            {
                throw new ArgumentException("parameter type cannot be LSLType.Void.", "type");
            }

            if (!LSLTokenTools.IDRegex.IsMatch(parameterName))
            {
                throw new ArgumentException(
                    "parameterName provided contained characters not allowed in an LSL ID token.", "parameterName");
            }

            Name = parameterName;

            Type = type;

            TypeName = Type.ToLSLTypeName();
        }


        /// <exception cref="ArgumentNullException"><paramref name="context" /> is <c>null</c>.</exception>
        internal LSLParameterNode(LSLParser.ParameterDefinitionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }


            Name = context.parameter_name.Text;

            TypeName = context.parameter_type.Text;

            Type = LSLTypeTools.FromLSLTypeName(TypeName);

            SourceRange = new LSLSourceCodeRange(context);
            SourceRangeType = new LSLSourceCodeRange(context.parameter_type);
            SourceRangeName = new LSLSourceCodeRange(context.parameter_name);

            SourceRangesAvailable = true;
        }


        /// <summary>
        ///     Create an <see cref="LSLParameterNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        public LSLParameterNode(ILSLParameterNode other)
        {
            if (other == null) throw new ArgumentNullException("other");


            SourceRangesAvailable = other.SourceRangesAvailable;
            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
                SourceRangeName = other.SourceRangeName;
                SourceRangeType = other.SourceRangeType;
            }

            TypeName = other.TypeName;
            Type = other.Type;
            Name = other.Name;
            ParameterIndex = other.ParameterIndex;

            HasErrors = other.HasErrors;
        }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        ///     The name of the parameter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     The <see cref="LSLType" /> associated with the parameter.
        /// </summary>
        public LSLType Type { get; private set; }

        /// <summary>
        ///     The string representation of the <see cref="LSLType" /> for the parameter, taken from the source code.
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        ///     The zero based index of the parameter definition in its parent <see cref="ILSLParameterListNode" />.
        /// </summary>
        public int ParameterIndex { get; set; }

        /// <summary>
        ///     The source code range of the parameter name.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeName { get; private set; }

        /// <summary>
        ///     The source code range of the parameter type specifier.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeType { get; private set; }

        /// <summary>
        ///     True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; private set; }

        /// <summary>
        ///     The source code range that this syntax tree node occupies.
        /// </summary>
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
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitParameterDefinition(this);
        }


        /// <summary>
        ///     The parent node of this syntax tree node.
        /// </summary>
        /// <exception cref="InvalidOperationException" accessor="set">If Parent has already been set.</exception>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value" /> is <see langword="null" />.</exception>
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
        ///     Deep clones the syntax tree node.  It should clone the node and all of its children and cloneable properties,
        ///     except the parent.
        ///     When cloned, the parent node reference should be left <c>null</c>.
        /// </summary>
        /// <returns>A deep clone of this syntax tree node.</returns>
        public LSLParameterNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLParameterNode(this);
        }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLParameterNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLParameterNode(sourceRange, Err.Err);
        }


        private enum Err
        {
            Err
        }
    }
}