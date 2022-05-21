using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IntelliTect.TestTools.Data.Test;

public class DatabaseFixtureMultipleDbContextsTests
{
    public DatabaseFixtureMultipleDbContextsTests()
    { }

    [Fact]
    public async Task DatabaseFixture_MultipleDbContexts_AccessToSameConnection()
    {
        var dbFixture = new DatabaseFixture<SampleDbContext, ReadOnlySampleDbContext>();
        const int personCount = 5;
        
        await dbFixture.PerformDatabaseOperation(async context =>
        {
            var persons = Enumerable.Range(1, 5)
                .Select(_ => FakesFactory.Create<Person>());
            context.Persons.AddRange(persons);
            await context.SaveChangesAsync();
        });

        await dbFixture.PerformDatabaseOperation<BaseDbContext>(context =>
        {
            Assert.Equal(personCount, context.Persons.Count());
        });

        await dbFixture.PerformDatabaseOperation(context =>
        {
            Assert.Equal(personCount, context.Persons.Count());
        });

        await dbFixture.PerformDatabaseOperation<SampleDbContext>(context =>
        {
            Assert.Equal(personCount, context.Persons.Count());
        });
    }

    [Fact]
    public async Task DatabaseFixture_MultipleDbContexts_UnregisteredDbContextRequested_DbContextCreated()
    {
        var dbFixture = new DatabaseFixture<SampleDbContext, BaseDbContext>();

        await dbFixture.PerformDatabaseOperation(async context =>
        {
            var persons = Enumerable.Range(1, 5)
                .Select(_ => FakesFactory.Create<Person>());
            context.Persons.AddRange(persons);
            await context.SaveChangesAsync();
        });

        await dbFixture.PerformDatabaseOperation<ReadOnlySampleDbContext>(context =>
        {
            Assert.Equal(5, context.Persons.Count());
        });
    }
}