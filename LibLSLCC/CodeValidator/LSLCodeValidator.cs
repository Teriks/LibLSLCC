#region FileInfo
// 
// File: LSLCodeValidator.cs
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

using System;
using System.IO;
using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Nodes;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Visitor;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    /// LSLCodeValidator is responsible for building a rich syntax tree with <see cref="ILSLCompilationUnitNode"/> as the top node.
    /// <para>
    /// It preforms full front end syntax checking of the source code as the tree is built.
    /// 
    /// It delegates syntax errors and syntax warning invocations/information to the <see cref="ILSLValidatorServiceProvider.SyntaxErrorListener"/> and <see cref="ILSLValidatorServiceProvider.SyntaxWarningListener"/> instances 
    /// inside of the <see cref="ILSLValidatorServiceProvider"/> implementation assigned to the <see cref="ValidatorServices"/> property.
    /// </para>
    /// </summary>
    public class LSLCodeValidator : ILSLCodeValidator
    {
        private readonly LSLAntlrErrorHandler<int> _antlrLexerErrorHandler;
        private readonly LSLAntlrErrorHandler<IToken> _antlrParserErrorHandler;
        private readonly LSLCodeValidationVisitor _validationVisitor;

        /// <summary>
        /// Constructs an LSLCodeValidator using the given <see cref="ILSLValidatorServiceProvider"/>.
        /// </summary>
        /// <see cref="ILSLValidatorServiceProvider"/>
        /// <param name="validatorServices">The <see cref="ILSLValidatorServiceProvider"/> to use.</param>
        public LSLCodeValidator(ILSLValidatorServiceProvider validatorServices)
        {
            ValidatorServices = validatorServices;
            if (!validatorServices.IsComplete())
            {
                throw new ArgumentException("An ILSLValidatorServiceProvider property was null", "validatorServices");
            }

            _validationVisitor = new LSLCodeValidationVisitor(validatorServices);
            _antlrLexerErrorHandler = new LSLAntlrErrorHandler<int>(validatorServices.SyntaxErrorListener);
            _antlrParserErrorHandler = new LSLAntlrErrorHandler<IToken>(validatorServices.SyntaxErrorListener);
        }

        /// <summary>
        /// Construct an LSLCodeValidator using LSLDefaultValidatorServiceProvider for the ValidatorServices property.
        /// <see cref="LSLDefaultValidatorServiceProvider"/>
        /// </summary>
        public LSLCodeValidator()
        {
            var validationServices = new LSLDefaultValidatorServiceProvider();
            _validationVisitor = new LSLCodeValidationVisitor(validationServices);
            _antlrLexerErrorHandler = new LSLAntlrErrorHandler<int>(validationServices.SyntaxErrorListener);
            _antlrParserErrorHandler = new LSLAntlrErrorHandler<IToken>(validationServices.SyntaxErrorListener);
        }

        /// <summary>
        /// <see cref="ILSLValidatorServiceProvider"/> that provides several components to the validator.
        /// Among them are the <see cref="ILSLValidatorServiceProvider.SyntaxErrorListener"/> and <see cref="ILSLValidatorServiceProvider.SyntaxWarningListener"/> implementations.
        /// </summary>
        public ILSLValidatorServiceProvider ValidatorServices { get; private set; }

        /// <summary>
        /// Set to true if the last call to validate revealed syntax errors and returned null
        /// </summary>
        public bool HasSyntaxErrors { get; private set; }


        /// <summary>
        /// Set to true if the last call to validate generated syntax warnings
        /// </summary>
        public bool HasSyntaxWarnings { get; set; }


        /// <summary>
        /// Validates the code content of a stream and returns the top of the compilation unit syntax tree as a
        /// <see cref="LSLCompilationUnitNode"/> object, if parsing resulted in syntax errors the result will be null
        /// </summary>
        /// <param name="stream">The TextReader to parse code from</param>
        /// <returns>Top level node of an LSL syntax tree</returns>
        public ILSLCompilationUnitNode Validate(TextReader stream)
        {
            HasSyntaxErrors = false;
            var inputStream = new AntlrInputStream(stream);

            var lexer = new LSLLexer(inputStream);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(_antlrLexerErrorHandler);

            var tokenStream = new CommonTokenStream(lexer);

            var parser = new LSLParser(tokenStream);

            parser.RemoveErrorListeners();

            parser.AddErrorListener(_antlrParserErrorHandler);

            var parseTree = parser.compilationUnit();


            if (parser.NumberOfSyntaxErrors > 0 || _antlrLexerErrorHandler.HasErrors)
            {
                HasSyntaxErrors = true;
                return null;
            }

            try
            {
                var tree = _validationVisitor.ValidateAndBuildTree(parseTree);

                if (_validationVisitor.HasSyntaxWarnings)
                {
                    HasSyntaxWarnings = true;
                }

                if (tree.HasErrors)
                {
                    HasSyntaxErrors = true;
                    return null;
                }

                tree.Comments = lexer.Comments;

                return tree;

            }
            finally
            {
                _validationVisitor.Reset();
            }
        }
    }
}