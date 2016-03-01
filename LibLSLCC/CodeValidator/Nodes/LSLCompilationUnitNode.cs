#region FileInfo

// 
// File: LSLCompilationUnitNode.cs
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
using System.Linq;
using LibLSLCC.AntlrParser;
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLCompilationUnitNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLCompilationUnitNode : ILSLCompilationUnitNode, ILSLSyntaxTreeNode
    {
        private readonly GenericArray<LSLFunctionDeclarationNode> _functionDeclarations =
            new GenericArray<LSLFunctionDeclarationNode>();

        private readonly GenericArray<LSLVariableDeclarationNode> _globalVariableDeclarations =
            new GenericArray<LSLVariableDeclarationNode>();

        private readonly GenericArray<LSLStateScopeNode> _stateDeclarations
            = new GenericArray<LSLStateScopeNode>();

        private int _addCounter;
        private LSLStateScopeNode _defaultState;
        private ILSLSyntaxTreeNode _parent;
        private readonly IReadOnlyGenericArray<LSLComment> _comments;

// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLCompilationUnitNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <summary>
        ///     Construct an <see cref="LSLCompilationUnitNode" /> with an empty default state node.
        /// </summary>
        public LSLCompilationUnitNode()
        {
            DefaultState = new LSLStateScopeNode("default");

            _comments = new GenericArray<LSLComment>();
        }


        /// <summary>
        ///     Construct an <see cref="LSLCompilationUnitNode" /> with the provided default state node.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="defaultState" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="defaultState" />.IsDefaultState is <c>false</c>.</exception>
        public LSLCompilationUnitNode(LSLStateScopeNode defaultState)
        {
            if (defaultState == null) throw new ArgumentNullException("defaultState");

            if (!defaultState.IsDefaultState)
            {
                throw new ArgumentException("defaultState.IsDefaultState is false", "defaultState");
            }

            DefaultState = defaultState;

            _comments = new GenericArray<LSLComment>();
        }


        /// <exception cref="ArgumentNullException"><paramref name="context" /> is <c>null</c>.</exception>
        internal LSLCompilationUnitNode(LSLParser.CompilationUnitContext context, IReadOnlyGenericArray<LSLComment> comments)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            SourceRange = new LSLSourceCodeRange(context);

            SourceRangesAvailable = true;

            _comments = comments;
        }


        /*
        /// <summary>
        ///     Create an <see cref="LSLCompilationUnitNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        public LSLCompilationUnitNode(LSLCompilationUnitNode other)
        {
            if (other == null) throw new ArgumentNullException("other");


            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
            }

            Comments = other.Comments.ToGenericArray();

            foreach (var v in other.GlobalVariableDeclarations)
            {
                AddVariableDeclaration((LSLVariableDeclarationNode) v.Clone());
            }

            foreach (var f in other.FunctionDeclarations)
            {
                AddFunctionDeclaration(f.Clone());
            }

            foreach (var s in other.StateDeclarations)
            {
                AddStateDeclaration(s.Clone());
            }

            HasErrors = other.HasErrors;
        }*/


        /// <summary>
        ///     Global variable declaration nodes, in order of appearance.
        ///     Returns an empty list if none exist.
        /// </summary>
        public IReadOnlyGenericArray<LSLVariableDeclarationNode> GlobalVariableDeclarations
        {
            get { return _globalVariableDeclarations; }
        }

        /// <summary>
        ///     User defined function nodes, in order of appearance.
        ///     Returns an empty list if none exist.
        /// </summary>
        public IReadOnlyGenericArray<LSLFunctionDeclarationNode> FunctionDeclarations
        {
            get { return _functionDeclarations; }
        }

        /// <summary>
        ///     User defined state nodes, in order of appearance.
        ///     Returns an empty list if none exist.
        /// </summary>
        public IReadOnlyGenericArray<LSLStateScopeNode> StateDeclarations
        {
            get { return _stateDeclarations; }
        }

        /// <summary>
        ///     The state node for the default script state.
        /// </summary>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value" /> is <c>null</c>.</exception>
        public LSLStateScopeNode DefaultState
        {
            get { return _defaultState; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _defaultState = value;
                _defaultState.Parent = this;
            }
        }

        /// <summary>
        ///     A list of objects describing the comments found in the source code and their position/range.
        ///     Will always be non null, even if there are no comments.
        /// </summary>
        public IReadOnlyGenericArray<LSLComment> Comments
        {
            get { return _comments; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        IReadOnlyGenericArray<ILSLVariableDeclarationNode> ILSLCompilationUnitNode.GlobalVariableDeclarations
        {
            get { return _globalVariableDeclarations; }
        }

        IReadOnlyGenericArray<ILSLFunctionDeclarationNode> ILSLCompilationUnitNode.FunctionDeclarations
        {
            get { return _functionDeclarations; }
        }

        IReadOnlyGenericArray<ILSLStateScopeNode> ILSLCompilationUnitNode.StateDeclarations
        {
            get { return _stateDeclarations; }
        }

        ILSLStateScopeNode ILSLCompilationUnitNode.DefaultStateNode
        {
            get { return DefaultState; }
        }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLCompilationUnitNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLCompilationUnitNode(sourceRange, Err.Err);
        }


        /// <summary>
        ///     Add a global variable declaration node to this compilation unit node.
        /// </summary>
        /// <param name="declaration">The global variable declaration node to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the 'declaration' parameter is <c>null</c>.</exception>
        public void Add(LSLVariableDeclarationNode declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }

            declaration.Parent = this;
            declaration.StatementIndex = _addCounter;
            declaration.IsLastStatementInScope = true;

            if (_globalVariableDeclarations.Count > 0)
            {
                _globalVariableDeclarations.Last().IsLastStatementInScope = false;
            }

            _addCounter++;
            _globalVariableDeclarations.Add(declaration);
        }


        /// <summary>
        ///     Add a function declaration node to this compilation unit node.
        /// </summary>
        /// <param name="declaration">The function declaration node to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the 'declaration' parameter is <c>null</c>.</exception>
        public void Add(LSLFunctionDeclarationNode declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }


            declaration.Parent = this;

            if (_globalVariableDeclarations.Count > 0)
            {
                _globalVariableDeclarations.Last().IsLastStatementInScope = false;
            }

            _addCounter++;
            _functionDeclarations.Add(declaration);
        }


        /// <summary>
        ///     Add a state declaration node to this compilation unit node.
        /// </summary>
        /// <param name="declaration">The state declaration node to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the 'declaration' parameter is <c>null</c>.</exception>
        public void Add(LSLStateScopeNode declaration)
        {
            if (declaration == null)
            {
                throw new ArgumentNullException("declaration");
            }

            declaration.Parent = this;

            _addCounter++;
            _stateDeclarations.Add(declaration);
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
        ///     Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}" />
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="visitor"/> is <c>null</c>.</exception>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");

            return visitor.VisitCompilationUnit(this);
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

        #endregion
    }
}