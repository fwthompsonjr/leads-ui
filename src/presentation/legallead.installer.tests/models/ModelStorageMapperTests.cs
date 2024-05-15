using legallead.installer.Interfaces;
using legallead.installer.Models;

namespace legallead.installer.tests.models
{
    public class ModelStorageMapperTests
    {
        [Theory]
        [InlineData(typeof(ModelStorageMapper), false)]
        [InlineData(typeof(ReleaseModelStorage), true)]
        [InlineData(typeof(RepositoryStorage), true)]
        public void MapperDefinesItem(Type type, bool expected)
        {
            var actual = ModelStorageMapper.Translators.TryGetValue(type, out var _);
            Assert.Equal(expected, actual);
        }
    }
}
