using Microsoft.EntityFrameworkCore;
using AutoFixture;
using dbms_mvc.Repositories;
using dbms_mvc.Data;

namespace dbms_mvc.Tests.Repositories;

[TestClass]
public class ContactsRepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly Fixture _fixture;

    public ContactsRepositoryTests()
    {
        DbContextOptionsBuilder<ApplicationDbContext> dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(
        Guid.NewGuid().ToString()
        );
        _context = new ApplicationDbContext(dbOptions.Options);

        _fixture = new Fixture();
    }

    [TestMethod]
    public async Task AddContact()
    {
        //Arrange
        CreateRepoAndContact(out var repository, out var testContact);

        //Act
        await repository.AddContact(testContact);

        //Assert
        Contact dbContact = await _context.contacts.FirstOrDefaultAsync();

        Assert.IsNotNull(dbContact);
        Assert.AreEqual(testContact, dbContact);
        Assert.AreEqual(_context.contacts.Count(), 1);
    }

    [TestMethod]
    public async Task GetContactById()
    {
        //Arrange
        CreateRepoAndContact(out IContactsRepository repository, out Contact testContact);

        _context.contacts.Add(testContact);
        await _context.SaveChangesAsync();

        //Act
        Contact result = await repository.GetContactById(testContact.ContactId);

        //Assert
        Assert.AreEqual(testContact, result);
        Assert.AreEqual(_context.contacts.Count(), 1);
    }

    private void CreateRepoAndContact(out IContactsRepository repository, out Contact contact)
    {
        repository = new ContactsRepository(_context);
        contact = _fixture.Create<Contact>();
    }
}
