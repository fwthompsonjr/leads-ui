using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace legallead.desktop
{
    internal static class ObjectExtensions
    {
        [ExcludeFromCodeCoverage(Justification = "All code is executing in 3rd party assembly.")]
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