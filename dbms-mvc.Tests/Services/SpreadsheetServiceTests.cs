using Microsoft.AspNetCore.Http;
using System.Reflection;
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

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void GetUnmappedColumnNames(bool createValidData)
    {
        //Arrange
        var service = new SpreadsheetService(null);
        IXLWorkbook workbook = new XLWorkbook();
        IXLWorksheet worksheet = workbook.AddWorksheet();

        if (createValidData)
        {
            CreateValidWorksheet(ref worksheet);
        }
        else
        {
            CreateInvalidWorksheet(ref worksheet);
        }

        //Act
        var result = service.GetUnmappedColumnNames(worksheet);

	//Assert
        if (createValidData)
        {
            Assert.AreEqual(0, result.UnmappedColumns.Count());
            Assert.AreEqual(0, result.AvailableProperties.Count());
        }
        else
        {
            int propsCount = typeof(Contact).GetProperties().Count() - 1;
            Assert.AreEqual(15, result.UnmappedColumns.Count());
            Assert.AreEqual(propsCount, result.AvailableProperties.Count());
        }
    }

    private void CreateValidWorksheet(ref IXLWorksheet worksheet)
    {
        var props = typeof(Contact).GetProperties();
        for (int i = 1; i <= props.Count(); i++)
        {
            var prop = props.ElementAt(i - 1);
            if (prop.Name == nameof(Contact.ContactId))
            {
                continue;
            }

            string attrName = prop.GetCustomAttribute<SpreadsheetColumnAttribute>().PrimaryName;
            worksheet.Row(1).Cell(i).SetValue(attrName);
        }
    }

    private void CreateInvalidWorksheet(ref IXLWorksheet worksheet)
    {
        int rowsToMake = 15;

        for (int i = 1; i <= rowsToMake; i++)
        {
            worksheet.Row(1).Cell(i).SetValue($"Value {i}");
        }
    }
}
