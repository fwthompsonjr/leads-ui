using System.ComponentModel.DataAnnotations;

namespace legallead.json.db.attr
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StateCheckAttribute : ValidationAttribute
    {
        public StateCheckAttribute(string stateNameField)
        {
            StateName = stateNameField;
        }

        private string StateName { get; set; }

#pragma warning disable CS8603 // Possible null reference return.

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            string? countyName = GetString(value);
            var propValue = validationContext.ObjectType.GetProperty(StateName)?.GetValue(validationContext.ObjectInstance, null);
            string? stateName = GetString(propValue);
            var stateObj = UsStatesList.Find(stateName);
            if (stateObj == null) return new ValidationResult("State field cannot be null.");
            var countyObj = UsStateCountyList.FindAll(countyName);
            if (countyObj == null) return new ValidationResult("County field cannot be null.");
            var match = countyObj.Exists(a => (a.StateCode ?? "").Equals(stateObj.ShortName, StringComparison.OrdinalIgnoreCase));
            if (match)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"County - {countyName} - is not located in {stateName}.");
            }
        }

#pragma warning restore CS8603 // Possible null reference return.

        private static string? GetString(object? value)
        {
            if (value == null) return null;
            if (value is string s) return s;
            return null;
        }
    }
}