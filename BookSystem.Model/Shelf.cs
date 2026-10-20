using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace BookSystem.Model
{
    /// <summary>
    /// Status przypisany do książki na półce.
    /// </summary>
    public enum ShelfStatus
    {
        /// <summary>
        /// Książka w planach do przeczytania.
        /// </summary>
        PlanToRead,
        /// <summary>
        /// Książka jest aktualnie czytana.
        /// </summary>
        Reading,
        /// <summary>
        /// Książka została już przeczytana.
        /// </summary>
        Read
    }
    /// <summary>
    /// Klasa łącząca użytkownika z książką, określająca jej status oraz ulubione.
    /// </summary>
    public class Shelf
    {
        /// <summary>
        /// Unikalny identyfikator wpisu na półce.
        /// </summary>
        [Key]
        public int ShelfId { get; set; }

        /// <summary>
        /// Identyfikator właściciela półki - klucz obcy.
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Identyfikator książki przypisanej do półki - klucz obcy.
        /// </summary>
        [Required]
        public int BookId { get; set; }

        /// <summary>
        /// Status czytania książki.
        /// </summary>
        [Required]
        public ShelfStatus Status { get; set; }

        /// <summary>
        /// Flaga określająca czy książka znajduje się w ulubionych.
        /// </summary>
        public bool IsFavourite { get; set; }

        /// <summary>
        /// Nawigacja do użytkownika posiadającego półkę.
        /// </summary>
        public virtual User? User { get; set; }

        /// <summary>
        /// Nawigacja do książki znajdującej się na półce.
        /// </summary>
        public virtual Book? Book { get; set; }
    }
}
