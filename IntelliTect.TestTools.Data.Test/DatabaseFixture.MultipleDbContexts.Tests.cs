using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IntelliTect.TestTools.Data.Test;

public class DatabaseFixtureMultipleDbContextsTests : IClassFixture<DatabaseFixture<SampleDbContext, OtherSampleDbContext>>
{
    private readonly DatabaseFixture<SampleDbContext, OtherSampleDbContext> _DatabaseFixture;

    public DatabaseFixtureMultipleDbContextsTests(DatabaseFixture<SampleDbContext, OtherSampleDbContext> dbFixture)
    {
        _DatabaseFixture = dbFixture ?? throw new ArgumentNullException(nameof(dbFixture));
    }

    [Fact]
    public async Task DatabaseFixture_MultipleDbContexts_AccessToSameConnection()
    {
        const int personCount = 5;
        
        await _DatabaseFixture.PerformDatabaseOperation(async context =>
        {
            var persons = Enumerable.Range(1, 5)
                .Select(_ => FakesFactory.Create<Person>());
            context.Persons.AddRange(persons);
            await context.SaveChangesAsync();
        });

        await _DatabaseFixture.PerformDatabaseOperation<OtherSampleDbContext>(context =>
        {
            Assert.Equal(personCount, context.Persons.Count());
        });

        await _DatabaseFixture.PerformDatabaseOperation(context =>
        {
            Assert.Equal(personCount, context.Persons.Count());
        });

        await _DatabaseFixture.PerformDatabaseOperation<SampleDbContext>(context =>
        {
            Assert.Equal(personCount, context.Persons.Count());
        });
    }
}