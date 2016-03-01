#region FileInfo

// 
// File: LSLControlStatementNode.cs
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
using LibLSLCC.AntlrParser;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLControlStatementNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLControlStatementNode : ILSLControlStatementNode, ILSLCodeStatement
    {
        private readonly List<LSLElseIfStatementNode> _elseIfStatement = new List<LSLElseIfStatementNode>();
        private LSLElseStatementNode _elseStatement;
        private LSLIfStatementNode _ifStatement;
        private ILSLSyntaxTreeNode _parent;
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLControlStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }



        /// <summary>
        ///     Construct an <see cref="LSLControlStatementNode" /> with an 'if' statement
        ///     node.
        /// </summary>
        /// <param name="ifStatement">The if statement that starts the control chain.</param>
        /// <exception cref="ArgumentNullException"><paramref name="ifStatement" /> is <c>null</c>.</exception>
        public LSLControlStatementNode(LSLIfStatementNode ifStatement)
        {
            if (ifStatement == null) throw new ArgumentNullException("ifStatement");

            IfStatement = ifStatement;
        }


        /// <summary>
        ///     Construct an <see cref="LSLControlStatementNode" /> with 'if' and 'else'
        ///     statement nodes.
        /// </summary>
        /// <param name="ifStatement">The if statement that starts the control chain.</param>
        /// <param name="elseStatement">The else statement.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="ifStatement" /> or <paramref name="elseStatement" /> is
        ///     <c>null</c>.
        /// </exception>
        public LSLControlStatementNode(LSLIfStatementNode ifStatement, LSLElseStatementNode elseStatement)
        {
            if (ifStatement == null) throw new ArgumentNullException("ifStatement");
            if (elseStatement == null) throw new ArgumentNullException("elseStatement");

            IfStatement = ifStatement;
            ElseStatement = elseStatement;
        }



        /// <summary>
        ///     Construct an <see cref="LSLControlStatementNode" /> with 'if', 'else-if' and
        ///     'else' statement nodes.
        /// </summary>
        /// <param name="ifStatement">The if statement that starts the control chain.</param>
        /// <param name="elseIfStatements">Else-if statements.</param>
        /// <param name="elseStatement">The else statement.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="ifStatement" /> or <paramref name="elseIfStatements" /> or
        ///     <paramref name="elseStatement" /> is <c>null</c>.
        /// </exception>
        public LSLControlStatementNode(LSLIfStatementNode ifStatement,
            IEnumerable<LSLElseIfStatementNode> elseIfStatements, LSLElseStatementNode elseStatement)
        {
            if (ifStatement == null) throw new ArgumentNullException("ifStatement");
            if (elseIfStatements == null) throw new ArgumentNullException("elseIfStatements");
            if (elseStatement == null) throw new ArgumentNullException("elseStatement");


            IfStatement = ifStatement;
            ElseStatement = elseStatement;

            foreach (var elif in elseIfStatements)
            {
                AddElseIfStatement(elif);
            }
        }




        /// <summary>
        ///     Construct an <see cref="LSLControlStatementNode" />  with 'if' and 'else-if'
        ///     statement nodes.
        /// </summary>
        /// <param name="ifStatement">The if statement that starts the control chain.</param>
        /// <param name="elseIfStatements">Else-if statements.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="ifStatement" /> or <paramref name="elseIfStatements" /> is
        ///     <c>null</c>.
        /// </exception>
        public LSLControlStatementNode(LSLIfStatementNode ifStatement,
            IEnumerable<LSLElseIfStatementNode> elseIfStatements)
        {
            if (ifStatement == null) throw new ArgumentNullException("ifStatement");
            if (elseIfStatements == null) throw new ArgumentNullException("elseIfStatements");


            IfStatement = ifStatement;

            foreach (var elif in elseIfStatements)
            {
                AddElseIfStatement(elif);
            }
        }


        /// <exception cref="ArgumentNullException"><paramref name="context" /> is <c>null</c>.</exception>
        internal LSLControlStatementNode(LSLParser.ControlStructureContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            SourceRange = new LSLSourceCodeRange(context);

            SourceRangesAvailable = true;
        }


        /*
        /// <summary>
        ///     Create an <see cref="LSLControlStatementNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        private LSLControlStatementNode(LSLControlStatementNode other)
        {
            if (other == null) throw new ArgumentNullException("other");

            SourceRangesAvailable = other.SourceRangesAvailable;
            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
            }

            if (other.HasIfStatement)
            {
                IfStatement = other.IfStatement.Clone();
                IfStatement.Parent = this;
            }

            if (other.HasElseIfStatements)
            {
                foreach (var e in other.ElseIfStatements)
                {
                    AddElseIfStatement(e.Clone());
                }
            }

            if (other.HasElseStatement)
            {
                ElseStatement = other.ElseStatement.Clone();
                ElseStatement.Parent = this;
            }

            LSLStatementNodeTools.CopyStatement(this, other);

            HasErrors = other.HasErrors;
        }*/


        /// <summary>
        ///     The else statement child of this control statement node if one exists, otherwise <c>null</c>.
        /// </summary>
        public LSLElseStatementNode ElseStatement
        {
            get { return _elseStatement; }
            set
            {
                if (value != null)
                {
                    value.Parent = this;
                }

                _elseStatement = value;
            }
        }

        /// <summary>
        ///     The if statement child of this control statement node if one exists, otherwise <c>null</c>.
        /// </summary>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value" /> is <c>null</c>.</exception>
        public LSLIfStatementNode IfStatement
        {
            get { return _ifStatement; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                value.Parent = this;
                _ifStatement = value;
            }
        }

        /// <summary>
        ///     The else-if statement children of this control statement node if one exists, otherwise an empty enumerable.
        /// </summary>
        public IEnumerable<LSLElseIfStatementNode> ElseIfStatements
        {
            get { return _elseIfStatement; }
        }

        /// <summary>
        ///     The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        public LSLDeadCodeType DeadCodeType { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        ///     True if the control statement node has an else statement child.
        /// </summary>
        public bool HasElseStatement
        {
            get { return ElseStatement != null; }
        }

        /// <summary>
        ///     True if the control statement node has an if statement child.
        ///     This can only really be false if the node contains errors.
        /// </summary>
        public bool HasIfStatement
        {
            get { return IfStatement != null; }
        }

        /// <summary>
        ///     True if the control statement node has any if-else statement children.
        /// </summary>
        public bool HasElseIfStatements
        {
            get { return ElseIfStatements.Any(); }
        }

        ILSLElseStatementNode ILSLControlStatementNode.ElseStatement
        {
            get { return ElseStatement; }
        }

        ILSLIfStatementNode ILSLControlStatementNode.IfStatement
        {
            get { return IfStatement; }
        }

        IEnumerable<ILSLElseIfStatementNode> ILSLControlStatementNode.ElseIfStatement
        {
            get { return _elseIfStatement; }
        }

        /// <summary>
        ///     Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        ///     this is not the scopes level.
        /// </summary>
        public int ParentScopeId { get; set; }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLControlStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLControlStatementNode(sourceRange, Err.Err);
        }


        /// <summary>
        ///     Add an <see cref="LSLElseIfStatementNode" /> to this control statement chain.
        /// </summary>
        /// <param name="node">The <see cref="LSLElseIfStatementNode" /> to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="node" /> is <c>null</c>.</exception>
        public void AddElseIfStatement(LSLElseIfStatementNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Parent = this;
            _elseIfStatement.Add(node);
        }


        private bool HaveReturnPath()
        {
            if (!ElseIfStatements.Any())
            {
                return IfStatement.HasReturnPath && (ElseStatement != null && ElseStatement.HasReturnPath);
            }

            return IfStatement.HasReturnPath && ElseIfStatements.All(x => x.HasReturnPath) &&
                   (ElseStatement != null && ElseStatement.HasReturnPath);
        }

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLCodeStatement Members

        /// <summary>
        ///     True if this statement belongs to a single statement code scope.
        ///     A single statement code scope is a braceless code scope that can be used in control or loop statements.
        /// </summary>
        /// <seealso cref="ILSLCodeScopeNode.IsSingleStatementScope" />
        public bool InsideSingleStatementScope { get; set; }


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
        ///     The index of this statement in its scope
        /// </summary>
        public int StatementIndex { get; set; }


        /// <summary>
        ///     Is this statement the last statement in its scope
        /// </summary>
        public bool IsLastStatementInScope { get; set; }


        /// <summary>
        ///     Is this statement dead code
        /// </summary>
        public bool IsDeadCode { get; set; }


        /// <summary>
        ///     True if the node represents a return path out of its ILSLCodeScopeNode parent, False otherwise.
        /// </summary>
        public bool HasReturnPath
        {
            get { return HaveReturnPath(); }
        }


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

            return visitor.VisitControlStatement(this);
        }


        /// <summary>
        ///     Returns a constant jump from this control statement chain if there is one,
        ///     otherwise null.  A constant jump is a singular jump to the same label
        ///     from every possible branch of the control chain. meaning the jump
        ///     happens in a constant manner regardless of which branch is taken.
        ///     DeterminingJump will point to the <see cref="LSLJumpStatementNode" /> in the 'if' code scope.
        ///     EffectiveJumpStatement will point to the control statement, because it can effectively
        ///     be considered a jump statement since it will always cause a jump to a known label to occur.
        ///     The label that this control statement jumps to can be found with DeterminingJump.JumpTarget.
        ///     EffectiveJumpStatement is mostly useful for its statement index information
        /// </summary>
        /// <returns>An object describing the constant jump, if one exists; otherwise <c>null</c>.</returns>
        /// <seealso cref="LSLConstantJumpDescription.DeterminingJump" />
        /// <seealso cref="LSLConstantJumpDescription.EffectiveJumpStatement" />
        /// <seealso cref="LSLConstantJumpDescription.JumpTarget" />
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        internal LSLConstantJumpDescription GetConstantJump()
        {
            if (!HasElseIfStatements && !HasElseStatement)
            {
                return null;
            }

            var cmp = new JumpCmp();
            var i = new HashSet<LSLConstantJumpDescription>(IfStatement.ConstantJumps, cmp);
            if (HasElseIfStatements)
            {
                foreach (var node in ElseIfStatements)
                {
                    var ie = new HashSet<LSLConstantJumpDescription>(node.ConstantJumps, cmp);
                    i.IntersectWith(ie);
                }
            }
            if (HasElseStatement)
            {
                var e = new HashSet<LSLConstantJumpDescription>(ElseStatement.ConstantJumps, cmp);
                i.IntersectWith(e);
            }

            var x = i.SingleOrDefault();

            if (x != null)
            {
                x = new LSLConstantJumpDescription(x, this);
            }

            return x;
        }


        private class JumpCmp :
            IEqualityComparer<LSLConstantJumpDescription>
        {
            public bool Equals(LSLConstantJumpDescription x, LSLConstantJumpDescription y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                return x.DeterminingJump.JumpTarget == y.DeterminingJump.JumpTarget &&
                       x.DeterminingJump.JumpTarget.ParentScopeId == y.DeterminingJump.JumpTarget.ParentScopeId;
            }


            public int GetHashCode(LSLConstantJumpDescription obj)
            {
                if (obj == null) return -1;

                var hash = 17;
                hash = hash*31 + obj.DeterminingJump.JumpTarget.ParentScopeId.GetHashCode();
                hash = hash*31 + obj.DeterminingJump.LabelName.GetHashCode();

                return hash;
            }
        }

        #endregion
    }
}