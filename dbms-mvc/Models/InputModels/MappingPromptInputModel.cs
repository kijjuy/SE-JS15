namespace dbms_mvc.Models;

public class MappingPromptInputModel
{
    public Guid FileId { get; set; }

    public Dictionary<string, string> ColumnMappings { get; set; }

    public MappingPromptInputModel()
    {
        ColumnMappings = new Dictionary<string, string>();
    }
}
