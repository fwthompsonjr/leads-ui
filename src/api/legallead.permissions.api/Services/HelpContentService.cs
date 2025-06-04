using legallead.permissions.api.Extensions;
using System.Text;

namespace legallead.permissions.api.Services
{
    internal static class HelpContentService
    {
        static HelpContentService()
        {
            Initialize();
        }
        private static void Initialize()
        {
            var keys = _resources.Keys;
            foreach (var key in keys)
            {
                var value = _resources[key];
                if (!string.IsNullOrEmpty(value)) continue;
                var stream = Properties.Resources.ResourceManager.GetObject(key);
                if (stream is not byte[] array) continue;                
                var revised = Encoding.UTF8.GetString(array);
                if(!string.IsNullOrEmpty(revised)) _resources[key] = revised;
            }
        }
        public static string? GetKey(string name)
        {
            if (!_resources.TryGetValue(name, out var key)) return null;
            var builder = new StringBuilder(key);
            var common = new List<string>
            {
                SectionNames.BaseCss,
                SectionNames.BaseJs,
                SectionNames.BaseMenu,
            };
            common.ForEach(c =>
            {
                var find = $"<!-- include section {c} -->";
                if (key.Contains(find) && _resources.TryGetValue(c, out var replacement) && !string.IsNullOrEmpty(replacement))
                {
                    builder.Replace(find, replacement);
                }
            });
            return builder.ToString().StandardizeBody();
        }

        public static string GetChildPage(string name)
        {
            var topics = new List<string>()
            {
                SectionNames.TopicGettingStarted,
                SectionNames.TopicAccountSettings,
            };
            if (!topics.Contains(name)) return string.Empty;

            var builder = new StringBuilder( GetKey(SectionNames.BaseLayout) ?? string.Empty);
            if (!_resources.TryGetValue(name, out var key)) return builder.ToString();
            builder.Replace(SectionNames.TopicsPlaceHolder, key);
            var changePwd = GetKey(SectionNames.TopicAccountSettingsChangePwd) ?? string.Empty;
            builder.Replace(SectionNames.TopicAccountChangePwdPlaceHolder, changePwd);
            var myprofile = GetKey(SectionNames.TopicAccountSettingsMyProfile) ?? string.Empty;
            builder.Replace(SectionNames.TopicAccountMyProfilePlaceHolder, myprofile);
            var mysearches = GetKey(SectionNames.TopicAccountSettingsMySearches) ?? string.Empty;
            builder.Replace(SectionNames.TopicAccountMySearchesPlaceHolder, mysearches);
            return builder.ToString().StandardizeBody();

        }

        private static readonly Dictionary<string, string> _resources = new()
        {
            { SectionNames.BaseLayout, "" },
            { SectionNames.BaseCss, "" },
            { SectionNames.BaseJs, "" },
            { SectionNames.BaseMenu, "" },
            { SectionNames.TopicGettingStarted, "" },
            { SectionNames.TopicAccountSettings, "" },
            { SectionNames.TopicAccountSettingsChangePwd, "" },
            { SectionNames.TopicAccountSettingsMyProfile, "" },
            { SectionNames.TopicAccountSettingsMySearches, "" },
        };
        internal static class SectionNames
        {
            public const string BaseLayout = "help-base-layout";
            public const string BaseCss = "help-css-base";
            public const string BaseJs = "help-js-base";
            public const string BaseMenu = "help-base-topics";
            public const string TopicsPlaceHolder = "<!-- include section topic details -->";
            public const string TopicAccountChangePwdPlaceHolder = "<!-- help-section-account-settings-change-password -->";
            public const string TopicAccountMyProfilePlaceHolder = "<!-- help-section-account-settings-view-profile -->";
            public const string TopicAccountMySearchesPlaceHolder = "<!-- help-section-account-settings-view-searches -->";
            public const string TopicAccountSettings = "help-section-account-settings";
            public const string TopicAccountSettingsChangePwd = "help-section-account-settings-change-password";
            public const string TopicGettingStarted = "help-section-getting-started";
            public const string TopicAccountSettingsMyProfile = "help-section-account-settings-view-profile";
            public const string TopicAccountSettingsMySearches = "help-section-account-settings-view-searches";
        }
    }
}
