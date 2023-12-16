using legallead.desktop.entities;

namespace legallead.desktop.interfaces
{
    internal interface IContentHtmlNames
    {
        List<ContentHtml> ContentNames { get; }
        List<string> Names { get; }

        bool IsValid(string name);
    }
}