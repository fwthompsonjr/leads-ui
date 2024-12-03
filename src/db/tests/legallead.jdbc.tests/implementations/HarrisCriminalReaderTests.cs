using legallead.jdbc.implementations;
using System.Diagnostics;

namespace legallead.jdbc.tests.implementations
{
    public class HarrisCriminalReaderTests
    {
        [Fact]
        public async Task ServiceCanNavigate()
        {
            if (!Debugger.IsAttached) return;
            var service = new HarrisCriminalReader();
            var response = await service.Navigate();
            Assert.True(response);
        }
        [Theory]
        [InlineData("CrimFilingsWithFutureSettings_withHeadings", true)]
        [InlineData("CrimFilingsMonthly_withHeadings", true)]
        public async Task ServiceCanFetch(string find, bool expected)
        {
            if (!Debugger.IsAttached) return;
            var service = new HarrisCriminalReader();
            _ = await service.Navigate();
            var response = await service.Fetch(find);
            Assert.Equal(expected, response);
        }
    }
}
