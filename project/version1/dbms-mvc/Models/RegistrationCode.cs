using System.ComponentModel.DataAnnotations;

public class RegistrationCode
{
    [Key]
    public int RegistrationCodeId { get; set; }

    public Guid Token { get; set; }

    public DateTime Expiration { get; set; }

    //TODO: Add email of user that created RegistrationCode

    public RegistrationCode()
    {
        Token = Guid.NewGuid();
        //TODO: Do something with this Expiration
        Expiration = DateTime.Now.AddDays(3);
    }
}
