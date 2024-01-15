using legallead.models;
using legallead.permissions.api.Model;
using legallead.records.search.Classes;
using Newtonsoft.Json;


namespace legallead.search.api.tests
{
    public class WebInterativeTests
    {
        private const string dentonjs = "{'state':'tx','county':{'name':'denton','value':26730},'details':[{'name':'Court Type','text':'All County Courts','value':'9'},{'name':'Case Type','text':'JP And County Courts','value':'0'}],'start':1696118400000,'end':1696636800000}";

        [Fact]
        public void MapperCanMapToWebInteractive()
        {
            var interactive = GetInteractive();
            Assert.NotNull(interactive);
        }
        [Fact]
        public void InteractiveCanFetch()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
                return;

            var interactive = GetInteractive();
            var response = interactive?.Fetch();
            Assert.NotNull(response);
        }
        private static WebInteractive? GetInteractive()
        {
            DateTime? dt1 = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var source = GetRequest();
            source.StartDate = dt1.ToUnixTime();
            source.EndDate = dt1.ToUnixTime();
            return WebMapper.MapFrom<UserSearchRequest, WebInteractive>(source);
        }
        private static UserSearchRequest GetRequest()
        {
            return JsonConvert.DeserializeObject<UserSearchRequest>(dentonjs) ?? new();
        }
    }
}
