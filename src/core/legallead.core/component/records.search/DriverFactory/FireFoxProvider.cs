using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace legallead.records.search.DriverFactory
{
    public class FireFoxProvider : IWebDriverProvider
    {
        public string Name => "Firefox";

        /// <summary>
        /// Gets the web driver.
        /// </summary>
        /// <returns></returns>
        public IWebDriver GetWebDriver(bool headless = false)
        {
            IWebDriver? driver = GetDefaultDriver();
            if (driver != null)
            {
                return driver;
            }
            return new FirefoxDriver(GetDriverFileName());
        }

        private static string? _driverFileName;

        /// <summary>
        /// Gets the default driver.
        /// </summary>
        /// <returns></returns>
        private static IWebDriver? GetDefaultDriver()
        {
            try
            {
                var isWindows = IsWindows();
                if (!isWindows) return GetLinuxDriver();
                var profile = new FirefoxOptions();
                profile.SetPreference("browser.safebrowsing.enabled", true);
                profile.SetPreference("browser.safebrowsing.malware.enabled", true);
                profile.UnhandledPromptBehavior = UnhandledPromptBehavior.Accept;
                return new FirefoxDriver(profile);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the name of the chrome driver file.
        /// </summary>
        /// <returns></returns>
        private static string GetDriverFileName()
        {
            if (_driverFileName != null)
            {
                return _driverFileName;
            }

            string? execName = new Uri(Assembly.GetExecutingAssembly().Location).AbsolutePath;
            execName = Path.GetDirectoryName(execName);
            if (!Directory.Exists(execName))
            {
                _driverFileName = string.Empty;
                return string.Empty;
            }
            _driverFileName = execName;
            return execName;
        }

        private static IWebDriver? GetLinuxDriver()
        {
            if (IsWindows()) return null;
            return LinuxDriverProvider.GetDriver();
        }

        private static bool IsWindows()
        {

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            return isWindows;
        }

        private static class LinuxDriverProvider
        {
            public static IWebDriver? GetDriver()
            {
                try
                {
                    var environmentDir = Environment.GetEnvironmentVariable("HOME");
                    if (string.IsNullOrEmpty(environmentDir) ||
                        string.IsNullOrEmpty(DriverDirectory) ||
                        string.IsNullOrEmpty(BinaryFile)) { return false; }

                    var downloadDir = Path.Combine(environmentDir, "download");
                    return GetDriver(1, downloadDir);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return null;
                }
            }

            private static FirefoxOptions GetOptions(int mode, string downloadDir)
            {

                var profile = new FirefoxOptions();
                if (mode == 0)
                {
                    profile.BrowserExecutableLocation = BinaryFile;
                }

                profile.AddArguments("-headless");
                profile.AddArguments("--headless");
                profile.AddAdditionalCapability("platform", "LINUX", true);
                profile.AddAdditionalCapability("video", "True", true);
                profile.SetPreference("download.default_directory", downloadDir);
                profile.SetPreference("browser.safebrowsing.enabled", true);
                profile.SetPreference("browser.safebrowsing.malware.enabled", true);
                profile.UnhandledPromptBehavior = UnhandledPromptBehavior.Accept;
                return profile;
            }
            private static FirefoxDriver GetDriver(int mode, string downloadDir)
            {
                var options = GetOptions(mode, downloadDir);
                var driver = mode switch
                {
                    0 => new FirefoxDriver(DriverDirectory, options),
                    1 => new FirefoxDriver(options),
                    _ => new FirefoxDriver()
                };
                return driver;
            }

            private static string DriverDirectory => driverDirectory ??= GetDriverDirectoryName();
            private static string? driverDirectory;
            private static string GetDriverDirectoryName()
            {
                var environmentDir = Environment.GetEnvironmentVariable("HOME");
                if (string.IsNullOrEmpty(environmentDir)) { return string.Empty; }
                var destinationDir = Path.Combine(environmentDir, "util");
                var geckoDir = Path.Combine(destinationDir, "gecko");
                return geckoDir;
            }



            private static string BinaryFile => binaryFile ??= GetBinaryFileName();
            private static string? binaryFile;
            private static string GetBinaryFileName()
            {
                var environmentDir = Environment.GetEnvironmentVariable("HOME");
                if (string.IsNullOrEmpty(environmentDir)) { return string.Empty; }
                var firefoxDir = Path.Combine(environmentDir, "firefox");
                var subfolders = 0;
                var firefoxFile = Path.Combine(firefoxDir, "firefox");
                while (!File.Exists(firefoxFile))
                {
                    if (subfolders > 5) return string.Empty;
                    firefoxFile = Path.Combine(firefoxFile, "firefox");
                    subfolders++;
                }
                return firefoxFile;
            }
        }

    }
}