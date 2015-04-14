using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.ThreadSafeEnumeration;

namespace LibLSLCC.CodeValidator.Components
{
    public class LSLLibraryDataProvider : ILSLMainLibraryDataProvider
    {
        private readonly Dictionary<string, LSLLibraryConstantSignature> _validConstants
            = new Dictionary<string, LSLLibraryConstantSignature>();

        private readonly Dictionary<string, LSLLibraryEventSignature> _validEventHandlers =
            new Dictionary<string, LSLLibraryEventSignature>();

        private readonly Dictionary<string, List<LSLLibraryFunctionSignature>> _validLibraryFunctions
            = new Dictionary<string, List<LSLLibraryFunctionSignature>>();

        public IEnumerable<LSLLibraryEventSignature> SupportedEventHandlers
        {
            get { return _validEventHandlers.Values.AsLocked(_validEventHandlers); }
        }

        public IEnumerable<IReadOnlyList<LSLLibraryFunctionSignature>> LibraryFunctions
        {
            get { return _validLibraryFunctions.Values.AsLocked(_validLibraryFunctions); }
        }

        public IEnumerable<LSLLibraryConstantSignature> LibraryConstants
        {
            get { return _validConstants.Values.AsLocked(_validLibraryFunctions); }
        }



        protected void ClearLibraryConstants()
        {
            _validConstants.Clear();
        }



        protected void ClearLibraryFunctions()
        {
            _validLibraryFunctions.Clear();
        }



        protected void ClearEventHandlers()
        {
            _validEventHandlers.Clear();
        }



        /// <summary>
        ///     Return true if an event handler with the given name exists in the default library.
        /// </summary>
        /// <param name="name">Name of the event handler.</param>
        /// <returns>True if the event handler with given name exists.</returns>
        public bool EventHandlerExist(string name)
        {
            return _validEventHandlers.ContainsKey(name);
        }



        /// <summary>
        ///     Return an LSLEventHandlerSignature object describing an event handler signature;
        ///     if the event handler with the given name exists, otherwise null.
        /// </summary>
        /// <param name="name">Name of the event handler</param>
        /// <returns>
        ///     An LSLEventHandlerSignature object describing the given event handlers signature,
        ///     or null if the event handler does not exist.
        /// </returns>
        public LSLLibraryEventSignature GetEventHandlerSignature(string name)
        {
            LSLLibraryEventSignature result;

            if (_validEventHandlers.TryGetValue(name, out result))
            {
                return result;
            }
            return null;
        }



        /// <summary>
        ///     Return true if a library function with the given name exists.
        /// </summary>
        /// <param name="name">Name of the library function.</param>
        /// <returns>True if the library function with given name exists.</returns>
        public bool LibraryFunctionExist(string name)
        {
            return _validLibraryFunctions.ContainsKey(name);
        }



        /// <summary>
        ///     Return an LSLFunctionSignature list object describing the function call signatures of a library function;
        ///     if the function with the given name exists as a singular or overloaded function, otherwise null.
        /// </summary>
        /// <param name="name">Name of the library function.</param>
        /// <returns>
        ///     An LSLFunctionSignature list object describing the given library functions signatures,
        ///     or null if the library function does not exist.
        /// </returns>
        public IReadOnlyList<LSLLibraryFunctionSignature> GetLibraryFunctionSignatures(string name)
        {
            List<LSLLibraryFunctionSignature> result;

            if (_validLibraryFunctions.TryGetValue(name, out result))
            {
                return result;
            }
            return null;
        }



        /// <summary>
        ///     Return true if a library constant with the given name exists.
        /// </summary>
        /// <param name="name">Name of the library constant.</param>
        /// <returns>True if a library constant with the given name exists.</returns>
        public bool LibraryConstantExist(string name)
        {
            return _validConstants.ContainsKey(name);
        }



        /// <summary>
        ///     Return the library constant if it exists, otherwise null.
        /// </summary>
        /// <param name="name">Name of the library constant.</param>
        /// <returns>
        ///     The library constants signature
        /// </returns>
        public LSLLibraryConstantSignature GetLibraryConstantSignature(string name)
        {
            LSLLibraryConstantSignature result;

            if (_validConstants.TryGetValue(name, out result))
            {
                return result;
            }
            return null;
        }



