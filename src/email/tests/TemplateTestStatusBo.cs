using legallead.email.utility;
using Newtonsoft.Json;

namespace legallead.email.tests
{
    internal class TemplateTestStatusBo
    {
        [JsonProperty("name")]
        public string Template { get; set; } = string.Empty;
        [JsonProperty("isTesting")]
        public bool IsTesting { get; set; }
        public TemplateNames? GetTemplateName()
        {
            if (Enum.TryParse<TemplateNames>(Template, true, out var templateName)) return templateName;
            return null;
        }
    }
}
