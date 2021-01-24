using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SampleDataProject;
using Xunit;

namespace TestToolsDataTest
{
    public class NotUsingTestToolsData : IDisposable
    {
        private SampleDbContext _Db;
        private SqliteConnection _SqliteConnection;

        public NotUsingTestToolsData()
        {
            _SqliteConnection = new SqliteConnection("DataSource=:memory:");
            _SqliteConnection.Open();

            var options = new DbContextOptionsBuilder<SampleDbContext>()
                .UseSqlite(_SqliteConnection);
            _Db = new SampleDbContext(options.Options);
            _Db.Database.EnsureCreated();
        }

        // This test does not use IntelliTect.TestTools.Data and does not properly handle scoping the SampleDbContext to
        // prevent caching. This results in a test that pass when it should not.
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

            await _Db.Persons.AddAsync(person);
            await _Db.BlogPosts.AddRangeAsync(blogPosts);
            await _Db.SaveChangesAsync();

            var myService = new MyService(_Db);

            var blogs = await myService.GetBlogsByPerson(person.Name);

            Assert.NotNull(blogs.First().Person);
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
                _SqliteConnection?.Dispose();
                _Db?.Dispose();
            }

            _Disposed = true;
        }
    }
}
