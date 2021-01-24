using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IntelliTect.TestTools.Data
{
    public static class DatabaseFixtureMixins
    {
        public static async Task PerformDatabaseOperation<TDbContext>(this DatabaseFixture<TDbContext> databaseFixture, 
            Action<TDbContext> operation) where TDbContext : DbContext
        {
            if (databaseFixture is null)
            {
                throw new ArgumentNullException(nameof(databaseFixture));
            }

            if (operation is null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            await databaseFixture.PerformDatabaseOperation(context =>
            {
                operation(context);
                return Task.CompletedTask;
            });
        }
    }
}
