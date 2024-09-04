using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.json.db.entity;
using legallead.permissions.api;
using legallead.permissions.api.Utility;
using permissions.api.tests.Contollers;

namespace permissions.api.tests
{
    public class SubscriptionInfrastructureTests : BaseControllerTest
    {
        [Fact]
        public async Task AddCountySubscriptionsHappyPathAsync()
        {
            var response = new KeyValuePair<bool, string>(true, "Unit test is passed.");
            var tmpCounty = new UsStateCounty();
            var tmpUser = new User();
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.AddCountySubscriptionsAsync(It.IsAny<User>(), It.IsAny<UsStateCounty>())).ReturnsAsync(response);
            permissionHistoryMock.Setup(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()));

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.AddCountySubscriptionsAsync(tmpUser, tmpCounty);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Once);
        }

        [Fact]
        public async Task AddCountySubscriptionsNoSubscriptionPathAsync()
        {
            var response = new KeyValuePair<bool, string>(false, "Unit test is passed.");
            var tmpCounty = new UsStateCounty();
            var tmpUser = new User();
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.AddCountySubscriptionsAsync(It.IsAny<User>(), It.IsAny<UsStateCounty>())).ReturnsAsync(response);
            permissionHistoryMock.Setup(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()));

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.AddCountySubscriptionsAsync(tmpUser, tmpCounty);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Never);
        }

        [Fact]
        public async Task AddStateSubscriptionsHappyPathAsync()
        {
            var response = new KeyValuePair<bool, string>(true, "Unit test is passed.");
            var tmpState = "1234567";
            var tmpUser = new User();
            var snapshot = Task.CompletedTask;
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.AddStateSubscriptionsAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(response);
            permissionHistoryMock.SetupSequence(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()))
                .Returns(snapshot);

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.AddStateSubscriptionsAsync(tmpUser, tmpState);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Once);
        }

        [Fact]
        public async Task AddStateSubscriptionsNoSubscriptionPathAsync()
        {
            var response = new KeyValuePair<bool, string>(false, "Unit test is passed.");
            var tmpState = "1234567";
            var tmpUser = new User();
            var snapshot = Task.CompletedTask;
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.AddStateSubscriptionsAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(response);
            permissionHistoryMock.SetupSequence(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()))
                .Returns(snapshot);

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.AddStateSubscriptionsAsync(tmpUser, tmpState);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Never);
        }

        [Fact]
        public async Task RemoveCountySubscriptionsHappyPathAsync()
        {
            var response = new KeyValuePair<bool, string>(true, "Unit test is passed.");
            var tmpCounty = new UsStateCounty();
            var tmpUser = new User();
            var snapshot = Task.CompletedTask;
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.RemoveCountySubscriptionsAsync(It.IsAny<User>(), It.IsAny<UsStateCounty>())).ReturnsAsync(response);
            permissionHistoryMock.SetupSequence(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()))
                .Returns(snapshot);

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.RemoveCountySubscriptionsAsync(tmpUser, tmpCounty);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Once);
        }

        [Fact]
        public async Task RemoveCountySubscriptionsNoSubscriptionPathAsync()
        {
            var response = new KeyValuePair<bool, string>(false, "Unit test is passed.");
            var tmpCounty = new UsStateCounty();
            var tmpUser = new User();
            var snapshot = Task.CompletedTask;
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.RemoveCountySubscriptionsAsync(It.IsAny<User>(), It.IsAny<UsStateCounty>())).ReturnsAsync(response);
            permissionHistoryMock.SetupSequence(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()))
                .Returns(snapshot);

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.RemoveCountySubscriptionsAsync(tmpUser, tmpCounty);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Never);
        }

        [Fact]
        public async Task RemoveStateSubscriptionsHappyPathAsync()
        {
            var response = new KeyValuePair<bool, string>(true, "Unit test is passed.");
            var tmpState = "1234567";
            var tmpUser = new User();
            var snapshot = Task.CompletedTask;
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.RemoveStateSubscriptionsAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(response);
            permissionHistoryMock.SetupSequence(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()))
                .Returns(snapshot);

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.RemoveStateSubscriptionsAsync(tmpUser, tmpState);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Once);
        }

        [Fact]
        public async Task RemoveStateSubscriptionsNoSubscriptionPathAsync()
        {
            var response = new KeyValuePair<bool, string>(false, "Unit test is passed.");
            var tmpState = "1234567";
            var tmpUser = new User();
            var snapshot = Task.CompletedTask;
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.RemoveStateSubscriptionsAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(response);
            permissionHistoryMock.SetupSequence(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()))
                .Returns(snapshot);

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.RemoveStateSubscriptionsAsync(tmpUser, tmpState);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Never);
        }
    }
}