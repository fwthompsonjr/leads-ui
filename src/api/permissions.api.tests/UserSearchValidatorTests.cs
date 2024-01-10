using legallead.permissions.api.Model;
using legallead.permissions.api.Utility;
using Newtonsoft.Json;

namespace permissions.api.tests
{
    public class UserSearchValidatorTests
    {
        private const string requestjs = "{'state':'tx','county':{'name':'denton','value':26730},'details':[{'name':'Court Type','text':'All County Courts','value':'9'},{'name':'Case Type','text':'JP And County Courts','value':'0'}],'start':1696118400000,'end':1696636800000}";

        [Fact]
        public void ModelCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserSearchValidator();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = GetValidator();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanValidate()
        {
            var sut = GetValidator();
            var request = GetRequest();
            var actual = sut.IsValid(request);
            Assert.True(actual.Key);
        }

        [Fact]
        public void ModelWithOutStateIsInvalid()
        {
            var sut = GetValidator();
            var request = GetRequest();
            request.State = string.Empty;
            var actual = sut.IsValid(request);
            Assert.False(actual.Key);
            Assert.Contains("State", actual.Value);
        }

        [Fact]
        public void ModelWithBadStateIsInvalid()
        {
            var sut = GetValidator();
            var request = GetRequest();
            request.State = "Zzzz";
            var actual = sut.IsValid(request);
            Assert.False(actual.Key);
            Assert.Contains("State", actual.Value);
        }

        [Fact]
        public void ModelWithOutCountyIsInvalid()
        {
            var sut = GetValidator();
            var request = GetRequest();
            request.County.Name = string.Empty;
            var actual = sut.IsValid(request);
            Assert.False(actual.Key);
            Assert.Contains("County", actual.Value);
        }

        [Fact]
        public void ModelWithOutCountyIndexIsInvalid()
        {
            var sut = GetValidator();
            var request = GetRequest();
            request.County.Value = 0;
            var actual = sut.IsValid(request);
            Assert.False(actual.Key);
            Assert.Contains("County", actual.Value);
        }

        [Fact]
        public void ModelWithMismatchedCountyIndexIsInvalid()
        {
            var sut = GetValidator();
            var request = GetRequest();
            request.County.Value = 2000;
            var actual = sut.IsValid(request);
            Assert.False(actual.Key);
            Assert.Contains("County", actual.Value);
        }

        [Fact]
        public void ModelWithInactiveCountyIndexIsInvalid()
        {
            var sut = GetValidator();
            var request = GetRequest();
            request.County.Value = 10;
            request.County.Name = "Aleutians East";
            var actual = sut.IsValid(request);
            Assert.False(actual.Key);
            Assert.Contains("County", actual.Value);
        }

        [Fact]
        public void ModelWithStartDateLessThanMinIsInvalid()
        {
            var sut = GetValidator();
            var request = GetRequest();
            var unixTime = GetUnixTime(DateTime.UtcNow.AddDays(2));
            sut.MinStartDate = unixTime;
            var actual = sut.IsValid(request);
            Assert.False(actual.Key);
            Assert.Contains("Start Date", actual.Value);
        }

        [Fact]
        public void ModelWithBadDateRangeIsInvalid()
        {
            var sut = GetValidator();
            var request = GetRequest();
            sut.MaxDays = 1;
            var actual = sut.IsValid(request);
            Assert.False(actual.Key);
            Assert.Contains("Date range", actual.Value);
        }

        private static UserSearchValidator GetValidator()
        {
            return new UserSearchValidator
            {
                MaxDays = 7,
                MinStartDate = 1514764800095,
                Api = "search.leads.test"
            };
        }
        private static UserSearchRequest GetRequest()
        {
            return JsonConvert.DeserializeObject<UserSearchRequest>(requestjs) ?? new();
        }


        private static long GetUnixTime(DateTime? startDate = null, DateTimeKind dateKind = DateTimeKind.Utc)
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
            unixStart = unixStart.Substring(0, unixStart.Length - 3);
            unixStart += milliseconds;
            return Convert.ToInt64(unixStart);
        }
    }
}
