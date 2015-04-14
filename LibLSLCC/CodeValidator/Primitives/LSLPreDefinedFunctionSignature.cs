using System.Linq;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;

namespace LibLSLCC.CodeValidator.Primitives
{
    public class LSLPreDefinedFunctionSignature : LSLFunctionSignature
    {
        public LSLPreDefinedFunctionSignature(LSLType returnType, string name, LSLParameterListNode parameters)
            : base(returnType, name, parameters.Parameters.Select(x => new LSLParameter(x.Type, x.Name,false)))
        {
            ParameterListNode = parameters;
        }



        public LSLParameterListNode ParameterListNode { get; private set; }


        internal LSLParser.FunctionDeclarationContext DeclarationContext { get; set; }


        public LSLFunctionDeclarationNode DefinitionNode { get; private set; }



        public void GiveDefinition(LSLFunctionDeclarationNode definition)
        {
            DefinitionNode = definition;
        }
    }
}