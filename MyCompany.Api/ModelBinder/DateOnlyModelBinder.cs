using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MyCompany.Api.ModelBinder;

/// <summary>
/// Allows DateOnly to work with Json Serializer
/// </summary>
public class DateOnlyModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var input = bindingContext.ValueProvider.GetValue(bindingContext.FieldName).FirstValue;

        if (DateOnly.TryParse(input, out var date))
            bindingContext.Result = ModelBindingResult.Success(date);
        else
            bindingContext.ModelState.AddModelError(bindingContext.FieldName, "Invalid date format");

        return Task.CompletedTask;
    }
}