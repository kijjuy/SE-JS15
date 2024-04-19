using ClosedXML.Excel;
using dbms_mvc.Models;

namespace dbms_mvc.Services;

/// <summary>
/// Interface used for reading data from and creating new spreadsheets.
/// </summary>
public interface ISpreadsheetService
{
    public IXLWorksheet GetWorksheetFromFile(IFormFile file);

    public IEnumerable<Contact> GetContactsFromWorksheet(IXLWorksheet worksheet);

    public MappingPromptViewModel GetUnmappedColumnNames(IXLWorksheet worksheet);

    public byte[] CreateFileFromContacts(IEnumerable<Contact> contacts);
}
