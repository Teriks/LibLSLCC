using System;
using System.Collections.Generic;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

namespace LibLSLCC.CodeValidator.Components
{
    public class LSLDefaultSyntaxErrorListener : ILSLSyntaxErrorListener
    {
        public virtual void GrammarLevelSyntaxError(int line, int column, string message)
        {
            OnError(new LSLSourceCodeRange(line,column), message);
        }



        public virtual void UndefinedVariableReference(LSLSourceCodeRange location, string name)
        {
            OnError(location, string.Format("Variable \"{0}\" is undefined", name));
        }



        public virtual void ParameterNameRedefined(LSLSourceCodeRange location, LSLType type, string name)
        {
            OnError(location, string.Format("Parameter name \"{0}\" is used more than once", name));
        }



        public virtual void InvalidBinaryOperation(LSLSourceCodeRange location, ILSLExprNode left, string operation,
            ILSLExprNode right)
        {
            OnError(location, string.Format(
                "{0} {1} {2} is not a valid operation. operator cannot handle these types, (missing a cast?)",
                left.DescribeType(), operation, right.DescribeType()));
        }



        public virtual void InvalidPrefixOperation(LSLSourceCodeRange location, string operation, ILSLExprNode right)
        {
            OnError(location, string.Format(
                "{0}{1} is not a valid operation. operator cannot handle this type, (missing a cast?)", operation,
                right.DescribeType()));
        }



        public virtual void InvalidPostfixOperation(LSLSourceCodeRange location, ILSLExprNode left, string operation)
        {
            OnError(location, string.Format(
                "{0}{1} is not a valid operation. operator cannot handle this type, (missing a cast?)",
                left.DescribeType(),
                operation));
        }



        public virtual void InvalidCastOperation(LSLSourceCodeRange location, LSLType castTo,
            ILSLExprNode fromExpression)
        {
            OnError(location, string.Format(
                "Cannot cast to {0} from {1}", castTo, fromExpression.DescribeType()));
        }



        public virtual void TypeMismatchInVariableDeclaration(LSLSourceCodeRange location, LSLType variableType,
            ILSLExprNode assignedExpression)
        {
            OnError(location, string.Format(
                "Type mismatch in variable declaration, {0} cannot be assigned to a {1} variable",
                assignedExpression.DescribeType(),
                "(" + variableType + ")"));
        }



        public virtual void VariableRedefined(LSLSourceCodeRange location, LSLType variableType, string variableName)
        {
            OnError(location, string.Format(
                "Variable name conflict, \"{0}\" is already defined and accessible from this scope", variableName));
        }



        public virtual void InvalidVectorContent(LSLSourceCodeRange location, LSLVectorComponent component,
            ILSLExprNode invalidExpressionContent)
        {
            OnError(location,
                string.Format("Vectors {0} component is not a float or integer, erroneous type is {1}",
                    component.ToComponentName(),
                    invalidExpressionContent.DescribeType()));
        }



        public virtual void InvalidListContent(LSLSourceCodeRange location, int index,
            ILSLExprNode invalidExpressionContent)
        {
            OnError(location,
                string.Format("Lists cannot contain the type {0}, encountered bad type at list index {1}",
                    invalidExpressionContent.DescribeType(), index));
        }



        public virtual void InvalidRotationContent(LSLSourceCodeRange location, LSLRotationComponent component,
            ILSLExprNode invalidExpressionContent)
        {
            OnError(location,
                string.Format("Rotations {0} component is not a float or integer, erroneous type is {1}",
                    component.ToComponentName(),
                    invalidExpressionContent.DescribeType()));
        }



        public virtual void ReturnedValueFromVoidFunction(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature,
            ILSLExprNode attemptedReturnExpression)
        {
            OnError(location, string.Format(
                "Cannot return {0} value from function \"{1}\" because it does not specify a return type",
                attemptedReturnExpression.DescribeType(), functionSignature.Name));
        }



