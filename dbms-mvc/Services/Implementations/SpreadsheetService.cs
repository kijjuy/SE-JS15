using System.Reflection;
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

        //TODO: refactor to use reflection and attributes
        var props = GetContactPropsWithoutId();

        for (int i = 0; i < props.Count(); i++)
        {
            string colName = GetColumnNameFromContactProp(props.ElementAt(i));
            worksheet.Cell(1, i + 1).Value = colName;
        }

        for (int i = 0; i < contacts.Count(); i++)
        {
            for (int j = 0; j < props.Count(); j++)
            {
                int rowNum = i + 2;
                int colNum = j + 1;
                var prop = props.ElementAt(j);
                Contact contact = contacts.ElementAt(i);

                var propVal = prop.GetValue(contact);
                string stringVal = propVal == null ? "" : propVal.ToString();

                worksheet.Cell(rowNum, colNum).Value = stringVal;
            }
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
        var props = GetContactPropsWithoutId();
        foreach (var prop in props)
        {

            string primaryName = GetColumnNameFromContactProp(prop);

            int colNum = colNames.IndexOf(primaryName);

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

    private IEnumerable<PropertyInfo> GetContactPropsWithoutId()
    {
        return typeof(Contact)
          .GetProperties().Where(p => p.Name != nameof(Contact.ContactId));
    }

    private string GetColumnNameFromContactProp(PropertyInfo prop)
    {
        var attributes = prop.GetCustomAttributes(false);
        var attribute = attributes
          .Where(a => a.GetType() == typeof(SpreadsheetColumnAttribute)).First() as SpreadsheetColumnAttribute;
        return attribute.PrimaryName;
    }
}
