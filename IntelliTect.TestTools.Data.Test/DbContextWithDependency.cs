using System;
using Microsoft.EntityFrameworkCore;

namespace IntelliTect.TestTools.Data.Test
{
    public class DbContextWithDependency : DbContext
    {
        public IDependency MyDependency { get; }

        public DbContextWithDependency()
        {
        }

        public DbContextWithDependency(DbContextOptions options, IDependency someOtherDependency) 
            : base(options)
        {
            MyDependency = someOtherDependency ?? throw new ArgumentNullException(nameof(someOtherDependency));
        }
    }

    public interface IDependency
    { }
}
