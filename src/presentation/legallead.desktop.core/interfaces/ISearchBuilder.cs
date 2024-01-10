using legallead.desktop.entities;

namespace legallead.desktop.interfaces
{
    internal interface ISearchBuilder
    {
        StateSearchConfiguration[]? GetConfiguration();

        string GetHtml();
    }
}