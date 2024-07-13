using legallead.models.Search;
using legallead.records.search.Classes;
using legallead.records.search.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Globalization;

namespace legallead.records.search.tests.Web
{
    [TestClass]
    public class HarrisCivilIntegrationTest
    {
        const int HarrisCivilId = 30;
        [TestMethod]
        [TestCategory("harris.county.actions")]
        public void CanGetAndMapSettings()
        {
            var startingDate = DateTime.Now.AddDays(-2);
            var endingDate = DateTime.Now.AddDays(-2);
            var settings = SettingsManager.GetNavigation().Find(x => x.Id == HarrisCivilId);
            Assert.IsNotNull(settings);
            var datelist = new List<string> { "startDate", "endDate" };
            var keys = settings.Keys.FindAll(s => datelist.Contains(s.Name));
            keys[0].Value = startingDate.ToString("MM/dd/yyyy", CultureInfo.CurrentCulture);
            keys[^1].Value = endingDate.ToString("MM/dd/yyyy", CultureInfo.CurrentCulture);
            Assert.IsNotNull(settings);
        }

        [TestMethod]
        [TestCategory("harris.county.actions")]
        public void CanGetFromJsonInteractive()
        {
            if (!Debugger.IsAttached) return;
            DayOfWeek[] weekends = new[] { DayOfWeek.Sunday, DayOfWeek.Saturday };
            var dte = DateTime.Now.Date.AddDays(-4);
            while (weekends.Contains(dte.DayOfWeek))
            {
                dte = dte.AddDays(-1);
            }
            var webParameter = BaseWebIneractive.GetWebNavigation(HarrisCivilId, dte, dte);
            var custom = new List<WebNavigationKey>
            {
                new () { Name = "courtIndex", Value = "0" },
                new () { Name = "caseStatusIndex", Value = "0" }
            };
            custom.ForEach(x => { AddOrUpdateKey(webParameter.Keys, x); });
            var interactive = new HarrisCivilInteractive(webParameter);
            var result = interactive.Fetch();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.PeopleList);
            Assert.IsTrue(result.PeopleList.Any());
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
    }
}
