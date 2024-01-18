using legallead.models;
using legallead.permissions.api.Model;
using legallead.records.search.Classes;
using Newtonsoft.Json;


namespace legallead.search.api.tests
{
    public class WebInterativeTests
    {
        private const string dentonjs = "{'state':'tx','county':{'name':'denton','value':26730},'details':[{'name':'Court Type','text':'All County Courts','value':'9'},{'name':'Case Type','text':'JP And County Courts','value':'0'}],'start':1696118400000,'end':1696636800000}";

        private const string tarrantjs = "{'state':'tx','county':{'name':'tarrant','value':28330},'details':[{'name':'Court Type','text':'All CCL Courts','value':'2'}],'start':1699833600000,'end':1700006400000}";
        private const string collinjs = "{'state':'tx','county':{'name':'collin','value':26550},'details':[{'name':'Search Type','text':'Civil And Family Case Records','value':'3'}],'start':1699833600000,'end':1700006400000}";


        [Fact]
        public void MapperCanMapToCollinWebInteractive()
        {
            var interactive = GetInteractive("collin");
            Assert.NotNull(interactive);
        }

        [Fact]
        public void MapperCanMapToDentonWebInteractive()
        {
            var interactive = GetInteractive();
            Assert.NotNull(interactive);
        }

        [Fact]
        public void MapperCanMapToTarrantWebInteractive()
        {
            var interactive = GetInteractive("tarrant");
            Assert.NotNull(interactive);
        }


        [Fact]
        public void InteractiveCanFetchCollin()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
                return;

            var interactive = GetInteractive("collin");
            var response = interactive?.Fetch();
            Assert.NotNull(response);
        }

        [Fact]
        public void InteractiveCanFetchDenton()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
                return;

            var interactive = GetInteractive();
            var response = interactive?.Fetch();
            Assert.NotNull(response);
        }

        [Fact]
        public void InteractiveCanFetchTarrant()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
                return;

            var interactive = GetInteractive("tarrant");
            var response = interactive?.Fetch();
            Assert.NotNull(response);
        }
        private static WebInteractive? GetInteractive(string target = "denton")
        {
            DateTime? dt1 = new DateTime(2023, 10, 3, 0, 0, 0, DateTimeKind.Utc);
            DateTime? dt2 = dt1.Value.AddDays(0);
            var source = GetRequest(target);
            source.StartDate = dt1.ToUnixTime();
            source.EndDate = dt2.ToUnixTime();
            return WebMapper.MapFrom<UserSearchRequest, WebInteractive>(source);
        }
        private static UserSearchRequest GetRequest(string target = "denton")
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            if (target.Equals("tarrant", oic))
                return JsonConvert.DeserializeObject<UserSearchRequest>(tarrantjs) ?? new();

            if (target.Equals("collin", oic))
                return JsonConvert.DeserializeObject<UserSearchRequest>(collinjs) ?? new();

            return JsonConvert.DeserializeObject<UserSearchRequest>(dentonjs) ?? new();
        }
    }
}
