using legallead.email.models;
using System.Text;

namespace legallead.email.transforms
{
    public abstract class HtmlTransformDetailBase : IHtmlTransformDetailBase
    {
        protected HtmlTransformDetailBase()
        {
            
        }

        public abstract string BaseHtml { get; }
        public abstract List<string> KeyNames { get; }
        public abstract Dictionary<string, string?> Substitutions { get; }


        public string? GetHtmlTemplate(List<UserEmailSettingBo> attributes)
        {
            var keynames = Substitutions.Keys.ToList();
            var builder = new StringBuilder(BaseHtml);
            if (attributes.Count == 0)
            {
                keynames.ForEach(x => builder.Replace(x, string.Empty));
                return builder.ToString();
            }
            attributes.ForEach(a =>
            {
                if (!string.IsNullOrEmpty(a.KeyName))
                {
                    var key = keynames.Find(kn => kn.Contains(a.KeyName, StringComparison.OrdinalIgnoreCase));
                    if (key != null && !string.IsNullOrEmpty(a.KeyValue)) { Substitutions[key] = a.KeyValue; }
                }
            });
            keynames.ForEach(x => builder.Replace(x, Substitutions[x] ?? string.Empty));
            return builder.ToString();
        }
    }
}
