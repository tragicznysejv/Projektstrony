using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BookSystem.Model
{
    /// <summary>
    /// Klasa reprezentująca użytkownika w systemie wirtualnej biblioteczki.
    /// Przechowuje dane personalne oraz logowania.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unikalny identyfikator użytkownika.
        /// </summary>
        [Key]
        public int UserId { get; set; }

        /// <summary>
        /// Imię użytkownika.
        /// </summary>
        [Required(ErrorMessage ="Imie jest wymagane")]
        public string? FirstName { get; set; }


        /// <summary>
        /// Nazwisko użytkownika.
        /// </summary>
        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public string? LastName { get; set; }

        /// <summary>
        /// Nazwa użytkownika.
        /// </summary>
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana")]
        public string? UserName { get; set; }


        /// <summary>
        /// Adres e-mail użytkownika.
        /// </summary>
        [Required(ErrorMessage = "Adres e-mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu e-mail.")]
        public string? Email { get; set; }


        /// <summary>
        /// Hasło użytkownika.
        /// </summary>
        [Required(ErrorMessage = "Hasło jest wymagane")]
        public string? Password { get; set; }


        /// <summary>
        /// Kolekcja przypisanych do użytkownika półek (relacja 1:N).
        /// </summary>
        public virtual ICollection<Shelf> Shelfs { get; set; } = new List<Shelf>();
        /// <summary>
        /// Kolekcja komentarzy dodanych przez użytkownika (relacja 1:N).
        /// </summary>
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
    
