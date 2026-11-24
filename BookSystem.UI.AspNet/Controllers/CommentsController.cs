using Microsoft.AspNetCore.Mvc;
using BookSystem.Database;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using BookSystem.Model;

namespace BookSystem.UI.AspNet.Controllers;
/// <summary>
/// Kontroler zarządzający komentarzami w systemie. (dodawanie komentarzy do książek)
/// </summary>
public class CommentsController : Controller
{

    private readonly BookSystemDbContext _context;
    /// <summary>
    /// Inicjalizuje kontroler i wstrzykuje połączenie z bazą danych.
    /// </summary>
    /// <param name="context">Kontekst bazy danych aplikacji.</param>
    public CommentsController(BookSystemDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Zwraca widok formularza do dodawania komentarza dla wybranej książki.
    /// Dostęp do tej akcji jest ograniczony do zalogowanych użytkowników.
    /// </summary>
    /// <param name="bookId">Unikalny identyfikator komentarza (BookId).</param>
    /// <returns>Widok formularza dodawania komentarza i oceny dla wybranej książki.</returns>
    [Authorize]
    public IActionResult Create(int bookId)
    {
        var comment = new Comment { BookId = bookId };
        return View(comment);
    }

    /// <summary>
    /// Odbiera dane z formularza oceniania, weryfikuje regułę jednej oceny i zapisuje do bazy.
    /// </summary>
    /// <param name="comment">Obiekt komentarza przesłany z formularza.</param>
    /// <returns>Przekierowanie do szczegółów książki po pomyślnym dodaniu komentarza.</returns>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Comment comment)
    {
        comment.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        bool alreadyRated = await _context.Comments
            .AnyAsync(c => c.BookId == comment.BookId && c.UserId == comment.UserId);

        if (alreadyRated)
        {
            ModelState.AddModelError(string.Empty, "Już dodałeś komentarz do tej książki. Nie możesz dodać kolejnego.");
        }

        ModelState.Remove(nameof(comment.UserId));
        ModelState.Remove(nameof(comment.User));
        ModelState.Remove(nameof(comment.Book));

        if(ModelState.IsValid && !alreadyRated)
        {
            comment.PublishDate = DateTime.Now;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Books", new { id = comment.BookId });
        }
        return View(comment);
    }
    /// <summary>
    /// Zwraca formularz edycji dla wybranego komentarza zalogowangeo użytkownika.
    /// </summary>
    /// <param name="id">Unikalny identyfikator komentarza</param>
    /// <returns>Widok formularza edycji lub błąd 404, jeśli komentarz nie istnieje lub nie należy do użytkownika.</returns>
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.CommentId == id && c.UserId == userId);

        if(comment == null)
        {
            return NotFound();
        }
        return View(comment);
    }

    /// <summary>
    /// Zapisuje zmodyfikowaną treść oraz ocenę komentarza w bazie danych.
    /// </summary>
    /// <param name="id">Unikalny identyfikator edytowanego komentarza.</param>
    /// <param name="comment">Zaktualizowany obiekt komentarza przesłany z formularza.</param>
    /// <returns>Przekierowanie do szczegółów książki po sukcesie lub powrót do formularza z błędami.</returns>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Comment comment)
    {
        if(id != comment.CommentId)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var existingComment = await _context.Comments
            .FirstOrDefaultAsync(c => c.CommentId == id && c.UserId == userId);

        if(existingComment == null)
        {
            return NotFound();
        }

        ModelState.Remove(nameof(comment.UserId));
        ModelState.Remove(nameof(comment.User));
        ModelState.Remove(nameof(comment.Book));

        if (ModelState.IsValid)
        {
            existingComment.Rating = comment.Rating;
            existingComment.Title = comment.Title;
            existingComment.Description = comment.Description;

            _context.Update(existingComment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Books", new { id = existingComment.BookId });
        }
        return View(comment);
    }
}