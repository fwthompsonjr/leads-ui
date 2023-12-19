using CefSharp.Wpf;
using legallead.desktop.js;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.models
{
    internal class ContentRegistrationResponse
    {
        public ChromiumWebBrowser? Browser { get; set; }

        public JsHandler? Handler { get; set; }
    }
}