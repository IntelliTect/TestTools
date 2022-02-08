using IntelliTect.TestTools.TestFramework.Tests.TestData;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestCaseTests
{
    public class MultipleDependencyTests
    {
        // Test for...
        // Two of the same type last one should win, but that's basic MS DI behavior

        [Fact]
        public void TestingReturningTwoOfSameType()
        {
            // What happens here? Will the return override the prior?
            TestBuilder builder = new();
            TestCase tc = builder
                .AddTestBlock<SomeTestBlock>()
                .AddTestBlock<SomeTestBlock>()
                .AddTestBlock<ExampleTestBlockWithBoolReturn>()
                .Build();
            tc.Execute();
        }
    }

    public class SomeTestBlock : ITestBlock
    {
        public ITestCaseLogger? Log { get; }

        public bool Execute()
        {
            return true;
        }
    }

    public class SomeOtherTestBlockWithExec : ITestBlock
    {
        public ITestCaseLogger? Log { get; }

        public void Execute(SomeDependency dep)
        {
            Assert.True(dep?.WillThisWork);
        }
    }

    public class SomeOtherTestBlockWithCtor : ITestBlock
    {
        public SomeOtherTestBlockWithCtor(SomeDependency dep)
        {
            Dep = dep;
        }

        protected SomeDependency Dep { get; }

        public ITestCaseLogger? Log { get; }

        public void Execute()
        {
            Assert.True(Dep.WillThisWork);
        }
    }

    public class SomeDependency
    {
        public SomeDependency(bool willThisWork)
        {
            WillThisWork = willThisWork;
        }

        public bool WillThisWork { get; }
    }
}
