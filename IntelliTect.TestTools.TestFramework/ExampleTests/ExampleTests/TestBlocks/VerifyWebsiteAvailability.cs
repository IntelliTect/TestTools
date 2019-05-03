using System.Diagnostics;
using Xunit;

namespace ExampleTests.TestBlocks
{
    public class VerifyWebsiteAvailability
    {
        public void Execute(Data.Expected.SiteStatus expected, Data.Actual.SiteStatus actual)
        {
            Assert.Equal(expected.IsAvailable, actual.IsAvailable);
        }
    }
}
