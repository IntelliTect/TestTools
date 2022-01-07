using IntelliTect.TestTools.TestFramework.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestCaseTests
{
    public class ResolveDependenciesTests
    {
        //[Fact]
        //public void TestingMultpDep()
        //{
        //    TestBuilder builder = new();
        //    TestCase tc = builder
        //        .AddDependencyService<SomeDependency>()
        //        .AddTestBlock<SomeOtherTestBlockWithExec>()
        //        .Build();
        //    tc.Execute();
        //}

        //[Fact]
        //public void TestingExecDep()
        //{
        //    TestBuilder builder = new();
        //    TestCase tc = builder
        //        .AddDependencyService<SomeDependency>()
        //        .AddTestBlock<SomeTestBlock>()
        //        .AddTestBlock<SomeOtherTestBlockWithExec>()
        //        .Build();
        //    tc.Execute();
        //}

        //[Fact]
        //public void TestingCtorDep()
        //{
        //    TestBuilder builder = new();
        //    TestCase tc = builder
        //        .AddDependencyService<SomeDependency>()
        //        .AddTestBlock<SomeTestBlock>()
        //        .AddTestBlock<SomeOtherTestBlockWithCtor>()
        //        .Build();
        //    tc.Execute();
        //}

        [Fact]
        public void TestingReturningTwoOfSameType()
        {
            // What happens here? Will the return override the prior?
            TestBuilder builder = new();
            TestCase tc = builder
                .AddTestBlock<SomeTestBlock>()
                .AddTestBlock<SomeTestBlock>()
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
