using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace legallead.search.api.tests.Utility
{
    internal class LocalContentDriver : IDisposable
    {
        private readonly IWebDriver? driver;
        private string? LocalFile;
        private bool disposedValue;

        public LocalContentDriver(string html)
        {
            var obj = GetDriver(html);
            if (obj != null) { driver = obj; }
        }
        public IWebDriver? Driver => driver;

        private IWebDriver? GetDriver(string html)
        {
            var tmp = Path.GetTempFileName();
            try
            {
                var opt = new FirefoxOptions();
                opt.AddArgument("--headless");
                var instance = new FirefoxDriver(opt);
                var dir = Path.GetDirectoryName(tmp);
                if (!Directory.Exists(dir)) { return null; }
                var resourceName = $"{Path.GetFileNameWithoutExtension(tmp)}.html";
                LocalFile = Path.Combine(dir, resourceName);
                File.WriteAllText(LocalFile, html);
                var url = $"file://{LocalFile}";
                instance.Navigate().GoToUrl(url);
                return instance;
            }
            finally
            {
                if (File.Exists(tmp)) { File.Delete(tmp); }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    driver?.Quit();
                    driver?.Dispose();
                    if (!string.IsNullOrEmpty(LocalFile) && File.Exists(LocalFile))
                    {
                        File.Delete(LocalFile);
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
