namespace BookSystem.UI.AspNet.Data
{
    /// <summary>
    /// Nazwy ról używanych w systemie.
    /// </summary>
    public static class AppRoles
    {
        /// <summary>Gość - użytkownik niezalogowany (rola utworzona dla kompletności, obsługiwana przez [AllowAnonymous]).</summary>
        public const string Guest = "Guest";

        /// <summary>Zalogowany użytkownik.</summary>
        public const string User = "User";

        /// <summary>Administrator systemu.</summary>
        public const string Admin = "Admin";

        /// <summary>Wartość dla atrybutu [Authorize(Roles = ...)] - zalogowany użytkownik lub administrator.</summary>
        public const string UserOrAdmin = User + "," + Admin;

        /// <summary>Wszystkie role tworzone przy starcie aplikacji.</summary>
        public static readonly string[] All = [Guest, User, Admin];
    }
}