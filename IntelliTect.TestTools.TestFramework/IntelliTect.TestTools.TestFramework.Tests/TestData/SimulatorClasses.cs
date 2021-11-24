using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestData
{
    // BREAK OUT INTO OWN FILES AS NEEDED
    
    public class ExampleTestBlockWithExecuteArgForOwnType : TestBlock
    {
        public void Execute(ExampleInterface input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            Assert.Equal("Testing", input.Testing);
        }
    }

    public class ExampleTestBlockWithExecuteArgForInterface : TestBlock
    {
        public void Execute(IExampleDataInterface input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            Assert.Equal("Testing", input.Testing);
        }
    }

    public class ExampleTestBlockWithPropertyForOwnType : TestBlock
    {
        public ExampleInterface Input { get; set; }

        public void Execute()
        {
            Assert.Equal("Testing", Input.Testing);
        }
    }

    public class ExampleTestBlockWithConstructorForOwnType : TestBlock
    {
        public ExampleTestBlockWithConstructorForOwnType(ExampleInterface input)
        {
            Input = input;
        }

        public void Execute()
        {
            Assert.Equal("Testing", Input.Testing);
        }

        private ExampleInterface Input { get; }
    }

    public class ExampleTestBlockForFactoryWithExecuteArg : TestBlock
    {
        public void Execute(ExampleInterface input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            Assert.Equal("TestingOverride", input.Testing);
        }
    }

    public class ExampleTestBlockForFactoryWithProperty : TestBlock
    {
        public ExampleInterface Input { get; set; }

        public void Execute()
        {
            Assert.Equal("TestingOverride", Input.Testing);
        }
    }

    public class ExampleTestBlockForFactoryWithConstructor : TestBlock
    {
        public ExampleTestBlockForFactoryWithConstructor(ExampleInterface input)
        {
            Input = input;
        }

        public void Execute()
        {
            Assert.Equal("TestingOverride", Input.Testing);
        }

        private ExampleInterface Input { get; }
    }

    public class ExampleTestBlockWithPropertyWithNoSetter : TestBlock
    {
        public string Input { get; }

        public void Execute()
        {
            Assert.Null(Input);
        }
    }

    public class ExampleTestBlockWithMultipleExecuteMethods : TestBlock
    {
        public void Execute()
        {
            Assert.True(true);
        }

        public void Execute(string input)
        {
            Assert.Equal("Tetsing", input);
        }
    }

    public class ExampleLoggerUsage : TestBlock
    {
        public void Execute(ILogger log)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            log.Debug("This should throw");
        }
    }

    

    public class ExampleFinallyBlock : TestBlock
    {
        public void Execute(bool result)
        {
            Assert.True(result, "Finally block did not receive correct input");
        }
    }
}
