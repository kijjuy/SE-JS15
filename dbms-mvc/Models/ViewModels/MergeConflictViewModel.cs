namespace dbms_mvc.Models;

public class MergeConflictViewModel
{

    public MergeConflictViewModel(Contact newContact, Contact dupeContact, string message)
    {
        NewContact = newContact;
        DupeContact = dupeContact;
        Message = message;
    }


    public Contact NewContact { get; set; }

    public Contact DupeContact { get; set; }

    public string Message { get; set; }

}
