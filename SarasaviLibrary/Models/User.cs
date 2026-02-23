namespace SarasaviLibrary.Models
{
    /// <summary>
    /// Represents a registered library user (Member or Visitor).
    /// Members can borrow books; Visitors can only reference/inquire.
    /// </summary>
    public class User
    {
        public string UserNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;            // "M" or "F"
        public string NIC { get; set; } = string.Empty;            // National Identity Card
        public string Address { get; set; } = string.Empty;
        public string UserType { get; set; } = "Member";           // "Member" or "Visitor"

        /// <summary>
        /// Only Members are allowed to borrow books.
        /// </summary>
        public bool CanBorrow => UserType == "Member";
    }
}
