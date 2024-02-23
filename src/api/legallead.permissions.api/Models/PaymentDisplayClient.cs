using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Models
{
    public class PaymentDisplayClient
    {
        [Required]
        public string? SessionId { get; set; }
        [Required]
        public string? ClientId { get; set; }
        [Required]
        public string? IntentId { get; set; }
    }
}
