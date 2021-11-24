using System;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class ExampleLogger : ILogger
    {
        public string TestCaseKey { get; set; }
        public string CurrentTestBlock { get; set; }

        public void Debug(string message)
        {
            throw new NotImplementedException();
        }

        public void Critical(string message)
        {
            throw new NotImplementedException();
        }

        public void Info(string message)
        {
            throw new NotImplementedException();
        }

        public void TestBlockInput(string input)
        {
            throw new NotImplementedException();
        }

        public void TestBlockOutput(string output)
        {
            throw new NotImplementedException();
        }
    }
}
