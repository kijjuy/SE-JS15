using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using AutoFixture;
using dbms_mvc.Controllers;
using dbms_mvc.Models;
using dbms_mvc.Repositories;
using dbms_mvc.Data;
using dbms_mvc.Services;


[TestClass]
public class ContactsControllerTests
{
    private readonly Fixture _fixture;
    private readonly IContactsRepository _repository;
    private readonly ISpreadsheetService _service;
    private readonly IMemoryCache _cache;

    public ContactsControllerTests()
    {
        DbContextOptionsBuilder<ApplicationDbContext> dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(
        Guid.NewGuid().ToString()
        );
        var contact = new ApplicationDbContext(dbOptions.Options);

        _fixture = new Fixture();
        _repository = new ContactsRepository(contact);
        _service = new SpreadsheetService(null);
        var cacheOptions = new MemoryCacheOptions();
        _cache = new MemoryCache(cacheOptions);
    }

    [TestMethod]
    public async Task Index()
    {
        //Arrange
        CreateControllerAndContact(out var controller, out Contact withDataContact);
        Contact nullContact = null;
        Contact emptyContact = new Contact();

        //Act
        var result_null_view = await controller.Index(nullContact, null);
        var result_emptyContact_view = await controller.Index(emptyContact, null);
        var result_withDataContact_view = await controller.Index(withDataContact, null);

        //Assert
        Assert.IsInstanceOfType<ViewResult>(result_null_view);
        Assert.IsInstanceOfType<ViewResult>(result_emptyContact_view);
        Assert.IsInstanceOfType<ViewResult>(result_withDataContact_view);
    }

    [TestMethod]
    public async Task Details()
    {
        //Arrange
        CreateControllerAndContact(out var controller, out Contact dbContact);

        await _repository.AddContact(dbContact);

        int id = dbContact.ContactId;

        //Act
        var result_nullId_notFound = await controller.Details(null);
        var result_noMatch_notFound = await controller.Details(id + 1);
        var result_match_view = await controller.Details(id);

        //Assert
        Assert.IsInstanceOfType<NotFoundResult>(result_nullId_notFound);
        Assert.IsInstanceOfType<NotFoundResult>(result_noMatch_notFound);
        Assert.IsInstanceOfType<ViewResult>(result_match_view);
    }

    [TestMethod]
    public async Task Create()
    {
        //Arrage
        CreateControllerAndContact(out var controller, out Contact validContact);
        Contact invalidContact = new Contact();

        //Act
        var result_validContact_redirect = await controller.Create(validContact);

        //Assert
        Assert.IsInstanceOfType<RedirectToActionResult>(result_validContact_redirect);
    }

    [TestMethod]
    public async Task EditView()
    {
        //Arrange
        CreateControllerAndContact(out var controller, out Contact dbContact);

        await _repository.AddContact(dbContact);

        //Act
        var result_idNull_notFound = await controller.Edit(null);
        var result_idNotMatch_notFound = await controller.Edit(dbContact.ContactId + 1);
        var result_idMatch_view = await controller.Edit(dbContact.ContactId);

        //Assert
        Assert.IsInstanceOfType<NotFoundResult>(result_idNull_notFound);
        Assert.IsInstanceOfType<NotFoundResult>(result_idNotMatch_notFound);
        Assert.IsInstanceOfType<ViewResult>(result_idMatch_view);
    }

    [TestMethod]
    public async Task EditConfirmed()
    {
        //Arrange
        CreateControllerAndContact(out var controller, out Contact dbContact);
        Contact nonDbContact = _fixture.Create<Contact>();

        await _repository.AddContact(dbContact);

        string origContactName = dbContact.FirstName;
        dbContact.FirstName = "Not the same";

        //Act
        var result_idNotMatch_notFound = await controller.Edit(dbContact.ContactId + 1, dbContact);
        var result_nonDbContact_notFound = await controller.Edit(nonDbContact.ContactId, nonDbContact);

        //Assert
        Assert.IsInstanceOfType<NotFoundResult>(result_idNotMatch_notFound);
        Assert.IsInstanceOfType<NotFoundResult>(result_nonDbContact_notFound);
    }

    [TestMethod]
    public async Task DeleteView()
    {
        //Arrange
        CreateControllerAndContact(out var controller, out Contact dbContact);

        await _repository.AddContact(dbContact);

        //Act
        var result_idNotMatch_notFound = await controller.Delete(dbContact.ContactId + 1);
        var result_idMatch_view = await controller.Delete(dbContact.ContactId);

        //Assert
        Assert.IsInstanceOfType<NotFoundResult>(result_idNotMatch_notFound);
        Assert.IsInstanceOfType<ViewResult>(result_idMatch_view);
    }

    [TestMethod]
    public async Task DeleteConfirmed()
    {
        //Arrange
        CreateControllerAndContact(out var controller, out Contact dbContact);

        await _repository.AddContact(dbContact);

        //Act
        var result_idNotMatch_redirect = await controller.DeleteConfirmed(dbContact.ContactId + 1);
        var result_validContact_redirect = await controller.DeleteConfirmed(dbContact.ContactId);

        //Assert
        Assert.IsInstanceOfType<RedirectToActionResult>(result_idNotMatch_redirect);
        Assert.IsInstanceOfType<RedirectToActionResult>(result_validContact_redirect);
    }

    private void CreateControllerAndContact(out ContactsController controller, out Contact contact)
    {
        controller = new ContactsController(_repository, _service, _cache, null);
        contact = _fixture.Create<Contact>();
    }
}
