using System;
using System.Threading.Tasks;
using Xunit;

namespace IntelliTect.TestTools.Data.Test
{
    public class DatabaseFixtureBadDbContextTests
    {
        [Fact]
        public async Task BadDbContext_MissingCorrectConstructor_ExceptionThrown()
        {
            var databaseFixture = new DatabaseFixture<BadDbContext>();
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await databaseFixture.PerformDatabaseOperation(_ => Task.CompletedTask);
            });
        }
    }
}
