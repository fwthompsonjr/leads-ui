using legallead.jdbc.implementations;

namespace legallead.jdbc.tests.implementations
{
    public class HarrisCriminalReaderTests
    {
        [Fact]
        public async Task ServiceCanNavigate()
        {
            var service = new HarrisCriminalReader();
            var response = await service.Navigate();
            Assert.True(response);
        }
        [Theory]
        [InlineData("CrimFilingsWithFutureSettings_withHeadings", true)]
        [InlineData("CrimFilingsMonthly_withHeadings", true)]
        public async Task ServiceCanFetch(string find, bool expected)
        {

            var service = new HarrisCriminalReader();
            _ = await service.Navigate();
            var response = await service.Fetch(find);
            Assert.Equal(expected, response);
        }
    }
}
