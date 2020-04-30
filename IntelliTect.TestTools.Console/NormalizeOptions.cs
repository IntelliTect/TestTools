using System;

namespace IntelliTect.TestTools.Console
{
    /// <summary>
    /// Provides available options for normalizing expected input and output
    /// </summary>
    [Flags]
    public enum NormalizeOptions
    {
        /// <summary>
        /// No normalization.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Apply all available normalization (currently NormalizeLineEndings and StripAnsiEscapeCodes).
        /// </summary>
        Default = NormalizeLineEndings | StripAnsiEscapeCodes,

        /// <summary>
        /// Whether differences in line ending styles should be ignored.
        /// </summary>
        NormalizeLineEndings = 1,
        
        /// <summary>
        /// Whether ansi color code characters should be ignored.
        /// </summary>
        StripAnsiEscapeCodes = 2,
    }
}