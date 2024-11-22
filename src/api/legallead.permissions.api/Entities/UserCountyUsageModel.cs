using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Entities
{
    public class UserCountyUsageModel
    {

        [Required]
        [MaxLength(255, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string CountyName { get; set; } = string.Empty;

        [Required]
        [MinLength(4, ErrorMessage = "{0} must have a minimum length of {1} characters")]
        [MaxLength(250, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Range(-1, 100000)]
        public int MonthlyUsage { get; set; } = 0;

        [MaxLength(50, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string? DateRange { get; set; } = string.Empty;
    }
}