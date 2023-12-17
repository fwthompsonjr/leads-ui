using legallead.desktop.entities;
using legallead.desktop.interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace legallead.desktop.implementations
{
    internal class ContentHtmlNames : IContentHtmlNames
    {
        public List<ContentHtml> ContentNames => _contents;

        public List<string> Names => _names ??= GetNames();

        public bool IsValid(string name)
        {
            return Names.Contains(name, StringComparer.OrdinalIgnoreCase);
        }

        public ContentHtml? GetContent(string name)
        {
            if (!IsValid(name)) return null;
            var item = ContentNames.Find(x =>
                x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            if (item == null) return null;
            if (string.IsNullOrEmpty(item.Content))
            {
                MapResourceContent(item);
            }
            return item;
        }

        public Stream GetContentStream(string name)
        {
            var item = GetContent(name);
            if (item == null || string.IsNullOrEmpty(item.Content))
                return new System.IO.MemoryStream();
            return GenerateStreamFromString(item.Content);
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private List<string>? _names;

        private static List<string> GetNames()
        {
            var names = _contents.Select(x => x.Name.Split('-')[0].ToLower()).ToList();
            return names;
        }

        [ExcludeFromCodeCoverage]
        private static void MapResourceContent(ContentHtml? item)
        {
            if (item == null) return;
            if (!string.IsNullOrEmpty(item.Content)) return;
            var manager = Properties.Resources.ResourceManager;
            var resourceText = manager.GetString(item.Name);
            if (string.IsNullOrEmpty(resourceText)) return;
            if (resourceText.Contains(CssBaseLink))
            {
                string scripttag = GetBaseCssScript();
                resourceText = resourceText.Replace(CssBaseLink, scripttag);
            }
            item.Content = resourceText;
        }

        private static string GetBaseCssScript()
        {
            var basecsstext = Properties.Resources.base_css;
            var builder = new StringBuilder("<script name=\"base-css\"");
            builder.AppendLine();
            builder.AppendLine(basecsstext);
            builder.AppendLine();
            var scripttag = builder.ToString();
            return scripttag;
        }

        private static readonly List<ContentHtml> _contents = new()
        {
            new() { Index = 10, Name = "base-css"},
            new() { Index = 100, Name = "introduction-html"}
        };

        private const string CssBaseLink = "<link rel=\"stylesheet\" name=\"base\" href=\"css/base.css\" />";
    }
}