namespace IntelliTect.TestTools.TestFramework
{
    public interface ILogger
    {
        void Debug(string testCase, string testBlock, string message);
        void Info(string testCase, string testBlock, string message);
        void Error(string testCase, string testBlock, string message);
        void TestBlockInput(string testCase, string testBlock, string input);
        void TestBlockOutput(string testCase, string testBlock, string output);
    }
}
