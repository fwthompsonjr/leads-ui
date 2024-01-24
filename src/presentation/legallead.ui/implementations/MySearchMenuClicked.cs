using legallead.desktop.utilities;
using legallead.ui.interfaces;

namespace legallead.ui.implementations
{
    internal class MySearchMenuClicked : MyAccountClickBase, IMenuClickHandler
    {
        protected override string PageTarget => GetSearchCode();

        private static string GetSearchCode()
        {
            const string keyname = "landings:search";
            const string fallback = "mysearch";
            var configuration = AppBuilder.Configuration;
            if (configuration == null) { return fallback; }
            var setting = configuration[keyname];
            if (string.IsNullOrEmpty(setting)) { return fallback; }
            return setting;
        }
    }
}