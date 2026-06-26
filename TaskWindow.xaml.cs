using System;
using System.Collections.Generic;
using System.Windows;

namespace chatbot_ai
{
    public partial class TaskWindow : Window
    {
        private DatabaseHelper database;
        private List<TaskItem> tasks;
        private Action<string> logCallback;

        public TaskWindow(Action<string> logCallback)
        {
            InitializeComponent();
            this.logCallback = logCallback;
            database = new DatabaseHelper();
            LoadTasks();
        }

        private void LoadTasks()
        {
            tasks = database.GetTasks(false);
            TaskList.ItemsSource = null;
            TaskList.ItemsSource = tasks;
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleBox.Text.Trim();
            string description = DescBox.Text.Trim();

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Please enter a title for the task.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime? reminderDate = ReminderPicker.SelectedDate;
            database.AddTask(title, description, reminderDate);

            logCallback?.Invoke($"Task added: '{title}'");

            TitleBox.Clear();
            DescBox.Clear();
            ReminderPicker.SelectedDate = null;
            LoadTasks();
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a task to mark as complete.", "Selection Required",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            TaskItem selectedTask = tasks[TaskList.SelectedIndex];
            database.MarkComplete(selectedTask.Id);
            logCallback?.Invoke($"Task completed: '{selectedTask.Title}'");
            LoadTasks();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a task to delete.", "Selection Required",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            TaskItem selectedTask = tasks[TaskList.SelectedIndex];

            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete '{selectedTask.Title}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                database.DeleteTask(selectedTask.Id);
                logCallback?.Invoke($"Task deleted: '{selectedTask.Title}'");
                LoadTasks();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}