        public virtual void TypeMismatchInReturnValue(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature,
            ILSLExprNode attemptedReturnExpression)
        {
            OnError(location, string.Format(
                "Type mismatch in return type for function \"{0}\", expected {1} and got {2}",
                functionSignature.Name,
                functionSignature.ReturnType,
                attemptedReturnExpression.DescribeType()));
        }



        public virtual void ReturnedVoidFromANonVoidFunction(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature)
        {
            OnError(location, string.Format("function \"{0}\" must return value, expected {1} but got Void",
                functionSignature.Name,
                functionSignature.ReturnType));
        }



        public virtual void JumpToUndefinedLabel(LSLSourceCodeRange location, string labelName)
        {
            OnError(location, string.Format("Label \"{0}\" is not defined",
                labelName));
        }



        public virtual void CallToUndefinedFunction(LSLSourceCodeRange location, string functionName)
        {
            OnError(location, string.Format("Function \"{0}\" is not defined",
                functionName));
        }



        public virtual void ImproperParameterCountInFunctionCall(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature, ILSLExprNode[] parameterExpressionsGiven)
        {
            object length = parameterExpressionsGiven.Length == 0 ?
                    (object)"no" :
                    parameterExpressionsGiven.Length;

            if (!functionSignature.HasVariadicParameter)
            {
                if (functionSignature.ParameterCount == 0)
                {
                    OnError(location, string.Format(
                        "Function \"{0}\" does not take any parameters, but {1} parameters were passed",
                        functionSignature.Name,
                        parameterExpressionsGiven.Length));
                }
                else
                {
                    OnError(location, string.Format(
                        "Function \"{0}\" takes {1} parameter(s), but {2} parameters were passed",
                        functionSignature.Name,
                        functionSignature.ParameterCount,
                        length));
                }
            }
            else
            {
                OnError(location, string.Format(
                    "Variadic Function \"{0}\" takes {1} concrete parameter(s), but {2} parameters were passed",
                    functionSignature.Name,
                    functionSignature.ConcreteParameterCount,
                    length));
                
            }
        }



        public virtual void ReturnedValueFromEventHandler(LSLSourceCodeRange location,
            ILSLExprNode attemptedReturnExpression)
        {
            OnError(location, "Cannot return an actual value from an Event Handler");
        }



        public virtual void RedefinedFunction(LSLSourceCodeRange location,
            LSLFunctionSignature previouslyDefinedSignature)
        {
            OnError(location,
                string.Format("Function \"{0}\" has already been defined", previouslyDefinedSignature.Name));
        }



        public virtual void RedefinedLabel(LSLSourceCodeRange location, string labelName)
        {
            OnError(location, string.Format("Label {0} is already defined", labelName));
        }



        public virtual void TupleAccessorOnLiteral(LSLSourceCodeRange location, ILSLExprNode lvalueLiteral,
            string operationText)
        {
            OnError(location,
                string.Format("\".{0}\" member access operator cannot be used on Literals", operationText));
        }



        public virtual void TupleAccessorOnCompoundExpression(LSLSourceCodeRange location, ILSLExprNode lvalueCompound,
            string operationText)
        {
            OnError(location,
                string.Format("\".{0}\" member access operator cannot be used on compound expressions", operationText));
        }



        public virtual void DeadCodeAfterReturnPathDetected(LSLSourceCodeRange location, LSLFunctionSignature inFunction,
            LSLDeadCodeSegment deadSegment)
        {
            if (deadSegment.SourceCodeRange.IsSingleLine)
            {
                OnError(location, "Unreachable code detected after return path in function \"" + inFunction.Name + "\"");
            }
            else
            {
                OnError(location,
                    string.Format(
                        "Unreachable code detected after return path in function \"" + inFunction.Name +
                        "\" between lines {0} and {1}",
                        deadSegment.SourceCodeRange.LineStart, deadSegment.SourceCodeRange.LineEnd));
            }
        }



