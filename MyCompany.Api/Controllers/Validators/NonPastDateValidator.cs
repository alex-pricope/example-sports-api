using System.ComponentModel.DataAnnotations;

namespace MyCompany.Api.Controllers.Validators;

/// <summary>
/// This validator is used to validate the date of birth
/// </summary>
public class ValidDateOnlyValidator (int minAge) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        switch (value)
        {
            case null:
                return new ValidationResult("Date cannot be empty");
            case DateOnly date:
            {
                if (date > DateOnly.FromDateTime(DateTime.UtcNow))
                {
                    return new ValidationResult("Date cannot be in the future");
                }
                
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var minDate = today.AddYears(-minAge);

                if (date > minDate)
                {
                    return new ValidationResult($"Age must be minimum {minAge} years.");
                }

                break;
            }
            default:
                return new ValidationResult("Date is not a valid date");
        }

        return ValidationResult.Success;
    }
}