using Bogus;
using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System.Data;

namespace legallead.jdbc.tests.implementations
{
    public class HolidayRepositoryTests
    {

        private static readonly Faker<HoliDateDto> faker
            = new Faker<HoliDateDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.HoliDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void RepoCanBeConstructed()
        {
            var provider = new RepoContainer();
            var repo = provider.Repository;
            Assert.NotNull(repo);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task RepoCanExecIsHolidayAsync(int recordcount)
        {
            var fkr = new Faker();
            var error = fkr.System.Exception();
            var request = fkr.Date.Past();
            var completion = recordcount switch
            {
                -1 => null,
                _ => faker.Generate()
            };
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            if (recordcount == 0)
            {
                mock.Setup(m => m.QuerySingleOrDefaultAsync<HoliDateDto>(
                    It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>()))
                        .ThrowsAsync(error);
            }
            else
            {
                mock.Setup(m => m.QuerySingleOrDefaultAsync<HoliDateDto>(
                    It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>()))
                        .ReturnsAsync(completion);
            }
            _ = await service.IsHolidayAsync(request);

            mock.Verify(m => m.QuerySingleOrDefaultAsync<HoliDateDto>(
                It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(50)]
        public async Task RepoCanGetHolidaysAsync(int recordcount)
        {
            var fkr = new Faker();
            var error = fkr.System.Exception();
            var completion = recordcount switch
            {
                -1 => null,
                -100 => faker.Generate(1),
                _ => faker.Generate(recordcount)
            };
            var provider = new RepoContainer();
            var mock = provider.CommandMock;
            var service = provider.Repository;
            if (recordcount == -100)
            {
                mock.Setup(m => m.QueryAsync<HoliDateDto>(
                    It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>()))
                        .ThrowsAsync(error);
            }
            else
            {
                mock.Setup(m => m.QueryAsync<HoliDateDto>(
                    It.IsAny<IDbConnection>(),
                        It.IsAny<string>(),
                        It.IsAny<DynamicParameters>()))
                        .ReturnsAsync(completion);
            }
            _ = await service.GetHolidaysAsync();

            mock.Verify(m => m.QueryAsync<HoliDateDto>(
                It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>()));
        }

        private sealed class RepoContainer
        {
            private readonly IHolidayRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {

                command = new Mock<IDapperCommand>();
                var dataContext = new DataContext(command.Object);
                repo = new HolidayRepository(dataContext);
            }

            public IHolidayRepository Repository => repo;
            public Mock<IDapperCommand> CommandMock => command;
        }
    }
}