using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;

namespace legallead.jdbc.tests.implementations
{
    public class UserUsageRepositoryTests
    {

        [Fact]
        public void RepoCanBeConstructed()
        {
            var provider = new RepoContainer();
            var repo = provider.Repository;
            Assert.NotNull(repo);
        }


        private sealed class RepoContainer
        {
            private readonly IUserUsageRepository repo;
            private readonly Mock<IDapperCommand> command;
            public RepoContainer()
            {

                command = new Mock<IDapperCommand>();
                var dataContext = new DataContext(command.Object);
                repo = new UserUsageRepository(dataContext);
            }

            public IUserUsageRepository Repository => repo;
        }
    }
}