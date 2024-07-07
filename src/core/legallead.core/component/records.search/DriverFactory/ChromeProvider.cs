using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace legallead.records.search.DriverFactory
{
    public class ChromeProvider : BaseChromeProvider, IWebDriverProvider
    {
        public string Name => "Chrome";

        /// <summary>
        /// Gets the web driver.
        /// </summary>
        /// <returns></returns>
        public IWebDriver GetWebDriver(bool headless = false)
        {
            ChromeOptions options = new();
            string binaryName = BinaryFileName();
            if (!string.IsNullOrEmpty(binaryName))
            {
                options.BinaryLocation = binaryName;
            }
            try
            {
                if (headless)
                {
                    options.AddArgument("headless");
                }
                options.AddArgument("guest");
                options.AddUserProfilePreference("reduce-security-for-testing", null);
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("download.directory_upgrade", true);
                options.AddUserProfilePreference("download.default_directory", CalculateDownloadPath());
                options.UnhandledPromptBehavior = UnhandledPromptBehavior.Accept;
                ChromeDriver driver = new(GetDriverFileName(), options);
                Console.WriteLine("Chrome executable location:\n {0}", binaryName);
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