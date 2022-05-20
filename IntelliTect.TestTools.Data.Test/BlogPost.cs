using System;

namespace IntelliTect.TestTools.Data.Test;

public class BlogPost
{
    public int BlogPostId { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public int NumberOfComments { get; set; }

    public int PersonId { get; set; }

    public Person Person { get; set; }
}