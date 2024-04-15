using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using dbms_mvc.Data;
using dbms_mvc.Repositories;
using dbms_mvc.Services;

namespace dbms_mvc;

public class Program
{
    public static void Main(string[] args)
    {
        string keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
        string kvUri = "https://" + keyVaultName + ".vault.azure.net";

        var vaultClient = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

        var conSrting = vaultClient.GetSecret("db-con-string");

        var builder = WebApplication.CreateBuilder(args);
        var connectionString = conSrting.Value.Value;
        var configurationManager = builder.Configuration;

        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
                .AddDefaultUI();

        builder.Services.AddControllersWithViews();

        builder.Services.AddScoped<IContactsRepository, ContactsRepository>();

        builder.Services.AddScoped<ISpreadsheetService, SpreadsheetService>();

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

