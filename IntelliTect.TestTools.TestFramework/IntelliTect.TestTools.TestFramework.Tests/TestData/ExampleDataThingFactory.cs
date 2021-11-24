using System;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class ExampleDataThingFactory
    {
        public ExampleDataThingFactory()
        {
            ExampleDataThing = GetExampleObject;
        }

        public Func<IServiceProvider, ExampleInterface> ExampleDataThing { get; private set; }

        private ExampleInterface GetExampleObject(IServiceProvider service)
        {
            return new ExampleInterface { Testing = "TestingOverride" };
        }
    }
}
