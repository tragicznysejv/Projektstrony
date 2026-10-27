using BookSystem.Model;

namespace BookSystem.UI.AspNet.Models
{
    /// <summary>
    /// Dane strony szczegółów książki.
    /// </summary>
    public class BookDetailsViewModel
    {
        /// <summary>Wyświetlana książka wraz z autorem i komentarzami.</summary>
        public required Book Book { get; init; }

        /// <summary>Wpis zalogowanego użytkownika na półce (null, gdy brak lub gość).</summary>
        public Shelf? MyShelf { get; init; }

        /// <summary>Formularz nowej recenzji.</summary>
        public CommentInputModel NewComment { get; init; } = new();

        /// <summary>Średnia ocena lub null, gdy brak ocen.</summary>
        public double? AverageRating => Book.Comments.Count == 0 ? null : Book.Comments.Average(c => c.Rating);
    }
}