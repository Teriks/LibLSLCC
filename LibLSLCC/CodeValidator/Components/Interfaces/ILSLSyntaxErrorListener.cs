using System.Collections.Generic;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

namespace LibLSLCC.CodeValidator.Components.Interfaces
{
    public enum LSLConditionalStatementType
    {
        If,
        ElseIf,
        While,
        DoWhile,
        For
    }

    public interface ILSLSyntaxErrorListener
    {
        void GrammarLevelSyntaxError(int line, int column, string message);

        void UndefinedVariableReference(LSLSourceCodeRange location, string name);

        void ParameterNameRedefined(LSLSourceCodeRange location, LSLType type, string name);

        void InvalidBinaryOperation(LSLSourceCodeRange location, ILSLExprNode left, string operation, ILSLExprNode right);

        void InvalidPrefixOperation(LSLSourceCodeRange location, string operation, ILSLExprNode right);

        void InvalidPostfixOperation(LSLSourceCodeRange location, ILSLExprNode left, string operation);

        void InvalidCastOperation(LSLSourceCodeRange location, LSLType castTo, ILSLExprNode fromExpression);



        void TypeMismatchInVariableDeclaration(LSLSourceCodeRange location, LSLType variableType,
            ILSLExprNode assignedExpression);



        void VariableRedefined(LSLSourceCodeRange location, LSLType variableType, string variableName);



        void InvalidVectorContent(LSLSourceCodeRange location, LSLVectorComponent component,
            ILSLExprNode invalidExpressionContent);



        void InvalidListContent(LSLSourceCodeRange location, int index, ILSLExprNode invalidExpressionContent);



        void InvalidRotationContent(LSLSourceCodeRange location, LSLRotationComponent component,
            ILSLExprNode invalidExpressionContent);



        void ReturnedValueFromVoidFunction(LSLSourceCodeRange location, LSLFunctionSignature functionSignature,
            ILSLExprNode attemptedReturnExpression);



        void TypeMismatchInReturnValue(LSLSourceCodeRange location, LSLFunctionSignature functionSignature,
            ILSLExprNode attemptedReturnExpression);



        void ReturnedVoidFromANonVoidFunction(LSLSourceCodeRange location, LSLFunctionSignature functionSignature);

        void JumpToUndefinedLabel(LSLSourceCodeRange location, string labelName);

        void CallToUndefinedFunction(LSLSourceCodeRange location, string functionName);



        void ImproperParameterCountInFunctionCall(LSLSourceCodeRange location,
            LSLFunctionSignature functionSignature, ILSLExprNode[] parameterExpressionsGiven);



        void ReturnedValueFromEventHandler(LSLSourceCodeRange location, ILSLExprNode attemptedReturnExpression);

        void RedefinedFunction(LSLSourceCodeRange location, LSLFunctionSignature previouslyDefinedSignature);

        void RedefinedLabel(LSLSourceCodeRange location, string labelName);

        void TupleAccessorOnLiteral(LSLSourceCodeRange location, ILSLExprNode lvalueLiteral, string operationText);



        void TupleAccessorOnCompoundExpression(LSLSourceCodeRange location, ILSLExprNode lvalueCompound,
            string operationText);



        void InvalidComponentAccessorOperation(LSLSourceCodeRange location, ILSLExprNode exprLvalue,
            string componentAccessed);



        void IfConditionNotValidType(LSLSourceCodeRange location, ILSLExprNode attemptedConditionExpression);

        void ElseIfConditionNotValidType(LSLSourceCodeRange location, ILSLExprNode attemptedConditionExpression);

        void DoLoopConditionNotValidType(LSLSourceCodeRange location, ILSLExprNode attemptedConditionExpression);

        void WhileLoopConditionNotValidType(LSLSourceCodeRange location, ILSLExprNode attemptedConditionExpression);

        void ForLoopConditionNotValidType(LSLSourceCodeRange location, ILSLExprNode attemptedConditionExpression);



        void ParameterTypeMismatchInFunctionCall(LSLSourceCodeRange location, int parameterNumberWithError,
            LSLFunctionSignature calledFunction, ILSLExprNode[] parameterExpressionsGiven);



        void RedefinedStateName(LSLSourceCodeRange location, string stateName);



        void UnknownEventHandlerDeclared(LSLSourceCodeRange location,
            LSLEventSignature givenEventHandlerSignature);



        void IncorrectEventHandlerSignature(LSLSourceCodeRange location,
            LSLEventSignature givenEventHandlerSignature,
            LSLLibraryEventSignature correctEventHandlerSignature);



        void RedefinedStandardLibraryConstant(LSLSourceCodeRange location, LSLType redefinitionType,
            LSLLibraryConstantSignature originalSignature);



        void RedefinedStandardLibraryFunction(LSLSourceCodeRange location, string functionName,
            IReadOnlyList<LSLLibraryFunctionSignature> libraryFunctionSignatureOverloads);



        void ChangeToUndefinedState(LSLSourceCodeRange location, string stateName);

        void ModifiedLibraryConstant(LSLSourceCodeRange location, string constantName);

        void RedefinedDefaultState(LSLSourceCodeRange location);

        void InvalidStatementExpression(LSLSourceCodeRange location);



        void DeadCodeAfterReturnPathDetected(LSLSourceCodeRange location, LSLFunctionSignature inFunction,
            LSLDeadCodeSegment deadSegment);



        void NotAllCodePathsReturnAValue(LSLSourceCodeRange location, LSLFunctionSignature inFunction);

        void StateHasNoEventHandlers(LSLSourceCodeRange location, string stateName);

        void MissingConditionalExpression(LSLSourceCodeRange location, LSLConditionalStatementType statementType);

        void DefinedVariableInNonScopeBlock(LSLSourceCodeRange location);

        void IllegalStringCharacter(LSLSourceCodeRange location, LSLStringCharacterError chr);

        void InvalidStringEscapeCode(LSLSourceCodeRange location, LSLStringCharacterError code);

        void CallToFunctionInStaticContext(LSLSourceCodeRange location);

        void ModifyingAssignmentToCompoundExpression(LSLSourceCodeRange location, string operation);

        void AssignmentToCompoundExpression(LSLSourceCodeRange location);

        void AssignmentToLiteral(LSLSourceCodeRange location);

        void ModifyingAssignmentToLiteral(LSLSourceCodeRange location, string operation);

        void RedefinedEventHandler(LSLSourceCodeRange location, string eventHandlerName, string stateName);

        void MissingDefaultState();

        void NoSuitableLibraryFunctionOverloadFound(LSLSourceCodeRange location, string functionName, IReadOnlyList<ILSLExprNode> givenParameters);
    }
}