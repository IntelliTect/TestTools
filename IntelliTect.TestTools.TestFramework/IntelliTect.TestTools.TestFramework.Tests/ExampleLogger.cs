using System;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class ThrowingLogger : ITestCaseLogger
    {
        public ThrowingLogger(TestCase tc)
        {
            TestCase = tc;
        }

        public string? TestCaseKey { get; set; }
        public string? CurrentTestBlock { get; set; }

        public TestCase TestCase { get; }

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

        public void TestBlockInput(object input)
        {
            throw new NotImplementedException();
        }

        public void TestBlockOutput(object output)
        {
            throw new NotImplementedException();
        }
    }
}
