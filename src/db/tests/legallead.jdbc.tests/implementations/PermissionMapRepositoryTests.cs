using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Moq;

namespace legallead.jdbc.tests.implementations
{
    public class PermissionMapRepositoryTests
    {
        [Fact]
        public void RepoCanConstruct()
        {
            var dataContext = new DataContext(new Mock<IDapperCommand>().Object);
            var repo = new PermissionMapRepository(dataContext);
            Assert.NotNull(repo);
        }
    }
}
