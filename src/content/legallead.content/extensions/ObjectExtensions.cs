using System.ComponentModel.DataAnnotations;

namespace legallead.content.extensions
{
    internal static class ObjectExtensions
    {
        public static List<ValidationResult> GetValidationResult<T>(this T source, out bool isValid) where T : class
        {
            var context = new ValidationContext(source, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(source, context, validationResults, true);
            return validationResults;
        }
    }
}