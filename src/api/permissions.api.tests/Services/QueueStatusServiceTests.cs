using legallead.jdbc;
using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Services;
using Newtonsoft.Json;

namespace permissions.api.tests.Services
{
    public class QueueStatusServiceTests
    {
        [Fact]
        public void ServiceCanBeConstructed()
        {
            var error = Record.Exception(() => { _ = new TheHarness(); });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ServiceCanInsert(int id)
        {
            var exception = new Faker().System.Exception();
            var sut = new TheHarness();
            var service = sut.Service;
            var mock = sut.MqQueueRepo;
            var request = faker.Generate();
            var response = new List<QueueWorkingBo>();
            if (id == 0)
            {
                mock.Setup(m => m.InsertRange(It.IsAny<string>())).Throws(exception);
            }
            else
            {
                mock.Setup(m => m.InsertRange(It.IsAny<string>())).Returns(response);
            }
            service.Insert(request);
            mock.Verify();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(1, 10)]
        [InlineData(1, 11)]
        [InlineData(1, 12)]
        [InlineData(1, 20)]
        [InlineData(1, 21)]
        [InlineData(1, 22)]
        [InlineData(1, 30)]
        [InlineData(1, 31)]
        [InlineData(1, 32)]
        public async Task ServiceCanUpdate(int id, int requestId = 5)
        {
            int[] badrequests = [0, 1, 2, 10, 11, 12, 20, 21, 22, 30, 31];
            var fkr = new Faker();
            var exception = new Faker().System.Exception();
            var sut = new TheHarness();
            var service = sut.Service;
            var mock = sut.MqQueueRepo;
            var request = updatefaker.Generate();
            var response = new QueueWorkingBo();
            request.Id = requestId switch
            {
                0 => null,
                1 => string.Empty,
                2 => "not-a-guid",
                _ => request.Id,
            };
            request.SearchId = requestId switch
            {
                10 => null,
                11 => string.Empty,
                12 => "not-a-guid",
                _ => request.SearchId,
            };
            request.Message = requestId switch
            {
                20 => null,
                21 => string.Empty,
                22 => fkr.Random.AlphaNumeric(300),
                _ => request.Message,
            };
            request.StatusId = requestId switch
            {
                30 => null,
                31 => 100,
                _ => request.StatusId,
            };
            if (id == 0)
            {
                mock.Setup(m => m.UpdateStatus(It.IsAny<QueueWorkingBo>())).Throws(exception);
            }
            else
            {
                mock.Setup(m => m.UpdateStatus(It.IsAny<QueueWorkingBo>())).Returns(response);
            }
            _ = await service.Update(request);
            if (badrequests.Contains(requestId))
            {
                mock.Verify(m => m.UpdateStatus(It.IsAny<QueueWorkingBo>()), Times.Never());
                return;
            }
            mock.Verify();
        }

        [Fact]
        public async Task ServiceCanStart()
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var payload = recordfaker.Generate();
                var response = new KeyValuePair<bool, string>(true, "");
                var sut = new TheHarness();
                var service = sut.Service;
                var mock = sut.MqSearchRepo;
                mock.Setup(m => m.Start(It.IsAny<SearchDto>())).ReturnsAsync(response);
                _ = await service.Start(payload);
                mock.Verify(m => m.Start(It.IsAny<SearchDto>()));
            });
            Assert.Null(error);
        }


        [Fact]
        public async Task ServiceCanComplete()
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var fkr = new Faker();
                var payload = new QueueRecordStatusRequest
                {
                    UniqueId = fkr.Random.Guid().ToString(),
                    MessageId = fkr.Random.Int(0, 6),
                    StatusId = fkr.Random.Int(0, 2)
                };
                var response = new KeyValuePair<bool, string>(true, "");
                var sut = new TheHarness();
                var service = sut.Service;
                var mock = sut.MqSearchRepo;
                mock.Setup(m => m.Complete(It.IsAny<string>())).ReturnsAsync(response);
                await service.Complete(payload);
                mock.Verify(m => m.Complete(It.IsAny<string>()));
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(1, 0)]
        [InlineData(1, 50)]
        public async Task ServiceCanFetch(int id, int recordCount = 5)
        {
            var fkr = new Faker();
            var exception = new Faker().System.Exception();
            var sut = new TheHarness();
            var service = sut.Service;
            var mock = sut.MqSearchRepo;
            var qmock = sut.MqQueueRepo;
            var response = bofaker.Generate(recordCount);
            var qresponse = new List<QueueWorkingBo>();
            response.ForEach(r =>
            {
                var add = fkr.Random.Bool();
                if (add)
                {
                    qresponse.Add(new() { Id = r.Id });
                }
            });
            qmock.Setup(x => x.Fetch()).Returns(qresponse);
            if (id == 0) mock.Setup(m => m.GetQueue()).ThrowsAsync(exception);
            else mock.Setup(m => m.GetQueue()).ReturnsAsync(response);

            _ = await service.Fetch();
            mock.Verify(m => m.GetQueue());
        }


        [Theory]
        [InlineData(30)]
        [InlineData(20)]
        [InlineData(20, 5, 1)]
        [InlineData(20, 10)]
        [InlineData(20, 15)]
        [InlineData(20, 20)]
        [InlineData(20, 25)]
        public async Task ServiceCanCompleteGeneration(int webId, int rowcount = 5, int identityId = 5)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var fkr = new Faker();
                var itm = itemfaker.Generate();
                var data = personfaker.Generate(rowcount);
                var id = identityId switch
                {
                    1 => string.Empty,
                    _ => fkr.Random.Guid().ToString()
                };
                var parameter = rowcount switch
                {
                    10 => string.Empty,
                    20 => "not serializable",
                    _ => JsonConvert.SerializeObject(itm)
                };
                var dataparameter = rowcount switch
                {
                    15 => string.Empty,
                    25 => "not serializable",
                    _ => JsonConvert.SerializeObject(data)
                };
                itm.WebId = webId;
                var payload = new QueueCompletionRequest
                {
                    UniqueId = id,
                    QueryParameter = parameter,
                    Data = dataparameter
                };
                var response = new KeyValuePair<bool, string>(true, "");
                var sut = new TheHarness();
                var service = sut.Service;
                var mock = sut.MqSearchRepo;
                var usrmock = sut.MqUserRepo;
                mock.Setup(m => m.Complete(It.IsAny<string>())).ReturnsAsync(response);
                usrmock.Setup(m => m.Append(
                    It.IsAny<SearchTargetTypes>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())).ReturnsAsync(response);
                usrmock.Setup(m => m.UpdateRowCount(
                    It.IsAny<string>(),
                    It.IsAny<int>())).ReturnsAsync(response);
                await service.GenerationComplete(payload);
                mock.Verify(m => m.Complete(It.IsAny<string>()));
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(5, 1)]
        [InlineData(5, 2)]
        [InlineData(5, 3)]
        [InlineData(5, 4)]
        [InlineData(5, 5)]
        [InlineData(5, 6)]
        [InlineData(5, 7)]
        [InlineData(5, 8)]
        [InlineData(5, 9)]
        [InlineData(5, 10)]
        [InlineData(5, 1, 10)]
        [InlineData(5, 1, 1)]
        [InlineData(5, 1, 2)]
        [InlineData(5, 1, 3)]
        [InlineData(5, 1, 4)]
        [InlineData(10, 1, 1)]
        public async Task ServiceCanPostStatus(int identityId, int messageId = 0, int statusId = 0)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var fkr = new Faker();
                var id = identityId switch
                {
                    1 => string.Empty,
                    _ => fkr.Random.Guid().ToString()
                };
                int? messageIndx = messageId switch
                {
                    10 => null,
                    _ => messageId
                };
                int? statusCode = statusId switch
                {
                    10 => null,
                    _ => statusId
                };
                var payload = new QueueRecordStatusRequest
                {
                    UniqueId = id,
                    MessageId = messageIndx,
                    StatusId = statusCode
                };
                var response = new KeyValuePair<bool, string>(identityId != 10, "");
                var sut = new TheHarness();
                var service = sut.Service;
                var mock = sut.MqSearchRepo;
                var usrmock = sut.MqStatusRepo;
                mock.Setup(m => m.Status(
                    It.IsAny<string>(),
                    It.IsAny<string>())).ReturnsAsync(response);
                usrmock.Setup(m => m.Update(It.IsAny<WorkStatusBo>())).Returns(true);
                await service.PostStatus(payload);
            });
            Assert.Null(error);
        }


        private sealed class TheHarness
        {
            public TheHarness()
            {
                Service = new(MqQueueRepo.Object, MqSearchRepo.Object, MqStatusRepo.Object, MqUserRepo.Object);
            }
            public Mock<IQueueWorkRepository> MqQueueRepo { get; set; } = new();
            public Mock<ISearchQueueRepository> MqSearchRepo { get; set; } = new();
            public Mock<ISearchStatusRepository> MqStatusRepo { get; set; } = new();
            public Mock<IUserSearchRepository> MqUserRepo { get; set; } = new();
            public QueueStatusService Service { get; }
        }

        private static string SamplePayload => samplepayload ??= GetSamplePayload();
        private static string? samplepayload;
        private static string GetSamplePayload()
        {
            return Properties.Resources.sample_search_requested_response;
        }

        private static readonly Faker<QueueInitializeRequestItem> faker1 =
            new Faker<QueueInitializeRequestItem>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        private static readonly Faker<QueueInitializeRequest> faker =
            new Faker<QueueInitializeRequest>()
            .RuleFor(x => x.MachineName, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Message, y => y.Random.Guid().ToString())
            .RuleFor(x => x.StatusId, y => y.Random.Int(0, 500000))
            .RuleFor(x => x.Items, y =>
            {
                var n = y.Random.Int(1, 10);
                return faker1.Generate(n);
            });
        private static readonly Faker<QueueUpdateRequest> updatefaker =
            new Faker<QueueUpdateRequest>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Message, y => y.Random.AlphaNumeric(250))
            .RuleFor(x => x.StatusId, y => y.Random.Int(-1, 2));

        private static readonly Faker<SearchQueueDto> bofaker =
            new Faker<SearchQueueDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.StartDate, y => y.Date.Recent(30))
            .RuleFor(x => x.EndDate, y => y.Date.Recent(60))
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(0, 50000))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(60))
            .RuleFor(x => x.Payload, y => SamplePayload);

        private static readonly Faker<QueuedRecord> recordfaker =
            new Faker<QueuedRecord>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.StartDate, y => y.Date.Recent(30))
            .RuleFor(x => x.EndDate, y => y.Date.Recent(60))
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(0, 50000))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(60))
            .RuleFor(x => x.Payload, y => SamplePayload);

        private static readonly Faker<QueueSearchItem> itemfaker =
            new Faker<QueueSearchItem>()
            .RuleFor(x => x.WebId, y => y.Random.Int(0, 100))
            .RuleFor(x => x.State, y => "TX")
            .RuleFor(x => x.County, y => y.PickRandom("DENTON,COLLIN".Split(',')))
            .RuleFor(x => x.StartDate, y => y.Date.Recent(30).ToString("f"))
            .RuleFor(x => x.EndDate, y => y.Date.Recent(60).ToString("f"));


        private static readonly Faker<QueuePersonItem> personfaker =
            new Faker<QueuePersonItem>()
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Zip, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Address1, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Address2, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Address3, y => y.Random.Guid().ToString())
            .RuleFor(x => x.CaseNumber, y => y.Random.Guid().ToString())
            .RuleFor(x => x.DateFiled, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Court, y => y.Random.Guid().ToString())
            .RuleFor(x => x.CaseType, y => y.Random.Guid().ToString())
            .RuleFor(x => x.CaseStyle, y => y.Random.Guid().ToString())
            .RuleFor(x => x.FirstName, y => y.Random.Guid().ToString())
            .RuleFor(x => x.LastName, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Plantiff, y => y.Random.AlphaNumeric(250))
            .RuleFor(x => x.Status, y => y.Random.Int(1, 20000).ToString());
    }
}
