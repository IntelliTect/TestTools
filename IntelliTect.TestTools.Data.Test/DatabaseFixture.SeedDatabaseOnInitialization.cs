using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntelliTect.TestTools.Data.Test;

public class DatabaseFixtureSeedDatabaseOnInitialization : IClassFixture<DatabaseFixture<SampleDbContext>>
{
    private readonly DatabaseFixture<SampleDbContext> _DatabaseFixture;

    public DatabaseFixtureSeedDatabaseOnInitialization(DatabaseFixture<SampleDbContext> dbFixture)
    {
        _DatabaseFixture = dbFixture ?? throw new ArgumentNullException(nameof(dbFixture));

        _DatabaseFixture.SetInitialize<SampleDbContext>(SeedData);
    }

    private Task SeedData(SampleDbContext dbContext)
    {
        var things = Enumerable.Range(1, 5).Select(_ => FakesFactory.Create<Person>());

        dbContext.Persons.AddRange(things);

        return dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task DatabaseFixtureSeed_DatabaseFixtureReused_SeedDataExists()
    {
        await _DatabaseFixture.PerformDatabaseOperation(context =>
        {
            Assert.Equal(5, context.Persons.Count());
            Assert.False(context.Persons.Any(x => x == null));
        });
    }

    [Fact]
    public async Task DatabaseFixtureSeed_PerformMultipleDatabaseOperations_SeedDataExistsOnce()
    {
        await _DatabaseFixture.PerformDatabaseOperation(_ => { });

        await _DatabaseFixture.PerformDatabaseOperation(context =>
        {
            Assert.Equal(5, context.Persons.Count());
            Assert.False(context.Persons.Any(x => x == null));
        });
    }

    [Fact]
    public async Task DatabaseFixtureSeed_DatabaseFixtureWithMultipleContexts_SeedDataExistsOnce()
    {
        var dbFixture = new DatabaseFixture<ReadOnlySampleDbContext, SampleDbContext>();
        dbFixture.SetInitialize<SampleDbContext>(SeedData);

        await dbFixture.PerformDatabaseOperation(async context =>
        {
            Assert.Equal(5, await context.Persons.CountAsync());
        });
    }

    [Fact]
    public async Task DatabaseFixtureSeed_UseDbContextOutsideOfFixtureGeneric_DbContextCreatedAndUsedForSeed()
    {
        var fixture = new DatabaseFixture<SampleDbContext>();
        fixture.SetInitialize<BaseDbContext>(async context =>
        {
            var persons = Enumerable.Range(1, 5).Select(_ => FakesFactory.Create<Person>());
            context.Persons.AddRange(persons);
            await context.SaveChangesAsync();
        });

        await _DatabaseFixture.PerformDatabaseOperation(context =>
        {
            Assert.Equal(5, context.Persons.Count());
        });
    }
}