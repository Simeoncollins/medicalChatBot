using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Web;

namespace medicalChatBot
{
    public static class DatabaseInitializer
    {
        private static bool _isInitialized = false;
        private static readonly object _lock = new object();

        public static void Initialize()
        {
            if (_isInitialized) return;

            lock (_lock)
            {
                if (_isInitialized) return;

                // Set DataDirectory if not set
                var dataDirectory = HttpContext.Current.Server.MapPath("~/App_Data");
                AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

                // Ensure App_Data folder exists
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }

                var dbPath = Path.Combine(dataDirectory, "chatbotDB.sqlite");

                // Create database file if it doesn't exist
                if (!File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                    CreateSchema(dbPath);
                }

                _isInitialized = true;
            }
        }

        private static void CreateSchema(string dbPath)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();

                // Enable foreign keys
                using (var command = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    command.ExecuteNonQuery();
                }

                // Create your tables here
                var createTablesSql = @"
                -- patient table
               CREATE TABLE IF NOT EXISTS Patients (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        fullname TEXT NOT NULL,
                        email TEXT NOT NULL,
                        dob TEXT NOT NULL,
                        gender TEXT NOT NULL,
                        medicalHistory TEXT NULL,
                        password TEXT NOT NULL,
                        photoUrl TEXT NOT NULL
                    );

                -- Example: conversation table
                CREATE TABLE IF NOT EXISTS Conversations (
                        conversationID INTEGER PRIMARY KEY AUTOINCREMENT,
                        patientID INTEGER NULL,
                        [text] TEXT NULL,
                        FOREIGN KEY(patientID) REFERENCES Patients(id) ON DELETE SET NULL
                    );

                -- Example: messages table
                CREATE TABLE IF NOT EXISTS Messages (
                        messageID INTEGER PRIMARY KEY AUTOINCREMENT,
                        conversationID INTEGER NOT NULL,
                        sender TEXT NOT NULL,
                        messageText TEXT NOT NULL,
                        timestamp TEXT NOT NULL,
                        FOREIGN KEY(conversationID) REFERENCES Conversations(conversationID) ON DELETE CASCADE
                    );
            ";

                using (var command = new SQLiteCommand(createTablesSql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static SQLiteConnection GetConnection()
        {
            Initialize(); // Ensure DB is initialized
            var connectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

            var builder = new SQLiteConnectionStringBuilder(connectionString)
            {
                CacheSize = 10000,
                JournalMode = SQLiteJournalModeEnum.Wal,
                PageSize = 4096,
                FailIfMissing = false,
                BusyTimeout = 2000 // Wait up to 2 seconds if database is locked
            };

            var connection = new SQLiteConnection(builder.ConnectionString);
            return connection;

        }
    }
}