namespace legallead.ui.Utilities
{
    internal static class MainPageFinder
    {
        public static MainPage? GetMain()
        {
            var page = Application.Current?.MainPage;
            if (page is not AppShell app) return null;
            if (app.CurrentPage is not MainPage main) return null;
            return main;
        }

        internal static void TryContentReload()
        {
            try
            {
                GetMain()?.WebViewer.Reload();
            }
            catch
            {
                // this empty catch block is intended
                // if reload fails the content is not valid html
                // and doesnt need a reload
            }
        }
    }
}
