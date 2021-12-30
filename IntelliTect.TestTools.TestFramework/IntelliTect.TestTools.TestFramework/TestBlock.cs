using System;

namespace IntelliTect.TestTools.TestFramework
{
    public class TestBlock : ITestBlock
    {
        //public TestBlock(ILogger log)
        //{
        //    Log = log ?? new DebugLogger();
        //}

        // NEED TO TEST HOW THIS WORKS WHEN THE LOGGER IS REMOVED!
        public ITestCaseLogger? Log { get; }
    }
}