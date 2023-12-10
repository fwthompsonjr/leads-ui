using legallead.permissions.api.Attr;
using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Model
{
    public class ChangeContactEmailRequest
    {
        [Required]
        [EmailType]
        [StringLength(15, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string? EmailType { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid e-mail address.")]
        public string? Email { get; set; }
    }
}