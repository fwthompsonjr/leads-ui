using legallead.permissions.api.Controllers;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Contollers
{
    public class QueueControllerTests : BaseControllerTest
    {
        [Theory]
        [InlineData("initialize")]
        [InlineData("update")]
        [InlineData("fetch")]
        [InlineData("start")]
        [InlineData("status")]
        [InlineData("complete")]
        [InlineData("finalize")]
        public async Task ControllerCanPost(string landing)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var controller = provider.GetRequiredService<QueueController>();
                var expectedCode = landing switch
                {
                    "fetch" => 400,
                    _ => 200
                };
                var action = landing switch
                {
                    "initialize" => controller.Initialize(new QueueInitializeRequest() { Source = applicationSource }),
                    "update" => await controller.Update(new QueueUpdateRequest() { Source = applicationSource }),
                    "fetch" => await controller.Fetch(GetRequest()),
                    "start" => await controller.Start(new QueuedRecord() { Source = applicationSource }),
                    "status" => await controller.Status(new QueueRecordStatusRequest() { Source = applicationSource }),
                    "complete" => await controller.Complete(new QueueRecordStatusRequest() { Source = applicationSource }),
                    "finalize" => await controller.Finalize(new QueueCompletionRequest() { Source = applicationSource }),
                    _ => new StatusCodeResult(500)
                };
                if (action is not JsonResult jsonResult)
                {
                    Assert.Fail("response is not of correct type.");
                    return;
                }
                Assert.Equal(expectedCode, jsonResult.StatusCode);

            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData("initialize")]
        [InlineData("update")]
        [InlineData("fetch")]
        [InlineData("start")]
        [InlineData("status")]
        [InlineData("complete")]
        [InlineData("finalize")]
        public async Task ControllerPostRequiresHeader(string landing)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var controller = provider.GetRequiredService<QueueController>();
                var mock = provider.GetRequiredService<Mock<HttpRequest>>();
                var headers = new HeaderDictionary();
                mock.SetupGet(m => m.Headers).Returns(headers);
                var action = landing switch
                {
                    "initialize" => controller.Initialize(new QueueInitializeRequest() { Source = applicationSource }),
                    "update" => await controller.Update(new QueueUpdateRequest() { Source = applicationSource }),
                    "fetch" => await controller.Fetch(GetRequest()),
                    "start" => await controller.Start(new QueuedRecord() { Source = applicationSource }),
                    "status" => await controller.Status(new QueueRecordStatusRequest() { Source = applicationSource }),
                    "complete" => await controller.Complete(new QueueRecordStatusRequest() { Source = applicationSource }),
                    "finalize" => await controller.Finalize(new QueueCompletionRequest() { Source = applicationSource }),
                    _ => new StatusCodeResult(500)
                };
                if (action is not BadRequestObjectResult _)
                    Assert.Fail("response is not of correct type.");
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task ControllerFetchRequiresValidName(int landingId)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var payload = GetRequest();
                var controller = provider.GetRequiredService<QueueController>();
                payload.Name = landingId switch
                {
                    0 => payload.Name,
                    1 => string.Empty,
                    2 => "   ",
                    _ => GetAppName()
                };
                var expectedCode = landingId switch
                {
                    0 => 400,
                    1 => 400,
                    2 => 400,
                    _ => 200
                };
                var action = await controller.Fetch(payload);
                if (action is not JsonResult jsonResult)
                {
                    Assert.Fail("response is not of correct type.");
                    return;
                }
                Assert.Equal(expectedCode, jsonResult.StatusCode);

            });
            Assert.Null(error);
        }

        private static ApplicationRequestModel GetRequest()
        {
            return new ApplicationRequestModel { Id = Guid.NewGuid(), Name = "oxford.test.client" };
        }
        private const string applicationSource = "oxford.leads.data.services";
        private static string GetAppName()
        {
            var fk = new Faker();
            var names = ApplicationModel.GetApplicationsFallback()
                .Select(x => x.Name).ToList();
            return fk.PickRandom(names);
        }


    }
}
