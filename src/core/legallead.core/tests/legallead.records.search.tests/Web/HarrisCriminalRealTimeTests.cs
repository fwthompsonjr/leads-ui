using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.IO;
using System.Reflection;
using legallead.records.search.Web;

namespace legallead.records.search.UnitTests.Web
{
    [TestClass]
    public class HarrisCriminalRealTimeTests
    {
        private static string? _srcDirectory;
        private static string? _srcFile;
        private static string SrcDirectoryName => _srcDirectory ??= SrcDir();
        private static string SrcFile => _srcFile ??= Path.Combine(SrcDirectoryName, "_html\\sample-harris-criminal-search-result.html");
        private IWebDriver? GetDriver;

        [TestInitialize]
        public void Setup()
        {
#if DEBUG
            if (GetDriver == null)
            {
                var src = SrcFile.Replace(@"\", "/");
                var url = string.Concat("file:", "///", src);
                GetDriver = new FirefoxDriver
                {
                    Url = url
                };
            }
#endif
        }
        [TestCleanup]
        public void CleanUp()
        {
#if DEBUG
            if (GetDriver != null)
            {
                GetDriver.Close();
                GetDriver.Quit();
                GetDriver.Dispose();
                GetDriver = null;
            }
#endif
        }

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
            Assert.Inconclusive("Test only runs in debug configuration.");            
#endif
        }

        private static string SrcDir()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return Path.GetDirectoryName(assembly.Location) ?? string.Empty;
        }
    }
}
