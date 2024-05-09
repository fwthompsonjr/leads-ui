using legallead.jdbc.interfaces;
using legallead.permissions.api.Services;
using Microsoft.Extensions.Logging;

namespace permissions.api.tests.Services
{
    public class QueueResetServiceTests
    {
        [Fact]
        public void ServiceContainsRepository()
        {
            var service = new MockQueueResetService();
            Assert.NotNull(service);
            Assert.NotNull(service.GetRepository());
        }

        private sealed class MockQueueResetService : QueueResetService
        {
            private static readonly Mock<ILogger<QueueResetService>> MqLog = new();
            private static readonly Mock<IUserSearchRepository> MqRepo = new();

            public MockQueueResetService() : base(MqRepo.Object, MqLog.Object)
            {
            }

            public IUserSearchRepository GetRepository()
            {
                return QueueDb;
            }
        }
    }
}
