using System.ComponentModel.DataAnnotations;

public class RegistrationCode
{
    //Amount of days to make the Registration Code valid for.
    private static int DurationValidDays = 3;

    [Key]
    public int RegistrationCodeId { get; set; }

    public Guid Token { get; set; }

    public DateTime Expiration { get; set; }

    //TODO: Add email of user that created RegistrationCode

    public RegistrationCode()
    {
        Token = Guid.NewGuid();
        //TODO: Do something with this Expiration
        Expiration = CreateExpirationDate();
    }

    private DateTime CreateExpirationDate()
    {
        return DateTime.Now.AddDays(DurationValidDays);
    }
}
