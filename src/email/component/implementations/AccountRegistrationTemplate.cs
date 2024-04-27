using legallead.email.models;
using legallead.email.transforms;
using legallead.email.utility;

namespace legallead.email.implementations
{
    public class AccountRegistrationTemplate : HtmlTransformDetailBase
    {
        public AccountRegistrationTemplate()
        {
            _keynames.ForEach(key =>
            {
                _substitions[key] = null;
            });
        }

        public override string BaseHtml => HtmlBase;

        public override List<string> KeyNames => _keynames;

        public override void FetchTemplateParameters(List<UserEmailSettingBo> attributes)
        {
            const string dash = " - ";
            var named = Enum.TryParse<TemplateNames>(TemplateName, out var template);
            if (!named) return;
            if (template != TemplateNames.AccountRegistration) return;

            var item = attributes.Find(a => !string.IsNullOrWhiteSpace(a.Email));
            if (item == null) return;
            var keys = new[]
            {
                new { KeyName = "Email Address", KeyValue = item.Email ?? dash},
                new { KeyName = "User Name", KeyValue = item.UserName ?? dash}
            };
            foreach (var key in keys)
            {
                var addition = attributes.Find(a => (a.KeyName ?? string.Empty).Equals(key.KeyName));
                var isnew = addition == null;
                addition ??= new() { Id = item.Id, Email = item.Email, UserName = item.UserName, KeyName = key.KeyName };
                addition.KeyValue = key.KeyValue;
                if (isnew) { attributes.Add(addition); }
            }
        }

        public override Dictionary<string, string?> Substitutions => _substitions;
        private const string EmailToken = "<!-- Email Address -->";
        private const string UserToken = "<!-- User Name -->";
        private static readonly string HtmlBase = Properties.Resources.email_template_html_body_account_registered;
        private static readonly List<string> _keynames = [
            EmailToken,
            UserToken
        ];
    }
}
