using legallead.installer.Classes;

namespace legallead.installer.tests
{
    public class EnvironmentStorageKeyTests
    {
        [Fact]
        public void KeysExist()
        {
            var items = EnvironmentStorageKey.StorageKeys;
            Assert.NotEmpty(items);
            Assert.Equal(3, items.Count);
        }
    }
}
