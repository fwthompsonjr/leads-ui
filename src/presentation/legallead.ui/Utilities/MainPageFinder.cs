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
    }
}
