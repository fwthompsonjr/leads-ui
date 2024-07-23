using legallead.permissions.api.Model;
using legallead.records.search.Classes;
using legallead.records.search.tests.Mapper;
using legallead.records.search.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace legallead.records.search.tests
{
    [TestClass]
    public class CollinInteractiveTest
    {

        private const string collinjs = "{'state':'tx','county':{'name':'collin','value':26550},'details':[{'name':'Search Type','text':'Civil And Family Case Records','value':'3'}],'start':1719187200000,'end':1719273600000}";


        [TestMethod]
        public void InteractiveCanFetchCollin()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
                return;

            var interactive = GetInteractive();
            var response = interactive?.Fetch();
            Assert.IsNotNull(response);
        }


        private static WebInteractive? GetInteractive()
        {
            var weekends = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };
            DateTime? dt1 = DateTime.Now.AddDays(-7);
            while(weekends.Contains(dt1.Value.DayOfWeek)) { dt1 = dt1.Value.AddDays(-1); }
            DateTime? dt2 = dt1.Value.AddDays(0);
            var source = GetRequest();
            source.StartDate = ToUnixTime(dt1);
            source.EndDate = ToUnixTime(dt2);
            var mapped = TheMapper.MapFrom<UserSearchRequest, WebInteractive>(source);
            if (mapped == null) return null;
            if (string.IsNullOrEmpty(mapped.UniqueId))
            {
                mapped.UniqueId = Guid.NewGuid().ToString("D");
            }
            return mapped;
        }
        private static UserSearchRequest GetRequest()
        {
            return JsonConvert.DeserializeObject<UserSearchRequest>(collinjs) ?? new();
        }

        private static long ToUnixTime(DateTime? startDate, DateTimeKind dateKind = DateTimeKind.Utc)
        {
            const string dateFormat = "U"; // universal date time
            var cultureEnglishUS = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            if (startDate.HasValue && dateKind == DateTimeKind.Local)
            {
                startDate = startDate.Value.ToUniversalTime();
            }
            if (!startDate.HasValue)
            {
                startDate = DateTime.UtcNow;
            }
            var utcNow = startDate.Value;
            var dateStart = utcNow.ToString(dateFormat, cultureEnglishUS);
            var unixEpoch = DateTime.UnixEpoch;
            DateTime start = DateTime.ParseExact(dateStart, dateFormat, cultureEnglishUS);
            var milliseconds = DateTime.Now.Millisecond.ToString("000");
            var unixStart = ((long)start
                .Subtract(unixEpoch)
                .TotalMilliseconds).ToString();
            unixStart = unixStart[..^3];
            unixStart += milliseconds;
            return Convert.ToInt64(unixStart);
        }
    }
}