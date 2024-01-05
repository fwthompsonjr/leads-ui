using AngleSharp;
using CefSharp.Wpf;
using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.js;
using legallead.desktop.utilities;
using System;
using System.Linq;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private void InitializeBrowserContent()
        {
            string[] names = new[] { "home", "blank" };
            var target = isBlankLoaded ? names[0] : names[1];
            var blankContent = ContentHandler.GetLocalContent(target);
            if (blankContent != null)
            {
                var blankHtml = ContentHandler.GetAddressBase64(blankContent);
                var browser = new ChromiumWebBrowser()
                {
                    Address = blankHtml
                };
                var handler = target == names[1] ? new JsHandler(browser) : new HomeJsHandler(browser);
                browser.JavascriptObjectRepository.Register("jsHandler", handler);
                content1.Content = browser;
            }
        }
    }
}