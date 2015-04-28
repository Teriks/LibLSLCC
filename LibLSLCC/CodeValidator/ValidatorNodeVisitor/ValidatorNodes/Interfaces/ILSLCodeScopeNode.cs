using System.Collections.Generic;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLCodeScopeNode : ILSLReadOnlyCodeStatement
    {
        /// <summary>
        ///     Constant jump descriptors for constant jumps that occur in this scope
        ///     used only with JumpStatementAnalysis is turned on and dead code caused by
        ///     jump statements is being detected
        /// </summary>
        IEnumerable<LSLConstantJumpDescription> ConstantJumps { get; }

        /// <summary>
        ///     Is this scope a single statement scope, like a brace-less 'if' branch.
        ///     true if IsCodeScope is false
        /// </summary>
        bool IsSingleStatement { get; }

        /// <summary>
        ///     Is this a normal braced code scope.
        ///     true if IsSingleStatement is false
        /// </summary>
        bool IsCodeScope { get; }

        /// <summary>
        ///     Code statements that are children of this code scope
        /// </summary>
        IEnumerable<ILSLReadOnlyCodeStatement> CodeStatements { get; }

        /// <summary>
        ///     ReturnStatementNode != null
        /// </summary>
        bool HasReturnStatementNode { get; }

        /// <summary>
        ///     FirstDeadStatementNode != null
        /// </summary>
        bool HasDeadStatementNodes { get; }

        /// <summary>
        ///     The first statement node to be considered dead, when dead code is detected
        /// </summary>
        ILSLReadOnlyCodeStatement FirstDeadStatementNode { get; }

        /// <summary>
        ///     Returns descriptions of all dead code segments in the top level of this scope,
        ///     if there are any
        /// </summary>
        IEnumerable<LSLDeadCodeSegment> DeadCodeSegments { get; }

        /// <summary>
        ///     The top level return statement for a code scope, if one exists
        /// </summary>
        LSLReturnStatementNode ReturnStatementNode { get; }


        LSLCodeScopeType CodeScopeType { get; }
    }
}