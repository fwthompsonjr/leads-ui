using legallead.search.api.tests.Utility;
using OpenQA.Selenium;

namespace legallead.search.api.tests
{
    public class LocalContentTests
    {

        [Fact]
        public void LocalDriverCanLoad()
        {
            var html = Properties.Resources.tx_denton_normal_search_result;
            Assert.False(string.IsNullOrWhiteSpace(html));
            using var web = new LocalContentDriver(html);
            Assert.NotNull(web.Driver);
            var driver = web.Driver;
            var element = driver.FindElement(By.TagName("table"));
            Assert.NotNull(element);
        }
    }
}
