using IntelliTect.TestTools.TestFramework.Logging;

namespace IntelliTect.TestTools.TestFramework
{
    public interface ITestBlock
    {
        ITestCaseLogger? Log { get; }
    }
}