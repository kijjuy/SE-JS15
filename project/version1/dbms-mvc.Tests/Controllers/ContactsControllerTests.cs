using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
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

    [TestMethod]
    public async Task Index()
    {
        //Arrange
        var controller = new ContactsController(_repository);

        Contact nullContact = null;
        Contact emptyContact = new Contact();
        Contact withDataContact = _fixture.Create<Contact>();

        //Act
        var result_null_view = await controller.Index(nullContact);
        var result_emptyContact_view = await controller.Index(emptyContact);
        var result_withDataContact_view = await controller.Index(withDataContact);

        //Assert
        Assert.IsInstanceOfType<ViewResult>(result_null_view);
        Assert.IsInstanceOfType<ViewResult>(result_emptyContact_view);
        Assert.IsInstanceOfType<ViewResult>(result_withDataContact_view);
    }
}
