using AutoFixture;
using dbms_mvc.Services;

namespace dbms_mvc.Tests.Services;

[TestClass]
public class SpreadsheetServiceTests
{
    private readonly ISpreadsheetService _spreadsheetService;
    private readonly IFixture _fixture;

    public SpreadsheetServiceTests()
    {
        _spreadsheetService = new SpreadsheetService(null);

        _fixture = new Fixture();
    }

    [TestMethod]
    public void GetContactsFromFile()
    {
        //Arrange
        var contacts = _fixture.CreateMany<Contact>();
        var contact = _fixture.Create<Contact>();

        var file = File.OpenRead("test-spreadsheet-all-valid.xlsx");
    }

}
