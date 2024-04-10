using dbms_mvc.Models;
namespace dbms_mvc.Repositories;

public interface IContactsRepository : IDisposable
{
    public Task<IEnumerable<Contact>> SearchContacts(Contact searchContact);
}
