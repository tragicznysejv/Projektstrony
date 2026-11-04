using BookSystem.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookSystem.UI.AspNet.Controllers;
/// <summary>
/// Kontroler zarządzający autorami w systemie. (wyświetlanie listy autorów oraz szczegółów autora)
/// </summary>
public class AuthorsController : Controller 
{
    private readonly BookSystemDbContext _context;
    /// <summary>
    /// Inicjalizuje kontroler i wstrzykuje połączenie z bazą danych.
    /// </summary>
    /// <param name="context">Kontekst bazy danych aplikacji.</param>
    public AuthorsController(BookSystemDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Wyświetla listę wszystkich autorów z opcją wyszukiwania.
    /// </summary>
    /// <param name="searchString">Fraza wpisana w wyszukiwarkę (imię lub nazwisko autora)</param>
    /// <returns>Widok z listą autorów i danymi ich książek</returns>
    public async Task<IActionResult> Index(string searchString)
    {
        var authorsQuery = _context.Authors.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            authorsQuery = authorsQuery.Where(a =>
                (a.LastName != null && a.LastName.Contains(searchString)) ||
                (a.FirstName != null && a.FirstName.Contains(searchString)));
        }

        return View(await authorsQuery.ToListAsync());
    }
    /// <summary>
    /// Wyświetla szczegóły wybranego autora na podstawie jego identyfikatora.
    /// </summary>
    /// <param name="id">Unikalny identyfikator autora (AuthorId).</param>
    /// <returns>Widok ze szczegółami autora lub błąd 404 w przypadku braku danych</returns>

    public async Task<IActionResult> Details(int? id)
    {
        if(id == null)
        {
            return NotFound();
        }

        var author = await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.AuthorId == id);

        if(author == null)
        {
            return NotFound();
        }
        return View(author);
    }

}
