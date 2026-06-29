using System;
using System.IO;

namespace PaintShopIMS.Data
{
    /// <summary>
    /// Application configuration and database paths.
    /// </summary>
    public static class AppConfig
    {
        /// <summary>
        /// Gets the absolute path to the local SQLite database file.
        /// Ensures the directory exists before returning.
        /// </summary>
        public static string DbPath
        {
            get
            {
                string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PaintShopIMS");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                return Path.Combine(folder, "paintshop.db");
            }
        }

        /// <summary>
        /// Gets the SQLite connection string.
        /// </summary>
        public static string ConnectionString => $"Data Source={DbPath};";
    }
}
