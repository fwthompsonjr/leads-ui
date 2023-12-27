using legallead.records.search.Classes;
using legallead.records.search.Dto;
using legallead.records.search.Interfaces;
using legallead.records.search.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Text;

namespace legallead.records.search.Tests
{
    [TestClass]
    public class TarrantCountyNavigationTests : TestingBase
    {
        #region Unit Tests

        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        public void CanGetElementActions()
        {
            var actions = ElementActions;
            Assert.IsNotNull(actions);
            Assert.IsTrue(actions.Any());
        }

        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        public void CanGetAndMapSettings()
        {
            var startingDate = DateTime.Now.AddDays(-2);
            var endingDate = DateTime.Now.AddDays(-2);
            var settings = SettingsManager
                .GetNavigation().Find(x => x.Id == 10);
            Assert.IsNotNull(settings);
            var datelist = new List<string> { "startDate", "endDate" };
            var keys = settings.Keys.FindAll(s => datelist.Contains(s.Name));
            keys[0].Value = startingDate.ToString("MM/dd/yyyy", CultureInfo.CurrentCulture);
            keys[^1].Value = endingDate.ToString("MM/dd/yyyy", CultureInfo.CurrentCulture);
            Assert.IsNotNull(settings);
        }

        private const string DataFileFoundMessage = "DataFile:= {0}";

        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        public void CanGetFromJsonInteractive()
        {
            if (!ExecutionManagement.CanExecuteFetch())
            {
                return;
            }

            var webParameter = BaseWebIneractive.GetWebNavigation(10,
                DateTime.Now.Date.AddDays(-4),
                DateTime.Now.Date.AddDays(-4));
            var interactive = new TarrantWebInteractive(webParameter);
            var result = interactive.Fetch();
            Console.WriteLine(DataFileFoundMessage, result.Result);
            Assert.IsNotNull(result);
            ExcelWriter.WriteToExcel(result);
        }

        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        [TestCategory("Web.Integration")]
        public void CanGetAndWriteFromJsonInteractive()
        {
            if (!ExecutionManagement.CanExecuteFetch())
            {
                return;
            }

            var webParameter = BaseWebIneractive.GetWebNavigation(10,
                DateTime.Now.Date.AddDays(0),
                DateTime.Now.Date.AddDays(0));
            var interactive = new TarrantWebInteractive(webParameter);
            var result = interactive.Fetch();
            Assert.IsNotNull(result);
            ExcelWriter.WriteToExcel(result);
        }

        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        public void CanNavigateSteps1()
        {
            var fileName = "tarrantCountyMapping_1";
            var steps = GetAppSteps(fileName);
            Assert.IsNotNull(steps);
            Assert.IsTrue(steps.Steps.Any());
        }

        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        public void CanNavigateSteps2()
        {
            var fileName = "tarrantCountyMapping_2";
            var steps = GetAppSteps(fileName);
            Assert.IsNotNull(steps);
            Assert.IsTrue(steps.Steps.Any());
        }

        [TestMethod]
        [TestCategory("tarrant.county.versioning")]
        public void CanInitVersionProvider()
        {
            var provider = new VersionNameProvider();
            Assert.IsNotNull(provider);
            Assert.IsFalse(string.IsNullOrEmpty(provider.Name));
            Assert.IsFalse(string.IsNullOrEmpty(VersionNameProvider.FileVersion));
            Assert.IsNotNull(VersionNameProvider.VersionNames);
        }

        [TestMethod]
        [TestCategory("tarrant.county.actions")]
        public void CanGetDropDownCourts()
        {
            var filex = TarrantWebInteractive.GetComboBoxValues();
            Assert.IsNotNull(filex);
        }

        #endregion Unit Tests

        #region Element Navigation Helpers

        private List<IElementActionBase>? elementActions;

        private List<IElementActionBase> ElementActions
        {
            get { return elementActions ??= GetElementActions(); }
        }

        private List<IElementActionBase> GetElementActions()
        {
            var container = ActionElementContainer.GetContainer;
            return container.GetAllInstances<IElementActionBase>().ToList();
        }

        private static NavigationInstructionDto GetAppSteps(string suffix = "")
        {
            const string dataFormat = @"{0}\xml\{1}.json";
            var appDirectory = ContextManagment.AppDirectory;
            var dataFile = string.Format(
                CultureInfo.InvariantCulture,
                dataFormat,
                appDirectory,
                suffix);
            var fallback = GetFallbackContent(suffix);
            var data = File.Exists(dataFile) ? File.ReadAllText(dataFile) : fallback;
            Assert.IsTrue(data.Length > 0);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<NavigationInstructionDto>(data) ?? new();
        }

