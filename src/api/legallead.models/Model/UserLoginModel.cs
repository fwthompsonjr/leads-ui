using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Model
{
    public class UserLoginModel
    {
        private const string Pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,255}$";

        [Required]
        [MinLength(8, ErrorMessage = "{0} must have a minimum length of {1} characters")]
        [MaxLength(255, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "{0} must have a minimum length of {1} characters")]
        [MaxLength(255, ErrorMessage = "{0} must have a maximum length of {1} characters")]
        [RegularExpression(Pattern, ErrorMessage = "Please enter a valid password")]
        public string Password { get; set; } = string.Empty;
    }
}