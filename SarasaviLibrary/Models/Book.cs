namespace SarasaviLibrary.Models
{
    /// <summary>
    /// Represents a book Title in the library catalogue.
    /// A Title is the class of all identical books (e.g. same ISBN).
    /// BookNumber format: X9999 (1 classification char + 4-digit sequence).
    /// </summary>
    public class Book
    {
        public string BookNumber { get; set; } = string.Empty;      // e.g. "A0001"
        public char Classification { get; set; }                     // Single letter A-Z
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
    }
}
