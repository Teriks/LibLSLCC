﻿namespace LibLSLCC.CodeValidator.Primitives
{

    public enum LSLCommentType
    {
        Block,
        SingleLine,
    }
    public class LSLComment
    {
        public string Text { get; set; }
        public LSLSourceCodeRange SourceCodeRange { get; set; }

        public LSLCommentType Type { get; set; }
    }
}