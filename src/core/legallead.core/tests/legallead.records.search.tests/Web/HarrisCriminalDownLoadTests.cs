using legallead.records.search.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace legallead.records.search.Tests
{
    [TestClass]
    public class HarrisCriminalDataTests : TestingBase
    {
        [TestMethod]
        public void Download_HasACorrectTarget()
        {
            var folder = HarrisCriminalData.DownloadFolder;
            folder.ShouldNotBeNullOrEmpty();
            Directory.Exists(folder).ShouldBeTrue();
        }
    }
}