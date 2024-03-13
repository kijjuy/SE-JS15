using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using dbms_mvc.Data;

namespace dbms_mvc;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        var ConfigurationManager = builder.Configuration;

        // Add services to the container.
        var connectionString = ConfigurationManager.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
                .AddDefaultUI();

        builder.Services.AddControllersWithViews();

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        using(var scope = app.Services.CreateAsyncScope())
        {
            Console.WriteLine("about to run role creator");
            EnsureRolesCreated(scope.ServiceProvider);
        }

        app.Run();
    }

    private static void EnsureRolesCreated(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
        Console.WriteLine($"Role manager: {roleManager.ToString()}");
        string[] roleNames = new string[] {
            "create",
            "delete",
            "update",
            "admin"
        };

        foreach (string roleName in roleNames)
        {
            bool roleExists = roleManager.RoleExistsAsync(roleName).Result;
            if (!roleExists)
            {
                Console.WriteLine($"Adding role: {roleName}");
                IdentityRole role = new IdentityRole(roleName);
                roleManager.CreateAsync(role);
            }
        }
    }
}

