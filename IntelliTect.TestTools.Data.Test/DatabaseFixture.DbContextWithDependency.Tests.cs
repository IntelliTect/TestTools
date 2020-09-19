using Microsoft.EntityFrameworkCore;
using System;
using System.Dynamic;
using System.Threading.Tasks;
using Xunit;

namespace IntelliTect.TestTools.Data.Test
{
    public class DatabaseFixtureDbContextWithDependencyTests
    {
        [Fact]
        public async Task BadDbContext_MissingCorrectConstructor_ExceptionThrown()
        {
            var databaseFixture = new DatabaseFixture<DbContextWithDependency>();
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await databaseFixture.PerformDatabaseOperation(_ => { });
            });
        }

        [Fact]
        public async Task BadDbContext_DependencySpecified_ContextCreated()
        {
            var dependency = new Dependency();
            var databaseFixture = new DatabaseFixture<DbContextWithDependency>();
            databaseFixture.AddDependency<IDependency>(dependency);

            await databaseFixture.PerformDatabaseOperation(context =>
            {
                Assert.Equal(dependency, context.MyDependency);
            });
        }

        private class Dependency : IDependency
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
}
