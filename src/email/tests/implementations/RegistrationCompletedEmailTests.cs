using HtmlAgilityPack;
using legallead.email.implementations;
using legallead.email.models;
using legallead.email.services;
using legallead.email.utility;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.email.tests.implementations
{
    public class RegistrationCompletedEmailTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var obj = new RegistrationCompletedTemplate();
            Assert.NotNull(obj);
        }

        [Fact]
        public void ServiceCanSubstituteParameters()
        {
            var html = GetHtmlTemplate();
            Assert.False(string.IsNullOrWhiteSpace(html));
        }

        [Fact]
        public void ServiceCanGetSubstiteDocument()
        {
            var exception = Record.Exception(() =>
            {
                var doc = GetDocument();
                Assert.NotNull(doc);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("heading", "h3")]
        [InlineData("para-1", "p")]
        [InlineData("list-1", "ul")]
        [InlineData("list-item-1", "li")]
        [InlineData("email", "span")]
        [InlineData("list-item-2", "li")]
        [InlineData("user-name", "span")]
        public void ServiceCanGetSubstituteElements(string name, string tagName)
        {
            const string pattern = @"//{1}[@name='account-registration-{0}']";

            var query = string.Format(pattern, name, tagName);
            var exception = Record.Exception(() =>
            {
                var doc = GetDocument();
                var body = doc.DocumentNode.SelectSingleNode("//body");
                Assert.NotNull(body);
                var node = body.SelectSingleNode(query);
                Assert.NotNull(node);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("email", "<!-- Email Address -->")]
        [InlineData("user-name", "<!-- User Name -->")]
        public void ServiceCanGetTransformElements(string name, string original)
        {
            const string pattern = "//*[@name='account-registration-{0}']";

            var query = string.Format(pattern, name);
            var exception = Record.Exception(() =>
            {
                var doc = GetDocument();
                var node = doc.DocumentNode.SelectSingleNode(query);
                var txt = node?.InnerHtml ?? string.Empty;
                Assert.False(string.IsNullOrEmpty(txt));
                Assert.DoesNotContain(original, txt);
            });
            Assert.Null(exception);
        }
        [Fact]
        public void MailMessageWillContainBody()
        {
            var provider = MessageMockInfrastructure.GetServiceProvider();
            var service = provider.GetRequiredService<MailMessageService>();
            service.With(TemplateNames.RegistrationCompleted, Guid.NewGuid().ToString());
            var email = service.Message;
            Assert.NotNull(email);
            var body = email.Body;
            Assert.NotNull(body);
            Assert.False(string.IsNullOrEmpty(body));
        }

        [Theory]
        [InlineData("span-sub-heading", "<!-- SubHeading.Email.Title -->")]
        [InlineData("span-sub-heading-date", "<!-- SubHeading.Email.Date -->")]
        [InlineData("span-greeting", "<!-- Greeting.UserName -->")]
        [InlineData("account-registration-email", "<!-- Email Address -->")]
        [InlineData("account-registration-user-name", "<!-- User Name -->")]
        public void MailMessageWillTransformElements(string name, string original)
        {
            const string pattern = "//*[@name='{0}']";
            var query = string.Format(pattern, name);
            var exception = Record.Exception(() =>
            {
                var provider = MessageMockInfrastructure.GetServiceProvider();
                var service = provider.GetRequiredService<MailMessageService>();
                service.With(TemplateNames.RegistrationCompleted, Guid.NewGuid().ToString());
                var email = service.Message;
                Assert.NotNull(email);
                var body = email.Body;
                var doc = new HtmlDocument();
                doc.LoadHtml(body);
                var node = doc.DocumentNode.SelectSingleNode(query);
                Assert.NotNull(node);
                var txt = node.InnerHtml;
                Assert.DoesNotContain(original, txt);
            });
            Assert.Null(exception);
        }

        private static HtmlDocument GetDocument()
        {
            const string wrapper = "<html><body>{0}</body></html>";
            var text = GetHtmlTemplate();
            var html = string.Format(wrapper, text);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
        private static string GetHtmlTemplate()
        {
            var obj = new RegistrationCompletedTemplate();
            var details = GetDefaultSettings();
            return obj.GetHtmlTemplate(details) ?? string.Empty;
        }
        private static List<UserEmailSettingBo> GetDefaultSettings()
        {
            return MessageMockInfrastructure.GetDefaultSettings();
        }
    }
}