        public virtual void NotAllCodePathsReturnAValue(LSLSourceCodeRange location, LSLFunctionSignature inFunction)
        {
            OnError(location, "Not all code paths return a value in function \"" + inFunction.Name + "\"");
        }



        public virtual void StateHasNoEventHandlers(LSLSourceCodeRange location, string stateName)
        {
            OnError(location,
                "State \"" + stateName +
                "\" has no event handlers defined, state's must have at least one event handler");
        }



        public virtual void MissingConditionalExpression(LSLSourceCodeRange location,
            LSLConditionalStatementType statementType)
        {
            OnError(location, "Conditional expression was required but not given");
        }



        public virtual void DefinedVariableInNonScopeBlock(LSLSourceCodeRange location)
        {
            OnError(location, "Declaration requires a new scope, use { and }");
        }



        public virtual void IllegalStringCharacter(LSLSourceCodeRange location, LSLStringCharacterError chr)
        {
            OnError(location,
                string.Format("Illegal character '{0}' found in string at index [{1}]", chr.CausingCharacter,
                    chr.StringIndex));
        }



        public virtual void InvalidStringEscapeCode(LSLSourceCodeRange location, LSLStringCharacterError code)
        {
            OnError(location,
                string.Format("Unknown escape sequence '\\{0}' found in string at index [{1}]", code.CausingCharacter,
                    code.StringIndex));
        }



        public virtual void CallToFunctionInStaticContext(LSLSourceCodeRange location)
        {
            OnError(location, "Functions cannot be called in a static context, ie. assigning global variables");
        }



        public virtual void ModifyingAssignmentToCompoundExpression(LSLSourceCodeRange location, string operation)
        {
            OnError(location,
                string.Format("'{0}' Operator cannot have a compound expression as a left operand", operation));
        }



        public virtual void AssignmentToCompoundExpression(LSLSourceCodeRange location)
        {
            OnError(location, string.Format("Cannot assign value to a compound expression"));
        }



        public virtual void AssignmentToLiteral(LSLSourceCodeRange location)
        {
            OnError(location, string.Format("Cannot assign value to a literal"));
        }



        public virtual void ModifyingAssignmentToLiteral(LSLSourceCodeRange location, string operation)
        {
            OnError(location, string.Format("'{0}' Operator cannot have a literal as a left operand", operation));
        }



        public void RedefinedEventHandler(LSLSourceCodeRange location, string eventHandlerName, string stateName)
        {
            OnError(location,
                string.Format("Event handler '{0}' was defined more than once in state '{1}'", eventHandlerName,
                    stateName));
        }



        public void MissingDefaultState()
        {
            OnError(new LSLSourceCodeRange(), "Default state is missing.");
        }



        public void NoSuitableLibraryFunctionOverloadFound(LSLSourceCodeRange location, string functionName,
            IReadOnlyList<ILSLExprNode> givenParameters)
        {
            OnError(location,"Overloads exist, but no matching overload found for library function \""+functionName+"\"");
        }



        public virtual void InvalidComponentAccessorOperation(LSLSourceCodeRange location, ILSLExprNode exprLvalue,
            string componentAccessed)
        {
            OnError(location,
                string.Format("\".{0}\" member accessor is not valid on {1}'s", componentAccessed, exprLvalue.Type));
        }



        public virtual void IfConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            OnError(location, string.Format(
                "If condition must evaluate to an Integer, Key, Vector, Rotation or List" +
                " but given expression evaluates to {0}",
                attemptedConditionExpression.DescribeType()));
        }



