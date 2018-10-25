using System;
using System.Threading.Tasks;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Tests
{
    public class UnExpectedExceptionsTests : BaseTest
    {
        [Fact]
        public async Task CheckActionParamsForUnExpectedExpectionThrows()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => Wait.Until(
                    () => CheckExceptionsVoidReturn(1, 1), TimeSpan.FromSeconds(2), typeof(NullReferenceException)));
        }

        [Fact]
        public async Task CheckFuncParamsForUnExpectedExpectionThrows()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => Wait.Until(
                    () => CheckExceptionsBoolReturn(1, 1), TimeSpan.FromSeconds(2), typeof(NullReferenceException)));
        }

        [Fact]
        public async Task CheckActionGenericForOneUnExpectedExpectionThrows()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => Wait.Until<NullReferenceException>(
                    () => CheckExceptionsVoidReturn(1, 1), TimeSpan.FromSeconds(2)));
        }

        [Fact]
        public async Task CheckFuncGenericForOneUnExpectedExpectionThrows()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => Wait.Until<NullReferenceException>(
                    () => CheckExceptionsBoolReturn(1, 1), TimeSpan.FromSeconds(2)));
        }

        [Fact]
        public async Task CheckActionGenericForTwoUnExpectedExpectionsThrows()
        {
            await Assert.ThrowsAsync<InvalidProgramException>(
                () => Wait.Until<NullReferenceException, InvalidOperationException>(
                    () => CheckExceptionsVoidReturn(1, 2), TimeSpan.FromSeconds(2)));
        }

        [Fact]
        public async Task CheckFuncGenericForTwoUnExpectedExpectionsThrows()
        {
            await Assert.ThrowsAsync<InvalidProgramException>(
                () => Wait.Until<NullReferenceException, InvalidOperationException>(
                    () => CheckExceptionsBoolReturn(1, 2), TimeSpan.FromSeconds(2)));
        }

        [Fact]
        public async Task CheckActionGenericForThreeUnExpectedExpectionsThrows()
        {
            await Assert.ThrowsAsync<IndexOutOfRangeException>(
                () => Wait.Until<NullReferenceException, InvalidOperationException, InvalidProgramException>(
                    () => CheckExceptionsVoidReturn(1, 3), TimeSpan.FromSeconds(2)));
        }

        [Fact]
        public async Task CheckFuncGenericForThreeUnExpectedExpectionsThrows()
        {
            await Assert.ThrowsAsync<IndexOutOfRangeException>(
                () => Wait.Until<NullReferenceException, InvalidOperationException, InvalidProgramException>(
                    () => CheckExceptionsBoolReturn(1, 3), TimeSpan.FromSeconds(2)));
        }

        [Fact]
        public async Task CheckActionGenericForFourUnExpectedExpectionsThrows()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => Wait.Until<NullReferenceException, InvalidOperationException, IndexOutOfRangeException, InvalidProgramException>(
                    () => CheckExceptionsVoidReturn(1, 4), TimeSpan.FromSeconds(2)));
        }

        [Fact]
        public async Task CheckFuncGenericForFourUnExpectedExpectionsThrows()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => Wait.Until<NullReferenceException, InvalidOperationException, IndexOutOfRangeException, InvalidProgramException>(
                    () => CheckExceptionsBoolReturn(1, 4), TimeSpan.FromSeconds(2)));
        }
    }
}
