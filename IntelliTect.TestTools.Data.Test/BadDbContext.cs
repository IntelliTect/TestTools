using System;
using Microsoft.EntityFrameworkCore;

namespace IntelliTect.TestTools.Data.Test
{
    public class BadDbContext : DbContext
    {
        private object MyDependency { get; }

        public BadDbContext()
        {
        }

        public BadDbContext(DbContextOptions options, object someOtherDependency) : base(options)
        {
            MyDependency = someOtherDependency ?? throw new ArgumentNullException(nameof(someOtherDependency));
        }
    }
}
