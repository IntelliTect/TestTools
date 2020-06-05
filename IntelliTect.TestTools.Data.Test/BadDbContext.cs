using Microsoft.EntityFrameworkCore;

namespace IntelliTect.TestTools.Data.Test
{
    public class BadDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        public BadDbContext()
        {
        }
    }
}
