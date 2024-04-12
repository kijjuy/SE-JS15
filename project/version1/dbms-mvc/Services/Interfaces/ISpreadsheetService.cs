namespace dbms_mvc.Services;

/// <summary>
/// Interface used for reading data from and creating new spreadsheets.
/// </summary>
public interface ISpreadsheetService
{
    public IEnumerable<Contact> GetContactsFromFile(IFormFile file);

    public byte[] CreateFileFromContacts(IEnumerable<Contact> contacts);
}
