using legallead.permissions.api.Attr;
using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Model
{
    public class ChangeContactAddressRequest
    {
        [Required]
        [AddressType]
        [StringLength(15, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string? AddressType { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string? Address { get; set; }
    }
}