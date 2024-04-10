using Microsoft.EntityFrameworkCore;
using System.Reflection;
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

    [TestMethod]
    public async Task Index_MatchingContact_ReturnsView()
    {
        //Arrange
        Contact matchingContact = _context.contacts.FirstOrDefault();

        var controller = new ContactsController(_context);

        //Act
        var result = await controller.Index(matchingContact);

        //Assert
        Assert.IsInstanceOfType(result, typeof(IActionResult));
    }

    [TestMethod]
    public void GetMatchingContacts_NotMatch_ReturnsEmptyList()
    {
        //Arrange
        Contact nonMatchingContact = new Contact
        {
            FirstName = Guid.NewGuid().ToString()
        };

        var allContacts = _context.contacts.ToList();

        var props = typeof(Contact).GetProperties();

        var controller = new ContactsController(_context);

        //Act
        var result = controller.GetMatchingContacts(nonMatchingContact, allContacts, props);

        //Assert
        Assert.AreEqual(result.Count(), 0, "Non-matching contacts are being returned");
    }

    [TestMethod]
    public void GetMatchingContacts_IsMatch_ReturnPopulatedList()
    {
        //Arrange
        Contact matchingContactFromDB = _context.contacts.FirstOrDefault();
        string contactName = matchingContactFromDB.FirstName;
        Contact matchingContact = new Contact
        {
            FirstName = contactName
        };

        var allContacts = _context.contacts.ToList();

        var props = typeof(Contact).GetProperties();

        var controller = new ContactsController(_context);

        //Act
        var result = controller.GetMatchingContacts(matchingContact, allContacts, props);

        //Assert
        Assert.IsTrue(result.Count() >= 1, "Matching contacts are not being returned");
    }
}
