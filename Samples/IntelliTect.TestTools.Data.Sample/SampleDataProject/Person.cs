using System.Collections.Generic;

namespace SampleDataProject
{
    public class Person
    {
        public int PersonId { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

#pragma warning disable CA2227 // Collection Properties should be read only
        public ICollection<BlogPost> BlogPosts { get; set; }
#pragma warning restore CA2227 // Collection Properties should be read only
    }
}
