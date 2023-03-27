using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestData.Dependencies
{
    // BREAK OUT INTO OWN FILES AS NEEDED

    public class ExampleTestBlockWithExecuteArgForOwnType : TestBlock
    {
        public void Execute(ExampleImplementation input)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            Assert.Equal("Testing", input.Testing);
        }
    }

    public class ExampleTestBlockWithExecuteArgForInterface : TestBlock
    {
        public void Execute(IExampleDataInterface input)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            Assert.Equal("Testing", input.Testing);
        }
    }

    public class ExampleTestBlockWithPropertyForOwnType : TestBlock
    {
        public ExampleImplementation Input { get; set; } = new();

        public void Execute()
        {
            Assert.Equal("Testing", Input.Testing);
        }
    }

    public class ExampleTestBlockWithConstructorForOwnType : TestBlock
    {
        public ExampleTestBlockWithConstructorForOwnType(ExampleImplementation input)
        {
            Input = input;
        }

        public void Execute()
        {
            Assert.Equal("Testing", Input.Testing);
        }

        private ExampleImplementation Input { get; }
    }

    public class ExampleTestBlockForFactoryWithExecuteArg : TestBlock
    {
        public void Execute(ExampleImplementation input)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            Assert.Equal("TestingOverride", input.Testing);
        }
    }

    public class ExampleTestBlockForFactoryWithProperty : TestBlock
    {
        public ExampleImplementation Input { get; set; } = new();

        public void Execute()
        {
            Assert.Equal("TestingOverride", Input.Testing);
        }
    }

    public class ExampleTestBlockForFactoryWithConstructor : TestBlock
    {
        public ExampleTestBlockForFactoryWithConstructor(ExampleImplementation input)
        {
            Input = input;
        }

        public void Execute()
        {
            Assert.Equal("TestingOverride", Input.Testing);
        }

        private ExampleImplementation Input { get; }
    }

    public class ExampleTestBlockWithPropertyWithNoSetter : TestBlock
    {
        public string Input { get; } = "Not Set";

        public void Execute()
        {
            Assert.Equal("Not Set", Input);
        }
    }

    public class ExampleLoggerUsage : TestBlock
    {
        public void Execute(ITestCaseLogger log)
        {
            if (log is null) throw new ArgumentNullException(nameof(log));
            log.Debug("This should throw");
        }
    }

    public class ExampleFinallyBlock : TestBlock
    {
        public void Execute(bool result)
        {
            Assert.True(result, "This is an expected failure.");
        }
    }
}
