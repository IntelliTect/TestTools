namespace IntelliTect.TestTools.TestFramework.Tests.TestData.Dependencies
{
    public interface IExampleDataInterface
    {
        string Testing { get; set; }
    }

    public class ExampleImplementation : IExampleDataInterface
    {
        public string Testing { get; set; } = "Testing";
    }

    public class OtherExampleImplementation : IExampleDataInterface
    {
        public string Testing { get; set; } = "Testing";
    }
}
