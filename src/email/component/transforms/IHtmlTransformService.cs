using legallead.email.models;

namespace legallead.email.transforms
{
    public interface IHtmlTransformService
    {
        string BaseHtml { get; }
        List<string> KeyNames { get; }
        Dictionary<string, string?> Substitutions { get; }

        Task<string?> GetHtmlTemplate(UserSettingQuery user, string templateName);
    }
}