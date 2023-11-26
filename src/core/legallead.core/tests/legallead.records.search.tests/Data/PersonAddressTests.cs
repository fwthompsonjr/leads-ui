using AutoMapper;
using CsvHelper;
using legallead.records.search.Classes;
using legallead.records.search.Dto;
using legallead.records.search.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Xml;

namespace legallead.records.search.Tests.Data
{
    [TestClass]
    public class PersonAddressTests
    {
        [TestMethod]
        [TestCategory("Person.Data.Mapping")]
        public void CanSerializeXmlString()
        {
            var personData = "<person>" +
            "<name>Holland, Stephen</name>" +
            "<address><![CDATA[  387 West FRK<br/>  APT #2838<br/>  Irving, TX 75039]]>" +
            "<addressA><![CDATA[387 WEST FRK]]></addressA>" +
            "<addressB><![CDATA[APT #2838]]></addressB>" +
            "<addressC><![CDATA[IRVING, TX 75039]]></addressC>" +
            "<zip><![CDATA[75039]]></zip>" +
            "</address>" +
            "<case><![CDATA[CV-2019-01210-OL]]></case>" +
            "<dateFiled><![CDATA[04/12/2019]]></dateFiled>" +
            "<court><![CDATA[County Court At Law #2]]></court>" +
            "<caseType><![CDATA[Occupational  License]]></caseType>" +
            "</person>";

            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);
            XmlNode rootNode = doc.CreateElement("personList");
            doc.AppendChild(rootNode);

            //Create a document fragment and load the xml into it
            XmlDocumentFragment fragment = doc.CreateDocumentFragment();
            fragment.InnerXml = personData;
            rootNode.AppendChild(fragment);
            // what do i do with these line-breaks?
            // how is that going to translate into Excel?
            var dto = PersonAddress.ConvertFrom(rootNode.FirstChild!);

            Assert.IsNotNull(dto);
        }

        [TestMethod]
        [TestCategory("Person.Data.Mapping")]
        public void CanReadFileAndGetCases()
        {
            const string testFile = @"C:/Code/SandBox/RecordSearch/legallead.records.search.Tests/bin/Debug/xml/data/data_rqst_dentoncounty_04152019_04172019.xml";
            var settings = SettingsManager.GetNavigation();
            settings.ShouldNotBeNull();
            var sttg = settings[0];
            sttg.ShouldNotBeNull();
            var startDate = DateTime.Now.Date.AddDays(-2);
            var endingDate = DateTime.Now.Date.AddDays(0);
            var webactive = new WebInteractive(sttg, startDate, endingDate);
            webactive.ReadFromFile(testFile);
        }

        [TestMethod]
        public void CanMapPersonNonCriminalCaseInfo()
        {
            var caseInstructions = SearchSettingDto.GetNonCriminalMapping();
            var expectedList = ExpectedNonCriminalValues();

            var doc = new XmlDocument();
            doc.LoadXml(NonCriminalRow());
            var indx = 0;
            foreach (var item in caseInstructions.NavInstructions)
            {
                var node = TryFindNode(doc, item.Value);
                Assert.IsNotNull(node, $"{item.FriendlyName} is null");
                Assert.IsFalse(string.IsNullOrEmpty(node.InnerText), $"{item.FriendlyName} is blank or empty");

                Assert.AreEqual(expectedList[indx++].CommandType, node.InnerText,
                    $"{item.FriendlyName} not matched. ");
            }
        }

        [TestMethod]
        public void CanMapPersonCriminalCaseInfo()
        {
            var caseInstructions = SearchSettingDto.GetCriminalMapping();
            var expectedList = ExpectedCriminalValues();

            var doc = new XmlDocument();
            doc.LoadXml(CriminalRow());
            var indx = 0;
            foreach (var item in caseInstructions.NavInstructions)
            {
                var node = TryFindNode(doc, item.Value);
                Assert.IsNotNull(node, $"{item.FriendlyName} is null");
                Assert.IsFalse(string.IsNullOrEmpty(node.InnerText), $"{item.FriendlyName} is blank or empty");
                if (item.Name.Equals("CaseStyle"))
                {
                    continue;
                }

                Assert.AreEqual(expectedList[indx++].CommandType, node.InnerText,
                    $"{item.FriendlyName} not matched. ");
            }
        }

