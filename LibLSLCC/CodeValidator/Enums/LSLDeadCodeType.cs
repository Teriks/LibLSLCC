namespace LibLSLCC.CodeValidator.Enums
{
    /// <summary>
    ///     Describes the cause of dead code
    /// </summary>
    public enum LSLDeadCodeType
    {
        None = 0,
        AfterJumpOutOfScope = 1,
        AfterJumpLoopForever = 2,
        JumpOverCode = 3,
        AfterReturnPath = 4
    }
}