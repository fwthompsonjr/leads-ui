using legallead.permissions.api.Attr;
using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Model
{
    public class ChangeContactPhoneRequest
    {
        [Required]
        [PhoneType]
        [StringLength(15, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string? PhoneType { get; set; }

        [Required(ErrorMessage = "You must provide a phone number")]
        [MaxLength(25, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string? Phone { get; set; }
    }
}