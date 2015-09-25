#region FileInfo

// 
// File: ILSLSyntaxWarningListener.cs
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

using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;

#endregion

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

        void ConditionalExpressionIsConstant(LSLSourceCodeRange expression,
            LSLConditionalStatementType conditionalStatementType);

        void UselessSemiColon(LSLSourceCodeRange location);
        void ExpressionStatementHasNoEffect(LSLSourceCodeRange location);
        void ForLoopAfterthoughtHasNoEffect(LSLSourceCodeRange location, int expressionIndex, int expressionCountTotal);
        void ForLoopInitExpressionHasNoEffect(LSLSourceCodeRange location, int expressionIndex, int expressionCountTotal);
        void RedundantCast(LSLSourceCodeRange location, LSLType castType);
        void FunctionNeverUsed(LSLSourceCodeRange location, ILSLFunctionDeclarationNode function);
        void GlobalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable);

        void LocalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable,
            LSLFunctionSignature inFunction);

        void LocalVariableNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode variable,
            LSLEventSignature inEvent);

        void FunctionParameterNeverUsed(LSLSourceCodeRange location, ILSLVariableDeclarationNode parameter,
            LSLFunctionSignature inFunction);

        void ParameterHidesGlobalVariable(LSLSourceCodeRange location, LSLFunctionSignature functionSignature,
            LSLParameterNode parameter, LSLVariableDeclarationNode globalVariable);

        void ParameterHidesGlobalVariable(LSLSourceCodeRange location, LSLEventSignature eventHandlerSignature,
            LSLParameterNode parameter, LSLVariableDeclarationNode globalVariable);

        void LocalVariableHidesParameter(LSLSourceCodeRange location, LSLPreDefinedFunctionSignature functionSignature,
            LSLVariableDeclarationNode localVariable, LSLParameterNode parameter);

        void LocalVariableHidesParameter(LSLSourceCodeRange location,
            LSLParsedEventHandlerSignature eventHandlerSignature, LSLVariableDeclarationNode localVariable,
            LSLParameterNode parameter);

        void LocalVariableHidesGlobalVariable(LSLSourceCodeRange location,
            LSLPreDefinedFunctionSignature functionSignature, LSLVariableDeclarationNode localVariable,
            LSLVariableDeclarationNode globalVariable);

        void LocalVariableHidesGlobalVariable(LSLSourceCodeRange location,
            LSLParsedEventHandlerSignature eventHandlerSignature, LSLVariableDeclarationNode localVariable,
            LSLVariableDeclarationNode globalVariable);
    }
}