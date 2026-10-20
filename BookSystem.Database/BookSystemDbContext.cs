using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookSystem.Model;

namespace BookSystem.Database
{
    /// <summary>
    /// Klasa kontekstu bazy danych dla systemu wirtualnej biblioteczki.
    /// Odpowiada za połączenie z bazą danych oraz integrację wbudowanego mechanizmu tożsamości (Identity) z autorskimi modelami.
    /// </summary>
    public class BookSystemDbContext : IdentityDbContext<User>
    {
        /// <summary>
        /// Konstruktor inicjalizujący kontekst bazy danych z podanymi opcjami.
        /// Wykorzystywany głównie przez mechanizm wstrzykiwania zależności (Dependency Injection) w pliku Program.cs.
        /// </summary>
        /// <param name="options">Opcje konfiguracyjne dla kontekstu bazy danych, w tym ciąg połączenia (Connection String).</param>
        public BookSystemDbContext(DbContextOptions<BookSystemDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Zbiór danych (tabela) przechowujący informacje o książkach dostępnych w systemie.
        /// </summary>
        public DbSet<Book> Books { get; set; }

        /// <summary>
        /// Zbiór danych (tabela) przechowujący informacje o autorach książek.
        /// </summary>
        public DbSet<Author> Authors { get; set; }

        /// <summary>
        /// Zbiór danych (tabela) przechowujący relacje między użytkownikami a książkami, 
        /// reprezentujący wirtualne półki i statusy czytania.
        /// </summary>
        public DbSet<Shelf> Shelfs { get; set; }

        /// <summary>
        /// Zbiór danych (tabela) przechowujący komentarze i oceny dodane do książek przez użytkowników.
        /// </summary>
        public DbSet<Comment> Comments { get; set; }

        /// <summary>
        /// Metoda konfigurująca modele, mapowania tabel oraz relacje w bazie danych podczas jej tworzenia.
        /// Wykorzystuje Fluent API do nadpisania domyślnych zachowań bazy, m.in. zabezpieczając przed konfliktami usuwania kaskadowego.
        /// </summary>
        /// <param name="builder">Kreator modeli dostarczany przez Entity Framework Core do definiowania kształtu bazy danych.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Niezbędne wywołanie metody bazowej. Generuje wbudowane tabele Identity (role, hasła, logowania).
            base.OnModelCreating(builder);

            // --- Konfiguracja relacji za pomocą Fluent API ---

            // Relacja: Użytkownik -> Półka (1:N)
            builder.Entity<Shelf>()
                .HasOne(s => s.User)
                .WithMany(u => u.Shelfs)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Zapobiega usunięciu użytkownika, jeśli na półkach znajdują się przypisane do niego książki.

            // Relacja: Użytkownik -> Komentarz (1:N)
            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Zabezpiecza przed błędem "multiple cascade paths" na serwerze SQL.

            // Relacja: Autor -> Książka (1:N)
            builder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade); // Usunięcie autora z systemu spowoduje usunięcie wszystkich jego książek.
        }
    }
}