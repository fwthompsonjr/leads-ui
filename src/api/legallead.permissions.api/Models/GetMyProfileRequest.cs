using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Models
{
    public class GetMyProfileRequest
    {
        [Required]
        [MinLength(32, ErrorMessage = "{0} must have a minimum length of {1} characters")]
        [MaxLength(50, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string LeadId { get; set; } = string.Empty;
    }

    public class UpdateMyProfileRequest
    {
        [Required]
        [MinLength(32, ErrorMessage = "{0} must have a minimum length of {1} characters")]
        [MaxLength(50, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string LeadId { get; set; } = string.Empty;

        [Required]
        public string Updates { get; set; } = string.Empty;

    }
}