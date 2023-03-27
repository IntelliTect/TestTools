using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestBuilderTests
{
    public class LoggerTests
    {
        [Fact]
        public void DefaultLoggerIsAddedOnCreate()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddTestBlock<DefaultLogBlock>()
                .Build();

            // Act / Assert
            tc.Execute();
        }

        [Fact]
        public void RemovedLoggerDoesNotThrowWhenAttemptingToActivateProp()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .RemoveLogger()
                .AddTestBlock<RemovedLogBlockProp>()
                .Build();

            // Act / Assert
            tc.Execute();
        }

        [Fact]
        public void RemovedLoggerDoesNotThrowWhenAttemptingToActivateCtor()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .RemoveLogger()
                .AddTestBlock<RemovedLogBlockCtor>()
                .Build();

            // Act / Assert
            tc.Execute();
        }

        [Fact]
        public void RemovedLoggerDoesNotThrowWhenAttemptingToActivateExecuteArg()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .RemoveLogger()
                .AddTestBlock<RemovedLogBlockExecuteArg>()
                .Build();

            // Act / Assert
            tc.Execute();
        }

        [Fact]
        public void CustomLoggerAddsWithoutError()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddLogger<CustomLogger>()
                .AddTestBlock<CustomLogBlock>()
                .Build();

            // Act / Assert
            tc.Execute();
        }

        [Fact]
        public void RemovingLoggerTwiceDoesNotThrow()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .RemoveLogger()
                .RemoveLogger()
                .AddTestBlock<RemovedLogBlockProp>()
                .Build();

            // Act / Assert
            tc.Execute();
        }

        [Fact]
        public void AddingLoggerThanRemovingDoesNotThrow()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddLogger<CustomLogger>()
                .RemoveLogger()
                .AddTestBlock<RemovedLogBlockProp>()
                .Build();

            // Act / Assert
            tc.Execute();
        }
    }

    public class DefaultLogBlock : TestBlock
    {
        public void Execute()
        {
            Assert.NotNull(Log);
            Assert.IsType<DebugLogger>(Log);
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

    public class RemovedLogBlockExecuteArg : TestBlock
    {
        public void Execute(ITestCaseLogger? log)
        {
            Assert.Null(log);
        }
    }

    public class CustomLogBlock : TestBlock
    {
        public void Execute()
        {
            Assert.NotNull(Log);
            Assert.IsType<CustomLogger>(Log);
            CustomLogger cl = (CustomLogger)Log;
            {
                Assert.True(cl.Invoked);
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

        public bool Invoked { get; set; }

        public void Critical(string message)
        {
            Invoked = true;
        }

        public void Debug(string message)
        {
            Invoked = true;
        }

        public void Info(string message)
        {
            Invoked = true;
        }

        public void TestBlockInput(object input)
        {
            Invoked = true;
        }

        public void TestBlockOutput(object output)
        {
            Invoked = true;
        }
    }
}
