using IntelliTect.TestTools.TestFramework.Tests.TestData;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestBuilderTests
{
    public class SingleDependencyTests
    {
        [Fact]
        public void Testing()
        {
            TestBuilder builder = new TestBuilder()
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithExecuteArg>();

            builder.Build();
        }
        // Check for logger
        // Check for removed logger

        // Check for singleton
        // Check for scoped
        // Check for output
        // Find for constructor
        // Find for property
        // Find for execute param

        // TO DO:
        // Check for test block argument
    }
}
