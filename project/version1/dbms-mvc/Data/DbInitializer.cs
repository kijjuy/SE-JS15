using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace dbms_mvc.Data;
public static class DbInitializer
{
    public static LoginConfig appSecrets { get; set; }

    private static string[] roleNames = new string[] {
        "admin",
        "create",
        "update",
        "delete",
        "upload",
        "export",
    };

    public static async Task<int> SeedUsersAndRoles(IServiceProvider serviceProvider)
    {
        // create the database if it doesn't exist
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Check if roles already exist and exit if there are
        if (roleManager.Roles.Count() > 0)
            return 1;  // should log an error message here

        // Seed roles
        int result = await SeedRoles(roleManager);
        if (result != 0)
            return 2;  // should log an error message here

        // Check if users already exist and exit if there are
        if (userManager.Users.Count() > roleNames.Count())
            return 3;  // should log an error message here

        // Seed users
        result = await SeedUsers(userManager);
        if (result != 0)
            return 4;  // should log an error message here

        return 0;
    }

    private static async Task<int> SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        foreach (string roleName in roleNames)
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
                return 1;  // should log an error message here
        }

        return 0;
    }

    private static async Task<int> SeedUsers(UserManager<ApplicationUser> userManager)
    {
        // Create admin User
        List<(ApplicationUser, string)> appUsers = new List<(ApplicationUser, string)> {
            (new ApplicationUser {
                UserName = "admin@email.com",
                Email = "admin@email.com",
                EmailConfirmed = true,
            }, "admin" ),
            (new ApplicationUser {
                UserName = "user@email.com",
                Email = "user@email.com",
                EmailConfirmed = true,
            }, "update" ),
        };

        foreach ((ApplicationUser, string) appUser in appUsers)
        {
            var result = await CreateUser(appUser.Item1, appUser.Item2, appSecrets.AdminPassword, userManager);
            if (!result.Succeeded)
                return 2;  // should log an error message here
        }
        return 0;
    }

    private static async Task<IdentityResult> CreateUser(ApplicationUser appUser, string role,
            string password, UserManager<ApplicationUser> userManager)
    {
        var result = await userManager.CreateAsync(appUser, password);
        result = await userManager.AddToRoleAsync(appUser, role);
        return result;
    }

}
