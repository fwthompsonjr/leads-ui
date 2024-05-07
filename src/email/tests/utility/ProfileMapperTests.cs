using legallead.email.models;
using legallead.email.utility;

namespace legallead.email.tests.utility
{
    public class ProfileMapperTests
    {
        [Theory]
        [InlineData("Address")]
        [InlineData("Email")]
        [InlineData("Name")]
        [InlineData("Phone")]
        [InlineData("NotMapped")]
        public void MapperCanMapObject(string changeType)
        {
            var data = ProfileMockInfrastructure.GetResult(200, changeType);
            var converted = ProfileMapper.Mapper.Map<ProfileChangedModel>(data);
            Assert.NotNull(converted);
            var expected = !changeType.Equals("NotMapped");
            Assert.Equal(expected, converted.IsValid);
        }
    }
}
