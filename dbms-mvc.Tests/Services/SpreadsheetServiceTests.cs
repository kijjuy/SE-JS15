using Microsoft.AspNetCore.Http;
using AutoFixture;
using dbms_mvc.Services;

namespace dbms_mvc.Tests.Services;

[TestClass]
public class SpreadsheetServiceTests
{
    private readonly IFixture _fixture;

    public SpreadsheetServiceTests()
    {
        _fixture = new Fixture();
    }

    [TestMethod]
    public void GetContactsFromFile()
    {
        //Arrange
        var spreadsheetService = new SpreadsheetService(null);
        var contacts = _fixture.CreateMany<Contact>();
        var contact = _fixture.Create<Contact>();

        Stream stream = File.OpenRead("Files/test-spreadsheet-all-valid.xlsx");
    }

}
