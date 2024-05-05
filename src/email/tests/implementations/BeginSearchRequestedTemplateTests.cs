using legallead.email.implementations;
using legallead.email.models;

namespace legallead.email.tests.implementations
{
    public class BeginSearchRequestedTemplateTests
    {
        [Fact]
        public void SutCanBeCreated()
        {
            var exception = Record.Exception(() => { _ = new BeginSearchRequestedTemplate(); });
            Assert.Null(exception);
        }

        [Fact]
        public void SutHasBaseHtml()
        {
            var exception = Record.Exception(() =>
            {
                var service = new BeginSearchRequestedTemplate();
                _ = service.BaseHtml;
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SutHasKeyNames()
        {
            var exception = Record.Exception(() =>
            {
                var service = new BeginSearchRequestedTemplate();
                var names = service.KeyNames;
                Assert.NotNull(names);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SutHasSubstitutions()
        {
            var exception = Record.Exception(() =>
            {
                var service = new BeginSearchRequestedTemplate();
                var names = service.Substitutions;
                Assert.NotNull(names);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("abc@temp.com", null)]
        [InlineData("abc@temp.com", "user-name")]
        public void SutCanFetchTemplateParameters(string? email, string? userName)
        {
            List<UserEmailSettingBo> list = [
                new() { Email = email, UserName = userName }
            ];
            var exception = Record.Exception(() =>
            {
                var service = new BeginSearchRequestedTemplate();
                service.FetchTemplateParameters(list);
            });
            Assert.Null(exception);
        }
    }
}