        private static string GetFallbackContent(string fileName)
        {
            var sbb = new StringBuilder();
            const char tilde = '~';
            const char qte = '"';
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            if (fileName.Equals("tarrantCountyMapping_1"))
            {
                sbb.AppendLine("{");
                sbb.AppendLine("  ~steps~: [");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~navigate~,");
                sbb.AppendLine("      ~displayName~: ~open-website-base-uri~,");
                sbb.AppendLine("      ~wait~: 1200,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~page~,");
                sbb.AppendLine("        ~query~: ~https://odyssey.tarrantcounty.com/PublicAccess/default.aspx~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~exists~,");
                sbb.AppendLine("      ~displayName~: ~case-type-selector~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#sbxControlID2~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-select-value~,");
                sbb.AppendLine("      ~displayName~: ~case-type-selector~,");
                sbb.AppendLine("      ~expectedValue~: ~1~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#sbxControlID2~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~case-records-hyperlink~,");
                sbb.AppendLine("      ~expectedValue~: ~Case Records Search~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~xpath~,");
                sbb.AppendLine("        ~query~: ~/html/body/table/tbody/tr[2]/td/table/tbody/tr[1]/td[2]/a[2]~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~exists~,");
                sbb.AppendLine("      ~displayName~: ~search-by-selector~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-select-value~,");
                sbb.AppendLine("      ~displayName~: ~case-type-search~,");
                sbb.AppendLine("      ~expectedValue~: ~3~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~case-type-search~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~send-key~,");
                sbb.AppendLine("      ~displayName~: ~case-type-search~,");
                sbb.AppendLine("      ~expaectedValue~: ~K~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-text~,");
                sbb.AppendLine("      ~displayName~: ~startDate~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#DateFiledOnAfter~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-text~,");
                sbb.AppendLine("      ~displayName~: ~endDate~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#DateFiledOnBefore~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~submit-button~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchSubmit~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~break-point-ignore~,");
                sbb.AppendLine("      ~displayName~: ~submit-button~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchSubmit~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~get-record-count~,");
                sbb.AppendLine("      ~displayName~: ~record-count~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~xpath~,");
                sbb.AppendLine("        ~query~: ~/html/body/table[3]/tbody/tr[1]/td[2]/b~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~get-table-html~,");
                sbb.AppendLine("      ~displayName~: ~get-case-data-table~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~xpath~,");
                sbb.AppendLine("        ~query~: ~/html/body/table[4]~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    }");
                sbb.AppendLine("  ]");
                sbb.AppendLine("}");
            }
            if (fileName.Equals("tarrantCountyMapping_2"))
            {
                // tarrantCountyMapping_2
                sbb.AppendLine("{");
                sbb.AppendLine("  ~steps~: [");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~navigate~,");
                sbb.AppendLine("      ~displayName~: ~open-website-base-uri~,");
                sbb.AppendLine("      ~wait~: 1200,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~page~,");
                sbb.AppendLine("        ~query~: ~https://odyssey.tarrantcounty.com/PublicAccess/default.aspx~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~exists~,");
                sbb.AppendLine("      ~displayName~: ~case-type-selector~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#sbxControlID2~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-select-value~,");
                sbb.AppendLine("      ~displayName~: ~case-type-selector~,");
                sbb.AppendLine("      ~expectedValue~: ~1~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#sbxControlID2~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~criminal-records-hyperlink~,");
                sbb.AppendLine("      ~expectedValue~: ~Criminal Case Records~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~xpath~,");
                sbb.AppendLine("        ~query~: ~//a[@class='ssSearchHyperlink'][contains(text(),'Misdemeanors')]~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~exists~,");
                sbb.AppendLine("      ~displayName~: ~search-by-selector~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-dropdown-value~,");
                sbb.AppendLine("      ~displayName~: ~case-type-search~,");
                sbb.AppendLine("      ~expectedValue~: ~5~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~case-type-search~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~send-key~,");
                sbb.AppendLine("      ~displayName~: ~case-type-search~,");
                sbb.AppendLine("      ~expaectedValue~: ~K~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-text~,");
                sbb.AppendLine("      ~displayName~: ~startDate~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#DateFiledOnAfter~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-text~,");
                sbb.AppendLine("      ~displayName~: ~endDate~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#DateFiledOnBefore~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~submit-button~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchSubmit~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~break-point-ignore~,");
                sbb.AppendLine("      ~displayName~: ~submit-button~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchSubmit~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~get-record-count~,");
                sbb.AppendLine("      ~displayName~: ~record-count~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~xpath~,");
                sbb.AppendLine("        ~query~: ~/html/body/table[3]/tbody/tr[1]/td[2]/b~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~get-table-html~,");
                sbb.AppendLine("      ~displayName~: ~get-case-data-table~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~xpath~,");
                sbb.AppendLine("        ~query~: ~/html/body/table[4]~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    }");
                sbb.AppendLine("  ]");
                sbb.AppendLine("}");
            }
            sbb.Replace(tilde, qte);
            return sbb.ToString();
        }

        #endregion Element Navigation Helpers
    }
}