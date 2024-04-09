using dbms_mvc.Data;
using dbms_mvc.Models;
using Microsoft.EntityFrameworkCore;
using AutoFixture;
namespace dbms_mvc.Tests.Helper;

public class TestHelper
{
    private ApplicationDbContext CreateAppDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "testDb")
    .Options;
        return new ApplicationDbContext(options);
    }

    private Fixture CreateFixture()
    {
        return new Fixture();
    }


    public async Task<ApplicationDbContext> GenerateContactsContext()
    {
        var context = CreateAppDbContext();
        var fixture = CreateFixture();
        var contactsList = fixture.CreateMany<Contact>(50).ToList();
        await context.contacts.AddRangeAsync(contactsList);
        return context;
    }
}
