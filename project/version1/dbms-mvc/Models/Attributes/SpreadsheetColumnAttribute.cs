using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public class SpreadsheetColumnAttribute : Attribute
{
    private string primaryName { get; set; }

    public SpreadsheetColumnAttribute(string columnName)
    {
        primaryName = columnName;
    }

    public virtual string PrimaryName
    {
        get { return primaryName; }
    }
}
