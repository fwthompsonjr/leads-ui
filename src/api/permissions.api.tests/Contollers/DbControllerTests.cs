using legallead.jdbc.entities;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Contollers
{
    public class DbControllerTests : BaseControllerTest
    {
        [Fact]
        public void ControllerCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                var provider = GetProvider();
                _ = provider.GetRequiredService<DbController>();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task ControllerCanBeginAsync(int testId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<DbController>();
                SetupUserInteractions(testId, provider);
                _ = await sut.BeginAsync(new());
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task ControllerCanCompleteAsync(int testId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<DbController>();
                SetupUserInteractions(testId, provider);
                _ = await sut.CompleteAsync(new());
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task ControllerCanFindAsync(int testId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<DbController>();
                SetupUserInteractions(testId, provider);
                _ = await sut.FindAsync(new());
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task ControllerCanUploadAsync(int testId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<DbController>();
                SetupUserInteractions(testId, provider);
                _ = await sut.UploadAsync(new());
            });
            Assert.Null(error);
        }



        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(0, -1)]
        [InlineData(0, 0, "", false)]
        [InlineData(0, 0, "alphabetic", false)]
        [InlineData(0, 0, "8/8/2001")]
        [InlineData(0, 0, "8-8-2001")]
        [InlineData(0, 1)]
        public async Task ControllerCanExecuteIsHolidayAsync(
            int testId = 0,
            int historyId = 0,
            string inputDate = "2020-02-02",
            bool isValidDate = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<DbController>();
                SetupUserInteractions(testId, provider);
                SetupHistoryInteractions(historyId, provider);
                var response = await sut.IsHolidayAsync(new() { HolidayDate = inputDate });
                if (testId == 1 && historyId >= 0 && isValidDate)
                {
                    Assert.IsAssignableFrom<OkObjectResult>(response);
                }
                else
                {
                    Assert.IsNotAssignableFrom<OkObjectResult>(response);
                }

            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(1, -1)]
        [InlineData(1, -100)]
        [InlineData(1, 100)]
        [InlineData(1, 0, "8/8/2001", false)]
        [InlineData(1, 0, "alphabet", false)]
        [InlineData(0, 1)]
        public async Task ControllerCanQueryHolidayAsync(
            int testId = 0,
            int historyId = 0,
            string inputDate = "",
            bool isValidDate = true)
        {
            var isvalidhistory = historyId >= 0 || historyId == -100;
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<DbController>();
                SetupUserInteractions(testId, provider);
                SetupHistoryListResponse(historyId, provider);
                var response = await sut.QueryHolidayAsync(new() { HolidayDate = inputDate });
                if (testId == 1 && isvalidhistory && isValidDate)
                {
                    Assert.IsAssignableFrom<OkObjectResult>(response);
                }
                else
                {
                    Assert.IsNotAssignableFrom<OkObjectResult>(response);
                }
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ControllerCanAppendUsageAsync(int testId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<DbController>();
                var request = UsageServiceMock.GetRequestJson(0).ToInstance<AppendUsageRecordRequest>() ?? new();
                if (testId == 0) request.LeadUserId = string.Empty;
                if (testId == 1) request.CountyId = 0;
                var response = await sut.UsageAppendAsync(request);
                if (testId == 1 || testId == 0)
                {
                    Assert.IsAssignableFrom<BadRequestResult>(response);
                }
                else
                {
                    Assert.IsAssignableFrom<OkObjectResult>(response);
                }
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ControllerCanCompleteUsageAsync(int testId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<DbController>();
                var request = UsageServiceMock.GetRequestJson(1).ToInstance<CompleteUsageRecordRequest>() ?? new();
                if (testId == 0) request.UsageRecordId = string.Empty;
                if (testId == 1) request.RecordCount = 0;
                var response = await sut.UsageCompleteAsync(request);
                if (testId == 1 || testId == 0)
                {
                    Assert.IsAssignableFrom<BadRequestResult>(response);
                }
                else
                {
                    Assert.IsAssignableFrom<OkObjectResult>(response);
                }
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task ControllerCanGetUsageLimitAsync(int testId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<DbController>();
                var request = UsageServiceMock.GetRequestJson(2).ToInstance<GetMonthlyLimitRequest>() ?? new();
                if (testId == 0) request.LeadId = string.Empty;
                var response = await sut.UsageGetLimitAsync(request);
                if (testId == 0)
                {
                    Assert.IsAssignableFrom<BadRequestResult>(response);
                }
                else
                {
                    Assert.IsAssignableFrom<OkObjectResult>(response);
                }
            });
            Assert.Null(error);
        }



        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task ControllerCanGetUsageHistoryAsync(int testId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<DbController>();
                var request = UsageServiceMock.GetRequestJson(4).ToInstance<GetUsageRequest>() ?? new();
                if (testId == 0) request.LeadId = string.Empty;
                var response = await sut.UsageGetHistoryAsync(request);
                if (testId == 0)
                {
                    Assert.IsAssignableFrom<BadRequestResult>(response);
                }
                else
                {
                    Assert.IsAssignableFrom<OkObjectResult>(response);
                }
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task ControllerCanGetUsageSummaryAsync(int testId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<DbController>();
                var request = UsageServiceMock.GetRequestJson(4).ToInstance<GetUsageRequest>() ?? new();
                if (testId == 0) request.LeadId = string.Empty;
                var response = await sut.UsageGetSummaryAsync(request);
                if (testId == 0)
                {
                    Assert.IsAssignableFrom<BadRequestResult>(response);
                }
                else
                {
                    Assert.IsAssignableFrom<OkObjectResult>(response);
                }
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task ControllerCanGetExcelDetailByIdAsync(int testId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<DbController>();
                var mock = provider.GetRequiredService<Mock<IUserUsageService>>();
                var request = "get-record-test";
                var data = new GetExcelDetailByIdResponse { };
                mock.Setup(m => m.GetExcelDetailAsync(It.IsAny<string>())).ReturnsAsync(data);
                if (testId == 0) request = string.Empty;
                var response = await sut.GetExcelDetailByIdAsync(new() { UsageRecordId = request });
                if (testId == 0)
                {
                    Assert.IsAssignableFrom<BadRequestResult>(response);
                }
                else
                {
                    Assert.IsAssignableFrom<OkObjectResult>(response);
                }
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task ControllerCanSetUsageLimitAsync(int testId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var sut = provider.GetRequiredService<DbController>();
                var request = UsageServiceMock.GetRequestJson(6).ToInstance<SetMonthlyLimitRequest>() ?? new();
                if (testId == 0) request.LeadId = string.Empty;
                var response = await sut.UsageSetLimitAsync(request);
                if (testId == 0)
                {
                    Assert.IsAssignableFrom<BadRequestResult>(response);
                }
                else
                {
                    Assert.IsAssignableFrom<OkObjectResult>(response);
                }
            });
            Assert.Null(error);
        }
        private static readonly Faker<HolidayResponse> listFaker
            = new Faker<HolidayResponse>()
            .RuleFor(x => x.HoliDate, y => y.Date.Past());

        private static void SetupUserInteractions(int testId, IServiceProvider provider)
        {
            LeadUserModel? empty = null;
            LeadUserModel user = new();
            var leadsvc = provider.GetRequiredService<Mock<ILeadAuthenicationService>>();
            if (testId == -1)
            {
                leadsvc.Setup(x => x.GetUserModel(
                    It.IsAny<HttpRequest>(),
                    It.IsAny<string>())).Throws<InvalidOperationException>();
            }
            if (testId == 0)
            {
                leadsvc.Setup(x => x.GetUserModel(
                    It.IsAny<HttpRequest>(),
                    It.IsAny<string>())).Returns(empty);
            }
            if (testId == 1)
            {
                leadsvc.Setup(x => x.GetUserModel(
                    It.IsAny<HttpRequest>(),
                    It.IsAny<string>())).Returns(user);
            }
            var dbmock = provider.GetRequiredService<Mock<IDbHistoryService>>();
            var rsp = new DataRequestResponse();
            var rlist = new List<FindRequestResponse>();
            var response = new KeyValuePair<bool, string>(true, "unit testing");
            dbmock.Setup(x => x.BeginAsync(It.IsAny<BeginDataRequest>())).ReturnsAsync(rsp);
            dbmock.Setup(x => x.CompleteAsync(It.IsAny<CompleteDataRequest>())).ReturnsAsync(rsp);
            dbmock.Setup(x => x.FindAsync(It.IsAny<FindDataRequest>())).ReturnsAsync(rlist);
            dbmock.Setup(x => x.UploadAsync(It.IsAny<UploadDataRequest>())).ReturnsAsync(response);
        }


        private static void SetupHistoryInteractions(int historyId, IServiceProvider provider)
        {
            var svc = provider.GetRequiredService<Mock<IHolidayService>>();
            if (historyId == -1)
            {
                svc.Setup(x => x.IsHolidayAsync(
                    It.IsAny<string>())).Throws<InvalidOperationException>();
            }
            if (historyId == 0)
            {
                svc.Setup(x => x.IsHolidayAsync(
                    It.IsAny<string>())).ReturnsAsync(false);
            }
            if (historyId == 1)
            {
                svc.Setup(x => x.IsHolidayAsync(
                    It.IsAny<string>())).ReturnsAsync(true);
            }
        }

        private static void SetupHistoryListResponse(int historyId, IServiceProvider provider)
        {
            var svc = provider.GetRequiredService<Mock<IHolidayService>>();
            List<HolidayResponse>? response = default;
            if (historyId == -1)
            {
                svc.Setup(x => x.GetHolidaysAsync()).Throws<InvalidOperationException>();
                return;
            }

            if (historyId == -100)
            {
                svc.Setup(x => x.GetHolidaysAsync()).ReturnsAsync(response);
                return;
            }
            response = listFaker.Generate(historyId);
            svc.Setup(x => x.GetHolidaysAsync()).ReturnsAsync(response);
        }


        private static class UsageServiceMock
        {

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

            private static readonly Faker<AppendUsageRecordRequest> appendFaker =
                new Faker<AppendUsageRecordRequest>()
                .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(16))
                .RuleFor(x => x.CountyId, y => y.Random.Int(1, 50))
                .RuleFor(x => x.CountyName, y => y.Address.County())
                .RuleFor(x => x.StartDate, y => y.Date.Recent())
                .RuleFor(x => x.EndDate, y => y.Date.Recent())
                .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 100) * 10);

            private static readonly Faker<CompleteUsageRecordRequest> completeFaker =
                new Faker<CompleteUsageRecordRequest>()
                .RuleFor(x => x.UsageRecordId, y => y.Random.AlphaNumeric(16))
                .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 100) * 10)
                .RuleFor(x => x.ExcelName, y=> y.System.FileName(".xlsx"));

            private static readonly Faker<GetMonthlyLimitRequest> getlimitFaker =
                new Faker<GetMonthlyLimitRequest>()
                .RuleFor(x => x.LeadId, y => y.Random.AlphaNumeric(16))
                .RuleFor(x => x.CountyId, y => y.Random.Int(1, 50));

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

