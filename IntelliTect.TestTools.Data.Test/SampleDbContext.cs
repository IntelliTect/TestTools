using Microsoft.EntityFrameworkCore;

namespace IntelliTect.TestTools.Data.Test
{
    public class SampleDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        public SampleDbContext(DbContextOptions options) : base(options)
        {
        }

        public SampleDbContext(DbContextOptions options, string something) : base(options)
        {
        }
    }
}