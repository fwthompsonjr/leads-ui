﻿// ChromeOlderProvider
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace legallead.records.search.DriverFactory
{
    public class ChromeOlderProvider : BaseChromeProvider, IWebDriverProvider
    {
        private const string ChromeLocationMessage = "Chrome executable location:\n {0}";

        public string Name => "Chrome Legacy";

        /// <summary>
        /// Gets the web driver.
        /// </summary>
        /// <returns></returns>
        public IWebDriver GetWebDriver(bool headless = false)
        {
            var options = GetChromeOptions();
            if (headless)
            {
                options.AddArgument("headless");
            }
            try
            {
                var legacy = $"{GetDriverFileName()}\\Legacy";
                var driver = new ChromeDriver(legacy, options);
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