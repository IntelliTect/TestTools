using Microsoft.EntityFrameworkCore;

namespace IntelliTect.TestTools.Data.Test;

public class SampleDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }

    public DbSet<BlogPost> BlogPosts { get; set; }

    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
    { }

    public SampleDbContext(DbContextOptions options, string something) : base(options)
    { }
}