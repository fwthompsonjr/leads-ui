using legallead.email.models;
using legallead.email.transforms;

namespace legallead.email.implementations
{
    public class PasswordChangedTemplate : HtmlTransformDetailBase
    {

        public override string BaseHtml => HtmlBase;

        public override List<string> KeyNames => _keynames;

        public override void FetchTemplateParameters(List<UserEmailSettingBo> attributes)
        {
        }

        public override Dictionary<string, string?> Substitutions => _substitions;
        private static readonly string HtmlBase = Properties.Resources.email_template_html_body_password_changed;
        private static readonly List<string> _keynames = [];
    }
}