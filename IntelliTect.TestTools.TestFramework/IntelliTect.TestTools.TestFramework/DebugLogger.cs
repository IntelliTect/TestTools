namespace IntelliTect.TestTools.TestFramework
{
    public class DebugLogger : ILogger
    {
        public string TestCaseKey { get; set; }
        public string CurrentTestBlock { get; set; }
        public IObjectSerializer Serializer { get; set; }

        public void Debug(string message)
        {
            LogToDebug($"{TestCaseKey} - {CurrentTestBlock} - Debug: {message}");
        }

        public void Critical(string message)
        {
            LogToDebug($"{TestCaseKey} - {CurrentTestBlock} - Error: {message}");
        }

        public void Info(string message)
        {
            LogToDebug($"{TestCaseKey} - {CurrentTestBlock} - Info: {message}");
        }

        public void TestBlockInput(object input)
        {
            string inputString = Serializer.Serialize(input);
            LogToDebug($"{TestCaseKey} - {CurrentTestBlock} - Input arguments: {inputString}");
        }

        public void TestBlockOutput(object output)
        {
            string outputString = Serializer.Serialize(output);
            LogToDebug($"{TestCaseKey} - {CurrentTestBlock} - Output returns: {outputString}");
        }

        private void LogToDebug(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
