using System;
using System.Threading.Tasks;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Tests
{
    public class TypeCheckingTests : BaseTest
    {
        [Fact]
        public async Task CheckActionParamsForInvalidTypeChecking()
        {
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(
                () => Wait.Until(TestVoidException, TimeSpan.FromSeconds(1), typeof(string)));
            Assert.Equal(ExceptionMessage, ex.Message);
        }

        [Fact]
        public async Task CheckFuncParamsForInvalidTypeChecking()
        {
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(
                () => Wait.Until(TestReturnException, TimeSpan.FromSeconds(1), typeof(string)));
            Assert.Equal(ExceptionMessage, ex.Message);
        }

        [Fact]
        public async Task CheckActionParamsForMixedInvalidTypeChecking()
        {
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(
                () => Wait.Until(TestVoidNullRefException, TimeSpan.FromSeconds(1), typeof(Exception), typeof(string)));
            Assert.Equal(ExceptionMessage, ex.Message);
        }

        [Fact]
        public async Task CheckFuncParamsForMixedInvalidTypeChecking()
        {
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(
                () => Wait.Until(TestReturnNullRefException, TimeSpan.FromSeconds(1), typeof(string), typeof(NullReferenceException)));
            Assert.Equal(ExceptionMessage, ex.Message);
        }

        private const string ExceptionMessage = "Invalid type passed into exceptionsToIgnore parameter. Must be of type Exception.";
    }
}
