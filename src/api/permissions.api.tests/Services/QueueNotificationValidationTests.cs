using legallead.jdbc.entities;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Services;

namespace permissions.api.tests.Services
{
    public class QueueNotificationTests
    {

        [Theory]
        [InlineData(0, 5)]
        [InlineData(1, 5)]
        [InlineData(2, 5)]
        [InlineData(3, 5)]
        [InlineData(5, 5)]
        [InlineData(11, 5)]
        [InlineData(12, 5)]
        [InlineData(13, 5)]
        [InlineData(5, 0)]
        [InlineData(5, 1)]
        [InlineData(5, 2)]
        public void NotifactionCanValidate(int payloadTypeId, int userTypeId)
        {
            var userids = new[] { 0, 1, 2 };
            var searchids = new[] { 0, 1, 2, 3, 10, 11, 12, 13 };
            var search = faker.Generate();
            var user = userfaker.Generate();
            var service = new QueueNotificationValidationService();
            user.Id = userTypeId switch
            {
                0 => null,
                1 => string.Empty,
                2 => "   ",
                _ => user.Id
            };
            search.Payload = payloadTypeId switch
            {
                0 => null,
                1 => string.Empty,
                2 => "    ",
                3 => "this-is-not-serializable",
                _ => search.Payload
            };
            search.Id = payloadTypeId switch
            {
                10 => null,
                11 => string.Empty,
                12 => "    ",
                13 => "this-is-not-a-guid",
                _ => search.Id
            };
            var actual = service.IsValid(search, user).IsValid;
            var expected = true;
            if (searchids.Contains(payloadTypeId)) { expected = false; }
            if (userids.Contains(userTypeId)) { expected = false; }
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NotifactionCanSend()
        {
            var search = faker.Generate();
            var user = userfaker.Generate();
            var mock = new Mock<IQueueNotificationService>();
            var service = new MockMailMessageWrapper(mock.Object);
            mock.Setup(m => m.Send(It.IsAny<QueuedRecord>(), It.IsAny<QueueWorkingUserBo>())).Verifiable();
            service.Send(search, user);
            mock.Verify(m => m.Send(It.IsAny<QueuedRecord>(), It.IsAny<QueueWorkingUserBo>()));
        }

        private static string SamplePayload => samplepayload ??= GetSamplePayload();
        private static string? samplepayload;
        private static string GetSamplePayload()
        {
            return Properties.Resources.sample_search_requested_response;
        }

        private static readonly Faker<QueuedRecord> faker =
            new Faker<QueuedRecord>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.StartDate, y => y.Date.Recent(30))
            .RuleFor(x => x.EndDate, y => y.Date.Recent(60))
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(0, 50000))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(60))
            .RuleFor(x => x.Payload, y => SamplePayload);

        private static readonly Faker<QueueWorkingUserBo> userfaker =
            new Faker<QueueWorkingUserBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserName, y => y.Person.UserName)
        .RuleFor(x => x.Email, y => y.Person.Email);

        private sealed class MockMailMessageWrapper(IQueueNotificationService service) : MailMessageWrapper(service) { }
    }
}
