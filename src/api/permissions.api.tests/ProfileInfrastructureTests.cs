using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Model;
using legallead.permissions.api.Utility;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace permissions.api.tests
{
    public class ProfileInfrastructureTests
    {
        private static readonly Faker<User> userFaker =
            new Faker<User>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.UserName, y => y.Person.FullName)
            .RuleFor(x => x.PasswordHash, y => y.Random.AlphaNumeric(50))
            .RuleFor(x => x.PasswordSalt, y => y.Random.AlphaNumeric(25));

        private static readonly Faker<UserProfileView> profileFaker =
            new Faker<UserProfileView>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ProfileMapId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.OrderId, y => y.IndexFaker)
            .RuleFor(x => x.KeyValue, y => y.Random.AlphaNumeric(50))
            .RuleFor(x => x.KeyName, y => y.Random.AlphaNumeric(25));

        private static readonly Faker<UserPermissionView> permissionFaker =
            new Faker<UserPermissionView>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.PermissionMapId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyValue, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.KeyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.OrderId, y => y.Random.Int(5, 25055));

        [Fact]
        public void ProfileCanBeCreated()
        {
            var exception = Record.Exception(() => _ = GetInfrastructure());
            Assert.Null(exception);
        }

        [Fact]
        public async Task ProfileGetContactDetailNoUser()
        {
            var service = GetInfrastructure();
            var response = await service.GetContactDetail(null, string.Empty);
            Assert.NotNull(response);
            Assert.Single(response);
            var item = response[0];
            Assert.Equal("Error", item.ResponseType);
            Assert.Equal("Unable to retrieve user detail", item.Message);
        }

        [Fact]
        public async Task ProfileGetContactRoleNoUser()
        {
            User? user = null;
            var service = GetInfrastructure();
            var response = await service.GetContactRole(user);
            Assert.NotNull(response);
            Assert.Equal("Guest", response);
        }

        [Theory]
        [InlineData("Guest")]
        [InlineData("Admin")]
        [InlineData("Platinum")]
        [InlineData("Gold")]
        [InlineData("Silver")]
        public async Task ProfileGetContactRole(string roleName)
        {
            var provider = GetTestArtifacts();
            var service = provider.GetRequiredService<IProfileInfrastructure>();
            var db = provider.GetRequiredService<Mock<IUserPermissionViewRepository>>();
            var user = provider.GetRequiredService<User>();
            var permissions = GetPermissions(roleName);
            db.Setup(m => m.GetAll(It.IsAny<User>())).ReturnsAsync(permissions);
            var response = await service.GetContactRole(user);
            Assert.NotNull(response);
            Assert.Equal(roleName, response);
        }

        [Fact]
        public async Task ProfileGetContactWithRoleMissingReturnsDefault()
        {
            const string permissionName = "Account.Permission.Level";
            var provider = GetTestArtifacts();
            var service = provider.GetRequiredService<IProfileInfrastructure>();
            var db = provider.GetRequiredService<Mock<IUserPermissionViewRepository>>();
            var user = provider.GetRequiredService<User>();
            var permissions = GetPermissions("").ToList();
            permissions.RemoveAll(a => a.KeyName.Equals(permissionName));
            db.Setup(m => m.GetAll(It.IsAny<User>())).ReturnsAsync(permissions);
            var response = await service.GetContactRole(user);
            Assert.NotNull(response);
            Assert.Equal("Guest", response);
        }

        [Fact]
        public async Task ProfileGetContactWithRoleEmptyReturnsDefault()
        {
            var provider = GetTestArtifacts();
            var service = provider.GetRequiredService<IProfileInfrastructure>();
            var db = provider.GetRequiredService<Mock<IUserPermissionViewRepository>>();
            var user = provider.GetRequiredService<User>();
            var permissions = GetPermissions("").ToList();
            db.Setup(m => m.GetAll(It.IsAny<User>())).ReturnsAsync(permissions);
            var response = await service.GetContactRole(user);
            Assert.NotNull(response);
            Assert.Equal("Guest", response);
        }

        [Fact]
        public async Task ProfileCanGetContactDetailNoResponseType()
        {
            var provider = GetTestArtifacts();
            var service = GetInfrastructure();
            var user = provider.GetRequiredService<User>();
            var response = await service.GetContactDetail(user, string.Empty);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Equal(4, response.Length);
        }

        [Theory]
        [InlineData("Address")]
        [InlineData("Email")]
        [InlineData("Phone")]
        [InlineData("Name")]
        public async Task ProfileCanGetContactDetailResponseType(string typeName)
        {
            var provider = GetTestArtifacts();
            var service = GetInfrastructure();
            var user = provider.GetRequiredService<User>();
            var response = await service.GetContactDetail(user, typeName);
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Single(response);
        }

        [Fact]
        public async Task ProfileGetContactDetailWillHandleException()
        {
            var provider = GetTestArtifacts();
            var service = provider.GetRequiredService<IProfileInfrastructure>();
            var user = provider.GetRequiredService<User>();
            var userProfileVwMock = provider.GetRequiredService<Mock<IUserProfileViewRepository>>();
            var exception = new Faker().System.Exception();
            var message = exception.Message;
            userProfileVwMock.Setup(m => m.GetAll(It.IsAny<User>())).ThrowsAsync(exception);
            var response = await service.GetContactDetail(user, string.Empty);
            Assert.NotNull(response);
            Assert.Single(response);
            var item = response[0];
            Assert.Equal(message, item.Message);
        }

        [Fact]
        public async Task ProfileCanChangeContactAddress()
        {
            var provider = GetTestArtifacts();
            var service = GetInfrastructure();
            var user = provider.GetRequiredService<User>();
            var change = GetResponse("Address");
            var changerequest = JsonConvert.DeserializeObject<ChangeContactAddressRequest[]>(change.Data);
            Assert.NotNull(changerequest);

            var actual = await service.ChangeContactAddress(user, changerequest);
            Assert.True(actual.Key);
        }

        [Fact]
        public async Task ProfileCanChangeContactEmail()
        {
            var provider = GetTestArtifacts();
            var service = GetInfrastructure();
            var user = provider.GetRequiredService<User>();
            var change = GetResponse("Email");
            var changerequest = JsonConvert.DeserializeObject<ChangeContactEmailRequest[]>(change.Data);
            Assert.NotNull(changerequest);

            var actual = await service.ChangeContactEmail(user, changerequest);
            Assert.True(actual.Key);
        }

        [Fact]
        public async Task ProfileCanChangeContactPhone()
        {
            var provider = GetTestArtifacts();
            var service = GetInfrastructure();
            var user = provider.GetRequiredService<User>();
            var change = GetResponse("Phone");
            var changerequest = JsonConvert.DeserializeObject<ChangeContactPhoneRequest[]>(change.Data);
            Assert.NotNull(changerequest);

            var actual = await service.ChangeContactPhone(user, changerequest);
            Assert.True(actual.Key);
        }

        [Fact]
        public async Task ProfileCanChangeContactName()
        {
            var provider = GetTestArtifacts();
            var service = GetInfrastructure();
            var user = provider.GetRequiredService<User>();
            var change = GetResponse("Name");
            var changerequest = JsonConvert.DeserializeObject<ChangeContactNameRequest[]>(change.Data);
            Assert.NotNull(changerequest);

            var actual = await service.ChangeContactName(user, changerequest);
            Assert.True(actual.Key);
        }

        private static GetContactResponse GetResponse(string responseType)
        {
            var profile = GetProfile();
            var mapper = ModelMapper.Mapper;
            var response = mapper.Map<GetContactResponse[]>(profile).ToList();
            var change = response.Find(x => x.ResponseType.Equals(responseType));
            return change ?? new();
        }

        private static UserPermissionView[] GetPermissions(string levelName)
        {
            var fieldNames = new[]
            {
                "Account.Permission.Level",
                "Account.IsPrimary",
                "Account.Linked.Accounts",
                "Setting.State.County.Subscriptions",
                "Setting.State.County.Subscriptions.Active",
                "Setting.State.Subscriptions",
                "Setting.State.Subscriptions.Active",
                "User.State.County.Discount",
                "User.State.Discount"
            };
            var list = permissionFaker.Generate(fieldNames.Length);
            list.ForEach(a =>
            {
                var indx = list.IndexOf(a);
                a.KeyName = fieldNames[indx];
                if (indx == 0) { a.KeyValue = levelName; }
            });
            return list.ToArray();
        }

        private static UserProfileView[] GetProfile()
        {
            var fieldNames = new[]
            {
                "First Name",
                "Last Name",
                "Company Name",
                "Phone 1",
                "Phone 2",
                "Phone 3",
                "Email 1",
                "Email 2",
                "Email 3",
                "Address 1 - Address Line 1",
                "Address 1 - Address Line 2",
                "Address 1 - Address Line 3",
                "Address 1 - Address Line 4",
                "Address 2 - Address Line 1",
                "Address 2 - Address Line 2",
                "Address 2 - Address Line 3",
                "Address 2 - Address Line 4",
            };
            var list = profileFaker.Generate(fieldNames.Length);
            list.ForEach(a => a.KeyName = fieldNames[list.IndexOf(a)]);
            return list.ToArray();
        }

        private static IProfileInfrastructure GetInfrastructure()
        {
            var provider = GetTestArtifacts();
            return provider.GetRequiredService<IProfileInfrastructure>();
        }

        private static IServiceProvider GetTestArtifacts()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton(m => new Mock<IDataProvider>());
            collection.AddSingleton(m => new Mock<IUserProfileRepository>());
            collection.AddSingleton(m => new Mock<IUserProfileViewRepository>());
            collection.AddSingleton(m => new Mock<IUserProfileHistoryRepository>());
            collection.AddSingleton(m => new Mock<IUserPermissionViewRepository>());
            collection.AddSingleton(m => m.GetRequiredService<Mock<IDataProvider>>().Object);
            collection.AddSingleton(m => m.GetRequiredService<Mock<IUserProfileRepository>>().Object);
            collection.AddSingleton(m => m.GetRequiredService<Mock<IUserProfileViewRepository>>().Object);
            collection.AddSingleton(m => m.GetRequiredService<Mock<IUserProfileHistoryRepository>>().Object);
            collection.AddSingleton(m => m.GetRequiredService<Mock<IUserPermissionViewRepository>>().Object);
            collection.AddSingleton(m => GetProfile());
            collection.AddSingleton(m => userFaker.Generate());
            collection.AddSingleton<IProfileInfrastructure>(m =>
            {
                var db = m.GetRequiredService<IDataProvider>();
                return new ProfileInfrastructure(db);
            });
            var provider = collection.BuildServiceProvider();
            var dataProvider = provider.GetRequiredService<Mock<IDataProvider>>();
            var userProfileVw = provider.GetRequiredService<IUserProfileViewRepository>();
            var profileHistoryDb = provider.GetRequiredService<IUserProfileHistoryRepository>();
            var userProfileDb = provider.GetRequiredService<IUserProfileRepository>();
            var userProfileVwMock = provider.GetRequiredService<Mock<IUserProfileViewRepository>>();
            var userPermissionVwMock = provider.GetRequiredService<Mock<IUserPermissionViewRepository>>();
            var profile = provider.GetRequiredService<UserProfileView[]>();
            userProfileVwMock.Setup(m => m.GetAll(It.IsAny<User>())).ReturnsAsync(profile);
            dataProvider.SetupGet(m => m.UserProfileVw).Returns(userProfileVw);
            dataProvider.SetupGet(m => m.ProfileHistoryDb).Returns(profileHistoryDb);
            dataProvider.SetupGet(m => m.UserProfileDb).Returns(userProfileDb);
            dataProvider.SetupGet(m => m.UserPermissionVw).Returns(userPermissionVwMock.Object);
            return provider;
        }
    }
}