        [TestMethod]
        public void CanGetCourtLookUpList()
        {
            var list = SearchSettingDto.GetCourtLookupList;
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void CanGetSampleDtoList()
        {
            var list = SamplePersonAddress();
            Assert.IsNotNull(list);
            Assert.AreEqual(6, list.Count);
            var noPlantiffList = list.FindAll(x => string.IsNullOrEmpty(x.Plantiff));
            Assert.AreEqual(0, noPlantiffList.Count);
        }

        [TestMethod]
        public void CanFindASampleRecordWithoutPlantiff()
        {
            var list = SamplePersonAddress();
            Assert.IsNotNull(list);
            var noPlantiffList = list.FindAll(x => string.IsNullOrEmpty(x.Plantiff));
            if (!noPlantiffList.Any())
            {
                return;
            }

            var failing = noPlantiffList[0];
            _ = failing.Plantiff;
        }

        private static XmlNode? TryFindNode(XmlDocument doc, string xpath)
        {
            try
            {
                var node = doc.FirstChild!.SelectSingleNode(xpath);
                return node;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string CriminalRow()
        {
            return " <tr> " + Environment.NewLine +
            " <td index='1' nowrap='true' valign='top'> " + Environment.NewLine +
            " <a href='CaseDetail.aspx?CaseID=2702640' style='color: blue'>19-1647J4</a> " + Environment.NewLine +
            " </td> " + Environment.NewLine +
            " <td index='2' nowrap='true' valign='top'> " + Environment.NewLine +
            " <div style='overflow:hidden'>304097</div> " + Environment.NewLine +
            " </td> " + Environment.NewLine +
            " <td index='3' nowrap='true' valign='top'> " + Environment.NewLine +
            " <div>Morton, Mason R</div> " + Environment.NewLine +
            " <div>08/29/2001</div> " + Environment.NewLine +
            " </td> " + Environment.NewLine +
            " <td index='4' valign='top' nowrap='true'> " + Environment.NewLine +
            " <div>12/03/2019</div> " + Environment.NewLine +
            " <div>Justice of the Peace Pct #4</div> " + Environment.NewLine +
            " <div>Hughey, Harris</div> " + Environment.NewLine +
            " </td> " + Environment.NewLine +
            " <td index='5' valign='top' nowrap='true'> " + Environment.NewLine +
            " <div>Adult Traffic Citation</div> " + Environment.NewLine +
            " <div>Filed</div> " + Environment.NewLine +
            " </td> " + Environment.NewLine +
            " <td index='6' nowrap='true' valign='top'> " + Environment.NewLine +
            " <table border='0' cellpadding='0' cellspacing='0'  " + Environment.NewLine +
            "  width='100%' style='table-layout: fixed; font-size: 8pt; font-family: arial'> " + Environment.NewLine +
            " <tbody> " + Environment.NewLine +
            " <tr> " + Environment.NewLine +
            " <td style='vertical-align:top;'>SPEEDING &gt;10% ABOVE POSTED LIMIT </td> " + Environment.NewLine +
            " </tr> " + Environment.NewLine +
            " </tbody> " + Environment.NewLine +
            " </table> " + Environment.NewLine +
            " </td> " + Environment.NewLine +
            " </tr>";
        }

        private static string NonCriminalRow()
        {
            return "<tr> " + Environment.NewLine +
            "<td index='1' nowrap='true' valign='top'>" + Environment.NewLine +
            "<a href='CaseDetail.aspx?CaseID=2701630' " + Environment.NewLine +
            "style='color: blue'>BF-2019-01279</a>" + Environment.NewLine +
            "</td>" + Environment.NewLine +
            "<td index='2' valign='top'>State of Texas VS. Jennifer Dansheal Lee</td>" + Environment.NewLine +
            "<td index='3' valign='top' nowrap='true'>" + Environment.NewLine +
            "<div>11/26/2019</div><div>County Court At Law #2</div>" + Environment.NewLine +
            "<div>Ramirez, Robert C.</div></td><td valign='top' nowrap='true'>" + Environment.NewLine +
            "<div>Bond Forfeiture-Surety</div><div>Active</div>" + Environment.NewLine +
            "</td>" + Environment.NewLine +
            "</tr>";
        }

        private static IList<WebNavInstruction> ExpectedCriminalValues()
        {
            var caseInstructions = SearchSettingDto.GetCriminalMapping();
            foreach (var item in caseInstructions.NavInstructions)
            {
                switch (item.Name)
                {
                    case "DateFiled":
                        item.CommandType = "12/03/2019";
                        break;

                    case "Case":
                        item.CommandType = "19-1647J4";
                        break;

                    case "Court":
                        item.CommandType = "Justice of the Peace Pct #4";
                        break;

                    case "CaseType":
                        item.CommandType = "Adult Traffic Citation";
                        break;

                    case "CaseStyle":
                        item.CommandType = "SPEEDING &gt;10% ABOVE POSTED LIMIT";
                        break;

                    default:
                        break;
                }
            }
            return caseInstructions.NavInstructions;
        }

        private static IList<WebNavInstruction> ExpectedNonCriminalValues()
        {
            var caseInstructions = SearchSettingDto.GetCriminalMapping();
            foreach (var item in caseInstructions.NavInstructions)
            {
                switch (item.Name)
                {
                    case "DateFiled":
                        item.CommandType = "11/26/2019";
                        break;

                    case "Case":
                        item.CommandType = "BF-2019-01279";
                        break;

                    case "Court":
                        item.CommandType = "County Court At Law #2";
                        break;

                    case "CaseType":
                        item.CommandType = "Bond Forfeiture-Surety";
                        break;

                    case "CaseStyle":
                        item.CommandType = "State of Texas VS. Jennifer Dansheal Lee";
                        break;

                    default:
                        break;
                }
            }
            return caseInstructions.NavInstructions;
        }

        private static List<PersonAddress> SamplePersonAddress()
        {
            var content = _sampleAddressText.Replace("~", '"'.ToString());
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            using MemoryStream stream = new(bytes);
            using TextReader reader = new StreamReader(stream);
            using var csv = new CsvReader(reader);
            var dto = csv.GetRecords<PersonAddressDto>().ToList();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<PersonAddressDto, PersonAddress>());
            var mapper = config.CreateMapper();
            var people = new List<PersonAddress>();
            dto.ForEach(x => people.Add(mapper.Map<PersonAddress>(x)));
            return people;
        }

        private static readonly string _sampleAddressText = ("Name,FirstName,LastName,Zip,Address1,Address2,Address3,CaseNumber,DateFiled,Court,CaseType,CaseStyle,Plantiff,County,Cou" +
            "rtAddress" + Environment.NewLine +
            "~McCraw, Susan~,Susan,McCraw,00000,No Match Found,,Not Matched 00000,GA1-0260-2019,12/17/2019,Probate Courts,Probate - G" +
            "uardianship for an Adult,In the Guardianship Of Jean Neal,Jean Neal,Collin,~2100 Bloomdale Road, McKinney, TX 75071~" + Environment.NewLine +
            "~Hanna, Frank A.~,Frank,Hanna,94506,31 Lilly CT,,~Danville, CA 94506~,PB1-2039-2019,12/17/2019,Probate Courts,Probate -" +
            "Small Estate Proceedings,In the Estate of Doris Hanna,,Collin,~2100 Bloomdale Road, McKinney, TX 75071~" + Environment.NewLine +
            "~NORDYKE, CANDICE ANN~,CANDICE,NORDYKE,75033,8505 TANGLEROSE DR.,,~FRISCO, TX 75033~,PB1-2040-2019,12/17/2019,Probate Co" +
            "urts,Probate - Independent Administration,In the Estate Of DAVID WAYNE WHITTEN,,Collin,~2100 Bloomdale Road, McKinney, T" +
            "X 75071~" + Environment.NewLine +
            "~AVERY, LUIS L.~,LUIS,AVERY,01740,397 Berlin Road,,~Bolton, MA 01740~,PB1-2041-2019,12/17/2019,Probate Courts,Probate -" +
            "Independent Administration,In the Estate Of DONALD G. AVERY,,Collin,~2100 Bloomdale Road, McKinney, TX 75071~" + Environment.NewLine +
            "~O'Neal, Sallianne~,Sallianne,O'Neal,76258,9110 Highway 377,,~Pilot Point, TX 76258~,PB1-2042-2019,12/17/2019,Probate Co" +
            "urts,Probate - Independent Administration,In the Estate Of Patricia Ann O'Neal,,Collin,~2100 Bloomdale Road, McKinney, T" +
            "X 75071~" + Environment.NewLine +
            "~Nicks, David~,David,Nicks,75001,~c/o Dena L. Mathis, Mathis Legal PLLC~,~15851 Dallas Parkway, Suite 800~,~Dallas, TX 7" +
            "5001~,PB1-2043-2019,12/17/2019,Probate Courts,Probate - Independent Administration,In the Estate Of Lori Ann Nicks,,Coll" +
            "in,~2100 Bloomdale Road, McKinney, TX 75071~" + Environment.NewLine);
    }
}