using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using dbms_mvc.Models;

namespace dbms_mvc.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Contact> contacts { get; set; }

    public DbSet<MailingList> mailingLists { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Contact>().HasKey(c => c.ContactId);
        builder.Entity<MailingList>().HasKey(m => m.MailingListId);

        builder.Entity<Contact>()
            .HasOne(c => c.MailingList);
    }

}
