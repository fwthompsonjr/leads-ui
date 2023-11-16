using OpenQA.Selenium;

namespace legallead.records.search.DriverFactory
{
    public interface IWebDriverProvider
    {
        /// <summary>
        /// Gets the web driver.
        /// </summary>
        /// <returns></returns>
        IWebDriver GetWebDriver(bool headless = false);

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }
    }
}