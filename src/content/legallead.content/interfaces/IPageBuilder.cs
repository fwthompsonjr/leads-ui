namespace legallead.content.interfaces
{
    public interface IPageBuilder
    {
        List<string> Pages { get; }

        Task<string> GetContent(string pageName);

        Task<string> GetContent(string pageName, IEnumerable<KeyValuePair<string, string>> substitutions);
    }
}