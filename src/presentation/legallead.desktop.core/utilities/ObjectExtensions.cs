using Newtonsoft.Json;

namespace legallead.desktop
{
    internal static class ObjectExtensions
    {
        internal static T TryGet<T>(string source) where T : new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(source) ?? new();
            }
            catch
            {
                return new T();
            }
        }
    }
}