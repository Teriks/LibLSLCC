using System;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLReadOnlyCodeStatement : ILSLReadOnlySyntaxTreeNode, ILSLReturnPathNode
    {

        /// <summary>
        /// The index of this statement in its scope
        /// </summary>
        int StatementIndex { get; }

        /// <summary>
        /// Is this statement the last statement in its scope
        /// </summary>
        bool IsLastStatementInScope { get; }

        /// <summary>
        /// The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        LSLDeadCodeType DeadCodeType { get; }

        /// <summary>
        /// Is this statement dead code
        /// </summary>
        bool IsDeadCode { get; }

        /// <summary>
        /// If the scope has a return path, this is set to the node that causes the function to return.
        /// it may be a return statement, or a control chain node.
        /// </summary>
        ILSLReadOnlyCodeStatement ReturnPath { get; }

        /// <summary>
        /// Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        /// this is not the scopes level.
        /// </summary>
        UInt64 ScopeId { get; }
    }


    public interface ILSLCodeStatement : ILSLReadOnlyCodeStatement, ILSLSyntaxTreeNode
    {
        /// <summary>
        /// The index of this statement in its scope
        /// </summary>
        new int StatementIndex { get; set; }

        /// <summary>
        /// Is this statement the last statement in its scope
        /// </summary>
        new bool IsLastStatementInScope { get; set; }

        /// <summary>
        /// The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        new LSLDeadCodeType DeadCodeType { get; set; }

        /// <summary>
        /// Is this statement dead code
        /// </summary>
        new bool IsDeadCode { get; set; }

        /// <summary>
        /// If the scope has a return path, this is set to the node that causes the function to return.
        /// it may be a return statement, or a control chain node.
        /// </summary>
        new ILSLCodeStatement ReturnPath { get; set; }


        /// <summary>
        /// Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        /// this is not the scopes level.
        /// </summary>
        new UInt64 ScopeId { get; set; }
    }
}