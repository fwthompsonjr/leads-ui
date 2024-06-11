using Bogus;
using Dapper;
using legallead.jdbc.helpers;
using System.Text;

namespace legallead.jdbc.tests.helpers
{
    public class BoMapperTests
    {
        [Theory]
        [InlineData(0, "user_index")]
        [InlineData(1, "user_index,message_id")]
        [InlineData(2, "user_index,last_created_date")]
        public void MapperCanGetParameters(int index, string parmNames)
        {
            var source = MockEmailObjectProvider.ListBoFaker.Generate();
            var parms = index switch
            {
                0 => BoMapper.GetCountParameters(source.UserId),
                1 => BoMapper.GetBodyParameters(source.Id, source.UserId),
                2 => BoMapper.GetMessagesParameters(source.UserId, source.CreateDate),
                _ => new DynamicParameters()
            };
            var expected = parmNames.Split(',').ToList();
            var names = parms.ParameterNames.ToList();
            Assert.Equal(expected.Count, names.Count);
            expected.ForEach(a =>
            {
                Assert.Contains(a, names);
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void MapperCanMapEmailBody(int index)
        {
            var source = index switch
            {
                1 => MockEmailObjectProvider.BodyDtoFaker.Generate(),
                _ => null
            };
            var dest = BoMapper.Map(source);
            if (index == 0) Assert.Null(dest);
            else Assert.NotNull(dest);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void MapperCanMapEmailCount(int index)
        {
            var source = index switch
            {
                1 => MockEmailObjectProvider.CountDtoFaker.Generate(),
                _ => null
            };
            var dest = BoMapper.Map(source);
            if (index == 0) Assert.Null(dest);
            else Assert.NotNull(dest);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public void MapperCanMapEmailList(int index)
        {
            var source = index switch
            {
                0 => null,
                _ => MockEmailObjectProvider.ListDtoFaker.Generate(index)
            };
            var dest = BoMapper.Map(source);
            if (index == 0) Assert.Null(dest);
            else Assert.NotNull(dest);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void MapperCanConvertFromBase64(int index)
        {
            var faker = new Faker();
            var original = faker.Lorem.Paragraph();
            string? expected = index switch
            {
                0 => null,
                1 => "",
                2 => "    ",
                3 => original,
                _ => EncodeBase64(original)
            };
            var actual = BoMapper.FromBase64(expected);
            if (index <= 3)
            {
                Assert.Equal(expected, actual);
            }
            else
            {
                Assert.Equal(original, actual);
            }
        }

        private static string EncodeBase64(string source)
        {
            var result = Encoding.UTF8.GetBytes(source);
            return Convert.ToBase64String(result);
        }
    }
}
