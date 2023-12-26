using legallead.desktop.entities;
using legallead.desktop.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.implementations
{
    internal class ErrorContentProvider : IErrorContentProvider
    {
        public List<ContentHtml> ContentNames => ContentList();

        public List<int> Names => ContentIndexes();

        public ContentHtml? GetContent(int name)
        {
            var id = ErrContentList.Find(s => s.StatusCode.Equals(name));
            if (id == null) return null;
            return new ContentHtml
            {
                Index = id.Index,
                Content = id.Content,
                Name = id.Name
            };
        }

        public bool IsValid(int name)
        {
            return Names.Contains(name);
        }

        private static List<int> ContentIndexes()
        {
            return ErrContentList.Select(x => x.StatusCode).ToList();
        }

        private static List<ContentHtml> ContentList()
        {
            return ErrContentList.Cast<ContentHtml>().ToList();
        }

        private static List<ErrorContentHtml> ErrContentList => ErrorContentHtml.ErrorContentList();
    }
}