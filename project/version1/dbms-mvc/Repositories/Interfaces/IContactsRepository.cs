namespace dbms_mvc.Repositories;

public interface IContactsRepository : IDisposable
{
    public Task<IEnumerable<Contact>> SearchContacts(Contact searchContact);

    public Task<Contact?> GetContactById(int? id);

    public Task AddContact(Contact contact);
}
