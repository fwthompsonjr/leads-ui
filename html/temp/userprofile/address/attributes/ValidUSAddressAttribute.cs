using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class ValidUSAddressAttribute : ValidationAttribute
{
    private static readonly Regex[] AddressPatterns = new[]
    {
        // Standard street address: 123 Main St, Springfield, IL 62704
        new Regex(@"^\d+\s+[\w\s]+\s+(Street|St|Avenue|Ave|Boulevard|Blvd|Road|Rd|Lane|Ln|Drive|Dr|Court|Ct|Circle|Cir|Way|Terrace|Ter|Place|Pl)\,?\s+[\w\s]+\,?\s+[A-Z]{2}\s+\d{5}(-\d{4})?$", RegexOptions.IgnoreCase),

        // PO Box: PO Box 123, Springfield, IL 62704
        new Regex(@"^(P\.?O\.?|Post\sOffice)\sBox\s\d+\,?\s+[\w\s]+\,?\s+[A-Z]{2}\s+\d{5}(-\d{4})?$", RegexOptions.IgnoreCase),

        // Military address: PSC 123 Box 4567, APO AE 09012
        new Regex(@"^(PSC|Unit)\s\d+\s(Box|BOX)\s\d+\,?\s+(APO|FPO|DPO)\s+(AA|AE|AP)\s+\d{5}(-\d{4})?$", RegexOptions.IgnoreCase)
    };

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var address = value as string;
        if (string.IsNullOrWhiteSpace(address))
        {
            return new ValidationResult("Address is required.");
        }

        foreach (var pattern in AddressPatterns)
        {
            if (pattern.IsMatch(address))
            {
                return ValidationResult.Success;
            }
        }

        return new ValidationResult("Invalid US postal address format.");
    }
}
