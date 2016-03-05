using LibLSLCC.Collections;

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    /// Read only interface for code segments.  <para/>
    /// Code segments are used to describe a sequential set of code statement nodes that exist in the same scope.
    /// </summary>
    public interface ILSLCodeSegment
    {
        /// <summary>
        ///     The <see cref="ILSLReadOnlyCodeStatement" /> at the end of the code segment.
        /// </summary>
        ILSLReadOnlyCodeStatement EndNode { get; }


        /// <summary>
        ///     The source code range that encompasses all <see cref="ILSLReadOnlyCodeStatement" /> objects in the <see cref="LSLCodeSegment"/>.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> for any added statements, this property will be <c>null</c>.
        /// </remarks>
        LSLSourceCodeRange SourceRange { get; }

        /// <summary>
        /// <c>true</c> if <see cref="SourceRange"/> could be calculated for this segment and is non <c>null</c>.
        /// </summary>
        bool SourceRangeAvailable { get; }


        /// <summary>
        ///     The <see cref="ILSLReadOnlyCodeStatement" /> at the start of the code segment.
        /// </summary>
        ILSLReadOnlyCodeStatement StartNode { get; }

        /// <summary>
        ///     All <see cref="ILSLReadOnlyCodeStatement" /> in the code segment, in order of definition.
        /// </summary>
        IReadOnlyGenericArray<ILSLReadOnlyCodeStatement> StatementNodes { get; }
    }
}