using legallead.records.search.Classes;
using legallead.records.search.Dto;
using legallead.records.search.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Globalization;

namespace legallead.records.search.tests.Web
{
    [TestClass]
    public class HarrisJpIntegrationTest
    {
        const int HarrisJpId = 50;
        [TestMethod]
        [TestCategory("harris.jp.county.actions")]
        public void CanGetAndMapSettings()
        {
            var startingDate = DateTime.Now.AddDays(-2);
            var endingDate = DateTime.Now.AddDays(-2);
            var settings = SettingsManager.GetNavigation().Find(x => x.Id == HarrisJpId);
            Assert.IsNotNull(settings);
            var datelist = new List<string> { "startDate", "endDate" };
            var keys = settings.Keys.FindAll(s => datelist.Contains(s.Name));
            keys[0].Value = startingDate.ToString("MM/dd/yyyy", CultureInfo.CurrentCulture);
            keys[^1].Value = endingDate.ToString("MM/dd/yyyy", CultureInfo.CurrentCulture);
            Assert.IsNotNull(settings);
        }

        [TestMethod]
        [TestCategory("harris.jp.county.actions")]
        public void CanGetNavigationSteps()
        {
            var steps = MockHarrisJpWeb.GetDto();
            Assert.IsNotNull(steps);
            Assert.IsNotNull(steps.Steps);
            Assert.AreEqual(9, steps.Steps.Count);
        }

        [TestMethod]
        [TestCategory("harris.jp.county.actions")]
        [DataRow("a")]
        [DataRow("0")]
        [DataRow("1")]
        [DataRow("2")]
        public void CanGetFromJsonInteractive(string courtIndex = "0")
        {
            if (!Debugger.IsAttached) return;
            var interactive = GetHarrisJpInteractive(courtIndex);
            var result = interactive.Fetch();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.PeopleList);
            Assert.IsTrue(result.PeopleList.Count != 0);
        }

        private static MockHarrisJpWeb GetHarrisJpInteractive(string courtIndex = "0")
        {

            DayOfWeek[] weekends = [DayOfWeek.Sunday, DayOfWeek.Saturday];
            var dte = DateTime.Now.Date.AddDays(-4);
            while (weekends.Contains(dte.DayOfWeek))
            {
                dte = dte.AddDays(-1);
            }
            var dteEnd = dte.AddDays(4);
            var webParameter = BaseWebIneractive.GetWebNavigation(HarrisJpId, dte, dteEnd);
            var custom = new List<WebNavigationKey>
            {
                new () { Name = "courtIndex", Value = courtIndex },
                new () { Name = "caseStatusIndex", Value = "0" }
            };
            custom.ForEach(x => { AddOrUpdateKey(webParameter.Keys, x); });
            return new MockHarrisJpWeb(webParameter);
        }

        private static void AddOrUpdateKey(List<WebNavigationKey> list, WebNavigationKey model)
        {
            var found = list.Find(x => x.Name.Equals(model.Name));
            if (found == null)
            {
                list.Add(model);
                return;
            }
            found.Value = model.Value;
        }

        private sealed class MockHarrisJpWeb(WebNavigationParameter parameters) : HarrisJpInteractive(parameters)
        {
            public static NavigationInstructionDto GetDto() => GetAppSteps("harrisJpMapping");
        }
    }
}