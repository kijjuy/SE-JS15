using Microsoft.EntityFrameworkCore;
using System.Reflection;
using dbms_mvc.Data;
using dbms_mvc.Models;

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

    //Upload Section

    /// <summary>
    /// Takes a list of <paramref name="newContacts"/> not in the database and checks if
    /// any of them match the <c>Contact.FirstName</c> and <c>Contact.LastName</c> of all contacts 
    /// in the database.
    /// </summary>
    /// <param name="newContacts">IEnumerable of new contacts not yet in the database</param>
    /// <returns>
    /// IEnumerable of all contacts that invoked a merge conflict.
    /// </returns>
    public async Task<IEnumerable<MergeConflictViewModel>> GetUploadMergeConflicts(IEnumerable<Contact> newContacts)
    {
        List<MergeConflictViewModel> unresolvedMerges = new List<MergeConflictViewModel>();

        foreach (Contact newContact in newContacts)
        {
            if (newContact.FirstName == "")
            {
                continue;
            }

            Contact dupeContact = await CheckContactNamesMatch(newContact);

            //Dupe doesn't exist, can add
            if (dupeContact == null)
            {
                //Log add
                await this.AddContact(newContact);
                continue;
            }

            {
            }

            string message = "There is already a contact with that name.";
            unresolvedMerges.Add(new MergeConflictViewModel(newContact, dupeContact, message));
            //Log Here
        }

        return unresolvedMerges;
    }

    /// <summary>
    /// Gets the <c>Contact</c> that matches the <c>Contact.FirstName</c> and <c>Contact.LastName</c>
    /// of <paramref name="newContact"/>.
    /// </summary>
    /// <param name="newContact"><c>Contact</c> to check the FirstName and LastName of.</param>
    /// <returns>
    /// <c>Contact</c> matching <paramref name="newContact"/> or <see langword="null"/>.
    /// </returns>
    private async Task<Contact?> CheckContactNamesMatch(Contact newContact)
    {
        return await _context.contacts
            .Where(c => c.FirstName == newContact.FirstName && c.LastName == newContact.LastName)
            .FirstOrDefaultAsync();
    }

    //Multi-Use

    /// <summary>
    /// Returns a contact based on its ID.
    /// </summary>
    /// <param name="id">id of the contact to find.</param>
    /// <returns>Contact found by <paramref name="id"/> or <see langword="null"/>.</returns>
    public async Task<Contact?> GetContactById(int? id)
    {
        return await _context.contacts.FirstOrDefaultAsync(m => m.ContactId == id);
    }

    /// <summary>
    /// Adds a contact to the context and saves the changes.
    /// </summary>
    /// <param name="contact">The contact to add to the database</param>
    /// <exception cref="ArgumentNullException">Thrown when null <paramref name="contact"/> is passed.</exception>
    public async Task AddContact(Contact contact)
    {
        if (contact == null)
        {
            throw new ArgumentNullException("ContactsRepository.AddContact must be passed a non-null value");
        }
        await _context.contacts.AddAsync(contact);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates the contact in the database with the same <paramref name="contact"/>.ContactId
    /// to new version of <paramref name="contact"/>.
    /// </summary>
    public async Task UpdateContact(Contact contact)
    {
        _context.Update(contact);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Removes <paramref name="contact"/> from the database.
    /// </summary>
    /// <param name="contact">Contact to remove from the database.</param>
    public async Task DeleteContact(Contact contact)
    {
        _context.Remove(contact);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if contact with <paramref name="id"/> exists.
    /// </summary>
    /// <param name="id">id used to check contact</param>
    /// <returns>
    ///	bool: True if exists, False if doesn't.
    /// </returns>
    public bool ContactExists(int id)
    {
        return _context.contacts.Any(e => e.ContactId == id);
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
