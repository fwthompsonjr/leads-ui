using Bogus;
using legallead.jdbc.entities;
using Newtonsoft.Json;

namespace legallead.jdbc.tests.entities
{
    public class UserLockStatusDtoTests
    {

        private static readonly Faker<UserLockStatusDto> faker =
            new Faker<UserLockStatusDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.IsLocked, y => y.Random.Bool())
            .RuleFor(x => x.FailedAttemptCount, y => y.Random.Int(1, 14))
            .RuleFor(x => x.LastFailedAttemptDt, y => y.Date.Recent())
            .RuleFor(x => x.FailedAttemptResetDt, y => y.Date.Recent())
            .RuleFor(x => x.MaxFailedAttempts, y => y.Random.Int(15, 25))
            .RuleFor(x => x.CanResetAccount, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        private static readonly Faker<UserLockStatusBo> bofaker =
            new Faker<UserLockStatusBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.IsLocked, y => y.Random.Bool())
            .RuleFor(x => x.FailedAttemptCount, y => y.Random.Int(1, 14))
            .RuleFor(x => x.LastFailedAttemptDt, y => y.Date.Recent())
            .RuleFor(x => x.FailedAttemptResetDt, y => y.Date.Recent())
            .RuleFor(x => x.MaxFailedAttempts, y => y.Random.Int(15, 25))
            .RuleFor(x => x.CanResetAccount, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void UserLockStatusDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserLockStatusDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserLockStatusDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserLockStatusBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserLockStatusBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserLockStatusBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = bofaker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserLockStatusDtoCanDeserialize()
        {
            var source = faker.Generate();
            var json = JsonConvert.SerializeObject(source);
            var dest = JsonConvert.DeserializeObject<UserLockStatusBo>(json);
            Assert.NotNull(dest);
            Assert.Equal(source.Id, dest.Id);
            Assert.Equal(source.UserId, dest.UserId);
            Assert.Equal(source.IsLocked, dest.IsLocked);
            Assert.Equal(source.FailedAttemptCount, dest.FailedAttemptCount);
            Assert.Equal(source.LastFailedAttemptDt, dest.LastFailedAttemptDt);
            Assert.Equal(source.FailedAttemptResetDt, dest.FailedAttemptResetDt);
            Assert.Equal(source.MaxFailedAttempts, dest.MaxFailedAttempts);
            Assert.Equal(source.CanResetAccount, dest.CanResetAccount);
            Assert.Equal(source.CreateDate, dest.CreateDate);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("IsLocked")]
        [InlineData("FailedAttemptCount")]
        [InlineData("LastFailedAttemptDt")]
        [InlineData("FailedAttemptResetDt")]
        [InlineData("MaxFailedAttempts")]
        [InlineData("CanResetAccount")]
        [InlineData("CreateDate")]
        public void UserLockStatusDtoHasExpectedFieldDefined(string name)
        {
            var sut = new UserLockStatusDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserId")]
        [InlineData("IsLocked")]
        [InlineData("FailedAttemptCount")]
        [InlineData("LastFailedAttemptDt")]
        [InlineData("FailedAttemptResetDt")]
        [InlineData("MaxFailedAttempts")]
        [InlineData("CanResetAccount")]
        [InlineData("CreateDate")]
        public void UserLockStatusDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new UserLockStatusDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}