using Harris.Criminal.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Harris.Criminal.UnitTests
{
    [TestClass]
    public class StartupTests
    {
        [TestMethod]
        public async Task CanRead_Async()
        {
            var progressHandler = new Progress<bool>(isComplete =>
            {
                if (isComplete)
                {
                    Console.WriteLine("Task has completed");
                }
            });
            var progress = progressHandler as IProgress<bool>;
            await Startup.ReadAsync(progress);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void CanGet_DownloadFolder()
        {
            Startup.DataFolder.ShouldNotBeNullOrEmpty();
            Directory.Exists(Startup.DataFolder).ShouldBeTrue();
        }
    }
}