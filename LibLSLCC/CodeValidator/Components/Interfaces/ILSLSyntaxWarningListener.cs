using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;

namespace LibLSLCC.CodeValidator.Components.Interfaces
{
    public interface ILSLSyntaxWarningListener
    {
        void MultipleListAssignmentsInExpression(LSLSourceCodeRange location);

        void MultipleStringAssignmentsInExpression(LSLSourceCodeRange location);

        void OnWarning(LSLSourceCodeRange location, string message);



        void DeadCodeDetected(LSLSourceCodeRange location, LSLFunctionSignature currentFunction,
            LSLDeadCodeSegment deadSegment);



        void DeadCodeDetected(LSLSourceCodeRange location, LSLEventSignature currentEvent,
            LSLDeadCodeSegment deadSegment);



        void UselessSemiColon(LSLSourceCodeRange location);

        void ExpressionStatementHasNoEffect(LSLSourceCodeRange location);

        void ForLoopAfterthoughtHasNoEffect(LSLSourceCodeRange location, int expressionIndex, int expressionCountTotal);

        void ForLoopInitExpressionHasNoEffect(LSLSourceCodeRange location, int expressionIndex, int expressionCountTotal);

        void RedundantCast(LSLSourceCodeRange location, LSLType castType);

        void FunctionNeverUsed(LSLSourceCodeRange location, ILSLFunctionDeclarationNode function);

        void GlobalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable);

        void LocalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable, LSLFunctionSignature inFunction);

        void LocalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable, LSLEventSignature inEvent);

        void FunctionParameterNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode parameter, LSLFunctionSignature inFunction);


        void ParameterHidesGlobalVariable(LSLSourceCodeRange location, LSLFunctionSignature functionSignature, LSLParameterNode parameter, LSLVariableDeclarationNode globalVariable);

        void ParameterHidesGlobalVariable(LSLSourceCodeRange location, LSLEventSignature eventHandlerSignature, LSLParameterNode parameter, LSLVariableDeclarationNode globalVariable);

        void LocalVariableHidesParameter(LSLSourceCodeRange location, LSLPreDefinedFunctionSignature functionSignature, LSLVariableDeclarationNode localVariable, LSLParameterNode parameter);

        void LocalVariableHidesParameter(LSLSourceCodeRange location, LSLParsedEventHandlerSignature eventHandlerSignature, LSLVariableDeclarationNode localVariable, LSLParameterNode parameter);

        void LocalVariableHidesGlobalVariable(LSLSourceCodeRange location, LSLPreDefinedFunctionSignature functionSignature, LSLVariableDeclarationNode localVariable, LSLVariableDeclarationNode globalVariable);

        void LocalVariableHidesGlobalVariable(LSLSourceCodeRange location, LSLParsedEventHandlerSignature eventHandlerSignature, LSLVariableDeclarationNode localVariable, LSLVariableDeclarationNode globalVariable);
    }
}