using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace chatbot_ai
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chatbot_data.db");
            connectionString = $"Data Source={dbPath}";
            EnsureDatabaseAndTableExist();
        }

        private void EnsureDatabaseAndTableExist()
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS tasks (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        title TEXT NOT NULL,
                        description TEXT,
                        reminder_date TEXT,
                        is_completed INTEGER DEFAULT 0,
                        created_at TEXT DEFAULT CURRENT_TIMESTAMP
                    );";

                using (var cmd = new SqliteCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddTask(string title, string description, DateTime? reminderDate)
        {
            string query = "INSERT INTO tasks (title, description, reminder_date) VALUES (@title, @desc, @reminder)";
            using (var conn = new SqliteConnection(connectionString))
            using (var cmd = new SqliteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@desc", description);
                cmd.Parameters.AddWithValue("@reminder", reminderDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<TaskItem> GetTasks(bool includeCompleted = false)
        {
            var tasks = new List<TaskItem>();
            string query = "SELECT id, title, description, reminder_date, is_completed FROM tasks";
            if (!includeCompleted) query += " WHERE is_completed = 0";

            using (var conn = new SqliteConnection(connectionString))
            using (var cmd = new SqliteCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new TaskItem
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Title = reader["title"].ToString(),
                            Description = reader["description"].ToString(),
                            ReminderDate = reader["reminder_date"] != DBNull.Value ? DateTime.Parse(reader["reminder_date"].ToString()) : (DateTime?)null,
                            IsCompleted = Convert.ToInt32(reader["is_completed"]) == 1
                        });
                    }
                }
            }
            return tasks;
        }

        public void MarkComplete(int taskId)
        {
            string query = "UPDATE tasks SET is_completed = 1 WHERE id = @id";
            using (var conn = new SqliteConnection(connectionString))
            using (var cmd = new SqliteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", taskId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteTask(int taskId)
        {
            string query = "DELETE FROM tasks WHERE id = @id";
            using (var conn = new SqliteConnection(connectionString))
            using (var cmd = new SqliteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", taskId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }

        public string DisplayText
        {
            get
            {
                string display = $"{Title} - {Description}";
                if (ReminderDate.HasValue)
                    display += $" (Reminder: {ReminderDate.Value.ToShortDateString()})";
                if (IsCompleted)
                    display += " [COMPLETED]";
                return display;
            }
        }
    }
}