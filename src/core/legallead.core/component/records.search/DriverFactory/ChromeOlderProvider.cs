// ChromeOlderProvider
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace legallead.records.search.DriverFactory
{
    public class ChromeOlderProvider : BaseChromeProvider, IWebDriverProvider
    {
        public string Name => "Chrome Legacy";

        /// <summary>
        /// Gets the web driver.
        /// </summary>
        /// <returns></returns>
        public IWebDriver GetWebDriver(bool headless = false)
        {
            ChromeOptions options = GetChromeOptions();
            if (headless)
            {
                options.AddArgument("headless");
            }
            try
            {
                string legacy = $"{GetDriverFileName()}\\Legacy";
                ChromeDriver driver = new(legacy, options);
                return driver;
            }
            catch (Exception)
            {
                return new ChromeDriver(GetDriverFileName());
                throw;
            }
        }
    }
}