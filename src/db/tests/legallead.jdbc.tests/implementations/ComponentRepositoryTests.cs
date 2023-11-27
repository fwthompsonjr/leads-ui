using Bogus;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System.Data;
using System.Net.Http.Headers;

namespace legallead.jdbc.tests.implementations
{
    public class ComponentRepositoryTests
    {
        private static readonly Faker<Component> faker =
            new Faker<Component>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Name, y => y.Company.CompanyName());

        private readonly DataContext dataContext;

        public ComponentRepositoryTests()
        {
            dataContext = new DataContext(new Mock<IDapperCommand>().Object);
        }

        [Fact]
        public void RepoCanConstruct()
        {
            var repo = new ComponentRepository(dataContext);
            Assert.NotNull(repo);
        }

        [Fact]
        public async Task RepoCanGetAll()
        {
            var repo = new ComponentRepository(dataContext);
            var response = await repo.GetAll();
            Assert.NotNull(response);
        }

        [Fact]
        public async Task RepoCanGetById()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var test = faker.Generate();
                var repo = new ComponentRepository(dataContext);
                _ = await repo.GetById(test.Id);
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task RepoCanGetByName()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var test = faker.Generate();
                var repo = new ComponentRepository(dataContext);
                _ = await repo.GetByName(test.Name);
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task RepoCanCreate()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var test = faker.Generate();
                var repo = new ComponentRepository(GetDbMock().Object);
                await repo.Create(test);
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task RepoCanUpdate()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var test = faker.Generate();
                var repo = new ComponentRepository(GetDbMock().Object);
                await repo.Update(test);
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task RepoCanDelete()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var test = faker.Generate();
                var repo = new ComponentRepository(GetDbMock().Object);
                await repo.Delete(test.Id);
            });
            Assert.Null(exception);
        }

        private static Mock<DataContext> GetDbMock()
        {
            var mqDb = new Mock<IDapperCommand>();
            var mqContext = new Mock<DataContext>(mqDb.Object);
            var mqConnect = new Mock<IDbConnection>();
            mqContext.Setup(m => m.CreateConnection()).Returns(mqConnect.Object);
            mqContext.SetupGet(m => m.GetCommand).Returns(mqDb.Object);
            return mqContext;
        }
    }
}