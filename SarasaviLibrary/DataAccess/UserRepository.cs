using Microsoft.Data.Sqlite;
using SarasaviLibrary.Models;
using System;
using System.Collections.Generic;

namespace SarasaviLibrary.DataAccess
{
    /// <summary>
    /// Handles all database operations for Users (Members and Visitors).
    /// </summary>
    public static class UserRepository
    {
        /// <summary>
        /// Gets the next available user number (auto-incrementing format: U0001, U0002...).
        /// </summary>
        public static string GetNextUserNumber()
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = "SELECT UserNumber FROM Users ORDER BY UserNumber DESC LIMIT 1";

            using var command = new SqliteCommand(sql, connection);
            var result = command.ExecuteScalar();

            if (result == null)
            {
                return "U0001";
            }

            string lastNumber = result.ToString()!;
            int sequence = int.Parse(lastNumber.Substring(1)) + 1;
            return $"U{sequence:D4}";
        }

        /// <summary>
        /// Registers a new user (Member or Visitor).
        /// </summary>
        public static void AddUser(User user)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = @"INSERT INTO Users (UserNumber, Name, Sex, NIC, Address, UserType)
                           VALUES (@userNumber, @name, @sex, @nic, @address, @userType)";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@userNumber", user.UserNumber);
            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@sex", user.Sex);
            command.Parameters.AddWithValue("@nic", user.NIC);
            command.Parameters.AddWithValue("@address", user.Address);
            command.Parameters.AddWithValue("@userType", user.UserType);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Gets all registered users.
        /// </summary>
        public static List<User> GetAllUsers()
        {
            var users = new List<User>();
            using var connection = DatabaseHelper.GetConnection();
            string sql = "SELECT * FROM Users ORDER BY UserNumber";

            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                users.Add(ReadUser(reader));
            }
            return users;
        }

        /// <summary>
        /// Finds a user by UserNumber.
        /// </summary>
        public static User? GetUserByNumber(string userNumber)
        {
            using var connection = DatabaseHelper.GetConnection();
            string sql = "SELECT * FROM Users WHERE UserNumber = @userNumber";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@userNumber", userNumber);

            using var reader = command.ExecuteReader();
            return reader.Read() ? ReadUser(reader) : null;
        }

        /// <summary>
        /// Searches users by name (partial match).
        /// </summary>
        public static List<User> SearchUsers(string searchTerm)
        {
            var users = new List<User>();
            using var connection = DatabaseHelper.GetConnection();
            string sql = @"SELECT * FROM Users 
                           WHERE Name LIKE @search OR UserNumber LIKE @search 
                           ORDER BY UserNumber";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@search", $"%{searchTerm}%");

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add(ReadUser(reader));
            }
            return users;
        }

        /// <summary>
        /// Helper to read a User from a data reader row.
        /// </summary>
        private static User ReadUser(SqliteDataReader reader)
        {
            return new User
            {
                UserNumber = reader.GetString(0),
                Name = reader.GetString(1),
                Sex = reader.IsDBNull(2) ? "" : reader.GetString(2),
                NIC = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Address = reader.IsDBNull(4) ? "" : reader.GetString(4),
                UserType = reader.GetString(5)
            };
        }
    }
}
