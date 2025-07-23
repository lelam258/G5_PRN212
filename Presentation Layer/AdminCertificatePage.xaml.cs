using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Business_Layer;
using Repositories.Repositories;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Diagnostics;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Exceptions;
using Microsoft.Win32;

namespace Presentation_Layer
{
    public partial class AdminCertificatePage : Page
    {
        // Repositories
        private readonly StudentRepository _studentRepo = new();
        private readonly LifeSkillCourseRepository _courseRepo = new();
        private readonly EnrollmentRepository _enrollRepo = new();
        private readonly CertificateRepository _certRepo = new();
        private readonly NotificationRepository _notifRepo = new();
        private readonly ActivityLogRepository _logRepo = new();

        // danh sách hiển thị
        private List<EligibleItem> _eligibleList = new();

        public AdminCertificatePage()
        {
            InitializeComponent();

            // Load danh sách khóa ngay lập tức
            var courses = _courseRepo.GetAllLifeSkillCourses();
            CourseComboBox.ItemsSource = courses;
            CourseComboBox.SelectedIndex = -1;
        }

        // 1) Tải danh sách sinh viên đủ điều kiện
        private void LoadEligible_Click(object sender, RoutedEventArgs e)
        {
            if (CourseComboBox.SelectedValue is not int courseId)
            {
                MessageBox.Show("Vui lòng chọn khóa học.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lấy studentId đã hoàn thành enrollment
            var done = _enrollRepo.GetAllEnrollments()
                         .Where(en => en.CourseId == courseId && en.CompletionStatus)
                         .Select(en => en.StudentId)
                         .ToHashSet();

            // Lấy studentId đã có certificate
            var already = _certRepo.GetAllCertificates()
                             .Where(c => c.CourseId == courseId)
                             .Select(c => c.StudentId)
                             .ToHashSet();
            Debug.WriteLine($"done.Count = {done.Count}, already.Count = {already.Count}");

            // Lọc còn lại
            _eligibleList = done
                .Where(sid => !already.Contains(sid))
                .Select(sid => {
                    var s = _studentRepo.GetStudentById(sid);
                    var c = _courseRepo.GetLifeSkillCourseById(courseId);
                    return new EligibleItem
                    {
                        StudentId = sid,
                        CourseId = courseId,
                        StudentCode = s.StudentCode,
                        StudentName = s.StudentName,
                        CourseName = c.CourseName,
                        IsSelected = false
                    };
                })
                .ToList();

            EligibleDataGrid.ItemsSource = _eligibleList;
        }

        // 2) Reset filter
        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            CourseComboBox.SelectedIndex = -1;
            _eligibleList.Clear();
            EligibleDataGrid.ItemsSource = null;
        }

        // 3) Cấp từng sinh viên
        private void GenerateSingle_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is EligibleItem item)
                IssueCertificate(item);
        }

        // 4) Cấp hàng loạt
        private void GenerateSelected_Click(object sender, RoutedEventArgs e)
        {
            var toIssue = _eligibleList.Where(x => x.IsSelected).ToList();
            if (!toIssue.Any())
            {
                MessageBox.Show("Vui lòng chọn ít nhất một sinh viên.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            foreach (var item in toIssue)
                IssueCertificate(item);
        }

        // 5) Quy trình cấp certificate
        private void IssueCertificate(EligibleItem item)
        {
            // 1) Mở hộp thoại để user chọn file PDF
            var dlg = new OpenFileDialog
            {
                Title = "Chọn file PDF chứng chỉ",
                Filter = "PDF Files (*.pdf)|*.pdf",
                CheckFileExists = true,
                Multiselect = false
            };
            if (dlg.ShowDialog() != true)
                return;  // user hủy

            var sourcePath = dlg.FileName;
            // 2) Copy vào thư mục /Certificates của app
            var projectFolder = AppDomain.CurrentDomain.BaseDirectory;
            var destFolder = Path.Combine(projectFolder, "Certificates");
            Directory.CreateDirectory(destFolder);

            // Sinh mã để đặt tên (không trùng)
            var code = $"CERT-{Guid.NewGuid():N}".ToUpper();
            // Giữ nguyên tên file gốc, hoặc bạn có thể rename thành: "{item.StudentCode}_{code}.pdf"
            var destName = $"{item.StudentCode}_{code}.pdf";
            var destPath = Path.Combine(destFolder, destName);

            try
            {
                File.Copy(sourcePath, destPath, overwrite: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không copy được file:\n{ex.Message}", "Lỗi cấp chứng chỉ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3) Lưu record vào DB
            try
            {
                var cert = new Business_Layer.Certificate
                {
                    StudentId = item.StudentId,
                    CourseId = item.CourseId,
                    IssueDate = DateTime.Now,
                    CertificateCode = code,
                    FilePath = destPath
                };
                _certRepo.AddCertificate(cert);

                // (Tuỳ chọn) Gửi notification + ghi log
                _notifRepo.AddNotification(new Notification
                {
                    Title = "Chứng chỉ mới",
                    Content = $"Bạn đã nhận chứng chỉ khóa {item.CourseName}.",
                    StudentId = item.StudentId,
                    CreatedDate = DateTime.Now
                });
                _logRepo.AddActivityLog(new ActivityLog
                {
                    UserId = item.StudentId,
                    Action = $"Nhập chứng chỉ từ file và cấp mã {code}",
                    Timestamp = DateTime.Now
                });

                MessageBox.Show($"✅ Đã cấp chứng chỉ cho {item.StudentCode}.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // 4) Cập nhật lại UI
                _eligibleList.Remove(item);
                EligibleDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu chứng chỉ vào database:\n{ex.Message}", "Lỗi cấp chứng chỉ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // DTO nội bộ với INotifyPropertyChanged
        private class EligibleItem : INotifyPropertyChanged
        {
            private bool _isSelected;

            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    if (_isSelected != value)
                    {
                        _isSelected = value;
                        OnPropertyChanged(nameof(IsSelected));
                    }
                }
            }

            public int StudentId { get; set; }
            public int CourseId { get; set; }
            public string StudentCode { get; set; }
            public string StudentName { get; set; }
            public string CourseName { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}