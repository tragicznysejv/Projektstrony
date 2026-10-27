using System.ComponentModel.DataAnnotations;
using BookSystem.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookSystem.UI.AspNet.Areas.Identity.Pages.Account.Manage
{
    /// <summary>
    /// Edycja profilu zalogowanego użytkownika (imię, nazwisko, telefon).
    /// Zmiana e-maila i hasła pozostaje na osobnych, standardowych stronach Identity.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<IndexModel> _logger;

        /// <summary>
        /// Konstruktor wstrzykujący usługi Identity.
        /// </summary>
        public IndexModel(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        /// Login użytkownika (e-mail) - tylko do odczytu.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Komunikat wyświetlany po zapisaniu zmian (przechowywany w TempData między przekierowaniami).
        /// </summary>
        [TempData]
        public string? StatusMessage { get; set; }

        /// <summary>
        /// Dane formularza.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = new();

        /// <summary>
        /// Pola edytowalne w profilu.
        /// </summary>
        public class InputModel
        {
            /// <summary>Imię użytkownika.</summary>
            [Required(ErrorMessage = "Imię jest wymagane.")]
            [StringLength(50, ErrorMessage = "Imię może mieć maksymalnie {1} znaków.")]
            [Display(Name = "Imię")]
            public string FirstName { get; set; } = string.Empty;

            /// <summary>Nazwisko użytkownika.</summary>
            [Required(ErrorMessage = "Nazwisko jest wymagane.")]
            [StringLength(50, ErrorMessage = "Nazwisko może mieć maksymalnie {1} znaków.")]
            [Display(Name = "Nazwisko")]
            public string LastName { get; set; } = string.Empty;

            /// <summary>Opcjonalny numer telefonu.</summary>
            [Phone(ErrorMessage = "Niepoprawny numer telefonu.")]
            [Display(Name = "Telefon (opcjonalnie)")]
            public string? PhoneNumber { get; set; }
        }

        /// <summary>
        /// Wyświetla formularz wypełniony aktualnymi danymi użytkownika.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound("Nie znaleziono użytkownika.");

            LoadForm(user);
            return Page();
        }

        /// <summary>
        /// Zapisuje zmiany w profilu.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound("Nie znaleziono użytkownika.");

            if (!ModelState.IsValid)
            {
                Username = user.UserName;
                return Page();
            }

            user.FirstName = Input.FirstName.Trim();
            user.LastName = Input.LastName.Trim();

            var phoneNumber = string.IsNullOrWhiteSpace(Input.PhoneNumber) ? null : Input.PhoneNumber.Trim();
            if (phoneNumber != user.PhoneNumber)
                user.PhoneNumber = phoneNumber;

            // UpdateAsync zapisuje wszystkie zmiany jednocześnie i pilnuje znacznika współbieżności (ConcurrencyStamp).
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                Username = user.UserName;
                return Page();
            }

            // Odświeżenie ciasteczka logowania, aby zawierało aktualne dane użytkownika.
            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("Użytkownik {UserId} zaktualizował profil.", user.Id);

            StatusMessage = "Profil został zaktualizowany.";
            return RedirectToPage();
        }

        private void LoadForm(User user)
        {
            Username = user.UserName;
            Input = new InputModel
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                PhoneNumber = user.PhoneNumber
            };
        }
    }
}
