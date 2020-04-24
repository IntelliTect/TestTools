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
        /// Whether VT100 color code characters should be ignored.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Whether differences in line ending styles should be ignored.
        /// </summary>
        NormalizeLineEndings = 1,
        
        /// <summary>
        /// No normalization
        /// </summary>
        StripVt100 = 2,
    }
}