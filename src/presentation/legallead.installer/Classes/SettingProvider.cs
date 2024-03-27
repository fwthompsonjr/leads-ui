using legallead.installer.Models;
using Newtonsoft.Json;

namespace legallead.installer.Classes
{
    internal static class SettingProvider
    {
        private static readonly string JsContent = Properties.Resources.configuration_js;

        private static CommonSettings? _common;
        public static CommonSettings Common()
        {
            if (_common != null) { return _common; }
            _common = JsonConvert.DeserializeObject<CommonSettings>(JsContent) ?? new();
            return _common;
        }
    }
}
