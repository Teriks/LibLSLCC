#region FileInfo

// 
// File: LSLFunctionDeclarationNode.cs
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
using System.Linq;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes
{
    public class LSLFunctionDeclarationNode : ILSLFunctionDeclarationNode, ILSLSyntaxTreeNode
    {
        private readonly List<LSLFunctionCallNode> _references = new List<LSLFunctionCallNode>();
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLFunctionDeclarationNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLFunctionDeclarationNode(LSLParser.FunctionDeclarationContext context,
            LSLParameterListNode parameterListNode, LSLCodeScopeNode functionBodyNode)
        {
            if (parameterListNode == null)
            {
                throw new ArgumentNullException("parameterListNode");
            }

            if (functionBodyNode == null)
            {
                throw new ArgumentNullException("functionBodyNode");
            }

            ParserContext = context;
            ParameterListNode = parameterListNode;
            FunctionBodyNode = functionBodyNode;


            FunctionBodyNode.Parent = this;

            ParameterListNode.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }

        internal LSLParser.FunctionDeclarationContext ParserContext { get; }

        public IReadOnlyList<LSLParameterNode> ParameterNodes
        {
            get { return ParameterListNode.Parameters; }
        }

        public IReadOnlyList<LSLFunctionCallNode> References
        {
            get { return _references; }
        }

        public LSLParameterListNode ParameterListNode { get; set; }
        public LSLCodeScopeNode FunctionBodyNode { get; }

        IReadOnlyList<ILSLFunctionCallNode> ILSLFunctionDeclarationNode.References
        {
            get { return _references; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public bool HasParameters
        {
            get { return ParameterListNode.Parameters.Any(); }
        }

        IReadOnlyList<ILSLParameterNode> ILSLFunctionDeclarationNode.ParameterNodes
        {
            get { return ParameterListNode.Parameters; }
        }

        public string ReturnTypeString
        {
            get
            {
                if (ParserContext.return_type == null) return "";

                return ParserContext.return_type.Text;
            }
        }

        public string Name
        {
            get { return ParserContext.function_name.Text; }
        }

        public LSLType ReturnType
        {
            get
            {
                if (ParserContext.return_type == null) return LSLType.Void;

                return LSLTypeTools.FromLSLTypeString(ReturnTypeString);
            }
        }

        ILSLParameterListNode ILSLFunctionDeclarationNode.ParameterListNode
        {
            get { return ParameterListNode; }
        }

        ILSLCodeScopeNode ILSLFunctionDeclarationNode.FunctionBodyNode
        {
            get { return FunctionBodyNode; }
        }

        internal void AddReference(LSLFunctionCallNode reference)
        {
            _references.Add(reference);
        }

        public static
            LSLFunctionDeclarationNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLFunctionDeclarationNode(sourceRange, Err.Err);
        }

        public LSLFunctionSignature ToSignature()
        {
            return new LSLFunctionSignature(ReturnType, Name,
                ParameterListNode.Parameters.Select(x => new LSLParameter(x.Type, x.Name, false)));
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitFunctionDeclaration(this);
        }


        public ILSLSyntaxTreeNode Parent { get; set; }

        #endregion
    }
}