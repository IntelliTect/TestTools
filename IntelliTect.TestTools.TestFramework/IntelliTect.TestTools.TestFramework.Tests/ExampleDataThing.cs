namespace IntelliTect.TestTools.TestFramework.Tests
{
    public interface IExampleDataInterface
    {
        string Testing { get; set; }
    }

    public class ExampleDataThing : IExampleDataInterface
    {
        public string Testing { get; set; } = "Testing";
    }
}
