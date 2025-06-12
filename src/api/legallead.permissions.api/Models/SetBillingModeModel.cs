using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Models
{
    public class SetBillingModeModel
    {
        [Required]
        [StringLength(50, MinimumLength = 30)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(4, MinimumLength = 1)]
        public string BillingCode { get; set; } = string.Empty;
    }
}
