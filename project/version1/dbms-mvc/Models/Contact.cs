using System.ComponentModel.DataAnnotations;

public class Contact
{

    //TODO: Do something with this commented code
    //    public Contact(string firstName, string lastName, string organization, string title, string streetAddress1, string city, string province, string postalCode, char subscribed, string email, string phone, MailingList mailingList)
    //    {
    //        this.FirstName = firstName;
    //        this.LastName = lastName;
    //        this.Organization = organization;
    //        this.Title = title;
    //        this.StreetAddress1 = streetAddress1;
    //        this.City = city;
    //        this.Province = province;
    //        this.PostalCode = postalCode;
    //        this.Subscribed  = subscribed;
    //        this.Email = email;
    //        this.Phone = phone;
    //        this.MailingList = mailingList;
    //    }

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
