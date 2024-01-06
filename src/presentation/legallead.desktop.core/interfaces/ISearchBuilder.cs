using legallead.desktop.entities;

namespace legallead.desktop.interfaces
{
    internal interface ISearchBuilder
    {
        IEnumerable<StateSearchConfiguration>? GetConfiguration();

        string GetHtml();
    }
}