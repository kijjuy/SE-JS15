using System.ComponentModel.DataAnnotations;
using System.Reflection;

public class Contact
{

    public override bool Equals(Object? obj)
    {
        if (obj.GetType != this.GetType)
        {
            return false;
        }
        Contact checkContact = (Contact)obj;
        var props = typeof(Contact).GetProperties().Where(p => p.Name != nameof(Contact.ContactId));

        foreach (var prop in props)
        {
            var thisVal = prop.GetValue(this);
            var checkVal = prop.GetValue(checkContact);

            if (!thisVal.Equals(checkVal))
            {
                return false;
            }
        }
        return true;
    }

    [Key]
    public int ContactId { get; set; }

    [Required]
    [StringLength(25)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(25)]
    public string LastName { get; set; }

    [Required]
    [StringLength(50)]
    public string Organization { get; set; }

    [Required]
    [StringLength(25)]
    public string Title { get; set; }

    [Required]
    [StringLength(50)]
    public string StreetAddress1 { get; set; }

    [Required]
    [StringLength(25)]
    public string City { get; set; }

    [Required]
    [StringLength(2)]
    public string Province { get; set; }

    [Required]
    [StringLength(6)]
    public string PostalCode { get; set; }

    [Required]
    [StringLength(1)]
    public string Subscribed { get; set; }

    [Required]
    [StringLength(50)]
    public string Email { get; set; }

    [Required]
    [StringLength(11)]
    public string Phone { get; set; }

    public string? HomeCategory { get; set; }

    [StringLength(11)]
    public string? Fax { get; set; }

    [StringLength(50)]
    public string? Website { get; set; }

    public int? BedsCount { get; set; }

    [StringLength(50)]
    public string? Address2 { get; set; }

    [StringLength(4)]
    public string? Extension { get; set; }

    [Required]
    public string MailingList { get; set; }
}
