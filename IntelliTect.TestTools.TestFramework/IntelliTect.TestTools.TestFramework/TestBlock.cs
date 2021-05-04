using System;

namespace IntelliTect.TestTools.TestFramework
{
    public class TestBlock : ITestBlock
    {
        public TestBlock(ILogger log)
        {
            Log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public ILogger Log { get; }
    }
}
