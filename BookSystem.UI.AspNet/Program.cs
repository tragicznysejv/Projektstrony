using BookSystem.Database;
using BookSystem.Model;
using BookSystem.UI.AspNet.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookSystem.UI.AspNet
{
    /// <summary>
    /// Punkt startowy aplikacji BookSystem - konfiguracja usług, Identity oraz inicjalizacja bazy danych.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Uruchamia aplikację webową.
        /// </summary>
        /// <param name="args">Argumenty wiersza poleceń.</param>

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<BookSystemDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<BookSystemDbContext>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            await SeedDatabaseAsync(app);

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            await app.RunAsync();
        }

        /// <summary>
        /// Stosuje migracje, tworzy role i konto administratora, a następnie importuje katalog z plików CSV.
        /// </summary>
        private static async Task SeedDatabaseAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                var context = services.GetRequiredService<BookSystemDbContext>();
                await context.Database.MigrateAsync();

                await IdentitySeeder.SeedAsync(services, app.Configuration, logger);
                await CsvDataSeeder.SeedAsync(context, Path.Combine(app.Environment.ContentRootPath, "SeedData"), logger);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Inicjalizacja bazy danych nie powiodła się.");
                throw;
            }
        }
    }
}