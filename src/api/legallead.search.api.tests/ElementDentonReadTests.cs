using legallead.records.search.Classes;
using legallead.records.search.Models;
using legallead.records.search.Web;
using legallead.search.api.tests.Utility;

namespace legallead.search.api.tests
{
    public class ElementDentonReadTests
    {
        [Fact]
        public void ReaderCanParseDentonNormal()
        {
            var content = Properties.Resources.tx_denton_normal_search_result;
            var exception = Record.Exception(() =>
            {
                TestContent(content);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ReaderCanParseDentonCriminal()
        {
            var content = Properties.Resources.tx_denton_criminal_search_result;
            var exception = Record.Exception(() =>
            {
                TestContent(content, true);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ReaderCanParseDentonDistrict()
        {
            var content = Properties.Resources.tx_denton_district_search_result;
            var exception = Record.Exception(() =>
            {
                TestContent(content);
            });
            Assert.Null(exception);
        }


        [Fact]
        public void ReaderListIsEmptyByDefault()
        {
            var sut = new DentonTableRead();
            var list = sut.ToCaseList();
            Assert.NotNull(list);
            Assert.Empty(list);
        }

        [Fact]
        public void ReaderListIsEmptyWithNoData()
        {
            var sut = new DentonTableRead { RecordSet = Array.Empty<DentonTableReadRecord>() };
            var list = sut.ToCaseList();
            Assert.NotNull(list);
            Assert.Empty(list);
        }
        private static void TestContent(string content, bool isCrime = false)
        {
            const string tblx = "/html/body/table[4]";
            var reader = new ElementDentonReadList
            {
                TableXPath = tblx
            };
            var item = new WebNavInstruction();
            using var web = new LocalContentDriver(content);
            Assert.NotNull(web.Driver);
            reader.Assertion = new ElementAssertion(web.Driver);
            var response = reader.Execute(item);
            var jsContent = reader.JsContent;
            var rows = jsContent?.ToCaseList(isCrime);
            Assert.NotNull(jsContent);
            Assert.NotNull(jsContent.Header);
            Assert.NotNull(jsContent.RecordSet);
            Assert.NotNull(rows);
            Assert.NotEmpty(rows);
            Assert.NotNull(response);
        }
    }
}
