using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BookSystem.Model
{
    /// <summary>
    /// Klasa reprezentująca użytkownika w systemie wirtualnej biblioteczki.
    /// Przechowuje dane personalne oraz logowania.
    /// </summary>
    public class User :IdentityUser
    {

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
        /// Kolekcja przypisanych do użytkownika półek (relacja 1:N).
        /// </summary>
        public virtual ICollection<Shelf> Shelfs { get; set; } = new List<Shelf>();
        /// <summary>
        /// Kolekcja komentarzy dodanych przez użytkownika (relacja 1:N).
        /// </summary>
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
    
