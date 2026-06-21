using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    /// <summary>
    /// Manages all MySQL database operations for the CyberTasks table.
    /// Provides CRUD functionality: Add, Read, Mark Complete, and Delete tasks.
    /// </summary>
    public class DatabaseManager
    {
        private readonly string _connStr = "Server=localhost;Database=CybersecurityBotDB;Uid=root;Pwd=@Labs2026!;";

        /// <summary>
        /// Adds a new cybersecurity task to the database.
        /// </summary>
        /// <param name="title">The task title/name.</param>
        /// <param name="desc">Optional description for the task.</param>
        /// <param name="reminder">Optional DateTime for reminder.</param>
        /// <returns>True if the task was added successfully, false otherwise.</returns>
        public bool AddTask(string title, string desc, DateTime? reminder)
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("INSERT INTO CyberTasks (Title, Description, ReminderDate) VALUES (@t, @d, @r)", conn))
                    {
                        cmd.Parameters.AddWithValue("@t", title);
                        cmd.Parameters.AddWithValue("@d", desc);
                        cmd.Parameters.AddWithValue("@r", reminder ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex) { Console.WriteLine($"Error adding task: {ex.Message}"); return false; }
        }

        /// <summary>
        /// Retrieves all incomplete (active) tasks from the database.
        /// </summary>
        /// <returns>List of formatted task strings with ID, title, and optional reminder date.</returns>
        public List<string> ReadActiveTasks()
        {
            var tasks = new List<string>();
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT Id, Title, ReminderDate FROM CyberTasks WHERE IsCompleted = FALSE", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string task = $"[{reader.GetInt32("Id")}] {reader.GetString("Title")}";
                            if (!reader.IsDBNull(reader.GetOrdinal("ReminderDate")))
                                task += $" (Due: {reader.GetDateTime("ReminderDate").ToShortDateString()})";
                            tasks.Add(task);
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error reading tasks: {ex.Message}"); }
            return tasks;
        }

        /// <summary>
        /// Marks a task as completed in the database.
        /// </summary>
        /// <param name="id">The task ID to mark as complete.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool MarkComplete(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("UPDATE CyberTasks SET IsCompleted = TRUE WHERE Id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex) { Console.WriteLine($"Error completing task: {ex.Message}"); return false; }
        }

        /// <summary>
        /// Deletes a task from the database by its ID.
        /// </summary>
        /// <param name="id">The task ID to delete.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool DeleteTask(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("DELETE FROM CyberTasks WHERE Id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex) { Console.WriteLine($"Error deleting task: {ex.Message}"); return false; }
        }
    }
}
