namespace IntelliTect.TestTools.TestFramework
{
    public class Log : ILogger
    {
        public void Debug(string testCase, string testBlock, string message)
        {
            LogToDebug($"{testCase} - {testBlock} - Debug: {message}");
        }

        public void Error(string testCase, string testBlock, string message)
        {
            LogToDebug($"{testCase} - {testBlock} - Error: {message}");
        }

        public void Info(string testCase, string testBlock, string message)
        {
            LogToDebug($"{testCase} - {testBlock} - Info: {message}");
        }

        public void TestBlockInput(string testCase, string testBlock, string input)
        {
            LogToDebug($"{testCase} - {testBlock} - Input arguments: {input}");
        }

        public void TestBlockOutput(string testCase, string testBlock, string output)
        {
            LogToDebug($"{testCase} - {testBlock} - Output returns: {output}");
        }

        private void LogToDebug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
