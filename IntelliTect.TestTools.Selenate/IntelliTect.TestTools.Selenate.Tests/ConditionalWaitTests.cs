using System;
using System.Threading.Tasks;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Tests
{
    public class ConditionalWaitTests
    {
        [Fact]
        public async Task CheckActionParamsForTypeChecking()
        {
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(() => Wait.Until(TestVoidDelegate, TimeSpan.FromSeconds(1), typeof(string)));
            Assert.Equal("Invalid type passed into exceptionsToIgnore paramter. Must be of type Exception.", ex.Message);
        }

        [Fact]
        public async Task CheckFuncParamsForTypeChecking()
        {
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(() => Wait.Until(TestReturnDelegate, TimeSpan.FromSeconds(1), typeof(string)));
            Assert.Equal("Invalid type passed into exceptionsToIgnore paramter. Must be of type Exception.", ex.Message);
        }

        [Fact]
        public async Task CheckForUnexpectedException()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => Wait.Until<ArgumentNullException>(() => TestNullRefException(), TimeSpan.FromSeconds(1)));
        }

        private void TestVoidDelegate()
        {
            throw new Exception();
        }

        private bool TestReturnDelegate()
        {
            throw new Exception();
        }

        private void TestNullRefException()
        {
            throw new NullReferenceException();
        }
    }
}
