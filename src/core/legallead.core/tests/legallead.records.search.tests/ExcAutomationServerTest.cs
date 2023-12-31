﻿using legallead.records.search.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace legallead.records.search.Tests
{
    [TestClass]
    public class ExcAutomationServerTest
    {
        [TestMethod]
        [TestCategory("Excel.Automation.Tests")]
        public void CanOpenExcel()
        {
            try
            {
                ExcAutomationServer.Open("");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}