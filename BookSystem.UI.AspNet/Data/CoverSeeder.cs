using System.Text;
using BookSystem.Database;
using Microsoft.EntityFrameworkCore;

namespace BookSystem.UI.AspNet.Data
{
    /// <summary>
    /// Uzupełnia adresy okładek książek na podstawie pliku covers.csv (ISBN;Title;CoverUrl).
    /// </summary>
    public static class CoverSeeder
    {
        /// <summary>
        /// Przypisuje okładki książkom, które jeszcze ich nie mają. Dopasowanie odbywa się po numerze ISBN.
        /// Okładki ustawione wcześniej (np. ręcznie przez administratora) nie są nadpisywane.
        /// </summary>
        /// <param name="context">Kontekst bazy danych.</param>
        /// <param name="dataDirectory">Katalog zawierający plik covers.csv.</param>
        /// <param name="logger">Logger do raportowania przebiegu.</param>
        public static async Task SeedAsync(BookSystemDbContext context, string dataDirectory, ILogger logger)
        {
            var path = Path.Combine(dataDirectory, "covers.csv");
            if (!File.Exists(path))
            {
                logger.LogWarning("Brak pliku {Path} - pomijam uzupełnianie okładek.", path);
                return;
            }

            // Klucz: ISBN, wartość: adres okładki. Wiersze bez adresu są pomijane.
            var coversByIsbn = File.ReadLines(path, Encoding.UTF8)
                .Skip(1)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Split(';'))
                .Where(columns => columns.Length == 3 && !string.IsNullOrWhiteSpace(columns[2]))
                .GroupBy(columns => columns[0].Trim())
                .ToDictionary(group => group.Key, group => group.First()[2].Trim());

            var booksWithoutCover = await context.Books
                .Where(b => b.CoverUrl == null)
                .ToListAsync();

            var updated = 0;
            foreach (var book in booksWithoutCover)
            {
                if (book.ISBN != null && coversByIsbn.TryGetValue(book.ISBN, out var coverUrl))
                {
                    book.CoverUrl = coverUrl;
                    updated++;
                }
            }

            if (updated > 0)
                await context.SaveChangesAsync();

            logger.LogInformation("Uzupełniono okładki dla {Count} książek.", updated);
        }
    }
}