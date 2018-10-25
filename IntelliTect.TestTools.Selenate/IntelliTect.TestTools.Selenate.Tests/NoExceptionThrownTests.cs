using System;
using System.Threading.Tasks;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Tests
{
    public class NoExceptionThrownTests : BaseTest
    {
        [Fact]
        public async Task CheckActionParamsForExpectedExpectionDoesNotThrow()
        {
            await Wait.Until(() => CheckExceptionsVoidReturn(1, 1), TimeSpan.FromSeconds(2), typeof(InvalidOperationException));
        }

        [Fact]
        public async Task CheckFuncParamsForExpectedExpectionDoesNotThrow()
        {
            await Wait.Until(() => CheckExceptionsBoolReturn(1, 1), TimeSpan.FromSeconds(2), typeof(InvalidOperationException));
        }

        [Fact]
        public async Task CheckActionsGenericForOneExpectedExceptionDoesNotThrow()
        {
            await Wait.Until<InvalidOperationException>(() => CheckExceptionsVoidReturn(1, 1), TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task CheckFuncGenericForOneExpectedExceptionDoesNotThrow()
        {
            await Wait.Until<InvalidOperationException>(() => CheckExceptionsBoolReturn(1, 1), TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task CheckActionsGenericForTwoExpectedExceptiosnDoesNotThrow()
        {
            await Wait.Until<InvalidOperationException, InvalidProgramException>(() => CheckExceptionsVoidReturn(1, 2), TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task CheckFuncGenericForTwoExpectedExceptionsDoesNotThrow()
        {
            await Wait.Until<InvalidOperationException, InvalidProgramException>(() => CheckExceptionsBoolReturn(1, 2), TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task CheckActionsGenericForThreeExpectedExceptiosnDoesNotThrow()
        {
            await Wait.Until<InvalidOperationException, InvalidProgramException, IndexOutOfRangeException>(() => CheckExceptionsVoidReturn(1, 3), TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task CheckFuncGenericForThreeExpectedExceptionsDoesNotThrow()
        {
            await Wait.Until<InvalidOperationException, InvalidProgramException, IndexOutOfRangeException>(() => CheckExceptionsBoolReturn(1, 3), TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task CheckActionsGenericForFourExpectedExceptiosnDoesNotThrow()
        {
            await Wait.Until<InvalidOperationException, InvalidProgramException, IndexOutOfRangeException, ArgumentNullException>(() => CheckExceptionsVoidReturn(1, 4), TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task CheckFuncGenericForFourExpectedExceptionsDoesNotThrow()
        {
            await Wait.Until<InvalidOperationException, InvalidProgramException, IndexOutOfRangeException, ArgumentNullException>(() => CheckExceptionsBoolReturn(1, 4), TimeSpan.FromSeconds(2));
        }
    }
}
