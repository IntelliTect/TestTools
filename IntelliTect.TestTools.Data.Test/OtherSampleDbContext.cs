using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace IntelliTect.TestTools.Data.Test;

public class ReadOnlySampleDbContext : BaseDbContext
{
    public new IQueryable<Person> Persons => base.Persons;
    public new IQueryable<BlogPost> BlogPosts => base.BlogPosts;

    public ReadOnlySampleDbContext(DbContextOptions options) : base(options)
    { }
}

public class BaseDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }

    public BaseDbContext(DbContextOptions options) : base(options)
    { }
}