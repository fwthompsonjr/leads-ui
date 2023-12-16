using legallead.desktop.entities;
using legallead.desktop.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

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

        private List<string>? _names;

        private static List<string> GetNames()
        {
            var names = _contents.Select(x => x.Name).ToList();
            return names;
        }

        private static readonly List<ContentHtml> _contents = new()
        {
            new() { Index = 10, Name = "Introduction"}
        };
    }
}