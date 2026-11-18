using System.ComponentModel.DataAnnotations;
using BookSystem.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookSystem.UI.AspNet.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Model strony logowania użytkownika
    /// </summary>
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        /// <summary>
        /// Inicjalizuje model logowania i wstrzykuje menadżer logowania.
        /// </summary>
        /// <param name="signInManager">Menadżer obsługi logowania użytkowników</param>
        public LoginModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        /// <summary>
        /// Dane wprowadzane przez użytkownika w formularzu logowania.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = new();

        /// <summary>
        /// Adres URL przekierowania po poprawnym zalogowaniu.
        /// </summary>
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Komunikat błędu wyswietlany na stronie logowania.
        /// </summary>
        [TempData]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Model danych wejściowych formularza logowania.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// Adres e-mail użytkownika.
            /// </summary>
            [Required(ErrorMessage = "Adres e-mail jest wymagany.")]
            [EmailAddress(ErrorMessage = "Niepoprawny adres e-mail.")]
            [Display(Name = "E-mail")]
            public string Email { get; set; } = string.Empty;

            /// <summary>
            /// Hasło użytkownika.
            /// </summary>
            [Required(ErrorMessage = "Hasło jest wymagane.")]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; } = string.Empty;

            /// <summary>
            /// Określa czy sesja logowania ma zostać zapisana.
            /// </summary>
            [Display(Name = "Zapamiętaj mnie")]
            public bool RememberMe { get; set; }
        }

        /// <summary>
        /// Obsługuje żądanie GET i przygotowuje formularz logowania.
        /// </summary>
        /// <param name="returnUrl">Opcjonalny adres powrotny.</param>
        /// <returns></returns>
        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ReturnUrl = returnUrl;
        }

        /// <summary>
        /// Obsługuje żądanie POST i weryfikuje dane logowania użytkownika.
        /// </summary>
        /// <param name="returnUrl">Opcjonalny adres powrotny po zalogowaniu.</param>
        /// <returns>Przekierowanie po zalogowaniu lub ponowne wyświetlenie formularza z błędami.</returns>
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Konto zostało zablokowane.");
                    return Page();
                }

                ModelState.AddModelError(string.Empty, "Nieprawidłowy e-mail lub hasło.");
            }

            return Page();
        }
    }
}