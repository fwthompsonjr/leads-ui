using legallead.records.search.Addressing;
using legallead.search.api.tests.Utility;

namespace legallead.search.api.tests
{
    public class DentonCountyAddressTests
    {
        [Fact]
        public void SutCanFetchAddresses()
        {
            var content = Properties.Resources.tx_denton_person_principal;
            using var web = new LocalContentDriver(content);
            Assert.NotNull(web.Driver);
            var mapper = new DentonCountyAddressMatch(web.Driver);
            var addresses = mapper.Addresses;
            Assert.NotEmpty(addresses);
        }
    }
}
