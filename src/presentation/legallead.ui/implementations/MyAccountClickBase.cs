using legallead.ui.Utilities;

namespace legallead.ui.implementations
{
    internal abstract class MyAccountClickBase
    {
        protected abstract string PageTarget { get; }

        public void Click()
        {
            var main = MainPageFinder.GetMain();
            if (main == null) return;
            var content = Transform(ContentLoadBase.GetHTML(PageTarget));
            main.Dispatcher.Dispatch(() =>
            {
                main.WebViewSource.Html = content;
                MainPageFinder.TryContentReload();
            });

        }

        protected virtual string Transform(string text)
        {
            return text;
        }
    }
}