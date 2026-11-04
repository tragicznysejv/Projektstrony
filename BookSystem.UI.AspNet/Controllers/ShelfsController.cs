using BookSystem.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookSystem.Model;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookSystem.UI.AspNet.Controllers;
/// <summary>
/// Kontroler zarządzający półkami użytkowników w systemie. (dodawanie książek do półki, wyświetlanie półki użytkownika)
/// </summary>
public class ShelfsController : Controller
{
    private readonly BookSystemDbContext _context;
    /// <summary>
    /// Inicjalizuje kontroler i wstrzykuje połączenie z bazą danych.
    /// </summary>
    /// <param name="context">Kontekst bazy danych aplikacji.</param>
    public ShelfsController(BookSystemDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Wyświetla wirtualną półkę zalogowanego użytkownika z jego książkami.
    /// </summary>
    /// <returns>Widok z listą książek przypisanych do półki danego użytkownika.</returns>
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var myShelf = await _context.Shelfs
            .Include(s => s.Book)
            .ThenInclude(b => b!.Author)
            .Where(s => s.UserId == userId)
            .ToListAsync();

        return View(myShelf);
    }

    /// <summary>
    /// Dodaje książkę do półki użytkownika. Jeśli książka już istnieje na półce, nie zostanie dodana ponownie.
    /// </summary>
    /// <param name="bookId">Identyfikator modyfikowanej książki</param>
    /// <param name="status">Nowy status czytania</param>
    /// <param name="isFavourite">Informacja, czy książka ma zostać oznaczona jako ulubiona.</param>
    /// <returns>Przekierowanie do szczegółów książki lub do wirtualnej półki.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateShelf(int bookId, ShelfStatus? status, bool isFavourite = false)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var existingShelf = await _context.Shelfs
            .FirstOrDefaultAsync(s => s.UserId == userId && s.BookId == bookId);

        if(existingShelf != null)
        {
            existingShelf.Status = status;
            existingShelf.IsFavourite = isFavourite;
            _context.Update(existingShelf);
        }
        else
        {
            var newShelf = new Shelf
            {
                UserId = userId,
                BookId = bookId,
                Status = status,
                IsFavourite = isFavourite
            };
            _context.Shelfs.Add(newShelf);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Books", new { id = bookId });
    }

    /// <summary>
    /// Usuwa książkę z wirtualnej półki użytkownika na podstawie identyfikatora książki.
    /// </summary>
    /// <param name="bookId">Identyfikator książki</param>
    /// <returns>Przekierowanie do widoku własnej półki.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromShelf(int bookId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var shelfEntry = await _context.Shelfs
            .FirstOrDefaultAsync(s => s.UserId == userId && s.BookId == bookId);
        if(shelfEntry != null)
        {
            _context.Shelfs.Remove(shelfEntry);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
