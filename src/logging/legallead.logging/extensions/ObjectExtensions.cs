using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text;

namespace legallead.logging.extensions
{
    internal static class ObjectExtensions
    {
        public static List<ValidationResult> ValidateMe<T>(this T source, out bool isValid) where T : class
        {
            var context = new ValidationContext(source, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(source, context, validationResults, true);
            return validationResults;
        }

        public static Dictionary<int, string> SplitByLength(this string sentence, int maxLength = 100)
        {
            string[] words = sentence.Split(' ');
            var parts = new Dictionary<int, string>();
            var sb = new StringBuilder();
            int partCounter = 0;
            foreach (var word in words)
            {
                var length = sb.Length;
                if (length + word.Length < maxLength)
                {
                    sb.Append(length == 0 ? word : $" {word}");
                }
                else
                {
                    parts.Add(partCounter, sb.ToString());
                    sb = new StringBuilder(word);
                    partCounter++;
                }
            }
            parts.Add(partCounter, sb.ToString());
            return parts;
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value[..maxLength];
        }

        public static string SubstituteSingleQuote(this string? value)
        {
            var sq = "'";
            var dq = "\"";
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Replace(sq, dq);
        }

        public static string ToSprocParameter(this string? parameter)
        {
            if (parameter == null) return NullString;
            return $"'{parameter}'";
        }

        public static string ToSprocParameter(this int? parameter)
        {
            if (parameter == null) return NullString;
            return $"{parameter}";
        }

        public static string ToSprocParameter(this long? parameter)
        {
            if (parameter == null) return NullString;
            return $"{parameter}";
        }

        private const string NullString = "NULL";
    }
}