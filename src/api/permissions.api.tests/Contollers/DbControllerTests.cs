using legallead.permissions.api.Controllers;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;

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

    }
}

