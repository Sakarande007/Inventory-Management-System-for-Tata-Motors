using System.IO;
using Dapper;
using Microsoft.Data.Sqlite;

namespace PaintShopIMS.Data
{
    /// <summary>
    /// Initializes the SQLite database schema and seed data.
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Creates all tables, indexes, and default admin user if they do not exist.
        /// </summary>
        public static void Initialize()
        {
            using var connection = new SqliteConnection(AppConfig.ConnectionString);
            connection.Open();

            // Check if Users table needs migration (has IsAdmin instead of Role)
            var tableInfo = connection.Query("PRAGMA table_info(Users)");
            bool hasRole = false;
            foreach (var col in tableInfo)
            {
                if (col.name == "Role") hasRole = true;
            }

            if (!hasRole && tableInfo.Any())
            {
                // Simple migration for development: drop and recreate
                connection.Execute("DROP TABLE Users");
            }

            // InwardTransaction Migration (Add Rework columns)
            var inwardTableInfo = connection.Query("PRAGMA table_info(InwardTransaction)");
            bool hasIsRework = false;
            foreach (var col in inwardTableInfo)
            {
                if (col.name == "IsRework") hasIsRework = true;
            }

            if (!hasIsRework && inwardTableInfo.Any())
            {
                connection.Execute("ALTER TABLE InwardTransaction ADD COLUMN IsRework INTEGER DEFAULT 0;");
                connection.Execute("ALTER TABLE InwardTransaction ADD COLUMN DefectCode TEXT NULL;");
                connection.Execute("ALTER TABLE InwardTransaction ADD COLUMN ModifiedQRCode TEXT NULL;");
            }

            // OutwardTransaction Migration (Add DefectCode)
            var outwardTableInfo = connection.Query("PRAGMA table_info(OutwardTransaction)");
            bool hasOutwardDefect = false;
            foreach (var col in outwardTableInfo)
            {
                if (col.name == "DefectCode") hasOutwardDefect = true;
            }

            if (!hasOutwardDefect && outwardTableInfo.Any())
            {
                connection.Execute("ALTER TABLE OutwardTransaction ADD COLUMN DefectCode TEXT NULL;");
            }

            // Clean up standalone Rework table
            connection.Execute("DROP TABLE IF EXISTS ReworkTransaction;");

            // Create Tables
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Users (
                    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT UNIQUE NOT NULL,
                    PasswordHash TEXT NOT NULL,
                    Role TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS PartMaster (
                    PartId INTEGER PRIMARY KEY AUTOINCREMENT,
                    PartNo TEXT UNIQUE NOT NULL,
                    PartName TEXT NOT NULL,
                    Description TEXT,
                    Remark TEXT
                );

                CREATE TABLE IF NOT EXISTS CustomerMaster (
                    CustomerId INTEGER PRIMARY KEY AUTOINCREMENT,
                    CustomerName TEXT NOT NULL,
                    Description TEXT,
                    Remark TEXT
                );

                CREATE TABLE IF NOT EXISTS InwardTransaction (
                    InwardId INTEGER PRIMARY KEY AUTOINCREMENT,
                    PartNo TEXT NOT NULL,
                    SerialNumber TEXT UNIQUE NOT NULL,
                    ScanDate TEXT NOT NULL,
                    ScannedBy TEXT NOT NULL,
                    IsRework INTEGER DEFAULT 0,
                    DefectCode TEXT NULL,
                    ModifiedQRCode TEXT NULL,
                    FOREIGN KEY(PartNo) REFERENCES PartMaster(PartNo)
                );

                CREATE TABLE IF NOT EXISTS OutwardTransaction (
                    OutwardId INTEGER PRIMARY KEY AUTOINCREMENT,
                    PartNo TEXT NOT NULL,
                    SerialNumber TEXT NOT NULL,
                    ScanDate TEXT NOT NULL,
                    ScannedBy TEXT NOT NULL,
                    DefectCode TEXT NULL,
                    FOREIGN KEY(PartNo) REFERENCES PartMaster(PartNo),
                    FOREIGN KEY(SerialNumber) REFERENCES InwardTransaction(SerialNumber)
                );
            ");

            // Create Indexes
            connection.Execute("CREATE INDEX IF NOT EXISTS IDX_PartMaster_PartNo ON PartMaster(PartNo);");
            connection.Execute("CREATE INDEX IF NOT EXISTS IDX_InwardTransaction_SerialNumber ON InwardTransaction(SerialNumber);");
            connection.Execute("CREATE INDEX IF NOT EXISTS IDX_Users_Username ON Users(Username);");

            // Seed/Update Admin User
            var adminUser = connection.QuerySingleOrDefault("SELECT * FROM Users WHERE Username = 'admin'");
            string hash = BCrypt.Net.BCrypt.HashPassword("admin123");

            if (adminUser == null)
            {
                connection.Execute(
                    "INSERT INTO Users (Username, PasswordHash, Role) VALUES (@Username, @Hash, 'Admin')", 
                    new { Username = "admin", Hash = hash });
            }
            else
            {
                // Force update password to admin123 to ensure user can login
                connection.Execute("UPDATE Users SET PasswordHash = @Hash WHERE Username = 'admin'", new { Hash = hash });
            }
        }
    }
}
