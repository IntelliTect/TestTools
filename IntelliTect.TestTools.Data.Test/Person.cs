using System.Collections.Generic;

namespace IntelliTect.TestTools.Data.Test;

public class Person
{
    public int PersonId { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }

    public ICollection<BlogPost> BlogPosts { get; set; }
}