using System;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Tests
{
    public class ConditionalWaitTests
    {
        [Fact]
        public void CheckActionParamsForTypeChecking()
        {
            ConditionalWait wait = new ConditionalWait();
            Exception ex = Assert.Throws<ArgumentException>(() => wait.WaitFor(TestVoidDelegate, TimeSpan.FromSeconds(1), typeof(string)).GetAwaiter().GetResult());
            Assert.Equal("Invalid type passed into exceptionsToIgnore paramter. Must be of type Exception.", ex.Message);
        }

        [Fact]
        public void CheckFuncParamsForTypeChecking()
        {
            ConditionalWait wait = new ConditionalWait();
            Exception ex = Assert.Throws<ArgumentException>(() => wait.WaitFor(TestReturnDelegate, TimeSpan.FromSeconds(1), typeof(string)).GetAwaiter().GetResult());
            Assert.Equal("Invalid type passed into exceptionsToIgnore paramter. Must be of type Exception.", ex.Message);
        }

        private void TestVoidDelegate()
        {
            throw new Exception();
        }

        private bool TestReturnDelegate()
        {
            throw new Exception();
        }
    }
}
