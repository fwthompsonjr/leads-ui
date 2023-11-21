﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;
using System.Xml;
using legallead.records.search.Classes;
using legallead.records.search.Dto;
using Shouldly;

namespace legallead.records.search.Tests
{
    [TestClass]
    public class SettingsManagerTest
    {
        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void CanGetNavigation()
        {
            var navigation = new SettingsManager();
            Assert.IsNotNull(navigation);
        }


        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void CanGetNavigationContent()
        {
            Assert.IsFalse(string.IsNullOrEmpty(SettingsManager.Content));
        }

        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void CanGetExcelFormatFile()
        {
            var navigation = new SettingsManager();
            Assert.IsFalse(string.IsNullOrEmpty(navigation.ExcelFormatFile));
        }


        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void CanGetNavigationLayout()
        {
            var navigation = new SettingsManager();
            Assert.IsFalse(string.IsNullOrEmpty(navigation.Layout));
        }


        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void CanGetNavigationWebsites()
        {
            var parms = SettingsManager.GetNavigation();
            Assert.IsNotNull(parms);
            Assert.IsTrue(parms.Count > 0);
        }

        [TestMethod]
        [TestCategory("Configuration.Integration")]
        public void CanGetGetOutput()
        {
            var navigation = new SettingsManager();
            var parms = SettingsManager.GetNavigation();
            Assert.IsNotNull(parms);
            Assert.IsTrue(parms.Count > 0);
            var sttg = parms[0];
            var startDate = DateTime.Now.AddDays(-3);
            var endingDate = DateTime.Now.AddDays(-1);
            var webactive = new WebInteractive(sttg, startDate, endingDate);
            var dx = navigation.GetOutput(webactive);
            Assert.IsNotNull(dx);
        }


        [TestMethod]
        [TestCategory("Configuration.Integration")]
        public void CanGetGetOutputData()
        {
            var navigation = new SettingsManager();
            var parms = SettingsManager.GetNavigation();
            Assert.IsNotNull(parms);
            Assert.IsTrue(parms.Count > 0);
            var sttg = parms[0];
            var startDate = DateTime.Now.AddDays(-3);
            var endingDate = DateTime.Now.AddDays(-1);
            var webactive = new WebInteractive(sttg, startDate, endingDate);
            var dx = navigation.GetOutput(webactive);
            Assert.IsNotNull(dx);
            Assert.IsNotNull(dx.Data);
        }


        [TestMethod]
        [TestCategory("Configuration.Integration")]
        public void CanGetGetOutputPeople()
        {
            var navigation = new SettingsManager();
            var parms = SettingsManager.GetNavigation();
            Assert.IsNotNull(parms);
            Assert.IsTrue(parms.Count > 0);
            var sttg = parms[0];
            var startDate = DateTime.Now.AddDays(-3);
            var endingDate = DateTime.Now.AddDays(-1);
            var webactive = new WebInteractive(sttg, startDate, endingDate);
            var dx = navigation.GetOutput(webactive);
            Assert.IsNotNull(dx);
            Assert.IsNotNull(dx.People);
        }

        [TestMethod]
        [TestCategory("Configuration.Integration")]
        public void CanAppendOutputPeople()
        {
            var navigation = new SettingsManager();
            var parms = SettingsManager.GetNavigation();
            Assert.IsNotNull(parms);
            Assert.IsTrue(parms.Count > 0);
            var sttg = parms[0];
            var startDate = DateTime.Now.AddDays(-3);
            var endingDate = DateTime.Now.AddDays(-1);
            var webactive = new WebInteractive(sttg, startDate, endingDate);
            var dx = navigation.GetOutput(webactive);
            Assert.IsNotNull(dx);
            Assert.IsNotNull(dx.People);

            dx.Append(new Models.HLinkDataRow
            {
                Address = "1234 Somewhare",
                Data = TestDataRow(),
                Defendant = "Some Person"
            });
        }


