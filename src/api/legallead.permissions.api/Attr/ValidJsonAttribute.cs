using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Attr
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidJsonAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not string json) return false;
            if (string.IsNullOrEmpty(json)) return true;
            try
            {
                var obj = JsonConvert.DeserializeObject(json);
                return obj != null;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}