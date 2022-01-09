namespace IntelliTect.TestTools.TestFramework
{
    public interface ITestCaseLogger
    {
        TestCase TestCase { get; }
        string? CurrentTestBlock { get; set; }
        void Debug(string message);
        void Info(string message);
        void Critical(string message);
        void TestBlockInput(object input);
        void TestBlockOutput(object output);
    }
}
