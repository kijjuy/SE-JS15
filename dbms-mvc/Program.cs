using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using dbms_mvc.Data;
using dbms_mvc.Repositories;
using dbms_mvc.Services;

namespace dbms_mvc;

public class Program
{
    private static void ConfigureRelease(ref WebApplicationBuilder builder)
    {
        Console.WriteLine("Launching in configuration: Release");
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
    }

    private static void ConfigureDevelopment(ref WebApplicationBuilder builder)
    {
        Console.WriteLine("Launching in configuration: Debug");
        var connectionStringSecret = builder.Configuration.GetSection("ConnectionStrings").Get<ConnectionStringConfig>();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionStringSecret.Docker));
    }


    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        var configurationManager = builder.Configuration;
        // Add services to the container.

        if (builder.Environment.IsProduction())
        {
            ConfigureRelease(ref builder);
        }
        else
        {
            ConfigureDevelopment(ref builder);
        }

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
                .AddDefaultUI();


        builder.Services.AddControllersWithViews();

        builder.Services.AddScoped<IContactsRepository, ContactsRepository>();

        builder.Services.AddScoped<ISpreadsheetService, SpreadsheetService>();

        builder.Services.AddMemoryCache();

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var app = builder.Build();

        //setup DbInitializer
        //var adminPassSecret = Environment.GetEnvironmentVariable("AdminPassword");
        //var loginSecrets = new LoginConfig
        //{
        //    AdminPassword = adminPassSecret
        //};

        //using (var scope = app.Services.CreateScope())
        //{
        //    if (loginSecrets == null)
        //    {
        //        Console.WriteLine("AppSecrets must be set up for database seeding.");
        //    }
        //    else
        //    {
        //        DbInitializer.appSecrets = loginSecrets;
        //        DbInitializer.SeedUsersAndRoles(scope.ServiceProvider).Wait();
        //    }
        //}

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

        app.UseForwardedHeaders();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        app.Run();
    }


}

