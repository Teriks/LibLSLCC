#region FileInfo

// 
// File: LSLVariableDeclarationNode.cs
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
using Antlr4.Runtime;
using LibLSLCC.AntlrParser;
using LibLSLCC.Collections;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLVariableDeclarationNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLVariableDeclarationNode : ILSLVariableDeclarationNode, ILSLCodeStatement
    {
        private readonly GenericArray<LSLVariableNode> _references = new GenericArray<LSLVariableNode>();
        private ILSLSyntaxTreeNode _parent;
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLVariableDeclarationNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        private LSLVariableDeclarationNode()
        {
        }


        /// <summary>
        ///     Create an <see cref="LSLVariableDeclarationNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        public LSLVariableDeclarationNode(LSLVariableDeclarationNode other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }


            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
                SourceRangeType = other.SourceRangeType;
                SourceRangeName = other.SourceRangeName;
                SourceRangeOperator = other.SourceRangeOperator;
            }

            VariableNode = other.VariableNode.Clone();
            VariableNode.Parent = this;


            if (other.HasDeclarationExpression)
            {
                DeclarationExpression = other.DeclarationExpression.Clone();
                DeclarationExpression.Parent = this;
            }

            LSLStatementNodeTools.CopyStatement(this, other);

            HasErrors = other.HasErrors;
        }


        /// <summary>
        ///     The variable node that was created upon construction of
        ///     this variable declaration
        /// </summary>
        public LSLVariableNode VariableNode { get; private set; }

        /// <summary>
        ///     The expression used to initialize this variable declaration, this will be null if 'HasDeclarationExpression' is
        ///     false.
        ///     If neither 'IsLocal' or 'IsGlobal' are true, than this property will always be null.
        /// </summary>
        public ILSLExprNode DeclarationExpression { get; private set; }

        /// <summary>
        ///     A list of variable nodes representing references to this variable declaration in the source code.  Or an empty
        ///     list.
        ///     ILSLVariableNodes are used to represent a reference to a declared variable, and are present in the syntax tree at
        ///     the site of reference.
        /// </summary>
        public IReadOnlyGenericArray<LSLVariableNode> References
        {
            get { return _references; }
        }

        /// <summary>
        ///     The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        public LSLDeadCodeType DeadCodeType { get; set; }

        /// <summary>
        ///     The name of the variable.
        /// </summary>
        public string Name
        {
            get { return VariableNode.Name; }
        }

        /// <summary>
        ///     The variable type.
        /// </summary>
        public LSLType Type
        {
            get { return VariableNode.Type; }
        }

        /// <summary>
        ///     The raw type string representing the variable type, taken from the source code.
        /// </summary>
        public string TypeName
        {
            get { return VariableNode.TypeName; }
        }

        ILSLVariableNode ILSLVariableDeclarationNode.VariableNode
        {
            get { return VariableNode; }
        }

        /// <summary>
        ///     True if an expression was used to initialize this variable declaration node when it was defined.
        /// </summary>
        public bool HasDeclarationExpression
        {
            get { return DeclarationExpression != null; }
        }

        /// <summary>
        ///     True if this variable declaration is local to a function or event handler.  False if it is a global variable,
        ///     parameter definition, or library constant.
        /// </summary>
        public bool IsLocal
        {
            get { return VariableNode.IsLocal; }
        }

        /// <summary>
        ///     True if this variable declaration is in the global program scope.  False if it is a local variable, parameter
        ///     definition, or library constant.
        /// </summary>
        public bool IsGlobal
        {
            get { return VariableNode.IsGlobal; }
        }

        /// <summary>
        ///     True if this variable declaration represents a local function/event handler parameter.  False if it is a local
        ///     variable, global variable, or library constant.
        /// </summary>
        public bool IsParameter
        {
            get { return VariableNode.IsParameter; }
        }

        /// <summary>
        ///     True if this variable declaration represents a library defined constant.  False if it is a local variable, global
        ///     variable, or parameter definition.
        /// </summary>
        public bool IsLibraryConstant
        {
            get { return VariableNode.IsLibraryConstant; }
        }

        ILSLReadOnlyExprNode ILSLVariableDeclarationNode.DeclarationExpression
        {
            get { return DeclarationExpression; }
        }

        /// <summary>
        ///     Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        ///     this is not the scopes level.
        /// </summary>
        public int ParentScopeId { get; set; }

        IReadOnlyGenericArray<ILSLVariableNode> ILSLVariableDeclarationNode.References
        {
            get { return References; }
        }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLVariableDeclarationNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLVariableDeclarationNode(sourceRange, Err.Err);
        }


        /// <summary>
        ///     Creates a reference to VariableNode by cloning and setting its <see cref="LSLVariableNode.SourceRange" />
        ///     to that of <paramref name="referenceToken" />
        /// </summary>
        /// <param name="referenceToken">The variable reference token from the parser</param>
        /// <returns>VariableNode cloned, with its <see cref="LSLVariableNode.SourceRange" /> set</returns>
        /// <exception cref="ArgumentNullException"><paramref name="referenceToken" /> is <c>null</c>.</exception>
        internal LSLVariableNode CreateReference(IToken referenceToken)
        {
            if (referenceToken == null)
            {
                throw new ArgumentNullException("referenceToken");
            }

            var v = VariableNode.Clone();

            v.SourceRange = new LSLSourceCodeRange(referenceToken);
            SourceRangesAvailable = true;
            _references.Add(v);

            return v;
        }


        /// <summary>
        ///     Creates a reference to <see cref="VariableNode" /> by cloning and setting its
        ///     <see cref="LSLVariableNode.SourceRange" />
        ///     to that of <paramref name="range" />
        /// </summary>
        /// <param name="range">The source-code range of the variable token from the parser</param>
        /// <returns>VariableNode cloned, with its <see cref="LSLVariableNode.SourceRange" /> set</returns>
        /// <exception cref="ArgumentNullException"><paramref name="range" /> is <c>null</c>.</exception>
        internal LSLVariableNode CreateReference(LSLSourceCodeRange range)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            var v = VariableNode.Clone();

            v.SourceRange = range;
            SourceRangesAvailable = true;
            _references.Add(v);

            return v;
        }


        /// <summary>
        ///     Creates a reference to this variable declaration by cloning <see cref="VariableNode" />.
        /// </summary>
        /// <returns>A clone of <see cref="VariableNode" />.</returns>
        public LSLVariableNode CreateReference()
        {
            var v = VariableNode.Clone();
            _references.Add(v);
            return v;
        }


        /// <summary>
        ///     Creates a global variable declaration node with the given <see cref="LSLType" /> and name.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="variableName" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="variableName" /> contained characters not allowed in an LSL ID
        ///     token.
        /// </exception>
        public static LSLVariableDeclarationNode CreateGlobalVar(LSLType type, string variableName)
        {
            if (variableName == null)
            {
                throw new ArgumentNullException("variableName");
            }

            if (!LSLTokenTools.IDRegex.IsMatch(variableName))
            {
                throw new ArgumentException(
                    "variableName provided contained characters not allowed in an LSL ID token.", "variableName");
            }

            var n = new LSLVariableDeclarationNode();
            n.VariableNode = LSLVariableNode.CreateGlobalVarReference(type, variableName, n);
            n.VariableNode.Parent = n;

            return n;
        }


        /// <summary>
        ///     Creates a local variable declaration node with the given <see cref="LSLType" /> and name.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="variableName" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="variableName" /> contained characters not allowed in an LSL ID
        ///     token.
        /// </exception>
        public static LSLVariableDeclarationNode CreateLocalVar(LSLType type, string variableName)
        {
            if (variableName == null)
            {
                throw new ArgumentNullException("variableName");
            }

            if (!LSLTokenTools.IDRegex.IsMatch(variableName))
            {
                throw new ArgumentException(
                    "variableName provided contained characters not allowed in an LSL ID token.", "variableName");
            }

            var n = new LSLVariableDeclarationNode();
            n.VariableNode = LSLVariableNode.CreateLocalVarReference(type, variableName, n);
            n.VariableNode.Parent = n;

            return n;
        }


        /// <summary>
        ///     Creates a local variable declaration node with the given <see cref="LSLType" />, name, and declaration expression.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="variableName" /> or <paramref name="declarationExpression" />
        ///     is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="variableName" /> contained characters not allowed in an LSL ID token.
        /// </exception>
        public static LSLVariableDeclarationNode CreateLocalVar(LSLType type, string variableName,
            ILSLExprNode declarationExpression)
        {
            if (variableName == null)
            {
                throw new ArgumentNullException("variableName");
            }
            if (declarationExpression == null)
            {
                throw new ArgumentNullException("declarationExpression");
            }

            if (!LSLTokenTools.IDRegex.IsMatch(variableName))
            {
                throw new ArgumentException(
                    "variableName provided contained characters not allowed in an LSL ID token.", "variableName");
            }

            var n = new LSLVariableDeclarationNode();
            n.VariableNode = LSLVariableNode.CreateLocalVarReference(type, variableName, n);
            n.VariableNode.Parent = n;

            n.DeclarationExpression = declarationExpression;

            return n;
        }


        /// <summary>
        ///     Creates a global variable declaration node with the given <see cref="LSLType" />, name, and declaration expression.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="variableName" /> or <paramref name="declarationExpression" />
        ///     is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="variableName" /> contained characters not allowed in an LSL ID token.
        /// </exception>
        public static LSLVariableDeclarationNode CreateGlobalVar(LSLType type, string variableName,
            ILSLExprNode declarationExpression)
        {
            if (variableName == null)
            {
                throw new ArgumentNullException("variableName");
            }
            if (declarationExpression == null)
            {
                throw new ArgumentNullException("declarationExpression");
            }

            if (!LSLTokenTools.IDRegex.IsMatch(variableName))
            {
                throw new ArgumentException(
                    "variableName provided contained characters not allowed in an LSL ID token.", "variableName");
            }

            var n = new LSLVariableDeclarationNode();
            n.VariableNode = LSLVariableNode.CreateGlobalVarReference(type, variableName, n);
            n.VariableNode.Parent = n;

            n.DeclarationExpression = declarationExpression;

            return n;
        }



        /// <summary>
        ///     Construct an <see cref="LSLVariableDeclarationNode" /> that references a library constant.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="constantName" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="type" /> is <see cref="LSLType.Void" /> or
        ///     <paramref name="constantName" /> is contains characters that are invalid in an LSL ID token.
        /// </exception>
        public static LSLVariableDeclarationNode CreateLibraryConstant(LSLType type, string constantName)
        {
            var n = new LSLVariableDeclarationNode
            {
                VariableNode = LSLVariableNode.CreateLibraryConstantReference(type, constantName)
            };

            n.VariableNode.Parent = n;

            return n;
        }


        /// <summary>
        ///     Construct an <see cref="LSLVariableDeclarationNode" /> that represents a parameter.
        /// </summary>
        /// <param name="declarationNode">A parameter node that declares the parameter variable.</param>
        public static LSLVariableDeclarationNode CreateParameter(ILSLParameterNode declarationNode)
        {
            var n = new LSLVariableDeclarationNode
            {
                VariableNode = LSLVariableNode.CreateParameterReference(declarationNode),
                SourceRange = declarationNode.SourceRange,
                SourceRangesAvailable = true
            };

            n.VariableNode.Parent = n;

            return n;
        }



        /// <exception cref="ArgumentNullException"><paramref name="context" /> is <c>null</c>.</exception>
        internal static LSLVariableDeclarationNode CreateVar(LSLParser.GlobalVariableDeclarationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var n = new LSLVariableDeclarationNode();
            n.VariableNode = LSLVariableNode.CreateVarReference(context, n);
            n.SourceRange = new LSLSourceCodeRange(context);
            n.VariableNode.Parent = n;

            n.SourceRangeType = new LSLSourceCodeRange(context.variable_type);
            n.SourceRangeName = new LSLSourceCodeRange(context.variable_name);

            n.SourceRangesAvailable = true;

            if (context.operation != null)
            {
                n.SourceRangeOperator = new LSLSourceCodeRange(context.operation);
            }

            return n;
        }


        /// <exception cref="ArgumentNullException">
        ///     <paramref name="context" /> or <paramref name="declarationExpression" /> is
        ///     <c>null</c>.
        /// </exception>
        internal static LSLVariableDeclarationNode CreateVar(LSLParser.GlobalVariableDeclarationContext context,
            ILSLExprNode declarationExpression)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (declarationExpression == null)
            {
                throw new ArgumentNullException("declarationExpression");
            }


            var n = new LSLVariableDeclarationNode();
            n.VariableNode = LSLVariableNode.CreateVarReference(context, n);
            n.SourceRange = new LSLSourceCodeRange(context);
            n.VariableNode.Parent = n;
            n.DeclarationExpression = declarationExpression;

            n.SourceRangeType = new LSLSourceCodeRange(context.variable_type);
            n.SourceRangeName = new LSLSourceCodeRange(context.variable_name);

            n.SourceRangesAvailable = true;

            declarationExpression.Parent = n;

            if (context.operation != null)
            {
                n.SourceRangeOperator = new LSLSourceCodeRange(context.operation);
            }

            return n;
        }


        /// <exception cref="ArgumentNullException">
        ///     <paramref name="context" /> or <paramref name="declarationExpression" /> is
        ///     <c>null</c>.
        /// </exception>
        internal static LSLVariableDeclarationNode CreateVar(LSLParser.LocalVariableDeclarationContext context,
            ILSLExprNode declarationExpression)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (declarationExpression == null)
            {
                throw new ArgumentNullException("declarationExpression");
            }


            var n = new LSLVariableDeclarationNode();
            n.VariableNode = LSLVariableNode.CreateVarReference(context, n);
            n.SourceRange = new LSLSourceCodeRange(context);
            n.VariableNode.Parent = n;
            n.DeclarationExpression = declarationExpression;

            n.SourceRangeType = new LSLSourceCodeRange(context.variable_type);
            n.SourceRangeName = new LSLSourceCodeRange(context.variable_name);

            n.SourceRangesAvailable = true;

            declarationExpression.Parent = n;

            if (context.operation != null)
            {
                n.SourceRangeOperator = new LSLSourceCodeRange(context.operation);
            }

            return n;
        }


        /// <exception cref="ArgumentNullException"><paramref name="context" /> is <c>null</c>.</exception>
        internal static LSLVariableDeclarationNode CreateVar(LSLParser.LocalVariableDeclarationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var n = new LSLVariableDeclarationNode();
            n.VariableNode = LSLVariableNode.CreateVarReference(context, n);
            n.SourceRange = new LSLSourceCodeRange(context);
            n.VariableNode.Parent = n;

            n.SourceRangeType = new LSLSourceCodeRange(context.variable_type);
            n.SourceRangeName = new LSLSourceCodeRange(context.variable_name);

            n.SourceRangesAvailable = true;

            if (context.operation != null)
            {
                n.SourceRangeOperator = new LSLSourceCodeRange(context.operation);
            }

            return n;
        }


        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLCodeStatement Members

        /// <summary>
        ///     Always <c>false</c> for <see cref="LSLVariableDeclarationNode" />.
        /// </summary>
        /// <seealso cref="ILSLCodeScopeNode.IsSingleStatementScope" />
        /// <exception cref="NotSupportedException" accessor="set">if <c>value</c> is <c>true</c>.</exception>
        public bool InsideSingleStatementScope
        {
            get { return false; }
            set
            {
                if (value)
                {
                    throw new NotSupportedException(GetType().Name + " cannot exist in a single statement scope.");
                }
            }
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
        ///     The index of this statement in its scope
        /// </summary>
        public int StatementIndex { get; set; }


        /// <summary>
        ///     True if the node represents a return path out of its ILSLCodeScopeNode parent, False otherwise.
        /// </summary>
        public bool HasReturnPath
        {
            get { return false; }
        }


        /// <summary>
        ///     Is this statement the last statement in its scope
        /// </summary>
        public bool IsLastStatementInScope { get; set; }


        /// <summary>
        ///     Is this statement dead code
        /// </summary>
        public bool IsDeadCode { get; set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        ///     True if this syntax tree node contains syntax errors.
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
        ///     The source code range of the type specifier for the variable declaration.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeType { get; private set; }


        /// <summary>
        ///     The source code range that encompasses the variables name in the declaration.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeName { get; private set; }


        /// <summary>
        ///     The source code range of the assignment operator in the declaration expression if one was used.
        ///     This value is only meaningful if either 'IsLocal' or 'IsGlobal' are true, and 'HasDeclarationExpression' is also
        ///     true.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeOperator { get; private set; }


        /// <summary>
        ///     Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}" />
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (IsGlobal)
            {
                return visitor.VisitGlobalVariableDeclaration(this);
            }
            return visitor.VisitLocalVariableDeclaration(this);
        }


        /// <summary>
        ///     Deep clones the node.  It should clone the node and all of its children and cloneable properties, except the parent.
        ///     When cloned, the parent node reference should be left <c>null</c>.
        ///     The <see cref="References"/> collection is not cloned.
        /// </summary>
        /// <returns>A deep clone of this statement tree node.</returns>
        public LSLVariableDeclarationNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLVariableDeclarationNode(this);
        }

        #endregion
    }
}