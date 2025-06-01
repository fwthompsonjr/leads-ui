using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class AddressValidator
{
    public ValidationModel Validate(ValidationModel model)
    {
		var response = new ValidationModel { Text = model.Text };
		var address = new AddressModel { AddressBlock = model.Text };
		var context = new ValidationContext(address);
		var results = new List<ValidationResult>();

		response.IsValid = Validator.TryValidateObject(address, context, results, true);
		if (response.IsValid) return response;
		foreach (var validationResult in results)
		{
			response.Messages.Add(validationResult.ErrorMessage);
		}

        return response;
    }
}

public class ValidationModel
{
	public string Text { get; set; } = string.Empty;
	public bool IsValid { get; set; }
	public List<string> Messages { get; set; } = [];
}