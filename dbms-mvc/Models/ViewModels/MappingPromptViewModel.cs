namespace dbms_mvc.Models;

public class MappingPromptViewModel
{
    public List<string> UnmappedColumns { get; set; }

    public List<string> AvailableProperties { get; set; }

    public Guid FileId { get; set; }

    public MappingPromptInputModel InputModel { get; set; }

    public MappingPromptViewModel()
    {
        UnmappedColumns = new List<string>();
        AvailableProperties = new List<string>();
        InputModel = new MappingPromptInputModel();
    }
}
