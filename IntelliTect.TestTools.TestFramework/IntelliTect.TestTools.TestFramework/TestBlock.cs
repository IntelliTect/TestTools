using System;

namespace IntelliTect.TestTools.TestFramework
{
    public class TestBlock : ITestBlock
    {
        //public TestBlock(ILogger log)
        //{
        //    Log = log ?? new DebugLogger();
        //}

        public ILogger Log { get; } = new DebugLogger();
    }
}
