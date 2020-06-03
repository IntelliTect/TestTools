using System;
using System.Linq;
using IntelliTect.IntelliTime.Data.Test.Util;
using Xunit;

namespace IntelliTect.TestTools.Data.Test
{
    public class DatabaseFixtureTests : IClassFixture<DatabaseFixture<SampleDbContext>>
    {
        private readonly DatabaseFixture<SampleDbContext> _DatabaseFixture;

        public DatabaseFixtureTests(DatabaseFixture<SampleDbContext> databaseFixture)
        {
            _DatabaseFixture = databaseFixture ?? throw new ArgumentNullException(nameof(databaseFixture));
        }

        [Fact]
        public void DatabaseFixture_CanPerformDatabaseOperation()
        {
            var person = FakesFactory.Create<Person>();

            _DatabaseFixture.PerformDatabaseOperationAsync(async context =>
            {
                await context.Persons.AddAsync(person);
                await context.SaveChangesAsync();
            });

            _DatabaseFixture.PerformDatabaseOperation(context =>
            {
                var personInDatabase = context.Persons.SingleOrDefault();

                Assert.NotNull(personInDatabase);
                Assert.Equal(person.Age, personInDatabase.Age);
                Assert.Equal(person.Name, personInDatabase.Name);
            });
        }
    }
}
