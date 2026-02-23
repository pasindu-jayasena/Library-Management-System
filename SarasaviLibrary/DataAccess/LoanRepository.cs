using Microsoft.Data.Sqlite;
using SarasaviLibrary.Models;
using System;
using System.Collections.Generic;

namespace SarasaviLibrary.DataAccess
{
    /// <summary>
    /// Handles all database operations for Loans and Returns.
    /// Business rules:
    ///   - A member can borrow maximum 5 books at a time.
    ///   - A copy can be borrowed for 14 days (2 weeks).
    ///   - Reference copies cannot be borrowed.
    ///   - Visitors cannot borrow books.
    /// </summary>
    public static class LoanRepository
    {
        /// <summary>
        /// Counts the number of active (unreturned) loans for a user.
        /// A user cannot borrow more than 5 books at a time.
        /// </summary>
        public static int GetActiveLoanCount(string userNumber)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = @"SELECT COUNT(*) FROM Loans 
                           WHERE UserNumber = @userNumber AND ReturnDate IS NULL";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@userNumber", userNumber);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>
        /// Checks if a user has any overdue books (past due date, not returned).
        /// </summary>
        public static bool HasOverdueBooks(string userNumber)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = @"SELECT COUNT(*) FROM Loans 
                           WHERE UserNumber = @userNumber 
                             AND ReturnDate IS NULL 
                             AND DueDate < @today";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@userNumber", userNumber);
            command.Parameters.AddWithValue("@today", DateTime.Today.ToString("yyyy-MM-dd"));

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        /// <summary>
        /// Creates a new loan record. Sets DueDate to LoanDate + 14 days.
        /// Also updates the copy status to "Loaned".
        /// </summary>
        public static void CreateLoan(string copyNumber, string userNumber)
        {
            DateTime loanDate = DateTime.Today;
            DateTime dueDate = loanDate.AddDays(14);

            using var connection = DatabaseHelper.GetConnection();

            // Insert the loan record
            string sql = @"INSERT INTO Loans (CopyNumber, UserNumber, LoanDate, DueDate)
                           VALUES (@copyNumber, @userNumber, @loanDate, @dueDate)";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@copyNumber", copyNumber);
            command.Parameters.AddWithValue("@userNumber", userNumber);
            command.Parameters.AddWithValue("@loanDate", loanDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@dueDate", dueDate.ToString("yyyy-MM-dd"));
            command.ExecuteNonQuery();

            // Update copy status to "Loaned"
            string updateSql = "UPDATE Copies SET Status = 'Loaned' WHERE CopyNumber = @copyNumber";
            using var updateCmd = new SqliteCommand(updateSql, connection);
            updateCmd.Parameters.AddWithValue("@copyNumber", copyNumber);
            updateCmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Processes a book return. Sets the ReturnDate and updates copy status.
        /// Returns the BookNumber of the returned copy (for reservation checking).
        /// </summary>
        public static string? ReturnBook(string copyNumber)
        {
            using var connection = DatabaseHelper.GetConnection();

            // Set the return date on the active loan
            string sql = @"UPDATE Loans SET ReturnDate = @returnDate 
                           WHERE CopyNumber = @copyNumber AND ReturnDate IS NULL";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@returnDate", DateTime.Today.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@copyNumber", copyNumber);
            command.ExecuteNonQuery();

            // Get the BookNumber for reservation checking
            string bookSql = "SELECT BookNumber FROM Copies WHERE CopyNumber = @copyNumber";
            using var bookCmd = new SqliteCommand(bookSql, connection);
            bookCmd.Parameters.AddWithValue("@copyNumber", copyNumber);
            var bookNumber = bookCmd.ExecuteScalar()?.ToString();

            // Update copy status to "Available"
            string updateSql = "UPDATE Copies SET Status = 'Available' WHERE CopyNumber = @copyNumber";
            using var updateCmd = new SqliteCommand(updateSql, connection);
            updateCmd.Parameters.AddWithValue("@copyNumber", copyNumber);
            updateCmd.ExecuteNonQuery();

            return bookNumber;
        }

        /// <summary>
        /// Gets all active (unreturned) loans for a specific user.
        /// </summary>
        public static List<Loan> GetActiveLoansForUser(string userNumber)
        {
            var loans = new List<Loan>();
            using var connection = DatabaseHelper.GetConnection();
            string sql = @"SELECT * FROM Loans 
                           WHERE UserNumber = @userNumber AND ReturnDate IS NULL 
                           ORDER BY DueDate";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@userNumber", userNumber);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                loans.Add(ReadLoan(reader));
            }
            return loans;
        }

        /// <summary>
        /// Gets the active loan for a specific copy (if any).
        /// </summary>
        public static Loan? GetActiveLoanForCopy(string copyNumber)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = @"SELECT * FROM Loans 
                           WHERE CopyNumber = @copyNumber AND ReturnDate IS NULL";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@copyNumber", copyNumber);

            using var reader = command.ExecuteReader();
            return reader.Read() ? ReadLoan(reader) : null;
        }

        /// <summary>
        /// Gets all active loans (for display purposes).
        /// </summary>
        public static List<Loan> GetAllActiveLoans()
        {
            var loans = new List<Loan>();
            using var connection = DatabaseHelper.GetConnection();
            string sql = "SELECT * FROM Loans WHERE ReturnDate IS NULL ORDER BY DueDate";

            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                loans.Add(ReadLoan(reader));
            }
            return loans;
        }

        /// <summary>
        /// Helper to read a Loan from a data reader row.
        /// </summary>
        private static Loan ReadLoan(SqliteDataReader reader)
        {
            return new Loan
            {
                LoanId = reader.GetInt32(0),
                CopyNumber = reader.GetString(1),
                UserNumber = reader.GetString(2),
                LoanDate = DateTime.Parse(reader.GetString(3)),
                DueDate = DateTime.Parse(reader.GetString(4)),
                ReturnDate = reader.IsDBNull(5) ? null : DateTime.Parse(reader.GetString(5))
            };
        }
    }
}
