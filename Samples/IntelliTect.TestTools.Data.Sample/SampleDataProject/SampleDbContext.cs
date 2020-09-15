using Microsoft.EntityFrameworkCore;

namespace SampleDataProject
{
    public class SampleDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        public DbSet<BlogPost> BlogPosts { get; set; }

        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
        {
        }
    }
}
