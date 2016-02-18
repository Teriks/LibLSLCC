using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;

namespace LibLSLCC.CodeValidator.Primitives
{
    /// <summary>
    /// Represents a basic LSL source code token.
    /// </summary>
    public class LSLToken
    {
        internal LSLToken(IToken token)
        {
            Text = token.Text;
            Location = new LSLSourceCodeRange(token);
        }


        /// <summary>
        /// Construct a source code token from the given text and location.
        /// </summary>
        /// <param name="text">The tokens text.</param>
        /// <param name="location">The tokens location.</param>
        public LSLToken(string text, LSLSourceCodeRange location)
        {
            Text = text;
            Location = location;
        }

        /// <summary>
        /// The tokens text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The tokens location.
        /// </summary>
        public LSLSourceCodeRange Location { get; private set; }


        /// <summary>
        /// Create a deep clone of this token.
        /// </summary>
        /// <returns>A deep clone of this token.</returns>
        public LSLToken Clone()
        {
            return new LSLToken(Text, Location.Clone());
        }
    }
}
