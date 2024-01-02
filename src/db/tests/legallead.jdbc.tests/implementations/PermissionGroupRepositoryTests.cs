using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;

namespace legallead.jdbc.tests.implementations
{
    public class PermissionGroupRepositoryTests
    {
        private readonly DataContext dataContext;

        public PermissionGroupRepositoryTests()
        {
            dataContext = new DataContext(new Mock<IDapperCommand>().Object);
        }

        [Fact]
        public void RepoCanConstruct()
        {
            var repo = new PermissionGroupRepository(dataContext);
            Assert.NotNull(repo);
        }
    }
}