using System.ComponentModel.DataAnnotations;

namespace Assignment5.Validation;

public class DateRange : ValidationAttribute
{
    private readonly DateTime _minDate;

    public DateRange(string minDate = "1900-01-01")
    {
        _minDate = DateTime.Parse(minDate);
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Date of birth cannot be empty.");
        }

        var isValidDate = DateTime.TryParse(value.ToString(), out var dateValue);

        if (!isValidDate)
        {
            return new ValidationResult("Date of birth is invalid.");
        }

        var maxDate = DateTime.Today;

        if (dateValue < _minDate || dateValue > maxDate)
        {
            return new ValidationResult(
                $"Date of birth must be between {_minDate:MM/dd/yyyy} and {maxDate:MM/dd/yyyy}.");
        }

        return ValidationResult.Success;
    }
}