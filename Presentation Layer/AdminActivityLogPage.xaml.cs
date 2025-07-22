using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Business_Layer;
using Data_Layer;
using Repositories.Repositories;

namespace Presentation_Layer
{

    public partial class AdminActivityLogPage : Page
    {
        private readonly ActivityLogRepository _logRepo = new ActivityLogRepository();
        private readonly StudentRepository _studentRepo = new StudentRepository();

        private List<UserItem> _users; // Fixed: Added generic type

        public class UserItem
        {
            public int UserId { get; set; }
            public string Display { get; set; }
        }

        public AdminActivityLogPage()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initialize users list
                _users = new List<UserItem>
                {
                    new UserItem { UserId = -1, Display = "Tất cả" }
                };

                // Get all students and add to users list
                var students = _studentRepo.GetAllStudents();
                if (students != null)
                {
                    _users.AddRange(students
                        .Where(s => s != null && !string.IsNullOrEmpty(s.StudentCode))
                        .Select(s => new UserItem { UserId = s.StudentId, Display = s.StudentCode }));
                }

                FilterUserComboBox.ItemsSource = _users;
                FilterUserComboBox.SelectedIndex = 0;

                // Load initial data
                FilterLogs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterLogs();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FilterUserComboBox.SelectedIndex = 0;
                FilterActionTextBox.Clear();
                FilterStartDatePicker.SelectedDate = null;
                FilterEndDatePicker.SelectedDate = null;
                FilterLogs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi reset bộ lọc: {ex.Message}");
            }
        }

        private void FilterLogs()
        {
            try
            {
                // Get all logs from repository
                var allLogs = _logRepo.GetAllActivityLogs();
                if (allLogs == null)
                {
                    LogDataGrid.ItemsSource = new List<object>();
                    return;
                }

                var query = allLogs.AsQueryable();

                // Filter by user
                if (FilterUserComboBox.SelectedItem is UserItem selectedUser && selectedUser.UserId != -1)
                {
                    query = query.Where(l => l.UserId == selectedUser.UserId);
                }

                // Filter by action
                var actionText = FilterActionTextBox.Text?.Trim() ?? "";
                if (!string.IsNullOrEmpty(actionText))
                {
                    query = query.Where(l => l.Action != null && l.Action.Contains(actionText));
                }

                // Filter by start date
                if (FilterStartDatePicker.SelectedDate.HasValue)
                {
                    var startDate = FilterStartDatePicker.SelectedDate.Value.Date;
                    query = query.Where(l => l.Timestamp.Date >= startDate);
                }

                // Filter by end date
                if (FilterEndDatePicker.SelectedDate.HasValue)
                {
                    var endDate = FilterEndDatePicker.SelectedDate.Value.Date;
                    query = query.Where(l => l.Timestamp.Date <= endDate);
                }

                // Create display list with user codes
                var displayList = query
                    .OrderByDescending(l => l.Timestamp)
                    .Select(l => new
                    {
                        l.Timestamp,
                        UserCode = GetUserCode(l.UserId),
                        Action = l.Action ?? "N/A"
                    })
                    .ToList();

                LogDataGrid.ItemsSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}");
                LogDataGrid.ItemsSource = new List<object>();
            }
        }

        private string GetUserCode(int userId)
        {
            try
            {
                var student = _studentRepo.GetStudentById(userId);
                return student?.StudentCode ?? "N/A";
            }
            catch
            {
                return "N/A";
            }
        }
    }
}