        [TestMethod]
        [TestCategory("Configuration.Integration")]
        public void CanParseCaseInformation()
        {
            var parms = SettingsManager.GetNavigation();
            Assert.IsNotNull(parms);
            Assert.IsTrue(parms.Count > 0);
            var sttg = parms[0];
            var dataRw = new Models.HLinkDataRow
            {
                Address = "1234 Somewhare",
                Data = TestDataRow2(),
                Defendant = "Some Person"
            };

            var doc = new XmlDocument();
            doc.LoadXml(dataRw.Data);
            foreach (var item in sttg.CaseInstructions)
            {
                var node = doc.FirstChild!.SelectSingleNode(item.Value);
                node.ShouldNotBeNull();
                Console.WriteLine("Attribute: {0}, Value: {1}", item.Name, node.InnerText);
            }
        }

        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void CanSplitAddress()
        {
            const string lineBreak = @"<br/>";
            const string addressLine = @"c/o Corporation Service Company<br/>  211 E. 7th Street, Suite 620<br/>  Austin, TX 78701-3218";
            var addresses = addressLine.Split(new string[] { lineBreak }, StringSplitOptions.None).ToList();
            addresses.ShouldNotBeNull();
            addresses = addresses.Select(a => a.Trim()).ToList();
            addresses.ForEach(Console.WriteLine);
        }

        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void CanGetDentonSettings()
        {
            const int indexValue = 5;
            var sourceDto = new SearchSettingDto
            {
                CountyCourtId = indexValue,
                CountySearchTypeId = indexValue,
                DistrictCourtId = indexValue,
                DistrictSearchTypeId = indexValue
            };
            SearchSettingDto.Save(sourceDto);

            var settingDto = SearchSettingDto.GetDto();
            Assert.IsNotNull(settingDto);

            Assert.IsTrue(settingDto.CountySearchTypeId == indexValue);
            Assert.IsTrue(settingDto.CountyCourtId == indexValue);
            Assert.IsTrue(settingDto.DistrictCourtId == indexValue);
            Assert.IsTrue(settingDto.DistrictSearchTypeId == indexValue);
        }


        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void CanGetHarrisCivilSettings()
        {
            const string name = "harris-civil-settings";
            const int indexValue = 5;
            var sourceDto = new GenericSettingDto(name)
            {
                CountyCourtId = indexValue,
                CountySearchTypeId = indexValue,
                DistrictCourtId = indexValue,
                DistrictSearchTypeId = indexValue
            };
            sourceDto.Save(sourceDto);

            var settingDto = sourceDto.GetDto();
            Assert.IsNotNull(settingDto);

            Assert.AreEqual(name, sourceDto.Name);
            Assert.IsTrue(settingDto.CountySearchTypeId == indexValue);
            Assert.IsTrue(settingDto.CountyCourtId == indexValue);
            Assert.IsTrue(settingDto.DistrictCourtId == indexValue);
            Assert.IsTrue(settingDto.DistrictSearchTypeId == indexValue);
        }


        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void CanGetHarrisCivilGenericSetting()
        {
            const string name = "harris-civil-settings";
            const int indexValue = 5;
            var setting = new GenericSetting { Name = name };
            var sourceDto = setting.GetDto();
            Assert.AreEqual(name, setting.Name);
            sourceDto.CountyCourtId = indexValue;
            sourceDto.CountySearchTypeId = indexValue;
            sourceDto.DistrictCourtId = indexValue;
            sourceDto.DistrictSearchTypeId = indexValue;

            setting.Save(sourceDto);

            var settingDto = setting.GetDto();
            Assert.IsNotNull(settingDto);
            Assert.IsFalse(string.IsNullOrEmpty(setting.DataFile));
            Assert.IsFalse(string.IsNullOrEmpty(setting.Content));
            Assert.IsTrue(settingDto.CountySearchTypeId == indexValue);
            Assert.IsTrue(settingDto.CountyCourtId == indexValue);
            Assert.IsTrue(settingDto.DistrictCourtId == indexValue);
            Assert.IsTrue(settingDto.DistrictSearchTypeId == indexValue);
        }

        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void CanGetCollinSettings()
        {
            var userDto = UserAccessDto.GetDto("collinCountyUserMap");
            Assert.IsNotNull(userDto);
        }


        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void CanGetCollinCredential()
        {
            var userDto = UserAccessDto.GetDto("collinCountyUserMap");
            Assert.IsNotNull(userDto);
            var credential = UserAccessDto.GetCredential(userDto);
            Assert.IsNotNull(credential);
            Assert.IsTrue(credential.Any());
            credential.ForEach(c => Console.Write(" {0}", c));
        }

        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void SettingsXmlFallbackContentCanBeRead()
        {
            var content = SettingsManager.GetXmlContent("settings.xml");
            Assert.IsNotNull(content);
            Assert.IsTrue(content.Any());
        }
        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void SettingsXmlFallbackContentCanBeLoaded()
        {
            var content = SettingsManager.GetXmlContent("settings.xml");
            Assert.IsNotNull(content);
            var document = XmlDocProvider.GetDoc(content);
            Assert.IsNotNull(document);
        }
        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void CaseLayoutXmlFallbackContentCanBeRead()
        {
            var content = SettingsManager.GetXmlContent("caselayout.xml");
            Assert.IsNotNull(content);
            Assert.IsTrue(content.Any());
        }
        [TestMethod]
        [TestCategory("Configuration.Mapping")]
        public void CaseLayoutXmlFallbackContentCanBeLoaded()
        {
            var content = SettingsManager.GetXmlContent("caselayout.xml");
            Assert.IsNotNull(content);
            var document = XmlDocProvider.GetDoc(content);
            Assert.IsNotNull(document);
        }

        private static string TestDataRow()
        {
            return @"<tr><td nowrap='true' valign='top'><a href='CaseDetail.aspx?CaseID=2490347' style='color: blue'>F18-88-16</a></td><td nowrap='true' valign='top'></td><td nowrap='true' valign='top'><div>Ortiz-Rosado, Giovanni</div><div>07/15/1993</div></td><td valign='top' nowrap='true'><div>01/05/2018</div><div>16th Judicial District Court</div><div>Shipman, Sherry</div></td><td valign='top' nowrap='true'><div>Felony by Indictment</div><div>Inactive: Disposed</div></td><td nowrap='true' valign='top'><div style='overflow: hidden'>Lesser Included Possession of a Controlled Substance </div></td></tr>";
        }


        private static string TestDataRow2()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");
            sb.AppendLine("<td nowrap='true' valign='top'>");
            sb.AppendLine("<a href='CaseDetail.aspx?CaseID=2631008' style='color: blue'>BF-2019-00159</a>");
            sb.AppendLine("</td>");
            sb.AppendLine("<td valign='top'>");
            sb.AppendLine("State of Texas VS. Luis Albert Cordova");
            sb.AppendLine("</td>");
            sb.AppendLine("<td valign='top' nowrap='true'>");
            sb.AppendLine("<div>02/28/2019</div>");
            sb.AppendLine("<div>County Court At Law #2</div>");
            sb.AppendLine("<div>Ramirez, Robert C.</div>");
            sb.AppendLine("</td>");
            sb.AppendLine("<td valign='top' nowrap='true'>");
            sb.AppendLine("<div>Bond Forfeiture-Surety</div>");
            sb.AppendLine("<div>Active</div>");
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");

            return sb.ToString();
        }

        
    }
}
