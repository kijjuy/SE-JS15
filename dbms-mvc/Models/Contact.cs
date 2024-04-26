using System.ComponentModel.DataAnnotations;

public class Contact
{

    public override bool Equals(Object? obj)
    {
        if (obj.GetType() != this.GetType())
        {
            return false;
        }
        Contact checkContact = (Contact)obj;
        var props = typeof(Contact).GetProperties().Where(p => p.Name != nameof(Contact.ContactId));

        foreach (var prop in props)
        {
            var thisVal = prop.GetValue(this);
            var checkVal = prop.GetValue(checkContact);

            //Both null, skip
            if (thisVal == null && checkVal == null)
            {
                continue;
            }

            //One is null, not match
            if (thisVal == null || checkVal == null)
            {
                return false;
            }

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
    [SpreadsheetColumn("First")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [StringLength(25)]
    [SpreadsheetColumn("Name")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [StringLength(50)]
    [SpreadsheetColumn("Organization")]
    [Display(Name = "Organization")]
    public string Organization { get; set; }

    [Required]
    [StringLength(50)]
    [SpreadsheetColumn("Title")]
    [Display(Name = "Title")]
    public string Title { get; set; }

    [Required]
    [StringLength(50)]
    [SpreadsheetColumn("Address")]
    [Display(Name = "Address")]
    public string StreetAddress1 { get; set; }

    [Required]
    [StringLength(50)]
    [SpreadsheetColumn("City")]
    [Display(Name = "City")]
    public string City { get; set; }

    [Required]
    [StringLength(2)]
    [SpreadsheetColumn("Province")]
    [Display(Name = "Province")]
    public string Province { get; set; }

    [Required]
    [StringLength(7)]
    [SpreadsheetColumn("Postal Code")]
    [Display(Name = "Postal Code")]
    public string PostalCode { get; set; }

    [Required]
    [StringLength(1)]
    [SpreadsheetColumn("Subscribed Y/N")]
    [Display(Name = "Subscribed Y/N")]
    public string Subscribed { get; set; }

    [Required]
    [StringLength(50)]
    [SpreadsheetColumn("Emails")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(15)]
    [SpreadsheetColumn("Phone")]
    [Display(Name = "Phone")]
    public string Phone { get; set; }

    [SpreadsheetColumn("Home Category")]
    [Display(Name = "Home Category ")]
    public string? HomeCategory { get; set; }

    [StringLength(15)]
    [SpreadsheetColumn("Fax")]
    [Display(Name = "Fax")]
    public string? Fax { get; set; }

    [StringLength(50)]
    [SpreadsheetColumn("Website")]
    [Display(Name = "Website")]
    public string? Website { get; set; }

    [SpreadsheetColumn("Number of Beds")]
    [Display(Name = "Number of Beds")]
    public int? BedsCount { get; set; }

    [StringLength(50)]
    [SpreadsheetColumn("Address 2")]
    [Display(Name = "Address 2")]
    public string? Address2 { get; set; }

    [StringLength(4)]
    [SpreadsheetColumn("Extension")]
    [Display(Name = "Extension")]
    public string? Extension { get; set; }

    [Required]
    [SpreadsheetColumn("Mailing List")]
    [Display(Name = "Mailing List")]
    public string MailingList { get; set; }
}
