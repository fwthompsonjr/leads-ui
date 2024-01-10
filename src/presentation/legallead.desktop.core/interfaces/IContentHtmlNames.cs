using legallead.desktop.entities;

namespace legallead.desktop.interfaces
{
    internal interface IContentHtmlNames
    {
        List<ContentHtml> ContentNames { get; }
        List<string> Names { get; }
        ISearchBuilder? SearchUi { get; set; }

        ContentHtml? GetContent(string name);

        Stream GetContentStream(string name);

        bool IsValid(string name);
    }
}