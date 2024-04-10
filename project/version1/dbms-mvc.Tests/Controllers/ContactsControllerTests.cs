using Microsoft.EntityFrameworkCore;
using AutoFixture;
using dbms_mvc.Controllers;
using dbms_mvc.Models;
using dbms_mvc.Repositories;
using dbms_mvc.Data;


[TestClass]
public class ContactsControllerTests
{
    private readonly Fixture _fixture;
    private readonly IContactsRepository _repository;

    public ContactsControllerTests()
    {
        DbContextOptionsBuilder<ApplicationDbContext> dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(
        Guid.NewGuid().ToString()
        );
        var contact = new ApplicationDbContext(dbOptions.Options);

        _fixture = new Fixture();
        _repository = new ContactsRepository(contact);
    }
}
