using System;

namespace IntelliTect.TestTools.TestFramework.Tests.TestData.Dependencies
{
    public class ExampleFactory
    {
        public ExampleFactory()
        {
            DoesNotThrow = GetExampleObject;
            Throws = GetAlwaysThrow;
        }

        public Func<IServiceProvider, AlwaysThrow> Throws { get; }
        public Func<IServiceProvider, ExampleInterface> DoesNotThrow { get; private set; }

        private ExampleInterface GetExampleObject(IServiceProvider service)
        {
            return new ExampleInterface { Testing = "TestingOverride" };
        }

        private AlwaysThrow GetAlwaysThrow(IServiceProvider provider)
        {
            return new AlwaysThrow();
        }
    }

    public class AlwaysThrow
    {
        public AlwaysThrow()
        {
            throw new InvalidOperationException("Oops");
        }
    }

    public class SomeDependency
    {
        public SomeDependency(AlwaysThrow alwaysThrow)
        {
            AlwaysThrow = alwaysThrow;
        }

        public AlwaysThrow AlwaysThrow { get; }
    }
}
