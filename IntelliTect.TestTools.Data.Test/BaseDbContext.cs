using Microsoft.EntityFrameworkCore;

namespace IntelliTect.TestTools.Data.Test;

public class BaseDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }

    public BaseDbContext(DbContextOptions options) : base(options)
    { }
}