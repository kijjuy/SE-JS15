using Microsoft.AspNetCore.Http;
using AutoFixture;
using Moq;
using ClosedXML.Excel;
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
    [DataRow("Files/test-spreadsheet-all-valid.xlsx")]
    [DataRow("Files/test-spreadsheet-empty.xlsx")]
    [DataRow("Files/test-spreadsheet-invalid-col-names.xlsx")]
    public void GetWorksheetFromFile(string fileDir)
    {
        //Arrange
        var spreadsheetService = new SpreadsheetService(null);
        var mockFormFile = new Mock<IFormFile>();

        Stream stream = File.OpenRead(fileDir);

        mockFormFile.Setup(f => f.OpenReadStream()).Returns(stream);
        mockFormFile.Setup(f => f.ContentType).Returns(SpreadsheetService.xlsxContentType);

        //Act
        var result = spreadsheetService.GetWorksheetFromFile(mockFormFile.Object);

        //Assert
        Assert.IsInstanceOfType<IXLWorksheet>(result);
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
