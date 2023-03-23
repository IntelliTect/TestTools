using IntelliTect.TestTools.TestFramework.Tests.TestData.Dependencies;
using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestBuilderTests.PositiveConditions
{
    public class ResolverTests
    {
        [Fact]
        public void Test1()
        {
            TestCase tc = new TestBuilder()
                .AddDependencyService<IExampleDataInterface, ExampleImplementation>()
                .AddTestBlock<InterfaceBlock>()
                .Build();

            tc.Execute();
        }

        [Fact]
        public void Test2()
        {
            TestCase tc = new TestBuilder()
                .AddDependencyService<IExampleDataInterface, ExampleImplementation>()
                .AddTestBlock<CorrectInterfaceBlock>()
                .Build();

            tc.Execute();
        }

        [Fact]
        public void Test3()
        {
            var tb = new TestBuilder()
                .AddDependencyService<IExampleDataInterface, ExampleImplementation>()
                .AddTestBlock<IncorrectInterfaceBlock>();

            var ex = Assert.Throws<AggregateException>(() => tb.Build());
            Assert.Single(ex.InnerExceptions);
        }

        [Fact]
        public void Test4()
        {
            TestCase tc = new TestBuilder()
                .AddDependencyService<IExampleDataInterface, OtherExampleImplementation>()
                .AddDependencyService<IExampleDataInterface, ExampleImplementation>()
                .AddTestBlock<IncorrectInterfaceBlock>()
                .Build();

            tc.Execute();
        }

        // Need to also test a dependency that's implementing multiple interfaces
    }

    public class InterfaceBlock : TestBlock
    {
        public void Execute(IExampleDataInterface iface)
        {
            Assert.NotNull(iface);
        }
    }

    public class CorrectInterfaceBlock : TestBlock
    {
        public void Execute(ExampleImplementation iface)
        {
            Assert.NotNull(iface);
        }
    }

    public class IncorrectInterfaceBlock : TestBlock
    {
        public void Execute(OtherExampleImplementation iface)
        {
            Assert.NotNull(iface);
        }
    }
}
