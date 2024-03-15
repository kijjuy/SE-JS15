using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using dbms_mvc.Data;

namespace dbms_mvc;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        var configurationManager = builder.Configuration;

        // Add services to the container.
        var connectionString = configurationManager.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
                .AddDefaultUI();

        builder.Services.AddControllersWithViews();

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        //TODO: Setup logging

        var app = builder.Build();

        //setup DbInitializer
        var loginSecrets = configurationManager.GetSection("Login").Get<LoginConfig>();
        using (var scope = app.Services.CreateScope())
        {
            if (loginSecrets == null)
            {
                Console.WriteLine("AppSecrets must be set up for database seeding.");
            }
            else
            {
                DbInitializer.appSecrets = loginSecrets;
                DbInitializer.SeedUsersAndRoles(scope.ServiceProvider).Wait();
            }
        }

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

        app.Run();
    }


}

