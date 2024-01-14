using legallead.models.Search;
using legallead.permissions.api.Model;
using Newtonsoft.Json;

namespace legallead.search.api.tests
{
    public class UnitTest1
    {
        private const string dentonjs = "{'state':'tx','county':{'name':'denton','value':26730},'details':[{'name':'Court Type','text':'All County Courts','value':'9'},{'name':'Case Type','text':'JP And County Courts','value':'0'}],'start':1696118400000,'end':1696636800000}";

        [Fact]
        public void MapperCanMapToSearchRequest()
        {
            var source = GetRequest();
            var dest = WebMapper.MapFrom<UserSearchRequest, SearchRequest>(source);
            Assert.NotNull(dest);
            Assert.Equal(0, dest.WebId);
            Assert.Equal("denton", dest.County);
            Assert.Equal("tx", dest.State);
            Assert.Equal("2023-10-01", dest.StartDate);
            Assert.Equal("2023-10-07", dest.EndDate);
        }

        [Theory]
        [InlineData("Denton", 0)]
        [InlineData("denton", 0)]
        [InlineData("Collin", 20)]
        [InlineData("collin", 20)]
        [InlineData("Tarrant", 10)]
        [InlineData("tarrant", 10)]
        [InlineData("tarrent", 0)]
        public void MapperCanMapToWebIndex(string countyName, int webid)
        {
            var source = GetRequest();
            source.County.Name = countyName;
            var dest = WebMapper.MapFrom<UserSearchRequest, SearchRequest>(source);
            Assert.NotNull(dest);
            Assert.Equal(webid, dest.WebId);
        }
        private static UserSearchRequest GetRequest()
        {
            return JsonConvert.DeserializeObject<UserSearchRequest>(dentonjs) ?? new();
        }
    }
}