using BookSystem.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BookSystem.Model
{
    /// <summary>
    /// Klasa reprezentująca komentarz i ocenę użytkownika do danej książki.
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Unikalny identyfikator komentarza.
        /// </summary>
        [Key]
        public int CommentId { get; set; }

        /// <summary>
        /// Tytuł komentarza lub recenzji.
        /// </summary>
        public string? Title {  get; set; }

        /// <summary>
        /// Treść komentarza dodanego przez użytkownika.
        /// </summary>
        [Required(ErrorMessage = "Treść komentarza jest wymagana.")]
        public string? Description { get; set; }


        /// <summary>
        /// Identyfikator użytkownika, który dodał komentarz - klucz obcy.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Data i czas dodania komentarza.
        /// </summary>
        [Required]
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// Identyfikator książki, której dotyczy komentarz - klucz obcy.
        /// </summary>
        [Required]
        public int BookId { get; set; }

        /// <summary>
        /// Ocena książki w skali od 1 do 5.
        /// </summary>
        [Required]
        [Range(1, 5, ErrorMessage = "Ocena musi mieścić się w przedziale od 1 do 5.")]
        public int Rating { get; set; }



        /// <summary>
        /// Nawigacja do użytkownika będącego autorem komentarza.
        /// </summary>
        public virtual User? User { get; set; }

        /// <summary>
        /// Nawigacja do książki, której dotyczy komentarz.
        /// </summary>
        public virtual Book? Book { get; set; }
    }
}
