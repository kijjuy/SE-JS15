using System.ComponentModel.DataAnnotations;

public class RegistrationCodeAttribute : ValidationAttribute
{
    private readonly int _length = 8;

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        string stringVal = value as string;
        if (stringVal == null)
        {
            return new ValidationResult("Value is not a string.");
        }

        if (stringVal != "asdf")
        {
            return new ValidationResult($"{stringVal} is not a valid registration code.");
        }

        return ValidationResult.Success;

        //TODO: Implement validation using database here. Will need to add model for database.
    }
}
