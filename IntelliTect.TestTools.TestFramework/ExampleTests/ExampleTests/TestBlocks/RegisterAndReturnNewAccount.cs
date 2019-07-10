using ExampleTests.Data;

namespace ExampleTests.TestBlocks
{
    public class RegisterAndReturnNewAccount : TestBlockBase
    {
        public Account Execute(string firstName, string lastName)
        {
            return new Account { FirstName = firstName, LastName = lastName, Id = 98765 };
        }
    }
}
