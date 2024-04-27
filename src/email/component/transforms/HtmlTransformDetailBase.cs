using legallead.email.models;
using System.Text;

namespace legallead.email.transforms
{
    public abstract class HtmlTransformDetailBase : IHtmlTransformDetailBase
    {
        protected readonly string _templateName;
        protected readonly Dictionary<string, string?> _substitions = [];
        protected HtmlTransformDetailBase()
        {
            _templateName= GetType().Name.Replace("Template", "");
        }
        public abstract string BaseHtml { get; }
        public abstract List<string> KeyNames { get; }
        public virtual Dictionary<string, string?> Substitutions => _substitions;
        public virtual string TemplateName => _templateName;

        public abstract void FetchTemplateParameters(List<UserEmailSettingBo> attributes);

        public string? GetHtmlTemplate(List<UserEmailSettingBo> attributes)
        {
            const string keyTemplate = "<!-- {0} -->";
            const string dash = " - ";
            var keynames = Substitutions.Keys.ToList();
            var builder = new StringBuilder(BaseHtml);
            if (attributes.Count == 0)
            {
                keynames.ForEach(x => builder.Replace(x, dash));
                return builder.ToString();
            }
            attributes.ForEach(a =>
            {
                if (!string.IsNullOrEmpty(a.KeyName))
                {
                    var find = string.Format(keyTemplate, a.KeyName);
                    var keyvalue = string.IsNullOrWhiteSpace(a.KeyValue) ? dash : a.KeyValue;
                    var key = keynames.Find(kn => kn.Equals(find, StringComparison.OrdinalIgnoreCase));
                    if (key != null) { Substitutions[key] = keyvalue; }
                }
            });
            keynames.ForEach(x => builder.Replace(x, Substitutions[x] ?? dash));
            return builder.ToString();
        }
    }
}
