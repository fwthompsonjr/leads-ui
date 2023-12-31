﻿using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Reflection;

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
                return new FirefoxDriver();
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
    }
}