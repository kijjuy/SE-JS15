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

    [TestMethod]
    public async Task UpdateContact()
    {
        //Arrange
        CreateRepoAndContact(out IContactsRepository repository, out Contact testContact);

        await _context.AddAsync(testContact);
        await _context.SaveChangesAsync();

        string oldName = testContact.FirstName;
        testContact.FirstName = "changed";

        //Act
        await repository.UpdateContact(testContact);

        //Assert
        Contact dbContact = _context.contacts.Find(testContact.ContactId);
        var props = typeof(Contact).GetProperties().Where(p => p.Name != nameof(Contact.FirstName));

        Assert.IsNotNull(dbContact);
        Assert.AreNotEqual(oldName, dbContact.FirstName);
        foreach (var prop in props)
        {
            var testVal = prop.GetValue(testContact);
            var dbVal = prop.GetValue(dbContact);
            Assert.AreEqual(testVal, dbVal);
        }
    }

    [TestMethod]
    public async Task DeleteContact()
    {
        //Arrange
        CreateRepoAndContact(out IContactsRepository repository, out Contact testContact);

        await _context.AddAsync(testContact);
        await _context.SaveChangesAsync();

        //Act
        await repository.DeleteContact(testContact);

        //Assert
        Assert.AreEqual(_context.contacts.Count(), 0);
    }

    [TestMethod]
    public async Task ContactExists()
    {
        //Arrange
        CreateRepoAndContact(out IContactsRepository repository, out Contact testContact);

        await _context.AddAsync(testContact);
        await _context.SaveChangesAsync();

        int notContactId = testContact.ContactId + 1;

        //Act
        bool doesExist = repository.ContactExists(testContact.ContactId);
        bool doesNotExist = repository.ContactExists(notContactId);

        //Assert
        Assert.IsTrue(doesExist);
        Assert.IsFalse(doesNotExist);

    }

    [TestMethod]
    public async Task SearchContacts()
    {
        //Arrange
        CreateRepoAndContact(out IContactsRepository repository, out Contact testContact);
        var dbContacts = _fixture.CreateMany<Contact>(50);

        await _context.AddRangeAsync(dbContacts);
        await _context.SaveChangesAsync();

        Contact firstDbContact = dbContacts.FirstOrDefault();
        Contact partialMatchContact = new Contact()
        {
            FirstName = firstDbContact.FirstName,
            LastName = firstDbContact.LastName,
            Organization = "Not the same"
        };

        //Act
        var result_noMatch_emptyList = await repository.SearchContacts(testContact);
        var result_partialMatch_oneItemList = await repository.SearchContacts(partialMatchContact);
        var result_exactMatch_emptyList = await repository.SearchContacts(firstDbContact);

        //Assert
        Assert.AreEqual(result_noMatch_emptyList.Count(), 0);
        Assert.AreEqual(result_partialMatch_oneItemList.Count(), 1);
        Assert.AreEqual(result_exactMatch_emptyList.Count(), 0);
    }

    private void CreateRepoAndContact(out IContactsRepository repository, out Contact contact)
    {
        repository = new ContactsRepository(_context);
        contact = _fixture.Create<Contact>();
    }
}
