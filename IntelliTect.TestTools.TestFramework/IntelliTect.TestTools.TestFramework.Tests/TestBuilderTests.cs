using System;
using Xunit;
using Xunit.Sdk;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class TestBuilderTests
    {
        [Fact]
        public void FetchByObjectInstanceForExecuteArg()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByObjectInstanceForTestBlockProperty()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithProperty>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByObjectInstanceForTestBlockConstructor()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithConstructor>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByObjectInstanceForMultipleDependencies()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyInstance("Testing")
                .AddDependencyInstance(1234)
                .AddTestBlock<ExampleTestBlockWithMultipleDependencies>()
                .ExecuteTestCase();
        }

        [Fact]
        public void PassExecuteArgumentsViaAddTestBlockParams()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void MismatchedCountAddTestBlockParamsAndExecuteArgsFails()
        {
            TestBuilder builder = new TestBuilder();
            builder.AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing", "Testing2");

            Assert.Throws<InvalidOperationException>(() => builder.ExecuteTestCase());
        }

        [Fact]
        public void MismatchedTypeAddTestBlockParamsAndExecuteArgsFails()
        {
            TestBuilder builder = new TestBuilder();
            builder.AddTestBlock<ExampleTestBlockWithExecuteArg>(1234);

            Assert.Throws<ArgumentException>(() => builder.ExecuteTestCase());
        }

        // This test probably isn't necessary. This is Autofac out-of-the-box functionality
        [Fact]
        public void FetchByServiceForConstructor()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyService<ExampleDataThing>()
                .AddTestBlock<ExampleTestBlockWithConstructorForOwnType>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByServiceForProperty()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyService<ExampleDataThing>()
                .AddTestBlock<ExampleTestBlockWithPropertyForOwnType>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByServiceForExecuteArg()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyService<ExampleDataThing>()
                .AddTestBlock<ExampleTestBlockWithExecuteArgForOwnType>()
                .ExecuteTestCase();
        }

        // This test probably isn't necessary. This is Autofac out-of-the-box functionality
        [Fact]
        public void FetchByFactoryForConstructor()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyService<ExampleDataThing>(new ExampleDataThingFactory().ExampleDataThing)
                .AddTestBlock<ExampleTestBlockForFactoryWithConstructor>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByFactoryForProperty()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyService<ExampleDataThing>(new ExampleDataThingFactory().ExampleDataThing)
                .AddTestBlock<ExampleTestBlockForFactoryWithProperty>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByFactoryForExecuteArg()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyService<ExampleDataThing>(new ExampleDataThingFactory().ExampleDataThing)
                .AddTestBlock<ExampleTestBlockForFactoryWithExecuteArg>()
                .ExecuteTestCase();
        }

        // This test probably isn't necessary. This is Autofac out-of-the-box functionality
        [Fact]
        public void AddTwoServicesOfSameTypeToServiceAndFetch()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyService<ExampleDataThing>()
                .AddDependencyService<ExampleDataThing>()
                .AddTestBlock<ExampleTestBlockWithExecuteArgForOwnType>()
                .ExecuteTestCase();
        }

        // This test probably isn't necessary. This is Autofac out-of-the-box functionality
        [Fact]
        public void AddTwoInstancesOfSameTypeToServiceAndFetch()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyInstance(new ExampleDataThing { Testing = "Testing2" })
                .AddDependencyInstance(new ExampleDataThing { Testing = "Testing" })
                .AddTestBlock<ExampleTestBlockWithExecuteArgForOwnType>()
                .ExecuteTestCase();
        }

        [Fact]
        public void TestBlockWithPropertyWithNoSetterDoesNotThrow()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithPropertyWithNoSetter>()
                .ExecuteTestCase();
        }

        [Fact]
        public void TestBlockWithMultipleExecuteMethodsThrows()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithMultipleExecuteMethods>();

            Assert.Throws<InvalidOperationException>(() => builder.ExecuteTestCase());
        }
    }

    public class ExampleTestBlockWithExecuteArg : ITestBlock
    {
        public void Execute(string input)
        {
            Assert.Equal("Testing", input);
        }
    }

    public class ExampleTestBlockWithProperty : ITestBlock
    {
        public string Input { get; set; }

        public void Execute()
        {
            Assert.Equal("Testing", Input);
        }
    }

    public class ExampleTestBlockWithConstructor : ITestBlock
    {
        public ExampleTestBlockWithConstructor(string input)
        {
            _Input = input;
        }

        public void Execute()
        {
            Assert.Equal("Testing", _Input);
        }

        private string _Input { get; }
    }

    public class ExampleTestBlockWithMultipleDependencies : ITestBlock
    {
        public string InputText { get; set; }

        public void Execute(int inputNumber)
        {
            Assert.Equal("Testing", InputText);
            Assert.Equal(1234, inputNumber);
        }
    }

    public class ExampleTestBlockWithExecuteArgForOwnType : ITestBlock
    {
        public void Execute(ExampleDataThing input)
        {
            Assert.Equal("Testing", input.Testing);
        }
    }

    public class ExampleTestBlockWithPropertyForOwnType : ITestBlock
    {
        public ExampleDataThing Input { get; set; }

        public void Execute()
        {
            Assert.Equal("Testing", Input.Testing);
        }
    }

    public class ExampleTestBlockWithConstructorForOwnType : ITestBlock
    {
        public ExampleTestBlockWithConstructorForOwnType(ExampleDataThing input)
        {
            _Input = input;
        }

        public void Execute()
        {
            Assert.Equal("Testing", _Input.Testing);
        }

        private ExampleDataThing _Input { get; }
    }

    public class ExampleTestBlockForFactoryWithExecuteArg : ITestBlock
    {
        public void Execute(ExampleDataThing input)
        {
            Assert.Equal("TestingOverride", input.Testing);
        }
    }

    public class ExampleTestBlockForFactoryWithProperty : ITestBlock
    {
        public ExampleDataThing Input { get; set; }

        public void Execute()
        {
            Assert.Equal("TestingOverride", Input.Testing);
        }
    }

    public class ExampleTestBlockForFactoryWithConstructor : ITestBlock
    {
        public ExampleTestBlockForFactoryWithConstructor(ExampleDataThing input)
        {
            _Input = input;
        }

        public void Execute()
        {
            Assert.Equal("TestingOverride", _Input.Testing);
        }

        private ExampleDataThing _Input { get; }
    }

    public class ExampleTestBlockWithPropertyWithNoSetter : ITestBlock
    {
        public string Input { get; }

        public void Execute()
        {
            Assert.Null(Input);
        }
    }

    public class ExampleTestBlockWithMultipleExecuteMethods : ITestBlock
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
}
