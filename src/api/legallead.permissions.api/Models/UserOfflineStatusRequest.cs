using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Models
{
    public class UserOfflineStatusRequest
    {
        [Required]
        [MinLength(32, ErrorMessage = "{0} must have a minimum length of {1} characters")]
        [MaxLength(50, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string LeadId { get; set; } = string.Empty;
    }
}
