using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using dbms_mvc.Data;
using Microsoft.AspNetCore.Authorization;

namespace dbms_mvc.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            List<ApplicationUser> users = await _context.Users.ToListAsync();
            List<AppUserViewModel> viewModels = await GenerateViewModels(users);
            return View(viewModels);
        }

        public async Task<List<AppUserViewModel>> GenerateViewModels(List<ApplicationUser> appUsers)
        {
            List<AppUserViewModel> viewModels = new List<AppUserViewModel>();
            foreach (ApplicationUser appUser in appUsers)
            {
                IList<string> roles = await _userManager.GetRolesAsync(appUser);
                AppUserViewModel newViewModel = new AppUserViewModel(appUser, roles);
                viewModels.Add(newViewModel);
            }
            return viewModels;
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            IList<string> userRoles = await _userManager.GetRolesAsync(applicationUser);
            IQueryable<IdentityRole> allRolesQuery = _roleManager.Roles;
            List<string> allRolesList = new List<string>();
            foreach (IdentityRole role in allRolesQuery)
            {
                allRolesList.Add(role.Name);
            }
            ModifyRoleViewModel viewModel = new ModifyRoleViewModel(applicationUser, userRoles, allRolesList);

            return View(viewModel);
        }

        [HttpPut]
        public async Task<IActionResult> AddRoleToUser([FromBody] UserRoleData roleData)
        {
            string id = roleData.Id;
            string role = roleData.Role;
            ApplicationUser appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
            {
                //todo: log error
                var returnErrorMessage = new
                {
                    status = "error",
                    message = $"Could not find user with id: {id}"
                };
                Console.WriteLine($"Id of user: {id}");
                return Json(returnErrorMessage);
            }
            await _userManager.AddToRoleAsync(appUser, role);
            var returnSuccessMessage = new
            {
                status = "success",
                message = $"Successfully added {role} to user"
            };
            return Json(returnSuccessMessage);
        }

        [HttpPut]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] UserRoleData roleData)
        {
            string id = roleData.Id;
            string role = roleData.Role;
            ApplicationUser appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
            {
                //todo: log error
                var returnErrorMessage = new
                {
                    status = "error",
                    message = $"Could not find user with id: {id}"
                };
                return Json(returnErrorMessage);
            }
            await _userManager.RemoveFromRoleAsync(appUser, role);
            var returnSuccessMessage = new
            {
                status = "success",
                message = $"Successfully removed {role} from user"
            };
            return Json(returnSuccessMessage);

        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(applicationUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(applicationUser);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users.FindAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            return View(applicationUser);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUser.Id))
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
            return View(applicationUser);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _context.Users.FindAsync(id);
            if (applicationUser != null)
            {
                _context.Users.Remove(applicationUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
