namespace SarasaviLibrary.Models
{
    /// <summary>
    /// Represents a physical Copy of a book on the shelf.
    /// CopyNumber format: X99991 (BookNumber + extra digit 1-9, 0 for 10th copy).
    /// </summary>
    public class Copy
    {
        public string CopyNumber { get; set; } = string.Empty;     // e.g. "A00011"
        public string BookNumber { get; set; } = string.Empty;     // FK → Book
        public string Status { get; set; } = "Available";          // Available | Loaned | Reserved | Reference
        public bool IsBorrowable { get; set; } = true;             // false = Reference only

        /// <summary>
        /// Convenience: is this copy currently available to borrow?
        /// </summary>
        public bool CanBeBorrowed => IsBorrowable && Status == "Available";
    }
}
