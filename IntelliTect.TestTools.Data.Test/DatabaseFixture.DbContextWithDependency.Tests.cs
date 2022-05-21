using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IntelliTect.TestTools.Data.Test;

public class DatabaseFixtureDbContextWithDependencyTests
{
    [Fact]
    public async Task DbContextWithDependency_MissingCorrectConstructor_ExceptionThrown()
    {
        var databaseFixture = new DatabaseFixture<DbContextWithDependency>();
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await databaseFixture.PerformDatabaseOperation(_ => { });
        });
    }

    [Fact]
    public async Task DbContextWithDependency_DependencyByTypeSpecified_ContextCreated()
    {
        var dependency = new Dependency();
        var databaseFixture = new DatabaseFixture<DbContextWithDependency>();
        databaseFixture.AddDependency<IDependency>(dependency);

        await databaseFixture.PerformDatabaseOperation(context =>
        {
            Assert.Equal(dependency, context.MyDependency);
        });
    }

    [Fact]
    public async Task DbContextWithDependency_DependencyByNameSpecified_ContextCreated()
    {
        var dependency1 = new Dependency();
        var dependency2 = new Dependency();
        var databaseFixture = new DatabaseFixture<HasDuplicateTypeDependencies>();
        databaseFixture.AddDependency<IDependency>(dependency1, "dependency1");
        databaseFixture.AddDependency<IDependency>(dependency2, "dependency2");

        await databaseFixture.PerformDatabaseOperation(context =>
        {
            Assert.Equal(dependency1, context.Dependency1);
            Assert.Equal(dependency2, context.Dependency2);
        });
    }

    [Fact]
    public async Task DbContextWithOnlyDefaultConstructor_ThrowsException()
    {
        var databaseFixture = new DatabaseFixture<HasDefaultConstructor>();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            databaseFixture.PerformDatabaseOperation(context => { }));

        Assert.Equal($"'{typeof(HasDefaultConstructor)}' does not contain constructor that has a valid signature", ex.Message);
    }

    [Fact]
    public async Task DbContextWithInvalidConstructor_ThrowsException()
    {
        var databaseFixture = new DatabaseFixture<HasInvalidConstructor>();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            databaseFixture.PerformDatabaseOperation(context => { }));

        Assert.Equal($"'{typeof(HasInvalidConstructor)}' does not contain constructor that has a valid signature", ex.Message);
    }

    private class Dependency : IDependency
    { }


    private class HasInvalidConstructor : DbContext
    {
        public HasInvalidConstructor(IDependency dependency)
        { }
    }

    private class HasDefaultConstructor : DbContext
    { }

    private class HasDuplicateTypeDependencies : DbContext
    {
        public HasDuplicateTypeDependencies(IDependency dependency1, DbContextOptions options, IDependency dependency2)
            : base(options)
        {
            Dependency1 = dependency1;
            Dependency2 = dependency2;
        }

        public IDependency Dependency1 { get; }
        public IDependency Dependency2 { get; }
    }
}