        public virtual void AddValidConstant(LSLLibraryConstantSignature signature)
        {
            if (_validConstants.ContainsKey(signature.Name))
            {
                if (!AccumulateDuplicates)
                {
                    throw new LSLDuplicateSignatureException("Constant with name \"" + signature.Name +
                                                             "\" is already defined");
                }
                _duplicateConstantsDefined.Add(signature);
            }
            else
            {
                _validConstants.Add(signature.Name, signature);
            }
        }

        /// <summary>
        /// Only useful when Subsets contains the "all" keyword
        /// </summary>
        public IEnumerable<LSLLibraryEventSignature> DuplicateEventsDefined
        {
            get { return _duplicateEventsDefined; }
        }


        /// <summary>
        /// Only useful when Subsets contains the "all" keyword
        /// </summary>
        public IEnumerable<LSLLibraryConstantSignature> DuplicateConstantsDefined
        {
            get { return _duplicateConstantsDefined; }
        }


        /// <summary>
        /// Only useful when Subsets contains the "all" keyword
        /// </summary>
        public IEnumerable<LSLLibraryFunctionSignature> DuplicateFunctionsDefined
        {
            get { return _duplicateFunctionsDefined; }
        }


        private readonly List<LSLLibraryConstantSignature> _duplicateConstantsDefined = new List<LSLLibraryConstantSignature>();
        private readonly List<LSLLibraryFunctionSignature> _duplicateFunctionsDefined = new List<LSLLibraryFunctionSignature>();
        private readonly List<LSLLibraryEventSignature> _duplicateEventsDefined = new List<LSLLibraryEventSignature>();

        public virtual void AddValidEventHandler(LSLLibraryEventSignature signature)
        {


            if (_validEventHandlers.ContainsKey(signature.Name))
            {
                if (!AccumulateDuplicates)
                {
                    throw new LSLDuplicateSignatureException("Event handler with name \"" + signature.Name +
                                                             "\" is already defined");
                }
                _duplicateEventsDefined.Add(signature);
            }
            else
            {
                _validEventHandlers.Add(signature.Name, signature);
            }
        }

        public bool AccumulateDuplicates { get; protected set; }


        public virtual void AddValidLibraryFunction(LSLLibraryFunctionSignature signature)
        {
            if (_validLibraryFunctions.ContainsKey(signature.Name))
            {
                if (_validLibraryFunctions[signature.Name].Any(y => y.SignatureMatches(signature)))
                {
                    if (!AccumulateDuplicates)
                    {
                        throw new LSLDuplicateSignatureException("Library function with name \"" + signature.Name +
                                                                 "\" is already defined");
                    }
                    _duplicateFunctionsDefined.Add(signature);
                }
                else
                {
                    _validLibraryFunctions[signature.Name].Add(signature);
                }
            }
            else
            {

                _validLibraryFunctions.Add(signature.Name, new List<LSLLibraryFunctionSignature>() {signature});
            }
        }


        public virtual void AddValidConstant(LSLType type, string name)
        {
            AddValidConstant(new LSLLibraryConstantSignature(type,name));
        }



        public virtual void AddValidEventHandler(string name, IEnumerable<LSLParameter> parameters)
        {
            AddValidEventHandler(new LSLLibraryEventSignature(name,parameters));
        }



        public virtual void AddValidLibraryFunction(LSLType returnType, string name, IEnumerable<LSLParameter> parameters)
        {
            AddValidLibraryFunction(new LSLLibraryFunctionSignature(returnType, name, parameters));
        }
    }

    [Serializable]
    public class LSLDuplicateSignatureException : Exception
    {
        public LSLDuplicateSignatureException()
        {
        }



        public LSLDuplicateSignatureException(string message)
            : base(message)
        {
        }



        public LSLDuplicateSignatureException(string message, Exception inner)
            : base(message, inner)
        {
        }



        protected LSLDuplicateSignatureException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}