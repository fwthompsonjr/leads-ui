using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Entities
{
    public class AppSettingRequest
    {
        [StringLength(50, MinimumLength = 5)]
        public string KeyName { get; set; } = string.Empty;
    }
}
