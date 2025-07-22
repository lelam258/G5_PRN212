using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Business_Layer;
using Repositories.Repositories;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace Presentation_Layer
{
    public partial class AdminCertificatePage : Page
    {
        // Repositories
        private readonly StudentRepository _studentRepo = new();
        private readonly LifeSkillCourseRepository _courseRepo = new();
        private readonly EnrollmentRepository _enrollRepo = new();
        private readonly CertificateRepository _certRepo = new();

        // Danh sách hiển thị
        private List<EligibleItem> _eligibleList = new();

        public AdminCertificatePage()
        {
           
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Load tất cả khóa lên combo
            CourseComboBox.ItemsSource = _courseRepo.GetAllLifeSkillCourses();
            CourseComboBox.SelectedIndex = -1;
        }

        // Khi bấm "Tải danh sách"
        private void LoadEligible_Click(object sender, RoutedEventArgs e)
        {
            if (CourseComboBox.SelectedValue is not int courseId)
            {
                MessageBox.Show("Vui lòng chọn khóa học.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lấy các enrollment đã hoàn thành và chưa có chứng chỉ
            var done = _enrollRepo
                .GetAllEnrollments()
                .Where(en => en.CourseId == courseId && en.CompletionStatus)
                .Select(en => en.StudentId)
                .ToHashSet();

            var already = _certRepo
                .GetAllCertificates()
                .Where(c => c.CourseId == courseId)
                .Select(c => c.StudentId)
                .ToHashSet();

            _eligibleList = done
                .Where(sid => !already.Contains(sid))
                .Select(sid => {
                    var s = _studentRepo.GetStudentById(sid);
                    return new EligibleItem
                    {
                        StudentId = s.StudentId,
                        StudentCode = s.StudentCode,
                        StudentName = s.StudentName,
                        CourseName = ((LifeSkillCourse)_courseRepo.GetLifeSkillCourseById(courseId)).CourseName,
                        IsSelected = false
                    };
                })
                .ToList();

            EligibleDataGrid.ItemsSource = _eligibleList;
        }

        // Khi bấm "Cấp" riêng từng dòng
        private void GenerateSingle_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is EligibleItem item)
                GenerateCertificate(item);
        }

        // Khi bấm button "Cấp cho tất cả đã chọn"
        private void GenerateSelected_Click(object sender, RoutedEventArgs e)
        {
            var selected = _eligibleList.Where(x => x.IsSelected).ToList();
            if (!selected.Any())
            {
                MessageBox.Show("Vui lòng chọn ít nhất một sinh viên.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            foreach (var item in selected)
                GenerateCertificate(item);
        }

        // Tạo PDF và lưu CSDL
        private void GenerateCertificate(EligibleItem item)
        {
            try
            {
                // 1) Tạo folder lưu
                var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                // 2) Mã certificate
                var code = $"CERT-{Guid.NewGuid():N}".ToUpper();

                // 3) File path
                var fileName = $"{item.StudentCode}_{item.CourseName}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                var path = Path.Combine(folder, fileName);

                // 4) Tạo PDF bằng iText7
                using var writer = new PdfWriter(path);
                using var pdf = new PdfDocument(writer);
                var doc = new Document(pdf);
                doc.Add(new Paragraph("CHỨNG NHẬN ĐÃ HOÀN THÀNH KHÓA HỌC")
                            .SetFontSize(18)
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                doc.Add(new Paragraph($"Sinh viên: {item.StudentName} ({item.StudentCode})"));
                doc.Add(new Paragraph($"Khóa học: {item.CourseName}"));
                doc.Add(new Paragraph($"Ngày cấp: {DateTime.Now:dd/MM/yyyy}"));
                doc.Add(new Paragraph($"Mã chứng chỉ: {code}"));
                doc.Close();

                // 5) Lưu vào DB
                _certRepo.AddCertificate(new Certificate
                {
                    StudentId = item.StudentId,
                    CourseId = _courseRepo.GetAllLifeSkillCourses().First(c => c.CourseName == item.CourseName).CourseId,
                    IssueDate = DateTime.Now,
                    CertificateCode = code,
                    FilePath = path
                });

                MessageBox.Show($"Đã cấp chứng chỉ cho {item.StudentCode}.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // 6) Cập nhật grid (loại bỏ đã cấp)
                _eligibleList.Remove(item);
                EligibleDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cấp chứng chỉ:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // DataContext cho DataGrid
        private class EligibleItem
        {
            public bool IsSelected { get; set; }
            public int StudentId { get; set; }
            public string StudentCode { get; set; }
            public string StudentName { get; set; }
            public string CourseName { get; set; }
        }
    }
}
