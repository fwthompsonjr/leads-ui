using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Models
{
    public class UserCountyCredentialModel
    {

        [Required]
        [MaxLength(255, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string CountyName { get; set; } = string.Empty;

        [Required]
        [MinLength(4, ErrorMessage = "{0} must have a minimum length of {1} characters")]
        [MaxLength(250, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string Password { get; set; } = string.Empty;
    }
}