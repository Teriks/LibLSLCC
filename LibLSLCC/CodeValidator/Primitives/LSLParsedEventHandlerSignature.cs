using System.Linq;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;

namespace LibLSLCC.CodeValidator.Primitives
{
    public class LSLParsedEventHandlerSignature : LSLEventSignature
    {
        public LSLParsedEventHandlerSignature(string name, LSLParameterListNode parameters) :
            base(name, parameters.Parameters.Select(x => new LSLParameter(x.Type, x.Name,false)))
        {
            ParameterListNode = parameters;
        }



        public LSLParameterListNode ParameterListNode { get; private set; }
    }
}