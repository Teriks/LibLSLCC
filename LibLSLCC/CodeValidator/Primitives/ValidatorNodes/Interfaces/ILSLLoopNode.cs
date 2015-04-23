using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLLoopNode
    {
        ILSLReadOnlyExprNode ConditionExpression { get; }

        ILSLCodeScopeNode Code { get; }
    }
}
