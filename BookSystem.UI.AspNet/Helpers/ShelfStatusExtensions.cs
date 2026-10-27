using BookSystem.Model;

namespace BookSystem.UI.AspNet.Helpers
{
    /// <summary>
    /// Polskie nazwy statusów półki do wyświetlania w widokach.
    /// </summary>
    public static class ShelfStatusExtensions
    {
        /// <summary>
        /// Zwraca czytelną nazwę statusu.
        /// </summary>
        public static string ToDisplayName(this ShelfStatus status) => status switch
        {
            ShelfStatus.PlanToRead => "Do przeczytania",
            ShelfStatus.Reading => "W trakcie",
            ShelfStatus.Read => "Przeczytane",
            _ => status.ToString()
        };
    }
}