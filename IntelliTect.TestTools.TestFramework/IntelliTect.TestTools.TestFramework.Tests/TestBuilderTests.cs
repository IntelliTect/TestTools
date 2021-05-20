using System;
using Xunit;
using Xunit.Sdk;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class TestBuilderTests
    {
        [Fact]
        [TestCase]
        public void FetchByObjectInstanceForExecuteArg()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .Build()
                //.ExecuteTestCase()
                ;
        }

        [Fact]
        public void FetchByObjectInstanceForTestBlockProperty()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithProperty>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByObjectInstanceForTestBlockConstructor()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithConstructor>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByObjectInstanceForMultipleDependencies()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance("Testing")
                .AddDependencyInstance(1234)
                .AddTestBlock<ExampleTestBlockWithMultipleDependencies>()
                .ExecuteTestCase();
        }

        [Fact]
        public void PassExecuteArgumentsViaAddTestBlockParams()
        {
            TestBuilder builder = new();
            builder
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void MismatchedCountAddTestBlockParamsAndExecuteArgsFails()
        {
            TestBuilder builder = new();
            builder.AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing", "Testing2");

            Assert.Throws<TestCaseException>(() => builder.ExecuteTestCase());
        }

        [Fact]
        public void MismatchedTypeAddTestBlockParamsAndExecuteArgsFails()
        {
            TestBuilder builder = new();
            builder.AddTestBlock<ExampleTestBlockWithExecuteArg>(1234);

            Assert.Throws<TestCaseException>(() => builder.ExecuteTestCase());
        }

        // This test probably isn't necessary. This is MS DI out-of-the-box functionality
        [Fact]
        public void FetchByServiceForConstructor()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<ExampleDataThing>()
                .AddTestBlock<ExampleTestBlockWithConstructorForOwnType>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByServiceForProperty()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<ExampleDataThing>()
                .AddTestBlock<ExampleTestBlockWithPropertyForOwnType>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByServiceForExecuteArg()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<ExampleDataThing>()
                .AddTestBlock<ExampleTestBlockWithExecuteArgForOwnType>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByImplementationForExecuteArg()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance<IExampleDataInterface>(new ExampleDataThing())
                .AddTestBlock<ExampleTestBlockWithExecuteArgForInterface>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByImplementationAndTypeForExecuteArg()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<IExampleDataInterface, ExampleDataThing>()
                .AddTestBlock<ExampleTestBlockWithExecuteArgForInterface>()
                .ExecuteTestCase();
        }

        // This test probably isn't necessary. This is MS DI out-of-the-box functionality
        [Fact]
        public void FetchByFactoryForConstructor()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<ExampleDataThing>(new ExampleDataThingFactory().ExampleDataThing)
                .AddTestBlock<ExampleTestBlockForFactoryWithConstructor>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByFactoryForProperty()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<ExampleDataThing>(new ExampleDataThingFactory().ExampleDataThing)
                .AddTestBlock<ExampleTestBlockForFactoryWithProperty>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByFactoryForExecuteArg()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<ExampleDataThing>(new ExampleDataThingFactory().ExampleDataThing)
                .AddTestBlock<ExampleTestBlockForFactoryWithExecuteArg>()
                .ExecuteTestCase();
        }

        // This test probably isn't necessary. This is MS DI out-of-the-box functionality
        [Fact]
        public void AddTwoServicesOfSameTypeToServiceAndFetch()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<ExampleDataThing>()
                .AddDependencyService<ExampleDataThing>()
                .AddTestBlock<ExampleTestBlockWithExecuteArgForOwnType>()
                .ExecuteTestCase();
        }

        // This test probably isn't necessary. This is MS DI out-of-the-box functionality
        [Fact]
        public void AddTwoInstancesOfSameTypeToServiceAndFetch()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance(new ExampleDataThing { Testing = "Testing2" })
                .AddDependencyInstance(new ExampleDataThing { Testing = "Testing" })
                .AddTestBlock<ExampleTestBlockWithExecuteArgForOwnType>()
                .ExecuteTestCase();
        }

        [Fact]
        public void TestBlockWithPropertyWithNoSetterDoesNotThrow()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithPropertyWithNoSetter>()
                .ExecuteTestCase();
        }

        [Fact]
        public void TestBlockWithMultipleExecuteMethodsThrows()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithMultipleExecuteMethods>();

            Assert.Throws<TestCaseException>(() => builder.ExecuteTestCase());
        }

        [Fact]
        public void TestBlockThatFailsThrowsCorrectException()
        {
            TestBuilder builder = new();
            builder
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Bad Value");

            try
            {
                builder.ExecuteTestCase();
            }
            catch (TestCaseException ex)
            {
                Assert.Equal(typeof(EqualException), ex.InnerException.GetType());
            }
        }

        [Fact]
        public void AddLoggerReturnsCorrectLogger()
        {
            TestBuilder builder = new();
            builder
                .AddLogger<ExampleLogger>()
                .AddTestBlock<ExampleLoggerUsage>();

            Assert.Throws<NotImplementedException>(() => builder.ExecuteTestCase());
        }

        [Fact]
        public void AddFinallyBlockThrowsExpectedException()
        {
            TestBuilder builder = new();
            builder
                .AddTestBlock<ExampleTestBlockWithReturn>(false)
                .AddFinallyBlock<ExampleFinallyBlock>()
                .ExecuteTestCase();
        }

        // Actually... this probably shouldn't throw since it's a "finally" block meant to clean stuff up
        // Figure out the right behavior and fix test before moving further with finally blocks
        [Fact]
        public void AddFinallyBlockDoesNotThrowIfExceptionOccursInFinally()
        {
            TestBuilder builder = new();
            builder
                .AddTestBlock<ExampleTestBlockWithReturn>(true)
                .AddFinallyBlock<ExampleFinallyBlock>()
                .ExecuteTestCase();
        }

        // How do we verify this is working correctly?
        [Fact]
        public void AddFinallyBlockExecutesAfterException()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance(true)
                .AddTestBlock<ExampleTestBlockWithMultipleExecuteMethods>()
                .AddFinallyBlock<ExampleFinallyBlock>();

            Assert.Throws<TestCaseException>(() => builder.ExecuteTestCase());
        }

        [Fact]
        public void OverridingLoggerDoesNotThrow()
        {
            TestBuilder builder = new();
            builder
                .RemoveLogger()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void RemovingLoggerTwiceDoesNotThrow()
        {
            TestBuilder builder = new();
            builder
                .RemoveLogger()
                .RemoveLogger()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void AddingLoggerThanRemovingDoesNotThrow()
        {
            TestBuilder builder = new();
            builder
                .AddLogger<ExampleLogger>()
                .RemoveLogger()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void OverrideTestCaseNameWithConstructor()
        {
            TestBuilder builder = new();
            builder
                .AddLogger<ExampleLogger>()
                .RemoveLogger()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void OverrideTestCaseNameWithMethod()
        {
            TestBuilder builder = new();
            builder
                .OverrideTestCaseKey()
                .AddLogger<ExampleLogger>()
                .RemoveLogger()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void OverrideTestCaseNameWithMethodOverride()
        {
            TestBuilder builder = new();
            builder
                //.OverrideTestCaseKey("Testing")
                .AddLogger<ExampleLogger>()
                .RemoveLogger()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void PropertyWithNoMatchingTypeInDiThrowsInvalidOperation()
        {
            TestBuilder builder = new();
            builder.AddTestBlock<ExampleTestBlockWithProperty>();

            Exception ex = Assert.Throws<TestCaseException>(() => builder.ExecuteTestCase());
            Assert.Equal(typeof(InvalidOperationException), ex.InnerException.GetType());
        }
    }

    public class ExampleTestBlockWithExecuteArg : TestBlock
    {
        public void Execute(string input)
        {
            Assert.Equal("Testing", input);
        }
    }

    public class ExampleTestBlockWithProperty : TestBlock
    {
        public string Input { get; set; }

        public void Execute()
        {
            Assert.Equal("Testing", Input);
        }
    }

    public class ExampleTestBlockWithConstructor : TestBlock
    {
        public ExampleTestBlockWithConstructor(string input)
        {
            Input = input;
        }

        public void Execute()
        {
            Assert.Equal("Testing", Input);
        }

        private string Input { get; }
    }

    public class ExampleTestBlockWithMultipleDependencies : TestBlock
    {
        public string InputText { get; set; }

        public void Execute(int inputNumber)
        {
            Assert.Equal("Testing", InputText);
            Assert.Equal(1234, inputNumber);
        }
    }

    public class ExampleTestBlockWithExecuteArgForOwnType : TestBlock
    {
        public void Execute(ExampleDataThing input)
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
        public ExampleDataThing Input { get; set; }

        public void Execute()
        {
            Assert.Equal("Testing", Input.Testing);
        }
    }

    public class ExampleTestBlockWithConstructorForOwnType : TestBlock
    {
        public ExampleTestBlockWithConstructorForOwnType(ExampleDataThing input)
        {
            Input = input;
        }

        public void Execute()
        {
            Assert.Equal("Testing", Input.Testing);
        }

        private ExampleDataThing Input { get; }
    }

    public class ExampleTestBlockForFactoryWithExecuteArg : TestBlock
    {
        public void Execute(ExampleDataThing input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            Assert.Equal("TestingOverride", input.Testing);
        }
    }

    public class ExampleTestBlockForFactoryWithProperty : TestBlock
    {
        public ExampleDataThing Input { get; set; }

        public void Execute()
        {
            Assert.Equal("TestingOverride", Input.Testing);
        }
    }

    public class ExampleTestBlockForFactoryWithConstructor : TestBlock
    {
        public ExampleTestBlockForFactoryWithConstructor(ExampleDataThing input)
        {
            Input = input;
        }

        public void Execute()
        {
            Assert.Equal("TestingOverride", Input.Testing);
        }

        private ExampleDataThing Input { get; }
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

    public class ExampleTestBlockWithReturn : TestBlock
    {
        public bool Execute(bool valueToReturn)
        {
            return !valueToReturn;
        }
    }

    public class ExampleFinallyBlock : TestBlock
    {
        public void Execute(bool result)
        {
            Assert.True(result, "Finally block did not receive correct input");
        }
    }

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
