namespace BookSystem.UI.AspNet.Models
{
    /// <summary>
    /// Wiersz listy autorów z liczbą książek.
    /// </summary>
    public record AuthorListItem(int AuthorId, string FullName, int BookCount);
}