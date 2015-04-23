using System.Collections.Generic;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLControlStatementNode : ILSLReadOnlyCodeStatement
    {
        bool HasElseStatement { get; }
        bool HasIfStatement { get; }
        bool HasElseIfStatements { get; }
        ILSLElseStatementNode ElseStatement { get; }
        ILSLIfStatementNode IfStatement { get; }
        IEnumerable<ILSLElseIfStatementNode> ElseIfStatements { get; }
    }
}