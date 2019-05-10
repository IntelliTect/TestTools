using System.Diagnostics;

namespace IntelliTect.TestTools.TestFramework
{
    public class Log
    {
        public void Info(string message)
        {
            LogToDebug(message);
        }

        private void LogToDebug(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
