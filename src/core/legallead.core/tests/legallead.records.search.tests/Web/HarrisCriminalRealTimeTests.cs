
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if DEBUG

using System.Reflection;
using legallead.records.search.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

#endif

namespace legallead.records.search.UnitTests.Web
{
    [TestClass]
    public class HarrisCriminalRealTimeTests
    {

#if DEBUG

        private static string? _srcDirectory;
        private static string? _srcFile;
        private static string SrcDirectoryName => _srcDirectory ??= SrcDir();
        private static string SrcFile =>
            _srcFile ??= Path.Combine(SrcDirectoryName, "_html\\sample-harris-criminal-search-result.html");
        private IWebDriver? GetDriver;

        [TestInitialize]
        public void Setup()
        {
            if (GetDriver == null)
            {
                var src = SrcFile.Replace(@"\", "/");
                var url = string.Concat("file:", "///", src);
                GetDriver = new FirefoxDriver
                {
                    Url = url
                };
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (GetDriver != null)
            {
                GetDriver.Close();
                GetDriver.Quit();
                GetDriver.Dispose();
                GetDriver = null;
            }
        }

        private static string SrcDir()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return Path.GetDirectoryName(assembly.Location) ?? string.Empty;
        }
#endif


        [TestMethod]
        public void Criminal_CanIterate()
        {
#if DEBUG
            const int expected = 4331;
            var obj = new HarrisCriminalRealTime();
            try
            {
                var driver = GetDriver;
                Assert.IsNotNull(driver);
                var result = obj.IteratePages(driver);
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Count > 0);
                Assert.AreEqual(expected, result.Count);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + Environment.NewLine + ex.StackTrace);
            }
#else
            Assert.IsTrue(true);
#endif
        }
    }
}