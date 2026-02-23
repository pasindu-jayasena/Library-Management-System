using SarasaviLibrary.DataAccess;
using SarasaviLibrary.Forms;
using System;
using System.Windows.Forms;

namespace SarasaviLibrary
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Initializes the database and launches the main form.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Create database tables on first run
            DatabaseHelper.InitializeDatabase();

            Application.Run(new MainForm());
        }
    }
}