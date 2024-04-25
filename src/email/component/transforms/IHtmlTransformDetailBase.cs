using legallead.email.models;

namespace legallead.email.transforms
{
    public interface IHtmlTransformDetailBase
    {
        string BaseHtml { get; }
        List<string> KeyNames { get; }
        Dictionary<string, string?> Substitutions { get; }

        string? GetHtmlTemplate(List<UserEmailSettingBo> attributes);
    }
}