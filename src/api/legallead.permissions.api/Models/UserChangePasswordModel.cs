using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Model
{
    public class UserChangePasswordModel
    {
        private const string Pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,255}$";

        [Required]
        [MinLength(8, ErrorMessage = "{0} must have a minimum length of {1} characters")]
        [MaxLength(255, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "{0} must have a minimum length of {1} characters")]
        [MaxLength(255, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        [RegularExpression(Pattern, ErrorMessage = "Please enter a valid password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [MinLength(8, ErrorMessage = "{0} must have a minimum length of {1} characters")]
        [MaxLength(255, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}