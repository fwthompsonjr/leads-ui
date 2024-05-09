using HtmlAgilityPack;
using legallead.email.interfaces;
using legallead.email.models;
using legallead.email.utility;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace legallead.email.transforms
{
    public class HtmlTransformService : IHtmlTransformService
    {
        private readonly Dictionary<string, string?> _substitions;
        private readonly IUserSettingInfrastructure _userRepo;
        public HtmlTransformService(IUserSettingInfrastructure userData)
        {
            _userRepo = userData;
            _substitions = [];
            _keynames.ForEach(key =>
            {
                _substitions[key] = null;
            });
        }

        public string BaseHtml => HtmlBase;
        public List<string> KeyNames => _keynames;
        public Dictionary<string, string?> Substitutions => _substitions;


        public async Task<string?> GetHtmlTemplate(UserSettingQuery user, string templateName)
        {
            if (!user.IsValid) return null;
            var fallbackName = string.IsNullOrWhiteSpace(user.Email) ? "Account User" : user.Email;
            var settingBo = await _userRepo.GetSettings(user);
            if (settingBo == null || settingBo.Count == 0) return TransformHtml(fallbackName, templateName, settingBo);
            fallbackName = ExtractUserName(settingBo, fallbackName);
            return TransformHtml(fallbackName, templateName, settingBo);

        }

        private string TransformHtml(string userName, string templateName, List<UserEmailSettingBo>? attributes = null)
        {
            const string detailQuery = "//td[@name='body-line-details']";
            var detail = ExtractDetail(attributes, templateName, "");
            var html = new StringBuilder(BaseHtml);
            html.Replace(BodyHtmlToken, detail);
            html.Replace(EmailDateToken, DateTime.UtcNow.ToString("MMM d, yyyy"));
            html.Replace(EmailSubjectToken, Substitutions[EmailSubjectToken] ?? "Account Information");
            html.Replace(UserNameToken, Substitutions[UserNameToken] ?? userName);
            var content = html.ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var node = doc.DocumentNode.SelectSingleNode(detailQuery);
            if (node == null) return content;
            node.InnerHtml = detail;
            return doc.DocumentNode.OuterHtml;
        }

        private static string ExtractUserName(List<UserEmailSettingBo> attributes, string fallback)
        {
            if (attributes.Count == 0) return fallback;
            var userName = attributes.Where(x => !string.IsNullOrEmpty(x.UserName)).Select(x => x.UserName).FirstOrDefault() ?? fallback;
            var firstName = attributes.Find(x => (x.KeyName ?? "").Equals("First Name"))?.KeyValue;
            var lastName = attributes.Find(x => (x.KeyName ?? "").Equals("Last Name"))?.KeyValue;
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName)) return userName;
            if (!string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName)) return firstName;
            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName)) return $"{firstName} {lastName}";
            return userName;
        }
        [ExcludeFromCodeCoverage(Justification = "Private method with common use cases tested thru public members.")]
        private static string ExtractDetail(List<UserEmailSettingBo>? attributes, string templateName, string fallback)
        {
            if (attributes == null) return fallback;
            var named = Enum.TryParse<TemplateNames>(templateName, out var template);
            if (!named) return fallback;
            var instance = ServiceInfrastructure.Provider?.GetKeyedService<IHtmlTransformDetailBase>(template.ToString());
            if (instance == null) return fallback;
            fallback = instance.BaseHtml;
            var content = instance.GetHtmlTemplate(attributes) ?? fallback;
            return content;
        }
        private const string BodyHtmlToken = "<!-- Body.Detail -->";
        private const string EmailDateToken = "<!-- SubHeading.Email.Date -->";
        private const string EmailSubjectToken = "<!-- SubHeading.Email.Title -->";
        private const string UserNameToken = "<!-- Greeting.UserName -->";
        private static readonly string HtmlBase = Properties.Resources.email_template_html;
        private static readonly List<string> _keynames = [
            EmailSubjectToken,
            EmailDateToken,
            UserNameToken,
            BodyHtmlToken
        ];
    }
}
