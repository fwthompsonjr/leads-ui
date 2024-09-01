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
                profile.AddArguments("--headless");
                profile.AddAdditionalCapability("video", "True", true);
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
                    var environmentDir = TheEnvironment.GetHomeFolder();
                    var binaryFile = GetBinaryFileName();
                    var driverDir = GetDriverDirectoryName();
                    if (string.IsNullOrEmpty(environmentDir) ||
                        string.IsNullOrEmpty(driverDir) ||
                        string.IsNullOrEmpty(binaryFile)) { return null; }

                    return GetDriver(1);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return null;
                }
            }

            private static FirefoxOptions GetOptions(int mode, string downloadDir)
            {
                var locations = _knownPaths.FindAll(x => Directory.Exists(x));
                locations.ForEach(name => AppendToPath(name));

                var profile = new FirefoxOptions();
                if (mode == 0)
                {
                    var binaryFile = GetBinaryFileName();
                    profile.BrowserExecutableLocation = binaryFile;
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
            private static FirefoxDriver GetDriver(int mode)
            {
                var options = GetOptions(mode, GetDriverDirectoryName());
                var driver = mode switch
                {
                    0 => new FirefoxDriver(GetDriverDirectoryName(), options),
                    1 => new FirefoxDriver(options),
                    _ => new FirefoxDriver()
                };
                return driver;
            }

            private static string GetDriverDirectoryName()
            {
                var environmentDir = TheEnvironment.GetHomeFolder();
                if (string.IsNullOrEmpty(environmentDir)) { return string.Empty; }
                var destinationDir = Path.Combine(environmentDir, "util");
                var geckoDir = Path.Combine(destinationDir, "gecko");
                return geckoDir;
            }

            private static string GetBinaryFileName()
            {
                var environmentDir = TheEnvironment.GetHomeFolder();
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


            private static void AppendToPath(string? keyValue)
            {
                const char colon = ':';
                const char semicolon = ';';
                const string name = "PATH";
                if (string.IsNullOrEmpty(keyValue)) return;
                var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                var separator = isWindows ? semicolon : colon;
                var scope = EnvironmentVariableTarget.User;
                var oldValue = Environment.GetEnvironmentVariable(name, scope) ?? string.Empty;
                var items = oldValue.Split(separator).ToList();
                if (items.Contains(keyValue)) return;
                items.Add(keyValue);
                var newValue = string.Join(separator, items);
                Environment.SetEnvironmentVariable(name, newValue, scope);
            }
            private static readonly List<string> _knownPaths = new() {
                "/var/app/current",
                "/var/app/current/geckodriver",
                "/home/webapp/.local/share"
            };
        }

    }
}