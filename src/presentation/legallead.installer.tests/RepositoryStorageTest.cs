using legallead.installer.Models;

namespace legallead.installer.tests
{
    public class RepositoryStorageTest
    {
        [Fact]
        public void SutCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                var item = new RepositoryStorage();
                Assert.False(string.IsNullOrEmpty(item.Name));
            });
            Assert.Null(exception);
        }
    }
}
