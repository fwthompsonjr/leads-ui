using legallead.permissions.api.Attr;
using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Model
{
    public class ChangeContactNameRequest
    {
        [Required]
        [NameType]
        [StringLength(15, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string? NameType { get; set; }

        [Required]
        [StringLength(175, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string? Name { get; set; }
    }
}