using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLVariableNode : ILSLVariableNode, ILSLExprNode
    {
        private string _libraryConstantName = "";


// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLVariableNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        private LSLVariableNode()
        {
            IsConstant = false;
        }



        internal LSLParser.LocalVariableDeclarationContext LocalDeclarationContext { get; private set; }

        internal LSLParser.GlobalVariableDeclarationContext GlobalDeclarationContext { get; private set; }

        internal LSLParser.ParameterDefinitionContext ParameterDeclarationContext { get; private set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        public bool IsLibraryConstant
        {
            get { return ExpressionType == LSLExpressionType.LibraryConstant; }
        }


        public bool IsGlobal
        {
            get { return ExpressionType == LSLExpressionType.GlobalVariable; }
        }

        public bool IsLocal
        {
            get { return ExpressionType == LSLExpressionType.LocalVariable; }
        }

        public ILSLVariableDeclarationNode Declaration { get; private set; }




        public bool IsParameter
        {
            get { return ExpressionType == LSLExpressionType.ParameterVariable; }
        }


        public string TypeString
        {
            get
            {
                if (IsGlobal)
                {
                    return GlobalDeclarationContext.variable_type.Text;
                }
                if (IsLocal)
                {
                    return LocalDeclarationContext.variable_type.Text;
                }
                if (IsParameter)
                {
                    return ParameterDeclarationContext.TYPE().GetText();
                }
                if (IsLibraryConstant)
                {
                    return LSLTypeTools.ToLSLTypeString(Type);
                }


                throw new InvalidOperationException("Object in invalid state");
            }
        }




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion




        #region ILSLExprNode Members


        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var n = new LSLVariableNode
            {
                GlobalDeclarationContext = GlobalDeclarationContext,
                LocalDeclarationContext = LocalDeclarationContext,
                ParameterDeclarationContext = ParameterDeclarationContext,
                IsConstant = IsConstant,
                Type = Type,
                ExpressionType = ExpressionType,
                HasErrors = HasErrors,
                Parent = Parent,
                _libraryConstantName = _libraryConstantName,
                Declaration = Declaration
            };
            return n;
        }



        public ILSLSyntaxTreeNode Parent { get; set; }


        public string Name
        {
            get
            {
                if (IsGlobal)
                {
                    return GlobalDeclarationContext.variable_name.Text;
                }

                if (IsLocal)
                {
                    return LocalDeclarationContext.variable_name.Text;
                }

                if (IsParameter)
                {
                    return ParameterDeclarationContext.ID().GetText();
                }

                if (IsLibraryConstant)
                {
                    return _libraryConstantName;
                }

                throw new InvalidOperationException("Object in invalid state");
            }
        }



        public bool HasErrors { get; set; }


        public LSLSourceCodeRange SourceCodeRange { get; set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (IsGlobal)
            {
                return visitor.VisitGlobalVariableReference(this);
            }
            if (IsLocal)
            {
                return visitor.VisitLocalVariableReference(this);
            }
            if (IsParameter)
            {
                return visitor.VisitParameterVariableReference(this);
            }
            if (IsLibraryConstant)
            {
                return visitor.VisitLibraryConstantVariableReference(this);
            }

            throw new InvalidOperationException("LSLVariableNode could not be visited, its state is invalid");
        }



        public LSLExpressionType ExpressionType { get; private set; }


        public bool IsConstant { get; set; }



        public string DescribeType()
        {
            return "(" + Type + (this.IsLiteral() ? " Literal)" : ")");
        }



        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }



        public LSLType Type { get; private set; }


        #endregion




        public static
            LSLVariableNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLVariableNode(sourceRange, Err.Err);
        }



        internal static LSLVariableNode CreateVar(LSLParser.GlobalVariableDeclarationContext context,ILSLVariableDeclarationNode declaration)
        {
            var n = new LSLVariableNode
            {
                Type = LSLTypeTools.FromLSLTypeString(context.variable_type.Text),
                ExpressionType = LSLExpressionType.GlobalVariable,
                IsConstant = false,
                GlobalDeclarationContext = context,
                SourceCodeRange = new LSLSourceCodeRange(context),
                Declaration = declaration
            };

            return n;
        }




        internal static LSLVariableNode CreateVar(LSLParser.LocalVariableDeclarationContext context, ILSLVariableDeclarationNode declaration)
        {
            var n = new LSLVariableNode
            {
                Type = LSLTypeTools.FromLSLTypeString(context.variable_type.Text),
                ExpressionType = LSLExpressionType.LocalVariable,
                IsConstant = false,
                LocalDeclarationContext = context,
                SourceCodeRange = new LSLSourceCodeRange(context),
                Declaration = declaration
            };

            return n;
        }



        internal static LSLVariableNode CreateLibraryConstant(LSLType type, string name)
        {
            var n = new LSLVariableNode
            {
                Type = type,
                ExpressionType = LSLExpressionType.LibraryConstant,
                IsConstant = true,
                _libraryConstantName = name,
                SourceCodeRange = new LSLSourceCodeRange()
            };

            return n;
        }



        internal static LSLVariableNode CreateParameter(LSLParser.ParameterDefinitionContext context)
        {
            var n = new LSLVariableNode
            {
                Type = LSLTypeTools.FromLSLTypeString(context.TYPE().GetText()),
                ExpressionType = LSLExpressionType.ParameterVariable,
                IsConstant = false,
                ParameterDeclarationContext = context,
                SourceCodeRange = new LSLSourceCodeRange(context)
            };

            return n;
        }
    }
}