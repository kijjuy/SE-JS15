using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using ClosedXML.Excel;
using dbms_mvc.Models;
using dbms_mvc.Repositories;
using dbms_mvc.Services;

namespace dbms_mvc.Controllers
{
    [Authorize]
    public class ContactsController : Controller
    {
        private readonly IContactsRepository _repository;

        private readonly ISpreadsheetService _spreadsheetService;

        private readonly IMemoryCache _cache;

        private readonly ILogger<ContactsController> _logger;

        private static int maxPerPage = 50;

        public ContactsController(IContactsRepository repository, ISpreadsheetService spreadsheetService, IMemoryCache cache, ILogger<ContactsController> logger)
        {
            _repository = repository;
            _spreadsheetService = spreadsheetService;
            _cache = cache;
            _logger = logger;
        }

        // GET: Contacts
        public async Task<IActionResult> Index(Contact? searchContact, int? page)
        {
            var contacts = await _repository.SearchContacts(searchContact);

            var paginatedContacts = PaginateContacts(contacts, page);

            SetSearchViewData(searchContact);

            ViewBag.searchContact = searchContact;

            return View(paginatedContacts);
        }


        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            var contact = await _repository.GetContactById(id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        [Authorize(Roles = "create, admin")]
        public IActionResult Create()
        {
            return View(new Contact());
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
                await _repository.AddContact(contact);
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

            var contact = await _repository.GetContactById(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("ContactId,FirstName,LastName,Organization,Title,StreetAddress1,City,Province,PostalCode,Subscribed,Email,Phone,Fax,Website,BedsCount,Address2,Extension,MailingList")] Contact contact)
        {
            if (id != contact.ContactId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.UpdateContact(contact);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_repository.ContactExists(contact.ContactId))
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
            var contact = await _repository.GetContactById(id);
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
            var contact = await _repository.GetContactById(id);
            if (contact != null)
            {
                await _repository.DeleteContact(contact);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Contacts/Upload
        [Authorize(Roles = "upload, admin")]
        public IActionResult Upload()
        {
            return View();
        }

        // POST: Contact/Upload
        [HttpPost, ActionName("Upload")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "upload, admin")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null)
            {
                //Log error
                ViewBag.ErrorMessage = "You must select a file before uploading.";
                return View();
            }

            var supportedContentTypes = SpreadsheetService.SupportedContentTypes;
            if (!supportedContentTypes.Contains(file.ContentType))
            {
                //Log error
                ViewBag.ErrorMessage = "File type is not supported.";
                return View();
            }

            var worksheet = _spreadsheetService.GetWorksheetFromFile(file);

            var unmappedColumns = _spreadsheetService.GetUnmappedColumnNames(worksheet);

            if (unmappedColumns.UnmappedColumns.Count() > 0)
            {
                _logger.LogInformation($"Columns did not match. Sending manual column mapper to user.");

                var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
                Guid worksheetId = Guid.NewGuid();
                unmappedColumns.FileId = worksheetId;
                _cache.Set(worksheetId, worksheet, cacheOptions);

                //Set keys of dictionary
                Dictionary<string, string> colKeys = unmappedColumns.UnmappedColumns.ToDictionary(col => col, _ => string.Empty);
                unmappedColumns.InputModel.ColumnMappings = colKeys;

                _logger.LogInformation($"Count of UnmappedColumns: {unmappedColumns.InputModel.ColumnMappings.Count()}");

                _logger.LogInformation($"Created memory cache item with id: {unmappedColumns.FileId}");
                return View("MappingPrompt", unmappedColumns);
            }
            return await HandleValidWorksheet(worksheet);

        }

        [HttpPost]
        [Authorize(Roles = "upload, admin")]
        public async Task<IActionResult> UpdateMappedColumns([FromForm] MappingPromptViewModel viewModel)
        {
            var inputModel = viewModel.InputModel;
            string serverErrorMessage = "Server error while attempting to load uploaded file. Please try again, or contact an administrator if this error persists.";

            List<string> usedColNames = new List<string>();
            foreach (var selectedProp in inputModel.ColumnMappings)
            {
                if (usedColNames.Contains(selectedProp.Value))
                {
                    ViewBag.ErrorMessage = "You cannot select a property name to map to more than once.";
                    return View("MappingPrompt", viewModel);
                }
                usedColNames.Add(selectedProp.Value);
            }

            if (inputModel.FileId == null)
            {
                _logger.LogError($"RequestId: {HttpContext.TraceIdentifier}\n"
                    + $"MappingPromptInputModel.FileId was set to null when entering UpdateMappedColumns method.");
                var error = new ErrorViewModel(HttpContext.TraceIdentifier, serverErrorMessage);
                return View("Error", error);
            }

            var cacheItem = _cache.Get(inputModel.FileId);
            if (cacheItem == null)
            {
                _logger.LogError($"RequestId: {HttpContext.TraceIdentifier}\n"
                    + $"Attempted to find cache item with id: {inputModel.FileId}, but could not.");
                var error = new ErrorViewModel(HttpContext.TraceIdentifier, serverErrorMessage);
                return View("Error", error);
            }

            IXLWorksheet worksheet;

            try
            {
                worksheet = cacheItem as IXLWorksheet;
            }
            catch (Exception e)
            {
                _logger.LogError($"RequestId: {HttpContext.TraceIdentifier}\n"
            + $"Could not convert memory cache item to IXLWorksheet.\n"
            + $"Exception: {e}");
                var error = new ErrorViewModel(HttpContext.TraceIdentifier, serverErrorMessage);
                return View("Error", error);
            }

            if (worksheet == null)
            {
                _logger.LogError($"RequestId: {HttpContext.TraceIdentifier}\n"
                    + $"worksheet was null after getting it from memory cache.");
                var error = new ErrorViewModel(HttpContext.TraceIdentifier, serverErrorMessage);
                return View("Error", error);
            }

            var mappings = inputModel.ColumnMappings;
            worksheet = _spreadsheetService.SetMappedColumns(worksheet, mappings);

            return await HandleValidWorksheet(worksheet);
        }

        private async Task<IActionResult> HandleValidWorksheet(IXLWorksheet worksheet)
        {
            var newContacts = _spreadsheetService.GetContactsFromWorksheet(worksheet);

            IEnumerable<MergeConflictViewModel> unresolvedMerges = await _repository.GetUploadMergeConflicts(newContacts);

            if (unresolvedMerges.Count() > 0)
            {
                return View("ResolveConflicts", unresolvedMerges);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("MergeResolver")]
        public async Task<IActionResult> MergeResolver([FromBody] IEnumerable<ReplaceInputModel> replaceInputModels)
        {
            foreach (var inputModel in replaceInputModels)
            {
                await _repository.AddContact(inputModel.NewContact);

                var removeContact = await _repository.GetContactById(inputModel.ReplaceContactId);

                await _repository.DeleteContact(removeContact);
            }

            return Ok();
        }

        [HttpPost, ActionName("Export")]
        [Authorize(Roles = "export, admin")]
        public async Task<IActionResult> Export([FromBody] Contact searchContact)
        {
            var contacts = await _repository.SearchContacts(searchContact);

            byte[] spreadsheetByteArr = _spreadsheetService.CreateFileFromContacts(contacts);

            return File(spreadsheetByteArr, "application/vnd.ms-excel", GetDateString());

        }

        private void SetSearchViewData(Contact? searchContact)
        {
            if (searchContact == null)
            {
                return;
            }

            var props = typeof(Contact).GetProperties().ToList();
            props = props.Where(p => p.Name != "ContactId").ToList();
            foreach (var prop in props)
            {
                var propValue = prop.GetValue(searchContact);
                if (propValue == null)
                {
                    ViewData["prop-" + prop.Name] = "";
                    continue;
                }
                ViewData["prop-" + prop.Name] = propValue.ToString();
            }
        }

        private string GetDateString()
        {
            DateTime date = DateTime.Now;
            string dateString =
              $"{date.Day}-{date.Month}-{date.Year}_contacts.xlsx";
            return dateString;
        }

        private List<Contact> PaginateContacts(IEnumerable<Contact> contacts, int? pageInput)
        {
            int page;
            if (pageInput == null)
            {
                page = 1;
            }
            else
            {
                page = (int)pageInput;
            }

            ViewBag.page = page;

            if (page < 1)
            {
                page = 1;
            }

            int maxPage = (contacts.Count() / maxPerPage) + 1;
            if (page > maxPage)
            {
                page = maxPage;
            }

            ViewBag.prevDisabled = "";
            ViewBag.nextDisabled = "";

            if (page == 1)
            {
                ViewBag.prevDisabled = "disabled";
            }

            if (page == maxPage)
            {
                ViewBag.nextDisabled = "disabled";
            }

            int startingContact = (page - 1) * maxPerPage;
            int finalContact = startingContact + maxPerPage;

            return contacts.Skip(startingContact).Take(maxPerPage).ToList();
        }
    }
}
