using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Parser;

namespace LibLSLCC.CodeValidator.Visitor
{
    internal class LSLFunctionAndStateDefinitionPrePass : LSLBaseVisitor<bool>, ILSLTreePreePass
    {
        private readonly LSLVisitorScopeTracker _scopingManager;
        private readonly ILSLValidatorServiceProvider _validatorServiceProvider;

        public LSLFunctionAndStateDefinitionPrePass(LSLVisitorScopeTracker scopingManager,
            ILSLValidatorServiceProvider validatorServiceProvider)
        {
            _scopingManager = scopingManager;
            _validatorServiceProvider = validatorServiceProvider;
        }

        public bool HasSyntaxErrors { get; private set; }

        public bool HasSyntaxWarnings { get; private set; }


        /// <summary>
        /// Gets the syntax warning listener.  this property should NOT be used to generate warning events,
        /// use <see cref="GenSyntaxWarning"/> for that instead.
        /// </summary>
        /// <value>
        /// The syntax warning listener.
        /// </value>
        private ILSLSyntaxWarningListener SyntaxWarningListener
        {
            get { return _validatorServiceProvider.SyntaxWarningListener; }
        }

        /// <summary>
        /// Gets the syntax error listener.  this property should NOT be used to generate error events,
        /// use <see cref="GenSyntaxError"/> for that instead.
        /// </summary>
        /// <value>
        /// The syntax error listener.
        /// </value>
        private ILSLSyntaxErrorListener SyntaxErrorListener
        {
            get { return _validatorServiceProvider.SyntaxErrorListener; }
        }

        /// <summary>
        /// Returns a reference to <see cref="SyntaxWarningListener"/> and sets <see cref="HasSyntaxWarnings"/> to <c>true</c>.
        /// </summary>
        /// <returns><see cref="SyntaxWarningListener"/></returns>
        private ILSLSyntaxWarningListener GenSyntaxWarning()
        {
            HasSyntaxWarnings = true;
            return SyntaxWarningListener;
        }


        /// <summary>
        /// Returns a reference to <see cref="SyntaxErrorListener"/> and sets <see cref="HasSyntaxErrors"/> to <c>true</c>.
        /// </summary>
        /// <returns><see cref="SyntaxErrorListener"/></returns>
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

            if (_validatorServiceProvider.LibraryDataProvider.LibraryFunctionExist(context.function_name.Text))
            {
                GenSyntaxError().RedefinedStandardLibraryFunction(
                    new LSLSourceCodeRange(context.function_name), context.function_name.Text,
                    _validatorServiceProvider.LibraryDataProvider.GetLibraryFunctionSignatures(
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


            var parameterListNode = LSLParameterListNode.BuildDirectlyFromContext(
                context.parameters,
                _validatorServiceProvider);

            if (parameterListNode.HasErrors)
            {
                return false;
            }

            var returnType = LSLType.Void;

            if (context.return_type != null)
            {
                returnType =
                    LSLTypeTools.FromLSLTypeString(context.return_type.Text);
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