using Microsoft.EntityFrameworkCore;

namespace IntelliTect.TestTools.Data.Test;

public class OtherSampleDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }

    public OtherSampleDbContext(DbContextOptions<OtherSampleDbContext> options) : base(options)
    { }
}

public class OtherSampleDbContextCopy : DbContext 
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<UnusedModel> UnusedModels { get; set; }

    public OtherSampleDbContextCopy(DbContextOptions<OtherSampleDbContextCopy> options) : base(options)
    { }
}