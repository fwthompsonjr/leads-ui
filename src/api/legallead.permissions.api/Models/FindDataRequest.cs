using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Models
{
    public class FindDataRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string Id { get; set; } = string.Empty;
    }
}
