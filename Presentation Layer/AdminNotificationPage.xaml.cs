using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Business_Layer;
using Data_Layer;
using Repositories.Repositories;

namespace Presentation_Layer
{
    public partial class AdminNotificationPage : Page
    {
        private readonly NotificationRepository _notificationRepo = new();
        private readonly StudentRepository _studentRepo = new();
        private List<RecipientItem> _allRecipients;

        public AdminNotificationPage()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        // Đổi từ fields sang properties
        public class RecipientItem
        {
            public int? StudentId { get; set; }
            public string Display { get; set; }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Prepare recipient list
                _allRecipients = new List<RecipientItem>
                {
                    new RecipientItem { StudentId = null, Display = "Tất cả sinh viên" }
                };

                var students = _studentRepo.GetAllStudents();
                if (students != null)
                {
                    _allRecipients.AddRange(students
                        .Where(s => s != null && !string.IsNullOrEmpty(s.StudentCode))
                        .Select(s => new RecipientItem { StudentId = s.StudentId, Display = s.StudentCode }));
                }

                RecipientComboBox.ItemsSource = _allRecipients;
                RecipientComboBox.SelectedIndex = 0;

                // Attach TextChanged event after setting ItemsSource
                RecipientComboBox.AddHandler(TextBoxBase.TextChangedEvent,
                    new TextChangedEventHandler(RecipientComboBox_TextChanged));

                LoadHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
        }

        // Cải thiện xử lý TextChanged
        private void RecipientComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (_allRecipients == null) return;

                var comboBox = sender as ComboBox;
                if (comboBox == null) return;

                var text = comboBox.Text?.Trim().ToLower() ?? "";

                var filtered = string.IsNullOrEmpty(text)
                    ? _allRecipients
                    : _allRecipients.Where(r => r?.Display?.ToLower().Contains(text) == true).ToList();

                // Tạm thời remove event để tránh infinite loop
                comboBox.RemoveHandler(TextBoxBase.TextChangedEvent,
                    new TextChangedEventHandler(RecipientComboBox_TextChanged));

                comboBox.ItemsSource = filtered;
                comboBox.IsDropDownOpen = filtered.Any();

                // Add lại event
                comboBox.AddHandler(TextBoxBase.TextChangedEvent,
                    new TextChangedEventHandler(RecipientComboBox_TextChanged));
            }
            catch (Exception ex)
            {
                // Log error nhưng không hiển thị MessageBox để tránh spam
                System.Diagnostics.Debug.WriteLine($"ComboBox TextChanged error: {ex.Message}");
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sel = RecipientComboBox.SelectedItem as RecipientItem;
                if (sel == null)
                {
                    MessageBox.Show("Vui lòng chọn người nhận.");
                    return;
                }

                var title = TitleTextBox.Text?.Trim() ?? "";
                var content = ContentTextBox.Text?.Trim() ?? "";

                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
                {
                    MessageBox.Show("Vui lòng nhập tiêu đề và nội dung.");
                    return;
                }

                // Determine recipients
                var targets = sel.StudentId == null
                    ? _studentRepo.GetAllStudents()?.Select(s => s.StudentId) ?? Enumerable.Empty<int>()
                    : new[] { sel.StudentId.Value };

                int sentCount = 0;
                foreach (var id in targets)
                {
                    _notificationRepo.AddNotification(new Notification
                    {
                        Title = title,
                        Content = content,
                        StudentId = id,
                        CreatedDate = DateTime.Now
                    });
                    sentCount++;
                }

                MessageBox.Show($"Gửi thông báo thành công cho {sentCount} người nhận.");
                TitleTextBox.Clear();
                ContentTextBox.Clear();
                RecipientComboBox.SelectedIndex = 0;
                LoadHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi thông báo: {ex.Message}");
            }
        }

        private void LoadHistory()
        {
            try
            {
                var notifications = _notificationRepo.GetAllNotifications();
                if (notifications == null)
                {
                    HistoryDataGrid.ItemsSource = new List<object>();
                    return;
                }

                var list = notifications
                    .OrderByDescending(n => n.CreatedDate)
                    .Select(n => new
                    {
                        n.CreatedDate,
                        RecipientDisplay = GetRecipientDisplay(n.StudentId),
                        n.Title,
                        n.Content
                    })
                    .ToList();

                HistoryDataGrid.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch sử: {ex.Message}");
                HistoryDataGrid.ItemsSource = new List<object>();
            }
        }

        private string GetRecipientDisplay(int? studentId)
        {
            if (studentId == null) return "Tất cả";

            try
            {
                var student = _studentRepo.GetStudentById(studentId.Value);
                return student?.StudentCode ?? "N/A";
            }
            catch
            {
                return "N/A";
            }
        }
    }
}