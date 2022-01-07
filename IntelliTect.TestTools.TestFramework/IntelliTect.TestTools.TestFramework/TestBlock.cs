namespace IntelliTect.TestTools.TestFramework
{
    public class TestBlock : ITestBlock
    {
        // Would it make more sense to have this as a constructor?
        // Would require more boilerplate code per testblock BUT it would be very clear logging is available.
        // I suppose if we have it as a ctor people can always implement their own.
        //public TestBlock(ILogger log)
        //{
        //    Log = log ?? new DebugLogger();
        //}

        // NEED TO TEST HOW THIS WORKS WHEN THE LOGGER IS REMOVED!
        public ITestCaseLogger? Log { get; set; }
    }
}