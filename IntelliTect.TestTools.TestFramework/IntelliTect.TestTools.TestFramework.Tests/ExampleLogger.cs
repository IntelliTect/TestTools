using System;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class ExampleLogger : ITestCaseLogger
    {
        public ExampleLogger(TestCase tc, IObjectSerializer os)
        {
            CurrentTestCase = tc;
            Serializer = os;
        }

        public string? TestCaseKey { get; set; }
        public string? CurrentTestBlock { get; set; }
        public IObjectSerializer Serializer { get; set; }

        public TestCase CurrentTestCase { get; }

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
