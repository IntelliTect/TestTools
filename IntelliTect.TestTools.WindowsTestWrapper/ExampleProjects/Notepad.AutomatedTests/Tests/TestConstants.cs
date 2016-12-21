using System;

namespace Notepad.AutomatedTests.Tests
{
    public partial class BaseTest
    {
        public static readonly string SaveLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public const string AlphaMin = "a";
        public const string NumericMin = "1";
        public const string Empty = "";
    }
}