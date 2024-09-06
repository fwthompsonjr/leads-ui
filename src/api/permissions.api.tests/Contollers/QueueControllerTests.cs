using legallead.permissions.api.Controllers;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

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
        [InlineData("save")]
        [InlineData("queue-status")]
        [InlineData("queue-summary")]
        public async Task ControllerCanPostAsync(string landing)
        {
            var persistence = GetPersistence();
            persistence.Source = applicationSource;
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
                    "update" => await controller.UpdateAsync(new QueueUpdateRequest() { Source = applicationSource }),
                    "fetch" => await controller.FetchAsync(GetRequest()),
                    "start" => await controller.StartAsync(new QueuedRecord() { Source = applicationSource }),
                    "status" => await controller.StatusAsync(new QueueRecordStatusRequest() { Source = applicationSource }),
                    "complete" => await controller.CompleteAsync(new QueueRecordStatusRequest() { Source = applicationSource }),
                    "finalize" => await controller.FinalizeAsync(new QueueCompletionRequest() { Source = applicationSource }),
                    "save" => await controller.SaveAsync(persistence),
                    "queue-status" => await controller.GetQueueStatusAsync(GetSummary(0)),
                    "queue-summary" => await controller.GetQueueSummaryAsync(GetSummary(0)),
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
        [InlineData("save")]
        [InlineData("queue-status")]
        [InlineData("queue-summary")]
        public async Task ControllerPostRequiresHeaderAsync(string landing)
        {
            var persistence = GetPersistence();
            persistence.Source = applicationSource;
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
                    "update" => await controller.UpdateAsync(new QueueUpdateRequest() { Source = applicationSource }),
                    "fetch" => await controller.FetchAsync(GetRequest()),
                    "start" => await controller.StartAsync(new QueuedRecord() { Source = applicationSource }),
                    "status" => await controller.StatusAsync(new QueueRecordStatusRequest() { Source = applicationSource }),
                    "complete" => await controller.CompleteAsync(new QueueRecordStatusRequest() { Source = applicationSource }),
                    "finalize" => await controller.FinalizeAsync(new QueueCompletionRequest() { Source = applicationSource }),
                    "save" => await controller.SaveAsync(persistence),
                    "queue-status" => await controller.GetQueueStatusAsync(GetSummary(0)),
                    "queue-summary" => await controller.GetQueueSummaryAsync(GetSummary(0)),
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
        public async Task ControllerFetchRequiresValidNameAsync(int landingId)
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
                var action = await controller.FetchAsync(payload);
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
        [InlineData("start")]
        [InlineData("status")]
        [InlineData("complete")]
        [InlineData("finalize")]
        [InlineData("save")]
        [InlineData("save-invalid")]
        [InlineData("save-no-id")]
        [InlineData("save-no-content")]
        public async Task ControllerPostNeedsSourceAsync(string landing)
        {
            var persistence = GetPersistence();
            if (landing == "save-no-id") persistence.Id = string.Empty;
            if (landing == "save-no-content") persistence.Content = null;
            var error = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var controller = provider.GetRequiredService<QueueController>();
                var expectedCode = landing switch
                {
                    "fetch" => 400,
                    _ => 401
                };
                var action = landing switch
                {
                    "initialize" => controller.Initialize(new QueueInitializeRequest()),
                    "update" => await controller.UpdateAsync(new QueueUpdateRequest()),
                    "fetch" => await controller.FetchAsync(GetRequest()),
                    "start" => await controller.StartAsync(new QueuedRecord()),
                    "status" => await controller.StatusAsync(new QueueRecordStatusRequest()),
                    "complete" => await controller.CompleteAsync(new QueueRecordStatusRequest()),
                    "finalize" => await controller.FinalizeAsync(new QueueCompletionRequest()),
                    "save" => await controller.SaveAsync(GetPersistence()),
                    "save-invalid" => await controller.SaveAsync(new QueuePersistenceRequest()),
                    "save-no-id" => await controller.SaveAsync(persistence),
                    "save-no-content" => await controller.SaveAsync(persistence),
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
        
        private static QueueSummaryRequest GetSummary(int? index, bool hasSource = true)
        {
            var source = hasSource ? applicationSource : string.Empty;
            return new()
            {
                StatusId = index,
                Source = source
            };
        }

        private static ApplicationRequestModel GetRequest()
        {
            return new ApplicationRequestModel { Id = Guid.NewGuid(), Name = "oxford.test.client" };
        }
        private static QueuePersistenceRequest GetPersistence()
        {
            var name = "oxford.test.client";
            var content = Encoding.UTF8.GetBytes(name);
            return new QueuePersistenceRequest
            {
                Id = Guid.NewGuid().ToString(),
                Content = content
            };
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
