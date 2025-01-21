using System;
using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1.Validation
{
    public class NonDefaultDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateValue && dateValue == DateTime.MinValue)
            {
                return new ValidationResult(ErrorMessage ?? "Date cannot be the default value (01/01/0001).");
            }
            return ValidationResult.Success!;
        }
    }
}
