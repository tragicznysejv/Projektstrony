using BookSystem.Model;
using Microsoft.AspNetCore.Identity;

namespace BookSystem.UI.AspNet.Data
{
    /// <summary>
    /// Tworzy role systemowe oraz domyślne konto administratora.
    /// </summary>
    public static class IdentitySeeder
    {
        private const string DefaultAdminEmail = "admin@booksystem.pl";
        private const string DefaultAdminPassword = "Admin123!";

        /// <summary>
        /// Zapewnia istnienie ról oraz konta administratora.
        /// </summary>
        /// <param name="services">Dostawca usług z utworzonego zakresu (scope).</param>
        /// <param name="configuration">Konfiguracja - pozwala nadpisać dane admina (sekcja AdminAccount).</param>
        /// <param name="logger">Logger.</param>
        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration, ILogger logger)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<User>>();

            foreach (var role in AppRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    EnsureSucceeded(await roleManager.CreateAsync(new IdentityRole(role)), $"utworzenie roli {role}");
                    logger.LogInformation("Utworzono rolę {Role}.", role);
                }
            }

            var email = configuration["AdminAccount:Email"] ?? DefaultAdminEmail;
            var password = configuration["AdminAccount:Password"] ?? DefaultAdminPassword;

            var admin = await userManager.FindByEmailAsync(email);
            if (admin is null)
            {
                admin = new User
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = "Administrator",
                    LastName = "Systemu"
                };
                EnsureSucceeded(await userManager.CreateAsync(admin, password), "utworzenie konta administratora");
                logger.LogInformation("Utworzono konto administratora {Email}.", email);
            }

            // Admin otrzymuje także rolę User, aby mógł korzystać z półki i recenzji.
            foreach (var role in new[] { AppRoles.Admin, AppRoles.User })
            {
                if (!await userManager.IsInRoleAsync(admin, role))
                    EnsureSucceeded(await userManager.AddToRoleAsync(admin, role), $"przypisanie roli {role} administratorowi");
            }
        }

        private static void EnsureSucceeded(IdentityResult result, string operation)
        {
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Nie powiodło się: {operation}. {string.Join(" ", result.Errors.Select(e => e.Description))}");
        }
    }
}