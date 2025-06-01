using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class ValidPhoneNumberAttribute : ValidationAttribute
{
    private static readonly Regex PhoneRegex = new Regex(
        @"^\\s*(?:\\+?1[-.\\s]?)?              # Optional country code
          (?:\\(?\\d{3}\\)?|\\d{3})[-.\\s]?    # Area code with or without parentheses
          \\d{3}[-.\\s]?\\d{4}                 # Local number
          (?:\\s*(?:ext|x|extension)\\s*\\d{1,5})?\\s*$", // Optional extension
        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var phone = value as string;
        if (string.IsNullOrWhiteSpace(phone))
        {
            return new ValidationResult("Phone number is required.");
        }

        if (!PhoneRegex.IsMatch(phone))
        {
            return new ValidationResult("Invalid phone number format.");
        }

        return ValidationResult.Success;
    }
}

public class PhoneNumberModel
{
    public string NormalizedNumber { get; set; }
    public string Extension { get; set; }

    public override string ToString()
    {
        return string.IsNullOrEmpty(Extension)
            ? NormalizedNumber
            : $"{NormalizedNumber} x{Extension}";
    }
}

public static class PhoneNumberHelper
{
    public static PhoneNumberModel Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var extensionMatch = Regex.Match(input, @"(?:ext|x|extension)\\s*(\\d{1,5})", RegexOptions.IgnoreCase);
        string extension = extensionMatch.Success ? extensionMatch.Groups[1].Value : null;

        string digitsOnly = Regex.Replace(input, @"\\D", "");

        if (digitsOnly.Length == 11 && digitsOnly.StartsWith("1"))
        {
            digitsOnly = digitsOnly.Substring(1);
        }

        if (digitsOnly.Length != 10)
        {
            return null;
        }

        string normalized = "1" + digitsOnly;

        return new PhoneNumberModel
        {
            NormalizedNumber = normalized,
            Extension = extension
        };
    }
}

public class EmailModel
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(256, ErrorMessage = "Email address cannot exceed 256 characters.")]
    public string Email { get; set; }
}
