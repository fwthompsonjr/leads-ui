using Newtonsoft.Json;

namespace page.load.utility.Extensions
{
    public static class ObjectExtensions
    {

        public static T? ToInstance<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default;
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static string? ToJsonString(this object obj)
        {
            if (obj == null) return null;
            return JsonConvert.SerializeObject(obj);
        }
    }
}
