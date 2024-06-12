using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Services;

namespace permissions.api.tests.Services
{
    public class UserMailboxServiceTests
    {
        private static readonly Faker<EmailBodyBo> bodyFaker = new();

        private static readonly Faker<EmailCountBo> countFaker = new();

        private static readonly Faker<EmailListBo> listFaker = new();

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanGetBody(bool hasResponse)
        {
            var request = new MailboxRequest();
            var response = hasResponse ? bodyFaker.Generate() : null;
            var svc = new Mock<IMailBoxRepository>();
            svc.Setup(x => x.GetBody(
                It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(response);
            var sut = new UserMailboxService(svc.Object);
            _ = await sut.GetBody(request);
            svc.Verify(x => x.GetBody(
                It.IsAny<string>(),
                It.IsAny<string>()));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ServiceCanGetCount(bool hasResponse)
        {
            var request = new MailboxRequest();
            var response = hasResponse ? countFaker.Generate() : null;
            var svc = new Mock<IMailBoxRepository>();
            svc.Setup(x => x.GetCount(
                It.IsAny<string>())).ReturnsAsync(response);
            var sut = new UserMailboxService(svc.Object);
            _ = await sut.GetCount(request);
            svc.Verify(x => x.GetCount(
                It.IsAny<string>()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public async Task ServiceCanGetMailMessages(int responseId)
        {
            var request = new MailboxRequest();
            var response = responseId switch
            {
                0 => null,
                _ => listFaker.Generate(responseId)
            };
            var svc = new Mock<IMailBoxRepository>();
            svc.Setup(x => x.GetMailMessages(
                It.IsAny<string>(), It.IsAny<DateTime?>())).ReturnsAsync(response);
            var sut = new UserMailboxService(svc.Object);
            _ = await sut.GetMailMessages(request);
            svc.Verify(x => x.GetMailMessages(
                It.IsAny<string>(), It.IsAny<DateTime?>()));
        }
    }
}
