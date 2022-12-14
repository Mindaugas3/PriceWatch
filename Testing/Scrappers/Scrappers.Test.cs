using ASP.NETCoreWebApplication.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Testing
{
    // NOTE
    // unit tests functions start with [Entity] Should [Do something]
    // Data and helper functions, compiling test modules are hidden in Fixture classes
    // With unit tests we test as long as no I/O is there
    // Unit tests should be no more than 4 lines long and should be as simple to read as possible
    // ~ Mindaugas
    public class AruodasTest
    {
        private ScrappersFixture _fixture = ScrappersFixture.Current;
        [Fact]
        public void AruodasScrapperShouldBuildCorrectUrl()
        {
            string expected = ScrappersFixture.GetExpectedAruodasUrl();
            string actual = ScrappersFixture.CreateTestAruodasUrl();
            Assert.Equal(expected, actual);
        }
    }
}