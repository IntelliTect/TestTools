using ExampleTests.Data;
using Xunit;

namespace ExampleTests.TestBlocks
{
    public class VerifyAccountSummary : TestBlockBase
    {
        public void Execute(Account account)
        {
            Assert.Equal("Tester", account.FirstName);
            Assert.Equal("McTesterson", account.LastName);
            Assert.Equal(012345, account.Id);
        }
    }
}
