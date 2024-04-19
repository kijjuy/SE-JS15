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
        SecretClientOptions kvOptions = new SecretClientOptions()
        {
            Retry =
        {
        Delay = TimeSpan.FromSeconds(2),
        MaxDelay = TimeSpan.FromSeconds(16),
        MaxRetries = 5,
        Mode = Azure.Core.RetryMode.Exponential
        }
        };
        string kvUri = "https://sm-app-secrets.vault.azure.net";

        var secretClient = new SecretClient(new Uri(kvUri), new DefaultAzureCredential(), kvOptions);

        var conStringSecret = secretClient.GetSecret("db-con-string");
        var connectionString = conStringSecret.Value.Value;

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        var appInsightsConString = secretClient.GetSecret("app-insights-con-string").Value;

        builder.Logging.AddApplicationInsights(
            configureTelemetryConfiguration: (config) =>
                config.ConnectionString = appInsightsConString.Value,
                configureApplicationInsightsLoggerOptions: (options) => { }
        );
    }

    private static void ConfigureDevelopment(ref WebApplicationBuilder builder)
    {
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

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var app = builder.Build();

        //setup DbInitializer
        //var adminPassSecret = vaultClient.GetSecret("AdminPassword").Value;
        //var loginSecrets = new LoginConfig
        //{
        //    AdminPassword = adminPassSecret.Value
        //};

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

