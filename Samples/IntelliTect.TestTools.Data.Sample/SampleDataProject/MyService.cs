using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SampleDataProject
{
    public class MyService
    {
        private SampleDbContext _Db;

        public MyService(SampleDbContext db)
        {
            _Db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // This service is supposed to return a collection of BlogPosts with the person property populated.
        // However, since it lacks a .Include(x => x.Person) in a production setting with a properly scoped DbContext,
        // the Person property will not be populated.
        public async Task<ICollection<BlogPost>> GetBlogsByPerson(string name)
        {
            return await _Db.BlogPosts
                .Where(x => x.Person.Name == name)
                .ToListAsync();
        }
    }
}
