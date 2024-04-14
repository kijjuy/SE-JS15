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

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }

    [TestMethod]
    public void GetContactsFromFile()
    {
        //Arrange
        var spreadsheetService = new SpreadsheetService(null);

        Stream validStream = File.OpenRead("Files/test-spreadsheet-all-valid.xlsx");
        Stream emptyStream = File.OpenRead("Files/test-spreadsheet-empty.xlsx");
        Stream invalidColNameStreat = File.OpenRead("Files/test-spreadsheet-invalid-col-names.xlsx");

        //Act
        var result_allValid_fullList = spreadsheetService.GetContactsFromFile(validStream);
        var result_emptyFile_emptyList = spreadsheetService.GetContactsFromFile(emptyStream);
        var result_invalidColNames = spreadsheetService.GetContactsFromFile(invalidColNameStreat);

        //Assert
        Assert.AreEqual(3, result_allValid_fullList.Count());
        Assert.AreEqual(0, result_emptyFile_emptyList.Count());
        Assert.AreEqual(0, result_invalidColNames.Count());
    }

    [TestMethod]
    public void CreateFileFromContacts()
    {
        //Arrange
        var spreadsheetService = new SpreadsheetService(null);
        var contacts = _fixture.CreateMany<Contact>(50);
        var emptyContacts = new List<Contact>();

        //Act
        var result = spreadsheetService.CreateFileFromContacts(contacts);
        var result_empty = spreadsheetService.CreateFileFromContacts(emptyContacts);

        //Assert
        Stream stream = new MemoryStream(result);

        Assert.AreNotEqual(null, result);
    }

}
