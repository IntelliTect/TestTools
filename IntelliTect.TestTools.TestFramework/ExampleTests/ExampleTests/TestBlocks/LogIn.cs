using ExampleTests.Data;
using Xunit;

namespace ExampleTests.TestBlocks
{
    public class LogIn : TestBlockBase
    {
        public Account Account { get; set; }

        public void Execute()
        {
            // Do stuff with account object
            Assert.True(Account.Id != 0);
            Assert.False(string.IsNullOrWhiteSpace(Account.Password));
        }
    }
}
