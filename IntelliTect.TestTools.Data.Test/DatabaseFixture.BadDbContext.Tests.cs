using System;
using System.Threading.Tasks;
using Xunit;

namespace IntelliTect.TestTools.Data.Test
{
    public class DatabaseFixtureBadDbContextTests
    {
        [Fact]
        public void BadDbContext_MissingCorrectConstructor_ExceptionThrown()
        {
            var databaseFixture = new DatabaseFixture<BadDbContext>();
            databaseFixture.PerformDatabaseOperation(_ => Task.CompletedTask);
        }
    }
}
