using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Services;

namespace permissions.api.tests.Services
{
    public class AppSettingServiceTest
    {
        [Fact]
        public void ServiceContainsRepository()
        {
            var service = new MockAppSettingService();
            Assert.NotNull(service);
            Assert.NotNull(service.GetRepository());
            Assert.NotNull(service.MqRepo);
        }
        [Theory]
        [InlineData("abc", 0)]
        [InlineData("efg", 1)]
        [InlineData("hij", 2)]
        [InlineData("123", 3)]
        [InlineData("456", 4)]
        public void ServiceCanFindKey(string keyname, int typeid)
        {
            var faker = new Faker();
            var keyvalue = typeid switch
            {
                2 => string.Empty,
                3 => null,
                4 => "    ",
                _ => faker.Random.AlphaNumeric(15)
            };
            var response = typeid switch
            {
                0 => null,
                _ => new AppSettingBo { KeyName = keyname, KeyValue = keyvalue }
            };
            var service = new MockAppSettingService();
            var mock = service.MqRepo;
            mock.Setup(m => m.FindKey(It.IsAny<string>())).Returns(response);
            _ = service.FindKey(keyname);
            mock.Verify(m => m.FindKey(It.IsAny<string>()));
        }

        private sealed class MockAppSettingService : AppSettingService
        {
            private static readonly Mock<IAppSettingRepository> mqRepo = new();

            public MockAppSettingService() : base(mqRepo.Object)
            {
            }
            public Mock<IAppSettingRepository> MqRepo => mqRepo;
            public IAppSettingRepository GetRepository()
            {
                return MqRepo.Object;
            }
        }
    }
}