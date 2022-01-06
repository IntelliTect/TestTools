using IntelliTect.TestTools.TestFramework.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestBuilderTests
{
    public class LoggerTests
    {
        [Fact]
        public void DefaultLoggerIsAddedOnCreate()
        {
            TestCase tc = new TestBuilder()
                .AddTestBlock<DefaultLogBlock>()
                .Build();

            tc.Execute();
        }

        [Fact]
        public void RemovedLoggerDoesNotThrowWhenAttemptingToActivateProp()
        {
            // Why did this pass?
            // I would have expected it to fail in TestCase.ActivateObject
            TestCase tc = new TestBuilder()
                .RemoveLogger()
                .AddTestBlock<RemovedLogBlockProp>()
                .Build();

            tc.Execute();
        }

        [Fact]
        public void RemovedLoggerDoesNotThrowWhenAttemptingToActivateCtor()
        {
            // Why did this pass?
            // I would have expected it to fail in TestCase.ActivateObject
            TestCase tc = new TestBuilder()
                .RemoveLogger()
                .AddTestBlock<RemovedLogBlockCtor>()
                .Build();

            tc.Execute();
        }

        [Fact]
        public void CustomLoggerAddsWithoutError()
        {
            TestCase tc = new TestBuilder()
                .AddLogger<CustomLogger>()
                .AddTestBlock<CustomLogBlock>()
                .Build();

            tc.Execute();
        }
    }

    public class DefaultLogBlock : TestBlock
    {
        public void Execute()
        {
            Assert.NotNull(Log);
            Assert.Equal(typeof(DebugLogger), Log!.GetType());
        }
    }

    public class RemovedLogBlockProp : TestBlock
    {
        public void Execute()
        {
            Assert.Null(Log);
        }
    }

    public class RemovedLogBlockCtor : TestBlock
    {
        public RemovedLogBlockCtor(ITestCaseLogger? log)
        {
            Log = log;
        }

        public void Execute()
        {
            Assert.Null(Log);
        }
    }

    public class CustomLogBlock : TestBlock
    {
        public void Execute()
        {
            Assert.NotNull(Log);
            Assert.Equal(typeof(CustomLogger), Log!.GetType());
            if(Log is CustomLogger cl)
            {
                Assert.True(cl.Invocations > 0);
            }
        }
    }

    public class CustomLogger : ITestCaseLogger
    {
        public CustomLogger(TestCase tc)
        {
            TestCase = tc;
        }
        public TestCase TestCase { get; }

        public string? CurrentTestBlock { get; set; }

        public int Invocations { get; set; }

        public void Critical(string message)
        {
            Invocations++;
        }

        public void Debug(string message)
        {
            Invocations++;
        }

        public void Info(string message)
        {
            Invocations++;
        }

        public void TestBlockInput(object input)
        {
            Invocations++;
        }

        public void TestBlockOutput(object output)
        {
            Invocations++;
        }
    }
}
