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
            const char minus = '-';
            if (!IsValid(name)) return null;
            var item = ContentNames
                    .Where(w => w.Name.Contains(minus))
                    .FirstOrDefault(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
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
            const char minus = '-';
            var names = _contents.Select(x => x.Name.Split(minus)[0].ToLower()).ToList();
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
            var keys = Replacements.Keys;
            foreach (var key in from key in keys
                                where resourceText.Contains(key)
                                select key)
            {
                resourceText = resourceText.Replace(key, Replacements[key]);
            }

            item.Content = resourceText;
        }

        private static string GetBaseCssScript()
        {
            var basecsstext = Properties.Resources.base_css;
            var builder = new StringBuilder("<style name=\"base-css\">");
            builder.AppendLine();
            builder.AppendLine(basecsstext);
            builder.AppendLine();
            builder.AppendLine("</style>");
            var scripttag = builder.ToString();
            return scripttag;
        }

        private static string GetLoginInclude()
        {
            var text = Properties.Resources.homelogin_html;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetRegistrationInclude()
        {
            var text = Properties.Resources.homeregister_html;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GetBootstrapCssScript()
        {
            var basecsstext = Properties.Resources.homelogin_html;
            var builder = new StringBuilder("<style name=\"base-css\">");
            builder.AppendLine();
            builder.AppendLine(basecsstext);
            builder.AppendLine();
            builder.AppendLine("</style>");
            var scripttag = builder.ToString();
            return scripttag;
        }

        private static string GetHomeValidationScript()
        {
            var basecsstext = Properties.Resources.homevalidation_js;
            var builder = new StringBuilder("<script name=\"home-form-validation\">");
            builder.AppendLine();
            builder.AppendLine(basecsstext);
            builder.AppendLine();
            builder.AppendLine("</script>");
            var scripttag = builder.ToString();
            return scripttag;
        }

        private static readonly List<ContentHtml> _contents = new()
        {
            new() { Index = -1, Name = "test"},
            new() { Index = 0, Name = "blank-html"},
            new() { Index = 10, Name = "base-css"},
            new() { Index = 100, Name = "introduction-html"},
            new() { Index = 110, Name = "home-html"},
            new() { Index = 110, Name = "homelogin-html"}
        };

        private const string CssBaseLink = "<link rel=\"stylesheet\" name=\"base\" href=\"css/base.css\" />";
        private const string CssBootStrapLink = "<link rel=\"stylesheet\" href=\"bootstrap.min.css\" />";
        private const string HtmLoginInclude = "<p>Login form</p>";
        private const string HtmRegistrationInclude = "<p>Registration form</p>";
        private const string JsHomeValidation = "<!-- script: home-form-validation -->";

        private static readonly Dictionary<string, string> Replacements = new() {
            { CssBaseLink, GetBaseCssScript() },
            { CssBootStrapLink, GetBootstrapCssScript() },
            { HtmLoginInclude, GetLoginInclude() },
            { HtmRegistrationInclude, GetRegistrationInclude() },
            { JsHomeValidation, GetHomeValidationScript() }
        };
    }
}