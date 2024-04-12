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
        var contacts = new List<Contact>();
        var steam = file.OpenReadStream();
        var reader = ExcelReaderFactory.CreateReader(steam);

        reader.Read();

        var cols = GetRow(reader);

        while (reader.Read())
        {
            var row = GetRow(reader);
            Contact newContact = CreateContactFromRow(cols, row);
            contacts.Add(newContact);
        }
        return contacts;
    }

    public byte[] CreateFileFromContacts(IEnumerable<Contact> contacts)
    {
        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Sheet1");

        worksheet.Cell(1, 1).Value = "FirstName";
        worksheet.Cell(1, 2).Value = "LastName";
        worksheet.Cell(1, 3).Value = "Organization";
        worksheet.Cell(1, 4).Value = "Title";
        worksheet.Cell(1, 5).Value = "StreetAddress";
        worksheet.Cell(1, 6).Value = "City";
        worksheet.Cell(1, 7).Value = "Province";
        worksheet.Cell(1, 8).Value = "PostalCode";
        worksheet.Cell(1, 9).Value = "Subscribed";
        worksheet.Cell(1, 10).Value = "Email";
        worksheet.Cell(1, 11).Value = "Phone";
        worksheet.Cell(1, 12).Value = "Fax";
        worksheet.Cell(1, 13).Value = "Website";
        worksheet.Cell(1, 14).Value = "BedsCount";
        worksheet.Cell(1, 15).Value = "Address2";
        worksheet.Cell(1, 16).Value = "Extension";
        worksheet.Cell(1, 17).Value = "MailingList";

        for (int i = 0; i < contacts.Count(); i++)
        {
            worksheet.Cell(i + 2, 1).Value = contacts.ElementAt(i).FirstName;
            worksheet.Cell(i + 2, 2).Value = contacts.ElementAt(i).LastName;
            worksheet.Cell(i + 2, 3).Value = contacts.ElementAt(i).Organization;
            worksheet.Cell(i + 2, 4).Value = contacts.ElementAt(i).Title;
            worksheet.Cell(i + 2, 5).Value = contacts.ElementAt(i).StreetAddress1;
            worksheet.Cell(i + 2, 6).Value = contacts.ElementAt(i).City;
            worksheet.Cell(i + 2, 7).Value = contacts.ElementAt(i).Province;
            worksheet.Cell(i + 2, 8).Value = contacts.ElementAt(i).PostalCode;
            worksheet.Cell(i + 2, 9).Value = contacts.ElementAt(i).Subscribed;
            worksheet.Cell(i + 2, 10).Value = contacts.ElementAt(i).Email;
            worksheet.Cell(i + 2, 11).Value = contacts.ElementAt(i).Phone;
            worksheet.Cell(i + 2, 12).Value = contacts.ElementAt(i).Fax;
            worksheet.Cell(i + 2, 13).Value = contacts.ElementAt(i).Website;
            worksheet.Cell(i + 2, 14).Value = contacts.ElementAt(i).BedsCount;
            worksheet.Cell(i + 2, 15).Value = contacts.ElementAt(i).Address2;
            worksheet.Cell(i + 2, 16).Value = contacts.ElementAt(i).Extension;
            worksheet.Cell(i + 2, 17).Value = contacts.ElementAt(i).MailingList;
        }

        using (MemoryStream memStream = new MemoryStream())
        {
            workbook.SaveAs(memStream);
            return memStream.ToArray();
        }
    }

    private IList<string> GetRow(IExcelDataReader reader)
    {
        var cols = new List<string>();
        for (int i = 0; i < reader.FieldCount; i++)
        {
            var val = reader.GetValue(i) ?? string.Empty;
            cols.Add(val.ToString());
        }
        return cols;
    }

    private Contact CreateContactFromRow(IList<string> colNames, IList<string> row)
    {
        Contact contact = new Contact();
        var props = contact.GetType().GetProperties().Where(p => p.Name != nameof(Contact.ContactId));
        foreach (var prop in props)
        {
            var attributes = prop.GetCustomAttributes(false);
            var attr = attributes.Where(a => a.GetType() == typeof(SpreadsheetColumnAttribute))
             .First() as SpreadsheetColumnAttribute;


            int colNum = colNames.IndexOf(attr.PrimaryName);

            if (colNum == -1)
            {
                //TODO: resolve mismatch colName here
                continue;
            }

            var rowVal = row.ElementAt(colNum);

            if (rowVal.Equals(string.Empty))
            {
                continue;
            }

            if (int.TryParse(rowVal, out int result))
            {
                prop.SetValue(contact, result);
                continue;
            }

            prop.SetValue(contact, rowVal);
        }
        return contact;
    }
}
