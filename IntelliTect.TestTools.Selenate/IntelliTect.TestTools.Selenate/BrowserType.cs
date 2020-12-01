using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// Enum representing all supported browser types of <see cref="Browser"/>
    /// </summary>
    public enum BrowserType
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Chrome,
        HeadlessChrome,
        InternetExplorer,
        Firefox,
        Edge
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
