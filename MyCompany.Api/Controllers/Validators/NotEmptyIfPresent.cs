using System.ComponentModel.DataAnnotations;

namespace MyCompany.Api.Controllers.Validators;

public class NotEmptyIfPresentAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string strValue && string.IsNullOrWhiteSpace(strValue))
            return new ValidationResult(ErrorMessage ?? "The field cannot be null, empty, or whitespace.");
        
        if (value is DateOnly dateValue && dateValue == default)
            return new ValidationResult(ErrorMessage ?? "The field cannot be an empty or default date.");

        return ValidationResult.Success;
    }
}