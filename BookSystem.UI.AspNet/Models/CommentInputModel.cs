using System.ComponentModel.DataAnnotations;

namespace BookSystem.UI.AspNet.Models
{
    /// <summary>
    /// Dane formularza recenzji. Osobny model chroni przed nadpisaniem UserId i daty (overposting).
    /// </summary>
    public class CommentInputModel
    {
        /// <summary>Identyfikator recenzowanej książki.</summary>
        [Required]
        public int BookId { get; set; }

        /// <summary>Opcjonalny tytuł recenzji.</summary>
        [StringLength(100)]
        [Display(Name = "Tytuł recenzji")]
        public string? Title { get; set; }

        /// <summary>Treść recenzji.</summary>
        [Required(ErrorMessage = "Treść komentarza jest wymagana.")]
        [StringLength(2000)]
        [Display(Name = "Treść")]
        public string? Description { get; set; }

        /// <summary>Ocena 1-5.</summary>
        [Range(1, 5, ErrorMessage = "Ocena musi mieścić się w przedziale od 1 do 5.")]
        [Display(Name = "Ocena")]
        public int Rating { get; set; } = 5;
    }
}