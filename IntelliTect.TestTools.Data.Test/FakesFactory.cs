using System;
using System.Linq;
using System.Reflection;
using Bogus;
using IntelliTect.TestTools.Data.Test;
using Person = IntelliTect.TestTools.Data.Test.Person;

namespace IntelliTect.IntelliTime.Data.Test.Util
{
    public static class FakesFactory
    {
        private static readonly object[] _AllFakers;

        private static readonly Faker<Person> _PersonFaker;
        private static readonly Faker<BlogPost> _BlogPostFaker;

        static FakesFactory()
        {
            _PersonFaker = new Faker<Person>()
                .StrictMode(true)
                .Ignore(x => x.PersonId)
                .Ignore(x => x.BlogPosts)
                .RuleFor(x => x.Age, f => f.Random.Int(1, 112))
                .RuleFor(x => x.Name, f => f.Person.FirstName);

            _BlogPostFaker = new Faker<BlogPost>()
                .StrictMode(false)
                .Ignore(x => x.BlogPostId)
                .Ignore(x => x.PersonId)
                .RuleFor(x => x.DateCreated, DateTimeOffset.Now)
                .RuleFor(x => x.NumberOfComments, f => f.Random.Int(0, 100));

            _AllFakers = typeof(FakesFactory).GetFields(BindingFlags.GetField
                                                        | BindingFlags.Static
                                                        | BindingFlags.NonPublic)
                .Where(x => x.Name.ToLower().EndsWith("faker"))
                .Select(field => field.GetValue(null))
                .ToArray();
        }

        public static T Create<T>() where T : class
        {
            Faker<T> faker = _AllFakers.OfType<Faker<T>>().FirstOrDefault();

            if (faker is null)
            {
                throw new InvalidOperationException(
                    $"Cannot construct and instance of type '{typeof(T).FullName}'");
            }

            return faker.Generate();
        }
    }
}
