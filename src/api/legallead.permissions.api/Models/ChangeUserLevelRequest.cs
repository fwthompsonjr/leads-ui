using legallead.permissions.api.Model;
using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Models
{
    public class ChangeUserLevelRequest : UserLevelRequest
    {
        [Required]
        [StringLength(10)]
        public string? ExternalId { get; set; }
    }
}
