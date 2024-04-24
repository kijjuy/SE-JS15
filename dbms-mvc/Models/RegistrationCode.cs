using System.ComponentModel.DataAnnotations;

public class RegistrationCode
{
    //Amount of days to make the Registration Code valid for.
    private static int DurationValidDays = 3;

    [Key]
    public int RegistrationCodeId { get; set; }

    public Guid Token { get; set; }

    public DateTime Expiration { get; set; }

    public RegistrationCode()
    {
        Token = Guid.NewGuid();
        Expiration = CreateExpirationDate();
    }

    private DateTime CreateExpirationDate()
    {
        return DateTime.Now.AddDays(DurationValidDays);
    }
}
