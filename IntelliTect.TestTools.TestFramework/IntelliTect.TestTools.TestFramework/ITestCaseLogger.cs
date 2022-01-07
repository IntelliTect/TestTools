namespace IntelliTect.TestTools.TestFramework
{
    public interface ITestCaseLogger
    {
        // Probably need to handle this differently
        // Maybe a constructor sets the test case name?
        //string? TestCaseKey { get; set; }
        // OR, what about:
        TestCase TestCase { get; } // Populate this with a constructor, and then the only thing that will need to change is the current test block.
        string? CurrentTestBlock { get; set; }
        // Does it make sense to carry the Serializer here?
        //IObjectSerializer Serializer { get; set; }
        void Debug(string message);
        void Info(string message);
        void Critical(string message);
        void TestBlockInput(object input);
        void TestBlockOutput(object output);
    }
}
