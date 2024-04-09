using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.InMemory;
using AutoFixture;
using dbms_mvc.Controllers;
using dbms_mvc.Models;
using dbms_mvc.Data;
using dbms_mvc.Tests.Helper;
namespace dbms_mvc.Tests.Controllers;

[TestClass]
public class ContactsController_IndexTests
{
    private static ApplicationDbContext _context;

    [ClassInitialize]
    public static async Task Setup(TestContext testContext)
    {
        _context = await TestHelper.GenerateContactsContext();
    }

    [TestMethod]
    public async Task Index_NullContact_ReturnsView()
    {
        //Arrange
        Contact nullContact = null;

        var controller = new ContactsController(_context);

        //Act
        var result = await controller.Index(nullContact);

        Assert.IsInstanceOfType(result, typeof(IActionResult));
    }
}
