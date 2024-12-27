using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Models;
using legallead.permissions.api.Services;

namespace permissions.api.tests.Services
{
    public class UserUsageServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var sut = new UsageServiceMock();
            Assert.NotNull(sut.Service);
            Assert.NotNull(sut.Repository);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ServicCanAppendUsageRecordAsync(int index)
        {
            const int requestId = 0;
            var sut = new UsageServiceMock();
            var service = sut.Service;
            var mock = sut.Repository;
            var request = UsageServiceMock.GetRequestJson(requestId).ToInstance<AppendUsageRecordRequest>() ?? new();
            var resp = UsageServiceMock.GetResponseJson(requestId);
            var response = index switch
            {
                0 => new(),
                1 => resp.ToInstance<DbCountyAppendLimitBo>(),
                _ => null
            };
            mock.Setup(s => s.AppendUsageRecord(It.IsAny<UserUsageAppendRecordModel>())).ReturnsAsync(response);
            await service.AppendUsageRecordAsync(request);
            mock.Verify(s => s.AppendUsageRecord(It.IsAny<UserUsageAppendRecordModel>()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task ServicCanCompleteUsageRecordAsync(int index)
        {
            const int requestId = 1;
            var sut = new UsageServiceMock();
            var service = sut.Service;
            var mock = sut.Repository;
            var request = UsageServiceMock.GetRequestJson(requestId).ToInstance<CompleteUsageRecordRequest>() ?? new();
            var resp = UsageServiceMock.GetResponseJson(requestId, index);
            var response = resp.ToInstance<KeyValuePair<bool, string>>();
            mock.Setup(s => s.CompleteUsageRecord(It.IsAny<UserUsageCompleteRecordModel>())).ReturnsAsync(response);
            await service.CompleteUsageRecordAsync(request);
            mock.Verify(s => s.CompleteUsageRecord(It.IsAny<UserUsageCompleteRecordModel>()));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task ServicCanGetMonthlyLimitAsync(int index)
        {
            const int requestId = 2;
            var sut = new UsageServiceMock();
            var service = sut.Service;
            var mock = sut.Repository;
            var request = UsageServiceMock.GetRequestJson(requestId).ToInstance<GetMonthlyLimitRequest>() ?? new();
            var resp = UsageServiceMock.GetResponseJson(requestId, index);
            var response = resp.ToInstance<List<DbCountyUsageLimitBo>>();
            mock.Setup(s => s.GetMonthlyLimit(It.IsAny<string>())).ReturnsAsync(response);
            await service.GetMonthlyLimitAsync(request);
            mock.Verify(s => s.GetMonthlyLimit(It.IsAny<string>()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task ServicCanGetMonthlyLimitWithCountyAsync(int index)
        {
            const int requestId = 3;
            var sut = new UsageServiceMock();
            var service = sut.Service;
            var mock = sut.Repository;
            var request = UsageServiceMock.GetRequestJson(requestId).ToInstance<GetMonthlyLimitRequest>() ?? new();
            var resp = UsageServiceMock.GetResponseJson(requestId, index);
            var response = resp.ToInstance<List<DbCountyUsageLimitBo>>();
            var single = response != null && response.Count > 0 ? response[0] : new();
            mock.Setup(s => s.GetMonthlyLimit(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(single);
            await service.GetMonthlyLimitAsync(request);
            mock.Verify(s => s.GetMonthlyLimit(It.IsAny<string>(), It.IsAny<int>()));
        }



        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task ServicCanGetUsageAsync(int index)
        {
            const int requestId = 4;
            var sut = new UsageServiceMock();
            var service = sut.Service;
            var mock = sut.Repository;
            var request = UsageServiceMock.GetRequestJson(requestId).ToInstance<GetUsageRequest>() ?? new();
            var resp = UsageServiceMock.GetResponseJson(requestId, index);
            var response = resp.ToInstance<List<DbCountyUsageRequestBo>>();
            mock.Setup(s => s.GetUsage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<bool>())).ReturnsAsync(response);
            await service.GetUsageAsync(request);
            mock.Verify(s => s.GetUsage(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<bool>()));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task ServicCanGetUsageSummaryAsync(int index)
        {
            const int requestId = 5;
            var sut = new UsageServiceMock();
            var service = sut.Service;
            var mock = sut.Repository;
            var request = UsageServiceMock.GetRequestJson(requestId).ToInstance<GetUsageRequest>() ?? new();
            var resp = UsageServiceMock.GetResponseJson(requestId, index);
            var response = resp.ToInstance<List<DbUsageSummaryBo>>();
            mock.Setup(s => s.GetUsageSummary(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<bool>())).ReturnsAsync(response);
            await service.GetUsageSummaryAsync(request);
            mock.Verify(s => s.GetUsageSummary(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<bool>()));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task ServicCanSetMonthlyLimitAsync(int index)
        {
            const int requestId = 6;
            var sut = new UsageServiceMock();
            var service = sut.Service;
            var mock = sut.Repository;
            var request = UsageServiceMock.GetRequestJson(requestId).ToInstance<SetMonthlyLimitRequest>() ?? new();
            var resp = UsageServiceMock.GetResponseJson(requestId, index);
            var response = resp.ToInstance<DbCountyUsageLimitBo>();
            mock.Setup(s => s.SetMonthlyLimit(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(response);
            await service.SetMonthlyLimitAsync(request);
            mock.Verify(s => s.SetMonthlyLimit(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()));
        }
        private sealed class UsageServiceMock
        {
            private readonly UserUsageService _service;
            private readonly Mock<IUserUsageRepository> _repository;

            public UsageServiceMock()
            {
                _repository = new Mock<IUserUsageRepository>();
                _service = new(_repository.Object);
            }

            public UserUsageService Service => _service;
            public Mock<IUserUsageRepository> Repository => _repository;

            public static string GetRequestJson(int requestId)
            {
                if (requestId == 0) return appendFaker.Generate().ToJsonString();
                if (requestId == 1) return completeFaker.Generate().ToJsonString();
                if (requestId == 2 || requestId == 3)
                {
                    var obj = getlimitFaker.Generate();
                    obj.GetAllCounties = requestId == 2;
                    return obj.ToJsonString();
                }
                if (requestId == 4 || requestId == 5)
                {
                    var obj = getusageFaker.Generate();
                    obj.GetAllCounties = requestId == 4;
                    return obj.ToJsonString();
                }
                if (requestId == 6) return setlimitFaker.Generate().ToJsonString();
                return string.Empty;
            }

            public static string GetResponseJson(int requestId, int customId = 0)
            {
                var fkr = new Faker();
                if (requestId == 0) return appendBo.Generate().ToJsonString();
                if (requestId == 1)
                {
                    var isgood = customId % 2 == 0;
                    var message = isgood ? string.Empty : fkr.Lorem.Sentence();
                    var tmp = new KeyValuePair<bool, string>(isgood, message);
                    return tmp.ToJsonString();
                }
                if (requestId == 2)
                {
                    var tmp2 = limitBo.Generate(5);
                    return tmp2.ToJsonString();
                }
                return string.Empty;
            }
            private static readonly Faker<AppendUsageRecordRequest> appendFaker =
                new Faker<AppendUsageRecordRequest>()
                .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(16))
                .RuleFor(x => x.CountyId, y => y.Random.Int(1, 50))
                .RuleFor(x => x.CountyName, y => y.Address.County())
                .RuleFor(x => x.StartDate, y => y.Date.Recent())
                .RuleFor(x => x.EndDate, y => y.Date.Recent())
                .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 100) * 10);
            private static readonly Faker<DbCountyAppendLimitBo> appendBo =
                new Faker<DbCountyAppendLimitBo>()
                .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

            private static readonly Faker<CompleteUsageRecordRequest> completeFaker =
                new Faker<CompleteUsageRecordRequest>()
                .RuleFor(x => x.UsageRecordId, y => y.Random.AlphaNumeric(16))
                .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 100) * 10);

            private static readonly Faker<GetMonthlyLimitRequest> getlimitFaker =
                new Faker<GetMonthlyLimitRequest>()
                .RuleFor(x => x.LeadId, y => y.Random.AlphaNumeric(16))
                .RuleFor(x => x.CountyId, y => y.Random.Int(1, 50));

            private static readonly Faker<DbCountyUsageLimitBo> limitBo =
                new Faker<DbCountyUsageLimitBo>()
                .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
                .RuleFor(x => x.LeadUserId, y => y.Random.Guid().ToString())
                .RuleFor(x => x.CountyId, y => y.Random.Int(1, 101))
                .RuleFor(x => x.IsActive, y => y.Random.Bool())
                .RuleFor(x => x.MaxRecords, y => y.Random.Int(5, 2500))
                .RuleFor(x => x.CompleteDate, y => y.Date.Recent())
                .RuleFor(x => x.CreateDate, y => y.Date.Recent());

            private static readonly Faker<GetUsageRequest> getusageFaker =
                new Faker<GetUsageRequest>()
                .RuleFor(x => x.LeadId, y => y.Random.AlphaNumeric(16))
                .RuleFor(x => x.SearchDate, y => y.Date.Recent());

            private static readonly Faker<SetMonthlyLimitRequest> setlimitFaker =
                new Faker<SetMonthlyLimitRequest>()
                .RuleFor(x => x.LeadId, y => y.Random.AlphaNumeric(16))
                .RuleFor(x => x.CountyId, y => y.Random.Int(1, 50))
                .RuleFor(x => x.MonthLimit, y => y.Random.Int(1, 5000));


        }
    }
}
