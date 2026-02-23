using System;

namespace SarasaviLibrary.Models
{
    /// <summary>
    /// Represents a reservation for a book Title (not a specific copy).
    /// When a copy of the reserved title is returned, the oldest reservation
    /// is fulfilled and the reserver is notified.
    /// </summary>
    public class Reservation
    {
        public int ReservationId { get; set; }
        public string BookNumber { get; set; } = string.Empty;     // FK → Book (per Title)
        public string UserNumber { get; set; } = string.Empty;     // FK → User
        public DateTime ReservedDate { get; set; }
    }
}
