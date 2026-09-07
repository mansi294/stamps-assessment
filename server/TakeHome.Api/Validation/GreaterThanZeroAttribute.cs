using System.ComponentModel.DataAnnotations;

namespace TakeHome.Api.Validation;

public class GreaterThanZeroAttribute : ValidationAttribute
{
    public GreaterThanZeroAttribute()
    {
        ErrorMessage = "The value must be greater than 0.";
    }

    public override bool IsValid(object? value)
    {
        return value switch
        {
            null => false,
            double number => number > 0,
            _ => false
        };
    }
}