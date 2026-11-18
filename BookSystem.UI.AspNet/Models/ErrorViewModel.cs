namespace BookSystem.UI.AspNet.Models
{
    /// <summary>
    /// Model widoku błędu przechowujący identyfikator żądania.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Unikalny identyfikator żądania HTTP w którym wystąpił błąd.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Określa czy identyfikator żądania powinien zostać wyświetlony w widoku.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
