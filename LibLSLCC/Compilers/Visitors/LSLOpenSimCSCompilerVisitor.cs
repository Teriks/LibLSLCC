#region FileInfo
// 
// File: LSLOpenSimCSCompilerVisitor.cs
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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Collections;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.Compilers.Visitors
{
    // ReSharper disable InconsistentNaming
    internal class LSLOpenSimCSCompilerVisitor : LSLValidatorNodeVisitor<bool>
        // ReSharper restore InconsistentNaming
    {

        private const string UtilityLibrary =
            @"
//============================
//== Compiler Utility Class ==
//============================
private static class UTILITIES
{
    public static bool ToBool(LSL_Types.Vector3 vector)
    {
        return vector.x!=0.0&&vector.y!=0.0&&vector.z!=0.0;
    }
    public static bool ToBool(LSL_Types.Quaternion rotation)
    {
        return rotation.x!=0.0&&rotation.y!=0.0&&rotation.z!=0.0&&rotation.s!=0.0;
    }
    public static bool ToBool(LSL_Types.list list)
    {
        return list.Length!=0;
    }
    public static bool ToBool(LSL_Types.LSLString str)
    {
        return str.Length!=0;
    }
    public static LSL_Types.Quaternion Negate(LSL_Types.Quaternion rotation)
    {
        rotation.x=(-rotation.x);
        rotation.y=(-rotation.y);
        rotation.z=(-rotation.z);
        rotation.s=(-rotation.s);
        return rotation;
    }
    public static LSL_Types.Vector3 Negate(LSL_Types.Vector3 vector)
    {
        vector.x=(-vector.x);
        vector.y=(-vector.y);
        vector.z=(-vector.z);
        return vector;
    }
}
";

        private readonly bool _parenthesizeExpressions = true;
        private bool _creatingGlobalsClass;
        private string _currentLSLStateBody;
        private int _indentLevel;
        public LSLOpenSimCSCompilerSettings Settings { get; set; }
        public TextWriter Writer { get; private set; }

        public void WriteAndFlush(ILSLCompilationUnitNode node, TextWriter writer, bool closeStream = true)
        {
            Writer = writer;

            Visit(node);
            Writer.Flush();

            if (closeStream)
            {
                Writer.Close();
            }
        }

        public void Reset()
        {
            _currentLSLStateBody = "";
            _creatingGlobalsClass = false;
            _indentLevel = 0;
            _binOpsUsed.Clear();
        }

        #region Expressions

        #region BasicExpressions

        private readonly HashSet<LSLBinaryOperationSignature> _binOpsUsed = new HashSet<LSLBinaryOperationSignature>();


        public override bool VisitBinaryExpression(ILSLBinaryExpressionNode node)
        {
            var parenths = !(node.Parent is ILSLCodeStatement || node.Parent is ILSLExpressionListNode);

            if (node.Operation == LSLBinaryOperationType.LogicalAnd)
            {
                Writer.Write("(");
                Writer.Write("(bool)(");
                Visit(node.RightExpression);
                Writer.Write("))");

                Writer.Write("&");

                Writer.Write("(");
                Writer.Write("(bool)(");
                Visit(node.LeftExpression);
                Writer.Write("))");

                return false;
            }

            if (node.Operation == LSLBinaryOperationType.LogicalOr)
            {
                Writer.Write("(");
                Writer.Write("(bool)(");
                Visit(node.RightExpression);
                Writer.Write("))");

                Writer.Write("|");

                Writer.Write("(");
                Writer.Write("(bool)(");
                Visit(node.LeftExpression);
                Writer.Write("))");

                return false;
            }

            if (node.Operation == LSLBinaryOperationType.MultiplyAssign)
            {
                if (node.LeftExpression.Type == LSLType.Integer && node.RightExpression.Type == LSLType.Float)
                {
                    if (parenths && _parenthesizeExpressions)
                    {
                        Writer.Write("(");
                    }

                    Visit(node.LeftExpression);
                    Writer.Write("=new LSL_Types.LSLInteger(System.Math.Round((double)");
                    Visit(node.LeftExpression);
                    Writer.Write(".value) * ");
                    Visit(node.RightExpression);
                    Writer.Write(".value)");

                    if (parenths && _parenthesizeExpressions)
                    {
                        Writer.Write(")");
                    }

                    return false;
                }
            }


            if (node.OperationString.EqualsOneOf("=", "*=", "+=", "/=", "%=", "-="))
            {
                Visit(node.LeftExpression);
                Writer.Write(node.OperationString);
                Visit(node.RightExpression);


                return false;
            }


            var operationSignature = new LSLBinaryOperationSignature(node.OperationString, node.Type,
                node.LeftExpression.Type,
                node.RightExpression.Type);


            if (!_binOpsUsed.Contains(operationSignature))
            {
                _binOpsUsed.Add(operationSignature);
            }


            Writer.Write(node.LeftExpression.Type + "_" + node.Operation + "_" + node.RightExpression.Type);
            Writer.Write("(");
            Visit(node.RightExpression);
            Writer.Write(",");
            Visit(node.LeftExpression);
            Writer.Write(")");


            return false;
        }


        public override bool VisitPostfixOperation(ILSLPostfixOperationNode node)
        {
            var parenths = !(node.Parent is ILSLCodeStatement || node.Parent is ILSLExpressionListNode);

            if (parenths && _parenthesizeExpressions)
            {
                Writer.Write("(");
            }

            Visit(node.LeftExpression);
            Writer.Write(node.OperationString);

            if (parenths && _parenthesizeExpressions)
            {
                Writer.Write(")");
            }


            return false;
        }


        public override bool VisitPrefixOperation(ILSLPrefixOperationNode node)
        {
            if (node.RightExpression.Type == LSLType.Rotation ||
                node.RightExpression.Type == LSLType.Vector && node.OperationString == "+")
            {
                Writer.Write("UTILITIES.Negate(");
                Visit(node.RightExpression);

                Writer.Write(")");
            }
            else
            {
                var parenths = !(node.Parent is ILSLCodeStatement || node.Parent is ILSLExpressionListNode);

                if (parenths && _parenthesizeExpressions)
                {
                    Writer.Write("(");
                }

                Writer.Write(node.OperationString);
                Visit(node.RightExpression);


                if (parenths && _parenthesizeExpressions)
                {
                    Writer.Write(")");
                }
            }


            return false;
        }


        public override bool VisitVecRotAccessor(ILSLTupleAccessorNode node)
        {
            Visit(node.AccessedExpression);
            Writer.Write(".");
            Writer.Write(node.AccessedComponentString);

            return false;
        }


        public override bool VisitParenthesizedExpression(ILSLParenthesizedExpressionNode node)
        {
            var parenths = !(node.Parent is ILSLExpressionListNode);

            if (parenths)
            {
                Writer.Write("(");
            }
            Visit(node.InnerExpression);

            if (parenths)
            {
                Writer.Write(")");
            }
            return false;
        }


        public override bool VisitTypecastExpression(ILSLTypecastExprNode node)
        {
            if (node.CastToType == node.CastedExpression.Type)
            {
                Visit(node.CastedExpression);
                return false;
            }

            Writer.Write("(" + LSLAtomType_To_CSharpType(node.CastToType) + ")");

            if (_parenthesizeExpressions)
            {
                Writer.Write("(");
            }

            Visit(node.CastedExpression);

            if (_parenthesizeExpressions)
            {
                Writer.Write(")");
            }
            return false;
        }

        #endregion

        #region FunctionCalls

        public override bool VisitUserFunctionCall(ILSLFunctionCallNode node)
        {
            var functionName = "Func_" + node.Name;

            if (node.ParameterExpressions.Count > 0)
            {
                Writer.Write(functionName + "(");

                VisitUserFunctionCallParameters(node.ParameterListNode);

                Writer.Write(")");
            }
            else
            {
                Writer.Write(functionName + "()");
            }
            return false;
        }


        private readonly Dictionary<LSLType, string> _modInvokeFunctionMap
            = new Dictionary<LSLType, string>
            {
                {LSLType.Void, "modInvokeN"},
                {LSLType.String, "modInvokeS"},
                {LSLType.Integer, "modInvokeI"},
                {LSLType.Float, "modInvokeF"},
                {LSLType.Key, "modInvokeK"},
                {LSLType.List, "modInvokeL"},
                {LSLType.Vector, "modInvokeV"},
                {LSLType.Rotation, "modInvokeR"}
            };

        


        public override bool VisitLibraryFunctionCall(ILSLFunctionCallNode node)
        {
            var libDataNode = Settings.LibraryDataProvider.GetLibraryFunctionSignature(node.Signature);



            if (libDataNode.ModInvoke)
            {
                var modInvokeFunction = "this." + _modInvokeFunctionMap[node.Signature.ReturnType];

                var afterName = node.ParameterExpressions.Count > 0 ? ", " : "";

                Writer.Write(modInvokeFunction + "(\"" + node.Name + "\"" + afterName);


                VisitLibraryFunctionCallParameters(node.ParameterListNode);

                Writer.Write(")");
            }
            else
            {
                var functionName = "this." + node.Name;

                if (node.ParameterExpressions.Count > 0)
                {
                    Writer.Write(functionName + "(");

                    VisitLibraryFunctionCallParameters(node.ParameterListNode);

                    Writer.Write(")");
                }
                else
                {
                    Writer.Write(functionName + "()");
                }
            }
            return false;
        }



        #endregion

        #region VariableReferences

        public override bool VisitGlobalVariableReference(ILSLVariableNode node)
        {
            if (_creatingGlobalsClass)
            {
                Writer.Write("this.Var_" + node.Name);
            }
            else
            {
                Writer.Write("this.Globals.Var_" + node.Name);
            }
            return false;
        }


        public override bool VisitLocalVariableReference(ILSLVariableNode node)
        {
            /*
                See VisitCodeScope to see where variables defined inside of dead code are put.
                If a variable is declared inside of dead code, it is put at the very top of the scope it was 
                defined in and initialized with the default value for its type.
            */
            Writer.Write("Var" + node.Declaration.ScopeId + "_" + node.Name);
            return false;
        }


        public override bool VisitParameterVariableReference(ILSLVariableNode node)
        {
            Writer.Write("Param_" + node.Name);
            return false;
        }



        private string GenerateExpandedListConstant(string constantValueString)
        {
            return "new LSL_Types.list("+string.Join(", ",LSLListParser.ParseListAsEnumerable("["+constantValueString+"]").Select(e =>
            {

                switch (e.Type)
                {
                    case LSLType.String:
                        return ("new LSL_Types.LSLString(\"" + LSLFormatTools.ShowControlCodeEscapes(e.ValueString) +"\")");

                    case LSLType.Key:
                        return ("new LSL_Types.key(\"" + LSLFormatTools.ShowControlCodeEscapes(e.ValueString) + "\")");

                    case LSLType.Vector:
                        return ("new LSL_Types.Vector3(" + e.ValueString + ")");

                    case LSLType.Rotation:
                        return ("new LSL_Types.Quaternion(" + e.ValueString + ")");

                    case LSLType.Integer:
                        return ("new LSL_Types.LSLInteger(" + e.ValueString + ")");

                    case LSLType.Float:
                        return ("new LSL_Types.LSLFloat(" + e.ValueString + ")");

                    case LSLType.List:
                        return ("new LSL_Types.list(" + e.ValueString + ")");
                    default:
                        throw new InvalidOperationException("LSLOpenSimCSCompilerVisitor.GenerateExpandedListConstant encountered a Void list element type.");
                }

            })) + ")";
        }


        public override bool VisitLibraryConstantVariableReference(ILSLVariableNode node)
        {
            var x = Settings.LibraryDataProvider.GetLibraryConstantSignature(node.Name);
            if (x.Expand)
            {
                switch (x.Type)
                {
                    case LSLType.String:
                        Writer.Write("new LSL_Types.LSLString(" + x.ValueStringAsCodeLiteral + ")");
                        break;
                    case LSLType.Key:
                        Writer.Write("new LSL_Types.key("+ x.ValueStringAsCodeLiteral + ")");
                        break;
                    case LSLType.Vector:
                        Writer.Write("new LSL_Types.Vector3(" + x.ValueString + ")");
                        break;
                    case LSLType.Rotation:
                        Writer.Write("new LSL_Types.Quaternion(" + x.ValueString + ")");
                        break;
                    case LSLType.Integer:
                        Writer.Write("new LSL_Types.LSLInteger(" + x.ValueString + ")");
                        break;
                    case LSLType.Float:
                        Writer.Write("new LSL_Types.LSLFloat(" + x.ValueString + ")");
                        break;
                    case LSLType.List:
                        Writer.Write(GenerateExpandedListConstant(x.ValueString));
                        break;
                    default:
                        throw new InvalidOperationException(
                            "LSLOpenSimCSCompilerVisitor.VisitLibraryConstantVariableReference retrieved a library  "
                            +"constant from the library data provider using 'LSLType.Void' as its Type.");
                }
            }
            else
            {
                Writer.Write(node.Name);
            }


            return false;
        }

        #endregion

        #endregion

        #region ExpressionLists

        public override bool VisitExpressionList(ILSLExpressionListNode node)
        {
            if (node.HasExpressionNodes)
            {
                if (node.ExpressionNodes.Count == 1)
                {
                    var expression = node.ExpressionNodes[0];
                    Visit(expression);
                    return false;
                }


                var i = 0;

                for (; i < node.ExpressionNodes.Count - 1; i++)
                {
                    var expression = node.ExpressionNodes[i];
                    Visit(expression);
                    Writer.Write(",");
                }

                var lastExpression = node.ExpressionNodes[i];

                Visit(lastExpression);
            }

            return false;
        }


       

        public override bool VisitLibraryFunctionCallParameters(ILSLExpressionListNode node)
        {
            /* leaving this skeleton function here and using it to visit all library function call parameters.
               In-case I find a reason that code generation needs to be different for library function call parameters in the future. */
               

            VisitExpressionList(node);
            return false;
        }



        public override bool VisitUserFunctionCallParameters(ILSLExpressionListNode node)
        {
            /* leaving this skeleton function here and using it to visit all user defined function call parameters.
               In-case I find a reason that code generation needs to be different for user defined function call parameters in the future. */

            /* previously I thought that list, keys and strings may need to be copied into user defined function calls to prevent them from being
               mutated, but some testing has revealed this is not the case. */

            VisitExpressionList(node);
            return false;
        }



        #endregion

        #region Literals

        public override bool VisitFloatLiteral(ILSLFloatLiteralNode node)
        {
            bool parentIsFunctionCall = false;
            var parentExpressionList = node.Parent as ILSLExpressionListNode;
            ILSLFunctionCallNode parentFunctionCallNode = null;
            if (parentExpressionList != null)
            {
                parentFunctionCallNode = node.Parent.Parent as ILSLFunctionCallNode;
                if (parentFunctionCallNode != null)
                {
                    parentIsFunctionCall = true;
                }
            }

            bool inModInvokeTopLevel = false;

            if (parentFunctionCallNode != null && parentFunctionCallNode.IsLibraryFunctionCall())
            {
                var libDataNode =
                    Settings.LibraryDataProvider.GetLibraryFunctionSignature(parentFunctionCallNode.Signature);

                inModInvokeTopLevel = libDataNode.ModInvoke;
            }

            var box = !parentIsFunctionCall || inModInvokeTopLevel;


            if (node.Parent is ILSLVectorLiteralNode || node.Parent is ILSLRotationLiteralNode)
            {
                box = false;
            }

            if (box)
            {
                Writer.Write("new LSL_Types.LSLFloat(");
            }

            var floatText = node.RawText;

            var match = Regex.Match(floatText, @"^([0-9]+)\.([eE][-+][0-9]+)?[fF]?$");
            if (match.Success)
            {
                //things like 0. or 0.f wont work in CSharp but are allowed in LSL

                Writer.Write(match.Groups[1].Value + ".0" + match.Groups[2]);
            }
            else
            {
                Writer.Write(node.RawText);
            }

            if (box)
            {
                Writer.Write(")");
            }

            return false;
        }


        public override bool VisitIntegerLiteral(ILSLIntegerLiteralNode node)
        {
            bool parentIsFunctionCall = false;
            var parentExpressionList = node.Parent as ILSLExpressionListNode;
            ILSLFunctionCallNode parentFunctionCallNode = null;
            if (parentExpressionList != null)
            {
                parentFunctionCallNode = node.Parent.Parent as ILSLFunctionCallNode;
                if (parentFunctionCallNode != null)
                {
                    parentIsFunctionCall = true;
                }
            }

            bool inModInvokeTopLevel = false;

            if (parentFunctionCallNode != null && parentFunctionCallNode.IsLibraryFunctionCall())
            {
                var libDataNode =
                    Settings.LibraryDataProvider.GetLibraryFunctionSignature(parentFunctionCallNode.Signature);

                inModInvokeTopLevel = libDataNode.ModInvoke;
            }

            var box = !parentIsFunctionCall || inModInvokeTopLevel;


            if (node.Parent is ILSLVectorLiteralNode || node.Parent is ILSLRotationLiteralNode)
            {
                box = false;
            }

            if (box)
            {
                Writer.Write("new LSL_Types.LSLInteger(");
            }

            Writer.Write(node.RawText);

            if (box)
            {
                Writer.Write(")");
            }

            return false;
        }


        public override bool VisitHexLiteral(ILSLHexLiteralNode node)
        {
            Writer.Write("new LSL_Types.LSLInteger(");

            Writer.Write(node.RawText);

            Writer.Write(")");

            return false;
        }


        public override bool VisitListLiteral(ILSLListLiteralNode node)
        {
            if (node.ExpressionListNode.HasExpressionNodes)
            {
                Writer.Write("(new LSL_Types.list(");
                Visit(node.ExpressionListNode);
                Writer.Write("))");
            }
            else
            {
                Writer.Write("(new LSL_Types.list())");
            }

            return false;
        }


        public override bool VisitRotationLiteral(ILSLRotationLiteralNode node)
        {
            Writer.Write("(new LSL_Types.Quaternion(");
            Visit(node.XExpression);
            Writer.Write(", ");
            Visit(node.YExpression);
            Writer.Write(", ");
            Visit(node.ZExpression);
            Writer.Write(", ");
            Visit(node.SExpression);
            Writer.Write("))");

            return false;
        }


        public override bool VisitStringLiteral(ILSLStringLiteralNode node)
        {
            bool parentIsFunctionCall = false;
            var parentExpressionList = node.Parent as ILSLExpressionListNode;
            ILSLFunctionCallNode parentFunctionCallNode = null;
            if (parentExpressionList != null)
            {
                parentFunctionCallNode = node.Parent.Parent as ILSLFunctionCallNode;
                if (parentFunctionCallNode != null)
                {
                    parentIsFunctionCall = true;
                }
            }

            bool inModInvokeTopLevel = false;

            if (parentFunctionCallNode != null && parentFunctionCallNode.IsLibraryFunctionCall())
            {
                var libDataNode =
                    Settings.LibraryDataProvider.GetLibraryFunctionSignature(parentFunctionCallNode.Signature);

                inModInvokeTopLevel = libDataNode.ModInvoke;
            }

            var box = !parentIsFunctionCall || inModInvokeTopLevel;


            if (box)
            {
                Writer.Write("new LSL_Types.LSLString(");
            }

            Writer.Write(node.PreProccessedText);

            if (box)
            {
                Writer.Write(")");
            }

            return false;
        }


        public override bool VisitVectorLiteral(ILSLVectorLiteralNode node)
        {
            Writer.Write("(new LSL_Types.Vector3(");
            Visit(node.XExpression);
            Writer.Write(", ");
            Visit(node.YExpression);
            Writer.Write(", ");
            Visit(node.ZExpression);
            Writer.Write("))");

            return false;
        }

        #endregion

        #region Utilitys



        private static string LSLType_To_CSharpDefaultInitializer(string name)
        {
            var type = LSLTypeTools.FromLSLTypeString(name);

            if (type == LSLType.String || type == LSLType.Key)
            {
                return "\"\"";
            }
            if (type == LSLType.Integer)
            {
                return "0";
            }
            if (type == LSLType.Float)
            {
                return "0.0";
            }
            if (type == LSLType.Rotation)
            {
                return "new " + LSLAtomType_To_CSharpType(type) + "(0,0,0,1)";
            }
            if (type == LSLType.Vector)
            {
                return "new " + LSLAtomType_To_CSharpType(type) + "(0,0,0)";
            }

            return "new " + LSLAtomType_To_CSharpType(type) + "()";
        }


        private static string LSLAtomType_To_CSharpType(LSLType type)
        {
            switch (type)
            {
                case LSLType.Vector:
                    return "LSL_Types.Vector3";
                case LSLType.Rotation:
                    return "LSL_Types.Quaternion";
                case LSLType.List:
                    return "LSL_Types.list";
                case LSLType.Key:
                    return "LSL_Types.LSLString";
                case LSLType.Integer:
                    return "LSL_Types.LSLInteger";
                case LSLType.String:
                    return "LSL_Types.LSLString";
                case LSLType.Float:
                    return "LSL_Types.LSLFloat";
                case LSLType.Void:
                    return "void";
            }

            return "";
        }


        private void CreateGlobalVariablesClass(ILSLCompilationUnitNode node)
        {
            _creatingGlobalsClass = true;

            Writer.WriteLine(GenIndent() + "//===============================");
            Writer.WriteLine(GenIndent() + "//== Global Variable Container ==");
            Writer.WriteLine(GenIndent() + "//===============================");
            Writer.WriteLine(GenIndent() + "private class GLOBALS");
            Writer.WriteLine(GenIndent() + "{");

            _indentLevel++;

            //define public members, without initialization
            foreach (var gvar in node.GlobalVariableDeclarations)
            {
                Writer.WriteLine(GenIndent() + "public " +
                                 LSLAtomType_To_CSharpType(gvar.Type) +
                                 " Var_" +
                                 gvar.Name + ";");
            }

            Writer.WriteLine(GenIndent() + "public GLOBALS()");
            Writer.WriteLine(GenIndent() + "{");

            _indentLevel++;


            //initialize them in the constructor, as LSL allows its globals to reference each other
            //and CSharp does not allow class members to reference each other when being initialized
            //in the top level of the class
            foreach (var gvar in node.GlobalVariableDeclarations)
            {
                Writer.Write(GenIndent() + "Var_" + gvar.Name + " = ");
                if (gvar.HasDeclarationExpression)
                {
                    Visit(gvar.DeclarationExpression);
                    Writer.WriteLine(";");
                }
                else
                {
                    Writer.Write(LSLType_To_CSharpDefaultInitializer(gvar.TypeString));
                    Writer.WriteLine(";");
                }
            }

            _indentLevel--;

            Writer.WriteLine(GenIndent() + "}");

            _indentLevel--;

            Writer.WriteLine(GenIndent() + "}");

            Writer.Write(Environment.NewLine + Environment.NewLine);

            Writer.WriteLine(GenIndent() + "GLOBALS Globals = new GLOBALS();");

            Writer.Write(Environment.NewLine + Environment.NewLine);

            _creatingGlobalsClass = false;
        }


        private string GenIndent(int extra = 0)
        {
            var result = "";
            for (var i = 0; i < _indentLevel + extra; i++)
            {
                result += "\t";
            }
            return result;
        }


        private void WriteBooleanConditionContent(LSLType expressionType, ILSLReadOnlyExprNode conditionExpression)
        {
            if (expressionType == LSLType.Key)
            {
                Writer.Write("new LSL_Types.key(");
                Visit(conditionExpression);
                Writer.Write(")");
            }
            else if (
                expressionType == LSLType.Vector ||
                expressionType == LSLType.Rotation ||
                expressionType == LSLType.List ||
                expressionType == LSLType.String)
            {
                //have to use our small utility class to convert these to bool
                //as they would be in Linden Labs LSL
                Writer.Write("UTILITIES.ToBool(");
                Visit(conditionExpression);
                Writer.Write(")");
            }
            else
            {
                Visit(conditionExpression);
            }
        }


        /// <summary>
        ///     need a small utility library compiled in the generated csharp code
        ///     because OpenSim lacks several lsl features like using vectors, rotations, and list in condition statements
        /// </summary>
        private void WriteUtilityLibrary()
        {
            WriteMultiLineIndentedString(UtilityLibrary);
        }


        private void WriteMultiLineIndentedString(string str)
        {
            var splitStr = str.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

            foreach (var line in splitStr)
            {
                Writer.WriteLine(GenIndent() + line);
            }
        }

        #endregion

        #region LoopConstructs

        public override bool VisitDoLoop(ILSLDoLoopNode node)
        {
            if (node.IsDeadCode) return false;

            Writer.WriteLine(GenIndent() + "do");

            Visit(node.Code);

            Writer.Write(Environment.NewLine + GenIndent() + "while(");


            WriteBooleanConditionContent(node.ConditionExpression.Type, node.ConditionExpression);

            Writer.Write(");");

            Writer.WriteLine();

            return false;
        }


        public override bool VisitForLoop(ILSLForLoopNode node)
        {
            if (node.IsDeadCode) return false;

            Writer.Write(GenIndent() + "for(");

            if (node.HasInitExpressions)
            {
                Visit(node.InitExpressionsList);
            }

            Writer.Write(";");

            if (node.HasConditionExpression)
            {
                WriteBooleanConditionContent(node.ConditionExpression.Type, node.ConditionExpression);
            }

            Writer.Write(";");


            if (node.HasAfterthoughtExpressions)
            {
                Visit(node.AfterthoughExpressions);
            }

            Writer.WriteLine(")");

            Visit(node.Code);

            return false;
        }


        public override bool VisitWhileLoop(ILSLWhileLoopNode node)
        {
            if (node.IsDeadCode) return false;

            Writer.Write(GenIndent() + "while(");

            WriteBooleanConditionContent(node.ConditionExpression.Type, node.ConditionExpression);

            Writer.WriteLine(")");


            Visit(node.Code);

            Writer.WriteLine();


            return false;
        }

        #endregion

        #region ScopesAndDeclarations

        public override bool VisitCodeScope(ILSLCodeScopeNode node)
        {
            if (node.IsDeadCode) return false;

            Writer.WriteLine(GenIndent() + "{");

            _indentLevel++;

            if (Settings.InsertCoOpTerminationCalls)
            {
                //if the co-op termination setting is enabled, insert call stubs
                //at the top of user defined functions, event handlers, and loop constructs
                switch (node.CodeScopeType)
                {
                    case LSLCodeScopeType.EventHandler:
                    case LSLCodeScopeType.Function:
                    case LSLCodeScopeType.DoLoop:
                    case LSLCodeScopeType.WhileLoop:
                    case LSLCodeScopeType.ForLoop:

                        Writer.WriteLine(GenIndent() + Settings.CoOpTerminationFunctionCall + ";");
                        break;
                }
            }


            /*
                Write all variable declarations that are considered to be dead due to a jump over code in this scope
                to the top of the scope and initialize them with the default value for their type.

                Dead variable declarations inside of a constant jump over code can still be referenced within the same scope,
                we need to make sure they are initialized or the CSharp compiler will complain when they are referenced.

                They can be put at the top of the scope because the code validator has guaranteed they are only referenced
                from places after their original definition point.

                When a dead variable declaration node is encountered later in the syntax tree, it is discarded.  Because if it
                needed to be defined to produce valid generated code it was already defined here at the top of the scope with a default value.
            */
            var deadVariableDeclarationNodes =
                node.CodeStatements.Where(x => x.IsDeadCode && x.DeadCodeType == LSLDeadCodeType.JumpOverCode && x is ILSLVariableDeclarationNode)
                    .Cast<ILSLVariableDeclarationNode>();



            foreach (var deadVariableDeclarationNode in deadVariableDeclarationNodes)
            {
                var variableName = "Var" + node.ScopeId + "_" + deadVariableDeclarationNode.Name;

                Writer.Write(GenIndent());
                Writer.Write(LSLAtomType_To_CSharpType(deadVariableDeclarationNode.Type));
                Writer.Write(" ");
                Writer.Write(variableName);
                Writer.Write(" = ");
                Writer.Write(LSLType_To_CSharpDefaultInitializer(deadVariableDeclarationNode.TypeString));
                Writer.WriteLine(";");
           }


          
            foreach (var statement in node.CodeStatements)
            {
                Visit(statement);
                Writer.WriteLine();
            }

            _indentLevel--;

            Writer.Write(GenIndent() + "}");

            return false;
        }


        private void WriteBinaryOperatorOverloadStubs()
        {
            //re-write overloads for used binary operations, they take parameters in reverse
            //order and execute the operation in normal order to simulate lsl's weird ass right to 
            //left evaluation, _binOpsUsed is added to whenever a binary expression is encountered


            var binopCount = _binOpsUsed.Count;
            var binopCounter = 0;


            if (binopCount > 0)
            {
                Writer.WriteLine();
                Writer.WriteLine(GenIndent() + "//===========================");
                Writer.WriteLine(GenIndent() + "//== Binary Operator Stubs ==");
                Writer.WriteLine(GenIndent() + "//===========================");
                Writer.Write(Environment.NewLine + Environment.NewLine);
            }


            foreach (var binOp in _binOpsUsed)
            {
                Writer.WriteLine(GenIndent() + "private " +
                                 LSLAtomType_To_CSharpType(binOp.Returns) + " " + binOp.Left + "_" + binOp.Operation +
                                 "_" + binOp.Right + "(" +
                                 LSLAtomType_To_CSharpType(binOp.Right) + " right, " +
                                 LSLAtomType_To_CSharpType(binOp.Left) + " left)");

                Writer.WriteLine(GenIndent() + "{");
                _indentLevel++;

                Writer.WriteLine(GenIndent() + "return left" + binOp.Operation.ToOperatorString() + "right;");

                _indentLevel--;
                Writer.WriteLine(GenIndent() + "}");

                if (binopCounter != (binopCount - 1))
                {
                    Writer.Write(Environment.NewLine + Environment.NewLine);
                }

                binopCounter++;
            }

            _binOpsUsed.Clear();
        }


        public override bool VisitCompilationUnit(ILSLCompilationUnitNode unode)
        {
            if (Settings.GenerateClass)
            {
                foreach (var ns in Settings.GeneratedNamespaceImports)
                {
                    Writer.WriteLine("using "+ns+";");
                    Writer.Write(Environment.NewLine);
                }

                Writer.Write(Environment.NewLine);

                if (!string.IsNullOrWhiteSpace(Settings.GenerateClassNamespace))
                {
                    Writer.WriteLine("namespace {0}", Settings.GenerateClassNamespace);
                    Writer.WriteLine("{");
                    _indentLevel++;
                }

                if (!string.IsNullOrWhiteSpace(Settings.GeneratedClassInherit))
                {
                    Writer.WriteLine(GenIndent() + "class {0} : {1}", Settings.GeneratedClassName,
                        Settings.GeneratedClassInherit);
                }
                else
                {
                    Writer.WriteLine(GenIndent() + "class {0}", Settings.GeneratedClassName);
                }

                Writer.WriteLine(GenIndent() + "{");

                Writer.Write(Environment.NewLine);

                _indentLevel++;

                WriteMultiLineIndentedString(Settings.GeneratedConstructorDefinition);
            }
            else
            {
                Writer.WriteLine(GenIndent() + "//C#");
                Writer.WriteLine(GenIndent() + "//OpenSim CSharp code, CSharp scripting must be enabled on the server to run.");
                Writer.WriteLine(GenIndent() + "//Do not remove the first comment.");
                Writer.WriteLine(GenIndent() + "//Compiled by LibLSLCC, Date: {0}", DateTime.Now);


                Writer.Write(Environment.NewLine);
            }


            WriteUtilityLibrary();


            Writer.Write(Environment.NewLine);


            if (unode.GlobalVariableDeclarations.Count > 0)
            {
                CreateGlobalVariablesClass(unode);
            }

            if (unode.FunctionDeclarations.Count > 0)
            {
                Writer.WriteLine(GenIndent() + "//============================");
                Writer.WriteLine(GenIndent() + "//== User Defined Functions ==");
                Writer.WriteLine(GenIndent() + "//============================");
                Writer.Write(Environment.NewLine + Environment.NewLine);
            }


            foreach (var ctx in unode.FunctionDeclarations)
            {
                VisitFunctionDeclaration(ctx);
                Writer.Write(Environment.NewLine + Environment.NewLine);
            }


            if (unode.StateDeclarations.Count > 0)
            {
                Writer.WriteLine();
                Writer.WriteLine(GenIndent() + "//=======================================");
                Writer.WriteLine(GenIndent() + "//== User Defined State Event Handlers ==");
                Writer.WriteLine(GenIndent() + "//=======================================");
                Writer.Write(Environment.NewLine + Environment.NewLine);
            }

            foreach (var ctx in unode.StateDeclarations)
            {
                VisitDefinedState(ctx);
                Writer.Write(Environment.NewLine + Environment.NewLine);
            }

            Writer.WriteLine();
            Writer.WriteLine(GenIndent() + "//==================================");
            Writer.WriteLine(GenIndent() + "//== Default State Event Handlers ==");
            Writer.WriteLine(GenIndent() + "//==================================");
            Writer.Write(Environment.NewLine + Environment.NewLine);


            VisitDefaultState(unode.DefaultState);


            Writer.Write(Environment.NewLine + Environment.NewLine);


            WriteBinaryOperatorOverloadStubs();


            if (Settings.GenerateClass)
            {
                if (!string.IsNullOrWhiteSpace(Settings.GenerateClassNamespace))
                {
                    _indentLevel--;

                    Writer.WriteLine(GenIndent() + "}");
                }


                Writer.WriteLine("}");
            }


            return false;
        }


        public override bool VisitEventHandler(ILSLEventHandlerNode node)
        {
            Writer.Write(GenIndent());

            var handlerName = _currentLSLStateBody + "_event_" + node.Name;


            if (node.HasParameterNodes)
            {
                Writer.Write("public void " + handlerName + "(");
                Visit(node.ParameterListNode);
                Writer.WriteLine(")");
            }
            else
            {
                Writer.WriteLine("public void " + handlerName + "()");
            }

            Visit(node.EventBodyNode);

            return false;
        }


        /// <summary>
        ///     Default implementation calls Visit(node.ParameterListNode) then Visit(node.FunctionBodyNode)
        ///     and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the function declaration</param>
        /// <returns>default(T)</returns>
        public override bool VisitFunctionDeclaration(ILSLFunctionDeclarationNode node)
        {
            Writer.Write(GenIndent() + "public ");

            if (node.ReturnType != LSLType.Void)
            {
                Writer.Write(LSLAtomType_To_CSharpType(node.ReturnType) + " ");
            }
            else
            {
                Writer.Write("void ");
            }

            var functionName = "Func_" + node.Name;


            Writer.Write(functionName + "(");

            if (node.HasParameters)
            {
                Visit(node.ParameterListNode);
            }

            Writer.WriteLine(")");

            Visit(node.FunctionBodyNode);

            return false;
        }


        public override bool VisitDefaultState(ILSLStateScopeNode node)
        {
            _currentLSLStateBody = "default";

            var eventHandlers = node.EventHandlers;
            var i = 0;
            for (; i < eventHandlers.Count - 1; i++)
            {
                Visit(eventHandlers[i]);
                Writer.WriteLine();
            }

            Visit(eventHandlers[i]);

            return false;
        }


        public override bool VisitDefinedState(ILSLStateScopeNode node)
        {
            _currentLSLStateBody = node.StateName;

            var eventHandlers = node.EventHandlers;
            var i = 0;
            for (; i < eventHandlers.Count - 1; i++)
            {
                Visit(eventHandlers[i]);
                Writer.WriteLine();
            }

            Visit(eventHandlers[i]);

            return false;
        }


        public override bool VisitParameterDefinition(ILSLParameterNode node)
        {
            Writer.Write(LSLAtomType_To_CSharpType(node.Type) + " Param_" + node.Name);
            return false;
        }


        public override bool VisitParameterDefinitionList(ILSLParameterListNode node)
        {
            if (node.Parameters.Count > 1)
            {
                var i = 0;
                for (; i < node.Parameters.Count - 1; i++)
                {
                    Visit(node.Parameters[i]);
                    Writer.Write(", ");
                }

                Visit(node.Parameters[i]);
            }
            else if (node.Parameters.Count == 1)
            {
                Visit(node.Parameters[0]);
            }


            return false;
        }

        #endregion

        #region CodeStatements

        public override bool VisitReturnStatement(ILSLReturnStatementNode node)
        {
            if (node.IsDeadCode) return false;

            Writer.Write(GenIndent() + "return");

            if (node.HasReturnExpression)
            {
                Writer.Write(" ");
                Visit(node.ReturnExpression);
            }

            Writer.Write(";");

            return false;
        }


        public override bool VisitSemiColonStatement(ILSLSemiColonStatement node)
        {
            //stand alone semi colons are not necessary as we transform all single block
            //statements into code scopes with { } around them

            //Writer.Write(GenIndent() + ";");

            return false;
        }


        public override bool VisitStateChangeStatement(ILSLStateChangeStatementNode node)
        {
            if (node.IsDeadCode) return false;

            Writer.Write(GenIndent() + "this.state(\"" + node.StateTargetName + "\");");
            return false;
        }


        public override bool VisitJumpStatement(ILSLJumpStatementNode node)
        {
            if (node.IsDeadCode) return false;

            Writer.Write(GenIndent() + "goto " + "LSLLabel_" + node.LabelName + ";");
            return false;
        }


        public override bool VisitLabelStatement(ILSLLabelStatementNode node)
        {
            //Labels should not be omitted if they are dead code, they can be referenced still and if the definition is missing that a problem.
            //if (node.IsDeadCode) return false;

            if (Settings.InsertCoOpTerminationCalls)
            {
                Writer.WriteLine(GenIndent() + "LSLLabel_" + node.LabelName + ":" + Settings.CoOpTerminationFunctionCall + ";");
            }
            else
            {
                Writer.Write(GenIndent() + "LSLLabel_" + node.LabelName + ":" +
                             (node.IsLastStatementInScope ? ";" : ""));
            }

            return false;
        }


        public override bool VisitExpressionStatement(ILSLExpressionStatementNode node)
        {
            if (node.IsDeadCode) return false;

            if (node.HasEffect)
            {
                Writer.Write(GenIndent());
                Visit(node.Expression);
                Writer.Write(";");
            }
            return false;
        }


        public override bool VisitLocalVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            /* 
               See VisitCodeScope to see where declarations of variables that exist inside dead code
               are put in the generated code.  They are put at the top of the scope they are declared in,
               and initialize with their default values.  They are written out in the VisitCodeScope method
               before we even get to this point in the syntax tree, so they do not need to be written again.
            */
            
            if (node.IsDeadCode) return false;


            Writer.Write(GenIndent());


            var variableName = "Var" + node.ScopeId + "_" + node.Name;


            if (!node.HasDeclarationExpression)
            {
                Writer.Write(LSLAtomType_To_CSharpType(node.Type));
                Writer.Write(" ");
                Writer.Write(variableName);
                Writer.Write(" = ");
                Writer.Write(LSLType_To_CSharpDefaultInitializer(node.TypeString));
                Writer.Write(";");
            }
            else
            {
                Writer.Write(LSLAtomType_To_CSharpType(node.Type));
                Writer.Write(" ");
                Writer.Write(variableName);
                Writer.Write(" = ");
                Visit(node.DeclarationExpression);
                Writer.Write(";");
            }

            return false;
        }

        #endregion

        #region BranchStatements

        public override bool VisitControlStatement(ILSLControlStatementNode node)
        {
            if (node.IsDeadCode) return false;

            return base.VisitControlStatement(node);
        }


        public override bool VisitIfStatement(ILSLIfStatementNode node)
        {
            Writer.Write(GenIndent() + "if(");

            WriteBooleanConditionContent(node.ConditionExpression.Type, node.ConditionExpression);

            Writer.WriteLine(")");

            Visit(node.Code);


            return false;
        }


        public override bool VisitElseIfStatement(ILSLElseIfStatementNode node)
        {
            Writer.Write(Environment.NewLine + GenIndent() + "else if(");

            WriteBooleanConditionContent(node.ConditionExpression.Type, node.ConditionExpression);

            Writer.WriteLine(")");


            Visit(node.Code);

            return false;
        }


        public override bool VisitElseStatement(ILSLElseStatementNode node)
        {
            Writer.WriteLine(Environment.NewLine + GenIndent() + "else");
            Visit(node.Code);
            return false;
        }

        #endregion
    }
}