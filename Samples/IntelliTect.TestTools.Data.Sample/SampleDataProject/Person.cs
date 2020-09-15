using System.Collections.Generic;

namespace SampleDataProject
{
    public class Person
    {
        public int PersonId { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public ICollection<BlogPost> BlogPosts { get; set; }
    }
}
