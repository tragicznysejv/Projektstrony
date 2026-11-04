using BookSystem.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookSystem.UI.AspNet.Controllers;
/// <summary>
/// Kontroler zarządzający katalogiem książek w systemie. (wyświetlanie listy książek oraz szczegółów książki)
/// </summary>
public class BooksController : Controller
{
    private readonly BookSystemDbContext _context;

    /// <summary>
    /// Inicjalizuje kontroler i wstrzykuje połączenie z bazą danych.
    /// </summary>
    /// <param name="context">Kontekst bazy danych aplikacji.</param>
    public BooksController(BookSystemDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Wyświetla główną stronę katalogu z opcją wyszukiwania.
    /// </summary>
    /// <param name="searchString">Fraza wpisana w wyszukiwarkę (tytuł lub nazwisko autora)</param>
    /// <returns>Widok z przefiltrowaną listą książek.</returns>
    public async Task<IActionResult> Index(string searchString)
    {
        var booksQuery = _context.Books.Include(b => b.Author).AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            booksQuery = booksQuery.Where(b =>
                (b.Title != null && b.Title.Contains(searchString)) ||
                (b.Author != null && b.Author.LastName != null && b.Author.LastName.Contains(searchString)));
        }

        return View(await booksQuery.ToListAsync());
    }
    /// <summary>
    /// Wyświetla szczegóły wybranej książki na podstawie jej identyfikatora.
    /// </summary>
    /// <param name="id">Unikalny identyfikator książki (BookId).</param>
    /// <returns>Widok ze szczegółami książki lub błąd 404 w przypadku braku danych.</returns>
    public async Task<IActionResult> Details(int? id)
    {
        if(id== null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Comments)
            .FirstOrDefaultAsync(b => b.BookId == id);

        if(book == null)
        {
            return NotFound();
        }

        return View(book);
    }

}
