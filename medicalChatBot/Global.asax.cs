using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Microsoft.Data.Sqlite;

namespace medicalChatBot
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Ensure DataDirectory points to App_Data (works in dev & deployed environments)
            AppDomain.CurrentDomain.SetData("DataDirectory", Server.MapPath("~/App_Data"));

            // Create DB file and tables if they don't exist
            EnsureDb();
        }

        private void EnsureDb()
        {
            var connString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

            // Using block ensures connection is closed promptly
            using (var conn = new SqliteConnection(connString))
            {
                conn.Open();

                // Improve concurrency for web apps
                using (var pragma = new SqliteCommand("PRAGMA journal_mode=WAL;", conn))
                    pragma.ExecuteNonQuery();

                // Define your table creation SQLs here. Add more entries to create more tables.
                var tableDefinitions = new Dictionary<string, string>
                {
                    ["Patients"] = @"
                    CREATE TABLE IF NOT EXISTS Patients (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        fullname TEXT NOT NULL,
                        email TEXT NOT NULL,
                        dob TEXT NOT NULL,
                        gender TEXT NOT NULL,
                        medicalHistory TEXT NULL,
                        password TEXT NOT NULL,
                        photoUrl TEXT NOT NULL
                    );",

                                ["Conversations"] = @"
                    CREATE TABLE IF NOT EXISTS Conversations (
                        conversationID INTEGER PRIMARY KEY AUTOINCREMENT,
                        patientID INTEGER NULL,
                        [text] TEXT NULL,
                        FOREIGN KEY(patientID) REFERENCES Patients(id) ON DELETE SET NULL
                    );",

                                ["Messages"] = @"
                    CREATE TABLE IF NOT EXISTS Messages (
                        messageID INTEGER PRIMARY KEY AUTOINCREMENT,
                        conversationID INTEGER NOT NULL,
                        sender TEXT NOT NULL,
                        messageText TEXT NOT NULL,
                        timestamp TEXT NOT NULL,
                        FOREIGN KEY(conversationID) REFERENCES Conversations(conversationID) ON DELETE CASCADE
                    );"
                };


                // Wrap schema creation in a transaction so all tables are created atomically
                using (var tran = conn.BeginTransaction())
                {
                    foreach (var kv in tableDefinitions)
                    {
                        using (var cmd = new SqliteCommand(kv.Value, conn, tran))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }

                    tran.Commit();
                }

                conn.Close();
            }
        }
    }
}