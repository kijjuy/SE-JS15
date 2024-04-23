namespace dbms_mvc.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public string? Message { get; set; }

    public bool ShowMessage => !string.IsNullOrEmpty(Message);

    public ErrorViewModel() { }

    public ErrorViewModel(string requestId, string message)
    {
        this.RequestId = requestId;
        this.Message = message;
    }
}
