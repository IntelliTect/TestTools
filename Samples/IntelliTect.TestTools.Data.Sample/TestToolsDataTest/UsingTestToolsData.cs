using System;
using System.Linq;
using System.Threading.Tasks;
using IntelliTect.TestTools.Data;
using SampleDataProject;
using Xunit;

namespace TestToolsDataTest
{
    public class UsingTestToolsData : IDisposable
    {
        private readonly DatabaseFixture<SampleDbContext> _DatabaseFixture;

        public UsingTestToolsData()
        {
            _DatabaseFixture = new DatabaseFixture<SampleDbContext>();
        }

        // This test uses IntelliTect.TestTools.Data and properly handles scoping the SampleDbContext to prevent caching
        // incorrectly resulting in passing tests. This test fails like a real DbContext would in production
        [Fact]
        public async Task GetBlogsByPerson_MultipleBlogPosts_BlogPostsWithPersonPopulated()
        {
            var person = FakesFactory.Create<Person>();
            var blogPosts = Enumerable.Range(1, 3)
                .Select(x => FakesFactory.Create<BlogPost>())
                .Select(x =>
                {
                    x.Person = person;
                    return x;
                });

            await _DatabaseFixture.PerformDatabaseOperation(async context =>
            {
                await context.Persons.AddAsync(person);
                await context.BlogPosts.AddRangeAsync(blogPosts);
                await context.SaveChangesAsync();
            });

            await _DatabaseFixture.PerformDatabaseOperation(async context =>
            {
                var myService = new MyService(context);

                var blogs = await myService.GetBlogsByPerson(person.Name);

                Assert.NotNull(blogs.First().Person);
            });
        }

        private bool _Disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed)
            {
                return;
            }

            if (disposing)
            {
                _DatabaseFixture.Dispose();
            }

            _Disposed = true;
        }
    }
}
