using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace BookSystem.Model
{
    /// <summary>
    /// Klasa reprezentująca pojedynczą książkę w systemie.
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Unikalny identyfikator książki.
        /// </summary>
        [Key]
        public int BookId { get; set; }

        /// <summary>
        /// Tytuł książki.
        /// </summary>
        [Required(ErrorMessage = "Tytuł książki jest wymagany.")]
        public string? Title { get; set; }

        /// <summary>
        /// Identyfikator autora książki - klucz obcy.
        /// </summary>
        [Required]
        public int? AuthorId { get; set; }

        /// <summary>
        /// Numer ISBN książki.
        /// </summary>
        [Required(ErrorMessage = "Numer ISBN jest wymagany.")]
        public string? ISBN { get; set; }

        /// <summary>
        /// Data publikacji książki.
        /// </summary>
        [Required]
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// Opis lub streszczenie książki.
        /// </summary>
        public string? Description { get; set; }


        /// <summary>
        /// Nawigacja do autora przypisanego do książki.
        /// </summary>
        public virtual Author? Author { get; set; }

        /// <summary>
        /// Kolekcja wystąpień książki na półkach użytkowników.
        /// </summary>
        public virtual ICollection<Shelf> Shelfs { get; set; } = new List<Shelf>();

        /// <summary>
        /// Kolekcja komentarzy przypisanych do książki.
        /// </summary>
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
