using legallead.permissions.api.Controllers;
using legallead.permissions.api.Entities;
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
    }
}

