using Microsoft.Data.Sqlite;
using SarasaviLibrary.Models;
using System.Collections.Generic;

namespace SarasaviLibrary.DataAccess
{
    /// <summary>
    /// Handles all database operations for Books and Copies.
    /// </summary>
    public static class BookRepository
    {
        // ───────────── BOOK (TITLE) OPERATIONS ─────────────

        /// <summary>
        /// Gets the next available book number for the given classification.
        /// Format: X9999 (e.g. A0001, A0002, B0001).
        /// </summary>
        public static string GetNextBookNumber(char classification)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = @"SELECT BookNumber FROM Books 
                           WHERE Classification = @classification 
                           ORDER BY BookNumber DESC LIMIT 1";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@classification", classification.ToString());

            var result = command.ExecuteScalar();
            if (result == null)
            {
                // First book for this classification
                return $"{classification}0001";
            }

            // Extract the 4-digit number and increment
            string lastNumber = result.ToString()!;
            int sequence = int.Parse(lastNumber.Substring(1)) + 1;
            return $"{classification}{sequence:D4}";
        }

        /// <summary>
        /// Adds a new book (title) to the catalogue.
        /// </summary>
        public static void AddBook(Book book)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = @"INSERT INTO Books (BookNumber, Classification, Title, Author, ISBN, Publisher)
                           VALUES (@bookNumber, @classification, @title, @author, @isbn, @publisher)";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@bookNumber", book.BookNumber);
            command.Parameters.AddWithValue("@classification", book.Classification.ToString());
            command.Parameters.AddWithValue("@title", book.Title);
            command.Parameters.AddWithValue("@author", book.Author);
            command.Parameters.AddWithValue("@isbn", book.ISBN);
            command.Parameters.AddWithValue("@publisher", book.Publisher);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Gets all books from the catalogue.
        /// </summary>
        public static List<Book> GetAllBooks()
        {
            var books = new List<Book>();
            using var connection = DatabaseHelper.GetConnection();
            string sql = "SELECT * FROM Books ORDER BY BookNumber";

            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                books.Add(ReadBook(reader));
            }
            return books;
        }

        /// <summary>
        /// Finds a book by its BookNumber.
        /// </summary>
        public static Book? GetBookByNumber(string bookNumber)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = "SELECT * FROM Books WHERE BookNumber = @bookNumber";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@bookNumber", bookNumber);

            using var reader = command.ExecuteReader();
            return reader.Read() ? ReadBook(reader) : null;
        }

        /// <summary>
        /// Searches books by title or author (partial match).
        /// </summary>
        public static List<Book> SearchBooks(string searchTerm)
        {
            var books = new List<Book>();
            using var connection = DatabaseHelper.GetConnection();
            string sql = @"SELECT * FROM Books 
                           WHERE Title LIKE @search OR Author LIKE @search 
                           ORDER BY BookNumber";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@search", $"%{searchTerm}%");

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                books.Add(ReadBook(reader));
            }
            return books;
        }

        /// <summary>
        /// Helper to read a Book from a data reader row.
        /// </summary>
        private static Book ReadBook(SqliteDataReader reader)
        {
            return new Book
            {
                BookNumber = reader.GetString(0),
                Classification = reader.GetString(1)[0],
                Title = reader.GetString(2),
                Author = reader.IsDBNull(3) ? "" : reader.GetString(3),
                ISBN = reader.IsDBNull(4) ? "" : reader.GetString(4),
                Publisher = reader.IsDBNull(5) ? "" : reader.GetString(5)
            };
        }

        // ───────────── COPY OPERATIONS ─────────────

        /// <summary>
        /// Gets the current count of copies for a given book number.
        /// Maximum allowed is 10 copies per book.
        /// </summary>
        public static int GetCopyCount(string bookNumber)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = "SELECT COUNT(*) FROM Copies WHERE BookNumber = @bookNumber";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@bookNumber", bookNumber);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>
        /// Adds a new copy of a book.
        /// CopyNumber = BookNumber + digit (1-9, 0 for 10th copy).
        /// </summary>
        public static void AddCopy(Copy copy)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = @"INSERT INTO Copies (CopyNumber, BookNumber, Status, IsBorrowable)
                           VALUES (@copyNumber, @bookNumber, @status, @isBorrowable)";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@copyNumber", copy.CopyNumber);
            command.Parameters.AddWithValue("@bookNumber", copy.BookNumber);
            command.Parameters.AddWithValue("@status", copy.IsBorrowable ? "Available" : "Reference");
            command.Parameters.AddWithValue("@isBorrowable", copy.IsBorrowable ? 1 : 0);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Gets all copies for a specific book number.
        /// </summary>
        public static List<Copy> GetCopiesByBookNumber(string bookNumber)
        {
            var copies = new List<Copy>();
            using var connection = DatabaseHelper.GetConnection();
            string sql = "SELECT * FROM Copies WHERE BookNumber = @bookNumber ORDER BY CopyNumber";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@bookNumber", bookNumber);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                copies.Add(ReadCopy(reader));
            }
            return copies;
        }

        /// <summary>
        /// Gets a specific copy by its CopyNumber.
        /// </summary>
        public static Copy? GetCopyByNumber(string copyNumber)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = "SELECT * FROM Copies WHERE CopyNumber = @copyNumber";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@copyNumber", copyNumber);

            using var reader = command.ExecuteReader();
            return reader.Read() ? ReadCopy(reader) : null;
        }

        /// <summary>
        /// Updates the status of a copy (Available, Loaned, Reserved, Reference).
        /// </summary>
        public static void UpdateCopyStatus(string copyNumber, string status)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = "UPDATE Copies SET Status = @status WHERE CopyNumber = @copyNumber";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@copyNumber", copyNumber);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Helper to read a Copy from a data reader row.
        /// </summary>
        private static Copy ReadCopy(SqliteDataReader reader)
        {
            return new Copy
            {
                CopyNumber = reader.GetString(0),
                BookNumber = reader.GetString(1),
                Status = reader.GetString(2),
                IsBorrowable = reader.GetInt32(3) == 1
            };
        }
    }
}
