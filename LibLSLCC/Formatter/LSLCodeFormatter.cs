using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.Formatter.Visitor;

namespace LibLSLCC.Formatter
{
    public class LSLCodeFormatter
    {
        public void Format(string sourceReference, ILSLCompilationUnitNode node, TextWriter writer,
            bool closeStream = true)
        {
            var formatter = new LSLCodeFormatterVisitor();
            formatter.WriteAndFlush(sourceReference, node, writer, closeStream);
        }
    }
}
