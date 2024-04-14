using dbms_mvc.Models;

namespace dbms_mvc.Repositories;

public interface IContactsRepository : IDisposable
{
    public Task<IEnumerable<Contact>> SearchContacts(Contact searchContact);

    public Task<IEnumerable<MergeConflictViewModel>> GetUploadMergeConflicts(IEnumerable<Contact> newContacts);

    public Task<Contact?> GetContactById(int? id);
    public Task AddContact(Contact contact);
    public Task UpdateContact(Contact contact);
    public Task DeleteContact(Contact contact);
    public bool ContactExists(int id);
}
