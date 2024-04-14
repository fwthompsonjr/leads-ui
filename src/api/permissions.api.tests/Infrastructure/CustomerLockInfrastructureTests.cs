using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Utility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace permissions.api.tests.Infrastructure
{
    public class CustomerLockInfrastructureTests
    {
        [Theory]
        [InlineData(typeof(IUserLockStatusRepository))]
        [InlineData(typeof(Mock<IUserLockStatusRepository>))]
        [InlineData(typeof(ICustomerLockInfrastructure))]
        public void MockProviderCanGetTypes(Type type)
        {
            var provider = GetProvider();
            var service = provider.GetService(type);
            Assert.NotNull(service);
        }

        [Fact]
        public async Task ServiceCanAddIncident()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var service = provider.GetRequiredService<ICustomerLockInfrastructure>();
                await service.AddIncident("abcd");
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task ServiceCanCalculateIsAccountLocked(int id)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var provider = GetProvider();
                var statusResponse = LockAccountBoService.Get(id);
                var cleared = LockAccountBoService.Get(10);
                if (cleared != null)
                {
                    cleared.IsLocked = false;
                    cleared.LastFailedAttemptDt = DateTime.UtcNow.AddMonths(-1);
                    cleared.FailedAttemptResetDt = DateTime.UtcNow.AddDays(-1);
                    cleared.CanResetAccount = true;
                    cleared.FailedAttemptCount = 0;
                    cleared.MaxFailedAttempts = 5;
                }
                var lockDb = provider.GetRequiredService<Mock<IUserLockStatusRepository>>();

                lockDb.SetupSequence(s => s.GetStatus(It.IsAny<string>()))
                    .ReturnsAsync(statusResponse)
                    .ReturnsAsync(cleared);
                var service = provider.GetRequiredService<ICustomerLockInfrastructure>();
                _ = await service.IsAccountLocked("abcd");
            });
            Assert.Null(exception);
        }

        private static IServiceProvider GetProvider()
        {
            var services = new ServiceCollection();
            var lockDb = new Mock<IUserLockStatusRepository>();
            
            // add mocks
            services.AddSingleton(lockDb);
            services.AddSingleton(lockDb.Object);

            // expose default implementation
            services.AddSingleton<ICustomerLockInfrastructure, CustomerLockInfrastructure>();

            return services.BuildServiceProvider();
        }

        private static class LockAccountBoService
        {
            private static readonly Faker<UserLockStatusBo> faker
                = new Faker<UserLockStatusBo>()
                .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
                .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
                .RuleFor(x => x.IsLocked, y => y.Random.Bool())
                .RuleFor(x => x.FailedAttemptCount, y => y.Random.Int(0, 5))
                .RuleFor(x => x.LastFailedAttemptDt, y => y.Date.Recent())
                .RuleFor(x => x.FailedAttemptResetDt, y => y.Date.Recent())
                .RuleFor(x => x.MaxFailedAttempts, y => y.Random.Int(0, 5))
                .RuleFor(x => x.CanResetAccount, y => y.Random.Bool())
                .RuleFor(x => x.CreateDate, y => y.Date.Recent());

            public static UserLockStatusBo? Get(int id)
            {
                if (id == 0) return GetNull();
                if (id == 1) return GetUnLockedWithViolation();
                if (id == 2) return GetUnLockedWithoutViolation();
                if (id == 3) return GetLocked();
                if (id == 4) return GetUnLockedWithViolationNoReset();
                return faker.Generate();
            }

            private static UserLockStatusBo? GetNull()
            {
                return null;
            }
            private static UserLockStatusBo? GetUnLockedWithViolation()
            {
                var item = faker.Generate();
                item.IsLocked = false;
                item.MaxFailedAttempts = item.FailedAttemptCount.GetValueOrDefault() - 1;
                item.FailedAttemptResetDt = DateTime.UtcNow.AddMinutes(10);
                item.CanResetAccount = true;
                return item;
            }

            private static UserLockStatusBo? GetUnLockedWithViolationNoReset()
            {
                var item = faker.Generate();
                item.IsLocked = false;
                item.MaxFailedAttempts = item.FailedAttemptCount.GetValueOrDefault() - 1;
                item.FailedAttemptResetDt = DateTime.UtcNow.AddMinutes(10);
                item.CanResetAccount = false;
                return item;
            }
            private static UserLockStatusBo? GetUnLockedWithoutViolation()
            {
                var item = faker.Generate();
                item.IsLocked = false;
                item.MaxFailedAttempts = item.FailedAttemptCount.GetValueOrDefault() + 1;
                item.FailedAttemptResetDt = DateTime.UtcNow.AddMinutes(-1);
                return item;
            }

            private static UserLockStatusBo? GetLocked()
            {
                var item = faker.Generate();
                item.IsLocked = true;
                item.MaxFailedAttempts = item.FailedAttemptCount.GetValueOrDefault() + 1;
                item.FailedAttemptResetDt = DateTime.UtcNow.AddMinutes(-1);
                return item;
            }
        }
    }
}
/*


*/