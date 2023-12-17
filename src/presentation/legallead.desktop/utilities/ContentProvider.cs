using legallead.desktop.implementations;
using legallead.desktop.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.utilities
{
    internal static class ContentProvider
    {
        private static IContentHtmlNames _localContentProvider;

        public static IContentHtmlNames LocalContentProvider =
            _localContentProvider ??= new ContentHtmlNames();
    }
}