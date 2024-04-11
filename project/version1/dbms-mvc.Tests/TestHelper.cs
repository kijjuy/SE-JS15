using dbms_mvc.Data;
using dbms_mvc.Models;
using Microsoft.EntityFrameworkCore;
using AutoFixture;
namespace dbms_mvc.Tests.Helper;

public class TestHelper
{
    private static ApplicationDbContext CreateAppDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "testDb")
    .Options;
        return new ApplicationDbContext(options);
    }

    private static Fixture CreateFixture()
    {
        return new Fixture();
    }


    public static async Task<ApplicationDbContext> GenerateContactsContext()
    {
        var context = CreateAppDbContext();
        var fixture = CreateFixture();
        var contactsList = fixture.CreateMany<Contact>(50).ToList();
        await context.contacts.AddRangeAsync(contactsList);
        await context.SaveChangesAsync();
        return context;
    }

    public static void CleanupDatabase(ApplicationDbContext context)
    {
        context.DisposeAsync();
    }
}
