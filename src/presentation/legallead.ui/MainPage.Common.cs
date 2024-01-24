using legallead.ui.Utilities;
using Msp = Microsoft.Maui.Controls.Shapes;

namespace legallead.ui
{
    public partial class MainPage
    {

        internal WebView WebViewer => mainWebViewer;
        internal HtmlWebViewSource WebViewSource => mainWebContent;
        internal Msp.Rectangle StatusIcon => statusBar.Icon;
        internal Label StatusText => statusBar.Text;
        internal Label StatusMessage => statusBar.Message;
        internal Label StatusConnection => statusBar.Connection;
        internal MenuBarItem[]? BindableToolbars => toolbars;


        internal MainContentLoadHandler HomeHandler => mainContentLoadHandler;
        internal MyAccountContentLoadHandler MyAccountHandler => myAccountContentLoadHandler;
        internal const string InternalDomain = "internal.legalead.com";

        private sealed class StatusBar
        {
            private readonly MainPage main;
            public StatusBar(MainPage main)
            {
                this.main = main;
            }
            public Msp.Rectangle Icon => main.sbStatusApplicationIcon;
            public Label Text => main.sbStatusApplicationText;
            public Label Message => main.sbComment;
            public Label Connection => main.sbConnectionStatusText;
        }
    }
}
