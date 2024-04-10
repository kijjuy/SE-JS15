using AutoFixture;
using dbms_mvc.Controllers;
using dbms_mvc.Models;
using dbms_mvc.Data;
using dbms_mvc.Tests.Helper;
namespace dbms_mvc.Tests.Controllers;

[TestClass]
public class ContactController_UploadTests
{

    private static Fixture _fixture;
    private static ApplicationDbContext _context;

    [ClassInitialize]
    public static async Task Setup(TestContext testContext)
    {
        _fixture = new Fixture();

        _context = await TestHelper.GenerateContactsContext();
        //Assert.IsTrue(_context.contacts.Count() > 0, "Failed to initialize class. Db was empty");
    }

    [TestMethod]
    public async Task GetDupeContacts_NotMatch_ReturnsEmptyList()
    {
        //Arrange

        var newContact = _fixture.Create<Contact>();

        var newContactInList = new List<Contact>();
        newContactInList.Add(newContact);

        var controller = new ContactsController(_context);

        //Act
        var result = await controller.GetDupeContacts(newContactInList);

        //Assert
        Assert.IsTrue(result.Count == 0, "Contacts were found to have matched when they should not have.");
    }

    [TestMethod]
    public async Task GetDupeContacts_IsPartialMatch_ReturnList()
    {
        //Arrange
        var firstThreeContacts = _context.contacts.Take(3).ToList();
        var contactList = new List<Contact>();
        foreach (var contact in firstThreeContacts)
        {
            contactList.Add(
            new Contact
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Organization = contact.Organization
            }
                );
        }

        var controller = new ContactsController(_context);

        //Act
        var result = await controller.GetDupeContacts(contactList);

        //Assert
        Assert.IsTrue(result.Count == 3, "Not all contacts matched that should have.");
    }

    [TestMethod]
    public async Task GetDupeContacts_IsExactMatch_ReturnsEmptyList()
    {
        //Arrange
        var firstThreeContacts = _context.contacts.Take(3).ToList();
        var contactList = new List<Contact>();
        foreach (var contact in firstThreeContacts)
        {
            contactList.Add(
        new Contact
        {
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Organization = contact.Organization
        }
            );
        }
        var controller = new ContactsController(_context);

        //Act
        var result = await controller.GetDupeContacts(contactList);

        //Assert
        Assert.IsTrue(result.Count == 0, "Exact duplicates are being matched when they should not be.");
    }
}
