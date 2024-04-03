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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

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
        public async Task<IActionResult> AddRoleToUser([FromBody] UserRoleDataInputModel roleData)
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
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] UserRoleDataInputModel roleData)
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

        [HttpGet]
        public async Task<IActionResult> GetRegistrationCode()
        {
            RegistrationCode registrationCode = new RegistrationCode();
            await _context.registrationCodes.AddAsync(registrationCode);
            await _context.SaveChangesAsync();
            var ret = new
            {
                registrationCode.Token,
                registrationCode.Expiration
            };
            return Json(ret);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromBody] string userId)
        {
            Console.WriteLine("called");
            ApplicationUser appUser = await _userManager.FindByIdAsync(userId);
            ApplicationUser loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            if (appUser == null)
            {
                //Add logging
                var errorMessage = new
                {
                    status = "error",
                    message = $"There was an error finding the user with id: {userId}. Please try again."
                };
                return Json(errorMessage);
            }

            if (appUser.Equals(loggedInUser))
            {
                //log
                //TODO: create class/struct for error and success messages
                var errorMessage = new
                {
                    status = "error",
                    message = "You cannot delete yourself."
                };
                return Json(errorMessage);
            }

            //delete user here

            Console.WriteLine(appUser.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
