using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace SarasaviLibrary.DataAccess
{
    /// <summary>
    /// Manages the SQLite database connection and creates all tables on first run.
    /// The database file is stored alongside the executable for portability.
    /// </summary>
    public static class DatabaseHelper
    {
        private const string DatabaseFileName = "SarasaviLibrary.db";

        /// <summary>
        /// Returns the full path to the database file (beside the .exe).
        /// </summary>
        private static string GetDatabasePath()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(appDirectory, DatabaseFileName);
        }

        /// <summary>
        /// Gets a new open connection to the SQLite database.
        /// </summary>
        public static SqliteConnection GetConnection()
        {
            string connectionString = $"Data Source={GetDatabasePath()}";
            var connection = new SqliteConnection(connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Creates all required tables if they do not already exist.
        /// Call this once at application startup.
        /// </summary>
        public static void InitializeDatabase()
        {
            using var connection = GetConnection();

            string createTables = @"
                CREATE TABLE IF NOT EXISTS Books (
                    BookNumber      TEXT PRIMARY KEY,
                    Classification  TEXT NOT NULL,
                    Title           TEXT NOT NULL,
                    Author          TEXT,
                    ISBN            TEXT,
                    Publisher       TEXT
                );

                CREATE TABLE IF NOT EXISTS Copies (
                    CopyNumber      TEXT PRIMARY KEY,
                    BookNumber      TEXT NOT NULL,
                    Status          TEXT NOT NULL DEFAULT 'Available',
                    IsBorrowable    INTEGER NOT NULL DEFAULT 1,
                    FOREIGN KEY (BookNumber) REFERENCES Books(BookNumber)
                );

                CREATE TABLE IF NOT EXISTS Users (
                    UserNumber      TEXT PRIMARY KEY,
                    Name            TEXT NOT NULL,
                    Sex             TEXT,
                    NIC             TEXT,
                    Address         TEXT,
                    UserType        TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Loans (
                    LoanId          INTEGER PRIMARY KEY AUTOINCREMENT,
                    CopyNumber      TEXT NOT NULL,
                    UserNumber      TEXT NOT NULL,
                    LoanDate        TEXT NOT NULL,
                    DueDate         TEXT NOT NULL,
                    ReturnDate      TEXT,
                    FOREIGN KEY (CopyNumber) REFERENCES Copies(CopyNumber),
                    FOREIGN KEY (UserNumber) REFERENCES Users(UserNumber)
                );

                CREATE TABLE IF NOT EXISTS Reservations (
                    ReservationId   INTEGER PRIMARY KEY AUTOINCREMENT,
                    BookNumber      TEXT NOT NULL,
                    UserNumber      TEXT NOT NULL,
                    ReservedDate    TEXT NOT NULL,
                    FOREIGN KEY (BookNumber) REFERENCES Books(BookNumber),
                    FOREIGN KEY (UserNumber) REFERENCES Users(UserNumber)
                );
            ";

            using var command = new SqliteCommand(createTables, connection);
            command.ExecuteNonQuery();
        }
    }
}
