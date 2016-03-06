using System;
using System.Linq;

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    /// Extensions for creating LSL signature primitives from related syntax tree nodes.
    /// </summary>
    public static class LSLCreateSignatureNodeExtensions
    {
        /// <summary>
        ///     Build a <see cref="LSLFunctionSignature" /> object based off the signature of this function declaration node.
        /// </summary>
        /// <param name="functionDeclarationNode">The <see cref="ILSLFunctionDeclarationNode"/>.</param>
        /// <returns>The created <see cref="LSLFunctionSignature" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="functionDeclarationNode"/> is <see langword="null" />.</exception>
        public static LSLFunctionSignature CreateSignature(this ILSLFunctionDeclarationNode functionDeclarationNode)
        {
            if (functionDeclarationNode == null) throw new ArgumentNullException("functionDeclarationNode");

            return new LSLFunctionSignature(functionDeclarationNode.ReturnType, functionDeclarationNode.Name,
                functionDeclarationNode.ParameterList.Parameters.Select(x => new LSLParameterSignature(x.Type, x.Name, false)));
        }


        /// <summary>
        ///     Build a <see cref="LSLParameterSignature" /> object based off the signature of this parameter declaration node.
        /// </summary>
        /// <param name="parameterNode">The <see cref="ILSLParameterNode"/>.</param>
        /// <returns>The created <see cref="LSLParameterSignature" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameterNode"/> is <see langword="null" />.</exception>
        public static LSLParameterSignature CreateSignature(this ILSLParameterNode parameterNode)
        {
            if (parameterNode == null) throw new ArgumentNullException("parameterNode");

            return new LSLParameterSignature(parameterNode.Type, parameterNode.Name, false);
        }


        /// <summary>
        ///     Build a <see cref="LSLParameterSignature" /> object based off the signature of this parameter declaration node, with the given parameter index.
        /// </summary>
        /// <param name="parameterNode">The <see cref="ILSLParameterNode"/>.</param>
        /// <param name="parameterIndex">The parameter index to use for the created paramter signature.</param>
        /// <returns>The created <see cref="LSLParameterSignature" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameterNode"/> is <see langword="null" />.</exception>
        public static LSLParameterSignature CreateSignature(this ILSLParameterNode parameterNode, int parameterIndex)
        {
            if (parameterNode == null) throw new ArgumentNullException("parameterNode");

            return new LSLParameterSignature(parameterNode.Type, parameterNode.Name, false, parameterIndex);
        }


        /// <summary>
        ///     Build a <see cref="LSLEventSignature" /> object based off the signature of this function declaration node.
        /// </summary>
        /// <param name="eventHandlerNode">The <see cref="ILSLEventHandlerNode"/>.</param>
        /// <returns>The created <see cref="LSLEventSignature" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="eventHandlerNode"/> is <see langword="null" />.</exception>
        public static LSLEventSignature CreateSignature(this ILSLEventHandlerNode eventHandlerNode)
        {
            if (eventHandlerNode == null) throw new ArgumentNullException("eventHandlerNode");

            return new LSLEventSignature(eventHandlerNode.Name,
                eventHandlerNode.ParameterList.Parameters.Select(x => new LSLParameterSignature(x.Type, x.Name, false)));
        }
    }
}
