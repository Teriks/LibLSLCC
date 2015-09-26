#region FileInfo
// 
// File: ILSLSyntaxWarningListener.cs
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