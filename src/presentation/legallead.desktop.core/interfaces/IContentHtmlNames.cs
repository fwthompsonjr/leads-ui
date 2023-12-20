using legallead.desktop.entities;

namespace legallead.desktop.interfaces
{
    internal interface IContentHtmlNames
    {
        List<ContentHtml> ContentNames { get; }
        List<string> Names { get; }

        ContentHtml? GetContent(string name);

        Stream GetContentStream(string name);

        bool IsValid(string name);
    }
}