using System.ComponentModel.DataAnnotations;
using dbms_mvc.Data;

public class RegistrationTokenAttribute : ValidationAttribute
{
    private readonly int _length = 8;

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        string regTokenStr = value as string;
        if (regTokenStr == null)
        {
            return new ValidationResult("Registration Token must be a string.");
        }

        if (regTokenStr.Length != new Guid().ToString().Length)
        {
            return new ValidationResult($"{regTokenStr} is not a valid registration code.");
        }

        return ValidationResult.Success;

        //TODO: Implement validation using database here. Will need to add model for database.
    }
}
