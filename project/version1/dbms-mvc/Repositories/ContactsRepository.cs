using Microsoft.EntityFrameworkCore;
using System.Reflection;
using dbms_mvc.Data;

namespace dbms_mvc.Repositories;

public class ContactsRepository : IContactsRepository, IDisposable
{
    private readonly ApplicationDbContext _context;

    public ContactsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    //Search Results section

    ///<summary>
    /// Gets the list of contacts based on the following possible
    /// states of <paramref name="searchContact"/>
    /// <list type="bullet">
    /// 	<item>
    /// 		<term>Null</term>
    /// 		<description>Returns all contacts</description>
    /// 	</item>
    /// 	<item>
    /// 		<term>All Props Empty/Null</term>
    /// 		<description>Returns all contacts</description>
    /// 	</item>
    /// 	<item>
    /// 		<term>At least one prop with data</term>
    /// 		<description>Returns contacts matching prop data</description>
    /// 	</item>
    /// </list>
    ///</summary>
    ///<param name="searchContact">Contact object to base search results on.</param>
    /// <returns>
    /// IEnumerable containing contacts from search results
    /// </returns>
    public async Task<IEnumerable<Contact>> SearchContacts(Contact? searchContact)
    {
        List<Contact> allContacts = await _context.contacts.ToListAsync();

        if (searchContact == null)
        {
            return allContacts;
        }

        if (ContactArePropsEmpty(searchContact))
        {
            return allContacts;
        }

        var searchResultContacts = GetSearchResultContacts(searchContact, allContacts);

        return searchResultContacts;
    }

    /// <summary>
    /// Gets the PropertyInfo of the Contact object minus Contact.ContactId.
    /// </summary>
    /// <returns>IEnumerable of properties of Contact object</returns>
    private IEnumerable<PropertyInfo> GetContactPropsWithoutId()
    {
        var props = typeof(Contact).GetProperties();
        return props.Where(p => p.Name != nameof(Contact.ContactId));
    }

    /// <summary>
    /// Checks each property of <paramref name="contact"/> to see if any of them contain data.
    /// </summary>
    /// <param name="contact">Contact object to check the properties of</param>
    /// <returns>bool: true if all props are empty. false if any prop contains data.</returns>
    private bool ContactArePropsEmpty(Contact contact)
    {
        var props = GetContactPropsWithoutId();
        bool propsAreAllEmpty = true;
        foreach (var prop in props)
        {
            var propVal = prop.GetValue(contact);
            if (propVal != null && !propVal.ToString().Equals(string.Empty))
            {
                propsAreAllEmpty = false;
                break;
            }
        }
        return propsAreAllEmpty;
    }

    /// <summary>
    /// Iterates through <paramref name="contacts"/> and checks each Contact prop against <paramref name="searchContact"/>
    /// inside of CheckContactMatch method to see if there is a match on the search.
    /// </summary>
    /// <param name="searchContact">Contact object to base search results on.</param>
    /// <param name="contacts">List of all contacts to get search results from</param>
    /// <returns>List of contacts that match all props of <paramref name="searchContact"/>.</returns>
    private IEnumerable<Contact> GetSearchResultContacts(Contact searchContact, IEnumerable<Contact> contacts)
    {
        List<Contact> matchingContacts = new List<Contact>();
        var props = GetContactPropsWithoutId();
        foreach (Contact contact in contacts)
        {
            bool isMatch = CheckContactMatch(searchContact, contact, props);
            if (isMatch)
            {
                matchingContacts.Add(contact);
            }
        }
        return matchingContacts;
    }

    /// <summary>
    /// Checks the props of <paramref name="dbContact"/> against props of <paramref name="searchContact"/>
    /// for each prop in <paramref name="props"/>.
    /// </summary>
    /// <param name="searchContact">The contact object to base the search on.</param>
    /// <param name="dbContact">Single contact to check against <paramref name="searchContact"/>.</param>
    /// <param name="props">List of reflection properties used to get values of contact objects.</param>
    private bool CheckContactMatch(Contact searchContact, Contact dbContact, IEnumerable<PropertyInfo> props)
    {
        bool isMatch = true;
        foreach (var prop in props)
        {
            if (!isMatch)
            {
                break;
            }

            var searchPropValue = prop.GetValue(searchContact);
            if (searchPropValue == null)
            {
                continue;
            }

            var dbPropValue = prop.GetValue(dbContact);
            if (dbPropValue == null)
            {
                isMatch = false;
                break;
            }

            string dbValStr = dbPropValue.ToString().ToLower();
            string searchValStr = searchPropValue.ToString().ToLower();
            bool dbValContainsSearchVal = dbValStr.Contains(searchValStr);
            if (dbPropValue == null || !dbValContainsSearchVal)
            {
                isMatch = false;
                break;
            }
        }
        return isMatch;
    }

    //Multi-Use
    public async Task<Contact?> GetContactById(int? id)
    {
        return await _context.contacts.FirstOrDefaultAsync(m => m.ContactId == id);
    }

    //IDisposable Section

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
    }
}
