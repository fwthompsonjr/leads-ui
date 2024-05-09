using legallead.email.utility;

namespace legallead.email.tests.utility
{
    public class StringExtensionsTests
    {
        private static readonly Faker faker = new();

        [Theory]
        [InlineData(10, 150)]
        [InlineData(10, 200)]
        [InlineData(20, 400)]
        public void SutCanSplitByLength(int sentences, int length)
        {
            const int mx = 500; // process must ensure string length less than 500
            var phrase = faker.Lorem.Sentences(sentences);
            var actual = phrase.SplitByLength(length).ToList();
            Assert.NotNull(actual);
            var serial = JsonConvert.SerializeObject(actual).RemoveLineEndings();
            Assert.False(string.IsNullOrEmpty(serial));
            var violations = actual.Exists(c => c.Length > mx);
            Assert.NotEmpty(actual);
            Assert.False(violations);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)]
        [InlineData(100)]
        public void SutCanRemoveLineEndingsForWhiteSpace(int count)
        {
            if (count == 0)
            {
                var test = string.Empty;
                Assert.Equal(test, test.RemoveLineEndings());
                return;
            }
            var empty = string.Concat(Enumerable.Repeat(" ", count));
            Assert.Equal(empty, empty.RemoveLineEndings());
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(13)]
        public void SutCanRemoveLineEndingsCarriageReturn(int count)
        {
            const string period = ".";
            const string doublepipe = "||";
            var hardreturn = string.Concat(Enumerable.Repeat(Environment.NewLine, count));
            var phrase = new Faker().Lorem.Paragraphs(4);
            var replace = $".{hardreturn}";
            phrase = phrase.Replace(period, replace);
            var actual = phrase.RemoveLineEndings();
            Assert.DoesNotContain(doublepipe, actual);
            Assert.Contains("|", actual);
        }
    }
}
