using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.Primitives
{
    /// <summary>
    ///     A code segment type for housing a range of statement nodes that are considered to be dead.
    ///     a DeadCodeType enum property is provided to describe what caused the code to be dead
    /// </summary>
    public class LSLDeadCodeSegment : LSLCodeSegment
    {



        public LSLDeadCodeSegment(LSLDeadCodeType deadCodeType)
        {
            DeadCodeType = deadCodeType;
        }



        public LSLDeadCodeType DeadCodeType { get; private set; }
    }
}