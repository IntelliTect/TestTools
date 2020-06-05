using System;
using System.Threading.Tasks;
using Xunit;

namespace IntelliTect.TestTools.Data.Test
{
    public class DatabaseFixtureTests
    {
        [Fact]
        public void DatabaseFixture_AttemptToGetInMemoryLoggersBeforeInitialization_InvalidOperationException()
        {
            var databaseFixture = new DatabaseFixture<SampleDbContext>();
            Assert.Throws<InvalidOperationException>(() =>
            {
                _ = databaseFixture.GetInMemoryLoggers();
            });
        }

        [Fact]
        public void DatabaseFixture_HookIntoBeforeLoggingSetup_EventExecuted()
        {
            var databaseFixture = new DatabaseFixture<SampleDbContext>();

            bool operationPerformed = false;
            databaseFixture.BeforeLoggingSetup += (sender, builder) =>
            {
                operationPerformed = true;
            };

            databaseFixture.PerformDatabaseOperation(_ => Task.CompletedTask);

            Assert.True(operationPerformed);
        }
    }
}
