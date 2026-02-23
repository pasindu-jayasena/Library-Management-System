using System;

namespace SarasaviLibrary.Models
{
    /// <summary>
    /// Represents a loan record — one copy lent to one member.
    /// Loan period is 2 weeks (14 days).
    /// A member can have a maximum of 5 active loans at a time.
    /// </summary>
    public class Loan
    {
        public int LoanId { get; set; }
        public string CopyNumber { get; set; } = string.Empty;     // FK → Copy
        public string UserNumber { get; set; } = string.Empty;     // FK → User
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }                       // LoanDate + 14 days
        public DateTime? ReturnDate { get; set; }                   // null = still on loan

        /// <summary>
        /// Is this loan currently active (book not yet returned)?
        /// </summary>
        public bool IsActive => ReturnDate == null;

        /// <summary>
        /// Is this loan overdue?
        /// </summary>
        public bool IsOverdue => IsActive && DateTime.Today > DueDate;
    }
}
