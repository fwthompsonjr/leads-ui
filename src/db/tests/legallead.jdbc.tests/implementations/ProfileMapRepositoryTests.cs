using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;

namespace legallead.jdbc.tests.implementations
{
    public class ProfileMapRepositoryTests
    {
        private readonly DataContext dataContext;

        public ProfileMapRepositoryTests()
        {
            dataContext = new DataContext(new Mock<IDapperCommand>().Object);
        }

        [Fact]
        public void RepoCanConstruct()
        {
            var repo = new ProfileMapRepository(dataContext);
            Assert.NotNull(repo);
        }
    }
}