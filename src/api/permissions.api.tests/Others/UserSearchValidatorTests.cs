using legallead.models;
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
        [Theory]
        [InlineData(26550, "Collin", true)]
        [InlineData(26730, "Denton", true)]
        [InlineData(27130, "Harris", true)]
        [InlineData(28330, "Tarrant", true)]
        [InlineData(32190, "Harris-JP", true)]
        [InlineData(32180, "Weston", false)]
        [InlineData(32195, "Harris-JP", false)]
        [InlineData(26550, "NoMatch", false)]
        public void ModelCountyValidationTest(int index, string name, bool expected)
        {
            var sut = GetValidator();
            var request = TransformRequest(name, GetRequest());
            request.County.Value = index;
            request.County.Name = name;
            var actual = sut.IsValid(request);
            Assert.Equal(expected, actual.Key);
        }

        private static UserSearchValidator GetValidator()
        {
            return new UserSearchValidator
            {
                MaxDays = 7,
                MinStartDate = 1514764800095
            };
        }
        private static UserSearchRequest GetRequest()
        {
            return JsonConvert.DeserializeObject<UserSearchRequest>(requestjs) ?? new();
        }
        private static UserSearchRequest TransformRequest(string countyName, UserSearchRequest request)
        {
            var oic = StringComparison.OrdinalIgnoreCase;
            if (countyName.Equals("collin", oic))
            {
                request.Details.Clear();
                request.Details.Add(new() { Name = "Search Type", Value = "0", Text = "criminal case records" });
                return request;
            }
            if (countyName.Equals("denton", oic))
            {
                return request;
            }
            if (countyName.Equals("harris", oic))
            {
                request.Details.Clear();
                request.Details.Add(new() { Name = "Search Type", Value = "0", Text = "All Civil Courts" });
                return request;
            }
            if (countyName.Equals("harris-jp", oic))
            {
                request.Details.Clear();
                request.Details.Add(new() { Name = "Search Type", Value = "0", Text = "All JP Courts" });
                return request;
            }
            if (countyName.Equals("tarrant", oic))
            {
                request.Details.Clear();
                request.Details.Add(new() { Name = "Search Type", Value = "0", Text = "All Probate Courts" });
                return request;
            }
            return request;
        }

        private static long GetUnixTime(DateTime? startDate = null, DateTimeKind dateKind = DateTimeKind.Utc)
        {
            return startDate.ToUnixTime(dateKind);
        }
    }
}
