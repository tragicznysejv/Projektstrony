using System.Globalization;
using System.Text;
using BookSystem.Database;
using BookSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace BookSystem.UI.AspNet.Data
{
    /// <summary>
    /// Zasila bazę danych autorami i książkami z plików CSV (separator: średnik, kodowanie: UTF-8).
    /// </summary>
    public static class CsvDataSeeder
    {
        private const char Separator = ';';
        private const string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Wczytuje pliki authors.csv oraz books.csv i zapisuje dane w jednej transakcji.
        /// Seeding wykonuje się tylko wtedy, gdy tabela autorów jest pusta.
        /// </summary>
        /// <param name="context">Kontekst bazy danych.</param>
        /// <param name="dataDirectory">Katalog zawierający pliki CSV.</param>
        /// <param name="logger">Logger do raportowania przebiegu seedowania.</param>
        public static async Task SeedAsync(BookSystemDbContext context, string dataDirectory, ILogger logger)
        {
            if (await context.Authors.AnyAsync())
            {
                logger.LogInformation("Katalog książek jest już wypełniony - pomijam import CSV.");
                return;
            }

            var authorsPath = Path.Combine(dataDirectory, "authors.csv");
            var booksPath = Path.Combine(dataDirectory, "books.csv");

            if (!File.Exists(authorsPath) || !File.Exists(booksPath))
            {
                logger.LogWarning("Brak plików CSV w katalogu {Directory} - import pominięty.", dataDirectory);
                return;
            }

            // Klucz: IdAutora z pliku CSV. Wartość: encja, której właściwy identyfikator nada SQL Server.
            var authorsByCsvId = new Dictionary<int, Author>();
            foreach (var (lineNumber, columns) in ReadCsv(authorsPath, expectedColumns: 3))
            {
                var csvId = ParseInt(columns[0], authorsPath, lineNumber);
                var author = new Author { FirstName = columns[1], LastName = columns[2] };

                if (!authorsByCsvId.TryAdd(csvId, author))
                    throw new InvalidDataException($"{authorsPath}, wiersz {lineNumber}: zduplikowane IdAutora {csvId}.");
            }

            var books = new List<Book>();
            foreach (var (lineNumber, columns) in ReadCsv(booksPath, expectedColumns: 5))
            {
                var csvAuthorId = ParseInt(columns[1], booksPath, lineNumber);
                if (!authorsByCsvId.TryGetValue(csvAuthorId, out var author))
                    throw new InvalidDataException($"{booksPath}, wiersz {lineNumber}: brak autora o IdAutora {csvAuthorId}.");

                if (!DateTime.TryParseExact(columns[3], DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var publishDate))
                    throw new InvalidDataException($"{booksPath}, wiersz {lineNumber}: niepoprawna data '{columns[3]}' (oczekiwano {DateFormat}).");

                books.Add(new Book
                {
                    Title = columns[0],
                    Author = author,
                    ISBN = columns[2],
                    PublishDate = publishDate,
                    Description = columns[4]
                });
            }

            context.Authors.AddRange(authorsByCsvId.OrderBy(a => a.Key).Select(a => a.Value));
            context.Books.AddRange(books);

            
            await context.SaveChangesAsync();

            logger.LogInformation("Zaimportowano {Authors} autorów i {Books} książek z plików CSV.",
                authorsByCsvId.Count, books.Count);
        }

        /// <summary>
        /// Czyta plik CSV wiersz po wierszu, pomijając nagłówek i puste linie.
        /// </summary>
        private static IEnumerable<(int LineNumber, string[] Columns)> ReadCsv(string path, int expectedColumns)
        {
            var lineNumber = 0;
            foreach (var line in File.ReadLines(path, Encoding.UTF8))
            {
                lineNumber++;
                if (lineNumber == 1 || string.IsNullOrWhiteSpace(line))
                    continue;

                
                var columns = line.Split(Separator, expectedColumns).Select(c => c.Trim()).ToArray();
                if (columns.Length != expectedColumns)
                    throw new InvalidDataException($"{path}, wiersz {lineNumber}: oczekiwano {expectedColumns} kolumn, znaleziono {columns.Length}.");

                yield return (lineNumber, columns);
            }
        }

        private static int ParseInt(string value, string path, int lineNumber) =>
            int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
                ? result
                : throw new InvalidDataException($"{path}, wiersz {lineNumber}: '{value}' nie jest liczbą całkowitą.");
    }
}