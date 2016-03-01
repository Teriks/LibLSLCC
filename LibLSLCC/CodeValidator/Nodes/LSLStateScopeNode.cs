#region FileInfo

// 
// File: LSLStateScopeNode.cs
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
using LibLSLCC.AntlrParser;
using LibLSLCC.Collections;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLStateScopeNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLStateScopeNode : ILSLStateScopeNode, ILSLSyntaxTreeNode, IEnumerable<ILSLEventHandlerNode>
    {
        private readonly GenericArray<LSLEventHandlerNode> _eventHandlers = new GenericArray<LSLEventHandlerNode>();
        private ILSLSyntaxTreeNode _parent;


        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        // ReSharper disable UnusedParameter.Local
        private LSLStateScopeNode(LSLSourceCodeRange sourceRange, Err err)
            // ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <summary>
        ///     Construct an empty <see cref="LSLStateScopeNode" /> with the given name.
        ///     if <paramref name="stateName" /> is "default", <see cref="IsDefaultState" /> will be set to <c>true</c>.
        /// </summary>
        /// <param name="stateName">The name of the state.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stateName" /> is <c>null</c>.</exception>
        /// <exception cref="LSLInvalidSymbolNameException"><paramref name="stateName" /> contained characters not allowed in an LSL ID token.</exception>
        public LSLStateScopeNode(string stateName)
        {
            if (stateName == null)
            {
                throw new ArgumentNullException("stateName");
            }

            if (!LSLTokenTools.IDRegexAnchored.IsMatch(stateName))
            {
                throw new LSLInvalidSymbolNameException("stateName provided contained characters not allowed in an LSL ID token.");
            }

            StateName = stateName;

            if (StateName == "default")
            {
                IsDefaultState = true;
            }
        }


        /// <exception cref="ArgumentNullException"><paramref name="context" /> is <c>null</c>.</exception>
        internal LSLStateScopeNode(LSLParser.DefaultStateContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            StateName = "default";
            IsDefaultState = true;

            SourceRange = new LSLSourceCodeRange(context);

            SourceRangeOpenBrace = new LSLSourceCodeRange(context.open_brace);
            SourceRangeCloseBrace = new LSLSourceCodeRange(context.close_brace);
            SourceRangeStateName = new LSLSourceCodeRange(context.state_name);
            SourceRangeStateKeyword = new LSLSourceCodeRange(context.state_name);

            SourceRangesAvailable = true;
        }


        /// <exception cref="ArgumentNullException"><paramref name="context" /> is <c>null</c>.</exception>
        internal LSLStateScopeNode(LSLParser.DefinedStateContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            StateName = context.state_name.Text;

            SourceRange = new LSLSourceCodeRange(context);

            SourceRangeOpenBrace = new LSLSourceCodeRange(context.open_brace);
            SourceRangeCloseBrace = new LSLSourceCodeRange(context.close_brace);
            SourceRangeStateName = new LSLSourceCodeRange(context.state_name);
            SourceRangeStateKeyword = new LSLSourceCodeRange(context.state_keyword);

            SourceRangesAvailable = true;
        }


        /// <exception cref="ArgumentNullException"><paramref name="eventHandlers" /> is <c>null</c>.</exception>
        internal LSLStateScopeNode(LSLParser.DefaultStateContext context, IEnumerable<LSLEventHandlerNode> eventHandlers)
            : this(context)

        {
            if (eventHandlers == null)
            {
                throw new ArgumentNullException("eventHandlers");
            }

            foreach (var lslEventHandlerNode in eventHandlers)
            {
                Add(lslEventHandlerNode);
            }

            StateName = "default";
            IsDefaultState = true;

            SourceRange = new LSLSourceCodeRange(context);

            SourceRangeOpenBrace = new LSLSourceCodeRange(context.open_brace);
            SourceRangeCloseBrace = new LSLSourceCodeRange(context.close_brace);
            SourceRangeStateName = new LSLSourceCodeRange(context.state_name);
            SourceRangeStateKeyword = new LSLSourceCodeRange(context.state_name);

            SourceRangesAvailable = true;
        }


        /// <exception cref="ArgumentNullException"><paramref name="eventHandlers" /> is <c>null</c>.</exception>
        internal LSLStateScopeNode(LSLParser.DefinedStateContext context, IEnumerable<LSLEventHandlerNode> eventHandlers)
            : this(context)
        {
            if (eventHandlers == null)
            {
                throw new ArgumentNullException("eventHandlers");
            }

            StateName = context.state_name.Text;

            foreach (var lslEventHandlerNode in eventHandlers)
            {
                Add(lslEventHandlerNode);
            }

            SourceRange = new LSLSourceCodeRange(context);

            SourceRangeOpenBrace = new LSLSourceCodeRange(context.open_brace);
            SourceRangeCloseBrace = new LSLSourceCodeRange(context.close_brace);
            SourceRangeStateName = new LSLSourceCodeRange(context.state_name);
            SourceRangeStateKeyword = new LSLSourceCodeRange(context.state_keyword);

            SourceRangesAvailable = true;
        }


        /*
        /// <summary>
        ///     Create an <see cref="LSLStateScopeNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        public LSLStateScopeNode(LSLStateScopeNode other)
        {
            if (other == null) throw new ArgumentNullException("other");


            SourceRangesAvailable = other.SourceRangesAvailable;
            if (SourceRangesAvailable)
            {
                SourceRangeCloseBrace = other.SourceRangeCloseBrace;
                SourceRangeOpenBrace = other.SourceRangeOpenBrace;
                SourceRangeStateKeyword = other.SourceRangeStateKeyword;
                SourceRangeStateName = other.SourceRangeStateName;
                SourceRange = other.SourceRange;
            }

            IsDefaultState = other.IsDefaultState;
            StateName = other.StateName;

            foreach (var e in EventHandlers)
            {
                AddEventHandler(e.Clone());
            }


            HasErrors = other.HasErrors;
        }*/


        /// <summary>
        ///     A list of event handlers nodes for each event handler that was used in the state.
        ///     This should never be empty.
        /// </summary>
        public IReadOnlyGenericArray<LSLEventHandlerNode> EventHandlers
        {
            get { return _eventHandlers; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        ///     The name of the state block in the source code.
        ///     'default' should be returned if the node represents the default state.
        /// </summary>
        public string StateName { get; private set; }

        /// <summary>
        ///     True if this state scope node represents the 'default' state,  False if it is a user defined state.
        /// </summary>
        public bool IsDefaultState { get; private set; }

        IReadOnlyGenericArray<ILSLEventHandlerNode> ILSLStateScopeNode.EventHandlers
        {
            get { return _eventHandlers; }
        }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLStateScopeNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLStateScopeNode(sourceRange, Err.Err);
        }


        /// <summary>
        ///     Adds an <see cref="LSLEventHandlerNode" /> as a child of this state scope node.
        /// </summary>
        /// <param name="node">The event handler node to add.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Add(LSLEventHandlerNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Parent = this;
            _eventHandlers.Add(node);
        }

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        /// <summary>
        ///     True if this syntax tree node contains syntax errors. <para/>
        ///     <see cref="SourceRange"/> should point to a more specific error location when this is <c>true</c>. <para/>
        ///     Other source ranges will not be available.
        /// </summary>
        public bool HasErrors { get; internal set; }


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
        ///     The source code range of the opening brace of the state block's scope.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeOpenBrace { get; private set; }


        /// <summary>
        ///     The source code range of the closing brace of the state block's scope.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeCloseBrace { get; private set; }


        /// <summary>
        ///     The source code range where the name of the state is located.
        ///     For the default state, this will be the location of the 'default' keyword.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeStateName { get; private set; }

        /// <summary>
        ///     The source code range where the state keyword is located.
        ///     For the default state, this will be the location of the 'default' keyword.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeStateKeyword { get; private set; }


        /// <summary>
        ///     Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}" />
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="visitor"/> is <see langword="null" />.</exception>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");

            return IsDefaultState ? visitor.VisitDefaultState(this) : visitor.VisitDefinedState(this);
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

        #endregion

        /// <summary>
        /// Returns an enumerator that iterates through the  <see cref="ILSLEventHandlerNode"/>'s in the state scope.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ILSLEventHandlerNode> GetEnumerator()
        {
            return _eventHandlers.GetEnumerator();
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