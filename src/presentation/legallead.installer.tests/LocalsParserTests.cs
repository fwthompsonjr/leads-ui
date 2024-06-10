using legallead.installer.Classes;

namespace legallead.installer.tests
{
    public class LocalsParserTests : BaseParserTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void ParserCanGetLatest(int testId)
        {
            var text = InstalledResult;
            var appname = testId switch
            {
                0 => AppNames[0],
                1 => AppNames[1],
                2 => AppNames[0],
                3 => AppNames[1],
                4 => "abc def ghi jk lm no p qrstu vwx yz",
                _ => string.Empty
            };
            if (testId > 1) { text = text.Replace("--", ""); }
            if (testId == 6) { text = string.Empty; }
            var sut = new LocalsParser();
            var response = sut.GetLatest(text, appname);
            if (testId > 1) Assert.Empty(response);
            else Assert.NotEmpty(response);
        }
    }
}