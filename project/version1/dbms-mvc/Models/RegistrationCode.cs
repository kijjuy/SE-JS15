using System.ComponentModel.DataAnnotations;

public class RegistrationCode
{
    [Key]
    public int RegistrationCodeId { get; set; }

    public Guid Token { get; set; }

    public DateTime Expiration { get; set; }

    public RegistrationCode()
    {
        Token = new Guid();
        //TODO: Do something with this Expiration
        Expiration = DateTime.Now.AddDays(3);
    }
}
