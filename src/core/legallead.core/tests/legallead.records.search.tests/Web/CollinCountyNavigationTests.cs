﻿using legallead.records.search.Classes;
using legallead.records.search.Dto;
using legallead.records.search.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Diagnostics;

namespace legallead.records.search.Tests
{
    [TestClass]
    public class CollinCountyNavigationTests
    {
        [AssemblyCleanup]
        public static void AssemblyCleanUp()
        {
            const string names = "chromedriver,geckodriver";
            var processes = names.Split(',').ToList();
            processes.ForEach(p =>
            {
                var items = Process.GetProcessesByName(p).ToList();
                items.ForEach(i => i.Kill());
            });
        }

        [TestMethod]
        public void CanGetCaseTypes()
        {
            var caseTypes = CaseTypeSelectionDto.GetDto("collinCountyCaseType");
            Assert.IsNotNull(caseTypes);
        }

        [TestMethod]
        public void CanGetProbateSelection()
        {
            var caseTypes = CaseTypeSelectionDto.GetDto("collinCountyCaseType");
            Assert.IsNotNull(caseTypes);
            Assert.IsTrue(caseTypes.DropDowns.Exists(x => x.Id == 1));
            var dropDown = caseTypes.DropDowns.First(x => x.Id == 1) ?? new();
            Assert.AreEqual("probate courts", dropDown.Name);
        }

        [TestMethod]
        [TestCategory("collin.county.actions")]
        [TestCategory("Web.Integration")]
        public void CanGetFromCollinInteractive()
        {
            if (!ExecutionManagement.CanExecuteFetch())
            {
                return;
            }

            var webId = 20;
            var startDate = DateTime.Now.Date.AddDays(-4);
            var endDate = DateTime.Now.Date.AddDays(-4);
            var webParameter = BaseWebIneractive.GetWebNavigation(webId,
                startDate,
                endDate);
            var interactive = new CollinWebInteractive(webParameter, startDate, endDate);
            var result = interactive.Fetch();
            Assert.IsNotNull(result);
            ExcelWriter.WriteToExcel(result);
        }

        [TestMethod]
        [TestCategory("collin.county.actions")]
        [TestCategory("Web.Integration")]
        public void CanGetCriminalCasesFromCollinInteractive()
        {
            if (!ExecutionManagement.CanExecuteFetch())
            {
                return;
            }

            const string jsFile = @"Json\collin-criminal-case-parameter.json";
            var appFile = GetAppDirectoryName();
            appFile = Path.Combine(appFile, jsFile);
            var webId = 20;
            var startDate = DateTime.Now.Date.AddDays(-4);
            var endDate = DateTime.Now.Date.AddDays(-4);
            var webParameter = BaseWebIneractive.GetWebNavigation(webId,
                startDate,
                endDate);
            webParameter = CreateOrLoadWebParameter(webParameter, appFile);
            var interactive = new CollinWebInteractive(webParameter, startDate, endDate);
            var result = interactive.Fetch();
            Assert.IsNotNull(result);
            ExcelWriter.WriteToExcel(result);
        }

        [TestMethod]
        [TestCategory("collin.county.actions")]
        [TestCategory("Web.Integration")]
        public void CanGetProbateCasesFromCollinInteractive()
        {
            if (!ExecutionManagement.CanExecuteFetch())
            {
                return;
            }

            const string jsFile = @"Json\collin-probate-case-parameter.json";
            var appFile = GetAppDirectoryName();
            appFile = Path.Combine(appFile, jsFile);
            var webId = 20;
            var startDate = DateTime.Now.Date.AddDays(-4);
            var endDate = DateTime.Now.Date.AddDays(-4);
            var webParameter = BaseWebIneractive.GetWebNavigation(webId,
                startDate,
                endDate);
            webParameter = CreateOrLoadWebParameter(webParameter, appFile);
            var interactive = new CollinWebInteractive(webParameter, startDate, endDate);
            var result = interactive.Fetch();
            Assert.IsNotNull(result);
            ExcelWriter.WriteToExcel(result);
        }

