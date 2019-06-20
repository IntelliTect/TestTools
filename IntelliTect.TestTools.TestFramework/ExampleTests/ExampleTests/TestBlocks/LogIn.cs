using ExampleTests.Data;

namespace ExampleTests.TestBlocks
{
    public class LogIn : TestBlockBase
    {
        public LogIn(Account account)
        {
            _Account = account;
        }

        public void Execute()
        {

        }

        private Account _Account { get; }
    }
}
