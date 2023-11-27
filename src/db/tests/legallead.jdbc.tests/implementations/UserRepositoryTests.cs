using Bogus;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;
using System.Data;

namespace legallead.jdbc.tests.implementations
{
    public class UserRepositoryTests
    {
        private static readonly Faker<User> faker =
            new Faker<User>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserName, y => y.Company.CompanyName())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.PasswordHash, y => y.Random.AlphaNumeric(60))
            .RuleFor(x => x.PasswordSalt, y => y.Random.AlphaNumeric(30));

        private readonly DataContext dataContext;

        public UserRepositoryTests()
        {
            dataContext = new DataContext(new Mock<IDapperCommand>().Object);
        }

        [Fact]
        public void RepoCanConstruct()
        {
            var repo = new UserRepository(dataContext);
            Assert.NotNull(repo);
        }

        [Fact]
        public async Task RepoCanGetAll()
        {
            var repo = new UserRepository(dataContext);
            var response = await repo.GetAll();
            Assert.NotNull(response);
        }

        [Fact]
        public async Task RepoCanGetById()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var test = faker.Generate();
                var repo = new UserRepository(dataContext);
                _ = await repo.GetById(test.Id);
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task RepoCanGetByEmail()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var test = faker.Generate();
                var repo = new UserRepository(dataContext);
                _ = await repo.GetByEmail(test.Email);
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task RepoCanGetByName()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var test = faker.Generate();
                var repo = new UserRepository(dataContext);
                _ = await repo.GetByName(test.UserName);
            });
            Assert.Null(exception);
        }

        [Fact]
        public async Task RepoCanCreate()
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                var test = faker.Generate();
                var repo = new UserRepository(GetDbMock().Object);
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
                var repo = new UserRepository(GetDbMock().Object);
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
                var repo = new UserRepository(GetDbMock().Object);
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