        public virtual void ElseIfConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            OnError(location, string.Format(
                "Else If condition must evaluate to an Integer, Key, Vector, Rotation or List" +
                " but given expression evaluates to {0}",
                attemptedConditionExpression.DescribeType()));
        }



        public virtual void DoLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            OnError(location, string.Format(
                "Do loop condition must evaluate to an Integer, Key, Vector, Rotation or List" +
                " but given expression evaluates to {0}", attemptedConditionExpression.DescribeType()));
        }



        public virtual void WhileLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            OnError(location, string.Format(
                "While loop condition must evaluate to an Integer, Key, Vector, Rotation or List" +
                " but given expression evaluates to {0}",
                attemptedConditionExpression.DescribeType()));
        }



        public virtual void ForLoopConditionNotValidType(LSLSourceCodeRange location,
            ILSLExprNode attemptedConditionExpression)
        {
            OnError(location, string.Format(
                "For loop condition must evaluate to an Integer, Key, Vector, Rotation or List" +
                " but given expression evaluates to {0}",
                attemptedConditionExpression.DescribeType()));
        }



        public virtual void ParameterTypeMismatchInFunctionCall(LSLSourceCodeRange location,
            int parameterNumberWithError,
            LSLFunctionSignature calledFunction, ILSLExprNode[] parameterExpressionsGiven)
        {
            string message = calledFunction.HasVariadicParameter ?
                "Type Mismatch in call to Variadic Function \"{0}\" at Parameter #{1} ({2}), expected {3} and got {4}" :
                "Type Mismatch in call to Function \"{0}\" at Parameter #{1} ({2}), expected {3} and got {4}";

            OnError(location, string.Format(
                message,
                calledFunction.Name,
                parameterNumberWithError,
                calledFunction.Parameters[parameterNumberWithError].Name,
                calledFunction.Parameters[parameterNumberWithError].Type,
                parameterExpressionsGiven[parameterNumberWithError].DescribeType()));
        }


        public virtual void RedefinedStateName(LSLSourceCodeRange location, string stateName)
        {
            OnError(location, "State \"" + stateName + "\" has already been defined");
        }



        public virtual void UnknownEventHandlerDeclared(LSLSourceCodeRange location,
            LSLEventSignature givenEventHandlerSignature)
        {
            OnError(location,
                "Event handler \"" + givenEventHandlerSignature.Name + "\" is not a valid LSL event handler");
        }



        public virtual void IncorrectEventHandlerSignature(LSLSourceCodeRange location,
            LSLEventSignature givenEventHandlerSignature,
            LSLLibraryEventSignature requiredEventHandlerSignature)
        {
            OnError(location,
                "Event handler \"" + givenEventHandlerSignature.Name + "\" has incorrect parameter definitions");
        }



        public virtual void RedefinedStandardLibraryConstant(LSLSourceCodeRange location, LSLType redefinitionType,
            LSLLibraryConstantSignature originalSignature)
        {
            OnError(location,
                "Cannot define variable with name \"" + originalSignature.Name +
                "\" as it's the name of an existing default library constant");
        }



        public virtual void RedefinedStandardLibraryFunction(LSLSourceCodeRange location, string functionName,
            IReadOnlyList<LSLLibraryFunctionSignature> libraryFunctionSignatureOverloads)
        {
            OnError(location,
                "Cannot define function with name \"" + functionName +
                "\" as it's the name of an existing default library function");
        }



        public virtual void ChangeToUndefinedState(LSLSourceCodeRange location, string stateName)
        {
            OnError(location,
                "Cannot change to state \"" + stateName +
                "\" as a state with that name does not exist");
        }



        public virtual void ModifiedLibraryConstant(LSLSourceCodeRange location, string constantName)
        {
            OnError(location,
                "Cannot modify library constant \"" + constantName + "\"");
        }



        public virtual void RedefinedDefaultState(LSLSourceCodeRange location)
        {
            OnError(location,
                "Cannot defined a new state with the name \"default\" as that is the name of LSL's default state");
        }



        public virtual void InvalidStatementExpression(LSLSourceCodeRange location)
        {
            OnError(location,
                "Only assignment, call, increment, decrement, and variable declaration expressions can be used as a statement");
        }



        public virtual void OnError(LSLSourceCodeRange location, string message)
        {
            Console.WriteLine("({0},{1}) ERROR: {2}", location.LineStart, location.ColumnStart,
                message + Environment.NewLine);
        }
    }
}