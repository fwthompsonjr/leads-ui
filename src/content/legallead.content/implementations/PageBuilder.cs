using legallead.content.entities;
using legallead.content.interfaces;
using System.Text;

namespace legallead.content.implementations
{
    public class PageBuilder : IPageBuilder
    {
        private readonly IWebPageRepository _repo;

        public PageBuilder(IWebPageRepository repo)
        {
            _repo = repo;
        }

        public List<string> Pages => pageNames;

        public async Task<string> GetContent(string pageName)
        {
            if (!Pages.Contains(pageName))
                throw new ArgumentException(pageName);

            var pages = await _repo.ContentRepository.GetAllActive();
            var requested = pages.Where(w => (w.ContentName ?? "").StartsWith(pageName)).ToList();
            var contentLines = new List<WebContentLineDto>();
            // fetch data
            requested.ForEach(async r =>
            {
                contentLines.AddRange(await _repo.LineRepository.GetAll(r));
            });
            var parentRows = requested.FindAll(x => !x.IsChild.GetValueOrDefault());
            var childRows = requested.FindAll(x => x.IsChild.GetValueOrDefault());
            var builder = new StringBuilder();
            parentRows.ForEach(p =>
            {
                var data = contentLines.Where(c => (c.ContentId ?? "").Equals(p.Id));
                var rows = string.Join(Environment.NewLine, data.Select(c => c.Content));
                if (parentRows.IndexOf(p) == 0)
                {
                    builder.AppendLine(rows);
                }
                else
                {
                    var identity = $"<!-- {p.ContentName} -->";
                    builder.Replace(identity, rows);
                }
            });
            childRows.ForEach(child =>
            {
                var data = contentLines.Where(c => (c.ContentId ?? "").Equals(child.Id));
                var rows = string.Join(Environment.NewLine, data.Select(c => c.Content));
                var identity = $"<!-- {child.ContentName} -->";
                builder.Replace(identity, rows);
            });
            return builder.ToString();
        }

        public async Task<string> GetContent(string pageName, IEnumerable<KeyValuePair<string, string>> substitutions)
        {
            var content = await GetContent(pageName);
            var builder = new StringBuilder(content);
            var list = ValidateCollection(substitutions.ToList());
            list.ForEach(s => builder.Replace(s.Key, s.Value));
            return builder.ToString();
        }

        private static List<KeyValuePair<string, string>> ValidateCollection(List<KeyValuePair<string, string>> source)
        {
            return source.FindAll(s =>
            {
                var key = s.Key;
                if (string.IsNullOrWhiteSpace(key)) return false;
                if (!key.StartsWith("{{")) return false;
                if (!key.EndsWith("}}")) return false;
                return true;
            });
        }

        private static readonly List<string> pageNames = new(){
            "HOME.PRE.LOGIN"
        };
    }
}