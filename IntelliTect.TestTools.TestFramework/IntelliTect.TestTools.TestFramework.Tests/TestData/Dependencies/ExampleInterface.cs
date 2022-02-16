namespace IntelliTect.TestTools.TestFramework.Tests.TestData.Dependencies
{
    public interface IExampleDataInterface
    {
        string Testing { get; set; }
    }

    public class ExampleInterface : IExampleDataInterface
    {
        public string Testing { get; set; } = "Testing";
    }
}
