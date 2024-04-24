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

    public static string xlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public static string csvContentType = "text/csv";

    public static IEnumerable<string> SupportedContentTypes = new List<string>
        {
        xlsxContentType,
        csvContentType
        };

    public const string WorksheetName = "Sheet1";

    public SpreadsheetService(ILogger<SpreadsheetService> logger)
    {
        _logger = logger;
    }

    public IXLWorksheet GetWorksheetFromFile(IFormFile file)
    {
        using (var stream = file.OpenReadStream())
        {

            if (file.ContentType == SpreadsheetService.xlsxContentType)
            {
                var workbook = new XLWorkbook(stream);
                return workbook.Worksheet(SpreadsheetService.WorksheetName);
            }

            if (file.ContentType == SpreadsheetService.csvContentType)
            {
                //TODO: implement csv support
                var workbook = new XLWorkbook();
                var worksheet = workbook.AddWorksheet(SpreadsheetService.WorksheetName);
                using (var reader = new StreamReader(stream))
                {
                    var rows = reader.ReadToEnd().Split("\n");

                    List<List<string>> cells = new List<List<string>>();
                    foreach (var row in rows)
                    {
                        cells.Add(row.Split(",").ToList());
                    }

                    worksheet.Cell(1, 1).InsertTable(cells);
                    worksheet.Table(0).LastRow().Delete();
                    worksheet.Row(1).Delete();
                    worksheet.Rows().AdjustToContents();
                    workbook.SaveAs("workbook.xlsx");

                }
                return worksheet;
            }

            throw new NotImplementedException();
        }
    }

    public MappingPromptViewModel GetUnmappedColumnNames(IXLWorksheet worksheet)
    {
        MappingPromptViewModel mappingPrompt = new MappingPromptViewModel();
        var colNames = worksheet.FirstRow().Cells();
        var props = GetContactPropsWithoutId();
        var colsToSkip = new List<string>
        {
        "Combined Name"
        };
        foreach (var col in colNames)
        {
            string colString = col.GetString();
            if (colsToSkip.Contains(colString))
            {
                continue;
            }

            var matchingProp = GetMatchingProp(props, colString);

            if (matchingProp == null)
            {
                mappingPrompt.UnmappedColumns.Add(colString);
                continue;
            }
            props = props.Where(p => !p.Name.Equals(matchingProp.Name));
        }

        var availableProps = new List<string>();
        foreach (var prop in props)
        {
            if (prop != null)
            {
                string propTitle = prop.GetCustomAttribute<SpreadsheetColumnAttribute>().PrimaryName;
                availableProps.Add(propTitle);
            }
        }
        mappingPrompt.AvailableProperties = availableProps;
        return mappingPrompt;
    }

    public IXLWorksheet SetMappedColumns(IXLWorksheet worksheet, Dictionary<string, string> mappings)
    {
        var firstRow = worksheet.FirstRow();
        for (int i = 1; i < firstRow.CellCount(); i++)
        {
            var cell = firstRow.Cell(i);
            string cellVal = cell.GetValue<string>();
            if (mappings.Keys.Contains(cellVal))
            {
                string mapping = mappings[cellVal];
                cell.SetValue(mapping);
            }
        }
        return worksheet;
    }

    public IEnumerable<Contact> GetContactsFromWorksheet(IXLWorksheet worksheet)
    {
        var contacts = new List<Contact>();

        //Iterate over rows
        for (int row = 2; row <= worksheet.Rows().Count(); row++)
        {
            //Iterate over each col in rows
            Contact contact = new Contact();
            var props = contact.GetType().GetProperties();

            for (int col = 1; col <= worksheet.Columns().Count(); col++)
            {
                string colName = worksheet.Row(1).Cell(col).GetText();
                string colVal = worksheet.Row(row).Cell(col).GetValue<string>();

                var prop = props.FirstOrDefault(p =>
        {
            var attribute = p.GetCustomAttribute<SpreadsheetColumnAttribute>();
            return attribute != null && attribute.PrimaryName == colName;
        });
                if (prop == null || colVal == string.Empty)
                {
                    continue;
                }

                bool isInt = (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?));
                if (isInt && int.TryParse(colVal, out int intVal))
                {
                    prop.SetValue(contact, intVal);
                    continue;
                }

                prop.SetValue(contact, colVal);

            }
            contacts.Add(contact);
        }

        return contacts;
    }

    public byte[] CreateFileFromContacts(IEnumerable<Contact> contacts)
    {
        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Sheet1");

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

        MemoryStream memStream = new MemoryStream();
        workbook.SaveAs(memStream);

        workbook.Dispose();
        memStream.Dispose();

        return memStream.ToArray();

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

    private PropertyInfo? GetMatchingProp(IEnumerable<PropertyInfo> props, string matchName)
    {
        return props.FirstOrDefault(p =>
        {
            var attribute = p.GetCustomAttribute<SpreadsheetColumnAttribute>();
            return ((attribute != null) && attribute.PrimaryName == matchName);
        });

    }

    private IXLWorksheet CreateWorksheet()
    {
        var workbook = new XLWorkbook();
        return workbook.AddWorksheet(SpreadsheetService.WorksheetName);
    }
}
