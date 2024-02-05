using Newtonsoft.Json;

namespace legallead.ui.Utilities
{
    internal static class JsonExtensions
    {
        public static T? TryDeserialize<T>(this string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
