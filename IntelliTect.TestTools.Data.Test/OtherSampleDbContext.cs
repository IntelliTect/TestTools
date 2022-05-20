using Microsoft.EntityFrameworkCore;

namespace IntelliTect.TestTools.Data.Test;

public class OtherSampleDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }

    public OtherSampleDbContext(DbContextOptions<OtherSampleDbContext> options) : base(options)
    { }
}