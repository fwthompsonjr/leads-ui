using legallead.email.models;
using legallead.email.transforms;

namespace legallead.email.tests.transforms
{
    public class HtmlTransformDetailBaseTests
    {
        [Fact]
        public void TestCanBeCreated()
        {
            var sut = new TestBaseTemplate();
            Assert.NotNull(sut);
        }

        [Fact]
        public void TestWillContainBaseHtml()
        {
            var sut = new TestBaseTemplate();
            var actual = sut.BaseHtml;
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void TestWillContainKeyNames()
        {
            var sut = new TestBaseTemplate();
            var actual = sut.KeyNames;
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void TestWillContainTemplateName()
        {
            var sut = new TestBaseTemplate();
            var actual = sut.TemplateName;
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void TestWillContainSubstitutions()
        {
            var sut = new TestBaseTemplate();
            var expected = sut.KeyNames.Count;
            var actual = sut.Substitutions;
            Assert.Equal(expected, actual.Count);
        }


        [Fact]
        public void TestFetchTemplateParametersShoultThrowExcpetion()
        {
            var exception = Record.Exception(() =>
            {
                var sut = new TestBaseTemplate();
                sut.FetchTemplateParameters([]);
            });
            Assert.Null(exception);
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(true, false)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true, false, null, "abc")]
        [InlineData(true, true, true, false, "", "abc")]
        [InlineData(true, true, true, false, "   ", "abc")]
        [InlineData(true, true, true, false, "Person", null)]
        [InlineData(true, true, true, false, "Person", "")]
        [InlineData(true, true, true, false, "Person", "   ")]
        public void TestCanGetHtmlTemplate(
            bool hasSettings,
            bool hasPerson = true,
            bool hasKeyNames = true,
            bool hasPersonKey = true,
            string? personKeyName = "Person",
            string? personKeyValue = null)
        {
            var attributes = hasSettings ? GetDefaultSettings() : MockMessageInfrastructure.UserEmailFaker.Generate(0);
            if (!hasPerson) { attributes.RemoveAll(a => a.KeyName == "Person"); }
            if (!hasKeyNames && attributes.Count > 0)
            {
                var template = attributes[0];
                attributes.Add(new()
                {
                    Email = template.Email,
                    Id = template.Id,
                    UserName = template.UserName,
                    KeyValue = "12345"
                });
            }
            if (!hasPersonKey && attributes.Count > 0)
            {
                var person = attributes.Find(a => a.KeyName == "Person");
                if (person != null)
                {
                    person.KeyName = personKeyName;
                    person.KeyValue = personKeyValue;
                }

            }
            var sut = new TestBaseTemplate();
            var expected = sut.GetHtmlTemplate(attributes);
            Assert.False(string.IsNullOrWhiteSpace(expected));
        }

        private static List<UserEmailSettingBo> GetDefaultSettings()
        {
            return MockMessageInfrastructure.GetDefaultSettings();
        }
        private sealed class TestBaseTemplate : HtmlTransformDetailBase
        {
            public TestBaseTemplate()
            {
                _keynames.ForEach(key => { _substitions[key] = null; });
            }
            public override string BaseHtml => _htmlBody;

            public override List<string> KeyNames => _keynames;

            public override void FetchTemplateParameters(List<UserEmailSettingBo> attributes)
            {
            }

            private static readonly List<string> _html = [
                "<html>",
                "\t<body>",
                "\t\t<p>Hello, <!-- person --></p>",
                "\t</body>",
                "</html>"
            ];
            private static readonly string _htmlBody = string.Join(Environment.NewLine, _html);
            private static readonly List<string> _keynames = [
                "<!-- person -->"
            ];
        }
    }
}
