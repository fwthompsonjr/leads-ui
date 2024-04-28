using legallead.email.implementations;
using legallead.email.models;

namespace legallead.email.tests.implementations
{
    public class AccountRegistrationTemplateTests
    {
        [Fact]
        public void SutCanBeCreated()
        {
            var exception = Record.Exception(() => { _ = new AccountRegistrationTemplate(); });
            Assert.Null(exception);
        }

        [Fact]
        public void SutHasBaseHtml()
        {
            var exception = Record.Exception(() =>
            {
                var service = new AccountRegistrationTemplate();
                var html = service.BaseHtml;
                Assert.False(string.IsNullOrWhiteSpace(html));
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SutHasKeyNames()
        {
            var exception = Record.Exception(() =>
            {
                var service = new AccountRegistrationTemplate();
                var names = service.KeyNames;
                Assert.NotNull(names);
                Assert.Equal(2, names.Count);
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
                var service = new AccountRegistrationTemplate();
                service.FetchTemplateParameters(list);
            });
            Assert.Null(exception);
        }
    }
}