        [TestMethod]
        [TestCategory("collin.county.actions")]
        [TestCategory("Web.Integration")]
        public void CanGetCivilAndFamilyCasesFromCollinInteractive()
        {
            if (!ExecutionManagement.CanExecuteFetch())
            {
                return;
            }

            const string jsFile = @"Json\collin-civil-and-family-case-parameter.json";
            var appFile = GetAppDirectoryName();
            appFile = Path.Combine(appFile, jsFile);
            var webId = 20;
            var startDate = DateTime.Now.Date.AddDays(-4);
            var endDate = DateTime.Now.Date.AddDays(-4);
            var webParameter = BaseWebIneractive.GetWebNavigation(webId,
                startDate,
                endDate);
            webParameter = CreateOrLoadWebParameter(webParameter, appFile);
            var interactive = new CollinWebInteractive(webParameter, startDate, endDate);
            var result = interactive.Fetch();
            Assert.IsNotNull(result);
            ExcelWriter.WriteToExcel(result);
        }

        [TestMethod]
        [TestCategory("collin.county.actions")]
        [TestCategory("Web.Integration")]
        public void CanGetJusticeCasesFromCollinInteractive()
        {
            if (!ExecutionManagement.CanExecuteFetch())
            {
                return;
            }

            const string jsFile = @"Json\collin-justice-case-parameter.json";
            var appFile = GetAppDirectoryName();
            appFile = Path.Combine(appFile, jsFile);
            var webId = 20;
            var startDate = DateTime.Now.Date.AddDays(-4);
            var endDate = DateTime.Now.Date.AddDays(-4);
            var webParameter = BaseWebIneractive.GetWebNavigation(webId,
                startDate,
                endDate);
            webParameter = CreateOrLoadWebParameter(webParameter, appFile);
            var interactive = new CollinWebInteractive(webParameter, startDate, endDate);
            var result = interactive.Fetch();
            Assert.IsNotNull(result);
            ExcelWriter.WriteToExcel(result);
        }

        #region Static Helper Functions

        private static WebNavigationParameter CreateOrLoadWebParameter(WebNavigationParameter webParameter, string jsFile)
        {
            // get key name
            if (!File.Exists(jsFile))
            {
                return webParameter;
            }

            var keyName = string.Concat(Path.GetFileName(jsFile), ".overwrite");
            var key = ConfigurationManager.AppSettings[keyName] ?? string.Empty;
            var createNewFile = key.Equals("true", StringComparison.CurrentCultureIgnoreCase);
            if (createNewFile) { CreateJsFile(webParameter, jsFile); }
            // load parameter from json
            return ReadJsFile(jsFile);
        }

        private static void CreateJsFile(WebNavigationParameter webParameter, string jsFile)
        {
            using var writer = new StreamWriter(jsFile);
            writer.Write(
            Newtonsoft.Json.JsonConvert.SerializeObject(webParameter));
        }

        private static WebNavigationParameter ReadJsFile(string jsFile)
        {
            using var reader = new StreamReader(jsFile);
            var content = reader.ReadToEnd();
            var webParameter =
                Newtonsoft.Json.JsonConvert
                .DeserializeObject<WebNavigationParameter>(content) ?? new();
            return webParameter;
        }

        private static string GetAppDirectoryName()
        {
            var navigation = new SettingsManager();
            var navFile = navigation.ExcelFormatFile;
            var folder = Path.GetDirectoryName(navFile) ?? string.Empty;
            while (new DirectoryInfo(folder).Name != "bin")
            {
                folder = new DirectoryInfo(folder).Parent!.FullName;
            }
            return new DirectoryInfo(folder).Parent!.FullName;
        }

        #endregion Static Helper Functions
    }
}