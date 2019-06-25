using System;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class ExampleDataThingFactory
    {
        public ExampleDataThingFactory()
        {
            ExampleDataThing = GetExampleObject;
        }

        public Func<IServiceProvider, ExampleDataThing> ExampleDataThing { get; private set; }

        private ExampleDataThing GetExampleObject(IServiceProvider service)
        {
            return new ExampleDataThing { Testing = "TestingOverride" };
        }
    }
}
