using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace IntelliTect.TestTools.Data
{
    public class DatabaseFixture<T> where T : DbContext
    {
        private SqliteConnection SqliteConnection { get; }
        private DbContextOptions<T> Options { get; }

        public DatabaseFixture()
        {
            SqliteConnection = new SqliteConnection("DataSource=:memory:");
            SqliteConnection.Open();

            Options = new DbContextOptionsBuilder<T>()
                .UseSqlite(SqliteConnection)
                .Options;

            using var db = CreateNewContext();
            db.Database.EnsureCreated();
        }

        private T CreateNewContext()
        {
            var constructor = typeof(T)
                .GetConstructors()
                .Select(x => x.GetParameters())
                .Where(x => x.Length == 1)
                .Select(x => x.SingleOrDefault())
                .Where(x => x != null)
                .SingleOrDefault(x => x.ParameterType == typeof(DbContextOptions));

            if (constructor is null)
            {
                throw new InvalidOperationException(
                    $"'{typeof(T)}' must contain a constructor that has a single parameter " +
                    "of type 'DbContextOptions'");
            }

            var db = Activator.CreateInstance(typeof(T), Options);

            if (db is null)
            {
                throw new InvalidOperationException($"'{typeof(T)} could not be instantiated");
            }

            return (T) db;
        }

        public async Task PerformDatabaseOperationAsync(Func<T, Task> operation)
        {
            var db = CreateNewContext();
            await operation(db);
        }

        public void PerformDatabaseOperation(Action<T> operation)
        {
            var db = CreateNewContext();
            operation(db);
        }
    }
}
