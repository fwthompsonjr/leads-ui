using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Models
{
    public class CountyCodeRequest
    {
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string UserId { get; set; } = string.Empty;
    }
}
