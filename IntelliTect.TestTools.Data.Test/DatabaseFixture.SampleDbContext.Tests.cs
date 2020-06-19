using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntelliTect.IntelliTime.Data.Test.Util;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntelliTect.TestTools.Data.Test
{
    public class DatabaseFixtureSampleDbContextTests : IClassFixture<DatabaseFixture<SampleDbContext>>
    {
        private readonly DatabaseFixture<SampleDbContext> _DatabaseFixture;

        public DatabaseFixtureSampleDbContextTests(DatabaseFixture<SampleDbContext> dbFixture)
        {
            _DatabaseFixture = dbFixture ?? throw new ArgumentNullException(nameof(dbFixture));
        }

        [Fact]
        public async Task DatabaseFixture_CanPerformDatabaseOperation()
        {
            var person = FakesFactory.Create<Person>();

            await _DatabaseFixture.PerformDatabaseOperation(async context =>
            {
                context.Persons.Add(person);
                await context.SaveChangesAsync();
            });

            await _DatabaseFixture.PerformDatabaseOperation(context =>
            {
                var personInDatabase = context.Persons.SingleOrDefault(x => x.PersonId == person.PersonId);

                Assert.NotNull(personInDatabase);
                Assert.Equal(person.Age, personInDatabase.Age);
                Assert.Equal(person.Name, personInDatabase.Name);

                return Task.CompletedTask;
            });
        }

        [Fact]
        public async Task DatabaseFixture_EFInMemoryLoggingEnabled_LogsPopulated()
        {
            var person = FakesFactory.Create<Person>();

            await _DatabaseFixture.PerformDatabaseOperation(async context =>
            {
                context.Persons.Add(person);
                await context.SaveChangesAsync();
            });

            var logger = _DatabaseFixture
                .GetInMemoryLoggers()["Microsoft.EntityFrameworkCore.Database.Command"];

            Assert.NotEmpty(logger.Logs);
            Assert.NotEqual(0, logger.Logs.First().EventId);
        }

        [Fact]
        public async Task DatabaseFixture_WriteAndRetrieveObjectWithNoCaching_NoIncludedNavigationProperties()
        {
            var person = FakesFactory.Create<Person>();

            await _DatabaseFixture.PerformDatabaseOperation(async context =>
            {
                context.Persons.Add(person);
                await context.SaveChangesAsync();
            });

            var blogPosts = Enumerable.Range(0, 5)
                .Select(x =>
                {
                    var blogPost = FakesFactory.Create<BlogPost>();
                    blogPost.PersonId = person.PersonId;

                    return blogPost;
                });

            await _DatabaseFixture.PerformDatabaseOperation(async context =>
            {
                context.BlogPosts.AddRange(blogPosts);
                await context.SaveChangesAsync();
            });

            List<Person> persons = null;
            await _DatabaseFixture.PerformDatabaseOperation(context =>
            {
                persons = context.Persons.Select(x => x).ToList();

                return Task.CompletedTask;
            });

            Assert.All(persons, curPerson =>
            {
                Assert.Null(curPerson.BlogPosts);
            });
        }

        [Fact]
        public async Task DatabaseFixture_ExplicitlyCallInclude_NavigationPropertyIncludedProperly()
        {
            var person = FakesFactory.Create<Person>();

            await _DatabaseFixture.PerformDatabaseOperation(async context =>
            {
                context.Persons.Add(person);
                await context.SaveChangesAsync();
            });

            var blogPosts = Enumerable.Range(0, 5)
                .Select(x =>
                {
                    var blogPost = FakesFactory.Create<BlogPost>();
                    blogPost.PersonId = person.PersonId;

                    return blogPost;
                });

            await _DatabaseFixture.PerformDatabaseOperation(async context =>
            {
                context.BlogPosts.AddRange(blogPosts);
                await context.SaveChangesAsync();
            });

            List<Person> persons = null;
            await _DatabaseFixture.PerformDatabaseOperation(context =>
            {
                persons = context
                    .Persons
                    .Include(x => x.BlogPosts)
                    .Select(x => x)
                    .ToList();

                return Task.CompletedTask;
            });

            Assert.All(persons, curPerson =>
            {
                Assert.NotNull(curPerson.BlogPosts);
            });
        }
    }
}
