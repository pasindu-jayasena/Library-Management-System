using Microsoft.Data.Sqlite;
using SarasaviLibrary.Models;
using System;
using System.Collections.Generic;

namespace SarasaviLibrary.DataAccess
{
    /// <summary>
    /// Handles all database operations for Reservations.
    /// Reservations are made per Title (BookNumber), not per Copy.
    /// On return, the oldest reservation for the title is fulfilled first (FIFO).
    /// </summary>
    public static class ReservationRepository
    {
        /// <summary>
        /// Creates a new reservation for a book title.
        /// </summary>
        public static void AddReservation(string bookNumber, string userNumber)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = @"INSERT INTO Reservations (BookNumber, UserNumber, ReservedDate)
                           VALUES (@bookNumber, @userNumber, @reservedDate)";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@bookNumber", bookNumber);
            command.Parameters.AddWithValue("@userNumber", userNumber);
            command.Parameters.AddWithValue("@reservedDate", DateTime.Today.ToString("yyyy-MM-dd"));
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Gets the oldest reservation for a specific book title.
        /// Returns null if no reservations exist.
        /// </summary>
        public static Reservation? GetOldestReservation(string bookNumber)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = @"SELECT * FROM Reservations 
                           WHERE BookNumber = @bookNumber 
                           ORDER BY ReservedDate ASC, ReservationId ASC 
                           LIMIT 1";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@bookNumber", bookNumber);

            using var reader = command.ExecuteReader();
            return reader.Read() ? ReadReservation(reader) : null;
        }

        /// <summary>
        /// Deletes a specific reservation by its ID (after fulfillment).
        /// </summary>
        public static void DeleteReservation(int reservationId)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = "DELETE FROM Reservations WHERE ReservationId = @reservationId";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@reservationId", reservationId);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Gets all reservations for a specific book title.
        /// </summary>
        public static List<Reservation> GetReservationsForBook(string bookNumber)
        {
            var reservations = new List<Reservation>();
            using var connection = DatabaseHelper.GetConnection();
            string sql = @"SELECT * FROM Reservations 
                           WHERE BookNumber = @bookNumber 
                           ORDER BY ReservedDate ASC";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@bookNumber", bookNumber);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                reservations.Add(ReadReservation(reader));
            }
            return reservations;
        }

        /// <summary>
        /// Gets all reservations (for display purposes).
        /// </summary>
        public static List<Reservation> GetAllReservations()
        {
            var reservations = new List<Reservation>();
            using var connection = DatabaseHelper.GetConnection();
            string sql = "SELECT * FROM Reservations ORDER BY ReservedDate ASC";

            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                reservations.Add(ReadReservation(reader));
            }
            return reservations;
        }

        /// <summary>
        /// Checks if a specific book title has any outstanding reservations.
        /// </summary>
        public static bool HasReservations(string bookNumber)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = "SELECT COUNT(*) FROM Reservations WHERE BookNumber = @bookNumber";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@bookNumber", bookNumber);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        /// <summary>
        /// Helper to read a Reservation from a data reader row.
        /// </summary>
        private static Reservation ReadReservation(SqliteDataReader reader)
        {
            return new Reservation
            {
                ReservationId = reader.GetInt32(0),
                BookNumber = reader.GetString(1),
                UserNumber = reader.GetString(2),
                ReservedDate = DateTime.Parse(reader.GetString(3))
            };
        }
    }
}
