using BookSystem.UI.AspNet.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BookSystem.Database;
using Microsoft.EntityFrameworkCore;

namespace BookSystem.UI.AspNet.Controllers;
/// <summary>
/// Główny kontroler aplikacji, odpowiedzialny za wyświetlanie strony głównej z najnowszymi książkami w systemie.
/// </summary>
public class HomeController : Controller
{
    private readonly BookSystemDbContext _context;
    /// <summary>
    /// Inicjalizuje kontroler i wstrzykuje połączenie z bazą danych.
    /// </summary>
    /// <param name="context">Kontekst bazy danych aplikacji.</param>
    public HomeController(BookSystemDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Wyświetla stronę główną aplikacji z listą najnowszych książek w systemie.
    /// </summary>
    /// <returns>Widok strony głównej z listą polecanych tytułów</returns>
    public async Task<IActionResult> Index()
    {
        var recentBooks = await _context.Books
            .Include(b => b.Author)
            .OrderByDescending(b => b.PublishDate)
            .Take(10)
            .ToListAsync();
        return View(recentBooks);
    }
}
