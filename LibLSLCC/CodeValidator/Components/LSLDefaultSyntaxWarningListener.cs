using System;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;

namespace LibLSLCC.CodeValidator.Components
{
    public class LSLDefaultSyntaxWarningListener : ILSLSyntaxWarningListener
    {
        public virtual void MultipleListAssignmentsInExpression(LSLSourceCodeRange location)
        {
            OnWarning(location,
                "Multiple list assignments in expression, may not evaluate how you expect");
        }



        public virtual void MultipleStringAssignmentsInExpression(LSLSourceCodeRange location)
        {
            OnWarning(location,
                "Multiple string assignments in expression, may not evaluate how you expect");
        }



        public virtual void OnWarning(LSLSourceCodeRange location, string message)
        {
            Console.WriteLine("({0},{1}) WARNING: {2}", location.LineStart, location.ColumnStart,
                message + Environment.NewLine);
        }



        public virtual void DeadCodeDetected(LSLSourceCodeRange location, LSLFunctionSignature currentFunction,
            LSLDeadCodeSegment deadSegment)
        {
            if (deadSegment.SourceCodeRange.IsSingleLine)
            {
                OnWarning(location, "Unreachable code detected in function \"" + currentFunction.Name + "\"");
            }
            else
            {
                OnWarning(location,
                    string.Format(
                        "Unreachable code detected in function \"" + currentFunction.Name +
                        "\" between lines {0} and {1}",
                        deadSegment.SourceCodeRange.LineStart, deadSegment.SourceCodeRange.LineEnd));
            }
        }



        public virtual void DeadCodeDetected(LSLSourceCodeRange location, LSLEventSignature currentEvent,
            LSLDeadCodeSegment deadSegment)
        {
            if (deadSegment.SourceCodeRange.IsSingleLine)
            {
                OnWarning(location, "Unreachable code detected in event handler \"" + currentEvent.Name + "\"");
            }
            else
            {
                OnWarning(location,
                    string.Format(
                        "Unreachable code detected in event handler \"" + currentEvent.Name +
                        "\" between lines {0} and {1}",
                        deadSegment.SourceCodeRange.LineStart, deadSegment.SourceCodeRange.LineEnd));
            }
        }



        public virtual void UselessSemiColon(LSLSourceCodeRange location)
        {
            OnWarning(location, "Pointless semicolon detected");
        }



        public virtual void ExpressionStatementHasNoEffect(LSLSourceCodeRange location)
        {
            OnWarning(location, "Expression statement has no effect");
        }



        public virtual void ForLoopAfterthoughtHasNoEffect(LSLSourceCodeRange location, int expressionIndex,
            int expressionCountTotal)
        {
            if (expressionCountTotal == 1)
            {
                OnWarning(location, "For loop afterthought has no effect");
            }
            else
            {
                OnWarning(location,
                    string.Format("For loop afterthought number {0} has no effect", expressionIndex + 1));
            }
        }



        public virtual void RedundantCast(LSLSourceCodeRange location, LSLType castType)
        {
            OnWarning(location, string.Format("Redundant cast to {0}", castType));
        }



        public void FunctionNeverUsed(LSLSourceCodeRange location, ILSLFunctionDeclarationNode function)
        {
            OnWarning(location, string.Format("Function \"{0}\" was never used", function.Name));
        }



        public void GlobalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable)
        {
            const string msg = "Global variable \"{0}\" was never used";


            OnWarning(location, string.Format(msg, variable.Name));
        }



        public void LocalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable,
            LSLFunctionSignature inFunction)
        {
            const string msg = "Local variable \"{0}\" was never used in function \"{1}\"";


            OnWarning(location, string.Format(msg, variable.Name, inFunction.Name));
        }



        public void LocalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable,
            LSLEventSignature inEvent)
        {
            const string msg = "Local variable \"{0}\" was never used in event \"{1}\"";

            OnWarning(location, string.Format(msg, variable.Name,inEvent.Name));

        }



        public void FunctionParameterNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode parameter, LSLFunctionSignature inFunction)
        {
            OnWarning(location, string.Format("Parameter \"{0}\" was never used in function \"{1}\"", parameter.Name,inFunction.Name));
        }



        public void ParameterHidesGlobalVariable(LSLSourceCodeRange location, LSLFunctionSignature functionSignature,
            LSLParameterNode parameter, LSLVariableDeclarationNode globalVariable)
        {
            OnWarning(location, string.Format("Parameter \"{0}\" of function \"{1}\" hides global variable \"{2}\" defined on line {3}",
                parameter.Name, functionSignature.Name, globalVariable.Name, globalVariable.SourceCodeRange.LineStart));
        }



        public void ParameterHidesGlobalVariable(LSLSourceCodeRange location, LSLEventSignature eventHandlerSignature,
            LSLParameterNode parameter, LSLVariableDeclarationNode globalVariable)
        {
            OnWarning(location, string.Format("Parameter \"{0}\" of event handler \"{1}\" hides global variable \"{2}\" defined on line {3}",
                parameter.Name, eventHandlerSignature.Name, globalVariable.Name, globalVariable.SourceCodeRange.LineStart));
        }



        public void LocalVariableHidesParameter(LSLSourceCodeRange location, LSLPreDefinedFunctionSignature functionSignature,
            LSLVariableDeclarationNode localVariable, LSLParameterNode parameter)
        {
            OnWarning(location, string.Format("Local variable \"{0}\" in function \"{1}\" hides parameter \"{2}\"",
                localVariable.Name, functionSignature.Name, parameter.Name));
        }



        public void LocalVariableHidesParameter(LSLSourceCodeRange location, LSLParsedEventHandlerSignature eventHandlerSignature,
            LSLVariableDeclarationNode localVariable, LSLParameterNode parameter)
        {
            OnWarning(location, string.Format("Local variable \"{0}\" in event handler \"{1}\" hides parameter \"{2}\"",
                localVariable.Name, eventHandlerSignature.Name, parameter.Name));
        }



        public void LocalVariableHidesGlobalVariable(LSLSourceCodeRange location, LSLPreDefinedFunctionSignature functionSignature,
            LSLVariableDeclarationNode localVariable, LSLVariableDeclarationNode globalVariable)
        {
            OnWarning(location, string.Format("Local variable \"{0}\" in function \"{1}\" hides global variable \"{2}\" defined on line {3}",
                localVariable.Name, functionSignature.Name, globalVariable.Name, globalVariable.SourceCodeRange.LineStart));
        }



        public void LocalVariableHidesGlobalVariable(LSLSourceCodeRange location, LSLParsedEventHandlerSignature eventHandlerSignature,
            LSLVariableDeclarationNode localVariable, LSLVariableDeclarationNode globalVariable)
        {
            OnWarning(location, string.Format("Local variable \"{0}\" in event handler \"{1}\" hides global variable \"{2}\" defined on line {3}",
                localVariable.Name, eventHandlerSignature.Name, globalVariable.Name, globalVariable.SourceCodeRange.LineStart));
        }



        public virtual void NotAllCodePathsReturnAValue(LSLSourceCodeRange location,
            LSLFunctionSignature currentFunction)
        {
            OnWarning(location, "Not all code paths return a value in function \"" + currentFunction.Name + "\"");
        }
    }
}