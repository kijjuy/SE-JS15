using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dbms_mvc.Data;
using ExcelDataReader;

namespace dbms_mvc.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contacts
        public async Task<IActionResult> Index()
        {
            return View(await _context.contacts.ToListAsync());
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.contacts
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        public async Task<IActionResult> Create()
        {
            List<MailingList> mailingLists = await _context.mailingLists.ToListAsync();
            ContactsViewModel viewModel = new ContactsViewModel
            {
                MailingLists = mailingLists
            };
            return View(viewModel);
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContactId,FirstName,LastName,Organization,Title,StreetAddress1,City,Province,PostalCode,Subscribed,Email,Phone,Fax,Website,BedsCount,Address2,Extension,MailingList")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContactId,FirstName,LastName,Organization,Title,StreetAddress1,City,Province,PostalCode,Subscribed,Email,Phone,Fax,Website,BedsCount,Address2,Extension")] Contact contact)
        {
            if (id != contact.ContactId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.ContactId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.contacts
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.contacts.FindAsync(id);
            if (contact != null)
            {
                _context.contacts.Remove(contact);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Upload()
        {
            return View();
        }


        [HttpPost, ActionName("Upload")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            GetFormData(file);
            return View(file);
        }

        private void GetFormData(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    reader.Read();
                    (string, List<string>)[] rows = new (string, List<string>)[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.WriteLine("data: " + reader.GetValue(i));
                        string key = reader.GetValue(i).ToString();
                        List<string> list = new List<string>();
                        rows[i] = new (key, list);
                    }
                    foreach(var tup in rows) {
                        Console.WriteLine(tup.ToString());
                    }
                    //while (reader.Read())
                    //{
                    //    for (int i = 0; i < reader.FieldCount; i++)
                    //    {
                    //        rows[i].
                    //        Console.WriteLine("data: " + reader.GetValue(i));
                    //    }
                    //}
                }
            }
        }

        private bool ContactExists(int id)
        {
            return _context.contacts.Any(e => e.ContactId == id);
        }
    }
}
