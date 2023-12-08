using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.json.db.entity;
using legallead.permissions.api;
using legallead.permissions.api.Utility;
using Microsoft.Extensions.DependencyInjection;
using permissions.api.tests.Contollers;

namespace permissions.api.tests
{
    public class SubscriptionInfrastructureTests : BaseControllerTest
    {
        [Fact]
        public async Task AddCountySubscriptionsHappyPath()
        {
            var response = new KeyValuePair<bool, string>(true, "Unit test is passed.");
            var tmpCounty = new UsStateCounty();
            var tmpUser = new User();
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.AddCountySubscriptions(It.IsAny<User>(), It.IsAny<UsStateCounty>())).ReturnsAsync(response);
            permissionHistoryMock.Setup(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()));

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.AddCountySubscriptions(tmpUser, tmpCounty);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Once);
        }

        [Fact]
        public async Task AddCountySubscriptionsNoSubscriptionPath()
        {
            var response = new KeyValuePair<bool, string>(false, "Unit test is passed.");
            var tmpCounty = new UsStateCounty();
            var tmpUser = new User();
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.AddCountySubscriptions(It.IsAny<User>(), It.IsAny<UsStateCounty>())).ReturnsAsync(response);
            permissionHistoryMock.Setup(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()));

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.AddCountySubscriptions(tmpUser, tmpCounty);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Never);
        }

        [Fact]
        public async Task AddStateSubscriptionsHappyPath()
        {
            var response = new KeyValuePair<bool, string>(true, "Unit test is passed.");
            var tmpState = "1234567";
            var tmpUser = new User();
            var snapshot = Task.CompletedTask;
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.AddStateSubscriptions(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(response);
            permissionHistoryMock.SetupSequence(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()))
                .Returns(snapshot);

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.AddStateSubscriptions(tmpUser, tmpState);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Once);
        }

        [Fact]
        public async Task AddStateSubscriptionsNoSubscriptionPath()
        {
            var response = new KeyValuePair<bool, string>(false, "Unit test is passed.");
            var tmpState = "1234567";
            var tmpUser = new User();
            var snapshot = Task.CompletedTask;
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.AddStateSubscriptions(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(response);
            permissionHistoryMock.SetupSequence(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()))
                .Returns(snapshot);

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.AddStateSubscriptions(tmpUser, tmpState);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Never);
        }
        [Fact]
        public async Task RemoveCountySubscriptionsHappyPath()
        {
            var response = new KeyValuePair<bool, string>(true, "Unit test is passed.");
            var tmpCounty = new UsStateCounty();
            var tmpUser = new User();
            var snapshot = Task.CompletedTask;
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.RemoveCountySubscriptions(It.IsAny<User>(), It.IsAny<UsStateCounty>())).ReturnsAsync(response);
            permissionHistoryMock.SetupSequence(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()))
                .Returns(snapshot);

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.RemoveCountySubscriptions(tmpUser, tmpCounty);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Once);
        }

        [Fact]
        public async Task RemoveCountySubscriptionsNoSubscriptionPath()
        {
            var response = new KeyValuePair<bool, string>(false, "Unit test is passed.");
            var tmpCounty = new UsStateCounty();
            var tmpUser = new User();
            var snapshot = Task.CompletedTask;
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.RemoveCountySubscriptions(It.IsAny<User>(), It.IsAny<UsStateCounty>())).ReturnsAsync(response);
            permissionHistoryMock.SetupSequence(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()))
                .Returns(snapshot);

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.RemoveCountySubscriptions(tmpUser, tmpCounty);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Never);
        }

        [Fact]
        public async Task RemoveStateSubscriptionsHappyPath()
        {
            var response = new KeyValuePair<bool, string>(true, "Unit test is passed.");
            var tmpState = "1234567";
            var tmpUser = new User();
            var snapshot = Task.CompletedTask;
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.RemoveStateSubscriptions(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(response);
            permissionHistoryMock.SetupSequence(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()))
                .Returns(snapshot);

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.RemoveStateSubscriptions(tmpUser, tmpState);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Once);
        }

        [Fact]
        public async Task RemoveStateSubscriptionsNoSubscriptionPath()
        {
            var response = new KeyValuePair<bool, string>(false, "Unit test is passed.");
            var tmpState = "1234567";
            var tmpUser = new User();
            var snapshot = Task.CompletedTask;
            var dp = new Mock<IDataProvider>();

            var permissionHistoryMock = new Mock<IUserPermissionHistoryRepository>();
            dp.SetupGet(x => x.PermissionHistoryDb).Returns(permissionHistoryMock.Object);
            dp.Setup(m => m.RemoveStateSubscriptions(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(response);
            permissionHistoryMock.SetupSequence(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()))
                .Returns(snapshot);

            var sut = new SubscriptionInfrastructure(dp.Object);
            var actual = await sut.RemoveStateSubscriptions(tmpUser, tmpState);

            Assert.Equal(response, actual);
            permissionHistoryMock.Verify(s => s.CreateSnapshot(It.IsAny<User>(), It.IsAny<legallead.jdbc.PermissionChangeTypes>()), Times.Never);
        }
    }
}
