using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace BookSystem.Model
{
    /// <summary>
    /// Klasa reprezentująca autora książki.
    /// </summary>
    public class Author
    {
        /// <summary>
        /// Unikalny identyfikator autora.
        /// </summary>
        [Key]
        public int AuthorId { get; set; }

        /// <summary>
        /// Imię autora.
        /// </summary>
        [Required(ErrorMessage = "Imię autora jest wymagane.")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Nazwisko autora.
        /// </summary>
        [Required(ErrorMessage = "Nazwisko autora jest wymagane.")]
        public string? LastName { get; set; }

        /// <summary>
        /// Kolekcja książek napisanych przez autora (relacja 1:N).
        /// </summary>
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
