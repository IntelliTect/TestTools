using System;

namespace ExampleTests.Data
{
    public class AccountFactory
    {
        public AccountFactory()
        {
            AccountId = GetAccountId;
            Account = GetFullAccount;
        }

        public Func<IServiceProvider, Account> AccountId { get; private set; }
        public Func<IServiceProvider, Account> Account { get; private set; }

        private Account GetFullAccount(IServiceProvider service)
        {
            var acct = new Account { Id = QueryAccountId() };
            acct = ExampleQuery(acct.Id);
            return acct;
        }

        private Account GetAccountId(IServiceProvider service)
        {
            var acct = new Account { Id = QueryAccountId() };
            return acct;
        }

        private int QueryAccountId()
        {
            // In real example, run a query here
            return 012345;
        }

        private Account ExampleQuery(int acctId)
        {
            // In real example, run a query here
            var acct = new Account
            {
                Id = acctId,
                FirstName = "Tester",
                LastName = "McTesterson",
                Password = "Password!"
            };
            return acct;
        }
    }
}
