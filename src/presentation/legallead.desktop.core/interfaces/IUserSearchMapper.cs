using HtmlAgilityPack;

namespace legallead.desktop.interfaces
{
    internal interface IUserSearchMapper
    {
        string Map(string? history);
        string Map(IHistoryPersistence? persistence, string? history);
        void SetFilter(IHistoryPersistence? persistence, HtmlNode? combo);
    }
}
