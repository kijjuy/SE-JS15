using System.ComponentModel.DataAnnotations;

public class MailingList
{

//    public MailingList(string fullName)
//    {
//        this.FullName = fullName;
//        this.CreationDate = DateTime.Now;
//    }

    [Key]
    public int MailingListId { get; set; }

    [Required]
    public string FullName { get; set; }

    [StringLength(10)]
    public string? ShortenedName { get; set; }

    public DateTime CreationDate { get; set; }
}
