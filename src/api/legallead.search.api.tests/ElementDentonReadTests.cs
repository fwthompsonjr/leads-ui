using legallead.records.search.Classes;
using legallead.records.search.Models;
using legallead.records.search.Web;
using legallead.search.api.tests.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                TestContent(content);
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

        private static void TestContent(string content)
        {
            var reader = new ElementDentonReadList();
            var item = new WebNavInstruction(); using var web = new LocalContentDriver(content);
            Assert.NotNull(web.Driver);
            reader.Assertion = new ElementAssertion(web.Driver);
            reader.Execute(item);
            Assert.NotNull(reader.JsContent);
            Assert.NotNull(reader.JsContent.Header);
            Assert.NotNull(reader.JsContent.RecordSet);
        }
    }
}
