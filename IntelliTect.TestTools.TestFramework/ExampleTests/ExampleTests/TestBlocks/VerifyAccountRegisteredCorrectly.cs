using ExampleTests.Data;
using Xunit;

namespace ExampleTests.TestBlocks
{
    public class VerifyAccountRegisteredCorrectly : TestBlockBase
    {
        public void Execute(Account account)
        {
            Assert.Equal("NewTester", account.FirstName);
            Assert.Equal("McTest", account.LastName);
            Assert.Equal(98765, account.Id);
        }
    }
}
