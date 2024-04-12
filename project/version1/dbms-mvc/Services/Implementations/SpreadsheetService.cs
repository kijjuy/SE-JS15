using ClosedXML.Excel;
using ExcelDataReader;
using dbms_mvc.Models;

namespace dbms_mvc.Services;

/// <summary>
/// Implementation of ISpreadsheetService
/// </summary>
public class SpreadsheetService : ISpreadsheetService
{
    private readonly ILogger<SpreadsheetService> _logger;

    public SpreadsheetService(ILogger<SpreadsheetService> logger)
    {
        _logger = logger;
    }

    public IEnumerable<Contact> GetContactsFromFile(IFormFile file)
    {
        throw new NotImplementedException();
    }
}
