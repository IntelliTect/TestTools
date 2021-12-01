namespace IntelliTect.TestTools.TestFramework
{
    public interface ILogger
    {
        // Probably need to handle this differently
        // Maybe a constructor sets the test case name?
        string TestCaseKey { get; set; }
        string CurrentTestBlock { get; set; }
        // Does it make sense to carry the Serializer here?
        IObjectSerializer Serializer { get; set; }
        void Debug(string message);
        void Info(string message);
        void Critical(string message);
        void TestBlockInput(object input);
        void TestBlockOutput(object output);
    }
}
