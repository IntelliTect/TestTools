namespace IntelliTect.TestTools.TestFramework
{
    public class Log : ILogger
    {
        public void Debug(string message)
        {
            LogToDebug($"Debug: {message}");
        }

        public void Error(string message)
        {
            LogToDebug($"Error: {message}");
        }

        public void Info(string message)
        {
            LogToDebug($"Info: {message}");
        }

        private void LogToDebug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
