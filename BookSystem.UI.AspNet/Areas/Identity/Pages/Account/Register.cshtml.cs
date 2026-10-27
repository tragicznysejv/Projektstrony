using System.ComponentModel.DataAnnotations;
using BookSystem.Model;
using BookSystem.UI.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookSystem.UI.AspNet.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Rejestracja nowego użytkownika z automatycznym przypisaniem roli User.
    /// </summary>
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(SignInManager<User> signInManager, UserManager<User> userManager, ILogger<RegisterModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>Dane formularza.</summary>
        [BindProperty]
        public InputModel Input { get; set; } = new();

        /// <summary>Adres powrotu po rejestracji.</summary>
        public string? ReturnUrl { get; set; }

        /// <summary>Pola formularza rejestracji.</summary>
        public class InputModel
        {
            [Required(ErrorMessage = "Imię jest wymagane.")]
            [StringLength(50)]
            [Display(Name = "Imię")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Nazwisko jest wymagane.")]
            [StringLength(50)]
            [Display(Name = "Nazwisko")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "E-mail jest wymagany.")]
            [EmailAddress(ErrorMessage = "Niepoprawny adres e-mail.")]
            [Display(Name = "E-mail")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Hasło jest wymagane.")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "Hasło musi mieć od {2} do {1} znaków.")]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Powtórz hasło")]
            [Compare(nameof(Password), ErrorMessage = "Hasła nie są identyczne.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return Page();

            var user = new User
            {
                UserName = Input.Email,
                Email = Input.Email,
                FirstName = Input.FirstName.Trim(),
                LastName = Input.LastName.Trim()
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                // Rola musi zostać nadana PRZED zalogowaniem, aby trafiła do ciasteczka uwierzytelniającego.
                var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.User);
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    AddErrors(roleResult);
                    return Page();
                }

                _logger.LogInformation("Zarejestrowano użytkownika {Email} z rolą {Role}.", user.Email, AppRoles.User);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            AddErrors(result);
            return Page();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}