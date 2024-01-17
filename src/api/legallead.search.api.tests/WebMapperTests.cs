using legallead.models.Search;
using legallead.permissions.api.Model;
using Newtonsoft.Json;

namespace legallead.search.api.tests
{
    public class WebMapperTests
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


        [Fact]
        public void MapperCanMapToDentonNavigationParameter()
        {
            var expectedStart = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var expectedEnd = new DateTime(2023, 10, 7, 0, 0, 0, DateTimeKind.Utc);
            var source = GetRequest();
            var dest = WebMapper.MapFrom<UserSearchRequest, SearchNavigationParameter>(source);
            Assert.NotNull(dest);
            Assert.Equal(0, dest.Id);
            Assert.Equal(expectedStart, dest.StartDate);
            Assert.Equal(expectedEnd, dest.EndDate);
            Assert.True(dest.Keys.Count >= 10);
            Assert.True(dest.Instructions.Count >= 15);
            Assert.True(dest.CaseInstructions.Count >= 5);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        public void MapperCanMapToDentonNavigationByCaseType(string caseIndex)
        {
            var expectedStart = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var expectedEnd = new DateTime(2023, 10, 7, 0, 0, 0, DateTimeKind.Utc);
            var source = GetRequest();
            var target = source.Details.Find(x => x.Name == "Case Type");
            if (target != null)
            {
                target.Value = caseIndex;
            }
            var dest = WebMapper.MapFrom<UserSearchRequest, SearchNavigationParameter>(source);
            Assert.NotNull(dest);
            Assert.Equal(0, dest.Id);
            Assert.Equal(expectedStart, dest.StartDate);
            Assert.Equal(expectedEnd, dest.EndDate);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        [InlineData("6")]
        [InlineData("7")]
        [InlineData("8")]
        [InlineData("9")]
        [InlineData("10")]
        [InlineData("11")]
        [InlineData("12")]
        [InlineData("13")]
        [InlineData("14")]
        [InlineData("15")]
        [InlineData("16")]
        public void MapperCanMapToDentonNavigationByCourtType(string caseIndex)
        {
            var expectedStart = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var expectedEnd = new DateTime(2023, 10, 7, 0, 0, 0, DateTimeKind.Utc);
            var source = GetRequest();
            var target = source.Details.Find(x => x.Name == "Court Type");
            if (target != null)
            {
                target.Value = caseIndex;
            }
            var dest = WebMapper.MapFrom<UserSearchRequest, SearchNavigationParameter>(source);
            Assert.NotNull(dest);
            Assert.Equal(0, dest.Id);
            Assert.Equal(expectedStart, dest.StartDate);
            Assert.Equal(expectedEnd, dest.EndDate);
        }

        [Fact]
        public void MapperCanMapToDentonNavigationWithoutCourtType()
        {
            var expectedStart = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var expectedEnd = new DateTime(2023, 10, 7, 0, 0, 0, DateTimeKind.Utc);
            var source = GetRequest();
            var target = source.Details.Find(x => x.Name == "Court Type");
            if (target != null)
            {
                source.Details.Remove(target);
            }
            var dest = WebMapper.MapFrom<UserSearchRequest, SearchNavigationParameter>(source);
            Assert.NotNull(dest);
            Assert.Equal(0, dest.Id);
            Assert.Equal(expectedStart, dest.StartDate);
            Assert.Equal(expectedEnd, dest.EndDate);
        }

        private static UserSearchRequest GetRequest()
        {
            return JsonConvert.DeserializeObject<UserSearchRequest>(dentonjs) ?? new();
        }
    }
}