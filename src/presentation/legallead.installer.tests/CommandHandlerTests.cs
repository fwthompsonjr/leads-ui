using Bogus;
using legallead.installer.Commands;
using legallead.installer.Interfaces;
using legallead.installer.Models;
using Moq;
using Xunit;

namespace legallead.installer.tests
{
    public class CommandHandlerTests
    {
        private static Faker<ReleaseModel> modelFaker =
            new Faker<ReleaseModel>()
            .RuleFor(x => x.Name, y => y.System.Version().ToString())
            .RuleFor(x => x.Id, y => y.Random.Long(1, 1000))
            .RuleFor(x => x.RepositoryId, y => y.Random.Long(1, 1000))
            .RuleFor(x => x.PublishDate, y => y.Date.Recent());

        [Fact]
        public void HandlerCanBeConstructed()
        {
            var mock = GetMock();
            var handler = new CommandHandler(mock.Object);
            Assert.NotNull(handler);
        }

        [Fact]
        public void HandlerCanExecuteRootCommand()
        {
            var exception = Record.Exception(() => {
                var mock = GetMock();
                var handler = new CommandHandler(mock.Object);
                handler.WhoAmI();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void HandlerCanExecuteVersionCommand()
        {
            var exception = Record.Exception(() => {
                var mock = GetMock();
                var handler = new CommandHandler(mock.Object);
                handler.Version();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(5)]
        public async Task HandlerCanExecuteListCommand(int? recordCount)
        {
            var records = (recordCount) switch
            {

                null => default,
                0 => [],
                _ => modelFaker.Generate(recordCount.GetValueOrDefault())
            };

            var exception = await Record.ExceptionAsync(async () => {
                var mock = GetMock();
                mock.Setup(m => m.GetReleases()).ReturnsAsync(records);
                var handler = new CommandHandler(mock.Object);
                await handler.List();
            });
            Assert.Null(exception);
        }

        private static Mock<IGitReader> GetMock()
        {
            return new Mock<IGitReader>();
        }
    }
}
