using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace IntelliTect.TestTools.Selenate.Tests
{
    public class ConditionalWaitTests
    {
        [Fact]
        public async Task CheckActionParamsForTypeChecking()
        {
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(() => Wait.Until(TestVoidDelegate, TimeSpan.FromSeconds(1), typeof(string)));
            Assert.Equal("Invalid type passed into exceptionsToIgnore parameter. Must be of type Exception.", ex.Message);
        }

        [Fact]
        public async Task CheckFuncParamsForTypeChecking()
        {
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(() => Wait.Until(TestReturnDelegate, TimeSpan.FromSeconds(1), typeof(string)));
            Assert.Equal("Invalid type passed into exceptionsToIgnore parameter. Must be of type Exception.", ex.Message);
        }

        [Fact]
        public async Task CheckForUnexpectedException()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => Wait.Until<ArgumentNullException>(() => TestNullRefException(), TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public async Task CheckForExpectedFailure()
        {
            await Assert.ThrowsAsync<AggregateException>(
                () => Wait.Until<EqualException>(() => Equals( 4, 5 ), TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public async Task CheckForExpectedSuccess()
        {
            // If this ever breaks, it should throw an exception
            await Wait.Until<EqualException>(() => Equals(5, 5), TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task CheckForSuccessAfterFailure()
        {
            // IN PROGRESS
            //int comparisonValue = 4;
            //Task result = new Task(() => Wait.Until<EqualException>(() => Equals(5, comparisonValue), TimeSpan.FromSeconds(1)));

            //await Task.Delay(100);
            // Change wait to valid arguments
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

        private void Equals(int expected, int value)
        {
            if (value == expected)
            {
                return;
            }
            throw new EqualException(expected, value);
        }
    }
}
