using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Business_Layer;
using Repositories.Repositories;

namespace Presentation_Layer
{
    public partial class StudentNotificationPage : Page
    {
        public ObservableCollection<NotificationDisplayItem> NotificationItems { get; set; } = new();

        private readonly int _studentId;
        private readonly NotificationRepository _notificationRepo = new();

        public StudentNotificationPage(int studentId)
        {
            InitializeComponent();
            _studentId = studentId;
            DataContext = this;
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            var all = _notificationRepo.GetAllNotifications();

            // Thông báo chung (StudentId == null) hoặc gửi riêng cho sinh viên này
            var filtered = all.Where(n => n.StudentId == null || n.StudentId == _studentId);

            foreach (var n in filtered)
            {
                NotificationItems.Add(new NotificationDisplayItem
                {
                    Title = n.Title,
                    Content = n.Content,
                    CreatedDate = n.CreatedDate?.ToString("dd/MM/yyyy HH:mm") ?? ""
                });
            }
        }

        public class NotificationDisplayItem
        {
            public string Title { get; set; }
            public string Content { get; set; }
            public string CreatedDate { get; set; }
        }
    }
}
