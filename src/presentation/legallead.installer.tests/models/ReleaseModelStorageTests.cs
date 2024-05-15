using legallead.installer.Models;

namespace legallead.installer.tests.models
{
    public class ReleaseModelStorageTests
    {
        [Fact]
        public void ItemCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                var obj = new ReleaseModelStorage();
                Assert.False(string.IsNullOrEmpty(obj.Name));
            });
            Assert.Null(exception);
        }
    }
}
