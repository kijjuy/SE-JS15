using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dbms_mvc.Data;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;

namespace dbms_mvc.Controllers
{
    [Authorize]
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contacts
        public async Task<IActionResult> Index(string searchString)
        {
            var contacts = from m in _context.contacts
                           select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                contacts = contacts.Where(s => s.FirstName.Contains(searchString)
                                               || s.LastName.Contains(searchString)
                                               || s.Email.Contains(searchString));
            }

            return View(await contacts.ToListAsync());
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
        [Authorize(Roles = "create, admin")]
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
        [Authorize(Roles = "create, admin")]
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
        [Authorize(Roles = "update, admin")]
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
        [Authorize(Roles = "update, admin")]
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
        [Authorize(Roles = "delete, admin")]
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
        [Authorize(Roles = "delete, admin")]
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

        [Authorize(Roles = "upload, admin")]
        public async Task<IActionResult> Upload()
        {
            return View();
        }


        /// Method that is called when the upload button is clicked
        [HttpPost, ActionName("Upload")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "upload, admin")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            GetFormData(file);
            return View(file);
        }

        // Main function that handles logic
        private void GetFormData(IFormFile file)
        {
            //convert file to stream
            using (var stream = file.OpenReadStream())
            {
                //use 3rd party library to read file stream
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    reader.Read(); //Reads the first row and stores all of the data
                    
                    //each column from the spreadsheet is stored as a tuple with the first element being
                    //the name of the column. The second element is a string list that stores all of the proceeding
                    //column values.
                    (string, List<string>)[] rows = new (string, List<string>)[reader.FieldCount];

                    //Initialize each tuple with column name and empty list
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string key = reader.GetValue(i).ToString(); //get coluumn name
                        List<string> list = new List<string>();
                        rows[i] = new(key, list);
                    }

                    //Read remaining data one row at a time
                    do {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string val;
                            try //get value of column if it exists
                            {
                                val = reader.GetValue(i).ToString();
                            }
                            catch (Exception e) // set as empty string if value is empty
                            {
                                Console.WriteLine(e.Message);
                                val = "";
                            }
                            rows[i].Item2.Add(val); //add value to list under column name
                        }
                    }
                    while (reader.Read()); // loop while there is more data to read in spreadsheet

                    //Create contact objects based on data gathered
                    for (int i = 1; i < rows[0].Item2.Count(); i++)
                    {
                        Contact contact = new Contact();
                        foreach (var row in rows)
                        {
                            string key = row.Item1; //column name
                            string val = row.Item2[i]; //column value

                            //pass contact object reference along with column name and value to add
                            //to contact object
                            UpdateContact(key, val, contact);
                        }
                        //for testing, can remove
                        Console.WriteLine(contact);
                        //save contact to database
                        _context.contacts.Add(contact);
                        _context.SaveChanges();
                    }
                }
            }
        }

        // THIS IS THE MAIN FUNCTION TO CHANGE
        // checks passed column name in switch case to choose which Contact property to add
        // the value to.
        // Params:
        // string key: the name of the column
        // string val: the value inside of that column
        // Contact contact: the object reference to add the value to
        private void UpdateContact(string key, string val, Contact contact)
        {
            switch (key)
            {
                case "Organization":
                    contact.Organization = val;
                    break;
                case "First":
                    contact.FirstName = val;
                    break;
                case "Name":
                    contact.LastName = val;
                    break;
                case "Title":
                    contact.Title = val;
                    break;
                case "Address":
                    contact.StreetAddress1 = val;
                    break;
                case "Address 2":
                    contact.Address2 = val;
                    break;
                case "City":
                    contact.City = val;
                    break;
                case "Province":
                    contact.Province = val;
                    break;
                case "Postal Code":
                    contact.PostalCode = val;
                    break;
                case "Phone":
                    contact.Phone = val;
                    break;
                case "Fax":
                    contact.Fax = val;
                    break;
                case "Website":
                    contact.Website = val;
                    break;
                case "Home Category":
                    contact.HomeCategory = val;
                    break;
                case "Mailing List":
                    contact.MailingList = val;
                    break;
                case "Number of Beds":
                    try
                    {
                        contact.BedsCount = int.Parse(val);
                    } catch (Exception e) {
                        Console.WriteLine("Bedcount not set");
                    }
                    break;
                case "Subscribed Y/N":
                    contact.Subscribed = val;
                    break;
                case "Emails":
                    contact.Email = val;
                    break;
            }
        }

        private void Export()
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet1");

            worksheet.Cell(1, 1).Value = "First Name";
            worksheet.Cell(1, 2).Value = "Last Name";
            worksheet.Cell(1, 3).Value = "Organization";
            worksheet.Cell(1, 4).Value = "Title";
            worksheet.Cell(1, 5).Value = "Street Address 1";
            worksheet.Cell(1, 6).Value = "City";
            worksheet.Cell(1, 7).Value = "Province";
            worksheet.Cell(1, 8).Value = "Postal Code";
            worksheet.Cell(1, 9).Value = "Subscribed";
            worksheet.Cell(1, 10).Value = "Email";
            worksheet.Cell(1, 11).Value = "Phone";
            worksheet.Cell(1, 12).Value = "Fax";
            worksheet.Cell(1, 13).Value = "Website";
            worksheet.Cell(1, 14).Value = "Beds Count";
            worksheet.Cell(1, 15).Value = "Address 2";
            worksheet.Cell(1, 16).Value = "Extension";
        }

        private bool ContactExists(int id)
        {
            return _context.contacts.Any(e => e.ContactId == id);
        }
        public bool Contactsearch(int id)
        {
            return true;
        }
    }
}
