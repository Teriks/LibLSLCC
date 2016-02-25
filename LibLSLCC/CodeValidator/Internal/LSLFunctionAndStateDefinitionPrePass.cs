#region FileInfo

// 
// File: LSLFunctionAndStateDefinitionPrePass.cs
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

using System.Collections.Generic;
using System.Linq;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator
{
    internal sealed class LSLFunctionAndStateDefinitionPrePass : LSLBaseVisitor<bool>, ILSLTreePreePass
    {
        private readonly LSLCodeValidatorVisitorScopeTracker _scopingManager;
        private readonly ILSLCodeValidatorStrategies _validatorStrategies;


        public LSLFunctionAndStateDefinitionPrePass(LSLCodeValidatorVisitorScopeTracker scopingManager,
            ILSLCodeValidatorStrategies validatorStrategies)
        {
            _scopingManager = scopingManager;
            _validatorStrategies = validatorStrategies;
        }


        /// <summary>
        ///     Gets the syntax warning listener.  this property should NOT be used to generate warning events,
        ///     use <see cref="GenSyntaxWarning" /> for that instead.
        /// </summary>
        /// <value>
        ///     The syntax warning listener.
        /// </value>
        private ILSLSyntaxWarningListener SyntaxWarningListener
        {
            get { return _validatorStrategies.SyntaxWarningListener; }
        }

        /// <summary>
        ///     Gets the syntax error listener.  this property should NOT be used to generate error events,
        ///     use <see cref="GenSyntaxError" /> for that instead.
        /// </summary>
        /// <value>
        ///     The syntax error listener.
        /// </value>
        private ILSLSyntaxErrorListener SyntaxErrorListener
        {
            get { return _validatorStrategies.SyntaxErrorListener; }
        }

        public bool HasSyntaxErrors { get; private set; }
        public bool HasSyntaxWarnings { get; private set; }


        /// <summary>
        ///     Returns a reference to <see cref="SyntaxWarningListener" /> and sets <see cref="HasSyntaxWarnings" /> to
        ///     <c>true</c>.
        /// </summary>
        /// <returns>
        ///     <see cref="SyntaxWarningListener" />
        /// </returns>
        private ILSLSyntaxWarningListener GenSyntaxWarning()
        {
            HasSyntaxWarnings = true;
            return SyntaxWarningListener;
        }


        /// <summary>
        ///     Returns a reference to <see cref="SyntaxErrorListener" /> and sets <see cref="HasSyntaxErrors" /> to <c>true</c>.
        /// </summary>
        /// <returns>
        ///     <see cref="SyntaxErrorListener" />
        /// </returns>
        private ILSLSyntaxErrorListener GenSyntaxError()
        {
            HasSyntaxErrors = true;
            return SyntaxErrorListener;
        }


        public override bool VisitCompilationUnit(LSLParser.CompilationUnitContext context)
        {
            if (context == null)
            {
                throw LSLCodeValidatorInternalException.VisitContextInvalidState("VisitCompilationUnit", true);
            }


            var defaultStateRule = context.defaultState();

            if (defaultStateRule == null)
            {
                GenSyntaxError().MissingDefaultState();
            }
            else
            {
                Visit(defaultStateRule);
            }

            foreach (var func in context.functionDeclaration())
            {
                Visit(func);
            }

            foreach (var state in context.definedState())
            {
                Visit(state);
            }

            return true;
        }


        public override bool VisitDefaultState(LSLParser.DefaultStateContext context)
        {
            if (context == null || context.children == null)
            {
                throw LSLCodeValidatorInternalException.VisitContextInvalidState("VisitDefaultState", true);
            }


            _scopingManager.PreDefineState("default");


            if (!context.eventHandler().Any())
            {
                GenSyntaxError().StateHasNoEventHandlers(new LSLSourceCodeRange(context.state_name), "default");
                return false;
            }


            var eventHandlerNames = new HashSet<string>();

            foreach (var ctx in context.eventHandler())
            {
                if (ctx.handler_name == null)
                {
                    throw LSLCodeValidatorInternalException
                        .VisitContextInInvalidState("VisitDefaultState", typeof (LSLParser.EventHandlerContext), true);
                }

                if (eventHandlerNames.Contains(ctx.handler_name.Text))
                {
                    GenSyntaxError().RedefinedEventHandler(
                        new LSLSourceCodeRange(ctx.handler_name),
                        ctx.handler_name.Text,
                        context.state_name.Text);

                    return false;
                }
                eventHandlerNames.Add(ctx.handler_name.Text);
            }


            return true;
        }


        public override bool VisitDefinedState(LSLParser.DefinedStateContext context)
        {
            if (context == null || Utility.AnyNull(context.children, context.state_name))
            {
                throw LSLCodeValidatorInternalException.VisitContextInvalidState("VisitDefinedState", true);
            }


            if (context.state_name.Text == "default")
            {
                GenSyntaxError().RedefinedDefaultState(
                    new LSLSourceCodeRange(context.state_name));

                return false;
            }


            if (_scopingManager.StatePreDefined(context.state_name.Text))
            {
                GenSyntaxError().RedefinedStateName(
                    new LSLSourceCodeRange(context.state_name),
                    context.state_name.Text);

                return false;
            }


            _scopingManager.PreDefineState(context.state_name.Text);


            if (!context.eventHandler().Any())
            {
                GenSyntaxError().StateHasNoEventHandlers(
                    new LSLSourceCodeRange(context.state_name),
                    context.state_name.Text);

                return false;
            }


            var eventHandlerNames = new HashSet<string>();

            foreach (var ctx in context.eventHandler())
            {
                if (ctx.handler_name == null)
                {
                    throw LSLCodeValidatorInternalException
                        .VisitContextInInvalidState("VisitDefinedState",
                            typeof (LSLParser.EventHandlerContext), true);
                }

                if (eventHandlerNames.Contains(ctx.handler_name.Text))
                {
                    GenSyntaxError().RedefinedEventHandler(
                        new LSLSourceCodeRange(ctx.handler_name),
                        ctx.handler_name.Text,
                        context.state_name.Text);


                    return false;
                }
                eventHandlerNames.Add(ctx.handler_name.Text);
            }


            return true;
        }


        public override bool VisitFunctionDeclaration(LSLParser.FunctionDeclarationContext context)
        {
            if (context == null || Utility.AnyNull(context.function_name, context.code))
            {
                throw LSLCodeValidatorInternalException.VisitContextInvalidState("VisitFunctionDeclaration", true);
            }

            if (_validatorStrategies.LibraryDataProvider.LibraryFunctionExist(context.function_name.Text))
            {
                GenSyntaxError().RedefinedStandardLibraryFunction(
                    new LSLSourceCodeRange(context.function_name), context.function_name.Text,
                    _validatorStrategies.LibraryDataProvider.GetLibraryFunctionSignatures(
                        context.function_name.Text));

                return false;
            }


            if (_scopingManager.FunctionIsPreDefined(context.function_name.Text))
            {
                GenSyntaxError().RedefinedFunction(
                    new LSLSourceCodeRange(context.function_name),
                    _scopingManager.ResolveFunctionPreDefine(context.function_name.Text));

                return false;
            }


            var parameterListNode = LSLParameterListNode.BuildFromParserContext(
                context.parameters,
                LSLParameterListType.FunctionParameters,
                _validatorStrategies);


            if (parameterListNode.HasErrors)
            {
                return false;
            }

            var returnType = LSLType.Void;

            if (context.return_type != null)
            {
                returnType =
                    LSLTypeTools.FromLSLTypeName(context.return_type.Text);
            }


            var func = new LSLPreDefinedFunctionSignature(returnType,
                context.function_name.Text,
                parameterListNode);

            _scopingManager.PreDefineFunction(func);


            base.VisitFunctionDeclaration(context);

            return true;
        }
    }
}