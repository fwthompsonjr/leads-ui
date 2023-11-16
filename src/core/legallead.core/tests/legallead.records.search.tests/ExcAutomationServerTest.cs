using Microsoft.VisualStudio.TestTools.UnitTesting;
using legallead.records.search.Classes;

namespace legallead.records.search.Tests
{
    [TestClass]
    public class ExcAutomationServerTest
    {
        [TestMethod]
        [TestCategory("Excel.Automation.Tests")]
        public void CanOpenExcel()
        {
            ExcAutomationServer.Open("");
        }
    }
}
