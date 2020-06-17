namespace IntelliTect.TestTools.TestFramework
{
    public interface ILogger
    {
        // Probably need to handle this differently
        // Maybe a constructor sets the test case name?
        string TestCaseKey { get; set; }
        string CurrentTestBlock { get; set; }
        void Debug(string message);
        void Info(string message);

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Naming", 
            "CA1716:Identifiers should not match keywords",
            Justification = "Deferring to next major rev.")]
        void Error(string message);
        void TestBlockInput(string input);
        void TestBlockOutput(string output);
    }
}
