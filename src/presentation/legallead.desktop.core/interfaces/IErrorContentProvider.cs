using legallead.desktop.entities;

namespace legallead.desktop.interfaces
{
    internal interface IErrorContentProvider
    {
        List<ContentHtml> ContentNames { get; }
        List<int> Names { get; }

        ContentHtml? GetContent(int name);

        bool IsValid(int name);
    }
}