using Microsoft.EntityFrameworkCore;
using AutoFixture;
using dbms_mvc.Repositories;
using dbms_mvc.Data;

namespace dbms_mvc.Tests.Repositories;

[TestClass]
public class ContactsRepositoryTests
{
    private ApplicationDbContext _context;
    private Fixture _fixture;

    public ContactsRepositoryTests()
    {
        DbContextOptionsBuilder<ApplicationDbContext> dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(
        Guid.NewGuid().ToString()
        );

        _context = new ApplicationDbContext(dbOptions.Options);

        _fixture = new Fixture();
    }

    public async Task AddContact()
    {
        //Arrange
        IContactsRepository repository = new ContactsRepository(_context);

        Contact testContact = _fixture.Create<Contact>();


        //Act
        await repository.AddContact(testContact);

        //Assert
        Contact dbContact = await _context.contacts.FirstOrDefaultAsync();
        Assert.Equals(testContact, dbContact);
    }
}
