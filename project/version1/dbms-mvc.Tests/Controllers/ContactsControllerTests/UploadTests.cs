using Microsoft.EntityFrameworkCore;
using AutoFixture;
using dbms_mvc.Controllers;
using dbms_mvc.Models;
using dbms_mvc.Data;
namespace dbms_mvc.Tests.Controllers;

[TestClass]
public class ContactController_UploadTests
{

    private static Fixture _fixture;
    private static ApplicationDbContext _context;

    [ClassInitialize]
    public static async Task Setup(TestContext testContext)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "testDb")
    .Options;
        _context = new ApplicationDbContext(options);

        _fixture = new Fixture();

        var contactList = _fixture.CreateMany<Contact>(50).ToList();
        await _context.AddRangeAsync(contactList);
        await _context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task GetDupeContacts_NotMatch_ReturnsEmptyList()
    {
        //Arrange

        var newContactInList = _fixture.CreateMany<Contact>().ToList();

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
        foreach (var contact in firstThreeContacts)
        {
            contact.Email = "Not Exact Match";
        }

        var controller = new ContactsController(_context);

        //Act
        var result = await controller.GetDupeContacts(firstThreeContacts);

        //Assert
        Assert.IsTrue(result.Count == 3, "Not all contacts matched that should have.");
    }

    [TestMethod]
    public async Task GetDupeContacts_IsExactMatch_ReturnsEmptyList()
    {
        //Arrange
        var firstThreeContacts = _context.contacts.Take(3).ToList();
        var controller = new ContactsController(_context);

        //Act
        var result = await controller.GetDupeContacts(firstThreeContacts);

        //Assert
        Assert.IsTrue(result.Count == 0, "Exact duplicates are being matched when they should not be.");
    }
}
