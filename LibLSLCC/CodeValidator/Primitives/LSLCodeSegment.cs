using System.Collections.Generic;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

namespace LibLSLCC.CodeValidator.Primitives
{
    public class LSLCodeSegment
    {
        private readonly List<ILSLReadOnlyCodeStatement> _statementNodes;





        public LSLCodeSegment()
        {
            _statementNodes = new List<ILSLReadOnlyCodeStatement>();
            StartNode = null;
            EndNode = null;
            SourceCodeRange = new LSLSourceCodeRange();
        }



        public LSLSourceCodeRange SourceCodeRange { get; private set; }
        public ILSLReadOnlyCodeStatement StartNode { get; private set; }
        public ILSLReadOnlyCodeStatement EndNode { get; private set; }

        public IReadOnlyList<ILSLReadOnlyCodeStatement> StatementNodes
        {
            get { return _statementNodes.AsReadOnly(); }
        }



        public virtual void AddStatement(ILSLReadOnlyCodeStatement statement)
        {
            EndNode = statement;
            if (StartNode == null)
            {
                StartNode = statement;
                SourceCodeRange = new LSLSourceCodeRange(statement);
                _statementNodes.Add(statement);
            }
            else
            {
                _statementNodes.Add(statement);
                SourceCodeRange.ExtendTo(EndNode);
            }
        }
    }
}