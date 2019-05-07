using System.Diagnostics;

namespace IntelliTect.TestTools.TestFramework
{
    public class Log
    {
        public void Info(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
