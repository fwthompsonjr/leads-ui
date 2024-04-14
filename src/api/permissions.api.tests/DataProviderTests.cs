using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace permissions.api.tests
{
    public class DataProviderTests
    {
        private static readonly List<string> CommonSettings = new List<string>()
            {
                "Account.IsPrimary",
                "Account.Permission.Level",
                "Setting.MaxRecords.Per.Year",
                "Setting.MaxRecords.Per.Month",
                "Setting.MaxRecords.Per.Request",
                "Setting.Pricing.Name",
                "Setting.Pricing.Per.Year",
                "Setting.Pricing.Per.Month",
                "Setting.Pricing.Per.Request"
            };
        private static readonly Faker<User> userfaker = new Faker<User>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        private static readonly Faker<ProfileMap> profilefaker = new Faker<ProfileMap>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.KeyName, y => y.Lorem.Word())
            .RuleFor(x => x.OrderId, y => y.IndexFaker);

        private static readonly Faker<PermissionGroup> permissionGroupFaker = new Faker<PermissionGroup>()
            .RuleFor(x => x.Name, y => y.PickRandom(CommonSettings))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.GroupId, y => y.IndexFaker)
            .RuleFor(x => x.PerRequest, y => y.Random.Int(5, 10))
            .RuleFor(x => x.PerMonth, y => y.Random.Int(20, 50))
            .RuleFor(x => x.PerYear, y => y.Random.Int(100, 500))
            .RuleFor(x => x.IsActive, y => true)
            .RuleFor(x => x.IsVisible, y => true)
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());



        [Fact]
        public void PermissionGroupCanGenerate()
        {
            var exception = Record.Exception(() =>
            {
                _ = GetPermissions();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserCanGenerate()
        {
            var exception = Record.Exception(() =>
            {
                _ = userfaker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ProfileCanGenerate()
        {
            var exception = Record.Exception(() =>
            {
                _ = profilefaker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ServiceCanGetDataProvider()
        {
            var provider = GetProvider();
            var dp = provider.GetRequiredService<DataProvider>();
            Assert.NotNull(dp);
        }

        [Fact]
        public void ServiceCanGetDataProviderProperties()
        {
            var provider = GetProvider();
            var dp = provider.GetRequiredService<DataProvider>();
            Assert.NotNull(dp.ComponentDb);
            Assert.NotNull(dp.PermissionDb);
            Assert.NotNull(dp.ProfileDb);
            Assert.NotNull(dp.UserPermissionDb);
            Assert.NotNull(dp.UserTokenDb);
            Assert.NotNull(dp.PermissionGroupDb);
            Assert.NotNull(dp.UserPermissionVw);
            Assert.NotNull(dp.UserProfileDb);
            Assert.NotNull(dp.UserProfileVw);
            Assert.NotNull(dp.PermissionHistoryDb);
            Assert.NotNull(dp.ProfileHistoryDb);
            Assert.NotNull(dp.UserDb);
        }


        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public async Task ServiceCanInitializeProfile(
            bool hasProfiles,
            bool hasProfileMatched
        )
        {
            var fkr = new Faker();
            var provider = GetProvider();
            var user = userfaker.Generate();
            var profiles = hasProfiles ? profilefaker.Generate(6) : new();
            var existing = hasProfileMatched && profiles.Any() ? fkr.PickRandom(profiles, 3) : new List<ProfileMap>();
            var userprofiles = existing.Select(p => new UserProfile
            {
                Id = Guid.NewGuid().ToString("D"),
                ProfileMapId = p.Id ?? string.Empty,
                UserId = user.Id ?? string.Empty,
                KeyValue = string.Empty
            });
            var profileDb = provider.GetRequiredService<Mock<IProfileMapRepository>>();
            var userProfileDb = provider.GetRequiredService<Mock<IUserProfileRepository>>();
            var dp = provider.GetRequiredService<DataProvider>();
            profileDb.Setup(s => s.GetAll()).ReturnsAsync(profiles);
            userProfileDb.Setup(s => s.GetAll(It.IsAny<User>())).ReturnsAsync(userprofiles);
            var response = await dp.InitializeProfile(user);
            Assert.True(response);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public async Task ServiceCanInitializePermission(
            bool hasProfiles,
            bool hasProfileMatched
        )
        {
            var fkr = new Faker();
            var provider = GetProvider();
            var user = userfaker.Generate();
            var permissions = hasProfiles ? profilefaker.Generate(6) : new();
            var existing = hasProfileMatched && permissions.Any() ? fkr.PickRandom(permissions, 3) : new List<ProfileMap>();
            var userpermissions = existing.Select(p => new UserPermission
            {
                Id = Guid.NewGuid().ToString("D"),
                PermissionMapId = p.Id ?? string.Empty,
                UserId = user.Id ?? string.Empty,
                KeyValue = string.Empty
            });
            var permissionsDb = provider.GetRequiredService<Mock<IPermissionMapRepository>>();
            var userPermissionsDb = provider.GetRequiredService<Mock<IUserPermissionRepository>>();
            var dp = provider.GetRequiredService<DataProvider>();
            permissionsDb.Setup(s => s.GetAll()).ReturnsAsync(permissions);
            userPermissionsDb.Setup(s => s.GetAll(It.IsAny<User>())).ReturnsAsync(userpermissions);
            var response = await dp.InitializePermission(user);
            Assert.True(response);
        }

        [Theory]
        [InlineData("Account.IsPrimary", true, true)]
        [InlineData("Setting.Pricing.Per.Year", true, true)]
        public async Task ServiceCanSetPermissionGroup(
            string groupName,
            bool hasGroups,
            bool hasPermissions
        )
        {
            var provider = GetProvider();
            var user = userfaker.Generate();
            var permissions = hasGroups ? GetPermissions() : new();
            var existing = hasPermissions && permissions.Any() ? GetPermissionsView() : new List<UserPermissionView>();
            var groupDb = provider.GetRequiredService<Mock<IPermissionGroupRepository>>();
            var userViewDb = provider.GetRequiredService<Mock<IUserPermissionViewRepository>>();
            var dp = provider.GetRequiredService<DataProvider>();
            groupDb.Setup(s => s.GetAll()).ReturnsAsync(permissions);
            userViewDb.Setup(s => s.GetAll(It.IsAny<User>())).ReturnsAsync(existing);
            var response = await dp.SetPermissionGroup(user, groupName);
            Assert.True(response.Key);
        }
        private static List<PermissionGroup> GetPermissions()
        {
            var list = permissionGroupFaker.Generate(9);
            list.ForEach(l =>
            {
                var id = list.IndexOf(l);
                l.Name = CommonSettings[id];
                l.OrderId = id;
            });
            return list;
        }

        private static List<UserPermissionView> GetPermissionsView()
        {
            return GetPermissions().Select(s =>
            {
                var js = JsonConvert.SerializeObject(s);
                var obj = JsonConvert.DeserializeObject<UserPermissionView>(js) ?? new();
                obj.KeyName = s.Name;
                return obj;
            }).ToList();
        }
        private static IServiceProvider GetProvider()
        {
            var services = new ServiceCollection();

            var component = new Mock<IComponentRepository>();
            var permissionDb = new Mock<IPermissionMapRepository>();
            var profileDb = new Mock<IProfileMapRepository>();
            var userPermissionDb = new Mock<IUserPermissionRepository>();
            var userProfileDb = new Mock<IUserProfileRepository>();
            var userTokenDb = new Mock<IUserTokenRepository>();
            var userPermissionVw = new Mock<IUserPermissionViewRepository>();
            var userProfileVw = new Mock<IUserProfileViewRepository>();
            var permissionGroupDb = new Mock<IPermissionGroupRepository>();
            var userDb = new Mock<IUserRepository>();
            var permissionHistoryDb = new Mock<IUserPermissionHistoryRepository>();
            var profileHistoryDb = new Mock<IUserProfileHistoryRepository>();


            services.AddSingleton(component);
            services.AddSingleton(component.Object);
            services.AddSingleton(permissionDb);
            services.AddSingleton(permissionDb.Object);
            services.AddSingleton(profileDb);
            services.AddSingleton(profileDb.Object);
            services.AddSingleton(userPermissionDb);
            services.AddSingleton(userPermissionDb.Object);
            services.AddSingleton(userProfileDb);
            services.AddSingleton(userProfileDb.Object);
            services.AddSingleton(userTokenDb);
            services.AddSingleton(userTokenDb.Object);
            services.AddSingleton(userPermissionVw);
            services.AddSingleton(userPermissionVw.Object);
            services.AddSingleton(userProfileVw);
            services.AddSingleton(userProfileVw.Object);
            services.AddSingleton(permissionGroupDb);
            services.AddSingleton(permissionGroupDb.Object);
            services.AddSingleton(userDb);
            services.AddSingleton(userDb.Object);
            services.AddSingleton(permissionHistoryDb);
            services.AddSingleton(permissionHistoryDb.Object);
            services.AddSingleton(profileHistoryDb);
            services.AddSingleton(profileHistoryDb.Object);
            services.AddSingleton(p =>
            {
                var a = p.GetRequiredService<IComponentRepository>();
                var b = p.GetRequiredService<IPermissionMapRepository>();
                var c = p.GetRequiredService<IProfileMapRepository>();
                var d = p.GetRequiredService<IUserPermissionRepository>();
                var e = p.GetRequiredService<IUserProfileRepository>();
                var f = p.GetRequiredService<IUserTokenRepository>();
                var g = p.GetRequiredService<IUserPermissionViewRepository>();
                var h = p.GetRequiredService<IUserProfileViewRepository>();
                var i = p.GetRequiredService<IPermissionGroupRepository>();
                var j = p.GetRequiredService<IUserRepository>();
                var k = p.GetRequiredService<IUserPermissionHistoryRepository>();
                var l = p.GetRequiredService<IUserProfileHistoryRepository>();
                return new DataProvider(a, b, c, d, e, f, g, h, i, j, k, l);
            });

            return services.BuildServiceProvider();
        }
    }
}
